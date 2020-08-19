<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="TermLinks.ascx.cs" Inherits="R7.News.Controls.TermLinks" %>
<ul runat="server" class="<%# CssClass %>" style="margin-left:inherit">
    <asp:ListView id="listTermLinks" runat="server" ItemType="R7.News.Controls.ViewModels.TermLinksViewModel">
	    <LayoutTemplate>
            <li runat="server" id="itemPlaceholder"></li>
        </LayoutTemplate>
        <ItemTemplate>
            <li class="list-inline-item">
                <a href="<%# Item.Url %>" rel="tag" class="badge badge-secondary"><%# Item.Name %></a>
            </li>
        </ItemTemplate>
    </asp:ListView>
</ul>
