using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DemoWebApi.Models;
using MongoDB.Bson;

namespace DemoWebApi.Controllers.Transport
{
    #region-------- Route Info -------
    public class RouteBasicInfo
    {
        public string routeName { get; set; }
        public double? routeLength { get; set; }
        public string description { get; set; }
    }
    
    public class Stoppage
    {
        public string stoppageName { get; set; }
        public double? distance { get; set; }
        public int? fairs { get; set; }
        public string description { get; set; }
        public object _id { get; set; }
        public int status { get; set; }
    }

    public class RouteModel
    {
        public RouteBasicInfo basicInfo { get; set; }
        public List<Stoppage> stoppage { get; set; }
        public object _id { get; set; }
        public int status { get; set; }
    }

#endregion

    #region------ Route Search ----------
    public class RouteSearch
    {
        public RouteBasicInfo SearchParameters;
        public Paging paging;
        public Sorting sorting;
    }

    #endregion
    
}