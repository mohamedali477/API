using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DemoWebApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DemoWebApi.Controllers.School
{
    #region-------- School Info -------
    public class SchoolBranch
    {
        public object _id { get; set; }
        public int status { get; set; }
        public SchoolBranchBasicInfo branchBasicInfo { get; set; }
        public Contact branchContact { get; set; }
        public Address branchAddress { get; set; }
    }

    public class SchoolModel
    {
        public object _id { get; set; }
        public int status { get; set; }
        public SchoolBranchBasicInfo schoolBasicInfo { get; set; }
        public Contact schoolContact { get; set; }
        public List<SchoolBranch> branch { get; set; }
    }

    #endregion

    #region---School branch class info ---------

    public class SchoolBranchDdl
    {
        public object _id { get; set; }
        public string name { get; set; }
    }

    public class classDdl
    {
        public object _id { get; set; }
        public int status { get; set; }
        public string name { get; set; }
        public BsonValue classSection { get; set; }
    }

    public class classSubjectDdl
    {
        public object _id { get; set; }
        public int status { get; set; }
        public string name { get; set; }
        public BsonValue classSection { get; set; }
        public List<subjectModel> subject { get; set; }
    }

    public class subjectModel
    {
        public object _id { get; set; }
        public int status { get; set; }
        public string name { get; set; }
    }
    #endregion

}