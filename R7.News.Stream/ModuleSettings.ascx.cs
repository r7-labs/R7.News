using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Framework.JavaScriptLibraries;
using R7.Dnn.Extensions.Client;
using R7.Dnn.Extensions.Controls;
using R7.Dnn.Extensions.Collections;
using R7.Dnn.Extensions.Modules;
using R7.Dnn.Extensions.Text;
using R7.News.Components;
using R7.News.Controls;
using R7.News.Data;
using R7.News.Stream.Models;

namespace R7.News.Stream
{
    public partial class ModuleSettings : ModuleSettingsBase<StreamSettings>
    {
        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            JavaScript.RequestRegistration ("Select2");
            var select2Library = JavaScriptLibraryHelper.GetHighestVersionLibrary ("Select2");
            if (select2Library != null) {
                JavaScriptLibraryHelper.RegisterStyleSheet (select2Library, Page, "css/select2.min.css");
            }

            // fill weight comboboxes
            for (var i = 0; i <= NewsConfig.Instance.NewsEntry.MaxWeight; i++) {
                comboMinThematicWeight.Items.Add (i.ToString ());
                comboMaxThematicWeight.Items.Add (i.ToString ());
                comboMinStructuralWeight.Items.Add (i.ToString ());
                comboMaxStructuralWeight.Items.Add (i.ToString ());
            }

            rblPagerShowFirstLast.AddItem (LocalizeString ("DefaultValue.Text"), "null");
            rblPagerShowFirstLast.AddItem (LocalizeString ("Yes"), "true");
            rblPagerShowFirstLast.AddItem (LocalizeString ("No"), "false");
            rblPagerShowFirstLast.SelectedIndex = 0;

            rblPagerShowStatus.AddItem (LocalizeString ("DefaultValue.Text"), "null");
            rblPagerShowStatus.AddItem (LocalizeString ("Yes"), "true");
            rblPagerShowStatus.AddItem (LocalizeString ("No"), "false");
            rblPagerShowStatus.SelectedIndex = 0;

            var termSelector = new TermSelector ();
            termSelector.InitTerms (selIncludeTerms);
        }

        /// <summary>
        /// Handles the loading of the module setting for this control
        /// </summary>
        public override void LoadSettings ()
        {
            try {
                if (!IsPostBack) {

                    textThumbnailWidth.Text = Settings.ThumbnailWidth.ToString ();

                    checkUseShowMore.Checked = Settings.UseShowMore;
                    checkShowTopPager.Checked = Settings.ShowTopPager;
                    checkShowBottomPager.Checked = Settings.ShowBottomPager;
                    textPageSize.Text = Settings.PageSize.ToString ();
                    textMaxPageLinks.Text = Settings.MaxPageLinks.ToString ();

                    checkShowAllNews.Checked = Settings.ShowAllNews;

                    var terms = Settings.IncludeTerms;
                    if (!terms.IsNullOrEmpty ()) {
                        var termSelector = new TermSelector ();
                        termSelector.SelectTerms (selIncludeTerms, terms);
                    }

                    comboMinThematicWeight.SelectByValue (Settings.MinThematicWeight);
                    comboMaxThematicWeight.SelectByValue (Settings.MaxThematicWeight);
                    comboMinStructuralWeight.SelectByValue (Settings.MinStructuralWeight);
                    comboMaxStructuralWeight.SelectByValue (Settings.MaxStructuralWeight);

                    chkEnableFeed.Checked = Settings.EnableFeed;
                    txtFeedMaxEntries.Text = Settings.FeedMaxEntries?.ToString ();

                    txtImageCssClass.Text = Settings.ImageCssClass;
                    txtTextCssClass.Text = Settings.TextCssClass;
                    txtImageColumnCssClass.Text = Settings.ImageColumnCssClass;
                    txtTextColumnCssClass.Text = Settings.TextColumnCssClass;

                    if (Settings.PagerShowFirstLast != null) {
                        rblPagerShowFirstLast.SelectedIndex = Settings.PagerShowFirstLast.Value ? 1 : 2;
                    }

                    if (Settings.PagerShowStatus != null) {
                        rblPagerShowStatus.SelectedIndex = Settings.PagerShowStatus.Value ? 1 : 2;
                    }
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

                Settings.UseShowMore = checkUseShowMore.Checked;
                Settings.ShowTopPager = checkShowTopPager.Checked;
                Settings.ShowBottomPager = checkShowBottomPager.Checked;
                Settings.PageSize = int.Parse (textPageSize.Text);
                Settings.MaxPageLinks = int.Parse (textMaxPageLinks.Text);

                Settings.ShowAllNews = checkShowAllNews.Checked;

                var termSelector = new TermSelector ();
                Settings.IncludeTerms = termSelector.GetSelectedTerms (selIncludeTerms);

                var minThematicWeight = int.Parse (comboMinThematicWeight.SelectedValue);
                var maxThematicWeight = int.Parse (comboMaxThematicWeight.SelectedValue);
                var minStructuralWeight = int.Parse (comboMinStructuralWeight.SelectedValue);
                var maxStructuralWeight = int.Parse (comboMaxStructuralWeight.SelectedValue);

                // TODO: Implement custom validator for this?
                if (minThematicWeight > maxThematicWeight) {
                    minThematicWeight = maxThematicWeight;
                }

                if (minStructuralWeight > maxStructuralWeight) {
                    minStructuralWeight = maxStructuralWeight;
                }

                Settings.MinThematicWeight = minThematicWeight;
                Settings.MaxThematicWeight = maxThematicWeight;
                Settings.MinStructuralWeight = minStructuralWeight;
                Settings.MaxStructuralWeight = maxStructuralWeight;

                Settings.EnableFeed = chkEnableFeed.Checked;
                Settings.FeedMaxEntries = ParseHelper.ParseToNullable<int> (txtFeedMaxEntries.Text);

                Settings.ImageCssClass = !string.IsNullOrEmpty (txtImageCssClass.Text) ? txtImageCssClass.Text : null;
                Settings.TextCssClass = !string.IsNullOrEmpty (txtTextCssClass.Text) ? txtTextCssClass.Text : null;
                Settings.ImageColumnCssClass = !string.IsNullOrEmpty (txtImageColumnCssClass.Text) ? txtImageColumnCssClass.Text : null;
                Settings.TextColumnCssClass = !string.IsNullOrEmpty (txtTextColumnCssClass.Text) ? txtTextColumnCssClass.Text : null;

                Settings.PagerShowStatus = ParseHelper.ParseToNullable<bool> (rblPagerShowStatus.SelectedValue);
                Settings.PagerShowFirstLast = ParseHelper.ParseToNullable<bool> (rblPagerShowFirstLast.SelectedValue);

                SettingsRepository.SaveSettings (ModuleConfiguration, Settings);

                NewsRepository.Instance.ClearModuleCache (ModuleId, TabModuleId);

                ModuleController.SynchronizeModule (ModuleId);
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }
    }
}

