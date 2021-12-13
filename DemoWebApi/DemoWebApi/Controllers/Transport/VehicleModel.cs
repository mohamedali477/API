using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DemoWebApi.Models;

namespace DemoWebApi.Controllers.Transport
{
    #region-------- Vehicle Info -------
    public class VehicleBasicInfo
    {
        public string companyName { get; set; }
        public string modelName { get; set; }
        public string registrationNo { get; set; }
        public long? registrationDate { get; set; }
        public int? seatCapacity { get; set; }
        public string customName { get; set; }
        public int? vehicleTypeId { get; set; }
        public int? fuelTypeId { get; set; }
        public bool? isRental { get; set; }
        public bool? isOnlyForOffice { get; set; }
    }
    
    public class Maintenance
    {
        public string maintenanceName { get; set; }
        public object maintenanceCost { get; set; }
        public long? maintenanceDate { get; set; }
        public bool isExpected { get; set; }
        public string description { get; set; }
        public object _id { get; set; }
        public int status { get; set; }
    }

    public class VehicleModel
    {
        public VehicleBasicInfo basicInfo { get; set; }
        public object routeId { get; set; }
        public List<Maintenance> maintenance { get; set; }
        public object _id { get; set; }
        public int status { get; set; }
    }

#endregion

    #region------ Vehicle Search ----------
public class VehicleSearch
    {
        public VehicleBasicInfo SearchParameters;
        public Paging paging;
        public Sorting sorting;
    }

    #endregion
}