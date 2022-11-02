using FFXIAHScrape.Entities;
using FFXIAHScrape.FFXIAHScrape._SharedModels;
using FFXIAHScrape.FFXIAHScrape.RareItemAlert;
using FFXIAHScrape.FFXIAHScrape.UndercutAlert;
using FFXIAHScrape.FFXIAHScrape.XServerArbitrage;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Windows.Forms;

namespace FFXIAHScrape
{
    public partial class Form1 : Form
    {
        public bool AutoRefreshMode { get; set; }
        public int RefreshTimer { get; set; }

        readonly UndercutAlert _undercutAlert = new UndercutAlert();
        readonly RareItemAlert _rareItemAlert = new RareItemAlert();
        readonly XServerArbitrage _xServerArbitrage = new XServerArbitrage();

        public Form1()
        {
            InitializeComponent();

            ModeDrop.DataSource = Enum.GetValues(typeof(Modes));

            var autoCompleteArray = Items.Info.Keys.ToArray();
            var source = new AutoCompleteStringCollection();
            source.AddRange(autoCompleteArray);

            TextBox.AutoCompleteCustomSource = source;

            var serverList1 = Servers.Info.Keys.ToList();
            serverList1.Add("-- Choose Server 1 --");
            serverList1.Sort();
            Server1Drop.DataSource = serverList1;

            var serverList2 = Servers.Info.Keys.ToList();
            serverList2.Add("-- Choose Server 2 --");
            serverList2.Sort();
            Server2Drop.DataSource = serverList2;
        }

        private void ModeDrop_SelectedIndexChanged(object sender, EventArgs e)
        {
            var stackCheckPos1 = new System.Drawing.Point(172, 107);
            var stackCheckPos2 = new System.Drawing.Point(164, 130);
            var goButtonPos1 = new System.Drawing.Point(139, 139);
            var goButtonPos2 = new System.Drawing.Point(142, 153);

            switch (ModeDrop.SelectedValue)
            {
                case Modes.RareItemAlert:
                    Server2Drop.SelectedIndex = -1;
                    Server2Drop.Hide();
                    ListPrice.Hide();
                    StackCheckBox.Location = stackCheckPos1;
                    GoButton.Location = goButtonPos1;
                    RefreshTimer = 60;
                    ListPrice.Text = "";
                    break;
                case Modes.UndercutAlert:
                    Server2Drop.SelectedIndex = -1;
                    Server2Drop.Hide();
                    ListPrice.Show();
                    ListPrice.Text = "Enter Listed Price...";
                    StackCheckBox.Location = stackCheckPos2;
                    GoButton.Location = goButtonPos2;
                    RefreshTimer = 300;
                    break;
                case Modes.XServerArbitrage:
                    Server2Drop.Show();
                    ListPrice.Hide();
                    StackCheckBox.Location = stackCheckPos1;
                    GoButton.Location = goButtonPos1;
                    Server2Drop.SelectedIndex = 0;
                    RefreshTimer = 3600;
                    ListPrice.Text = "";
                    break;
            }

            GoButton.Text = "Check FFXI-AH";
            AutoRefreshMode = false;
            ModeDisplay.Text = "";
            StackCheckBox.Checked = false;
            ItemList.Items.Clear();
            Result1Grid.DataSource = null;
        }

        private void Server1Drop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Server1Drop.SelectedIndex == -1 || Server1Drop.SelectedValue.ToString().Contains("Choose Server"))
                return;

            if (Server1Drop.SelectedValue == Server2Drop.SelectedValue)
            {
                MessageBox.Show($"Seems silly to compare results from {Server1Drop.SelectedValue} to results from itself, no?");
                Server1Drop.SelectedIndex = 0;
            }
        }

        private void Server2Drop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Server2Drop.SelectedIndex == -1 || Server2Drop.SelectedValue.ToString().Contains("Choose Server"))
                return;
            
            if (Server1Drop.SelectedValue == Server2Drop.SelectedValue)
            {
                MessageBox.Show($"Seems silly to compare results from {Server2Drop.SelectedValue} to results from itself, no?");
                Server2Drop.SelectedIndex = 0;
            }
        }

        private void ItemList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ItemList.SelectedIndex == -1)
                return;

            var selectedItemInfo = JsonConvert.DeserializeObject<ItemListInfoBase>(ItemList.SelectedItem.ToString());
            if (selectedItemInfo.Stack == true)
            {
                StackCheckBox.Checked = true;
            }
            else
            {
                StackCheckBox.Checked = false;
            }
        }

        private void StackCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ItemList.SelectedIndex == -1)
                return;

            var itemInfo = JsonConvert.DeserializeObject<ItemListInfoBase>(ItemList.SelectedItem.ToString());
            
            if (StackCheckBox.Checked && itemInfo.Stack == false)
            {
                var currIndex = ItemList.SelectedIndex;
                ItemList.Items.RemoveAt(currIndex);
                itemInfo.Stack = true;
                ItemList.Items.Insert(currIndex, JsonConvert.SerializeObject(itemInfo));
            }

            if (!StackCheckBox.Checked && itemInfo.Stack == true)
            {
                var currIndex = ItemList.SelectedIndex;
                ItemList.Items.RemoveAt(currIndex);
                itemInfo.Stack = false;
                ItemList.Items.Insert(currIndex, JsonConvert.SerializeObject(itemInfo));
            }
        }

        private void AddToListButton_Click(object sender, EventArgs e)
        {
            var inputString = TextBox.Text;

            if (Items.Info.ContainsKey(inputString))
            {
                switch (ModeDrop.SelectedValue)
                {
                    case Modes.RareItemAlert:
                        ItemList.Items.Add(JsonConvert.SerializeObject(
                            new
                            {
                                Item = TextBox.Text.Replace(" ", "-").Replace("'", ""),
                                Id = Items.Info[TextBox.Text],
                                Stack = StackCheckBox.Checked,
                            }
                        ));
                        break;

                    case Modes.UndercutAlert:
                        var validPrice = int.TryParse(ListPrice.Text, out var listPrice);
                        if (!validPrice)
                        {
                            MessageBox.Show($"Enter a valid number for your list price. {ListPrice.Text} is a nogo!");
                            return;
                        }

                        ItemList.Items.Add(JsonConvert.SerializeObject(
                            new
                            {
                                Item = TextBox.Text.Replace(" ", "-").Replace("'", ""),
                                Id = Items.Info[TextBox.Text],
                                Stack = StackCheckBox.Checked,
                                MyPrice = listPrice,
                            }
                        ));
                        break;

                    case Modes.XServerArbitrage:
                        ItemList.Items.Add(JsonConvert.SerializeObject(
                            new
                            {
                                Item = TextBox.Text.Replace(" ", "-").Replace("'", ""),
                                Id = Items.Info[TextBox.Text],
                                Stack = StackCheckBox.Checked,
                            }
                        ));
                        break;
                }

                TextBox.Text = "";
                ListPrice.Text = "";
                StackCheckBox.Checked = false;
            }
            else
            {
                MessageBox.Show("THIS ISNT A REAL ITEM. NOT ALLOWED!");
                TextBox.Text = "";
                ListPrice.Text = "";
            }
        }

        private void RemFromListButton_Click(object sender, EventArgs e)
        {
            if (ItemList.SelectedIndex == -1)
            {
                MessageBox.Show("Select an item to remove before clicking remove button!");
                return;
            }
            ItemList.Items.RemoveAt(ItemList.SelectedIndex);
        }
                
        private async void GoButton_Click(object sender, EventArgs e)
        {
            if (ItemList.Items.Count == 0) return;
            if (!AutoRefreshMode)
            {
                AutoRefreshMode = true;
                GoButton.Text = "Stop Scraping";
                
                while (AutoRefreshMode == true)
                {
                    switch (ModeDrop.SelectedValue)
                    {
                        case Modes.RareItemAlert:
                            await _rareItemAlert.Action(ItemList, Result1Grid, ModeDisplay, Server1Drop.SelectedValue.ToString());
                            break;
                        case Modes.UndercutAlert:
                            await _undercutAlert.Action(ItemList, Result1Grid, ModeDisplay, Server1Drop.SelectedValue.ToString());
                            break;

                        case Modes.XServerArbitrage:
                            await _xServerArbitrage.Action(ItemList, Result1Grid, ModeDisplay, Server1Drop.SelectedValue.ToString(), Server2Drop.SelectedValue.ToString());
                            break;
                    }
                    WaitWithoutLockingGui(300);
                }
            }
            else
            {
                AutoRefreshMode = false;
                GoButton.Text = "Check FFXI-AH";
            }
            ModeDisplay.Text = "";
        }

        public void WaitWithoutLockingGui(int seconds)
        {
            var timer1 = new System.Windows.Forms.Timer();
            if (seconds <= 0) return;

            timer1.Interval = seconds * 1000;
            timer1.Enabled = true;
            timer1.Start();

            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };

            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }
    }
}