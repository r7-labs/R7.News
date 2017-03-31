<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ActionButtons.ascx.cs" Inherits="R7.News.Controls.ActionButtons" %>
<div class="<%: CssClass %>">
    <asp:ListView id="listActionButtons" runat="server" ItemType="R7.News.Controls.ViewModels.NewsEntryAction">
        <LayoutTemplate>
            <ul runat="server" class="list-inline">
                <li runat="server" id="itemPlaceholder"></li>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
				<asp:LinkButton id="linkActionButton" runat="server" CssClass="btn btn-sm btn-default"
				    Enabled="<%# Item.Enabled %>" Visible="<%# Item.Visible %>"
                    role="button" aria-disabled="<%# !Item.Enabled %>"
					Text="<%# LocalizeString (Item.ActionKey) %>"
					CommandName="<%# Item.ActionKey %>" CommandArgument="<%# Item.EntryId %>" OnCommand="linkActionButton_Command" />
            </li>
        </ItemTemplate>
    </asp:ListView>
</div>