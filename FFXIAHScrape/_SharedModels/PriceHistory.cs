namespace FFXIAHScrape.FFXIAHScrape._SharedModels
{

    public class PriceHistory
    {
        public int saleon { get; set; }
        public int seller { get; set; }
        public int buyer { get; set; }
        public int price { get; set; }
        public string seller_name { get; set; }
        public int seller_id { get; set; }
        public int seller_server { get; set; }
        public int buyer_id { get; set; }
        public string buyer_name { get; set; }
        public int buyer_server { get; set; }
    }
}