using System;
using System.Windows.Forms;

namespace FFXIAHScrape.FFXIAHScrape._SharedModels
{
    public class GridRowInfo
    {
        public string Server { get; set; }
        public string Item { get; set; }
        public int Qty { get; set; }
        public string LastSale { get; set; }

        public GridRowInfo(DataGridViewRow row)
        {
            Server = row.Cells["Server"].Value.ToString().Trim();
            Item = row.Cells["ItemName"].Value.ToString().Trim();
            Qty = Convert.ToInt16(row.Cells["Stock"].Value.ToString().Trim());
            LastSale = row.Cells["LastSalePrice"].Value.ToString().Trim();
        }
    }
}