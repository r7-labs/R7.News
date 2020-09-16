//
//  AgplSignature.cs
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

using System;
using System.Reflection;
using System.Web.UI;
using DotNetNuke.Services.Localization;
using DnnWebUiUtilities = DotNetNuke.Web.UI.Utilities;

namespace R7.News.Controls
{
    public class AgplSignature : UserControl
    {
        private bool showRule = true;

        public bool ShowRule {
            get { return showRule; }
            set { showRule = value; }
        }

        private string localResourceFile;

        protected string LocalResourceFile {
            get {
                if (localResourceFile == null) {
                    localResourceFile = DnnWebUiUtilities.GetLocalResourceFile (this);
                }

                return localResourceFile;
            }
        }

        protected string LocalizeString (string key)
        {
            return Localization.GetString (key, LocalResourceFile);
        }

        protected string GetVersionString ()
        {
            var assembly = Assembly.GetExecutingAssembly ();
            var assemblyInformationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute> ();
            if (assemblyInformationalVersion != null) {
                return assemblyInformationalVersion.InformationalVersion;
            }

            return assembly.GetName ().Version.ToString (3);
        }

        protected string AppName {
            get { return Assembly.GetExecutingAssembly ().GetName ().Name; }
        }
    }
}
