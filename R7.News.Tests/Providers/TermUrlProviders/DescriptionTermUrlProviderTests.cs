//
//  DescriptionTermUrlProviderTests.cs
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

using DotNetNuke.Entities.Content.Taxonomy;
using R7.News.Providers.TermUrlProviders;
using Xunit;

namespace R7.News.Tests.Providers.TermUrlProviders
{
    public class DescriptionTermUrlProviderTests
    {
        [Fact]
        public void GetUrlTest ()
        {
            var provider = new DescriptionTermUrlProvider ();
            Assert.Equal ("1", provider.GetUrl (new Term {Description = "1"}));
            Assert.Equal ("http://www.bar.com", provider.GetUrl (new Term { Description = "http://www.bar.com" }));
            Assert.Equal ("ftps://bar.com", provider.GetUrl (new Term { Description = "ftps://bar.com" }));
            Assert.Equal ("//bar.com", provider.GetUrl (new Term { Description = "//bar.com" }));
            Assert.Equal ("mailto:foo@bar.com", provider.GetUrl (new Term { Description = "mailto:foo@bar.com" }));
            Assert.Equal (string.Empty, provider.GetUrl (new Term { Description = "/LinkClick.aspx?link=10" }));
            Assert.Equal (string.Empty, provider.GetUrl (new Term { Description = "Hello!" }));
        }
    }
}
