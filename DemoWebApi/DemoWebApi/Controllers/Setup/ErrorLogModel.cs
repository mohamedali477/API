using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoWebApi.Controllers.Setup
{
    public class ErrorLogModel
    {
        public string message { get; set; }
        public string functionName { get; set; }
        public object error { get; set; }
        public object schoolId { get; set; }
        public object schoolBranchId { get; set; }
        public object userId { get; set; }
        public object userRoleId { get; set; }

    }
}