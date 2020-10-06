using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using R7.Dnn.Extensions.Modules;
using R7.Dnn.Extensions.ViewModels;
using R7.News.Data;
using R7.Dnn.Extensions.Controls;
using R7.Dnn.Extensions.Text;
using R7.News.Agent.Models;

namespace R7.News.Agent
{
    public partial class ModuleSettings : ModuleSettingsBase<AgentSettings>
    {
        ViewModelContext viewModelContext;
        protected ViewModelContext ViewModelContext {
            get { return viewModelContext ?? (viewModelContext = new ViewModelContext (this)); }
        }

        /// <summary>
        /// Handles the loading of the module setting for this control
        /// </summary>
        public override void LoadSettings ()
        {
            try {
                if (!IsPostBack) {
                    textThumbnailWidth.Text = Settings.ThumbnailWidth.ToString ();
                    txtImageCssClass.Text = Settings.ImageCssClass;
                    txtTextCssClass.Text = Settings.TextCssClass;
                    txtTopEntryTextCssClass.Text = Settings.TopEntryTextCssClass;
                    txtImageColumnCssClass.Text = Settings.ImageColumnCssClass;
                    txtTextColumnCssClass.Text = Settings.TextColumnCssClass;
                }
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }

        /// <summary>
        /// handles updating the module settings for this control
        /// </summary>
        public override void UpdateSettings ()
        {
            try {
                Settings.ThumbnailWidth = ParseHelper.ParseToNullable<int> (textThumbnailWidth.Text);

                Settings.ImageCssClass = !string.IsNullOrEmpty (txtImageCssClass.Text) ? txtImageCssClass.Text : null;
                Settings.TextCssClass = !string.IsNullOrEmpty (txtTextCssClass.Text) ? txtTextCssClass.Text : null;
                Settings.TopEntryTextCssClass = !string.IsNullOrEmpty (txtTopEntryTextCssClass.Text) ? txtTopEntryTextCssClass.Text : null;

                Settings.ImageColumnCssClass = !string.IsNullOrEmpty (txtImageColumnCssClass.Text) ? txtImageColumnCssClass.Text : null;
                Settings.TextColumnCssClass = !string.IsNullOrEmpty (txtTextColumnCssClass.Text) ? txtTextColumnCssClass.Text : null;

                SettingsRepository.SaveSettings (ModuleConfiguration, Settings);

                ModuleController.SynchronizeModule (ModuleId);
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }
    }
}

