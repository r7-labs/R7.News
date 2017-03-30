//
//  DescriptionTermUrlProvider.cs
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
using System.Text.RegularExpressions;
using DotNetNuke.Entities.Content.Taxonomy;

namespace R7.News.Providers.TermUrlProviders
{
    /// <summary>
    /// Basic term URL provider which assume what term have URL in the description
    /// </summary>
    public class DescriptionTermUrlProvider: ITermUrlProvider
    {
        #region ITermUrlProvider implementation

        public string GetUrl (Term term)
        {
            return IsUrl (term.Description) ? term.Description : string.Empty;
        }

        protected bool IsUrl (string url)
        {
            if (url.StartsWith ("mailto:", StringComparison.InvariantCultureIgnoreCase)
                || url.IndexOf ("://", StringComparison.InvariantCultureIgnoreCase) >= 0
                || url.StartsWith ("//", StringComparison.InvariantCultureIgnoreCase)
                || IsTabUrl (url)) {
                return true;
            }

            return false;
        }

        protected bool IsTabUrl (string url)
        {
            return Regex.IsMatch (url, @"^\d+$");
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

