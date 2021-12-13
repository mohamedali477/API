using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace DemoWebApi.Controllers
{
    public static class HardCodedRoles
    {
        public static ObjectId superAdminRoleId
        {
            get
            {
                return new ObjectId("5e2d5f6a16dd8e3e0df01980");
            }
        }

        public static ObjectId schoolAdminRoleId
        {
            get
            {
                return new ObjectId("5e2d5f6a16dd8e3e0df01981");
            }
        }
        public static ObjectId schoolBranchAdminRoleId
        {
            get
            {
                return new ObjectId("5e2d5f6a16dd8e3e0df01982");
            }
        }

        public static ObjectId studentRoleId
        {
            get
            {
                return new ObjectId("5e2d5f6a16dd8e3e0df01990");
            }
        }

        public static ObjectId guardianRoleId
        {
            get
            {
                return new ObjectId("5e2d5f6a16dd8e3e0df01991");
            }
        }

        public static ObjectId teacherRoleId
        {
            get
            {
                return new ObjectId("5e2d5f6a16dd8e3e0df01992");
            }
        }

        public static ObjectId driverRoleId
        {
            get
            {
                return new ObjectId("5e2d5f6a16dd8e3e0df01993");
            }
        }

        public static ObjectId conductorRoleId
        {
            get
            {
                return new ObjectId("5e2d5f6a16dd8e3e0df01994");
            }
        }


    }
}