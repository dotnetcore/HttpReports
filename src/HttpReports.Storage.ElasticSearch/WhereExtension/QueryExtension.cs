using HttpReports.Storage.FilterOptions;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using TimeUnit = HttpReports.Storage.FilterOptions.TimeUnit;

namespace HttpReports.Storage.ElasticSearch.WhereExtension
{
    public static class QueryExtension
    { 
        public static SearchDescriptor<T> BuildWhere<T>(this SearchDescriptor<T> search, RequestInfoFilterOption option) where T:class
        {   
            return search; 
        }

        public static SearchDescriptor<T> SizeWhere<T>(this SearchDescriptor<T> search, RequestInfoFilterOption option) where T : class
        {
            if (option.Take > 0)
            { 
                search = search.Size(option.Take); 
            }

            return search;
        }


        public static SearchDescriptor<T> StartWhere<T>(this SearchDescriptor<T> search, RequestInfoFilterOption option) where T : class
        { 
            if (option.StartTime.HasValue)
            {
                search = search.Query(x => x.DateRange(c => c.Field("createTime").GreaterThanOrEquals(option.StartTime.Value)));
            }  

            return search;
        }

        public static SearchDescriptor<T> EndWhere<T>(this SearchDescriptor<T> search, RequestInfoFilterOption option) where T : class
        {
            if (option.StartTime.HasValue)
            {
                search = search.Query(x => x.DateRange(c => c.Field("createTime").GreaterThanOrEquals(option.StartTime.Value)));
            }

            return search;
        }

        public static QueryContainerDescriptor<T> HasDateWhere<T>(this QueryContainerDescriptor<T> container, DateTime? start, DateTime? end) where T:class
        {
            if (start == null && end == null)
            {
                return container;
            } 

            container.DateRange(c => { 

                c.Field("createTime");

                if (start.HasValue)
                {
                    c.GreaterThanOrEquals(start.Value); 
                }

                if (end.HasValue)
                {
                    c.LessThan(end.Value);
                }

                return c; 

            }); 

            return container; 
        }


        public static DateHistogramAggregationDescriptor<T> AutoFormatTime<T>(this DateHistogramAggregationDescriptor<T> descriptor, FilterOptions.TimeUnit type) where T:class
        {
            switch (type)
            {
                case TimeUnit.Hour:
                    descriptor.CalendarInterval(DateInterval.Hour).Format("HH");
                    break;

                case TimeUnit.Month:
                    descriptor.CalendarInterval(DateInterval.Month).Format("MM");
                    break;

                case TimeUnit.Year:
                    descriptor.CalendarInterval(DateInterval.Year).Format("YYYY");
                    break;

                case TimeUnit.Day:
                default:
                    descriptor.CalendarInterval(DateInterval.Day).Format("dd");
                    break;
            } 

            return descriptor;
        
        } 
        
    }  
}
