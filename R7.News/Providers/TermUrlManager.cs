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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using R7.News.Integrations;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Framework.Providers;

namespace R7.News.Providers
{
    public class TermUrlManager
    {
        private readonly Collection<ITermUrlProvider> providers = 
            new Collection<ITermUrlProvider> ();
        
        #region Singleton implementation

        private static readonly Lazy<TermUrlManager> instance = 
            new Lazy<TermUrlManager> (() => new TermUrlManager ());

        public static TermUrlManager Instance
        {
            get { return instance.Value; }
        }

        private TermUrlManager ()
        {
            providers.Add (new UniversityDivisionTermUrlProvider ());
            providers.Add (new DescriptionTermUrlProvider ());
        }

        #endregion

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
