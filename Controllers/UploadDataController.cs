
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Mail;
using DotNetNuke.Web.Mvc.Helpers;
using System;
using System.Linq;
using System.Net;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using WireMayr.Modules.DzUpload.Components;
using WireMayr.Modules.DzUpload.Data;
using WireMayr.Modules.DzUpload.Models;


namespace WireMayr.Modules.DzUpload.Controllers
{
    [DnnHandleError]
    public class UploadDataController : DnnController
    {
        private ILog _log;

        protected ILog Log
        {
            get { return _log ?? (_log = LoggerSource.Instance.GetLogger(this.GetType())); }
        }


        [HttpGet]
        public ActionResult Index()
        {
            return View(new UploadData());
        }


        [HttpGet]
        public ActionResult Confirmation(UploadData model)
        {
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Index(UploadData currentUpload)
        {
            /* Todo:
             * - Set cookie for filled out data (?) -> https://stackoverflow.com/questions/14420624/how-to-keep-changed-form-content-when-leaving-and-going-back-to-https-page-wor
             * - Send Email on exception
            */
            if (ModelState.IsValid)
            {
                try
                {
                    // File upload
                    if (Request.Files.Count > 0)
                    {
                        string uploadPath = Server.MapPath("~/Portals/0/WM-Uploads/" + currentUpload.GUID + "/");
                        bool uploadDirExists = Directory.Exists(uploadPath);


                        // DB entry to UploadData on first call 
                        if (currentUpload.UploadId == -1 && !uploadDirExists)
                        {
                            currentUpload.ModuleId = ModuleContext.ModuleId;
                            currentUpload.CreatedByUserId = User.UserID;
                            currentUpload.CreatedOnDate = DateTime.UtcNow;
                            currentUpload.LastUpdatedByUserId = User.UserID;
                            currentUpload.LastUpdatedOnDate = DateTime.UtcNow;

                            UploadDataRepository.Instance.CreateItem(currentUpload);
                        }
                        
                        if (!uploadDirExists)
                        {
                            Directory.CreateDirectory(uploadPath);
                        }
                        else
                        {
                            currentUpload = UploadDataRepository.Instance.GetItems(currentUpload.GUID, ModuleContext.ModuleId).First();
                        }

                        foreach (string s in Request.Files)
                        {
                            UploadFile fileData = new UploadFile();
                            var file = Request.Files[s];
                            fileData.UploadId = currentUpload.UploadId;
                            fileData.FileName = file.FileName;
                            fileData.FileSize = file.ContentLength / 1024; // Filesize in KB
                            fileData.MimeType = file.ContentType;

                            if (file.ContentLength > 0)
                            {
                                string fileName = Path.GetFileName(file.FileName);
                                var path = Path.Combine(uploadPath, fileName);
                                file.SaveAs(path);
                            }

                            // DB entry to UploadFiles
                            UploadFilesRepository.Instance.CreateItem(fileData);
                        }

                        /*
                        MailInfo mailInfo = new MailInfo();
                        mailInfo.From = "upload@wiremayr.com";
                        mailInfo.To = "office@wiremayr.com";
                        mailInfo.Subject = "Upload notification";
                        mailInfo.Body = "Successful upload " + currentUpload.GUID.ToString();

                        MailKitMailProvider.Instance().SendMail(mailInfo);
                        */


                        // all good: upload in DB, files uploaded and each file in DB
                        return Content(currentUpload.UploadId.ToString());
                    }
                    // Model is valid, but no files have been sent
                    else
                    {   
                        Response.StatusCode = 400;                        
                        return Content("Filess");
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("An error occurred in processing the upload. Exception: {0}", ex);
                    // Any other exceptions
                    Response.StatusCode = 400;
                    return Content("Exception");
                }
            }
            // Model validation failed
            else
            {
                string valErrors = "";
                foreach (var item in ModelState)
                {
                    if (item.Value.Errors.Count > 0)
                        valErrors += item.Key + ";;";
                }
                Response.StatusCode = 400;
                return Content(valErrors);
            }

            //return Redirect(Url.Action("Confirmation", "UploadData", currentUpload));
            //return View(items);
            //return RedirectToAction("Index", "UploadData", new { model = currentUpload });
        }
    }
}
