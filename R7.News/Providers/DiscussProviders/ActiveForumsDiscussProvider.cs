//
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
using System.IO;
using System.Reflection;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;
using Assembly = System.Reflection.Assembly;

namespace R7.News.Providers.DiscussProviders
{
    public class ActiveForumsDiscussProvider : IDiscussProvider
    {
        static Assembly ForumAssembly;

        static ActiveForumsDiscussProvider ()
        {
            var dnnBinPath = Path.Combine (Globals.ApplicationMapPath, "bin");
            ForumAssembly = ReflectionHelper.TryLoadAssembly (Path.Combine (dnnBinPath, "DotNetNuke.Modules.ActiveForums.dll"));
        }

        public bool IsAvailable {
            get { return ForumAssembly != null; }
        }

        public int AddPost (string postSubject, string postBody, int tabId, int moduleId, int portalId, int userId, int forumId, List<Term> terms)
        {
            try {
                if (IsAvailable) {
                    var connectorType = ForumAssembly.GetType ("DotNetNuke.Modules.ActiveForums.API.Content", true);

                    var connector = ReflectionHelper.New (connectorType);
                    var postMethod = ReflectionHelper.TryGetMethod (connectorType, "Topic_QuickCreate", BindingFlags.Instance | BindingFlags.Public);

                    var result = postMethod.Invoke (connector, new object [] {
                        portalId,
                        moduleId,
                        forumId,
                        postSubject,
                        postBody,
                        userId,
                        UserController.Instance.GetUserById (portalId, userId).DisplayName,
                        true, // IsApproved
                        "0.0.0.0"
                    });

                    var postId = (int) result;

                    return postId;
                }
            } catch (Exception ex) {
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
                                        "postId", postId.ToString ());
        }
    }
}