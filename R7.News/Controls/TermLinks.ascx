<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="TermLinks.ascx.cs" Inherits="R7.News.Controls.TermLinks" %>
<div class="<%: CssClass %>">
    <asp:ListView id="listTermLinks" runat="server" DataKeyNames="TermId">
        <LayoutTemplate>
            <ul runat="server" class="list-inline" style="margin-left:inherit">
                <li runat="server" id="itemPlaceholder"></li>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li style="padding-left:inherit">
                <span class="glyphicon glyphicon-tag"></span>
                <a href="<%# Eval ("Url") %>"><%# Eval ("Name") %></a>
            </li>
        </ItemTemplate>
    </asp:ListView>
</div>