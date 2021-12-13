using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DemoWebApi.Models;

namespace DemoWebApi.Controllers
{
    [System.Web.Http.RoutePrefix("api/UploadFile")]
    public class UploadFileController : ApiController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("Upload")]
        public HttpResponseMessage Upload()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();

                foreach (string file in httpRequest.Files)
                {

                    var storagePath =
                        HttpContext.Current.Server.MapPath(string.Format(@"~/{0}", httpRequest.Files.AllKeys[0]));


                    var directoryPath = storagePath.Substring(0, storagePath.LastIndexOf("\\"));
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }


                    var postedFile = httpRequest.Files[file];

                    postedFile.SaveAs(storagePath);
                    docfiles.Add(storagePath);
                }

                result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("UploadBytes")]
        public HttpResponseMessage UploadBytes([FromBody] UploadByteMode data)
        {

            HttpResponseMessage result = null;

            var storagePath = HttpContext.Current.Server.MapPath("~/" + data.fileName);

            var directoryPath = storagePath.Substring(0, storagePath.LastIndexOf("\\"));
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var idx = data.file.IndexOf("base64,") + 7;
            var img = Convert.FromBase64String(data.file.Substring(idx));
            File.WriteAllBytes(storagePath, img);
            result = Request.CreateResponse(HttpStatusCode.Created, "Success");

            return result;
        }

    }

}