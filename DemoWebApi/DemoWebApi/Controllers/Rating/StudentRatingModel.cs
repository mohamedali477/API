using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace DemoWebApi.Controllers.Rating
{
    public class StudentRatingSearchModel
    {
        public long? ratingDate { get; set; }
        public object classId { get; set; }
        public object subjectId { get; set; }
        public int? sectionId { get; set; }
    }

    public class StudentWholeRatingSearchModel
    {
        public object studentId { get; set; }
    }

    public class StudentRatingModel
    {
        public object _id { get; set; }
        public long ratingDate { get; set; }

        public List<RatingClass> classes { get; set; }
        public int status { get; set; }
    }

    public class RatingClass
    {
        public object classId { get; set; }
        public object subjectId { get; set; }
        public int sectionId { get; set; }
        public List<StudentRatingValue> students { get; set; }

    }

    public class StudentRatingValue
    {
        public object studentId { get; set; }
        public int ratingValue { get; set; }
    }

    public class StudentRatingProjection
    {
        public object _id { get; set; }
        public object studentId { get; set; }
        public int ratingValue { get; set; }
        public object classId { get; set; }
        public object subjectId { get; set; }
        public int sectionId { get; set; }
    }

    public class SpecificRatingStudentProjection
    {
        public object _id { get; set; }
        public object studentId { get; set; }
        public int ratingValue { get; set; }
        public object classId { get; set; }
        public BsonArray subjects { get; set; }
    }
}