<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ViewStream.ascx.cs" Inherits="R7.News.Stream.ViewStream" %>
<%@ Register TagPrefix="news" TagName="TermLinks" Src="~/DesktopModules/R7.News/R7.News/Controls/TermLinks.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.R7" Namespace="DotNetNuke.R7" %>
<%@ Import Namespace="System.Web" %>

<asp:ListView id="listStream" DataKeyNames="EntryId" runat="server" OnItemDataBound="listStream_ItemDataBound">
    <LayoutTemplate>
        <div runat="server">
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
            <p class="small" style="color:gray"><%# Eval ("CreatedOnDateString") %> - <%# Eval ("CreatedByUserName") %></p>
            <div class="row">
                <div class="col-sm-4">
                    <asp:Image id="imageImage" runat="server" ImageUrl='<%# Eval ("ImageUrl") %>'
                        CssClass="img img-rounded img-responsive" Style="margin-bottom:1em" />
                </div>
                <div class="col-sm-8">
                   <%# HttpUtility.HtmlDecode ((string) Eval ("Description")) %>
                </div>
            </div>
            <p>
                <news:TermLinks id="termLinks" runat="server" />
            </p>
            <div class="small">
                EntryId: <%# Eval ("EntryId") %><br />
                AgentModuleId: <%# Eval ("AgentModuleId") %><br />
                LastModifiedOnDate: <%# Eval ("ContentItem.LastModifiedOnDate") %><br />
                LastModifiedByUserID: <%# Eval ("ContentItem.LastModifiedByUserID") %><br />    
                Visibility: <%# Eval ("Visibility") %><br />
                Source: <%# Eval ("Source.Title") %>, Url: <%# Eval ("Source.Url") %>
            </div>
        </div>
    </ItemTemplate>
    <ItemSeparatorTemplate>
        <hr />
    </ItemSeparatorTemplate>
</asp:ListView>
<hr />
<dnn:PagingControl id="pagingControl" runat="server" OnPageChanged="pagingControl_PageChanged" />
<hr />
<h2>Config:</h2>
DataCacheTime: <%: R7.News.Components.NewsConfig.Instance.DataCacheTime %><br />
DefaultImagePath: <%: R7.News.Components.NewsConfig.Instance.DefaultImagesPath %><br />