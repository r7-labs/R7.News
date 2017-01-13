//
//  AgentSettings.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016-2017 Roman M. Yagodin
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using DotNetNuke.Entities.Modules.Settings;
using DotNetNuke.Entities.Portals;
using R7.News.Components;

namespace R7.News.Agent.Components
{
    /// <summary>
    /// Provides strong typed access to settings used by module
    /// </summary>
    [Serializable]
    public class AgentSettings
    {
        protected const string SettingPrefix = Const.Prefix + "_Agent_";

        public AgentSettings ()
        {
            ThumbnailWidth = NewsConfig.GetInstance (PortalSettings.Current.PortalId).AgentModule.DefaultThumbnailWidth;
            GroupThumbnailWidth = NewsConfig.GetInstance (PortalSettings.Current.PortalId).AgentModule.DefaultGroupThumbnailWidth;
        }

        [TabModuleSetting (Prefix = SettingPrefix)]
        public bool EnableGrouping { get; set; } = false;

        [TabModuleSetting (Prefix = SettingPrefix)]
        public int? GroupEntryId { get; set; }

        [TabModuleSetting (Prefix = SettingPrefix)]
        public bool HideImages { get; set; } = false;

        [TabModuleSetting (Prefix = SettingPrefix)]
        public int ThumbnailWidth { get; set; }

        [TabModuleSetting (Prefix = SettingPrefix)]
        public int GroupThumbnailWidth { get; set; }
    }
}

