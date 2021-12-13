using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using DemoWebApi.Models;
using DemoWebApi.Models.StudentGuardian;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace DemoWebApi.Controllers.Guardian
{
    [System.Web.Http.RoutePrefix("api/guardian")]
    public class GuardianController : ApiController
    {
        // GET api/<controller>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("getlist")]
        public HttpResponseMessage getlist([FromBody]GuardianSearch gurdianSearch)
        {
            FilterDefinition<GuardianModel> filter = new BsonDocumentFilterDefinition<GuardianModel>(new BsonDocument());
            
            #region---------- Search and Sort Criteria -----------------------

            var searchParams = gurdianSearch.SearchParameters;
            var paging = gurdianSearch.paging;
            var sorting = gurdianSearch.sorting;

            if (searchParams != null)
            {
                if (!string.IsNullOrEmpty(searchParams.firstName))
                {
                    filter = filter & Builders<GuardianModel>.Filter.Regex(x => x.basicInfo.firstName, BsonRegularExpression.Create(new Regex(searchParams.firstName, RegexOptions.IgnoreCase)));
                }

                if (!string.IsNullOrEmpty(searchParams.lastName))
                {
                    filter = filter & Builders<GuardianModel>.Filter.Regex(x => x.basicInfo.lastName, BsonRegularExpression.Create(new Regex(searchParams.lastName, RegexOptions.IgnoreCase)));
                }

                if (searchParams.dob != null)
                {
                    filter = filter & Builders<GuardianModel>.Filter.Where(x => x.basicInfo.dob == searchParams.dob);
                }

                if (searchParams.castId != null)
                {
                    filter = filter & Builders<GuardianModel>.Filter.Where(x => x.basicInfo.castId == searchParams.castId);
                }

                if (searchParams.genderId != null)
                {
                    filter = filter & Builders<GuardianModel>.Filter.Where(x => x.basicInfo.genderId == searchParams.genderId);
                }
                if (searchParams.religionId != null)
                {
                    filter = filter & Builders<GuardianModel>.Filter.Where(x => x.basicInfo.religionId == searchParams.religionId);
                }
                if (searchParams.relationId != null)
                {
                    filter = filter & Builders<GuardianModel>.Filter.Where(x => x.relationId == searchParams.relationId);
                }
            }

            var sort = Builders<GuardianModel>.Sort.Descending(sorting.fieldName);
            if (sorting.isAsc)
            {
                sort = Builders<GuardianModel>.Sort.Ascending(sorting.fieldName);
            }

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<GuardianModel>("view_guardians");



            var data = mongoCollection.Find(filter).Sort(sort).Skip(paging.pageSize * paging.pageIndex).Limit(paging.pageSize).ToList();
            var rowCount = mongoCollection.Find(filter).CountDocuments();

            var result = new Result()
            {
                count = rowCount,
                data = data
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("Deactivate")]
        public HttpResponseMessage Deactivate(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<StudentGuardianModel>("StudentGuardian");

            var filter = Builders<StudentGuardianModel>.Filter.Where(
                x => x.guardians.Any(
                    guardian => (ObjectId) guardian._id == new ObjectId(id)
                ));

            var update = Builders<StudentGuardianModel>.Update.Set(
                x => x.guardians[-1].status, 0
                );

            var result = mongoCollection.UpdateOne(filter, update);
            var jsonWriterSettings = new JsonWriterSettings {OutputMode = JsonOutputMode.Strict};
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

    }

}