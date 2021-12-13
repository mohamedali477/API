using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DemoWebApi.Models;

namespace DemoWebApi.Controllers.Guardian
{
        #region----- view_Guardians----------

        public class GuardianModel
        {
            public object guardianId { get; set; }
            public int relationId { get; set; }
            public GuardianBasicInfo basicInfo { get; set; }
            public GuardianContact contactInfo { get; set; }
            public Credential credentialInfo { get; set; }
            public object _id { get; set; }
            public int status { get; set; }
        }

        public class GuardianBasicInfo
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public long? dob { get; set; }
            public int genderId { get; set; }
            public int religionId { get; set; }
            public int castId { get; set; }
        }
        public class GuardianContact
        {
            public string contactNo { get; set; }
            public string altContactNo { get; set; }
            public string faxNo { get; set; }
            public string email { get; set; }
        }


        #endregion

        #region--- Guardian Search -------
        public class GuardianSearch
        {
            public GuardianSearchParameters SearchParameters;
            public Paging paging;
            public Sorting sorting;
        }
        public class GuardianSearchParameters
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public long? dob { get; set; }
            public int? genderId { get; set; }
            public int? castId { get; set; }
            public int? religionId { get; set; }
            public int? relationId { get; set; }
    }

        #endregion
}