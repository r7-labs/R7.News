<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModuleSettings.ascx.cs" Inherits="R7.News.Agent.ModuleSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web.Deprecated" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.News/R7.News.Agent/admin.css" Priority="200" />
<div class="dnnForm dnnClear">
    <fieldset>
		<div class="dnnFormItem">
            <dnn:Label id="labelEnableGrouping" runat="server" ControlName="checkEnableGrouping" />
            <asp:CheckBox id="checkEnableGrouping" runat="server" />
        </div>
		<div class="dnnFormItem">
            <dnn:Label id="labelGroupEntry" runat="server" ControlName="comboGroupEntry" />
			<asp:DropDownList id="comboGroupEntry" runat="server"
                DataTextField="Title"
                DataValueField="EntryId"
            />
        </div>
		<div class="dnnFormItem">
            <dnn:Label id="labelHideImages" runat="server" ControlName="checkHideImages" />
            <asp:CheckBox id="checkHideImages" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelThumbnailWidth" runat="server" ControlName="textThumbnailWidth" />
            <asp:TextBox id="textThumbnailWidth" runat="server" />
            <asp:RangeValidator runat="server" ControlToValidate="textThumbnailWidth" 
                Type="Integer" MinimumValue="1" MaximumValue="4096"
                CssClass="dnnFormMessage dnnFormError" Display="Dynamic" resourcekey="ThumbnailWidth.Invalid" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelGroupThumbnailWidth" runat="server" ControlName="textGroupThumbnailWidth" />
            <asp:TextBox id="textGroupThumbnailWidth" runat="server" />
            <asp:RangeValidator runat="server" ControlToValidate="textGroupThumbnailWidth" 
                Type="Integer" MinimumValue="1" MaximumValue="1024"
                CssClass="dnnFormMessage dnnFormError" Display="Dynamic" resourcekey="GroupThumbnailWidth.Invalid" />
        </div>
    </fieldset>
</div>
