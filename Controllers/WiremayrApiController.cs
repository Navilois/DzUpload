using System;
using System.IO;
using System.Web;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using DotNetNuke.Web.Api;

namespace WireMayr.Modules.DzUpload.Controllers
{
    public class WiremayrApiController : DnnApiController
    {/*
        /// <summary>
        /// Upload files via dropzone.js
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        [HttpGet]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public HttpResponseMessage UploadFiles()
        {
            bool isSavedSuccessfully = false;
            try
            {
                foreach (string fileName in HttpContext.Current.Request.Files)
                {
                    HttpPostedFile file = HttpContext.Current.Request.Files[fileName];
                    //Save file content goes here
                    if (file != null && file.ContentLength > 0)
                    {
                        var baseDir = new DirectoryInfo(string.Format("{0}Uploads", HttpContext.Current.Server.MapPath(@"\")));
                        string pathString = Path.Combine(baseDir.ToString(), HttpContext.Current.Request.Headers["WM-Guid"]);
                        if (!Directory.Exists(pathString))
                            Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, file.FileName);
                        file.SaveAs(path);
                        isSavedSuccessfully = true;
                    }

                }

            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, isSavedSuccessfully);
        }
    }

    /// <summary>
    /// Custom routing outside of DNN
    /// </summary>
    public class RouteMapper : IServiceRouteMapper
    {
        /// <inheritdoc/>
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "DzUpload",
                routeName: "DzUpload",
                url: "UploadFiles",
                defaults: new { controller = "WiremayrApi", action = "UploadFiles" },
                namespaces: new[] { "WireMayr.Modules.DzUpload.Controllers" }
                );
        }*/
    }
}