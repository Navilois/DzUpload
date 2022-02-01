
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Log.EventLog;
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

    public class ReCaptchaResponse
    {
        public bool Success { get; set; }
        public List<string> ErrorCodes { get; set; }
    }



    [DnnHandleError]
    public class UploadDataController : DnnController
    {
        private ILog _log;

        protected ILog Log
        {
            get { return _log ?? (_log = LoggerSource.Instance.GetLogger(this.GetType())); }
        }

        private ReCaptchaResponse ValidateRecaptcha(string encodedResponse, string privateKey)
        {
            var client = new WebClient();
            var googleResponse = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", privateKey, encodedResponse));
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Deserialize<ReCaptchaResponse>(googleResponse);
        }


        [HttpGet]
        public ActionResult Index()
        {
            this.DnnPage.ClientScript.RegisterStartupScript(this.GetType(), "Recaptcha v2 script", "<script src=\"https://www.google.com/recaptcha/api.js\" async defer></script>");
            return View(new UploadData());
        }


        [HttpGet]
        public ActionResult ThankYou(UploadData currentUpload)
        {

            /*
                MailInfo mailInfo = new MailInfo();
                mailInfo.From = "upload@wiremayr.com";
                mailInfo.To = "office@wiremayr.com";
                mailInfo.Subject = "Upload notification";
                mailInfo.Body = "Successful upload " + currentUpload.GUID.ToString();

                MailKitMailProvider.Instance().SendMailAsync(mailInfo);
            */


            //return Redirect(Url.Action("Confirmation", "UploadData", currentUpload));
            //return View(items);
            //return RedirectToAction("Index", "UploadData", new { model = currentUpload });


            return View(currentUpload);
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

            // Error codes
            // 1 => form validation error
            // 2 => no files
            // 3 => reCaptcha validation error
            // 4 => exception
            
            // Upload complete, redirect to confirmation page
            if (!string.IsNullOrEmpty(Request.Form["UploadComplete"]))
            {
                /*
                MailInfo mailInfo = new MailInfo();
                mailInfo.From = "upload@wiremayr.com";
                mailInfo.To = "office@wiremayr.com";
                mailInfo.Subject = "Upload notification";
                mailInfo.Body = "Successful upload " + currentUpload.GUID.ToString();

                MailKitMailProvider.Instance().SendMailAsync(mailInfo);
            */

                return View("ThankYou", currentUpload);
            }


            // Server side form validation failed -> return to form with valErrors
            if (!ModelState.IsValid)
            {
                string valErrors = "1;;";
                foreach (var item in ModelState)
                {
                    if (item.Value.Errors.Count > 0)
                        valErrors += item.Key + ";;";
                }
                Response.StatusCode = 400;
                return Content(valErrors);
            }


            // No files to upload -> return to form
            if (Request.Files.Count < 1)
            {
                Response.StatusCode = 400;
                return Content("2;;");
            }


            var objEventLog = new EventLogController();
            try
            {   
                string uploadPath = Server.MapPath("~/Portals/0/WM-Uploads/" + currentUpload.GUID + "/");
                bool uploadDirExists = Directory.Exists(uploadPath);

                // *** First call
                // - Validate reCAPTCHA
                // - DB entry to UploadData
                if (currentUpload.UploadId == -1 && !uploadDirExists)
                {
                    string captchaResponse = Request.Form["g-recaptcha-response"];

                    if (string.IsNullOrEmpty(captchaResponse))
                    {
                        // reCaptcha response not posted
                        Log.ErrorFormat("UploadDataController: reCaptcha validation failed -> g-recaptcha-response NullOrEmpty");
                        objEventLog.AddLog("UploadDataController: reCaptcha validation failed", "g-recaptcha-response NullOrEmpty", EventLogController.EventLogType.HOST_ALERT);
                        return Content("3;;");
                    }

                    // Verify client response with google api
                    ReCaptchaResponse resp = ValidateRecaptcha(captchaResponse, LocalizeString("ReCaptchaValKey"));
                    
                    if (!resp.Success)
                    {
                        foreach (string err in resp.ErrorCodes)
                        {
                            // Validation not successful -> log errors and return to form with valError
                            Log.ErrorFormat("UploadDataController: reCaptcha validation failed: {0}", err);
                            objEventLog.AddLog("UploadDataController: reCaptcha validation failed", err, EventLogController.EventLogType.HOST_ALERT);
                            return Content("3;;");
                        }
                    }

                    // Complete model and create DB entry
                    currentUpload.ModuleId = ModuleContext.ModuleId;
                    currentUpload.CreatedByUserId = User.UserID;
                    currentUpload.CreatedOnDate = DateTime.UtcNow;
                    currentUpload.LastUpdatedByUserId = User.UserID;
                    currentUpload.LastUpdatedOnDate = DateTime.UtcNow;

                    UploadDataRepository.Instance.CreateItem(currentUpload);
                }

                // Create dir or get (cached?) data for current GUID
                if (!uploadDirExists)
                {
                    Directory.CreateDirectory(uploadPath);
                }
                else
                {
                    currentUpload = UploadDataRepository.Instance.GetItems(currentUpload.GUID, ModuleContext.ModuleId).First();
                }


                // Upload files
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


                // all good: upload in DB, files uploaded and each file in DB
                return Content(currentUpload.UploadId.ToString());
            }
            catch (Exception ex)
            {
                // Any other exceptions
                Log.ErrorFormat("An error occurred in processing the upload. Exception: {0}", ex);
                objEventLog.AddLog("An error occurred in processing the upload. Exception: ", ex.Message, EventLogController.EventLogType.HOST_ALERT);
                
                Response.StatusCode = 400;
                return Content("4;;");
            }
        }
    }
}
