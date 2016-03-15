<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModuleSettings.ascx.cs" Inherits="R7.News.Stream.ModuleSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.News/R7.News.Stream/admin.css" Priority="200" />
<div class="dnnForm dnnClear">
    <fieldset>  
        <div class="dnnFormItem">
            <dnn:Label id="labelShowAllNews" runat="server" ControlName="checkShowAllNews" />
            <asp:CheckBox id="checkShowAllNews" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelIncludeTerms" runat="server" ControlName="termsIncludeTerms" />
            <dnn:TermsSelector id="termsIncludeTerms" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelThumbnailWidth" runat="server" ControlName="textThumbnailWidth" />
            <asp:TextBox id="textThumbnailWidth" runat="server" />
            <asp:RegularExpressionValidator runat="server" resourcekey="ThumbnailWidth.Invalid"
                ControlToValidate="textThumbnailWidth" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" 
                ValidationExpression="^0*[1-9]\d*$" />
        </div>
    </fieldset> 
</div>
