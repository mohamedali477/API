using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoWebApi.Controllers.School
{
    public class BranchClassModel
    {
            public object _id { get; set; }
            public int status { get; set; }
            public ClassBasicInfo basicInfo { get; set; }
            public List<ClassSubjectInfo> subject { get; set; }
            public List<SectionConfig> sectionConfig { get; set; }
    }

    public class ClassBasicInfo
    {
        public string className { get; set; }
        public int classFee { get; set; }
        public bool isSemesterSystem { get; set; }
        public int?[] classSection { get; set; }
    }

    public class ClassSubjectInfo
    {
        public object subjectId { get; set; }
        public int sectionId { get; set; }
        public int status { get; set; }
        public object teacherId { get; set; }
        public int maxMarks { get; set; }
        public int passingMarks { get; set; }
        public List<int> teachingDays { get; set; }
        public string fromTime { get; set; }
        public string toTime { get; set; }
    }
    public class SectionConfig
    {
        public int sectionId { get; set; }
        public int? fee { get; set; }
        public string description { get; set; }
    }

    public class ClassDdlMode
    {
        public object _id { get; set; }
        public int status { get; set; }
        public string name { get; set; }
        public int?[] classSection { get; set; }
    }

    public class ClassSubjectDdlModel
    {
        public object _id { get; set; }
        public int status { get; set; }
        public string name { get; set; }
        public int?[] classSection { get; set; }
        public List<classSubjectDdlModel> subject { get; set; }
    }

    public class classSubjectDdlModel
    {
        public object _id { get; set; }
        public int sectionId { get; set; }
        public int maxMarks { get; set; }
        public int passingMarks { get; set; }
        public int status { get; set; }
        public List<int> teachingDays { get; set; }
        public string fromTime { get; set; }
        public string toTime { get; set; }
        public object teacherId { get; set; }
    }

    public class CJClassSectionStudentModel
    {
        public CJClassSectionStudentId _id { get; set; }
        public List<object> students { get; set; }
    }

    public class CJClassSectionStudentId
    {
        public object classId { get; set; }
        public int sectionId { get; set; }
    }
}