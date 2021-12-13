using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DemoWebApi.Models;

namespace DemoWebApi.Controllers.Exam
{
    public class ExamTypeModel
    {
        public object _id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int status { get; set; }
    }

    public class ExamTypeSearch
    {
        public ExamTypeSearchParameters SearchParameters;
        public Paging paging;
        public Sorting sorting;
    }

    public class ExamTypeSearchParameters
    {
        public string code { get; set; }
        public string name { get; set; }
    }
    public class ExamTypeDdlModel
    {
        public object _id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int status { get; set; }
    }
}