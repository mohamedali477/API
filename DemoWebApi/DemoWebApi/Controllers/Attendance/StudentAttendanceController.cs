using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Web.Http;
using DemoWebApi.Controllers.CronJob;
using DemoWebApi.Controllers.Student;
using DemoWebApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DemoWebApi.Controllers.Attendance
{
    [System.Web.Http.RoutePrefix("api/studentAttendance")]
    public class StudentAttendanceController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("getClassSectionAttendance")]
        public HttpResponseMessage getClassSectionAttendance([FromBody] StudentAttendanceSearchModel attendanceSearch)
        {
            FilterDefinition<BsonDocument> filter =
                new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument());

            #region--------- filter the date  -------------

            filter = Builders<BsonDocument>.Filter.Where(x =>
                x["attendanceDate"] == attendanceSearch.attendanceDate);

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var allClasses = db.GetCollection<BsonDocument>("StudentManualAttendance")
                .Aggregate().Match(filter).Unwind(x => x["classes"]);


            #region------ Now filter the class and section ------------

            // reinitialize the filter to remove the date filter, so don't use & while adding classId filter
            filter = Builders<BsonDocument>.Filter.Where(x =>
                x["classes"]["classId"] == new ObjectId(attendanceSearch.classId.ToString())
            );

            if (attendanceSearch.sectionId > 0)
            {
                filter = filter & Builders<BsonDocument>.Filter.Where(x =>
                             x["classes"]["sectionId"] == attendanceSearch.sectionId
                         );
            }

            #endregion

            var studentAttendanceStatus = allClasses.Match(filter).Unwind(y => y["classes"]["students"])
                .Project<StudentManualAttendanceProjection>(@"{
                            '_id' : '$_id',
                            'studentId' : '$classes.students.studentId',
                            'attendanceStatusId' : '$classes.students.attendanceStatusId',
                            'classId' : '$classes.classId',
                            'sectionId' : '$classes.sectionId',
                            'status' : '$status'
                }")
                .ToList();

            var result = from attendance in studentAttendanceStatus.AsQueryable()
                join stu in db.GetCollection<StudentModel>("view_all_students").AsQueryable() on
                    attendance.studentId equals stu.studentId
                orderby stu.academicInfo.rollNo
                select new
                {
                    attendance._id,
                    attendance.studentId,
                    attendance.attendanceStatusId,
                    attendance.classId,
                    attendance.sectionId,
                    stu.basicInfo,
                    stu.contactInfo,
                    stu.academicInfo
                };


            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("saveAttendance")]
        public HttpResponseMessage saveAttendance([FromBody] StudentAttendanceModel attendance)
        {

            #region------- convert mongoId string into mongo Object ids

            var document = BsonSerializer.Deserialize<StudentAttendanceModel>(Newtonsoft.Json.JsonConvert.SerializeObject(attendance));

            document._id = new ObjectId(Convert.ToString(document._id));

            foreach (var cls in document.classes)
            {
                for (int idx = 0; idx < cls.students.Count; idx++)
                {
                    cls.students[idx].studentId = new ObjectId(Convert.ToString(cls.students[idx].studentId));
                }
            }

            #endregion

            FilterDefinition<StudentAttendanceModel> filter =
                new BsonDocumentFilterDefinition<StudentAttendanceModel>(new BsonDocument());

            filter = filter & Builders<StudentAttendanceModel>.Filter.Where(x =>
                         x.attendanceDate == attendance.attendanceDate);

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var mongoCollection = db.GetCollection<StudentAttendanceModel>("StudentManualAttendance").Find(filter).ToList();

            // We have only one document for one date in attendance collection
            var stuAttendances = mongoCollection[0];
            
            foreach (var cls in document.classes)
            {
                // Remove the attendance record for this class and section id for given date
                var pullFilter =
                    Builders<StudentAttendanceModel>.Update.PullFilter(p => p.classes,
                        c => c.classId == cls.classId && c.sectionId == cls.sectionId);
                db.GetCollection<StudentAttendanceModel>("StudentManualAttendance")
                    .FindOneAndUpdate(p => p.attendanceDate == attendance.attendanceDate, pullFilter);


                // Add the attendance record for this class and section id for given date
                var pushFilter =
                    Builders<StudentAttendanceModel>.Update.Push(p => p.classes, cls);
                db.GetCollection<StudentAttendanceModel>("StudentManualAttendance")
                    .FindOneAndUpdate(p => p.attendanceDate == attendance.attendanceDate, pushFilter);

            }

            var result = new WriteResult("Student attendance get saved successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("resetAttendance")]
        public HttpResponseMessage resetAttendance()
        {
            var today = BaseClass.computeToDayTicks();

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            db.GetCollection<StudentAttendanceModel>("StudentManualAttendance")
                .DeleteOne(p => p.attendanceDate == today);

            var cron = new CronJobController();

            var dbList = new List<string>();
            dbList.Add(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            cron.autoMarkStudentAttendance(dbList, today);

            var result = new WriteResult("Student Attendance reset successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
