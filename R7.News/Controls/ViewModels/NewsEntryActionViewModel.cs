//
//  NewsEntryActionViewModel.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2017 Roman M. Yagodin
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

using DotNetNuke.Services.Localization;
using R7.Dnn.Extensions.ViewModels;
using R7.News.Controls.Models;

namespace R7.News.Controls.ViewModels
{
    public class NewsEntryActionViewModel: NewsEntryAction
    {
        protected ViewModelContext Context;

        public NewsEntryActionViewModel (NewsEntryAction action, ViewModelContext context)
        {
            Action = action.Action;
            EntryId = action.EntryId;
            Params = action.Params;
            Enabled = action.Enabled;
            Context = context;
        }

        public string Text {
            get {
                var text = Localization.GetString (Action, Context.LocalResourceFile);
                for (var i = 0; i < Params.Length; i++) {
                    text = text.Replace ("{" + i + "}", Params [i]);
                }
                return text;
            }
        }

        public string Title {
            get { return Localization.GetString (Action + ".Title", Context.LocalResourceFile); }
        }
    }
}
