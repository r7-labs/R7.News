//
//  TermSelector.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2020 Roman M. Yagodin
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
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Content.Taxonomy;

namespace R7.News.Controls
{
    public static class TermSelector
    {
        public static void InitTerms (ListControl listControl)
        {
            // TODO: Add more vocabularies
            // TODO: Configure list of vocabularies
            var termCtrl = new TermController ();
            var terms = termCtrl.GetTermsByVocabulary ("University_Structure")
               .OrderBy (t => t.Name)
               .ToList ();

            listControl.DataSource = terms;
            listControl.DataBind ();
        }

        public static void SelectTerms (ListControl listControl, IEnumerable<Term> selectedTerms)
        {
            foreach (ListItem item in listControl.Items) {
                var itemId = int.Parse (item.Value);
                var term = selectedTerms.FirstOrDefault (t => t.TermId == itemId);
                if (term != null) {
                    item.Selected = true;
                }
            }
        }

        // TODO: Return IList of IEnumerable
        public static List<Term> GetSelectedTerms (ListControl listControl)
        {
            var termCtrl = new TermController ();
            var selectedTerms = new List<Term> ();
            foreach (ListItem item in listControl.Items) {
                if (item.Selected) {
                    var term = termCtrl.GetTerm (int.Parse (item.Value));
                    if (term != null) {
                        selectedTerms.Add (term);
                    }
                }
            }

            return selectedTerms;
        }
    }
}
