<%@ Control Language="C#" AutoEventWireup="false" Inherits="R7.News.Controls.AgplSignature" CodeBehind="AgplSignature.ascx.cs" %>
<div class="news-agpl-footer">
    <% if (ShowRule) { %><hr class="my-0 ml-0" /><% } %>
    <a href="https://github.com/roman-yagodin/R7.News" rel="nofollow" target="_blank" class="text-muted" title='<%= LocalizeString ("SourceLink_Title.Text") %>'>
        <%= string.Format ("{0} v{1}", AppName, GetVersionString ()) %>
    </a>
</div>
