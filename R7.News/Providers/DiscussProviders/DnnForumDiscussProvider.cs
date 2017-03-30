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
using System.IO;
using System.Reflection;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;
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

    public class DnnForumDiscussProvider: IDiscussProvider
    {
        static Assembly ForumAssembly;

        static Assembly ForumAssembly2;

        static DnnForumDiscussProvider ()
        {
            var dnnBinPath = Path.Combine (Globals.ApplicationMapPath, "bin");
            ForumAssembly = ReflectionHelper.TryLoadAssembly (Path.Combine (dnnBinPath, "DotNetNuke.Modules.Forum.dll"));
            ForumAssembly2 = ReflectionHelper.TryLoadAssembly (Path.Combine (dnnBinPath, "DotNetNuke.Forum.Library.dll"));
        }

        public bool IsAvailable {
            get { return ForumAssembly != null && ForumAssembly2 != null; }
        }

        public int AddPost (string postSubject, string postBody, int tabId, int moduleId, int portalId, int userId, int forumId, List<Term> terms)
        {
            try {
                if (IsAvailable) {
                    var connectorType = ForumAssembly.GetType ("DotNetNuke.Modules.Forum.PostConnector", true);
                    var submitResultType = ForumAssembly2.GetType ("DotNetNuke.Forum.Library.Data.SubmitPostResult", true);

                    var connector = ReflectionHelper.New (connectorType);
                    var postMethod = ReflectionHelper.TryGetMethod (connectorType, "SubmitExternalPost", BindingFlags.Instance | BindingFlags.Public, 13);

                    var result = postMethod.Invoke (connector, new object [] {
                            tabId,
                            moduleId,
                            portalId,
                            userId,
                            postSubject,
                            postBody,
                            forumId,
                            0, // ParentPostID
                            null, // string of attachments
                            "R7.News", // provider
                            -1, // ParentThreadID
                            terms,
                            true // IsPinned
                        });

                    var postId = (int) submitResultType.GetField ("PostId").GetValue (result);
                    var postMessage = (DnnForumPostMessage) submitResultType.GetField ("Result").GetValue (result);

                    return postId;
                }
            }
            catch (Exception ex) {
                var log = new LogInfo ();
                log.Exception = new ExceptionInfo (ex);
                log.LogPortalID = portalId;
                log.LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString ();
                EventLogController.Instance.AddLog (log);
            }

            return Null.NullInteger;
        }

        public string GetPostUrl (int tabId, int forumId, int postId)
        {
            return Globals.NavigateURL (tabId, string.Empty,
                                        "forumId", forumId.ToString (),
                                        "threadId", postId.ToString (),
                                        "scope", "posts");
        }
    }
}
