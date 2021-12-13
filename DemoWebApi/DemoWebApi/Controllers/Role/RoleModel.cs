using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DemoWebApi.Models;

namespace DemoWebApi.Controllers.Role
{
    public class RoleModel
    {
        public object _id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<Entitlement> entitlements { get; set; }
        public int status { get; set; }
    }

    public class Entitlement
    {
        public  int pageId { get; set; }
        public int accessTypeId { get; set; }
        public List<SubPageDetails> subPage { get; set; }
    }

    public class SubPageDetails
    {
        public int subPageId { get; set; }
        public int accessTypeId { get; set; }
    }

    public class RoleSearch
    {
        public RoleSearchParameters SearchParameters;
        public Paging paging;
        public Sorting sorting;
    }

    public class RoleSearchParameters
    {
        public string name { get; set; }
    }
    public class RoleDdlModel
    {
        public object _id { get; set; }
        public string name { get; set; }
        public int status { get; set; }
        public bool isHidden { get; set; }
        public List<Entitlement> entitlements { get; set; }
    }
}