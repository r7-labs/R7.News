//
//  StreamModuleViewModel.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016-2019 Roman M. Yagodin
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using DotNetNuke.Common;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using R7.Dnn.Extensions.Text;
using R7.Dnn.Extensions.ViewModels;
using R7.News.Models;
using R7.News.Stream.Models;

namespace R7.News.Stream.ViewModels
{
    public class StreamModuleViewModel
    {
        protected ViewModelContext Context;

        protected ModuleInfo Module;

        protected int NewsEntry_ThematicWeight;

        protected int NewsEntry_StructuralWeight;

        protected IList<Term> NewsEntry_Terms;

        protected StreamSettings Settings;

        #region Bindable properites

        public int ModuleId
        { 
            get { return Module.ModuleID; }
        }

        public string ModuleTitle
        { 
            get { return Module.ModuleTitle; }
        }

        public string ModuleLink
        { 
            get { 
                return string.Format ("<a href=\"{1}\" target=\"_blank\">{0}</a>", ModuleTitle,
                    Globals.NavigateURL (Module.TabID) + "#" + ModuleId
                );
            }
        }

        public string PassesByString
        {
            get { return GetPassesByString (); }
        }

        #endregion

        public StreamModuleViewModel (ModuleInfo module, StreamSettings settings, ViewModelContext context, int newsEntryThematicWeight, int newsEntryStructuralWeight, 
            IList<Term> newsEntryTerms)
        {
            Module = module;
            Settings = settings;
            Context = context;

            NewsEntry_ThematicWeight = newsEntryThematicWeight;
            NewsEntry_StructuralWeight = newsEntryStructuralWeight;
            NewsEntry_Terms = newsEntryTerms;
        }

        public static bool IsNewsEntryWillBePassedByModule (
            StreamSettings settings,
            int thematicWeight,
            int structuralWeight,
            IList<Term> terms)
        {
            return (settings.ShowAllNews || ModelHelper.IsTermsOverlaps (terms, settings.IncludeTerms))
            && ModelHelper.IsVisible (thematicWeight, structuralWeight, 
                settings.MinThematicWeight, settings.MaxThematicWeight,
                settings.MinStructuralWeight, settings.MaxStructuralWeight
            );
        }

        protected string GetPassesByString ()
        {
            var passesBy = new Collection<string> ();

            var passesByTerms = false;
            if (Settings.ShowAllNews) {
                passesBy.Add (Localization.GetString ("PassesByShowAllSetting.Text", Context.LocalResourceFile));
                passesByTerms = true;
            }
            else if (ModelHelper.IsTermsOverlaps (NewsEntry_Terms, Settings.IncludeTerms)) {
                passesBy.Add (Localization.GetString ("PassesByTerms.Text", Context.LocalResourceFile));
                passesByTerms = true;
            }

            if (passesByTerms) {
                if (ModelHelper.IsThematicVisible (
                        NewsEntry_ThematicWeight,
                        Settings.MinThematicWeight,
                        Settings.MaxThematicWeight)) {
                    passesBy.Add (Localization.GetString ("PassesByThematicWeight.Text", Context.LocalResourceFile));
                }
                if (ModelHelper.IsStructuralVisible (
                        NewsEntry_StructuralWeight,
                        Settings.MinStructuralWeight,
                        Settings.MaxStructuralWeight)) {
                    passesBy.Add (Localization.GetString ("PassesByStructuralWeight.Text", Context.LocalResourceFile));
                }
            }

            return FormatHelper.JoinNotNullOrEmpty (", ", passesBy); 
        }
    }
}
