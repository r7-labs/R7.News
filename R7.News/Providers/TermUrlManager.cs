//
//  TermUrlManager.cs
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
using System.Collections.ObjectModel;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Common;
using DotNetNuke.Services.Log.EventLog;
using R7.News.Components;
using Assembly = System.Reflection.Assembly;

namespace R7.News.Providers
{
    public class TermUrlManager
    {
        #region Singleton implementation

        private static readonly Lazy<TermUrlManager> instance = 
            new Lazy<TermUrlManager> (() => new TermUrlManager ());

        public static TermUrlManager Instance
        {
            get { return instance.Value; }
        }

        private TermUrlManager ()
        {
            LoadProviders ();
        }

        #endregion

        private readonly Collection<ITermUrlProvider> providers = 
            new Collection<ITermUrlProvider> ();
        
        private static readonly char [] colon = { ':' };

        private void LoadProviders ()
        {
            foreach (var providerEntry in NewsConfig.Instance.TermUrlProviders) {
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
                    if (string.IsNullOrEmpty (assemblyName)){
                        assembly = Assembly.GetExecutingAssembly ();
                    }
                    else {
                        assembly = Assembly.LoadFrom (
                            Path.Combine (Globals.ApplicationMapPath, "bin", assemblyName)
                        );
                    }

                    var type = assembly.GetType (typeName);
                    var provider = Activator.CreateInstance (type) as ITermUrlProvider;
                    providers.Add (provider);
                }
                catch (Exception ex) {
                    var logController = new ExceptionLogController ();
                    logController.AddLog (ex);
                }
            }
        }

        public string GetUrl (int termId)
        {
            var termController = new TermController ();
            foreach (var provider in providers) {
                var url = provider.GetUrl (termId, termController);
                if (!string.IsNullOrEmpty (url)) {
                    return url;
                }
            }

            return string.Empty;
        }

        public string GetUrl (Term term)
        {
            foreach (var provider in providers) {
                var url = provider.GetUrl (term);
                if (!string.IsNullOrEmpty (url)) {
                    return url;
                }
            }

            return string.Empty;
        }


    }
}
