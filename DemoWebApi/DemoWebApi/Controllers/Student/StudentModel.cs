using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DemoWebApi.Models;
using DemoWebApi.Models.StudentGuardian;

namespace DemoWebApi.Controllers.Student
{
    #region----- view_Students----------
    public class StudentModel
    {
        public object studentId { get; set; }
        public StudentBasicInfo basicInfo { get; set; }
        public StudentContact contactInfo { get; set; }
        public Credential credentialInfo { get; set; }
        public AcademicInfo academicInfo { get; set; }
        public object _id { get; set; }
        public int status { get; set; }
    }
    public class StudentBasicInfo
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public long? dob { get; set; }
        public int genderId { get; set; }
        public int religionId { get; set; }
        public int castId { get; set; }
    }
    public class StudentContact
    {
        public string contactNo { get; set; }
        public string altContactNo { get; set; }
        public string faxNo { get; set; }
        public string email { get; set; }
    }


    #endregion

    #region--- Student Search -------
    public class StudentSearch
    {
        public StudentSearchParameters SearchParameters;
        public Paging paging;
        public Sorting sorting;
    }
    public class StudentSearchParameters
    {
        public object classId { get; set; }
        public int? sectionId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public long? dob { get; set; }
        public int? genderId { get; set; }
        public int? castId { get; set; }
        public int? religionId { get; set; }
    }

    #endregion
}