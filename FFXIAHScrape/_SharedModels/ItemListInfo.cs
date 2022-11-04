namespace FFXIAHScrape.FFXIAHScrape._SharedModels
{
    public class ItemListInfoBase
    {
        public string Server { get; set; }
        public string Item { get; set; }
        public string Id { get; set; }
        public bool Stack { get; set; }
    }

    public class UndercutAlertItemInfo : ItemListInfoBase
    {
        public string MyPrice { get; set; }
    }

    public class RareItemAlertItemInfo : ItemListInfoBase
    {
    }

    public class XServerItemInfo : ItemListInfoBase
    {
    }
}