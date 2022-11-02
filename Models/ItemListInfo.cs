namespace FFXIAHScrape.Models
{
    public class ItemListInfoBase
    {
        public string Item { get; set; }
        public string Id { get; set; }
        public bool Stack { get; set; }
    }

    public class UndercutAlertInfo : ItemListInfoBase
    {
        public string MyPrice { get; set; }
    }
}