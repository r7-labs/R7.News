//
//  ForumDiscussParams.cs
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

using System.Collections.Generic;

namespace R7.News.Providers.DiscussProviders
{
    internal struct ForumDiscussParams
    {
        public int TabId;

        public int ModuleId;

        public int ForumId;

        public static ForumDiscussParams Parse (IList<string> providerParams)
        {
            return new ForumDiscussParams {
                TabId = int.Parse (providerParams [0]),
                ModuleId = int.Parse (providerParams [1]),
                ForumId = int.Parse (providerParams [2])
            };
        }
    }
}
