<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ViewStream.ascx.cs" Inherits="R7.News.Stream.ViewStream" %>
<%@ Register TagPrefix="news" TagName="TermLinks" Src="~/DesktopModules/R7.News/R7.News/Controls/TermLinks.ascx" %>
<%@ Register TagPrefix="news" TagName="BadgeList" Src="~/DesktopModules/R7.News/R7.News/Controls/BadgeList.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.R7" Namespace="DotNetNuke.R7" %>
<%@ Import Namespace="System.Web" %>

<asp:Panel id="panelStream" runat="server">
    <dnn:PagingControl id="pagerTop" runat="server" OnPageChanged="pagingControl_PageChanged" />
    <asp:ListView id="listStream" DataKeyNames="EntryId" runat="server" OnItemDataBound="listStream_ItemDataBound">
        <LayoutTemplate>
            <div runat="server" class="news-stream">
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
                <news:BadgeList id="listBadges" runat="server" BadgeCssClass="badge" />
                <p class="small" style="color:gray"><span class="glyphicon glyphicon-calendar"></span> <%# Eval ("PublishedOnDateString") %> 
                <span class="glyphicon glyphicon-user" style="margin-left:1em"></span> <%# Eval ("CreatedByUserName") %></p>
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
            </div>
        </ItemTemplate>
        <ItemSeparatorTemplate>
            <hr />
        </ItemSeparatorTemplate>
    </asp:ListView>
    <hr />
    <dnn:PagingControl id="pagerBottom" runat="server" OnPageChanged="pagingControl_PageChanged" />
    <asp:LinkButton id="buttonShowMore" runat="server" resourcekey="buttonShowMore.Text" CssClass="btn btn-default btn-block" OnClick="buttonShowMore_Click" />
</asp:Panel>