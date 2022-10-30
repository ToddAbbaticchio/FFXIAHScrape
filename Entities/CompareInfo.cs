using System.Collections.Generic;

namespace FFXIAHScrape.Entities
{
    public class CompareInfo
    {
        public string ItemName { get; set; }
        public string S1_Stock { get; set; }
        public string S1_Rate { get; set; }
        public string S1_Median { get; set; }
        public List<string> S1_Last3 { get; set; }
        public string S2_Stock { get; set; }
        public string S2_Rate { get; set; }
        public string S2_Median { get; set; }
        public List<string> S2_Last3 { get; set; }
    }
}