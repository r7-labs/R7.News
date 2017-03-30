//
//  NewsConfig.cs
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Log.EventLog;
using R7.News.Providers.DiscussProviders;
using R7.News.Providers.TermUrlProviders;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Assembly = System.Reflection.Assembly;

namespace R7.News.Components
{
    public static class NewsConfig
    {
        #region Singleton implementation

        private static readonly ConcurrentDictionary<int,Lazy<NewsPortalConfig>> portalConfigs = 
            new ConcurrentDictionary<int,Lazy<NewsPortalConfig>> ();

        public static NewsPortalConfig Instance
        {
            get { return GetInstance (PortalSettings.Current.PortalId); }
        }

        public static NewsPortalConfig GetInstance (int portalId)
        {
            var lazyPortalConfig = portalConfigs.GetOrAdd (portalId, newKey => 
                new Lazy<NewsPortalConfig> (() => {

                var portalSettings = new PortalSettings (portalId);
                var portalConfigFile = Path.Combine (portalSettings.HomeDirectoryMapPath, "R7.News.yml");

                // ensure portal config file exists
                if (!File.Exists (portalConfigFile)) {
                    File.Copy (Path.Combine (
                        Globals.ApplicationMapPath,
                        "DesktopModules\\R7.News\\R7.News\\R7.News.yml"), 
                        portalConfigFile);
                }

                using (var configReader = new StringReader (File.ReadAllText (portalConfigFile))) {
                    var deserializer = new Deserializer (namingConvention: new HyphenatedNamingConvention ());
                    var portalConfig = deserializer.Deserialize<NewsPortalConfig> (configReader);

                    LoadProviders<ITermUrlProvider> (portalConfig, portalConfig.TermUrlProviders.Cast<IProviderConfig> ());
                    LoadProviders<IDiscussProvider> (portalConfig, portalConfig.DiscussProviders.Cast<IProviderConfig> ());

                    return portalConfig;
                }
            }
            ));

            return lazyPortalConfig.Value;
        }

        #endregion

        static void LoadProviders<TProvider> (NewsPortalConfig portalConfig, IEnumerable<IProviderConfig> providerConfigs)
        {
            foreach (var providerConfig in providerConfigs) {
                try {
                    var providerType = GetProviderType (providerConfig.Type);
                    if (providerType != null) {
                        portalConfig.AddProvider ((TProvider)Activator.CreateInstance (providerType), providerConfig);
                    }
                }
                catch (Exception ex) {
                    var logController = new ExceptionLogController ();
                    logController.AddLog (ex);
                }
            }
        }

        static Type GetProviderType (string fullTypeName)
        {
            return LoadProviderAssembly (GetAssemblyName (fullTypeName)).GetType (GetProviderTypeName (fullTypeName));
        }

        static readonly char [] colon = { ':' };

        static string GetAssemblyName (string fullTypeName)
        {
            var fullTypeNameParts = fullTypeName.Split (colon, StringSplitOptions.RemoveEmptyEntries);
            if (fullTypeNameParts.Length == 2) {
                return fullTypeNameParts [0];
            }

            return null;
        }

        static string GetProviderTypeName (string fullTypeName)
        {
            var fullTypeNameParts = fullTypeName.Split (colon, StringSplitOptions.RemoveEmptyEntries);
            if (fullTypeNameParts.Length == 1) {
                return fullTypeNameParts [0];
            }

            if (fullTypeNameParts.Length == 2) {
                return fullTypeNameParts [1];
            }

            return null;
        }

        static Assembly LoadProviderAssembly (string assemblyName)
        {
            if (string.IsNullOrEmpty (assemblyName)) {
                return Assembly.GetExecutingAssembly ();
            }

            return Assembly.LoadFrom (Path.Combine (Globals.ApplicationMapPath, "bin", assemblyName));
        }
   }
}
