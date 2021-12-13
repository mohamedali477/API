using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoWebApi.Controllers.SignIn
{
    public class SignInModel
    {
        public string userType { get; set; }
        public string loginId { get; set; }
        public string password { get; set; }
    }
}