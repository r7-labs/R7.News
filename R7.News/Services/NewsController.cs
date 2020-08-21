using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using R7.News.Components;
using R7.News.Models;
using R7.News.ViewModels;
using R7.News.Data;

namespace R7.News.Services
{
    public class NewsController: DnnApiController
    {
        [HttpGet]
        [SupportedModules("R7.News.Stream,R7.News.Agent")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public HttpResponseMessage GetNewsEntryText (int entryTextId)
        {
            try {
                var newsEntryText = NewsRepository.Instance.GetNewsEntryText (entryTextId);
                if (newsEntryText != null) {
                    var jsonNewsEntryText = new JsonNewsEntryText {
                        EntryTextId = newsEntryText.EntryTextId,
                        EntryId = newsEntryText.EntryId,
                        Text = newsEntryText.Text
                    };
                    return Request.CreateResponse (jsonNewsEntryText);
                }

                return Request.CreateResponse (HttpStatusCode.NotFound);
            }
            catch (Exception ex) {
                Exceptions.LogException (ex);
                return Request.CreateErrorResponse (HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
