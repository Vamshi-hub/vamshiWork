namespace BluecatsReceiver
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
            this.Start = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.txtBeaconData = new System.Windows.Forms.TextBox();
            this.txtEvent = new System.Windows.Forms.TextBox();
            this.txtEntries = new System.Windows.Forms.TextBox();
            this.txtZones = new System.Windows.Forms.TextBox();
            this.txtLocations = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Entries = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.IPEndPoint = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Start
            // 
            this.Start.Location = new System.Drawing.Point(-2, 107);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(258, 98);
            this.Start.TabIndex = 0;
            this.Start.Text = "UDP Client";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(274, 107);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(219, 98);
            this.button1.TabIndex = 1;
            this.button1.Text = "MQTT";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1812, 129);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(213, 54);
            this.button2.TabIndex = 2;
            this.button2.Text = "SEND";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(1000, 128);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(747, 54);
            this.textBox1.TabIndex = 3;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(511, 107);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(471, 98);
            this.button3.TabIndex = 4;
            this.button3.Text = "Recive";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtBeaconData
            // 
            this.txtBeaconData.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.txtBeaconData.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBeaconData.Location = new System.Drawing.Point(-2, 264);
            this.txtBeaconData.Multiline = true;
            this.txtBeaconData.Name = "txtBeaconData";
            this.txtBeaconData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtBeaconData.Size = new System.Drawing.Size(1101, 596);
            this.txtBeaconData.TabIndex = 5;
            this.txtBeaconData.WordWrap = false;
            this.txtBeaconData.TextChanged += new System.EventHandler(this.txtMessage_TextChanged);
            // 
            // txtEvent
            // 
            this.txtEvent.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.txtEvent.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEvent.Location = new System.Drawing.Point(1150, 264);
            this.txtEvent.Multiline = true;
            this.txtEvent.Name = "txtEvent";
            this.txtEvent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEvent.Size = new System.Drawing.Size(1246, 590);
            this.txtEvent.TabIndex = 6;
            this.txtEvent.WordWrap = false;
            // 
            // txtEntries
            // 
            this.txtEntries.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.txtEntries.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEntries.Location = new System.Drawing.Point(1729, 947);
            this.txtEntries.Multiline = true;
            this.txtEntries.Name = "txtEntries";
            this.txtEntries.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEntries.Size = new System.Drawing.Size(650, 575);
            this.txtEntries.TabIndex = 7;
            this.txtEntries.WordWrap = false;
            // 
            // txtZones
            // 
            this.txtZones.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.txtZones.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtZones.Location = new System.Drawing.Point(3, 936);
            this.txtZones.Multiline = true;
            this.txtZones.Name = "txtZones";
            this.txtZones.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtZones.Size = new System.Drawing.Size(820, 595);
            this.txtZones.TabIndex = 8;
            this.txtZones.WordWrap = false;
            // 
            // txtLocations
            // 
            this.txtLocations.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.txtLocations.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLocations.Location = new System.Drawing.Point(875, 936);
            this.txtLocations.Multiline = true;
            this.txtLocations.Name = "txtLocations";
            this.txtLocations.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLocations.Size = new System.Drawing.Size(820, 586);
            this.txtLocations.TabIndex = 9;
            this.txtLocations.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 226);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 25);
            this.label1.TabIndex = 10;
            this.label1.Text = "Beacon Data";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1669, 226);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 25);
            this.label2.TabIndex = 11;
            this.label2.Text = "Events";
            // 
            // Entries
            // 
            this.Entries.AutoSize = true;
            this.Entries.Location = new System.Drawing.Point(1757, 893);
            this.Entries.Name = "Entries";
            this.Entries.Size = new System.Drawing.Size(79, 25);
            this.Entries.TabIndex = 12;
            this.Entries.Text = "Entries";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 883);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 25);
            this.label3.TabIndex = 13;
            this.label3.Text = "Zones";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(896, 893);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 25);
            this.label4.TabIndex = 14;
            this.label4.Text = "Locations";
            // 
            // IPEndPoint
            // 
            this.IPEndPoint.Location = new System.Drawing.Point(292, 44);
            this.IPEndPoint.Name = "IPEndPoint";
            this.IPEndPoint.Size = new System.Drawing.Size(709, 31);
            this.IPEndPoint.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(105, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 25);
            this.label5.TabIndex = 16;
            this.label5.Text = "End Point";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2464, 1527);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.IPEndPoint);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Entries);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtLocations);
            this.Controls.Add(this.txtZones);
            this.Controls.Add(this.txtEntries);
            this.Controls.Add(this.txtEvent);
            this.Controls.Add(this.txtBeaconData);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Start);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox txtBeaconData;
        private System.Windows.Forms.TextBox txtEvent;
        private System.Windows.Forms.TextBox txtEntries;
        private System.Windows.Forms.TextBox txtZones;
        private System.Windows.Forms.TextBox txtLocations;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label Entries;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox IPEndPoint;
        private System.Windows.Forms.Label label5;
    }
}

