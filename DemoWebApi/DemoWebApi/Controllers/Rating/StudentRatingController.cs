using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web.Http;
using DemoWebApi.Controllers.CronJob;
using DemoWebApi.Controllers.School;
using DemoWebApi.Controllers.Student;
using DemoWebApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DemoWebApi.Controllers.Rating
{
    [System.Web.Http.RoutePrefix("api/studentRating")]
    public class StudentRatingController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("getClassSectionRating")]
        public HttpResponseMessage getClassSectionRating([FromBody] StudentRatingSearchModel ratingSearch)
        {
            ratingSearch.ratingDate = BaseClass.computeRatingDate();

            FilterDefinition<BsonDocument> filter =
                new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument());

            #region--------- filter the date  -------------

            filter = Builders<BsonDocument>.Filter.Where(x =>
                x["ratingDate"] == ratingSearch.ratingDate);

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var allClasses = db.GetCollection<BsonDocument>("StudentRating")
                .Aggregate().Match(filter).Unwind(x => x["classes"]);




            #region------ Now filter the class and section ------------

            // reinitialize the filter to remove the date filter, so don't use & while adding classId filter
            filter = Builders<BsonDocument>.Filter.Where(x =>
                x["classes"]["classId"] == new ObjectId(ratingSearch.classId.ToString())
            );

            filter = Builders<BsonDocument>.Filter.Where(x =>
                x["classes"]["subjectId"] == new ObjectId(ratingSearch.subjectId.ToString())
            );

            if (ratingSearch.sectionId > 0)
            {
                filter = filter & Builders<BsonDocument>.Filter.Where(x =>
                             x["classes"]["sectionId"] == ratingSearch.sectionId
                         );
            }

            #endregion

            var studentRatingStatus = allClasses.Match(filter).Unwind(y => y["classes"]["students"])
                .Project<StudentRatingProjection>(@"{
                            '_id' : '$_id',
                            'studentId' : '$classes.students.studentId',
                            'ratingValue' : '$classes.students.ratingValue',
                            'classId' : '$classes.classId',
                            'subjectId' : '$classes.subjectId',
                            'sectionId' : '$classes.sectionId'
                }")
                .ToList();

            var result = from rating in studentRatingStatus.AsQueryable()
                         join stu in db.GetCollection<StudentModel>("view_all_students").AsQueryable() on
                             rating.studentId equals stu.studentId
                         orderby stu.academicInfo.rollNo
                         select new
                         {
                             rating._id,
                             rating.studentId,
                             rating.ratingValue,
                             rating.classId,
                             rating.subjectId,
                             rating.sectionId,
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

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getSpecificRatingStudents")]
        public HttpResponseMessage getSpecificRatingStudents(int ratingValue)
        {
            var ratingDate = BaseClass.computeRatingDate();

            FilterDefinition<BsonDocument> filter =
                new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument());

            #region--------- filter the date  -------------

            filter = Builders<BsonDocument>.Filter.Where(x =>
                x["ratingDate"] == ratingDate);

            filter = filter & Builders<BsonDocument>.Filter.Where(x =>
                x["ratingValue"] == ratingValue);

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var studentRatingStatus = db.GetCollection<BsonDocument>("view_studentRating")
                .Aggregate().Match(filter)
                .Project<SpecificRatingStudentProjection>(@"{
                            '_id' : '$_id',
                            'studentId' : '$_id',
                            'ratingValue' : '$ratingValue',
                            'classId' : '$classId',
                            'subjects' : '$subjects'
                }")
                .ToList();

            var result = from rating in studentRatingStatus.AsQueryable()
                         join stu in db.GetCollection<StudentModel>("view_all_students").AsQueryable() on
                             rating.studentId equals stu.studentId
                         join cls in db.GetCollection<BranchClassModel>("Class").AsQueryable() on
                             rating.classId equals cls._id
                         orderby cls.basicInfo.className, stu.academicInfo.rollNo
                         select new
                         {
                             rating._id,
                             rating.studentId,
                             rating.ratingValue,
                             rating.classId,
                             subjectIds = rating.subjects,
                             cls.basicInfo.className,
                             subjects = cls.subject,
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
        [System.Web.Http.Route("getStudentWholeRating")]
        public HttpResponseMessage getStudentWholeRating([FromBody]StudentWholeRatingSearchModel ratingSearch)
        {

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var collection = db.GetCollection<BsonDocument>("view_student_wholeRating");

            var filter = Builders<BsonDocument>.Filter.Where(x => (ObjectId)x["_id"] == BaseClass.convertStringObjectIdToMongoObjectId(ratingSearch.studentId));

            var result = collection.Find(filter).ToList();

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("saveRating")]
        public HttpResponseMessage saveRating([FromBody] StudentRatingModel rating)
        {
            rating.ratingDate = BaseClass.computeRatingDate();

            #region------- convert mongoId string into mongo Object ids

            var document =
                BsonSerializer.Deserialize<StudentRatingModel>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(rating));

            document._id = new ObjectId(Convert.ToString(document._id));

            foreach (var cls in document.classes)
            {
                for (int idx = 0; idx < cls.students.Count; idx++)
                {
                    cls.students[idx].studentId = new ObjectId(Convert.ToString(cls.students[idx].studentId));
                }
            }

            #endregion

            FilterDefinition<StudentRatingModel> filter =
                new BsonDocumentFilterDefinition<StudentRatingModel>(new BsonDocument());

            filter = filter & Builders<StudentRatingModel>.Filter.Where(x =>
                         x.ratingDate == rating.ratingDate);

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var mongoCollection = db.GetCollection<StudentRatingModel>("StudentRating").Find(filter).ToList();

            // We have only one document for one date in rating collection
            var stuRatings = mongoCollection[0];
            
            foreach (var cls in document.classes)
            {
                // Add the rating record for this class and section id
                var pullFilter =
                    Builders<StudentRatingModel>.Update.PullFilter(p => p.classes,
                        c => c.classId == cls.classId && c.sectionId == cls.sectionId && c.subjectId == cls.subjectId);
                db.GetCollection<StudentRatingModel>("StudentRating")
                    .FindOneAndUpdate(p => p.ratingDate == rating.ratingDate, pullFilter);


                // Add the rating record for this class and section id
                var pushFilter =
                    Builders<StudentRatingModel>.Update.Push(p => p.classes, cls);
                db.GetCollection<StudentRatingModel>("StudentRating")
                    .FindOneAndUpdate(p => p.ratingDate == rating.ratingDate, pushFilter);

            }

            var result = new WriteResult("Student rating saved successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("resetRating")]
        public HttpResponseMessage resetRating()
        {
            var today = BaseClass.computeRatingDate();

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            db.GetCollection<StudentRatingModel>("StudentRating")
                .DeleteOne(p => p.ratingDate == today);

            var cron = new CronJobController();

            var dbList = new List<string>();
            dbList.Add(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            cron.autoMarkStudentRating(dbList, today);

            var result = new WriteResult("Student rating reset successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
