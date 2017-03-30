//
//  DiscussProvider.cs
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

using R7.News.Components;
using R7.News.Models;

namespace R7.News.Providers.DiscussProviders
{
    public class DiscussProvider: IDiscussProvider
    {
        public DiscussProvider (ForumProvider forumProvider)
        {
            switch (forumProvider) {
                case ForumProvider.DnnForum: Implementation = new DnnForumDiscussProvider (); break;
                case ForumProvider.ActiveForums: Implementation = new ActiveForumsDiscussProvider (); break;
            }
        }

        protected IDiscussProvider Implementation;

        public bool IsAvailable
        {
            get { return Implementation.IsAvailable; }
        }

        public int Discuss (INewsEntry newsEntry, int tabId, int moduleId, int portalId, int userId, int forumId)
        {
            return Implementation.Discuss (newsEntry, tabId, moduleId, portalId, userId, forumId);
        }

        public string GetDiscussUrl (int tabId, int forumId, int discussId)
        {
            return Implementation.GetDiscussUrl (tabId, forumId, discussId);
        }
    }
}
