using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DotNetNuke.UI.UserControls;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;

namespace R7.News.Stream
{
    public partial class EditNewsEntry
    {
        protected LinkButton buttonUpdate;
        protected LinkButton buttonDelete;
        protected HyperLink linkCancel;
        protected ModuleAuditControl ctlAudit;
        protected TextBox textTitle;
        protected TextEditor textDescription;
        protected DnnDateTimePicker datetimeThresholdDate;
        protected DnnDateTimePicker datetimeDueDate;
        protected DnnDateTimePicker datetimeStartDate;
        protected DnnDateTimePicker datetimeEndDate;
        protected TermsSelector termsTerms;
        protected DnnFilePickerUploader pickerImage;
        protected DnnUrlControl urlUrl;
        protected TextBox sliderThematicWeight;
        protected TextBox sliderStructuralWeight;
        protected LinkButton buttonGetModules;
        protected GridView gridModules;
        protected TextBox textPermalinkRaw;
        protected TextBox textPermalinkFriendly;
    }
}
