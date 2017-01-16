//
//  DescriptionTermUrlProvider.cs
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
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Common;
using DotNetNuke.Entities.Tabs;

namespace R7.News.Providers
{
    /// <summary>
    /// Basic term URL provider which assume what term have URL in the description
    /// </summary>
    public class DescriptionTermUrlProvider: ITermUrlProvider
    {
        #region ITermUrlProvider implementation

        public string GetUrl (Term term)
        {
            var url = term.Description;
            var urlType = Globals.GetURLType (url);
            if (urlType != TabType.Normal) {
                if (urlType != TabType.Url) {
                    return term.Description;
                }
                if (url.StartsWith ("mailto:", StringComparison.InvariantCultureIgnoreCase)
                    || url.IndexOf ("://", StringComparison.InvariantCultureIgnoreCase) >= 0
                    || url.StartsWith ("\\\\", StringComparison.InvariantCultureIgnoreCase)
                    || url.StartsWith ("/", StringComparison.InvariantCultureIgnoreCase)) {
                    return term.Description;
                }
            }

            return string.Empty;
        }

        public string GetUrl (int termId, TermController termController)
        {
            var term = termController.GetTerm (termId);

            if (term != null) {
                return GetUrl (term);
            }

            return string.Empty;
        }

        #endregion
    }
}

