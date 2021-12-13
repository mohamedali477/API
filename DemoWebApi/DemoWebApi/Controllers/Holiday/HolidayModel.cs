using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DemoWebApi.Models;

namespace DemoWebApi.Controllers.Holiday
{
    public class HolidayModel
    {
        public object _id { get; set; }
        public long holidayDate { get; set; }
        public string holidayName { get; set; }
        public string description { get; set; }
        public int status { get; set; }
    }

    public class HolidaySearch
    {
        public HolidaySearchParameters SearchParameters;
        public Paging paging;
        public Sorting sorting;
    }

    public class HolidaySearchParameters
    {
        public long? fromDate { get; set; }
        public long? toDate { get; set; }
        public string holidayName { get; set; }
    }
}