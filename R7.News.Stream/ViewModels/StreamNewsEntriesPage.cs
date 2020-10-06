using System;
using System.Collections;
using System.Collections.Generic;

namespace R7.News.Stream.ViewModels
{
    public struct StreamNewsEntriesPage
    {
        public IList<StreamNewsEntry> Page { get; private set; }

        public int TotalItems { get; private set; }

        public static StreamNewsEntriesPage Empty
        {
            get { return new StreamNewsEntriesPage (0, null); }
        }

        public StreamNewsEntriesPage (int totalItems, IList<StreamNewsEntry> page)
        {
            TotalItems = totalItems;
            Page = page;
        }
    }
}
