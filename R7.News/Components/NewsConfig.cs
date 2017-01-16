//
//  NewsConfig.cs
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

using System;
using System.IO;
using System.Collections.Concurrent;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using DotNetNuke.Services.Log.EventLog;
using R7.News.Providers;
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

                    LoadTermUrlProviders (portalConfig);
                    return portalConfig;
                }
            }
                                   ));

            return lazyPortalConfig.Value;
        }

        #endregion

        private static readonly char [] colon = { ':' };

        private static void LoadTermUrlProviders (NewsPortalConfig portalConfig)
        {
            foreach (var providerEntry in portalConfig.TermUrlProviders) {
                try {
                    // parse config entry
                    var providerEntrySplitted = providerEntry.Split (colon, StringSplitOptions.RemoveEmptyEntries);
                    string assemblyName;
                    string typeName;
                    if (providerEntrySplitted.Length == 1) {
                        assemblyName = null;
                        typeName = providerEntrySplitted [0];
                    }
                    else if (providerEntrySplitted.Length == 2) {
                        assemblyName = providerEntrySplitted [0];
                        typeName = providerEntrySplitted [1];
                    }
                    else {
                        continue;
                    }

                    // load assembly and type
                    Assembly assembly;
                    if (string.IsNullOrEmpty (assemblyName)) {
                        assembly = Assembly.GetExecutingAssembly ();
                    }
                    else {
                        assembly = Assembly.LoadFrom (
                            Path.Combine (Globals.ApplicationMapPath, "bin", assemblyName)
                        );
                    }

                    var type = assembly.GetType (typeName);
                    var provider = Activator.CreateInstance (type) as ITermUrlProvider;
                    portalConfig.AddTermUrlProvider (provider);
                }
                catch (Exception ex) {
                    var logController = new ExceptionLogController ();
                    logController.AddLog (ex);
                }
            }
        }

    }
}
