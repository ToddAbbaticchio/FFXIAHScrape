using System.Collections.Generic;

namespace FFXIAHScrape.Models
{
    public class ItemQueryReturnBase
    {
        public string Server { get; set; }
        public string ItemName { get; set; }
    }

    public class RareItemAlertReturn : ItemQueryReturnBase
    {
        public string Stock { get; set; }
        public string LastSalePrice { get; set; }
    }

    public class UndercutAlertReturn : ItemQueryReturnBase
    {
        public string MyListPrice { get; set; }
        public string LastSalePrice { get; set; }
    }

    public class XServerArbitrageReturn : ItemQueryReturnBase
    {
        public string Stock { get; set; }
        public string Rate { get; set; }
        public string LastSalePrice { get; set; }
    }
}