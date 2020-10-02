using R7.Dnn.Extensions.ViewModels;
using R7.News.Components;
using R7.News.Models;
using R7.News.Stream.Models;
using R7.News.ViewModels;

namespace R7.News.Stream.ViewModels
{
    public class NewsEntryViewModel: NewsEntryViewModelBase
    {
        public NewsEntryViewModel (INewsEntry newsEntry, ViewModelContext<StreamSettings> context, AgentModuleConfig config) :
            base (newsEntry, context)
        {
            Config = config;
        }

        // TODO: Introduce separate config options and maybe module settings GH-119
        protected AgentModuleConfig Config;

        public string ImageUrl
        {
            get { return NewsEntry.GetImageUrl (width: Config.DefaultThumbnailWidth); }
        }

        public string ImageCssClass => Config.ImageCssClass;

        public string TextCssClass => "lead";

        public string ImageColumnCssClass
        {
            get { return HasImage ? Config.ImageColumnCssClass : Const.NoImageColumnCssClass; }
        }

        public string TextColumnCssClass
        {
            get { return HasImage ? Config.TextColumnCssClass : Const.NoImageTextColumnCssClass; }
        }
    }
}

