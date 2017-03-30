//
//  TermLinksViewModel.cs
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

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Portals;
using R7.DotNetNuke.Extensions.ViewModels;
using R7.News.Providers.TermUrlProviders;

namespace R7.News.Controls.ViewModels
{
    public class TermLinksViewModel
    {
        public ViewModelContext Context { get; protected set; }

        public Term Term { get; protected set; }

        public int TermId
        {
            get { return Term.TermId; }    
        }

        public string Name
        {
            get { return Term.Name; }
        }

        public string Url
        {
            get {
                var url = TermUrlManager.GetUrl (Term);
                if (!string.IsNullOrEmpty (url)) {
                    int tabId;
                    if (int.TryParse (url, out tabId)) {
                        // url contains tabId
                        return Globals.NavigateURL (tabId);
                    }

                    // url of another type
                    return Globals.LinkClick (url, Context.Module.TabId, Context.Module.ModuleId);
                }

                var searchTabId = PortalSettings.Current.SearchTabId;
                if (!Null.IsNull (searchTabId)) {
                    // return term search link
                    return Globals.NavigateURL (searchTabId) + "?Tag=" + Term.Name;
                }

                // TODO: Should not display hyperlink at all
                return string.Empty;
            }
        }

        public TermLinksViewModel (Term term, ViewModelContext context)
        {
            Term = term;
            Context = context;
        }
    }
}

