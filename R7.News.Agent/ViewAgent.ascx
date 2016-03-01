<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewAgent.ascx.cs" Inherits="R7.News.Agent.ViewAgent" %>
<%@ Import Namespace="System.Web" %>

<asp:ListView id="listAgent" DataKeyNames="EntryId" runat="server" OnItemDataBound="listAgent_ItemDataBound">
    <LayoutTemplate>
        <div runat="server">
            <div runat="server" id="itemPlaceholder"></div>
        </div>
    </LayoutTemplate>
    <ItemTemplate>
        <div>
            <asp:HyperLink id="linkEdit" runat="server">
                <asp:Image id="imageEdit" runat="server" IconKey="Edit" resourcekey="Edit" />
            </asp:HyperLink>
            EntryId: <%# Eval ("EntryId") %><br />
            Title: <%# Eval ("Title") %><br />
            CreatedOnDate: <%# Eval ("ContentItem.CreatedOnDate") %><br />
            LastModifiedOnDate: <%# Eval ("ContentItem.LastModifiedOnDate") %><br />
            CreatedByUserID: <%# Eval ("ContentItem.CreatedByUserID") %><br />
            LastModifiedByUserID: <%# Eval ("ContentItem.LastModifiedByUserID") %><br />
            Description: <%# HttpUtility.HtmlDecode ((string) Eval ("Description")) %><br />
        </div>
    </ItemTemplate>
    <ItemSeparatorTemplate>
        <hr />
    </ItemSeparatorTemplate>
</asp:ListView>

