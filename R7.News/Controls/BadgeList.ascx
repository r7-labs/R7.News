<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="BadgeList.ascx.cs" Inherits="R7.News.Controls.BadgeList" %>
<div class="<%: CssClass %>">
    <asp:ListView id="listBadges" runat="server" ItemType="R7.News.Controls.Badge">
        <LayoutTemplate>
            <ul runat="server" class="list-inline">
                <li runat="server" id="itemPlaceholder"></li>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li class="<%# BadgeCssClass %> <%# Item.CssClass %>"><%# Item.Text %></li>
        </ItemTemplate>
    </asp:ListView>
</div>