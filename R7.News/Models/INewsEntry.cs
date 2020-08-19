//
//  INewsEntry.cs
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
using System.Collections.Generic;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Modules;
using R7.News.Models;

namespace R7.News.Models
{
    public interface INewsEntry
    {
        int EntryId { get; set; }

        int PortalId { get; set; }

        int ContentItemId { get; set; }

        int? AgentModuleId { get; set; }

        string Url { get; set; }

        DateTime? StartDate { get; set; }

        DateTime? EndDate { get; set; }

        DateTime? ThresholdDate { get; set; }

        DateTime? DueDate { get; set; }

        string Title { get; set; }

        string Description { get; set; }

        int? EntryTextId { get; set; }

        string Text { get; set; }

        int ThematicWeight { get; set; }

        int StructuralWeight { get; set; }

        ContentItem ContentItem { get; set; }

        ModuleInfo AgentModule { get; set; }

        ICollection<INewsEntry> Group { get; set; }

        string DiscussProviderKey { get; set; }

        string DiscussEntryId { get; set; }
    }
}
