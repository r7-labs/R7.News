//
//  AgentNewsEntryViewModel.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016-2020 Roman M. Yagodin
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

using R7.Dnn.Extensions.ViewModels;
using R7.News.Agent.Models;
using R7.News.Components;
using R7.News.Models;
using R7.News.ViewModels;

namespace R7.News.Agent.ViewModels
{
    public class AgentNewsEntryViewModel: NewsEntryViewModelBase
    {
        public AgentNewsEntryViewModel (INewsEntry newsEntry, ViewModelContext<AgentSettings> context, AgentModuleConfig config) :
            base (newsEntry, context)
        {
            Config = config;
        }

        protected AgentModuleConfig Config;

        protected AgentSettings Settings
        {
            get { return ((ViewModelContext<AgentSettings>) Context).Settings; }
        }

        public string ImageUrl
        {
            get { return NewsEntry.GetImageUrl (width: Settings.ThumbnailWidth ?? Config.DefaultThumbnailWidth); }
        }

        public string GroupImageUrl
        {
            get { return NewsEntry.GetImageUrl (width: Settings.GroupThumbnailWidth ?? Config.DefaultGroupThumbnailWidth); }
        }

        public string ImageCssClass => Settings.ImageCssClass ?? Config.ImageCssClass;

        public string TextCssClass => Settings.TextCssClass ?? Config.TextCssClass;

        public string ImageColumnCssClass
        {
            get { return HasImage ? (Settings.ImageColumnCssClass ?? Config.ImageColumnCssClass) : Const.NoImageColumnCssClass; }
        }

        public string TextColumnCssClass
        {
            get { return HasImage ? (Settings.TextColumnCssClass ?? Config.TextColumnCssClass) : Const.NoImageTextColumnCssClass; }
        }
    }
}

