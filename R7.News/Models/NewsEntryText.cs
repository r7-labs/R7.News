using DotNetNuke.ComponentModel.DataAnnotations;
using R7.News.Components;

namespace R7.News.Models
{
    public interface INewsEntryText
    {
        int EntryTextId { get; }

        int EntryId { get; }

        string Text { get; }
    }

    public interface INewsEntryTextWritable : INewsEntryText
    {
        new int EntryTextId { get; set; }

        new int EntryId { get; set; }

        new string Text { get; set; }
    }

    [TableName (Const.Prefix + "_Texts")]
    [PrimaryKey ("EntryTextId", AutoIncrement = true)]
    public class NewsEntryText: INewsEntryTextWritable
    {
        public int EntryTextId { get; set; }

        public int EntryId { get; set; }

        public string Text { get; set; }
    }
}
