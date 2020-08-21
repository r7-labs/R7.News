using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Content.Taxonomy;

namespace R7.News.Controls
{
    public class TermSelector
    {
        public void InitTerms (ListControl listControl)
        {
            listControl.DataSource = GetTerms ();
            listControl.DataBind ();
        }

        protected IEnumerable<Term> GetTerms ()
        {
            var terms = new List<Term> ();
            var vocCtrl = new VocabularyController ();
            var vocs = vocCtrl.GetVocabularies ();
            foreach (var voc in vocs) {
                if (!voc.IsSystem) {
                    terms.AddRange (voc.Terms);
                }
            }
            return terms.OrderBy (t => t.Name);
        }

        public void SelectTerms (ListControl listControl, IEnumerable<Term> selectedTerms)
        {
            foreach (ListItem item in listControl.Items) {
                var itemId = int.Parse (item.Value);
                var term = selectedTerms.FirstOrDefault (t => t.TermId == itemId);
                if (term != null) {
                    item.Selected = true;
                }
            }
        }

        // TODO: Can use IList or IEnumerable here
        public List<Term> GetSelectedTerms (ListControl listControl)
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
