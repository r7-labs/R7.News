﻿//
//  StreamSettings.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016-2020 Roman M. Yagodin
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Modules.Settings;
using R7.Dnn.Extensions.Text;
using R7.News.Components;

namespace R7.News.Stream.Models
{
    /// <summary>
    /// Provides strong typed access to settings used by module
    /// </summary>
    [Serializable]
    public class StreamSettings
    {
        protected const string SettingPrefix = Const.Prefix + "_Stream_";

        public StreamSettings ()
        {
            if (HttpContext.Current != null) {
                MaxThematicWeight = NewsConfig.Instance.NewsEntry.MaxWeight;
                MaxStructuralWeight = NewsConfig.Instance.NewsEntry.MaxWeight;
            }
        }

        [ModuleSetting (Prefix = SettingPrefix, ParameterName = "IncludeTerms")]
        public string IncludeTerms_Internal { get; set; } = string.Empty;

        public List<Term> IncludeTerms {
            get {
                var termController = new TermController ();

                var termIds = IncludeTerms_Internal
                    .Split (new [] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select (ti => int.Parse (ti));

                var terms = new List<Term> ();
                foreach (var termId in termIds) {
                    var term = termController.GetTerm (termId);
                    if (term != null) {
                        terms.Add (term);
                    }
                }

                return terms;
            }

            set {
                IncludeTerms_Internal = FormatHelper.JoinNotNullOrEmpty (";", value.Select (t => t.TermId.ToString ()));
            }
        }

        [ModuleSetting (Prefix = SettingPrefix)]
        public bool ShowAllNews { get; set; } = false;

        // TODO: Separate config settings for weight filters?
        [ModuleSetting (Prefix = SettingPrefix)]
        public int MinThematicWeight { get; set; } = 0;

        [ModuleSetting (Prefix = SettingPrefix)]
        public int MaxThematicWeight { get; set; }

        [ModuleSetting (Prefix = SettingPrefix)]
        public int MinStructuralWeight { get; set; } = 0;

        [ModuleSetting (Prefix = SettingPrefix)]
        public int MaxStructuralWeight { get; set; }

        [TabModuleSetting (Prefix = SettingPrefix)]
        public bool UseShowMore { get; set; } = true;

        [TabModuleSetting (Prefix = SettingPrefix)]
        public bool ShowTopPager { get; set; } = false;

        [TabModuleSetting (Prefix = SettingPrefix)]
        public bool ShowBottomPager { get; set; } = false;

        [TabModuleSetting (Prefix = SettingPrefix)]
        public int PageSize { get; set; } = 3;

        [TabModuleSetting (Prefix = SettingPrefix)]
        public int MaxPageLinks { get; set; } = 3;

        [TabModuleSetting (Prefix = SettingPrefix)]
        public int? ThumbnailWidth { get; set; }

        [TabModuleSetting (Prefix = SettingPrefix)]
        public bool EnableFeed { get; set; } = false;

        [TabModuleSetting (Prefix = SettingPrefix)]
        public int? FeedMaxEntries { get; set; }

        [TabModuleSetting (Prefix = SettingPrefix)]
        public string ImageCssClass { get; set; }

        [TabModuleSetting (Prefix = SettingPrefix)]
        public string TextCssClass { get; set; }

        [TabModuleSetting (Prefix = SettingPrefix)]
        public string ImageColumnCssClass { get; set; }

        [TabModuleSetting (Prefix = SettingPrefix)]
        public string TextColumnCssClass { get; set; }

        [TabModuleSetting (Prefix = SettingPrefix)]
        public bool? PagerShowFirstLast { get; set; }

        [TabModuleSetting (Prefix = SettingPrefix)]
        public bool? PagerShowStatus { get; set; }
    }

    public class StreamSettingsRepository : SettingsRepository<StreamSettings>
    {
    }
}
