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
                <h3><%# HttpUtility.HtmlDecode (Item.TitleLink) %></h3>
                <p>
                    <news:TermLinks id="termLinks" runat="server" CssClass="list-inline term-links" />
                </p>
                <news:BadgeList id="listBadges" runat="server" CssClass="list-inline visibility-badges" BadgeCssClass="badge" />
                <ul class="list-inline news-entry-info">
                    <li><i class="fas fa-calendar-alt"></i> <%# Item.PublishedOnDateString %></li>
                    <li><i class="fas fa-user"></i> <%# Item.CreatedByUserName %></li>
                </ul>
                <div class="row news-entry-main-row">
                    <div class="<%# Item.ImageColumnCssClass %>">
                        <asp:HyperLink id="linkImage" runat="server" NavigateUrl="<%# Item.Link %>" Visible="<%# Item.HasImage %>">
                            <asp:Image id="imageImage" runat="server" 
                                ImageUrl="<%# Item.ImageUrl %>" AlternateText="<%# Item.Title %>"
                                CssClass='<%# Item.ImageCssClass + " news-entry-image" %>' />
                        </asp:HyperLink>
                    </div>
                    <div class='<%# Item.TextColumnCssClass + " news-entry-text-column" %>'>
						<div class="<%# Item.TextCssClass %>">
                            <%# HttpUtility.HtmlDecode (Item.Description) %>
						</div>	
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
						    <news:ActionButtons id="actionButtons" runat="server" />
						</ul>	
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:FormView>
</div>	