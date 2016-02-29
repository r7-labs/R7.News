//
//  UniversityDivisionNewsSource.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016 Roman M. Yagodin
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using R7.News.Models;

namespace R7.News.Integrations
{
    // TODO: Move to separate assembly?

    public class UniversityDivisionInfo
    {
        public int DivisionID { get; }

        public string Title { get; }
            
        public string HomePage { get; }
    }

    public class UniversityDivisionNewsSource: INewsSource
    {
        protected UniversityDivisionInfo Division;

        #region INewsSource implementation

        public string Key
        {
            get { return "UniversityDivisionID"; }
        }

        public int ItemId 
        { 
            get { return Division.DivisionID; }
        }

        public string Title 
        { 
            get { return Division.Title; }
        }

        public string Url 
        { 
            get { return Division.HomePage; }
        }

        #endregion
    }
}

