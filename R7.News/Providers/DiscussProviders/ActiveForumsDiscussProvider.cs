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
using System.IO;
using System.Reflection;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;
using R7.News.Models;
using Assembly = System.Reflection.Assembly;
using System.Collections.Generic;

namespace R7.News.Providers.DiscussProviders
{
    public class ActiveForumsDiscussProvider : IDiscussProvider
    {
        static Assembly ForumAssembly;

        public IList<string> Params { get; set; }

        static ActiveForumsDiscussProvider ()
        {
            var dnnBinPath = Path.Combine (Globals.ApplicationMapPath, "bin");
            ForumAssembly = ReflectionHelper.TryLoadAssembly (Path.Combine (dnnBinPath, "DotNetNuke.Modules.ActiveForums.dll"));
        }

        public bool IsAvailable {
            get { return ForumAssembly != null; }
        }

        public int Discuss (INewsEntry newsEntry, int portalId, int userId)
        {
            try {
                if (IsAvailable) {
                    var connectorType = ForumAssembly.GetType ("DotNetNuke.Modules.ActiveForums.API.Content", true);

                    var connector = ReflectionHelper.New (connectorType);
                    var postMethod = ReflectionHelper.TryGetMethod (connectorType, "Topic_QuickCreate", BindingFlags.Instance | BindingFlags.Public);

                    var forumParams = ForumDiscussParams.Parse (Params);

                    var result = postMethod.Invoke (connector, new object [] {
                        portalId,
                        forumParams.ModuleId,
                        forumParams.ForumId,
                        newsEntry.Title,
                        newsEntry.Description,
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

        public string GetDiscussUrl (int discussId)
        {
            var forumParams = ForumDiscussParams.Parse (Params);
            return Globals.NavigateURL (forumParams.TabId, string.Empty,
                                        "forumId", forumParams.ForumId.ToString (),
                                        "postId", discussId.ToString ());
        }
    }
}