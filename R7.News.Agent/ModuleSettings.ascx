<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModuleSettings.ascx.cs" Inherits="R7.News.Agent.ModuleSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.News/R7.News.Agent/admin.css" Priority="200" />
<div class="dnnForm dnnClear">
    <fieldset>  
        <div class="dnnFormItem">
            <dnn:Label id="labelEnableGrouping" runat="server" ControlName="checkEnableGrouping" />
            <asp:CheckBox id="checkEnableGrouping" runat="server" />
        </div>
    </fieldset>
</div>
