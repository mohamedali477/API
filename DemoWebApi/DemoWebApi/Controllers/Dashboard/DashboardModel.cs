using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoWebApi.Controllers.Dashboard
{
    public class DashboardModel
    {
    }

    public class SchoolAttendanceCount
    {
        public SchoolAttendanceCountId _id { get; set; }
        public long count { get; set; }
    }

    public class SchoolAttendanceCountId
    {
        public long attendanceDate { get; set; }
        public int attendanceStatusId { get; set; }
    }

    public class classAttendanceCount
    {
        public classAttendanceCountId _id { get; set; }
        public long count { get; set; }
    }

    public class classAttendanceCountId
    {
        public long attendanceDate { get; set; }
        public object classId { get; set; }
        public int attendanceStatusId { get; set; }
    }

    public class StudentRatingCount
    {
        public StudentRatingCountId _id { get; set; }
        public long count { get; set; }
    }

    public class StudentRatingCountId
    {
        public long ratingDate { get; set; }
        public int ratingValue { get; set; }
    }
}