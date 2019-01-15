using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DotNetNuke.UI.UserControls;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;

namespace R7.News.Stream
{
    public partial class ModuleSettings
    {
        protected CheckBox checkShowAllNews;
        protected TermsSelector termsIncludeTerms;
        protected TextBox textThumbnailWidth;
        protected CheckBox checkUseShowMore;
        protected CheckBox checkShowTopPager;
        protected CheckBox checkShowBottomPager;
        protected TextBox textPageSize;
        protected TextBox textMaxPageLinks;
        protected DropDownList comboMinThematicWeight;
        protected DropDownList comboMaxThematicWeight;
        protected DropDownList comboMinStructuralWeight;
        protected DropDownList comboMaxStructuralWeight;
        protected CheckBox chkEnableFeed;
    }
}
