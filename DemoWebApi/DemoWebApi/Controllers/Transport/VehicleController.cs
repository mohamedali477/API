using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using DemoWebApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DemoWebApi.Controllers.Transport
{
    [System.Web.Http.RoutePrefix("api/Vehicle")]
    public class VehicleController : ApiController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getVehicle")]
        public HttpResponseMessage Get(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("Vehicle");

            var documents = mongoCollection.Find(x => x["_id"] == new ObjectId(id)).ToList();

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        // GET api/<controller>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("getVehicleList")]
        public HttpResponseMessage getVehicleList([FromBody]VehicleSearch vehicleSearch)
        {
            FilterDefinition<VehicleModel> filter = new BsonDocumentFilterDefinition<VehicleModel>(new BsonDocument());
            
            #region---------- Search and Sort Criteria -----------------------
            var searchParams = vehicleSearch.SearchParameters;
            var paging = vehicleSearch.paging;
            var sorting = vehicleSearch.sorting;

            if (searchParams != null)
            {
                if (!string.IsNullOrEmpty(searchParams.companyName))
                {
                    filter = filter & Builders<VehicleModel>.Filter.Regex(x => x.basicInfo.companyName, BsonRegularExpression.Create(new Regex(searchParams.companyName, RegexOptions.IgnoreCase)));
                }

                if (!string.IsNullOrEmpty(searchParams.modelName))
                {
                    filter = filter & Builders<VehicleModel>.Filter.Regex(x => x.basicInfo.modelName, BsonRegularExpression.Create(new Regex(searchParams.modelName, RegexOptions.IgnoreCase)));
                }

                if (!string.IsNullOrEmpty(searchParams.registrationNo))
                {
                    filter = filter & Builders<VehicleModel>.Filter.Regex(x => x.basicInfo.registrationNo, BsonRegularExpression.Create(new Regex(searchParams.registrationNo, RegexOptions.IgnoreCase)));
                }

                if (searchParams.registrationDate != null)
                {
                    filter = filter & Builders<VehicleModel>.Filter.Where(x => x.basicInfo.registrationDate == searchParams.registrationDate);
                }

                if (searchParams.seatCapacity != null)
                {
                    filter = filter & Builders<VehicleModel>.Filter.Where(x => x.basicInfo.seatCapacity == searchParams.seatCapacity);
                }

                if (!string.IsNullOrEmpty(searchParams.customName))
                {
                    filter = filter & Builders<VehicleModel>.Filter.Regex(x => x.basicInfo.customName, BsonRegularExpression.Create(new Regex(searchParams.customName, RegexOptions.IgnoreCase)));
                }

                if (searchParams.vehicleTypeId != null)
                {
                    filter = filter & Builders<VehicleModel>.Filter.Where(x => x.basicInfo.vehicleTypeId == searchParams.vehicleTypeId);
                }

                if (searchParams.fuelTypeId != null)
                {
                    filter = filter & Builders<VehicleModel>.Filter.Where(x => x.basicInfo.fuelTypeId == searchParams.fuelTypeId);
                }

                if (searchParams.isRental != null)
                {
                    filter = filter & Builders<VehicleModel>.Filter.Where(x => x.basicInfo.isRental == searchParams.isRental);
                }
                if (searchParams.isOnlyForOffice != null)
                {
                    filter = filter & Builders<VehicleModel>.Filter.Where(x => x.basicInfo.isOnlyForOffice == searchParams.isOnlyForOffice);
                }
            }
            
            var sort = Builders<VehicleModel>.Sort.Descending(sorting.fieldName);
            if (sorting.isAsc)
            {
                sort = Builders<VehicleModel>.Sort.Ascending(sorting.fieldName);
            }

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<VehicleModel>("view_vehicles");



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

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("saveVehicle")]
        public HttpResponseMessage saveVehicle([FromBody]VehicleModel vehicle)
        {
            //  vehicle.routeInfo.routeId = MongoDB.Bson.ObjectId.Parse(Convert.ToString(vehicle.routeInfo.routeId));
          //  vehicle.routeInfo.stoppageId = MongoDB.Bson.ObjectId.Parse(Convert.ToString(vehicle.routeInfo.stoppageId));

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var mongoCollection = db.GetCollection<BsonDocument>("Vehicle");
            var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(vehicle));

                mongoCollection.ReplaceOneAsync(
                filter: new BsonDocument("_id", document.GetValue("_id")),
                options: new UpdateOptions { IsUpsert = true },
                replacement: document);

            var result = new WriteResult("Vehicle saved successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("deleteVehicle")]
        public HttpResponseMessage deleteVehicle(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("Vehicle");

            mongoCollection.DeleteOne(x => x["_id"] == new ObjectId(id));

            var result = new WriteResult("Vehicle get deleted successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

    }
}