using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DemoWebApi.Models;

namespace DemoWebApi.Controllers.Subject
{
    public class SubjectModel
    {
        public object _id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int status { get; set; }
    }

    public class SubjectSearch
    {
        public SubjectSearchParameters SearchParameters;
        public Paging paging;
        public Sorting sorting;
    }

    public class SubjectSearchParameters
    {
        public string code { get; set; }
        public string name { get; set; }
    }
    public class SubjectDdlModel
    {
        public object _id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int status { get; set; }
    }
}