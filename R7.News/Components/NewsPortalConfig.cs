//
//  NewsPortalConfig.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016-2017 Roman M. Yagodin
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
using System.Collections.ObjectModel;
using R7.News.Providers.DiscussProviders;
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

        #region TermUrl providers

        public Collection<TermUrlProviderConfig> TermUrlProviders { get; set; }

        protected readonly Collection<ITermUrlProvider> TermUrlProvidersInternal = new Collection<ITermUrlProvider> ();

        public Collection<ITermUrlProvider> GetTermUrlProviders ()
        {
            return TermUrlProvidersInternal;
        }

        #endregion

        #region Discuss providers

        public Collection<DiscussProviderConfig> DiscussProviders { get; set; }

        protected readonly Collection<IDiscussProvider> DiscussProviders_Internal = new Collection<IDiscussProvider> ();

        public Collection<IDiscussProvider> GetDiscussProviders ()
        {
            return DiscussProviders_Internal;
        }

        #endregion

        public void AddProvider<TProvider> (TProvider provider, IProviderConfig providerConfig)
        {
            if (provider is ITermUrlProvider) {
                TermUrlProvidersInternal.Add ((ITermUrlProvider)provider);
            }
            else if (provider is IDiscussProvider) {
                var discussProvider = (IDiscussProvider) provider;
                discussProvider.Params = providerConfig.Params;
                discussProvider.ProviderKey = ((DiscussProviderConfig) providerConfig).ProviderKey;
                DiscussProviders_Internal.Add (discussProvider);
            }
        }

        public NodeManipulatorConfig NodeManipulator { get; set; }
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

    public interface IProviderConfig
    {
        string Type { get; set; }

        List<string> Params { get; set; }
    }

    public class TermUrlProviderConfig : IProviderConfig
    {
        public string Type { get; set; }

        public List<string> Params { get; set; }
    }

    public class DiscussProviderConfig: IProviderConfig
    {
        public string Type { get; set; }

        public List<string> Params { get; set; }

        public string ProviderKey { get; set; }
    }

    public class NodeManipulatorConfig
    {
        public int ParentNodeTabId { get; set; }

        public int StreamModuleId { get; set; }

        public int StreamModuleTabId { get; set; }
    }
}
