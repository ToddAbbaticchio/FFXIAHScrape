﻿using System;
using System.Drawing;

namespace FFXIAHScrape.Extensions
{
    class PHTextBox : System.Windows.Forms.TextBox
    {
        Color DefaultColor;
        public string PlaceHolderText { get; set; }
        
        public PHTextBox(string placeholdertext)
        {
            DefaultColor = this.ForeColor;
        
            this.GotFocus += (object sender, EventArgs e) =>
            {
                this.Text = String.Empty;
                this.ForeColor = DefaultColor;
            };

            this.LostFocus += (Object sender, EventArgs e) => {
                if (String.IsNullOrEmpty(this.Text) || this.Text == PlaceHolderText)
                {
                    this.ForeColor = System.Drawing.Color.Gray;
                    this.Text = PlaceHolderText;
                }
                else
                {
                    this.ForeColor = DefaultColor;
                }
            };

            if (!string.IsNullOrEmpty(placeholdertext))
            {
                // change style   
                this.ForeColor = System.Drawing.Color.Gray;
                // Add text
                PlaceHolderText = placeholdertext;
                this.Text = placeholdertext;
            }
        }
    }
}