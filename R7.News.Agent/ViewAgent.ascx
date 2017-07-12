<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewAgent.ascx.cs" Inherits="R7.News.Agent.ViewAgent" %>
<%@ Register TagPrefix="news" TagName="TermLinks" Src="~/DesktopModules/R7.News/R7.News/Controls/TermLinks.ascx" %>
<%@ Register TagPrefix="news" TagName="BadgeList" Src="~/DesktopModules/R7.News/R7.News/Controls/BadgeList.ascx" %>
<%@ Register TagPrefix="news" TagName="ActionButtons" Src="~/DesktopModules/R7.News/R7.News/Controls/ActionButtons.ascx" %>
<%@ Register TagPrefix="news" TagName="AgplSignature" Src="~/DesktopModules/R7.News/R7.News/Controls/AgplSignature.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Import Namespace="System.Web" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.News/R7.News/assets/css/module.css" />

<asp:Panel id="panelAddDefaultEntry" runat="server" Visible="false" CssClass="dnnFormMessage dnnFormInfo">
    <asp:LinkButton id="buttonAddFromTabData" runat="server" resourcekey="buttonAddFromTabData.Text"
        CssClass="dnnSecondaryAction dnnRight button-add-from-tab-data" OnClick="buttonAddFromTabData_Click" />
    <%: LocalizeString ("NothingToDisplay.Text") %>
</asp:Panel>
<asp:ListView id="listAgent" ItemType="R7.News.Agent.ViewModels.AgentNewsEntryViewModel" runat="server" OnItemDataBound="listAgent_ItemDataBound">
    <LayoutTemplate>
        <div runat="server" class="news-module news-agent">
            <div runat="server" id="itemPlaceholder"></div>
        </div>
    </LayoutTemplate>
    <ItemTemplate>
        <div>
            <div class="row">
                <div class="<%# Item.FirstColumnContainerCssClass %>">
                    <asp:HyperLink id="linkImage" runat="server" NavigateUrl="<%# Item.Link %>" Visible="<%# Item.HasImage %>">
                        <asp:Image id="imageImage" runat="server" 
                            ImageUrl="<%# Item.ImageUrl %>" AlternateText="<%# Item.Title %>"
                            CssClass="img img-rounded img-responsive" />
                    </asp:HyperLink>
                </div>
                <div class="<%# Item.SecondColumnContainerCssClass %>">
                    <h3 style="margin-top:0">
                        <asp:HyperLink id="linkEdit" runat="server" Visible="<%# IsEditable %>" NavigateUrl='<%# EditUrl ("entryid", Item.EntryId.ToString (), "EditNewsEntry") %>'>
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
                        <span class="glyphicon glyphicon-user" style="margin-left:1em"></span> <%# Item.CreatedByUserName %></p>
                    <p>
                    <div class="news-entry-description">
                        <%# HttpUtility.HtmlDecode (Item.Description) %>
				        <news:ActionButtons id="actionButtons" CssClass="list-inline news-action-btns" runat="server" />
                    </div>
                    <asp:ListView id="listGroup" ItemType="R7.News.Agent.ViewModels.AgentNewsEntryViewModel" runat="server" OnItemDataBound="listGroup_ItemDataBound">
                        <LayoutTemplate>
                            <div runat="server" class="list-group">
                                <div runat="server" id="itemPlaceholder"></div>
                            </div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <h4>
                                <asp:HyperLink id="linkEdit" runat="server" Visible="<%# IsEditable %>" NavigateUrl='<%# EditUrl ("entryid", Item.EntryId.ToString (), "EditNewsEntry") %>'>
                                    <asp:Image id="imageEdit" runat="server" IconKey="Edit" resourcekey="Edit" />
                                </asp:HyperLink>
								<%# HttpUtility.HtmlDecode (Item.TitleLink) %>
                            </h4>
                            <news:BadgeList id="listBadges" runat="server" CssClass="list-inline visibility-badges" BadgeCssClass="badge" />
                            <p class="news-entry-info">
                                <span class="glyphicon glyphicon-calendar"></span> <%# Item.PublishedOnDateString %>
                                <span class="glyphicon glyphicon-user" style="margin-left:1em"></span> <%# Item.CreatedByUserName %>
                            </p>
                            <div class="news-entry-main-row">
                                <div>
                                    <asp:HyperLink id="linkImage" runat="server" NavigateUrl="<%# Item.Link %>" Visible="<%# Item.HasImage %>">
                                        <asp:Image id="imageImage" runat="server" 
                                            ImageUrl="<%# Item.GroupImageUrl %>" AlternateText="<%# Item.Title %>"
                                            CssClass="img-thumbnail news-entry-image" />
                                    </asp:HyperLink>
                                </div>
                                <div class="news-entry-description">
                                    <%# HttpUtility.HtmlDecode (Item.Description) %>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
            </div>
        </div>
    </ItemTemplate>
    <ItemSeparatorTemplate>
        <hr />
    </ItemSeparatorTemplate>
</asp:ListView>
<news:AgplSignature id="agplSignature" runat="server" />
