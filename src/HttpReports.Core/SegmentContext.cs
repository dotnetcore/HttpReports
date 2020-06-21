using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HttpReports.Core
{
    public class SegmentContext:ISegmentContext
    {
        private ConcurrentDictionary<string,Segment> _concurrent;

        public SegmentContext()
        {
            _concurrent = new ConcurrentDictionary<string, Segment>(); 
        }

        public Segment Get(string Id)
        {
            Segment entity;

            _concurrent.TryGetValue(Id,out entity);

            return entity;
        }

        public bool Push(string Id, Segment segment)
        {
            if (!_concurrent.ContainsKey(Id))
            {
               return _concurrent.TryAdd(Id, segment);
            }

            return false; 
        }
    }

    public class Segment
    {
        public DateTime CreateTime { get; set; }

        public object Value { get; set; }

        public Activity activity { get; set; }


    }

}
