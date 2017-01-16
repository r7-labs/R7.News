//
//  AnnouncementInfo.cs
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
using DotNetNuke.ComponentModel.DataAnnotations;

namespace R7.News.Integrations.AnnoView
{
    [TableName ("Announcements")]
    internal class AnnouncementInfo
    {
        #region Properties

        public int ItemId { get; set; }

        public int ModuleId { get; set; }

        public bool Export { get; set; }

        public string ImageSource { get; set; }

        public int ViewOrder { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int CreatedByUser { get; set; }

        public DateTime PublishDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ExpireDate { get; set; }

        #endregion
    }
}

