using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using DemoWebApi.Controllers.Attendance;
using DemoWebApi.Controllers.Rating;
using DemoWebApi.Controllers.School;
using DemoWebApi.Models;
using DemoWebApi.Models.CronJob;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using StudentAttendanceStatus = DemoWebApi.Controllers.Attendance.StudentAttendanceStatus;

namespace DemoWebApi.Controllers.CronJob
{
    [System.Web.Http.RoutePrefix("api/cronJob")]
    public class CronJobController : ApiController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("autoMarkStudentAttendance")]
        public HttpResponseMessage autoMarkStudentAttendance(List<string> dbList = null, long today = 0)
        {
            if (today == 0)
            {
                today = BaseClass.computeToDayTicks();
            }

            FilterDefinition<BsonDocument> filter = new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument());

            // Get list of all databases
            if (dbList == null)
            {
                dbList = BaseClass.GetAllDatabaseNames(0);
            }

            foreach (var db in dbList)
            {
                var client = BaseClass.GetDatabase(db);

                FilterDefinition<StudentAttendanceModel> filterAttendance =
                    new BsonDocumentFilterDefinition<StudentAttendanceModel>(new BsonDocument());
                filterAttendance = filterAttendance &
                                   Builders<StudentAttendanceModel>.Filter.Where(x => x.attendanceDate == today);

                var attendanceCollection = client.GetCollection<StudentAttendanceModel>("StudentManualAttendance");
                var attenanceCount = attendanceCollection.Find(filterAttendance).ToList().Count();


                // if there is no record in attendance table for today
                // then mark auto attendance and create entries in db
                if (attenanceCount == 0)
                {


                    StudentAttendanceModel attendance = new StudentAttendanceModel()
                    {
                        _id = ObjectId.GenerateNewId(),
                        attendanceDate = today,
                        status = 1
                    };


                    var classes = new List<AttendanceClass>();

                    // List of all active students group by their classId and sectionId
                    var mongoCollection = client.GetCollection<BsonDocument>("view_cj_classSectionStudents");
                    var documents = mongoCollection.Find(filter).ToList();

                    Dictionary<string, AttendanceClass> c = new Dictionary<string, AttendanceClass>();

                    // Mark auto attendance for each class and section
                    foreach (var doc in documents)
                    {
                        var cls = new AttendanceClass();

                        cls.classId = new ObjectId(Convert.ToString(doc["_id"]["classId"]));
                        cls.sectionId = Convert.ToInt32(doc["_id"]["sectionId"]);

                        cls.students = new List<StudentAttendanceStatus>();
                        foreach (var stu in (BsonArray) doc["students"])
                        {
                            cls.students.Add(new StudentAttendanceStatus()
                            {
                                studentId = new ObjectId(stu.ToString()),
                                attendanceStatusId = 1 // by default pending/not taken
                            });
                        }

                        classes.Add(cls);
                    }

                    attendance.classes = classes;
                    attendanceCollection.InsertOne(attendance);
                }
            }

            var result = new WriteResult("Auto mark attendance done successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("autoMarkStudentRating")]
        public HttpResponseMessage autoMarkStudentRating(List<string> dbList = null, long today = 0)
        {
            if (today == 0)
            {
                today = BaseClass.computeRatingDate();
            }


            FilterDefinition<BsonDocument> filter = new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument());

            // Get list of all databases
            if (dbList == null)
            {
                dbList = BaseClass.GetAllDatabaseNames(0);
            }


            foreach (var db in dbList)
            {
                var client = BaseClass.GetDatabase(db);

                FilterDefinition<StudentRatingModel> filterRating =
                    new BsonDocumentFilterDefinition<StudentRatingModel>(new BsonDocument());
                filterRating = filterRating &
                               Builders<StudentRatingModel>.Filter.Where(x => x.ratingDate == today);

                var ratingCollection = client.GetCollection<StudentRatingModel>("StudentRating");
                var ratingCount = ratingCollection.Find(filterRating).ToList().Count();


                // if there is no record in attendance table for today
                // then mark auto attendance and create entries in db
                if (ratingCount == 0)
                {

                    StudentRatingModel rating = new StudentRatingModel()
                    {
                        _id = ObjectId.GenerateNewId(),
                        ratingDate = today,
                        status = 1
                    };


                    var classes = new List<RatingClass>();

                    // List of all active students group by their classId and sectionId
                    var mongoCollection = client.GetCollection<BsonDocument>("view_cj_class_subject_students");
                    var documents = mongoCollection.Find(filter).ToList();

                    Dictionary<string, RatingClass> c = new Dictionary<string, RatingClass>();

                    // Mark auto attendance for each class and section
                    foreach (var doc in documents)
                    {
                        foreach (var subject in (BsonArray) doc["subjects"])
                        {

                            var cls = new RatingClass();

                            cls.classId = new ObjectId(Convert.ToString(doc["_id"]["classId"]));
                            cls.subjectId = new ObjectId(Convert.ToString(subject["subjectId"]));
                            cls.sectionId = Convert.ToInt32(doc["_id"]["sectionId"]);

                            cls.students = new List<StudentRatingValue>();
                            foreach (var stu in (BsonArray) doc["students"])
                            {
                                cls.students.Add(new StudentRatingValue()
                                {
                                    studentId = new ObjectId(stu.ToString()),
                                    ratingValue = 3 // by default satisfied rating
                                });
                            }

                            if (cls.sectionId == Convert.ToInt32(subject["sectionId"]))
                            {
                                classes.Add(cls);
                            }
                        }
                    }

                    rating.classes = classes;
                    ratingCollection.InsertOne(rating);
                }
            }

            var result = new WriteResult("Auto mark student rating done successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

    }
}
