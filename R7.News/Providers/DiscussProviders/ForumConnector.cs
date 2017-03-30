//
//  ForumConnector.cs
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
using DotNetNuke.Entities.Content.Taxonomy;
using R7.News.Components;

namespace R7.News.Providers.DiscussProviders
{
    public class ForumConnector: IForumConnector
    {
        public ForumConnector (ForumProvider forumProvider)
        {
            switch (forumProvider) {
                case ForumProvider.DnnForum: Implementation = new DnnForumConnector (); break;
                case ForumProvider.ActiveForums: Implementation = new ActiveForumsConnector (); break;
            }
        }

        protected IForumConnector Implementation;

        public bool IsAvailable
        {
            get { return Implementation.IsAvailable; }
        }

        public int AddPost (string postSubject, string postBody, int tabId, int moduleId, int portalId, int userId, int forumId, List<Term> terms)
        {
            return Implementation.AddPost (postSubject, postBody, tabId, moduleId, portalId, userId, forumId, terms);
        }

        public string GetPostUrl (int tabId, int forumId, int postId)
        {
            return Implementation.GetPostUrl (tabId, forumId, postId);
        }
    }
}
