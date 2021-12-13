using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using DemoWebApi.Models;

namespace DemoWebApi.Controllers
{
    public class LogMetadata
    {
        public HttpRequestHeaders RequestHeader { get; set; }
        public string RequestContentType { get; set; }
        public string RequestUri { get; set; }
        public string RequestMethod { get; set; }
        public DateTime? RequestTimestamp { get; set; }
        public string RequestBody { get; set; }
        public string ResponseContentType { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
        public DateTime? ResponseTimestamp { get; set; }
        public string ResponseBody { get; set; }
    }

    public class Logs : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var logMetadata = BuildRequestMetadata(request);
            logMetadata.RequestBody = await request.Content.ReadAsStringAsync();

            var response = await base.SendAsync(request, cancellationToken);
            logMetadata = BuildResponseMetadata(logMetadata, response);
            logMetadata.ResponseBody = await response.Content.ReadAsStringAsync();

            await SendToLog(logMetadata);
            return response;
        }
        private LogMetadata BuildRequestMetadata(HttpRequestMessage request)
        {
           // string  = await request.Content.ReadAsStringAsync();

            LogMetadata log = new LogMetadata
            {
                RequestMethod = request.Method.Method,
                RequestTimestamp = DateTime.Now,
                RequestUri = request.RequestUri.ToString(),
                RequestHeader = request.Headers
            };
            return log;
        }
        private LogMetadata BuildResponseMetadata(LogMetadata logMetadata, HttpResponseMessage response)
        {
            logMetadata.ResponseStatusCode = response.StatusCode;
            logMetadata.ResponseTimestamp = DateTime.Now;
            logMetadata.ResponseContentType = response.Content.Headers.ContentType.MediaType;
            return logMetadata;
        }
        private async Task<bool> SendToLog(LogMetadata logMetadata)
        {
            AuthorizationHeader authorization = new AuthorizationHeader();

            var session = HttpContext.Current.Session;
            if (session != null)
            {
                if (session["authorization"] == null)
                {
                    authorization = (AuthorizationHeader) session["authorization"];
                }
            }




            // TODO: Write code here to store the logMetadata instance to a pre-configured log store...
            return true;
        }
    }
}