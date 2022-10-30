using System.Collections.Generic;

namespace FFXIAHScrape.Models
{
    public class PriceInfo
    {
        public string Server { get; set; }
        public string ItemName { get; set; }
        public string Stock { get; set; }
        public string Rate { get; set; }
        public string Median { get; set; }
        public List<string> Last3 { get; set; }
    }
}