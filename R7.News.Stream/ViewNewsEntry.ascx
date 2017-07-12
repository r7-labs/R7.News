<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ViewNewsEntry.ascx.cs" Inherits="R7.News.Stream.ViewNewsEntry" %>
<%@ Register TagPrefix="news" TagName="TermLinks" Src="~/DesktopModules/R7.News/R7.News/Controls/TermLinks.ascx" %>
<%@ Register TagPrefix="news" TagName="BadgeList" Src="~/DesktopModules/R7.News/R7.News/Controls/BadgeList.ascx" %>
<%@ Register TagPrefix="news" TagName="ActionButtons" Src="~/DesktopModules/R7.News/R7.News/Controls/ActionButtons.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Import Namespace="System.Web" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.News/R7.News/assets/css/module.css" />
<div class="news-module news-entry">
    <asp:FormView id="formNewsEntry" runat="server" RenderOuterTable="false"
            ItemType="R7.News.Stream.ViewModels.StreamNewsEntryViewModel" OnDataBound="formNewsEntry_DataBound">
        <ItemTemplate>
            <div>
                <h3>
                    <asp:HyperLink id="linkEdit" runat="server">
                        <asp:Image id="imageEdit" runat="server" IconKey="Edit" resourcekey="Edit" />
                    </asp:HyperLink>
                    <%# HttpUtility.HtmlDecode (Item.TitleLink) %>
                </h3>
                <p>
                    <news:TermLinks id="termLinks" runat="server" CssClass="list-inline term-links" />
                </p>
                <news:BadgeList id="listBadges" runat="server" CssClass="list-inline visibility-badges" BadgeCssClass="badge" />
                <p class="news-entry-info">
                    <span class="glyphicon glyphicon-calendar"></span> <%# Item.PublishedOnDateString %> 
                    <span class="glyphicon glyphicon-user"></span> <%# Item.CreatedByUserName %>
                </p>
                <div class="row news-entry-main-row">
                    <div class="<%# Item.ImageContainerCssClass %>">
                        <asp:HyperLink id="linkImage" runat="server" NavigateUrl='<%# Item.Link %>' Visible='<%# Item.HasImage %>'>
                            <asp:Image id="imageImage" runat="server" 
                                ImageUrl='<%# Item.ImageUrl %>' AlternateText='<%# Item.Title %>'
                                CssClass="img-thumbnail news-entry-image" />
                        </asp:HyperLink>
                    </div>
                    <div class="<%# Item.DescriptionContainerCssClass %> news-entry-description">
                        <%# HttpUtility.HtmlDecode (Item.Description) %>
					    <news:ActionButtons id="actionButtons" CssClass="list-inline news-action-btns" runat="server" />
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:FormView>
</div>	