//
//  NewsConfig.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016 Roman M. Yagodin
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
using System.IO;
using System.Collections.Concurrent;
using DotNetNuke.Entities.Portals;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using DotNetNuke.Common;

namespace R7.News.Components
{
    public static class NewsConfig
    {
        #region Singleton implementation

        private static readonly ConcurrentDictionary<int,Lazy<NewsPortalConfig>> portalConfigs = 
            new ConcurrentDictionary<int,Lazy<NewsPortalConfig>> ();

        public static NewsPortalConfig Instance
        {
            get {
                var portalId = PortalSettings.Current.PortalId;

                var lazyPortalConfig = portalConfigs.GetOrAdd (portalId, newKey => 
                    new Lazy<NewsPortalConfig> (() => {

                        var portalConfigFile = Path.Combine (PortalSettings.Current.HomeDirectoryMapPath, "R7.News.yml");

                        // ensure portal config file exists
                        if (!File.Exists (portalConfigFile)) {
                            File.Copy (Path.Combine (Globals.ApplicationMapPath, "DesktopModules\\R7.News\\R7.News\\R7.News.yml"), 
                                portalConfigFile);
                        }

                        using (var configReader = new StringReader (File.ReadAllText (portalConfigFile))) {
                            var deserializer = new Deserializer (namingConvention: new HyphenatedNamingConvention ());
                            return deserializer.Deserialize<NewsPortalConfig> (configReader);
                        }
                    }
                 ));
                
                return lazyPortalConfig.Value;
            }
        }

        #endregion
    }
}
