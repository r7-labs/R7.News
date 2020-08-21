<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EditNewsEntry.ascx.cs" Inherits="R7.News.Stream.EditNewsEntry" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/labelcontrol.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx"%>
<%@ Register TagPrefix="dnn" TagName="UrlControl" Src="~/controls/DnnUrlControl.ascx"%>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Picker" Src="~/controls/filepickeruploader.ascx" %>
<%@ Register TagPrefix="news" TagName="AgplSignature" Src="~/DesktopModules/R7.News/R7.News/Controls/AgplSignature.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web.Deprecated" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.News/R7.News/assets/css/module.css" />
<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.News/R7.News/assets/css/admin.css" />
<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.News/R7.News.Stream/admin.css" Priority="200" />
<div class="dnnForm dnnClear edit-newsentry">
    <div id="editNewsEntry_Tabs">
        <ul class="dnnAdminTabNav dnnClear">
            <li><a href="#editNewsEntry_commonTab"><%= LocalizeString("Common.Tab") %></a></li>
			<li><a href="#editNewsEntry_textTab"><%= LocalizeString("Text_Tab.Text") %></a></li>
            <li><a href="#editNewsEntry_termsAndWeigthsTab"><%= LocalizeString("TermsAndWeights.Tab") %></a></li>
            <li><a href="#editNewsEntry_advancedTab"><%= LocalizeString("Advanced.Tab") %></a></li>
			<li><a href="#editNewsEntry_auditTab"><%= LocalizeString("Audit.Tab") %></a></li>
        </ul>
        <div id="editNewsEntry_commonTab">
            <fieldset>
                <div class="dnnFormItem dnnFormRequired newsentry-title">
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
					<div class="dnnLabel"></div>
					<asp:LinkButton id="btnDefaultImagesPath" runat="server" resourcekey="btnDefaultImagesPath.Text"
						CssClass="btn btn-sm btn-outline-secondary" OnClick="btnDefaultImagesPath_Click" CausesValidation="false" />
				</div>
                <div class="dnnFormItem newsentry-description">
                    <dnn:Label id="labelDescription" runat="server" ControlName="textDescription" />
                    <dnn:TextEditor id="textDescription" runat="server" ChooseMode="false" />
                </div>
				<div class="dnnFormItem mb-3">
                    <dnn:Label id="labelStartDate" runat="server" ControlName="datetimeStartDate" />
                    <dnn:DnnDateTimePicker id="datetimeStartDate" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="labelEndDate" runat="server" ControlName="datetimeEndDate" />
                    <dnn:DnnDateTimePicker id="datetimeEndDate" runat="server" />
                </div>
            </fieldset>
        </div>
		<div id="editNewsEntry_textTab">
			<fieldset>
				<div class="dnnFormItem">
					<dnn:Label id="lblText" runat="server" ControlName="txtText" />
					<dnn:TextEditor id="txtText" runat="server" ChooseMode="false" />
				</div>
			</fieldset>
		</div>
        <div id="editNewsEntry_termsAndWeigthsTab">
            <fieldset>
                <div class="dnnFormItem mb-3">
                    <dnn:Label id="lblTerms" runat="server" ControlName="selTerms" />
                    <asp:ListBox id="selTerms" runat="server" SelectionMode="Multiple"
						DataValueField="TermId" DataTextField="Name"
						CssClass="dnn-select2" Style="width:100%" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="labelThematicWeight" runat="server" ControlName="sliderThematicWeight" />
                    <asp:TextBox id="sliderThematicWeight" runat="server" CssClass="dnnSliderInput" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="labelStructuralWeight" runat="server" ControlName="sliderStructuralWeight" />
                    <asp:TextBox id="sliderStructuralWeight" runat="server" CssClass="dnnSliderInput" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="labelModules" runat="server" ControlName="buttonGetModules" />
                    <asp:LinkButton id="buttonGetModules" runat="server" resourcekey="buttonGetModules.Text"
						CssClass="btn btn-sm btn-outline-secondary"
                        CausesValidation="false" OnClick="buttonGetModules_Click" />
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
                            <asp:BoundField DataField="ModuleLink" HeaderText="Module.Column" HtmlEncode="false" />
                            <asp:BoundField DataField="PassesByString" HeaderText="PassesBy.Column" />
                        </Columns>
                    </asp:GridView>
                </div>
            </fieldset>
        </div>
        <div id="editNewsEntry_advancedTab">
            <fieldset>
				<div class="dnnFormItem">
                    <dnn:Label id="lblAgentModule" runat="server" ControlName="txtAgentModuleId" />
					<asp:TextBox id="txtAgentModuleId" runat="server" ReadOnly="true" />
                </div>
                <div class="dnnFormItem mb-3">
                    <dnn:Label id="labelThresholdDate" runat="server" ControlName="datetimeThresholdDate" />
                    <dnn:DnnDateTimePicker id="datetimeThresholdDate" runat="server" />
                </div>
                <div class="dnnFormItem mb-3">
                    <dnn:Label id="labelDueDate" runat="server" ControlName="datetimeDueDate" />
                    <dnn:DnnDateTimePicker id="datetimeDueDate" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="labelUrl" runat="server" ControlName="ctlUrl" />
                    <dnn:UrlControl id="ctlUrl" runat="server"
                        IncludeActiveTab="true"
                        UrlType="N"
                        ShowNone="true"
                        ShowLog="false"
                        ShowTrack="false"
                    />
					<asp:CheckBox id="chkCurrentPage" runat="server" resourcekey="CurrentPage.Text" />
                </div>
            </fieldset>
        </div>
		<div id="editNewsEntry_auditTab">
            <fieldset>
			    <div class="dnnFormItem">
                    <dnn:Label id="labelPermalinks" runat="server" ControlName="textPermalinkFriendly" />
                    <asp:TextBox id="textPermalinkFriendly" runat="server" ReadOnly="true" />
                </div>
				<div class="dnnFormItem">
                    <div class="dnnLabel"></div>
					<asp:TextBox id="textPermalinkRaw" runat="server" ReadOnly="true" />
                </div>
				<div class="dnnFormItem">
                    <dnn:Label id="labelDiscussionLink" runat="server" ControlName="textDiscussionLink" />
					<asp:TextBox id="textDiscussionLink" runat="server" ReadOnly="true" />
                </div>
				<div class="dnnFormItem">
                    <label class="dnnLabel"></label>
                    <asp:LinkButton id="buttonClearDiscussionLink" runat="server" Visible="false"
                        CssClass="btn btn-outline-danger" ResourceKey="buttonClearDiscussionLink"
						OnClick="buttonClearDiscussionLink_Click" />
                </div>
				<asp:HiddenField id="hiddenDiscussProviderKey" runat="server" />
				<asp:HiddenField id="hiddenDiscussEntryId" runat="server" />
				<div class="dnnFormItem">
                    <dnn:Label id="labelAudit" runat="server" ControlName="ctlAudit" />
                    <dnn:Audit id="ctlAudit" runat="server" />
                </div>
			</fieldset>
        </div>
    </div>
    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton id="buttonUpdate" runat="server" CssClass="dnnPrimaryAction" CausesValidation="true" /></li>
		<li>&nbsp;</li>
		<li><asp:LinkButton id="buttonDelete" runat="server" CssClass="btn btn-danger" ResourceKey="cmdDelete" /></li>
		<li>&nbsp;</li>
        <li><asp:HyperLink id="linkCancel" runat="server" CssClass="btn btn-outline-secondary" ResourceKey="cmdCancel" /></li>
	</ul>
	<news:AgplSignature runat="server" ShowRule="false" />
</div>
<input id="hiddenSelectedTab" type="hidden" value="<%= (int) SelectedTab %>" />
<script type="text/javascript">
(function($, Sys) {
    function setupModule() {
        var selectedTab = document.getElementById("hiddenSelectedTab").value;
        $("#editNewsEntry_Tabs").dnnTabs({selected: selectedTab});
        $(".edit-newsentry .dnnSliderInput").each(function(){
            $(this).dnnSliderInput({min: -1, max: this.getAttribute ("data-max")});
        });
		$(".dnn-select2").select2();
    };
    $(document).ready(function() {
        setupModule();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function() {
            setupModule();
        });
    });
} (jQuery, window.Sys));
</script>
