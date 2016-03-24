<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="BadgeList.ascx.cs" Inherits="R7.News.Controls.BadgeList" %>
<asp:ListView id="listBadges" runat="server">
    <LayoutTemplate>
        <ul runat="server" class="visibility-badges list-inline small">
            <li runat="server" id="itemPlaceholder"></li>
        </ul>
    </LayoutTemplate>
    <ItemTemplate>
        <li class="<%# BadgeCssClass %> <%# Eval ("CssClass") %>"><%# Eval ("Text") %></li>
    </ItemTemplate>
</asp:ListView>