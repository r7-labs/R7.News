<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EditNewsEntry.ascx.cs" Inherits="R7.News.Stream.EditNewsEntry" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/labelcontrol.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx"%>
<%@ Register TagPrefix="dnn" TagName="UrlControl" Src="~/controls/DnnUrlControl.ascx"%>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Picker" Src="~/controls/filepickeruploader.ascx" %> 
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.News/R7.News.Stream/admin.css" Priority="200" />
<div class="dnnForm dnnClear">
 <fieldset>  
    <div class="dnnFormItem">
        <dnn:Label id="labelImage" runat="server" ControlName="pickerImage" />
        <dnn:Picker id="pickerImage" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="labelTitle" runat="server" ControlName="textTitle" />
        <asp:TextBox id="textTitle" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="labelDescription" runat="server" ControlName="textDescription" />
        <dnn:TextEditor id="textDescription" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="labelThresholdDate" runat="server" ControlName="datetimeThresholdDate" />
        <dnn:DnnDateTimePicker id="datetimeThresholdDate" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="labelDueDate" runat="server" ControlName="datetimeDueDate" />
        <dnn:DnnDateTimePicker id="datetimeDueDate" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="labelSortIndex" runat="server" ControlName="textSortIndex" />
        <asp:TextBox id="textSortIndex" runat="server" Value="0" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="labelStartDate" runat="server" ControlName="datetimeStartDate" />
        <dnn:DnnDateTimePicker id="datetimeStartDate" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="labelEndDate" runat="server" ControlName="datetimeEndDate" />
        <dnn:DnnDateTimePicker id="datetimeEndDate" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="labelTerms" runat="server" ControlName="termsTerms" />
        <dnn:TermsSelector id="termsTerms" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="labelNewsSourceProvider" runat="server" ControlName="comboNewSourceProvider" />
        <asp:DropDownList id="comboNewsSourceProvider" runat="server" AutoPostBack="true"
            OnSelectedIndexChanged="comboNewsSourceProvider_SelectedIndexChanged"
            DataValueField="SourceId"
            DataTextField="Title"
        />
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="labelNewsSource" runat="server" ControlName="comboNewsSource" />
        <asp:DropDownList id="comboNewsSource" runat="server" 
            DataValueField="SourceItemId"
            DataTextField="Title"
        />
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="labelUrl" runat="server" ControlName="urlUrl" />
        <dnn:UrlControl id="urlUrl" runat="server"
            IncludeActiveTab="true"
            UrlType="N"
            ShowNone="true"
            ShowLog="false"
            ShowTrack="false"
        />
    </div>
 </fieldset>
 <ul class="dnnActions dnnClear">
     <li><asp:LinkButton id="buttonUpdate" runat="server" CssClass="dnnPrimaryAction" CausesValidation="true" /></li>
     <li><asp:LinkButton id="buttonDelete" runat="server" CssClass="dnnSecondaryAction" ResourceKey="cmdDelete" /></li>
     <li><asp:HyperLink id="linkCancel" runat="server" CssClass="dnnSecondaryAction" ResourceKey="cmdCancel" Style="margin-left:2em" /></li>
 </ul>
 <dnn:Audit id="ctlAudit" runat="server" />
</div>

