﻿using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DotNetNuke.UI.UserControls;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;

namespace R7.News.Agent
{
    public partial class ModuleSettings
    {
        protected CheckBox checkEnableGrouping;
        protected DropDownList comboGroupEntry;
        protected TextBox textThumbnailWidth;
        protected TextBox textGroupThumbnailWidth;
        protected TextBox txtImageCssClass;
        protected TextBox txtTextCssClass;
        protected TextBox txtTopEntryTextCssClass;
        protected TextBox txtImageColumnCssClass;
        protected TextBox txtTextColumnCssClass;
    }
}
