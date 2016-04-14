<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EditNewsEntry.ascx.cs" Inherits="R7.News.Stream.EditNewsEntry" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/labelcontrol.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx"%>
<%@ Register TagPrefix="dnn" TagName="UrlControl" Src="~/controls/DnnUrlControl.ascx"%>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Picker" Src="~/controls/filepickeruploader.ascx" %> 
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.News/R7.News.Stream/admin.css" Priority="200" />
<div class="dnnForm dnnClear edit-newsentry">
    <div id="newsentry-tabs">
        <ul class="dnnAdminTabNav dnnClear">
            <li><a href="#newsentry-common-tab"><%= LocalizeString("Common.Tab") %></a></li>
            <li><a href="#newsentry-terms-and-weigths-tab"><%= LocalizeString("TermsAndWeights.Tab") %></a></li>
            <li><a href="#newsentry-advanced-tab"><%= LocalizeString("Advanced.Tab") %></a></li>
            <li><a href="#newsentry-audit-tab"><%= LocalizeString("Audit.Tab") %></a></li>
        </ul>
        <div id="newsentry-common-tab">
            <fieldset>
                <div class="dnnFormItem dnnFormRequired">
                    <dnn:Label id="labelTitle" runat="server" ControlName="textTitle" />
                    <asp:TextBox id="textTitle" runat="server" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="textTitle" 
                        CssClass="dnnFormMessage dnnFormError" Display="Dynamic" resourcekey="Title.Required" />
                    <asp:RegularExpressionValidator runat="server"
                        ControlToValidate="textTitle" ValidationExpression="[\s\S]{0,255}"
                        CssClass="dnnFormMessage dnnFormError" Display="Dynamic" resourcekey="Title.MaxLength" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="labelImage" runat="server" ControlName="pickerImage" />
                    <dnn:Picker id="pickerImage" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="labelDescription" runat="server" ControlName="textDescription" />
                    <dnn:TextEditor id="textDescription" runat="server" />
                </div>
            </fieldset>
        </div>
        <div id="newsentry-terms-and-weigths-tab">
            <fieldset>
                <div class="dnnFormItem">
                    <dnn:Label id="labelTerms" runat="server" ControlName="termsTerms" />
                    <dnn:TermsSelector id="termsTerms" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="labelThematicWeight" runat="server" ControlName="comboThematicWeight" />
                    <asp:DropDownList id="comboThematicWeight" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="labelStructuralWeight" runat="server" ControlName="comboStructuralWeight" />
                    <asp:DropDownList id="comboStructuralWeight" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="labelModules" runat="server" ControlName="buttonGetModules" />
                    <asp:LinkButton id="buttonGetModules" runat="server" resourcekey="buttonGetModules.Text" CssClass="dnnSecondaryAction"
                        OnClick="buttonGetModules_Click" />
                </div>
                <div class="dnnFormItem">
                    <div class="dnnLabel"></div>
                    <asp:GridView id="gridModules" runat="server" AutoGenerateColumns="false"
                            UseAccessibleHeader="true" CssClass="dnnGrid grid-modules" GridLines="None">
                        <HeaderStyle CssClass="dnnGridHeader" />
                        <RowStyle CssClass="dnnGridItem" />
                        <AlternatingRowStyle CssClass="dnnGridAltItem" />
                        <Columns>
                            <asp:BoundField DataField="ModuleId" Visible="false" />
                            <asp:BoundField DataField="ModuleLink" HeaderText="Module" HtmlEncode="false" />
                            <asp:BoundField DataField="PassesByString" HeaderText="PassesBy" />
                        </Columns>
                    </asp:GridView>
                </div>
            </fieldset>
        </div>
        <div id="newsentry-advanced-tab">
            <fieldset>
                <div class="dnnFormItem">
                    <dnn:Label id="labelStartDate" runat="server" ControlName="datetimeStartDate" />
                    <dnn:DnnDateTimePicker id="datetimeStartDate" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="labelEndDate" runat="server" ControlName="datetimeEndDate" />
                    <dnn:DnnDateTimePicker id="datetimeEndDate" runat="server" />
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
        </div>
        <div id="newsentry-audit-tab">
            <fieldset>
                <div class="dnnFormItem">
                    <dnn:Label id="labelPermalink" runat="server" ControlName="textPermalink" /> 
                    <asp:TextBox id="textPermalink" runat="server" ReadOnly="true" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="labelAudit" runat="server" ControlName="ctlAudit" /> 
                    <dnn:Audit id="ctlAudit" runat="server" />
                </div>
            </fieldset>
        </div>
    </div>
    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton id="buttonUpdate" runat="server" CssClass="dnnPrimaryAction" CausesValidation="true" /></li>
        <li><asp:LinkButton id="buttonDelete" runat="server" CssClass="dnnSecondaryAction" ResourceKey="cmdDelete" /></li>
        <li><asp:HyperLink id="linkCancel" runat="server" CssClass="dnnSecondaryAction cancel-action" ResourceKey="cmdCancel" /></li>
    </ul>
</div>
<input id="hiddenSelectedTab" type="hidden" value="<%= (int) SelectedTab %>" />
<script type="text/javascript">
(function($, Sys) {
    function setupModule() {
        var selectedTab = document.getElementById("hiddenSelectedTab").value;
        $("#newsentry-tabs").dnnTabs({selected: selectedTab});
    };

    $(document).ready(function() {
        setupModule();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function() {
            setupModule();
        });
    });
} (jQuery, window.Sys));
</script>