<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="TermLinks.ascx.cs" Inherits="R7.News.Controls.TermLinks" %>
<asp:ListView id="listTermLinks" runat="server" DataKeyNames="TermId">
    <LayoutTemplate>
        <ul runat="server">
            <li runat="server" id="itemPlaceholder"></li>
        </ul>
    </LayoutTemplate>
    <ItemTemplate>
        <a href="<%# Eval ("Url") %>"><%# Eval ("Name") %></a>
    </ItemTemplate>
</asp:ListView>