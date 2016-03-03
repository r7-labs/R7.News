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
            EntryId: <%# Eval ("EntryId") %><br />
            Title: <%# Eval ("Title") %><br />
            CreatedOnDate: <%# Eval ("ContentItem.CreatedOnDate") %><br />
            LastModifiedOnDate: <%# Eval ("ContentItem.LastModifiedOnDate") %><br />
            CreatedByUserID: <%# Eval ("ContentItem.CreatedByUserID") %><br />
            LastModifiedByUserID: <%# Eval ("ContentItem.LastModifiedByUserID") %><br />
            Description: <%# HttpUtility.HtmlDecode ((string) Eval ("Description")) %><br />
            Visibility: <%# Eval ("Visibility") %><br />
            Source: <%# Eval ("Source.Title") %>, Url: <%# Eval ("Source.Url") %>
        </div>
    </ItemTemplate>
    <ItemSeparatorTemplate>
        <hr />
    </ItemSeparatorTemplate>
</asp:ListView>
<h2>Test:</h2>
DataCacheTime: <%: R7.News.Components.NewsConfig.Instance.DataCacheTime %><br />
DefaultImagePath: <%: R7.News.Components.NewsConfig.Instance.DefaultImagesPath %><br />