<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewStream.ascx.cs" Inherits="R7.News.Stream.ViewStream" %>
<%@ Import Namespace="System.Web" %>

<asp:ListView id="listStream" DataKeyNames="EntryId" runat="server" OnItemDataBound="listStream_ItemDataBound">
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
            Title: <%# Eval ("Title") %><br />
            CreatedOnDate: <%# Eval ("ContentItem.CreatedOnDate") %><br />
            LastModifiedOnDate: <%# Eval ("ContentItem.LastModifiedOnDate") %><br />
            CreatedByUserID: <%# Eval ("ContentItem.CreatedByUserID") %><br />
            LastModifiedByUserID: <%# Eval ("ContentItem.LastModifiedByUserID") %><br />
            Description: <%# HttpUtility.HtmlDecode ((string) Eval ("Description")) %>
        </div>
    </ItemTemplate>
    <ItemSeparatorTemplate>
        <hr />
    </ItemSeparatorTemplate>
</asp:ListView>

