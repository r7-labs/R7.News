//
//  NewsNodeManipulator.cs
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
using System.Linq;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Web.DDRMenu;
using R7.News.Components;
using R7.News.Data;
using R7.News.Models;

namespace R7.News.Integrations.DDRMenu
{
    public class NewsNodeManipulator : INodeManipulator
	{
    	#region INodeManipulator implementation

		public List<MenuNode> ManipulateNodes (List<MenuNode> nodes, PortalSettings portalSettings)
		{
            try {
                var config = NewsConfig.GetInstance (portalSettings.PortalId).NodeManipulator;

                var parentNode = nodes.FirstOrDefault (n => n.TabId == config.ParentTabId);
                if (parentNode != null) {

                    // TODO: Cache the result?
                    var newsEntries = NewsRepository.Instance.GetNewsEntries_FirstPage (
                        portalSettings.PortalId, config.NewsCount, DateTime.Now,
                        config.ThematicWeightRange, config.StructuralWeightRange
                    );

                    foreach (var newsEntry in newsEntries) {
                        parentNode.Children.Add (CreateMenuNode (newsEntry, parentNode, portalSettings));
                    }
                }
                else {
                    LogAdminAlert ($"Parent node with TabID={config.ParentTabId} not found.", portalSettings.PortalId);
                }
            }
            catch (Exception ex) {
                Exceptions.LogException (ex);
            }

			return nodes;
		}

        #endregion

        protected MenuNode CreateMenuNode (INewsEntry newsEntry, MenuNode parentNode, PortalSettings portalSettings)
        {
            var node = new MenuNode ();
            node.Enabled = true;
            node.Parent = parentNode;
            node.Text = newsEntry.Title;
            node.Title = newsEntry.Title;
            node.Description = HtmlUtils.StripTags (HttpUtility.HtmlDecode (newsEntry.Description), false);

            if (newsEntry.AgentModule != null) {
                node.TabId = newsEntry.AgentModule.TabID;
            }

            // TODO: Don't assume Stream module on the parent page
            var parentStreamModule = ModuleController.Instance.GetTabModules (parentNode.TabId)
                                                     .FirstOrDefault (m => m.Value.ModuleDefinition.DefinitionName == Const.StreamModuleDefinitionName).Value;
            if (parentStreamModule != null) {
                node.Url = newsEntry.GetUrl (parentNode.TabId, parentStreamModule.ModuleID);
            }
            else {
                LogAdminAlert ($"Cannot find Stream module on the parent page with TabID={parentNode.TabId}.", portalSettings.PortalId);
            }

            return node;
        }

        void LogAdminAlert (string message, int portalId)
        {
            var log = new LogInfo ();
            log.LogPortalID = portalId;
            log.LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString ();
            log.AddProperty (GetType ().ToString (), message);
            EventLogController.Instance.AddLog (log);
        }
	}
}