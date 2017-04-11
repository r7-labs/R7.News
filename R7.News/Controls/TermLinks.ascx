<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="TermLinks.ascx.cs" Inherits="R7.News.Controls.TermLinks" %>
<ul runat="server" class="<%# CssClass %>" style="margin-left:inherit">
    <asp:ListView id="listTermLinks" runat="server" ItemType="R7.News.Controls.ViewModels.TermLinksViewModel">
	    <LayoutTemplate>
            <li runat="server" id="itemPlaceholder"></li>
        </LayoutTemplate>
        <ItemTemplate>
            <li style="padding-left:inherit">
                <span class="glyphicon glyphicon-tag"></span>
                <a href="<%# Item.Url %>"><%# Item.Name %></a>
            </li>
        </ItemTemplate>
    </asp:ListView>
</ul>