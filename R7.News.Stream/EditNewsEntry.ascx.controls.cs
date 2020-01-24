using System.Web.UI.WebControls;
using DotNetNuke.UI.UserControls;
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
        protected DnnUrlControl ctlUrl;
        protected TextBox sliderThematicWeight;
        protected TextBox sliderStructuralWeight;
        protected LinkButton buttonGetModules;
        protected GridView gridModules;
        protected TextBox textPermalinkRaw;
        protected TextBox textPermalinkFriendly;
        protected HiddenField hiddenDiscussProviderKey;
        protected HiddenField hiddenDiscussEntryId;
        protected TextBox textDiscussionLink;
        protected LinkButton buttonClearDiscussionLink;
        protected CheckBox chkCurrentPage;
        protected TextBox txtAgentModuleId;
    }
}
