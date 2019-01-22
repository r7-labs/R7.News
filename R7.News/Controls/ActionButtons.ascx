<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ActionButtons.ascx.cs" Inherits="R7.News.Controls.ActionButtons" %>
<%@ Import Namespace="DotNetNuke.Common.Utilities" %>
<asp:ListView id="listActionButtons" runat="server" ItemType="R7.News.Controls.ViewModels.NewsEntryActionViewModel">
    <LayoutTemplate>
        <li runat="server" id="itemPlaceholder"></li>
    </LayoutTemplate>
    <ItemTemplate>
        <li>
			<asp:LinkButton runat="server" role="button" 
				Enabled="<%# Item.Enabled %>"
			    aria-disabled="<%# (!Item.Enabled).ToString ().ToLowerInvariant () %>"
				CssClass='<%# "btn btn-sm btn-default" + (Item.Enabled? string.Empty : " disabled") %>'
				Text='<%# Item.Text %>' ToolTip='<%# Item.Title %>'
				CommandName="<%# Item.Action %>" CommandArgument="<%# JsonExtensionsWeb.ToJson (Item) %>" OnCommand="linkActionButton_Command" />
        </li>
    </ItemTemplate>
</asp:ListView>
