﻿using System;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;
using R7.Dnn.Extensions.Text;
using R7.News.Data;
using R7.News.Models;
using R7.News.Providers.DiscussProviders;
using R7.News.Controls.Models;

namespace R7.News.Components
{
    public class ActionHandler
    {
        static readonly object discussLock = new object ();

        protected HttpResponse Response = HttpContext.Current.Response;

        public void ExecuteAction (NewsEntryAction action, int portalId, int tabId, int? userId = null)
        {
            try {
                switch (action.Action) {
                    case NewsEntryActions.Duplicate:
                        Duplicate (action.EntryId, portalId, tabId, action.ModuleId);
                        break;
                    case NewsEntryActions.SyncTab:
                        SyncTab (action.EntryId, portalId, tabId);
                        break;
                    case NewsEntryActions.StartDiscussion:
                        StartDiscussion (action.EntryId, portalId, userId.Value, action.Params [0]);
                        break;
                    case NewsEntryActions.JoinDiscussion:
                        JoinDiscussion (action.EntryId, portalId);
                        break;
                }
            }
            catch (Exception ex) {
                var log = new LogInfo ();
                log.Exception = new ExceptionInfo (ex);
                log.LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString ();
                log.LogPortalID = portalId;
                log.AddProperty ("Message", $"Cannot execute {action.Action} action");
                EventLogController.Instance.AddLog (log);
            }
        }

        protected void Duplicate (int entryId, int portalId, int tabId, int moduleId)
        {
            var newsEntry = NewsRepository.Instance.GetNewsEntry (entryId, portalId).WithText ();
            if (newsEntry != null) {
                NewsRepository.Instance.DuplicateNewsEntry ((NewsEntry) newsEntry, moduleId, tabId);
            }
            Response.Redirect (Globals.NavigateURL ());
        }

        protected void SyncTab (int entryId, int portalId, int tabId)
        {
            var newsEntry = NewsRepository.Instance.GetNewsEntry (entryId, portalId);
            if (newsEntry != null) {
                var tabCtrl = new TabController ();
                var activeTab = tabCtrl.GetTab (tabId, portalId, false);
                new TabSynchronizer ().UpdateTabFromNewsEntry (activeTab, newsEntry);
            }
            Response.Redirect (Globals.NavigateURL ());
        }

        protected void StartDiscussion (int entryId, int portalId, int userId, string providerKey)
        {
            lock (discussLock) {
                var newsEntry = NewsRepository.Instance.GetNewsEntry (entryId, portalId);
                if (newsEntry != null && string.IsNullOrEmpty (newsEntry.DiscussProviderKey)) {
                    var discussProvider = GetDiscussProviderByKey (providerKey);
                    if (discussProvider != null) {
                        var discussEntryId = discussProvider.Discuss (newsEntry, portalId, userId);
                        if (!string.IsNullOrEmpty (discussEntryId)) {
                            newsEntry.DiscussProviderKey = discussProvider.ProviderKey;
                            newsEntry.DiscussEntryId = discussEntryId;
                            NewsRepository.Instance.UpdateNewsEntry (newsEntry);
                            RedirectToDiscussion (newsEntry, discussProvider);
                            return;
                        }
                        else {
                            LogAdminAlert ($"Error adding discussion for news entry using {providerKey} provider", portalId);
                        }
                    }
                    else {
                        LogAdminAlert ($"Cannot add discussion for news entry, {providerKey} provider does not exists", portalId);
                    }
                }
            }
            Response.Redirect (Globals.NavigateURL ());
        }

        protected void JoinDiscussion (int entryId, int portalId)
        {
            var newsEntry = NewsRepository.Instance.GetNewsEntry (entryId, portalId);
            if (newsEntry != null && !string.IsNullOrEmpty (newsEntry.DiscussProviderKey)) {
                var discussProvider = GetDiscussProviderByKey (newsEntry.DiscussProviderKey);
                if (discussProvider != null) {
                    RedirectToDiscussion (newsEntry, discussProvider);
                    return;
                }
                else {
                    LogAdminAlert ($"Cannot redirect to discussion, {newsEntry.DiscussProviderKey} provider does not exists", portalId);
                }
            }
            Response.Redirect (Globals.NavigateURL ());
        }

        protected IDiscussProvider GetDiscussProviderByKey (string providerKey)
        {
            return NewsConfig.Instance.GetDiscussProviders ().FirstOrDefault (dp => dp.ProviderKey == providerKey);
        }

        protected void RedirectToDiscussion (INewsEntry newsEntry, IDiscussProvider discussProvider)
        {
            Response.Redirect (discussProvider.GetDiscussUrl (newsEntry.DiscussEntryId), false);
        }

        protected void LogAdminAlert (string message, int portalId)
        {
            var log = new LogInfo ();
            log.LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString ();
            log.LogPortalID = portalId;
            log.AddProperty ("Message", message);
            EventLogController.Instance.AddLog (log);
        }
    }
}

