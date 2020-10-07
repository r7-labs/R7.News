using System;
using System.Text;
using System.Web.UI;
using DotNetNuke.Services.Localization;
using DnnWebUiUtilities = DotNetNuke.Web.UI.Utilities;

namespace R7.News.Controls
{
    public class JsVars: UserControl
    {
        string _localResourceFile;

        protected string LocalResourceFile {
            get {
                if (_localResourceFile == null) {
                    _localResourceFile = DnnWebUiUtilities.GetLocalResourceFile (this);
                }
                return _localResourceFile;
            }
        }

        public string LocalizationResources
        {
            get {
                var sb = new StringBuilder ();
                sb.AppendFormat ("errorLoadingExpandedText:'{0}'", Localization.GetString ("ErrorLoadingExpandedText.Text", LocalResourceFile));
                return sb.ToString ();
            }
        }
    }
}
