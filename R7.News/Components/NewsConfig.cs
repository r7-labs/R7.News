//
//  NewsConfig.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016-2019 Roman M. Yagodin
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
using System.Web.Compilation;
using DotNetNuke.Services.Exceptions;
using R7.Dnn.Extensions.Configuration;
using R7.News.Providers.DiscussProviders;
using R7.News.Providers.TermUrlProviders;

namespace R7.News.Components
{
    public static class NewsConfig
    {
        static readonly ExtensionYamlConfig<NewsPortalConfig> _config;

        static NewsConfig () => _config = new ExtensionYamlConfig<NewsPortalConfig> ("R7.News.yml", Init);

        public static NewsPortalConfig GetInstance (int portalId) => _config.GetInstance (portalId);

        public static NewsPortalConfig Instance => _config.Instance;

        static NewsPortalConfig Init (NewsPortalConfig portalConfig)
        {
            LoadProviders<ITermUrlProvider> (portalConfig, portalConfig.TermUrlProviders.Cast<IProviderConfig> ());
            LoadProviders<IDiscussProvider> (portalConfig, portalConfig.DiscussProviders.Cast<IProviderConfig> ());

            return portalConfig;
        }

        static void LoadProviders<TProvider> (NewsPortalConfig portalConfig, IEnumerable<IProviderConfig> providerConfigs)
        {
            foreach (var providerConfig in providerConfigs) {
                try {
                    var providerType = BuildManager.GetType (providerConfig.Type, true, true);
                    portalConfig.AddProvider ((TProvider) Activator.CreateInstance (providerType), providerConfig);
                }
                catch (Exception ex) {
                    Exceptions.LogException (ex);
                }
            }
        }
   }
}
