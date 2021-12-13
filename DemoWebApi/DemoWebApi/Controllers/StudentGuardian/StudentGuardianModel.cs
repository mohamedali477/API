using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using DemoWebApi.Controllers.Student;
using MongoDB.Bson;

namespace DemoWebApi.Models.StudentGuardian
{
    public class Guardian
    {
        public BasicInfo basicInfo { get; set; }
        public Contact contactInfo { get; set; }
        public Credential credentialInfo { get; set; }
        public Occupation occupationInfo { get; set; }
        public GovernmentIds governmentIds { get; set; }
        public int relationId { get; set; }
        public object _id { get; set; }
        public int status { get; set; }
    }
    
    public class Student
    {
        public BasicInfo basicInfo { get; set; }
        public Contact contactInfo { get; set; }
        public Credential credentialInfo { get; set; }
        public Transport transportInfo { get; set; }
        public AcademicInfo academicInfo { get; set; }
        public MedicalInfo medicalInfo { get; set; }
        public GovernmentIds governmentIds { get; set; }
        public long? feePaidUpto { get; set; }
        public object _id { get; set; }
        public int status { get; set; }
    }

    public class AcademicInfo
    {
        public string registrationNo { get; set; }
        public int rollNo { get; set; }
        public object classId { get; set; }
        public int sectionId { get; set; }
    }

    public class StudentGuardianModel
    {
        public List<Student> students { get; set; }
        public List<Guardian> guardians { get; set; }
        public Address address { get; set; }
        public object _id { get; set; }
        public int status { get; set; }
    }
}