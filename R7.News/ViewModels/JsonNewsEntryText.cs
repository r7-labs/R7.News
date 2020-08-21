using System.Web;
using Newtonsoft.Json;
using R7.News.Models;

namespace R7.News.ViewModels
{
    [JsonObject (MemberSerialization.OptIn)]
    public class JsonNewsEntryText: INewsEntryTextWritable
    {
        [JsonProperty ("entryTextId")]
        public int EntryTextId { get; set; }

        [JsonProperty ("entryId")]
        public int EntryId { get; set; }

        [JsonProperty ("text")]
        public string Text { get; set; }

        [JsonProperty ("rawText")]
        public string RawText => HttpUtility.HtmlDecode (Text);
    }
}
