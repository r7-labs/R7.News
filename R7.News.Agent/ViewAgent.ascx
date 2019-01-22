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
                            CssClass="img-thumbnail news-entry-image" />
                    </asp:HyperLink>
				</div>
                <div class="<%# Item.SecondColumnContainerCssClass %>">
                    <h3 style="margin-top:0">
                        <asp:HyperLink id="linkEdit" runat="server" Visible="<%# IsEditable %>" NavigateUrl='<%# EditUrl ("entryid", Item.EntryId.ToString (), "EditNewsEntry") %>'>
                            <asp:Image id="imageEdit" runat="server" IconKey="Edit" IconSize="16X16" IconStyle="Gray" resourcekey="Edit" />
                        </asp:HyperLink>
                        <%# HttpUtility.HtmlDecode (Item.TitleLink) %>
                    </h3>
					<p>
						<news:TermLinks id="termLinks" runat="server" CssClass="list-inline term-links" />
					</p>
                    <news:BadgeList id="listBadges" runat="server" CssClass="list-inline visibility-badges" BadgeCssClass="badge" />
                    <ul class="list-inline news-entry-info">
                        <li><i class="fas fa-calendar-alt"></i> <%# Item.PublishedOnDateString %></li>
                        <li><i class="fas fa-user" style="margin-left:1em"></i> <%# Item.CreatedByUserName %></li>
                    </ul>
                    <div class="news-entry-description">
                        <%# HttpUtility.HtmlDecode (Item.Description) %>
				        <ul class="list-inline news-action-btns">
							<li runat="server" class="dropdown" Visible="<%# IsEditable %>">
	                            <button class="btn btn-default btn-sm dropdown-toggle" type="button" data-toggle="dropdown"><i class="fas fa-cog"></i>
	                            <span class="caret"></span></button>
	                            <ul class="dropdown-menu">
								    <li>
										<a href='<%# HttpUtility.HtmlAttributeEncode (EditUrl ("entryid", Item.EntryId.ToString (), "EditNewsEntry")) %>'>
											<i class="fas fa-pencil-alt"></i>
											<%# LocalizeString ("EditNewsEntry.Text") %>
										</a>
									</li>
									<li>
										<asp:LinkButton runat="server" OnCommand="btnSyncTab_Command"
												CommandName="SyncTab" CommandArgument="<%# Item.EntryId.ToString () %>">
											<i class="fas fa-sync"></i>
											<%# LocalizeString ("SyncTab.Text") %>
	                                    </asp:LinkButton>
									</li>
									<li>
										<asp:LinkButton runat="server" OnCommand="btnUnbind_Command"
												CommandName="Unbind" CommandArgument="<%# Item.EntryId.ToString () %>">
											<i class="fas fa-unlink"></i>
											<%# LocalizeString ("Unbind.Text") %>
	                                    </asp:LinkButton>
									</li>
								</ul>
							</li>
							<news:ActionButtons id="actionButtons" runat="server" />
						</ul>	
                    </div>
                    <asp:ListView id="listGroup" ItemType="R7.News.Agent.ViewModels.AgentNewsEntryViewModel" runat="server" OnItemDataBound="listGroup_ItemDataBound">
                        <LayoutTemplate>
                            <div runat="server" class="list-group">
                                <div runat="server" id="itemPlaceholder"></div>
                            </div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <h4><%# HttpUtility.HtmlDecode (Item.TitleLink) %></h4>
                            <news:BadgeList id="listBadges" runat="server" CssClass="list-inline visibility-badges" BadgeCssClass="badge" />
                            <ul class="list-inline news-entry-info">
                                <li><i class="fas fa-calendar-alt"></i> <%# Item.PublishedOnDateString %></li>
                                <li><i class="fas fa-user" style="margin-left:1em"></i> <%# Item.CreatedByUserName %></li>
                            </ul>
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
									<ul class="list-inline news-action-btns">
										<li runat="server" class="dropdown" Visible="<%# IsEditable %>">
				                            <button class="btn btn-default btn-sm dropdown-toggle" type="button" data-toggle="dropdown"><i class="fas fa-cog"></i>
				                            <span class="caret"></span></button>
				                            <ul class="dropdown-menu">
											    <li>
													<a href='<%# HttpUtility.HtmlAttributeEncode (EditUrl ("entryid", Item.EntryId.ToString (), "EditNewsEntry")) %>'>
														<i class="fas fa-pencil-alt"></i>
														<%# LocalizeString ("EditNewsEntry.Text") %>
													</a>
												</li>
											</ul>
										</li>
									</ul>	
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
