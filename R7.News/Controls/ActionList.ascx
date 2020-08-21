<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ActionButtons.ascx.cs" Inherits="R7.News.Controls.ActionList" %>
<%@ Register TagPrefix="news" TagName="ActionButtons" Src="~/DesktopModules/R7.News/R7.News/Controls/ActionButtons.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="DotNetNuke.Common.Utilities" %>
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/R7.News/R7.News/assets/js/news.js" />
<ul class="list-inline news-entry-actions">
	<li runat="server" class="list-inline-item" Visible="<%# IsEditable %>">
		<div class="btn-group" role="group" aria-label='<%: LocalizeString ("EditActions.Text") %>'>
			<a class="btn btn-sm btn-outline-secondary" href='<%# HttpUtility.HtmlAttributeEncode (EditUrl ()) %>'
					title='<%: LocalizeString ("EditNewsEntry.Text") %>'>
				<i class="fas fa-pencil-alt"></i>
			</a>
			<button id="btnDropMoreEditActions" runat="server" type="button" class="btn btn-sm btn-outline-secondary dropdown-toggle"
					title='<%# LocalizeString ("MoreEditActions.Text") %>'
					data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
				<i class="fas fa-cog"></i>
			</button>
			<div class="dropdown-menu" aria-labelledby="<%# btnDropMoreEditActions.ClientID %>">
				<asp:LinkButton runat="server" role="button" CssClass="dropdown-item" Visible="<%# ShowDuplicateAction %>"
						OnCommand="btnExecuteAction_Command" CommandName="Duplicate" CommandArgument="<%# JsonExtensionsWeb.ToJson(DuplicateAction) %>">
					<i class="far fa-clone"></i>
					<%# LocalizeString ("Duplicate.Text") %>
				</asp:LinkButton>
				<div class="dropdown-divider" runat="server" Visible="<%# ShowSyncTabAction %>"></div>
				<asp:LinkButton runat="server" role="button" CssClass="dropdown-item" Visible="<%# ShowSyncTabAction %>"
						OnCommand="btnExecuteAction_Command" CommandName="SyncTab" CommandArgument="<%# JsonExtensionsWeb.ToJson(SyncTabAction) %>">
					<i class="fas fa-sync"></i>
					<%# LocalizeString ("SyncTab.Text") %>
				</asp:LinkButton>
			</div>
		</div>
	</li>
	<li runat="server" class="list-inline-item" Visible="<%# ShowExpandTextAction && EntryTextId != null %>">
		<button type="button" class="btn btn-sm btn-outline-secondary"
				onclick="r7_news.expandText(this,<%# EntryTextId %>,<%# DnnContext.Module.ModuleId %>)">
			<%# LocalizeString ("btnExpandText.Text") %>
		</button>
	</li>
	<news:ActionButtons id="actionButtons" runat="server" />
</ul>
