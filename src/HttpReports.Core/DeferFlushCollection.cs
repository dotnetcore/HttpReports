using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace HttpReports
{
    /// <summary>
    /// 异步回调的延时冲洗集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncCallbackDeferFlushCollection<T> : DeferFlushCollection<T>
    {
        public Func<IEnumerable<T>, CancellationToken, Task> Callback { get; }

        public AsyncCallbackDeferFlushCollection(Func<IEnumerable<T>, CancellationToken, Task> callback, int flushThreshold, TimeSpan flushInterval) : base(flushThreshold, flushInterval)
        {
            Callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        protected override async Task FlushAsync(IEnumerable<T> list, CancellationToken token) => await Callback(list, token).ConfigureAwait(false);
    }

    /// <summary>
    /// 延时冲洗集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DeferFlushCollection<T> : IDisposable
    {
        /// <summary>
        /// 自动冲洗TokenSource
        /// </summary>
        private readonly CancellationTokenSource _autoFlushCTS = null;

        private ConcurrentBag<T> _list = new ConcurrentBag<T>();

        /// <summary>
        /// 同步锁对象
        /// </summary>
        private readonly object _syncRoot = new object();

        /// <summary>
        /// 内部计数，避免直接访问ConcurrentBag<T>.Count
        /// </summary>
        private int _count = 0;

        /// <summary>
        /// 最后Flush的时间
        /// </summary>
        private DateTime _lastFlushTime;

        /// <summary>
        /// 定时触发间隔
        /// </summary>
        public TimeSpan FlushInterval { get; }

        /// <summary>
        /// 触发阈值
        /// </summary>
        public int FlushThreshold { get; }

        /// <summary>
        /// 元素总数
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// 延时冲洗集合
        /// </summary>
        /// <param name="flushThreshold">触发阈值</param>
        /// <param name="flushInterval">定时触发间隔</param>
        public DeferFlushCollection(int flushThreshold, TimeSpan flushInterval)
        {
            if (flushThreshold < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(flushThreshold));
            }

            if (flushInterval.TotalMilliseconds < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(flushInterval));
            }

            FlushThreshold = flushThreshold;
            FlushInterval = flushInterval;

            _autoFlushCTS = new CancellationTokenSource();
            Task.Run(AutoFlushAsync, _autoFlushCTS.Token).ConfigureAwait(false);
        }

        public void Push(T item)
        {
            lock (_syncRoot)
            {
                _list.Add(item);
                if (++_count > FlushThreshold)
                {
                    Debug.WriteLine("Out of FlushThreshold");

                    InternalFlush(SwitchBag());
                }
            }
        }

        private ConcurrentBag<T> SwitchBag()
        {
            Debug.WriteLine("SwitchBag");

            lock (_syncRoot)
            {
                _lastFlushTime = DateTime.Now;
                _count = 1;
                return Interlocked.Exchange(ref _list, new ConcurrentBag<T>());
            }
        }

        public void Flush()
        {
            Debug.WriteLine("Manual Flush");

            InternalFlush(SwitchBag());
        }

        private void InternalFlush(ConcurrentBag<T> list)
        {
            if (list.Count > 0)
            {
                Task.Run(async () =>
                {
                    Debug.WriteLine($"Flush: {list.Count}");

                    await FlushAsync(list, _autoFlushCTS.Token).ConfigureAwait(false);
                }, _autoFlushCTS.Token).ConfigureAwait(false);
            }
        }

        protected abstract Task FlushAsync(IEnumerable<T> list, CancellationToken token);

        private async Task AutoFlushAsync()
        {
            var token = _autoFlushCTS.Token;
            _lastFlushTime = DateTime.Now;

            while (!token.IsCancellationRequested)
            {
                var interval = DateTime.Now - _lastFlushTime;
                if (interval < FlushInterval)
                {
                    await Task.Delay(FlushInterval - interval, token).ConfigureAwait(false);
                    continue;
                }

                try
                {
                    Debug.WriteLine("Auto Flush");

                    var list = SwitchBag();

                    if (list.Count > 0)
                    {
                        Debug.WriteLine($"Auto Flush: {list.Count}");

                        await FlushAsync(list, _autoFlushCTS.Token).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    if (!token.IsCancellationRequested)
                    {
                        throw;
                    }
                }

                await Task.Delay(FlushInterval, token).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            _autoFlushCTS.Cancel(true);
            _autoFlushCTS.Dispose();
        }
    }
}