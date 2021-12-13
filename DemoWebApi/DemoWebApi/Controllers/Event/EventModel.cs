using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DemoWebApi.Models;

namespace DemoWebApi.Controllers.Event
{
    public class EventModel
    {
        public object _id { get; set; }
        public long fromDate { get; set; }
        public long toDate { get; set; }
        public string eventName { get; set; }
        public string description { get; set; }
        public int status { get; set; }
    }

    public class EventSearch
    {
        public EventSearchParameters SearchParameters;
        public Paging paging;
        public Sorting sorting;
    }

    public class EventSearchParameters
    {
        public long? fromDate { get; set; }
        public long? toDate { get; set; }
        public string eventName { get; set; }
    }
}