//
//  Badges.ascx.cs
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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Modules;
using R7.News.Controls.ViewModels;
using DotNetNuke.R7.ViewModels;
using System.Web.UI.HtmlControls;

namespace R7.News.Controls
{
    public class Badge
    {
        public string Text { get; set; }

        public string CssClass { get; set; }
    }

    public class BadgeList: UserControl
    {
        #region Controls

        protected ListView listBadges;

        #endregion

        #region Public properties

        public List<Badge> DataSource { get; set; }

        public string CssClass { get; set; }

        public string BadgeCssClass { get; set; }

        #endregion
       
        public override void DataBind ()
        {
            if (DataSource != null) {
                listBadges.DataSource = DataSource;
                listBadges.DataBind ();
            }

            base.DataBind ();
        }
    }
}

