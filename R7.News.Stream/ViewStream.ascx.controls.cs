using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Linq;
using DotNetNuke.UI.UserControls;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;
using PagingControl = DotNetNuke.R7.PagingControl;

namespace R7.News.Stream
{
    public partial class ViewStream
    {
        protected Panel panelStream;
        protected CheckBox checkShowDefaultHidden;
        protected ListView listStream;
        protected PagingControl pagerTop;
        protected PagingControl pagerBottom;
        protected LinkButton buttonShowMore;
    }
}
