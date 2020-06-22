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
        private ConcurrentDictionary<string,List<Segment>> _concurrent;

        public SegmentContext()
        {
            _concurrent = new ConcurrentDictionary<string,List<Segment>>();
        }

        public List<Segment> GetSegments(string Id)
        {
            List<Segment> segments = new List<Segment>();

            _concurrent.TryGetValue(Id,out segments);

            return segments;
        }

        public bool Push(string Id, Segment segment)
        {
            if (_concurrent.ContainsKey(Id))
            {
                List<Segment> segments;

                _concurrent.TryGetValue(Id,out segments);

                _concurrent.TryRemove(Id,out _);

                segments.Add(segment);

                return _concurrent.TryAdd(Id, segments); 

            }
            else
            {
                return _concurrent.TryAdd(Id,new List<Segment> { segment });
            }  
        }

        public bool Release(string Id) => _concurrent.TryRemove(Id, out _); 
         
    }

    public class Segment
    {
        public DateTime CreateTime { get; set; }

        public object Value { get; set; }

        public Activity activity { get; set; } 

    }

}
