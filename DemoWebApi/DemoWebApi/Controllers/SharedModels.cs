using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DemoWebApi.Models
{
    public class UploadByteMode
    {
        public string fileName { get; set; }
        public string file { get; set; }
    }
    public class BasicInfo
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public long? dob { get; set; }
        public int genderId { get; set; }
        public int religionId { get; set; }
        public int castId { get; set; }
    }

    public class Contact
    {
        public string contactNo { get; set; }
        public string altContactNo { get; set; }
        public string faxNo { get; set; }
        public string email { get; set; }
    }

    public class Credential
    {
        public string loginId { get; set; }
        public string password { get; set; }
    }

    public class MedicalInfo
    {
        public int? bloodGroupId { get; set; }
        public string hospitalName { get; set; }
        public string doctorName { get; set; }
        public string doctorContactNo { get; set; }
        public string specialConsiderations { get; set; }
    }

    public class GovernmentIds
    {
        public string passportNo { get; set; }
        public string aadharCardNo { get; set; }
        public string panNo { get; set; }
        public string drivingLicenceNo { get; set; }
        public string voterCardNo { get; set; }
    }

    public class Occupation
    {
        public string jobTitle { get; set; }
        public string jobLocation { get; set; }
        public int jobTypeId { get; set; }
        public string description { get; set; }
    }

    public class Address
    {
        public int countryId { get; set; }
        public int stateId { get; set; }
        public string cityName { get; set; }
        public string pinCode { get; set; }
        public string addressLine { get; set; }
    }

    public class Transport
    {
        public object routeId { get; set; }
        public object stoppageId { get; set; }
    }

    public class AuthorizationHeader
    {
        public string dateTime { get; set; }
        public string currentUserRoleId { get; set; }
        public string currentUserId { get; set; }
        public string currentSchoolId { get; set; }
        public string currentSchoolBranchId { get; set; }
    }

    public class SchoolBranchBasicInfo
    {
        public string name { get; set; }
        public string description { get; set; }
    }

    public class countModel
    {
        public countId _id { get; set; }
        public double count { get; set; }
    }
    public class countId
    {
        public int genderId { get; set; }
    }

    public class DateRange
    {
        public long? fromDate { get; set; }
        public long? toDate { get; set; }
    }
    public class NoSearchModel
    {
        public Paging paging;
        public Sorting sorting;
    }
    
    public class Paging
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }
    public class Sorting
    {
        public string fieldName { get; set; }
        public bool isAsc { get; set; }
    }
    public class Result
    {
        public object count;
        public object data;
    }

    public class WriteResult
    {
        public WriteResult(string msg, bool success = true)
        {
            this.isSuccess = success;
            this.message = msg;
        }
        public bool isSuccess { get; set; }
        public string message { get; set; }
    }

    public class MongoId
    {
        [BsonId]
        public ObjectId _id { get; set; }
    }
}