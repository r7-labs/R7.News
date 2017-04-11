<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="BadgeList.ascx.cs" Inherits="R7.News.Controls.BadgeList" %>
<ul runat="server" class="<%# CssClass %>">
	<asp:ListView id="listBadges" runat="server" ItemType="R7.News.Controls.Badge">
        <LayoutTemplate>
            <li runat="server" id="itemPlaceholder"></li>
        </LayoutTemplate>
        <ItemTemplate>
            <li class="<%# BadgeCssClass %> <%# Item.CssClass %>"><%# Item.Text %></li>
        </ItemTemplate>
    </asp:ListView>
 </ul>