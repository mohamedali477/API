using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DemoWebApi.Controllers.Attendance
{
    public class StudentAttendanceSearchModel
    {
        public long? attendanceDate { get; set; }
        public object classId { get; set; }
        public int? sectionId { get; set; }
    }

    public class StudentAttendanceModel
    {
        public object _id { get; set; }
        public long attendanceDate { get; set; }

        public List<AttendanceClass> classes { get; set; }
        public int status { get; set; }
    }

    public class AttendanceClass
    {
        public object classId { get; set; }
        public int sectionId { get; set; }
        public List<StudentAttendanceStatus> students { get; set; }

    }

    public class StudentAttendanceStatus
    {
        public object studentId { get; set; }
        public int attendanceStatusId { get; set; }
    }

    public class StudentManualAttendanceProjection
    {
        public object _id { get; set; }
        public object studentId { get; set; }
        public int attendanceStatusId { get; set; }
        public object classId { get; set; }
        public int sectionId { get; set; }
        public int status { get; set; }
    }
}