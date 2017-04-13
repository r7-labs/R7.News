//
//  DnnForumDiscussProvider.cs
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
using DotNetNuke.Data;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using R7.News.Models;
using Assembly = System.Reflection.Assembly;

namespace R7.News.Providers.DiscussProviders
{
    internal enum DnnForumPostMessage
    {
        ForumClosed = 0,
        ForumDoesntExist = 1,
        ForumIsParent = 2,
        ForumNoAttachments = 3,
        PostApproved = 4,
        PostEditExpired = 5,
        PostInvalidBody = 6,
        PostInvalidSubject = 7,
        PostModerated = 8,
        ThreadLocked = 9,
        UserAttachmentPerms = 10,
        UserBanned = 11,
        UserCannotEditPost = 12,
        UserCannotPostReply = 13,
        UserCannotStartThread = 14,
        UserCannotViewForum = 15,
    }

    public class DnnForumDiscussProvider : IDiscussProvider
    {
        static readonly Assembly forumAssembly;

        static readonly Assembly forumLibraryAssembly;

        public IList<string> Params { get; set; }

        public string ProviderKey { get; set; }

        static DnnForumDiscussProvider ()
        {
            var dnnBinPath = Path.Combine (Globals.ApplicationMapPath, "bin");
            forumAssembly = ReflectionHelper.TryLoadAssembly (Path.Combine (dnnBinPath, "DotNetNuke.Modules.Forum.dll"));
            forumLibraryAssembly = ReflectionHelper.TryLoadAssembly (Path.Combine (dnnBinPath, "DotNetNuke.Forum.Library.dll"));
        }

        public bool IsAvailable {
            get { return forumAssembly != null && forumLibraryAssembly != null; }
        }

        public string Discuss (INewsEntry newsEntry, int portalId, int userId)
        {
            try {
                if (IsAvailable) {
                    var connectorType = forumAssembly.GetType ("DotNetNuke.Modules.Forum.PostConnector", true);
                    var submitResultType = forumLibraryAssembly.GetType ("DotNetNuke.Forum.Library.Data.SubmitPostResult", true);

                    var connector = ReflectionHelper.New (connectorType);
                    var postMethod = ReflectionHelper.TryGetMethod (connectorType, "SubmitExternalPost", BindingFlags.Instance | BindingFlags.Public, 13);

                    var forumParams = ForumDiscussParams.Parse (Params);

                    var result = postMethod.Invoke (connector, new object [] {
                            forumParams.TabId,
                            forumParams.ModuleId,
                            portalId,
                            userId,
                            newsEntry.Title,
                            FormatMessage (newsEntry, forumParams.TabId, forumParams.ModuleId),
                            forumParams.ForumId,
                            0, // ParentPostID
                            null, // string of attachments
                            "R7.News", // provider
                            -1, // ParentThreadID
                            newsEntry.ContentItem.Terms,
                            true // IsPinned
                        });

                    var postId = (int) submitResultType.GetField ("PostId").GetValue (result);
                    // var postMessage = (DnnForumPostMessage) submitResultType.GetField ("Result").GetValue (result);

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
                                        "threadId", discussEntryId,
                                        "scope", "posts");
        }

        public int GetReplyCount (string discussEntryId)
        {
            try {
                using (IDataContext dataContext = DataContext.Instance ()) {
                    return dataContext.ExecuteScalar<int> (
                        CommandType.Text,
                        @"SELECT Replies FROM {databaseOwner}[{objectQualifier}Forum_Threads]
                            WHERE ThreadID = (SELECT ThreadID FROM {databaseOwner}[{objectQualifier}Forum_Posts] WHERE PostID = @0)",
                        discussEntryId
                    );
                }
            }
            catch {
                return -1;
            }
        }

        protected string FormatMessage (INewsEntry newsEntry, int tabId, int moduleId)
        {
            var label = Localization.GetString ("ReadMore.Text", "DesktopModules/R7.News/R7.News/App_LocalResources/SharedResources.resx");
            return HttpUtility.HtmlEncode ($"<p>{newsEntry.PublishedOnDate ().ToShortDateString ()}</p>") + newsEntry.Description
                              + HttpUtility.HtmlEncode ($"<p><a href=\"{newsEntry.GetUrl (tabId, moduleId)}\" target=\"_blank\">{label}...</a></p>");
        }       
    }
}
