<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ViewNewsEntry.ascx.cs" Inherits="R7.News.Stream.ViewNewsEntry" %>
<%@ Register TagPrefix="news" TagName="TermLinks" Src="~/DesktopModules/R7.News/R7.News/Controls/TermLinks.ascx" %>
<%@ Register TagPrefix="news" TagName="BadgeList" Src="~/DesktopModules/R7.News/R7.News/Controls/BadgeList.ascx" %>
<%@ Import Namespace="System.Web" %>

<asp:FormView id="formNewsEntry" CssClass="news-entry" runat="server" OnDataBound="formNewsEntry_DataBound">
    <ItemTemplate>
        <div>
            <h3>
                <asp:HyperLink id="linkEdit" runat="server">
                    <asp:Image id="imageEdit" runat="server" IconKey="Edit" resourcekey="Edit" />
                </asp:HyperLink>
                <%# HttpUtility.HtmlDecode ((string) Eval ("TitleLink")) %>
            </h3>
            <news:BadgeList id="listBadges" runat="server" CssClass="visibility-badges" BadgeCssClass="badge" />
            <p class="news-entry-info">
                <span class="glyphicon glyphicon-calendar"></span> <%# Eval ("PublishedOnDateString") %> 
                <span class="glyphicon glyphicon-user"></span> <%# Eval ("CreatedByUserName") %>
            </p>
            <div class="row news-entry-main-row">
                <div class="<%# Eval ("ImageContainerCssClass") %>">
                    <asp:Image id="imageImage" runat="server" ImageUrl='<%# Eval ("ImageUrl") %>'
                        CssClass="img img-rounded news-entry-image" />
                </div>
                <div class="<%# Eval ("DescriptionContainerCssClass") %> news-entry-description">
                   <%# HttpUtility.HtmlDecode ((string) Eval ("Description")) %>
                </div>
            </div>
            <p>
                <news:TermLinks id="termLinks" runat="server" CssClass="term-links" />
            </p>
        </div>
    </ItemTemplate>
</asp:FormView>