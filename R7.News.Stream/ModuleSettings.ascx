<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModuleSettings.ascx.cs" Inherits="R7.News.Stream.ModuleSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web.Deprecated" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.News/R7.News.Stream/admin.css" Priority="200" />
<div id="news-stream-settings" class="dnnForm dnnClear">
    <h2 class="dnnFormSectionHead dnnClear"><a href="#"><%: LocalizeString ("sectionBasic.Text") %></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label id="labelIncludeTerms" runat="server" ControlName="termsIncludeTerms" />
            <dnn:TermsSelector id="termsIncludeTerms" CssClass="terms-include-terms" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelShowAllNews" runat="server" ControlName="checkShowAllNews" />
            <asp:CheckBox id="checkShowAllNews" runat="server" CssClass="check-show-all-news" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelThematicWeightFilter" runat="server" ControlName="comboMinThematicWeight" />
            <asp:DropDownList id="comboMinThematicWeight" runat="server" CssClass="dnnSmallSizeComboBox" />
            <span>&ndash;</span>
            <asp:DropDownList id="comboMaxThematicWeight" runat="server" CssClass="dnnSmallSizeComboBox" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelStructuralWeightFilter" runat="server" ControlName="comboMinStructuralWeight" />
            <asp:DropDownList id="comboMinStructuralWeight" runat="server" CssClass="dnnSmallSizeComboBox" />
            <span>&ndash;</span>
            <asp:DropDownList id="comboMaxStructuralWeight" runat="server" CssClass="dnnSmallSizeComboBox" />
        </div>
    </fieldset>
	<h2 class="dnnFormSectionHead dnnClear"><a href="#"><%: LocalizeString ("sectionAppearance.Text") %></a></h2>
    <fieldset>
		<div class="dnnFormItem">
            <dnn:Label id="labelThumbnailWidth" runat="server" ControlName="textThumbnailWidth" />
            <asp:TextBox id="textThumbnailWidth" runat="server" />
            <asp:RangeValidator runat="server" ControlToValidate="textThumbnailWidth" 
                Type="Integer" MinimumValue="1" MaximumValue="4096"
                CssClass="dnnFormMessage dnnFormError" Display="Dynamic" resourcekey="ThumbnailWidth.Invalid" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblImageCssClass" runat="server" ControlName="txtImageCssClass" />
            <asp:TextBox id="txtImageCssClass" runat="server" />
        </div>
		<div class="dnnFormItem">
            <dnn:Label id="lblTextCssClass" runat="server" ControlName="txtTextCssClass" />
            <asp:TextBox id="txtTextCssClass" runat="server" />
        </div>
		<div class="dnnFormItem">
            <dnn:Label id="lblImageColumnCssClass" runat="server" ControlName="txtImageColumnCssClass" />
            <asp:TextBox id="txtImageColumnCssClass" runat="server" />
        </div>
		<div class="dnnFormItem">
            <dnn:Label id="lblTextColumnCssClass" runat="server" ControlName="txtTextColumnCssClass" />
            <asp:TextBox id="txtTextColumnCssClass" runat="server" />
        </div>
	</fieldset>	
    <h2 class="dnnFormSectionHead dnnClear"><a href="#"><%: LocalizeString ("sectionPagination.Text") %></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label id="labelUseShowMore" runat="server" ControlName="checkUseShowMore" />
            <asp:CheckBox id="checkUseShowMore" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelShowTopPager" runat="server" ControlName="checkShowTopPager" />
            <asp:CheckBox id="checkShowTopPager" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelShowBottomPager" runat="server" ControlName="checkShowBottomPager" />
            <asp:CheckBox id="checkShowBottomPager" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelPageSize" runat="server" ControlName="textPageSize" />
            <asp:TextBox id="textPageSize" runat="server" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="textPageSize"
                CssClass="dnnFormMessage dnnFormError" Display="Dynamic" resourcekey="PageSize.Required" />
            <asp:RangeValidator runat="server" ControlToValidate="textPageSize" 
                Type="Integer" MinimumValue="1" MaximumValue="100"
                CssClass="dnnFormMessage dnnFormError" Display="Dynamic" resourcekey="PageSize.Invalid" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelMaxPageLinks" runat="server" ControlName="textMaxPageLinks" />
            <asp:TextBox id="textMaxPageLinks" runat="server" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="textMaxPageLinks"
                CssClass="dnnFormMessage dnnFormError" Display="Dynamic" resourcekey="MaxPageLinks.Required" />
            <asp:RangeValidator runat="server" ControlToValidate="textMaxPageLinks"
                Type="Integer" MinimumValue="1" MaximumValue="100"
                CssClass="dnnFormMessage dnnFormError" Display="Dynamic" resourcekey="MaxPageLinks.Invalid" />
        </div>
    </fieldset>
	<h2 class="dnnFormSectionHead dnnClear"><a href="#"><%: LocalizeString ("sectionFeed.Text") %></a></h2>
	<fieldset>
        <div class="dnnFormItem">
            <dnn:Label id="lblEnableFeed" runat="server" ControlName="chkEnableFeed" />
            <asp:CheckBox id="chkEnableFeed" runat="server" />
        </div>
		<div class="dnnFormItem">
            <dnn:Label id="lblFeedMaxEntries" runat="server" ControlName="txtFeedMaxEntries" />
            <asp:TextBox id="txtFeedMaxEntries" runat="server" />
        </div>
	</fieldset>
</div>