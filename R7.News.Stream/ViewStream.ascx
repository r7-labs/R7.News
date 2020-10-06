<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ViewStream.ascx.cs" Inherits="R7.News.Stream.ViewStream" %>
<%@ Register TagPrefix="news" TagName="TermLinks" Src="~/DesktopModules/R7.News/R7.News/Controls/TermLinks.ascx" %>
<%@ Register TagPrefix="news" TagName="BadgeList" Src="~/DesktopModules/R7.News/R7.News/Controls/BadgeList.ascx" %>
<%@ Register TagPrefix="news" TagName="ActionList" Src="~/DesktopModules/R7.News/R7.News/Controls/ActionList.ascx" %>
<%@ Register TagPrefix="news" TagName="AgplSignature" Src="~/DesktopModules/R7.News/R7.News/Controls/AgplSignature.ascx" %>
<%@ Register TagPrefix="r7" Assembly="R7.Dnn.Extensions" Namespace="R7.Dnn.Extensions.Controls.PagingControl" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Import Namespace="System.Web" %>
<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.News/R7.News/assets/css/module.css" />
<asp:Panel id="panelStream" runat="server" CssClass="news-module news-stream">
	<asp:Panel id="pnlFeed" runat="server" CssClass="news-top-actions">
		<a href="<%= ViewModel.FeedUrl %>" target="_blank" class="btn btn-link"
			title='<%: LocalizeString ("btnFeed.Title") %>'>
			<span class="fas fa-rss-square"></span>
			<%: LocalizeString ("btnFeed.Text") %> <%: ModuleConfiguration.ModuleTitle %>
		</a>
	</asp:Panel>
    <r7:PagingControl id="pagerTop" runat="server" OnPageChanged="pagingControl_PageChanged" />
    <asp:ListView id="listStream" ItemType="R7.News.Stream.ViewModels.StreamNewsEntry" runat="server" OnItemDataBound="listStream_ItemDataBound">
        <LayoutTemplate>
            <div runat="server">
                <div runat="server" id="itemPlaceholder"></div>
            </div>
        </LayoutTemplate>
        <ItemTemplate>
            <div>
                <h3><%# HttpUtility.HtmlDecode (Item.TitleLink) %></h3>
				<p>
                    <news:TermLinks id="termLinks" runat="server" CssClass="list-inline term-links" />
                </p>
                <news:BadgeList id="listBadges" runat="server" CssClass="list-inline visibility-badges" BadgeCssClass="list-inline-item badge" />
                <ul class="list-inline news-entry-info">
                    <li class="list-inline-item"><i class="fas fa-calendar-alt"></i> <%# Item.PublishedOnDateString %></li>
                    <li class="list-inline-item"><i class="fas fa-user"></i> <%# Item.CreatedByUserName %></li>
                </ul>
                <div class="row news-entry-main-row">
                    <div class="<%# Item.ImageColumnCssClass %>">
                        <asp:HyperLink id="linkImage" runat="server" NavigateUrl="<%# Item.Link %>" Visible="<%# Item.HasImage %>">
                            <asp:Image id="imageImage" runat="server"
                                ImageUrl="<%# Item.ImageUrl %>" AlternateText="<%# Item.Title %>"
                                CssClass='<%# Item.ImageCssClass + " news-entry-image"%>' />
                        </asp:HyperLink>
                    </div>
                    <div class='<%# Item.TextColumnCssClass + " news-entry-text-column" %>'>
                        <div class="<%# Item.TextCssClass %>">
                            <%# HttpUtility.HtmlDecode (Item.Description) %>
						</div>
						<news:ActionList id="listActions" runat="server"
							EntryId="<%# Item.EntryId %>"
							EntryTextId="<%# Item.EntryTextId %>"
							ShowDuplicateAction="true"
							ShowExpandTextAction="true" />
                    </div>
                </div>
			</div>
        </ItemTemplate>
        <ItemSeparatorTemplate>
            <hr />
        </ItemSeparatorTemplate>
    </asp:ListView>
    <hr />
    <r7:PagingControl id="pagerBottom" runat="server" OnPageChanged="pagingControl_PageChanged" />
    <asp:LinkButton id="buttonShowMore" runat="server" resourcekey="buttonShowMore.Text" CssClass="btn btn-sm btn-primary btn-block" OnClick="buttonShowMore_Click" />
</asp:Panel>
<news:AgplSignature id="agplSignature" runat="server" />
