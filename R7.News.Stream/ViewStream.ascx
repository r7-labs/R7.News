<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ViewStream.ascx.cs" Inherits="R7.News.Stream.ViewStream" %>
<%@ Register TagPrefix="news" TagName="TermLinks" Src="~/DesktopModules/R7.News/R7.News/Controls/TermLinks.ascx" %>
<%@ Register TagPrefix="news" TagName="BadgeList" Src="~/DesktopModules/R7.News/R7.News/Controls/BadgeList.ascx" %>
<%@ Register TagPrefix="news" TagName="AgplSignature" Src="~/DesktopModules/R7.News/R7.News/Controls/AgplSignature.ascx" %>
<%@ Register TagPrefix="r7" Assembly="R7.DotNetNuke.Extensions" Namespace="R7.DotNetNuke.Extensions.Controls" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Import Namespace="System.Web" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.News/R7.News/assets/css/module.css" />

<asp:Panel id="panelStream" runat="server">
    <r7:PagingControl id="pagerTop" runat="server" OnPageChanged="pagingControl_PageChanged" />
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
                <news:BadgeList id="listBadges" runat="server" CssClass="visibility-badges" BadgeCssClass="badge" />
                <p class="news-entry-info">
                    <span class="glyphicon glyphicon-calendar"></span> <%# Eval ("PublishedOnDateString") %> 
                    <span class="glyphicon glyphicon-user"></span> <%# Eval ("CreatedByUserName") %>
                </p>
                <div class="row news-entry-main-row">
                    <div class="<%# Eval ("ImageContainerCssClass") %>">
                        <asp:HyperLink id="linkImage" runat="server" NavigateUrl='<%# Eval ("Link") %>' Visible='<%# Eval ("HasImage") %>'>
                            <asp:Image id="imageImage" runat="server"
                                ImageUrl='<%# Eval ("ImageUrl") %>' AlternateText='<%# Eval ("Title") %>'
                                CssClass="img img-rounded img-responsive news-entry-image" />
                        </asp:HyperLink>
                    </div>
                    <div class="<%# Eval ("DescriptionContainerCssClass") %> news-entry-description">
                       <%# HttpUtility.HtmlDecode ((string) Eval ("Description")) %>
                    </div>
                </div>
                <p>
                    <news:TermLinks id="termLinks" runat="server" CssClass="term-links" />
                </p>
				<asp:LinkButton id="linkDiscuss" runat="server" resourceKey="linkDiscuss.Text" CssClass="btn btn-default btn-sm" OnCommand="linkDiscuss_Command" CommandArgument='<%# Eval ("EntryId") %>' />
            </div>
        </ItemTemplate>
        <ItemSeparatorTemplate>
            <hr />
        </ItemSeparatorTemplate>
    </asp:ListView>
    <hr />
    <r7:PagingControl id="pagerBottom" runat="server" OnPageChanged="pagingControl_PageChanged" />
    <asp:LinkButton id="buttonShowMore" runat="server" resourcekey="buttonShowMore.Text" CssClass="btn btn-default btn-block" OnClick="buttonShowMore_Click" />
</asp:Panel>
<news:AgplSignature id="agplSignature" runat="server" />