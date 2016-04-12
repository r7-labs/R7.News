<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModuleSettings.ascx.cs" Inherits="R7.News.Stream.ModuleSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.News/R7.News.Stream/admin.css" Priority="200" />
<div id="news-stream-settings" class="dnnForm dnnClear">
    <h2 class="dnnFormSectionHead dnnClear"><a href="#"><%: LocalizeString ("sectionBasic.Text") %></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label id="labelIncludeTerms" runat="server" ControlName="termsIncludeTerms" />
            <dnn:TermsSelector id="termsIncludeTerms" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelShowAllNews" runat="server" ControlName="checkShowAllNews" />
            <asp:CheckBox id="checkShowAllNews" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelThematicWeightFilter" runat="server" ControlName="comboMinThematicWeight" />
            <asp:DropDownList id="comboMinThematicWeight" runat="server" Style="width:auto" />
            <span style="padding-right:.5em;padding-left:.5em">&ndash;</span>
            <asp:DropDownList id="comboMaxThematicWeight" runat="server" Style="width:auto" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelStructuralWeightFilter" runat="server" ControlName="comboMinStructuralWeight" />
            <asp:DropDownList id="comboMinStructuralWeight" runat="server" Style="width:auto" />
            <span style="padding-right:.5em;padding-left:.5em">&ndash;</span>
            <asp:DropDownList id="comboMaxStructuralWeight" runat="server" Style="width:auto" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelThumbnailWidth" runat="server" ControlName="textThumbnailWidth" />
            <asp:TextBox id="textThumbnailWidth" runat="server" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="textThumbnailWidth"
                CssClass="dnnFormMessage dnnFormError" Display="Dynamic" resourcekey="ThumbnailWidth.Required" />
            <asp:RangeValidator runat="server" ControlToValidate="textThumbnailWidth" 
                Type="Integer" MinimumValue="1" MaximumValue="4096"
                CssClass="dnnFormMessage dnnFormError" Display="Dynamic" resourcekey="ThumbnailWidth.Invalid" />
        </div>
        <hr />
        <div class="dnnFormItem">
            <div class="dnnLabel"></div>
            <asp:LinkButton id="buttonImport" runat="server" CssClass="dnnSecondaryAction" OnClick="buttonImport_Click" Text="Import" />
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
</div>