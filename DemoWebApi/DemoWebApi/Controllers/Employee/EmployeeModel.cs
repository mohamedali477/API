using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DemoWebApi.Controllers;
using DemoWebApi.Models;

namespace DemoWebApi.Models.Employee
{
    public class EmployeeModel
    {
        public BasicInfo basicInfo { get; set; }
        public Contact contactInfo { get; set; }
        public Credential credentialInfo { get; set; }
        public Transport transportInfo { get; set; }
        public MedicalInfo medicalInfo { get; set; }
        public GovernmentIds governmentIds { get; set; }
        public Address addressInfo { get; set; }
        public object roles { get; set; }
        public object _id { get; set; }
        public int status { get; set; }
    }

    #region--- Employee Search -------
    public class EmployeeSearch
    {
        public EmployeeSearchParameters SearchParameters;
        public Paging paging;
        public Sorting sorting;
    }
    public class EmployeeSearchParameters
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public long? dob { get; set; }
        public int? genderId { get; set; }
        public int? castId { get; set; }
        public int? religionId { get; set; }
    }

    #endregion

    public class EmployeeDdl
    {
        public object roleId { get; set; }
        public string name { get; set; }
        public object _id { get; set; }
        public int status { get; set; }
    }
}