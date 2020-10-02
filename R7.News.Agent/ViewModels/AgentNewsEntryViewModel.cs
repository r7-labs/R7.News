using R7.Dnn.Extensions.ViewModels;
using R7.News.Agent.Models;
using R7.News.Components;
using R7.News.Models;
using R7.News.ViewModels;

namespace R7.News.Agent.ViewModels
{
    public class AgentNewsEntryViewModel: NewsEntryViewModelBase
    {
        public AgentNewsEntryViewModel (INewsEntry newsEntry, ViewModelContext<AgentSettings> context, AgentModuleConfig config) :
            base (newsEntry, context)
        {
            Config = config;
        }

        public bool IsTopEntry { get; set; }

        protected AgentModuleConfig Config;

        protected AgentSettings Settings
        {
            get { return ((ViewModelContext<AgentSettings>) Context).Settings; }
        }

        public string ImageUrl
        {
            get { return NewsEntry.GetImageUrl (width: Settings.ThumbnailWidth ?? Config.DefaultThumbnailWidth); }
        }

        public string GroupImageUrl
        {
            get { return NewsEntry.GetImageUrl (width: Settings.GroupThumbnailWidth ?? Config.DefaultGroupThumbnailWidth); }
        }

        public string ImageCssClass => Settings.ImageCssClass ?? Config.ImageCssClass;

        public string TextCssClass => IsTopEntry ? (Settings.TopEntryTextCssClass ?? Config.TopEntryTextCssClass) : (Settings.TextCssClass ?? Config.TextCssClass);

        public string ImageColumnCssClass
        {
            get { return HasImage ? (Settings.ImageColumnCssClass ?? Config.ImageColumnCssClass) : Const.NoImageColumnCssClass; }
        }

        public string TextColumnCssClass
        {
            get { return HasImage ? (Settings.TextColumnCssClass ?? Config.TextColumnCssClass) : Const.NoImageTextColumnCssClass; }
        }
    }
}

