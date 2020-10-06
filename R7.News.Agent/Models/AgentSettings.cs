using System;
using DotNetNuke.Entities.Modules.Settings;
using R7.News.Components;

namespace R7.News.Agent.Models
{
    /// <summary>
    /// Provides strong typed access to settings used by module
    /// </summary>
    [Serializable]
    public class AgentSettings
    {
        protected const string SettingPrefix = Const.Prefix + "_Agent_";

        [TabModuleSetting (Prefix = SettingPrefix)]
        public int? ThumbnailWidth { get; set; }

        [TabModuleSetting (Prefix = SettingPrefix)]
        public string ImageCssClass { get; set; }

        [TabModuleSetting (Prefix = SettingPrefix)]
        public string TextCssClass { get; set; }

        [TabModuleSetting (Prefix = SettingPrefix)]
        public string TopEntryTextCssClass { get; set; }

        [TabModuleSetting (Prefix = SettingPrefix)]
        public string ImageColumnCssClass { get; set; }

        [TabModuleSetting (Prefix = SettingPrefix)]
        public string TextColumnCssClass { get; set; }
    }

    public class AgentSettingsRepository : SettingsRepository<AgentSettings>
    {
    }
}
