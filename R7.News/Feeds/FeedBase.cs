using System;
using System.Collections.Generic;
using System.Xml;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using R7.News.Models;

namespace R7.News.Feeds
{
    public abstract class FeedBase : IFeed
    {
        protected string Base64ToCanonicalFormFix (string base64String) => base64String.Replace ("%3d", "%3D");

        protected abstract string FormatDateTime (DateTime dateTime);

        public abstract void Render (XmlWriter writer, IEnumerable<NewsEntry> newsEntries, ModuleInfo module, PortalSettings portalSettings, string requestUrl, bool withImages);
    }
}
