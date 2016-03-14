<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ViewStream.ascx.cs" Inherits="R7.News.Stream.ViewStream" %>
<%@ Register TagPrefix="news" TagName="TermLinks" Src="~/DesktopModules/R7.News/R7.News/Controls/TermLinks.ascx" %>
<%@ Import Namespace="System.Web" %>

<asp:ListView id="listStream" DataKeyNames="EntryId" runat="server" OnItemDataBound="listStream_ItemDataBound">
    <LayoutTemplate>
        <div runat="server" style="display:table">
            <div runat="server" id="itemPlaceholder"></div>
        </div>
    </LayoutTemplate>
    <ItemTemplate>
        <div>
            <h3>
                <asp:HyperLink id="linkEdit" runat="server">
                    <asp:Image id="imageEdit" runat="server" IconKey="Edit" resourcekey="Edit" />
                </asp:HyperLink>
                <%# HttpUtility.HtmlDecode ((string) Eval ("TitleLink")) %>
            </h3>
            <div style="display:table-row">
                <asp:Image id="imageImage" runat="server" CssClass="img img-rounded" 
                    Style="display:table-cell;vertical-align:top;margin:0 1em 1em 0" />
                <div style="display:table-cell">
                    EntryId: <%# Eval ("EntryId") %><br />
                    CreatedOnDate: <%# Eval ("ContentItem.CreatedOnDate") %><br />
                    AgentModuleId: <%# Eval ("AgentModuleId") %><br />
                    Url: <%# Eval ("Url") %><br />
                    LastModifiedOnDate: <%# Eval ("ContentItem.LastModifiedOnDate") %><br />
                    CreatedByUserID: <%# Eval ("ContentItem.CreatedByUserID") %><br />
                    LastModifiedByUserID: <%# Eval ("ContentItem.LastModifiedByUserID") %><br />
                    Description: <%# HttpUtility.HtmlDecode ((string) Eval ("Description")) %><br />
                    Visibility: <%# Eval ("Visibility") %><br />
                    Source: <%# Eval ("Source.Title") %>, Url: <%# Eval ("Source.Url") %>
                    <news:TermLinks id="termLinks" runat="server" />
                </div>
            </div>
        </div>
    </ItemTemplate>
    <ItemSeparatorTemplate>
        <hr />
    </ItemSeparatorTemplate>
</asp:ListView>
<h2>Test:</h2>
DataCacheTime: <%: R7.News.Components.NewsConfig.Instance.DataCacheTime %><br />
DefaultImagePath: <%: R7.News.Components.NewsConfig.Instance.DefaultImagesPath %><br />