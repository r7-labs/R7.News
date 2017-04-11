//
//  NewsModuleBase.cs
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

using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using R7.DotNetNuke.Extensions.Modules;
using R7.News.Components;
using R7.News.Controls.Models;
using R7.News.Models;

namespace R7.News.Modules
{
    public abstract class NewsModuleBase<TSettings>: PortalModuleBase<TSettings>, IActionable
        where TSettings: class, new ()
    {
        #region IActionable implementation

        public virtual ModuleActionCollection ModuleActions {
            get {
                var actions = new ModuleActionCollection ();
                actions.Add (
                    GetNextActionID (),
                    LocalizeString ("AddNewsEntry.Action"),
                    ModuleActionType.AddContent,
                    "",
                    IconController.IconURL ("Add"),
                    EditUrl ("EditNewsEntry"),
                    false,
                    SecurityAccessLevel.Edit,
                    true,
                    false
                );

                return actions;
            }
        }

        #endregion

        public IList<NewsEntryAction> GetNewsEntryActions (INewsEntry newsEntry)
        {
            var actions = new List<NewsEntryAction> ();
            var discussionStarted = !string.IsNullOrEmpty (newsEntry.DiscussProviderKey);
            if (!discussionStarted) {
                var discussProvider = NewsConfig.Instance.GetDiscussProviders ().FirstOrDefault ();
                if (discussProvider != null) {
                    actions.Add (new NewsEntryAction {
                        EntryId = newsEntry.EntryId,
                        Action = NewsEntryActions.StartDiscussion,
                        Params = new string [] { discussProvider.ProviderKey },
                        Enabled = Request.IsAuthenticated
                    });
                }
            }
            else {
                var discussProvider = NewsConfig.Instance.GetDiscussProviders ()
                                                .FirstOrDefault (dp => dp.ProviderKey == newsEntry.DiscussProviderKey);
                if (discussProvider != null) {
                    actions.Add (new NewsEntryAction {
                        EntryId = newsEntry.EntryId,
                        Action = NewsEntryActions.JoinDiscussion,
                        Params = new string [] {
                            discussProvider.ProviderKey,
                            discussProvider.GetReplyCount (newsEntry.DiscussEntryId).ToString ()
                        },
                        Enabled = true
                    });
                }
            }

            return actions;
        }
    }
}
