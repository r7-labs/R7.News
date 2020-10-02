using R7.Dnn.Extensions.ViewModels;
using R7.News.Components;
using R7.News.Models;
using R7.News.Stream.Models;
using R7.News.ViewModels;

namespace R7.News.Stream.ViewModels
{
    public class NewsEntryViewModel: NewsEntryViewModelBase
    {
        public NewsEntryViewModel (INewsEntry newsEntry, ViewModelContext<StreamSettings> context, StreamModuleConfig config) :
            base (newsEntry, context)
        {
            Config = config;
        }

        protected StreamModuleConfig Config;

        protected StreamSettings Settings
        {
            get { return ((ViewModelContext<StreamSettings>) Context).Settings; }
        }

        public string ImageUrl
        {
            get { return NewsEntry.GetImageUrl (width: Settings.ThumbnailWidth ?? Config.DefaultThumbnailWidth); }
        }

        // TODO: Introduce config options and maybe module settings

        public string ImageCssClass => "img-thumbnail";

        public string TextCssClass => "lead";

        public string ImageColumnCssClass
        {
            get { return HasImage ? "col-md" : Const.NoImageColumnCssClass; }
        }

        public string TextColumnCssClass
        {
            get { return HasImage ? "col-md" : Const.NoImageTextColumnCssClass; }
        }
    }
}

