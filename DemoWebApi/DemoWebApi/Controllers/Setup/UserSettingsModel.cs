using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace DemoWebApi.Controllers.Setup
{
    public class UserSettingsModel
    {
        public object _id { get; set; }
        public bool showErrorMessages { get; set; }
        public bool outlineFields { get; set; }
        public bool expendMultiple { get; set; }
        public bool multipleMenuOpen { get; set; }
        public bool formAnimation { get; set; }
        public string lineChartBgColor { get; set; }
        public int lineChartPointRadius { get; set; }
        public string websiteMainColor { get; set; }
        public string websiteBgImage { get; set; }
    }
}