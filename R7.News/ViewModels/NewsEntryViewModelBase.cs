using System;
using System.Collections.Generic;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Localization;
using R7.Dnn.Extensions.ViewModels;
using R7.News.Controls;
using R7.News.Models;

namespace R7.News.ViewModels
{
    public class NewsEntryViewModelBase: INewsEntry
    {
        protected ViewModelContext Context;

        protected INewsEntry NewsEntry;

        public NewsEntryViewModelBase (INewsEntry newsEntry, ViewModelContext context)
        {
            NewsEntry = newsEntry;
            Context = context;
        }

        #region INewsEntry implementation

        public int EntryId
        {
            get { return NewsEntry.EntryId; }
            set { throw new InvalidOperationException (); }
        }

        public int PortalId
        {
            get { return NewsEntry.PortalId; }
            set { throw new InvalidOperationException (); }
        }

        public int ContentItemId
        {
            get { return NewsEntry.ContentItemId; }
            set { throw new InvalidOperationException (); }
        }

        public int? AgentModuleId
        {
            get { return NewsEntry.AgentModuleId; }
            set { throw new InvalidOperationException (); }
        }

        public string Url
        {
            get { return NewsEntry.Url; }
            set { throw new InvalidOperationException (); }
        }

        public DateTime? StartDate
        {
            get { return NewsEntry.StartDate; }
            set { throw new InvalidOperationException (); }
        }

        public DateTime? EndDate
        {
            get { return NewsEntry.EndDate; }
            set { throw new InvalidOperationException (); }
        }

        public DateTime? ThresholdDate
        {
            get { return NewsEntry.ThresholdDate; }
            set { throw new InvalidOperationException (); }
        }

        public DateTime? DueDate
        {
            get { return NewsEntry.DueDate; }
            set { throw new InvalidOperationException (); }
        }

        public string Title
        {
            get { return NewsEntry.Title; }
            set { throw new InvalidOperationException (); }
        }

        public string Description
        {
            get { return NewsEntry.Description; }
            set { throw new InvalidOperationException (); }
        }

        public int? EntryTextId
        {
            get { return NewsEntry.EntryTextId; }
            set { throw new InvalidOperationException (); }
        }

        public string Text
        {
            get { return NewsEntry.Text; }
            set { throw new InvalidOperationException (); }
        }

        public int ThematicWeight
        {
            get { return NewsEntry.ThematicWeight; }
            set { throw new InvalidOperationException (); }
        }

        public int StructuralWeight
        {
            get { return NewsEntry.StructuralWeight; }
            set { throw new InvalidOperationException (); }
        }

        public ContentItem ContentItem
        {
            get { return NewsEntry.ContentItem; }
            set { throw new InvalidOperationException (); }
        }

        public ModuleInfo AgentModule
        {
            get { return NewsEntry.AgentModule; }
            set { throw new InvalidOperationException (); }
        }

        public ICollection<INewsEntry> Group
        {
            get { return NewsEntry.Group; }
            set { throw new InvalidOperationException (); }
        }

        public string DiscussProviderKey {
            get { return NewsEntry.DiscussProviderKey; }
            set { throw new InvalidOperationException (); }
        }

        public string DiscussEntryId {
            get { return NewsEntry.DiscussEntryId; }
            set { throw new InvalidOperationException (); }
        }

        #endregion

        bool? _hasImage;
        public bool HasImage => _hasImage ?? (_hasImage = NewsEntry.GetImage () != null).Value;

        public string Link
        {
            get { return NewsEntry.GetUrl (Context.Module.TabId, Context.Module.ModuleId); }
        }

        public string TitleLink
        {
            get {
                if (!string.IsNullOrEmpty (Link)) {
                    var targetAttr = string.Empty;
                    if (Globals.GetURLType (NewsEntry.Url) == TabType.Url) {
                        targetAttr = " target=\"_blank\"";
                    }
                    return string.Format ("<a href=\"{0}\"{2}>{1}</a>", Link, Title, targetAttr);
                }

                return Title;
            }
        }

        public string PublishedOnDateString
        {
            get {
                return this.PublishedOnDate ().ToString (
                    Localization.GetString ("PublishedOnDate.Format", Context.LocalResourceFile));
            }
        }

        public string CreatedByUserName
        {
            get {
                var user = ContentItem.CreatedByUser (Context.Module.PortalId);
                if (user != null) {
                    return user.DisplayName;
                }

                return Localization.GetString ("SystemUser.Text", Context.LocalResourceFile);
            }
        }

        public List<Badge> Badges
        {
            get {
                if (Context.Module.IsEditable) {
                    var badges = new List<Badge> ();
                    var now = HttpContext.Current.Timestamp;

                    if (!NewsEntry.IsPublished (now)) {
                        if (NewsEntry.HasBeenExpired (now)) {
                            badges.Add (new Badge {
                                CssClass = "badge-danger",
                                Text = string.Format (Localization.GetString (
                                    "Visibility_Expired.Format", Context.LocalResourceFile), NewsEntry.EndDate)
                            });
                        }
                        else {
                            badges.Add (new Badge {
                                CssClass = "badge-warning",
                                Text = string.Format (Localization.GetString (
                                    "Visibility_NotPublished.Format", Context.LocalResourceFile), NewsEntry.StartDate)
                            });
                        }
                    }

                    return badges;
                }

                return null;
            }
        }
    }
}
