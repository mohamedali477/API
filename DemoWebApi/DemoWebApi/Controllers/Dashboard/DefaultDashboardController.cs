using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using DemoWebApi.Controllers.Event;
using DemoWebApi.Controllers.Exam;
using DemoWebApi.Controllers.Guardian;
using DemoWebApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DemoWebApi.Controllers.Dashboard
{
    [System.Web.Http.RoutePrefix("api/dashboard")]
    public class DefaultDashboardController : ApiController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("schoolStrengthAndAttendanceCount")]
        public HttpResponseMessage schoolStrengthAndAttendanceCount()
        {
            var eventController = new EventController();
            var examController = new ExamController();
            var today = BaseClass.computeToDayTicks();
            var next10Days = BaseClass.convertMSTicksToJavascriptTics(BaseClass.IndianDateTime.AddDays(10).Ticks);

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var data = new 
            {
                strengthCount = stuGuardianEmpCount(db),
                attendanceCount = schoolAttendanceCount(db, today),
                studentRatingCount = studentRatingCount(db),
                schoolEvent = eventController.getSchoolEventByDateRange(db,today, next10Days),
                schoolExam = examController.getSchoolExamByDateRange(db, today, next10Days)
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json , Encoding.UTF8, "application/json")
            };
        }


        private List<Result> stuGuardianEmpCount(IMongoDatabase db)
        {
            FilterDefinition<countModel> filter = new BsonDocumentFilterDefinition<countModel>(new BsonDocument());

            var guardianCollection = db.GetCollection<countModel>("view_guardian_count");
            var guardianCounts = guardianCollection.Find(filter).ToList();

            var studentCollection = db.GetCollection<countModel>("view_student_count");
            var studentCounts = studentCollection.Find(filter).ToList();

            var employeeCollection = db.GetCollection<countModel>("view_employee_count");
            var employeeCounts = employeeCollection.Find(filter).ToList();

            var result = new List<Result>();

            result.Add(new Result()
            {
                data = "guardian",
                count = guardianCounts
            });

            result.Add(new Result()
            {
                data = "student",
                count = studentCounts
            });

            result.Add(new Result()
            {
                data = "employee",
                count = employeeCounts
            });

            return result;
        }

        private List<SchoolAttendanceCount> schoolAttendanceCount(IMongoDatabase db, long today)
        {
            FilterDefinition<SchoolAttendanceCount> filter = new BsonDocumentFilterDefinition<SchoolAttendanceCount>(new BsonDocument());


            #region------- Filter to check whether attendance already taken or not. If taken the update that

            filter = filter & Builders<SchoolAttendanceCount>.Filter.Where(x =>
                         x._id.attendanceDate == today);
            #endregion
            
            var mongoCollection = db.GetCollection<SchoolAttendanceCount>("view_school_attendance_count");

            return mongoCollection.Find(filter).ToList();
        }

        private List<StudentRatingCount> studentRatingCount(IMongoDatabase db)
        {
            var today = BaseClass.computeRatingDate();

            FilterDefinition<StudentRatingCount> filter = new BsonDocumentFilterDefinition<StudentRatingCount>(new BsonDocument());


            #region------- Filter to check whether attendance already taken or not. If taken the update that

            filter = filter & Builders<StudentRatingCount>.Filter.Where(x =>
                         x._id.ratingDate == today);
            #endregion

            var mongoCollection = db.GetCollection<StudentRatingCount>("view_student_rating_count");

            return mongoCollection.Find(filter).ToList();
        }
    }
}
