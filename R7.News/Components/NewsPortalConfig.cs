using System.Collections.Generic;
using R7.News.Providers.DiscussProviders;
using R7.News.Providers.TermUrlProviders;

namespace R7.News.Components
{
    public class NewsPortalConfig
    {
        public int DataCacheTime { get; set; } = 60;

        public string DefaultImagesPath { get; set; } = "images";

        public StreamModuleConfig StreamModule { get; set; } = new StreamModuleConfig ();

        public AgentModuleConfig AgentModule { get; set; } = new AgentModuleConfig ();

        public NewsEntryConfig NewsEntry { get; set; } = new NewsEntryConfig ();

        public FeedsConfig Feeds { get; set; } = new FeedsConfig ();

        #region TermUrl providers

        public List<TermUrlProviderConfig> TermUrlProviders { get; set; } = new List<TermUrlProviderConfig> {
            new TermUrlProviderConfig {
                Type = typeof (DescriptionTermUrlProvider).FullName
            },
        };

        protected readonly List<ITermUrlProvider> TermUrlProvidersInternal = new List<ITermUrlProvider> ();
        public List<ITermUrlProvider> GetTermUrlProviders () => TermUrlProvidersInternal;

        #endregion

        #region Discuss providers

        public List<DiscussProviderConfig> DiscussProviders { get; set; } = new List<DiscussProviderConfig> ();

        protected readonly List<IDiscussProvider> DiscussProviders_Internal = new List<IDiscussProvider> ();
        public List<IDiscussProvider> GetDiscussProviders () => DiscussProviders_Internal;

        #endregion

        public void AddProvider<TProvider> (TProvider provider, IProviderConfig providerConfig)
        {
            if (provider is ITermUrlProvider) {
                TermUrlProvidersInternal.Add ((ITermUrlProvider)provider);
            }
            else if (provider is IDiscussProvider) {
                var discussProvider = (IDiscussProvider) provider;
                discussProvider.Params = providerConfig.Params;
                discussProvider.ProviderKey = ((DiscussProviderConfig) providerConfig).ProviderKey;
                DiscussProviders_Internal.Add (discussProvider);
            }
        }

        public NodeManipulatorConfig NodeManipulator { get; set; }
    }

    public class NewsEntryConfig
    {
        public int MaxWeight { get; set; } = 10;

        public int DefaultThematicWeight { get; set; } = 10;

        public int DefaultStructuralWeight { get; set; } = 10;
    }

    public class StreamModuleConfig
    {
        public int DefaultThumbnailWidth { get; set; } = 768;

        public string ImageCssClass { get; set; } = "img-thumbnail";

        public string TextCssClass { get; set; }

        public string ImageColumnCssClass { get; set; } = "col-md-4";

        public string TextColumnCssClass { get; set; } = "col-md-8";

        public bool PagerShowFirstLast { get; set; }

        public bool PagerShowStatus { get; set; }
    }

    public class AgentModuleConfig
    {
        public int DefaultThumbnailWidth { get; set; } = 768;

        public string ImageCssClass { get; set; } = "img-thumbnail";

        public string TextCssClass { get; set; }

        public string TopEntryTextCssClass { get; set; } = "lead";

        public string ImageColumnCssClass { get; set; } = "col-md";

        public string TextColumnCssClass { get; set; } = "col-md";
    }

    public enum PermalinkMode
    {
        Friendly,
        Raw
    }

    public interface IProviderConfig
    {
        string Type { get; set; }

        List<string> Params { get; set; }
    }

    public class TermUrlProviderConfig : IProviderConfig
    {
        public string Type { get; set; }

        public List<string> Params { get; set; }
    }

    public class DiscussProviderConfig: IProviderConfig
    {
        public string Type { get; set; }

        public List<string> Params { get; set; }

        public string ProviderKey { get; set; }
    }

    public class NodeManipulatorConfig
    {
        public int ParentNodeTabId { get; set; }

        public int StreamModuleId { get; set; }

        public int StreamModuleTabId { get; set; }
    }

    public class FeedsConfig
    {
        public int DefaultMaxEntries { get; set; } = 10;

        public bool EnableRss { get; set; } = true;

        public bool EnableAtom { get; set; } = true;
    }
}
