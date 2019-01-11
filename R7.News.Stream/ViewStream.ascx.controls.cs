using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.UI.UserControls;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;
using PagingControl = R7.Dnn.Extensions.Controls.PagingControl.PagingControl;
using R7.News.Controls;

namespace R7.News.Stream
{
    public partial class ViewStream
    {
        protected Panel panelStream;
        protected ListView listStream;
        protected PagingControl pagerTop;
        protected PagingControl pagerBottom;
        protected LinkButton buttonShowMore;
        protected AgplSignature agplSignature;
    }
}
