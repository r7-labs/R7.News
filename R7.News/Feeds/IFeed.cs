
//
//  IFeed.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2019 Roman M. Yagodin
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

using System.Collections.Generic;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using R7.News.Models;

namespace R7.News.Feeds
{
    public interface IFeed
    {
        void Render (HtmlTextWriter writer, IEnumerable<NewsEntryInfo> newsEntries, ModuleInfo module, PageBase page);
    }
}
