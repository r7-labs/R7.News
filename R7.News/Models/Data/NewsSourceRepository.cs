//
//  NewsSourceRepository.cs
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
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using DotNetNuke.Common;
using R7.News.Components;
using R7.News.Models;

namespace R7.News.Models.Data
{
    public class NewsSourceRepository
    {
        #region Singleton implementation

        private static readonly Lazy<NewsSourceRepository> instance = 
            new Lazy<NewsSourceRepository> (() => new NewsSourceRepository ());

        public static NewsSourceRepository Instance
        {
            get { return instance.Value; }
        }

        private NewsSourceRepository ()
        {
            LoadNewsSources ();
        }

        #endregion

        private readonly ConcurrentDictionary<int,INewsSourceProvider> NewsSourceProviders = 
            new ConcurrentDictionary<int,INewsSourceProvider> ();

        private void LoadNewsSources ()
        {
            var newsSources = NewsDataProvider.Instance.GetObjects<NewsSourceInfo> ("WHERE Type IS NOT NULL");

            foreach (var newsSource in newsSources) {
                try {
                    
                    System.Reflection.Assembly assembly;
                    if (string.IsNullOrWhiteSpace (newsSource.Assembly)){
                        assembly = System.Reflection.Assembly.GetExecutingAssembly ();
                    }
                    else {
                        assembly = System.Reflection.Assembly.LoadFrom (
                            Path.Combine (Globals.ApplicationMapPath, "bin", newsSource.Assembly)
                        );
                    }

                    var type = assembly.GetType (newsSource.Type);
                    var newsSourceProvider = Activator.CreateInstance (type) as INewsSourceProvider;
                    NewsSourceProviders.GetOrAdd (newsSource.SourceId, newKey => newsSourceProvider);
                }
                catch (Exception ex) {
                    throw new Exception ("Cannot load type '" + newsSource.Type
                        + "' from assembly '" + newsSource.Assembly + "'", ex);

                }
            }
        }

        public INewsSource GetSource (int? sourceId, int? sourceItemId)
        {
            if (sourceId != null) {
                INewsSourceProvider newsSourceProvider;
                if (NewsSourceProviders.TryGetValue (sourceId.Value, out newsSourceProvider)) {
                    return newsSourceProvider.GetSource (sourceItemId);
                }
            }

            return null;
        }

        public IEnumerable<INewsSource> GetSources (int? sourceId)
        {
            if (sourceId != null) {
                INewsSourceProvider newsSourceProvider;
                if (NewsSourceProviders.TryGetValue (sourceId.Value, out newsSourceProvider)) {
                    return newsSourceProvider.GetSources ();
                }
            }

            return Enumerable.Empty<INewsSource> ();
        }

        public IList<INewsSource> GetSelfSources ()
        {
            lock (NewsSourceProviders) {
                return NewsSourceProviders.Select (nsp => nsp.Value.GetSelfSource ()).ToList ();
            }
        }
    }
}

