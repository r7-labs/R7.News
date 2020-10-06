using System;
using System.Collections.Generic;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Modules;
using R7.News.Components;

namespace R7.News.Models
{
    public interface INewsEntry
    {
        int EntryId { get; set; }

        int PortalId { get; set; }

        int ContentItemId { get; set; }

        int? AgentModuleId { get; set; }

        string Url { get; set; }

        DateTime? StartDate { get; set; }

        DateTime? EndDate { get; set; }

        DateTime? ThresholdDate { get; set; }

        DateTime? DueDate { get; set; }

        string Title { get; set; }

        string Description { get; set; }

        int? EntryTextId { get; set; }

        string Text { get; set; }

        int ThematicWeight { get; set; }

        int StructuralWeight { get; set; }

        ContentItem ContentItem { get; set; }

        ModuleInfo AgentModule { get; set; }

        string DiscussProviderKey { get; set; }

        string DiscussEntryId { get; set; }
    }

    [TableName (Const.Prefix)]
    [PrimaryKey ("EntryId", AutoIncrement = true)]
    [Scope ("PortalId")]
    public class NewsEntry: INewsEntry
    {
        #region INewsEntry implementation

        public int EntryId { get; set; }

        public int PortalId { get; set; }

        public int ContentItemId { get; set; }

        public int? AgentModuleId { get; set; }

        public string Url { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? ThresholdDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? EntryTextId { get; set; }

        [IgnoreColumn]
        public string Text { get; set; }

        public int ThematicWeight { get; set; }

        public int StructuralWeight { get; set; }

        [IgnoreColumn]
        public ContentItem ContentItem { get; set; }

        [IgnoreColumn]
        public ModuleInfo AgentModule { get; set; }

        public string DiscussProviderKey { get; set; }

        public  string DiscussEntryId { get; set; }

        #endregion
    }
}
