using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DemoWebApi.Models;

namespace DemoWebApi.Controllers.Exam
{
    public class ExamModel
    {
        public object _id { get; set; }
        public int academicSessionId { get; set; }
        public object examTypeId { get; set; }
        public object classId { get; set; }
        public int?[] classSection { get; set; }
        public List<ExamSubject> subject { get; set; }
        public int status { get; set; }
        
    }

    public class ExamSubject
    {
        public long examDate { get; set; }
        public string room { get; set; }
        public object teacherId { get; set; }
        public object subjectId { get; set; }
        public string fromTime { get; set; }
        public string toTime { get; set; }
        public string comments { get; set; }
        public int maxMarks { get; set; }
        public int passingMarks { get; set; }
        public List<ExamSection> sections { get; set; }
    }

    public class sectionResultModel
    {
        public ExamSection sectionResult { get; set; }
        public object subjectId { get; set; }
        public object examId { get; set; }
    }

    public class ExamSection
    {
        public int sectionId { get; set; }
        public List<ExamStudents> students { get; set; }
    }

    public class ExamStudents
    {
        public object studentId { get; set; }
        public double? marksGained { get; set; }
    }

    public class ExamSearch
    {
        public ExamSearchParameters SearchParameters;
        public Paging paging;
        public Sorting sorting;
    }

    public class ExamSearchParameters
    {
        public int? academicSessionId { get; set; }
        public object examTypeId { get; set; }
        public object classId { get; set; }
    }

    public class ViewSchoolExamModel
    {
        public object _id { get; set; }
        public object classId { get; set; }
        public object subjectId { get; set; }
        public long examDate { get; set; }
        public string room { get; set; }
        public object teacherId { get; set; }
        public string fromTime { get; set; }
        public string toTime { get; set; }
        public string comments { get; set; }
        public int status { get; set; }
    }
}