//
//  ActionHandler.cs
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
using System.Linq;
using System.Web;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;
using R7.News.Data;

namespace R7.News.Components
{
    public class ActionHandler
    {
        public void ExecuteAction (string actionKey, int entryId, int portalId, int userId)
        {
            try {
                if (actionKey.StartsWith ("Discuss", StringComparison.InvariantCulture)) {
                    ExecuteDiscussAction (actionKey, entryId, portalId, userId);
                }
            }
            catch (Exception ex) {
                var log = new LogInfo ();
                log.Exception = new ExceptionInfo (ex);
                log.LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString ();
                log.LogPortalID = portalId;
                log.AddProperty ("Message", $"Cannot execute {actionKey} action");
                EventLogController.Instance.AddLog (log);
            }
        }

        public void ExecuteDiscussAction (string actionKey, int entryId, int portalId, int userId)
        {
            var newsEntry = NewsRepository.Instance.GetNewsEntry (entryId, portalId);
            var discussProvider = NewsConfig.Instance.GetDiscussProviders ().FirstOrDefault (dp => dp.ActionKey == actionKey);
            if (newsEntry != null && discussProvider != null) {
                var discussId = discussProvider.Discuss (newsEntry, portalId, userId);
                if (discussId > 0) {
                    // TODO: Remember discuss was added for news entry
                    HttpContext.Current.Response.Redirect (discussProvider.GetDiscussUrl (discussId), false);
                }
                else {
                    var log = new LogInfo ();
                    log.LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString ();
                    log.LogPortalID = portalId;
                    log.AddProperty ("Message", $"Cannot add discussion for news entry");
                    EventLogController.Instance.AddLog (log);
                }
            }
        }
    }
}
