//
//  NewsPortalConfig.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016 Roman M. Yagodin
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

using System.Collections.ObjectModel;
using R7.News.Providers.TermUrlProviders;

namespace R7.News.Components
{
    public class NewsPortalConfig
    {
        public int DataCacheTime { get; set; }

        public string DefaultImagesPath { get; set; }

        public StreamModuleConfig StreamModule { get; set; }

        public AgentModuleConfig AgentModule { get; set; }

        public NewsEntryConfig NewsEntry { get; set; }

        public DiscussOnForumConfig DiscussOnForum { get; set; }

        #region TermUrlProviders

        public Collection<string> TermUrlProviders { get; set; }

        protected readonly Collection<ITermUrlProvider> TermUrlProvidersInternal = new Collection<ITermUrlProvider> ();

        public Collection<ITermUrlProvider> GetTermUrlProviders ()
        {
            return TermUrlProvidersInternal;
        }

        public void AddTermUrlProvider (ITermUrlProvider provider)
        {
            TermUrlProvidersInternal.Add (provider);
        }

        #endregion
    }

    public class NewsEntryConfig
    {
        public int MaxWeight { get; set; }

        public int DefaultThematicWeight { get; set; }

        public int DefaultStructuralWeight { get; set; }
    }

    public class StreamModuleConfig
    {
        public int DefaultThumbnailWidth { get; set; }
    }

    public class AgentModuleConfig
    {
        public int DefaultThumbnailWidth { get; set; }

        public int DefaultGroupThumbnailWidth { get; set; }
    }

    public enum PermalinkMode
    {
        Friendly,
        Raw
    }

    // TODO: Move to Integrations.Forum namespace
    public enum ForumProvider
    {
        None,
        DnnForum,
        ActiveForums,
        YAF
    }

    public class DiscussOnForumConfig
    {
        public ForumProvider ForumProvider { get; set; }

        public int TabId { get; set; }

        public int ModuleId { get; set; }

        public int ForumId { get; set; }
    }
}
