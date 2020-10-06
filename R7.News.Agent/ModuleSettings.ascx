<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModuleSettings.ascx.cs" Inherits="R7.News.Agent.ModuleSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web.Deprecated" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.News/R7.News.Agent/admin.css" Priority="200" />
<div class="dnnForm dnnClear">
	<!--
	<h2 class="dnnFormSectionHead dnnClear"><a href="#"><%: LocalizeString ("sectionBasic.Text") %></a></h2>
    <fieldset>
    </fieldset>
	-->
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
            <dnn:Label id="lblTopEntryTextCssClass" runat="server" ControlName="txtTopEntryTextCssClass" />
            <asp:TextBox id="txtTopEntryTextCssClass" runat="server" />
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
</div>
