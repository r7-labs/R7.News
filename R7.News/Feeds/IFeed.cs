
using System;
using System.Collections.Generic;
using System.Xml;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using R7.News.Models;

namespace R7.News.Feeds
{
    // TODO: Implement withImages support in the separate feed class for VK
    public interface IFeed
    {
        void Render (XmlWriter writer, IEnumerable<NewsEntry> newsEntries, ModuleInfo module,
            PortalSettings portalSettings, string requestUrl, bool withImages);
    }
}
