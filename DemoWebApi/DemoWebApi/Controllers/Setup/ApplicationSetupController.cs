using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web.Http;
using DemoWebApi.Controllers.Employee;
using DemoWebApi.Controllers.Exam;
using DemoWebApi.Controllers.Role;
using DemoWebApi.Controllers.School;
using DemoWebApi.Controllers.Subject;
using DemoWebApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace DemoWebApi.Controllers.Setup
{
    [System.Web.Http.RoutePrefix("api/appSetUp")]
    public class ApplicationSetupController : ApiController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("deleteAllViews")]
        public HttpResponseMessage deleteAllViews(string targetSchoolBranchId = null)
        {
            var viewFilePath = @"C:\Users\e3025807\OneDrive - FIS\Simranjeet Singh\POC\Angular\DemoApp\src\MongoQueries.txt";
            FilterDefinition<BsonDocument> filter = new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument());

            string superAdminSchoolId = "293535a80000000000000000";
            string superAdminBranchId = "293543f50000000000000000";

            string viewCollection = "system.views";

            var mainDb = BaseClass.GetDatabase(superAdminBranchId);
            List<string> viewsList = new List<string>();


            var collections = mainDb.GetCollection<BsonDocument>(viewCollection);
            var allViews = collections.Find(filter);


            // Drop all the views from all DBS
            List<string> allDbs = new List<string>();
            allDbs = BaseClass.GetAllDatabaseNames();

            foreach (var dbName in allDbs)
            {
                if (dbName.Equals(superAdminBranchId))
                {
                    continue; ;
                }

                if (targetSchoolBranchId != null && !dbName.Equals(targetSchoolBranchId))
                {
                    continue;
                }

                var db = BaseClass.GetDatabase(dbName);
                var collection = db.GetCollection<BsonDocument>(viewCollection);
                collection.DeleteMany(filter);

            }

            var result = new WriteResult("All views get deleted successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("versioncheck")]
        public HttpResponseMessage versioncheck(long t)
        {
            var appVersion = ConfigurationManager.AppSettings["appVersion"];
            var forceUpdate = ConfigurationManager.AppSettings["forceUpdate"];

            var data = new
            {
                version = appVersion,
                forceUpdate = forceUpdate
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getAllMasterData")]
        public HttpResponseMessage getAllMasterData()
        {
            var masterData = fetchAllMasterDataFromDb(this);

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = masterData.ToJson((jsonWriterSettings));

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
        public object fetchAllMasterDataFromDb(ApiController apiController)
        {
            var classController = new BranchClassController();
            var subjectController = new SubjectController();
            var empController = new EmployeeController();
            var roleController = new RoleController();
            var examTypeController = new ExamTypeController();
            var userSettingsController = new UserSettingsController();
            var schoolController = new SchoolController();

            var dataClassSubjectDdl = classController.masterDataClassSubjectDdl(apiController);
            var dataSubjectDdl = subjectController.masterDataSubjectDdl(apiController);
            var dataTeacherDdl = empController.masterDataEmployeeDdl(HardCodedRoles.teacherRoleId, apiController);
            var dataDriverDdl = empController.masterDataEmployeeDdl(HardCodedRoles.driverRoleId, apiController);
            var dataConductorDdl = empController.masterDataEmployeeDdl(HardCodedRoles.conductorRoleId, apiController);
            var dataRoleDdl = roleController.masterDataRoleDdl(apiController);
            var dataExamTypeDdl = examTypeController.masterDataExamTypeDdl(apiController);
            var dataUserSettings = userSettingsController.masterDataUserSettings(apiController);
            var dataSchoolBranch = schoolController.masterDataSchoolBranchDdl();

            var masterData = new
            {
                classSubjectDdl = dataClassSubjectDdl,
                subjectDdl = dataSubjectDdl,
                teacherDdl = dataTeacherDdl,
                driverDdl = dataDriverDdl,
                conductorDdl = dataConductorDdl,
                roleDdl = dataRoleDdl,
                examTypeDdl = dataExamTypeDdl,
                userSettings = dataUserSettings,
                schoolBranchDdl = dataSchoolBranch
            };

            return masterData;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("resetCreateViews")]
        public HttpResponseMessage resetCreateViews(string targetSchoolBranchId = null)
        {
          //  var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);


            var result = new WriteResult("All views get deleted successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
