using Newtonsoft.Json;
using RestServiceGolden.Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace RestServiceGolden.App_Start
{
    public class PreflightRequestsHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Contains("Origin") && request.Method.Method.Equals("OPTIONS"))
            {
                var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
                // Define and add values to variables: origins, headers, methods (can be global)               
                response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
                response.Headers.Add("Access-Control-Allow-Headers", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "*");
                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);
                return tsc.Task;
            }
            else
            {
                if (!request.RequestUri.ToString().Contains("/api/archivos/"))
                {
                    var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
                    bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                    var bodyText = bodyStream.ReadToEnd();
                    var logger = new Logger("Request");
                    logger.AgregarMensaje(request.RequestUri.ToString(), " Parametros de entrada: " +
                       bodyText.Trim());
                    logger.EscribirLog();
                }
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}