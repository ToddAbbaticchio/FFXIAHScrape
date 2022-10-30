namespace FFXIAHScrape
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Server1Drop = new System.Windows.Forms.ComboBox();
            this.Server2Drop = new System.Windows.Forms.ComboBox();
            this.ItemList = new System.Windows.Forms.ListBox();
            this.TextBox = new System.Windows.Forms.TextBox();
            this.AddToListButton = new System.Windows.Forms.Button();
            this.StackCheckBox = new System.Windows.Forms.CheckBox();
            this.GoButton = new System.Windows.Forms.Button();
            this.Result1Grid = new System.Windows.Forms.DataGridView();
            this.RemFromListButton = new System.Windows.Forms.Button();
            this.constantsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.constantsBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.Result1Grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.constantsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.constantsBindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // Server1Drop
            // 
            this.Server1Drop.AllowDrop = true;
            this.Server1Drop.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.Server1Drop.FormattingEnabled = true;
            this.Server1Drop.Location = new System.Drawing.Point(119, 30);
            this.Server1Drop.Name = "Server1Drop";
            this.Server1Drop.Size = new System.Drawing.Size(162, 21);
            this.Server1Drop.TabIndex = 0;
            this.Server1Drop.SelectedIndexChanged += new System.EventHandler(this.Server1Drop_SelectedIndexChanged);
            // 
            // Server2Drop
            // 
            this.Server2Drop.AllowDrop = true;
            this.Server2Drop.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.Server2Drop.FormattingEnabled = true;
            this.Server2Drop.Location = new System.Drawing.Point(378, 30);
            this.Server2Drop.Name = "Server2Drop";
            this.Server2Drop.Size = new System.Drawing.Size(162, 21);
            this.Server2Drop.TabIndex = 1;
            this.Server2Drop.SelectedIndexChanged += new System.EventHandler(this.Server2Drop_SelectedIndexChanged);
            // 
            // ItemList
            // 
            this.ItemList.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ItemList.FormattingEnabled = true;
            this.ItemList.Location = new System.Drawing.Point(378, 76);
            this.ItemList.Name = "ItemList";
            this.ItemList.Size = new System.Drawing.Size(344, 108);
            this.ItemList.TabIndex = 2;
            this.ItemList.SelectedIndexChanged += new System.EventHandler(this.ItemList_SelectedIndexChanged);
            // 
            // TextBox
            // 
            this.TextBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.TextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.TextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.TextBox.Location = new System.Drawing.Point(119, 76);
            this.TextBox.Name = "TextBox";
            this.TextBox.Size = new System.Drawing.Size(162, 20);
            this.TextBox.TabIndex = 3;
            // 
            // AddToListButton
            // 
            this.AddToListButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.AddToListButton.Location = new System.Drawing.Point(317, 76);
            this.AddToListButton.Name = "AddToListButton";
            this.AddToListButton.Size = new System.Drawing.Size(27, 21);
            this.AddToListButton.TabIndex = 4;
            this.AddToListButton.Text = ">>";
            this.AddToListButton.UseVisualStyleBackColor = true;
            this.AddToListButton.Click += new System.EventHandler(this.AddToListButton_Click);
            // 
            // StackCheckBox
            // 
            this.StackCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.StackCheckBox.AutoSize = true;
            this.StackCheckBox.Location = new System.Drawing.Point(172, 107);
            this.StackCheckBox.Name = "StackCheckBox";
            this.StackCheckBox.Size = new System.Drawing.Size(60, 17);
            this.StackCheckBox.TabIndex = 5;
            this.StackCheckBox.Text = "Stack?";
            this.StackCheckBox.UseVisualStyleBackColor = true;
            this.StackCheckBox.CheckedChanged += new System.EventHandler(this.StackCheckBox_CheckedChanged);
            // 
            // GoButton
            // 
            this.GoButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.GoButton.Location = new System.Drawing.Point(139, 139);
            this.GoButton.Name = "GoButton";
            this.GoButton.Size = new System.Drawing.Size(120, 23);
            this.GoButton.TabIndex = 6;
            this.GoButton.Text = "Compare Prices";
            this.GoButton.UseVisualStyleBackColor = true;
            this.GoButton.Click += new System.EventHandler(this.GoButton_Click);
            // 
            // Result1Grid
            // 
            this.Result1Grid.AllowUserToAddRows = false;
            this.Result1Grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Result1Grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.Result1Grid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllHeaders;
            this.Result1Grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Result1Grid.Location = new System.Drawing.Point(12, 195);
            this.Result1Grid.Name = "Result1Grid";
            this.Result1Grid.Size = new System.Drawing.Size(710, 254);
            this.Result1Grid.TabIndex = 7;
            // 
            // RemFromListButton
            // 
            this.RemFromListButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.RemFromListButton.Location = new System.Drawing.Point(317, 103);
            this.RemFromListButton.Name = "RemFromListButton";
            this.RemFromListButton.Size = new System.Drawing.Size(27, 21);
            this.RemFromListButton.TabIndex = 8;
            this.RemFromListButton.Text = "<<";
            this.RemFromListButton.UseVisualStyleBackColor = true;
            this.RemFromListButton.Click += new System.EventHandler(this.RemFromListButton_Click);
            // 
            // constantsBindingSource
            // 
            this.constantsBindingSource.DataSource = typeof(FFXIAHScrape.Constants);
            // 
            // constantsBindingSource1
            // 
            this.constantsBindingSource1.DataSource = typeof(FFXIAHScrape.Constants);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(734, 461);
            this.Controls.Add(this.RemFromListButton);
            this.Controls.Add(this.Result1Grid);
            this.Controls.Add(this.GoButton);
            this.Controls.Add(this.StackCheckBox);
            this.Controls.Add(this.AddToListButton);
            this.Controls.Add(this.TextBox);
            this.Controls.Add(this.ItemList);
            this.Controls.Add(this.Server2Drop);
            this.Controls.Add(this.Server1Drop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(750, 500);
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "FFXI-AH Scraper";
            ((System.ComponentModel.ISupportInitialize)(this.Result1Grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.constantsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.constantsBindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox Server1Drop;
        private System.Windows.Forms.BindingSource constantsBindingSource;
        private System.Windows.Forms.BindingSource constantsBindingSource1;
        private System.Windows.Forms.ComboBox Server2Drop;
        private System.Windows.Forms.ListBox ItemList;
        private System.Windows.Forms.TextBox TextBox;
        private System.Windows.Forms.Button AddToListButton;
        private System.Windows.Forms.CheckBox StackCheckBox;
        private System.Windows.Forms.Button GoButton;
        private System.Windows.Forms.DataGridView Result1Grid;
        private System.Windows.Forms.Button RemFromListButton;
    }
}

