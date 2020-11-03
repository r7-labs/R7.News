
using System;
using System.Collections.Generic;
using System.Xml;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using R7.News.Models;

namespace R7.News.Feeds
{
    public interface IFeed
    {
        void Render (XmlWriter writer, IEnumerable<NewsEntry> newsEntries, ModuleInfo module,
            PortalSettings portalSettings, string requestUrl);
    }
}
