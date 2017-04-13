﻿//
//  ActiveForumsDiscussProvider.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2017 Roman M. Yagodin
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using R7.News.Models;
using Assembly = System.Reflection.Assembly;

namespace R7.News.Providers.DiscussProviders
{
    public class ActiveForumsDiscussProvider : IDiscussProvider
    {
        static readonly Assembly forumAssembly;

        public IList<string> Params { get; set; }

        public string ProviderKey { get; set; }

        static ActiveForumsDiscussProvider ()
        {
            var dnnBinPath = Path.Combine (Globals.ApplicationMapPath, "bin");
            forumAssembly = ReflectionHelper.TryLoadAssembly (Path.Combine (dnnBinPath, "DotNetNuke.Modules.ActiveForums.dll"));
        }

        public bool IsAvailable {
            get { return forumAssembly != null; }
        }

        public string Discuss (INewsEntry newsEntry, int portalId, int userId)
        {
            try {
                if (IsAvailable) {
                    var connectorType = forumAssembly.GetType ("DotNetNuke.Modules.ActiveForums.API.Content", true);

                    var connector = ReflectionHelper.New (connectorType);
                    var postMethod = ReflectionHelper.TryGetMethod (connectorType, "Topic_QuickCreate", BindingFlags.Instance | BindingFlags.Public);

                    var forumParams = ForumDiscussParams.Parse (Params);

                    var result = postMethod.Invoke (connector, new object [] {
                        portalId,
                        forumParams.ModuleId,
                        forumParams.ForumId,
                        newsEntry.Title,
                        FormatMessage (newsEntry, forumParams.TabId, forumParams.ModuleId),
                        userId,
                        UserController.Instance.GetUserById (portalId, userId).DisplayName,
                        true, // IsApproved
                        "0.0.0.0"
                    });

                    var postId = (int) result;

                    return postId.ToString ();
                }
            } catch (Exception ex) {
                var log = new LogInfo ();
                log.Exception = new ExceptionInfo (ex);
                log.LogPortalID = portalId;
                log.LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString ();
                EventLogController.Instance.AddLog (log);
            }

            return null;
        }

        public string GetDiscussUrl (string discussEntryId)
        {
            var forumParams = ForumDiscussParams.Parse (Params);
            return Globals.NavigateURL (forumParams.TabId, string.Empty,
                                        "forumId", forumParams.ForumId.ToString (),
                                        "postId", discussEntryId);
        }

        public int GetReplyCount (string discussEntryId)
        {
            try {
                using (IDataContext dataContext = DataContext.Instance ()) {
                    return dataContext.ExecuteScalar<int> (
                        CommandType.Text,
                        @"SELECT ReplyCount FROM {databaseOwner}[{objectQualifier}activeforums_Topics]
                            WHERE TopicId = @0",
                        discussEntryId
                    );
                }
            } catch {
                return -1;
            }
        }

        protected string FormatMessage (INewsEntry newsEntry, int tabId, int moduleId)
        {
            var label = Localization.GetString ("ReadMore.Text", "~/DesktopModules/R7.News/R7.News/App_LocalResources");
            return HtmlUtils.StripTags (HttpUtility.HtmlDecode (newsEntry.Description), true) +
                            $"\n\n{label}: {newsEntry.GetUrl (tabId, moduleId)}";
        }
    }
}