namespace JigApp
{
    partial class FormNwConfig
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
            this.textBox_IpAddr = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.button_SetConfig = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_SSID = new System.Windows.Forms.TextBox();
            this.textBox_Password = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton_Server = new System.Windows.Forms.RadioButton();
            this.radioButton_Client = new System.Windows.Forms.RadioButton();
            this.textBox_ServerIpAddr = new System.Windows.Forms.TextBox();
            this.label_ServerIpAddr = new System.Windows.Forms.Label();
            this.groupBox_WiFi = new System.Windows.Forms.GroupBox();
            this.groupBox_EMail = new System.Windows.Forms.GroupBox();
            this.numericUpDown_MailIntervalHour = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox_ToEMailAddress = new System.Windows.Forms.TextBox();
            this.textBox_GMailAppPassword = new System.Windows.Forms.TextBox();
            this.textBox_GMailAddress = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox_TcpSocketCom = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.radioButton_BLE = new System.Windows.Forms.RadioButton();
            this.radioButton_Wifi = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox_WiFi.SuspendLayout();
            this.groupBox_EMail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_MailIntervalHour)).BeginInit();
            this.groupBox_TcpSocketCom.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_IpAddr
            // 
            this.textBox_IpAddr.Location = new System.Drawing.Point(221, 34);
            this.textBox_IpAddr.Name = "textBox_IpAddr";
            this.textBox_IpAddr.Size = new System.Drawing.Size(200, 28);
            this.textBox_IpAddr.TabIndex = 43;
            this.textBox_IpAddr.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 35);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(197, 21);
            this.label8.TabIndex = 45;
            this.label8.Text = "PicoW IP Address:";
            // 
            // button_SetConfig
            // 
            this.button_SetConfig.Location = new System.Drawing.Point(339, 687);
            this.button_SetConfig.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button_SetConfig.Name = "button_SetConfig";
            this.button_SetConfig.Size = new System.Drawing.Size(174, 50);
            this.button_SetConfig.TabIndex = 47;
            this.button_SetConfig.TabStop = false;
            this.button_SetConfig.Text = "setting change";
            this.button_SetConfig.UseVisualStyleBackColor = true;
            this.button_SetConfig.Click += new System.EventHandler(this.button_SetConfig_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(153, 21);
            this.label2.TabIndex = 49;
            this.label2.Text = "SSID(2.4GHz):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 21);
            this.label3.TabIndex = 50;
            this.label3.Text = "Password:";
            // 
            // textBox_SSID
            // 
            this.textBox_SSID.Location = new System.Drawing.Point(178, 29);
            this.textBox_SSID.Name = "textBox_SSID";
            this.textBox_SSID.Size = new System.Drawing.Size(615, 28);
            this.textBox_SSID.TabIndex = 51;
            this.textBox_SSID.TabStop = false;
            // 
            // textBox_Password
            // 
            this.textBox_Password.Location = new System.Drawing.Point(178, 68);
            this.textBox_Password.Name = "textBox_Password";
            this.textBox_Password.Size = new System.Drawing.Size(615, 28);
            this.textBox_Password.TabIndex = 52;
            this.textBox_Password.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_SSID);
            this.groupBox1.Controls.Add(this.textBox_Password);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(11, 77);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(805, 110);
            this.groupBox1.TabIndex = 54;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "WPA2";
            // 
            // radioButton_Server
            // 
            this.radioButton_Server.AutoSize = true;
            this.radioButton_Server.Checked = true;
            this.radioButton_Server.Location = new System.Drawing.Point(22, 66);
            this.radioButton_Server.Name = "radioButton_Server";
            this.radioButton_Server.Size = new System.Drawing.Size(101, 25);
            this.radioButton_Server.TabIndex = 56;
            this.radioButton_Server.TabStop = true;
            this.radioButton_Server.Text = "Server";
            this.radioButton_Server.UseVisualStyleBackColor = true;
            this.radioButton_Server.CheckedChanged += new System.EventHandler(this.radioButton_Server_CheckedChanged);
            // 
            // radioButton_Client
            // 
            this.radioButton_Client.AutoSize = true;
            this.radioButton_Client.Location = new System.Drawing.Point(22, 102);
            this.radioButton_Client.Name = "radioButton_Client";
            this.radioButton_Client.Size = new System.Drawing.Size(101, 25);
            this.radioButton_Client.TabIndex = 57;
            this.radioButton_Client.Text = "Client";
            this.radioButton_Client.UseVisualStyleBackColor = true;
            // 
            // textBox_ServerIpAddr
            // 
            this.textBox_ServerIpAddr.Location = new System.Drawing.Point(376, 99);
            this.textBox_ServerIpAddr.Name = "textBox_ServerIpAddr";
            this.textBox_ServerIpAddr.Size = new System.Drawing.Size(200, 28);
            this.textBox_ServerIpAddr.TabIndex = 58;
            this.textBox_ServerIpAddr.TabStop = false;
            // 
            // label_ServerIpAddr
            // 
            this.label_ServerIpAddr.AutoSize = true;
            this.label_ServerIpAddr.Location = new System.Drawing.Point(162, 102);
            this.label_ServerIpAddr.Name = "label_ServerIpAddr";
            this.label_ServerIpAddr.Size = new System.Drawing.Size(208, 21);
            this.label_ServerIpAddr.TabIndex = 59;
            this.label_ServerIpAddr.Text = "Server IP Address:";
            // 
            // groupBox_WiFi
            // 
            this.groupBox_WiFi.Controls.Add(this.groupBox1);
            this.groupBox_WiFi.Controls.Add(this.textBox_IpAddr);
            this.groupBox_WiFi.Controls.Add(this.groupBox_EMail);
            this.groupBox_WiFi.Controls.Add(this.label8);
            this.groupBox_WiFi.Controls.Add(this.groupBox_TcpSocketCom);
            this.groupBox_WiFi.Location = new System.Drawing.Point(12, 68);
            this.groupBox_WiFi.Name = "groupBox_WiFi";
            this.groupBox_WiFi.Size = new System.Drawing.Size(826, 603);
            this.groupBox_WiFi.TabIndex = 61;
            this.groupBox_WiFi.TabStop = false;
            this.groupBox_WiFi.Text = "Wi-Fi";
            // 
            // groupBox_EMail
            // 
            this.groupBox_EMail.Controls.Add(this.numericUpDown_MailIntervalHour);
            this.groupBox_EMail.Controls.Add(this.label10);
            this.groupBox_EMail.Controls.Add(this.textBox_ToEMailAddress);
            this.groupBox_EMail.Controls.Add(this.textBox_GMailAppPassword);
            this.groupBox_EMail.Controls.Add(this.textBox_GMailAddress);
            this.groupBox_EMail.Controls.Add(this.label7);
            this.groupBox_EMail.Controls.Add(this.label5);
            this.groupBox_EMail.Controls.Add(this.label1);
            this.groupBox_EMail.Location = new System.Drawing.Point(12, 383);
            this.groupBox_EMail.Name = "groupBox_EMail";
            this.groupBox_EMail.Size = new System.Drawing.Size(804, 201);
            this.groupBox_EMail.TabIndex = 63;
            this.groupBox_EMail.TabStop = false;
            this.groupBox_EMail.Text = "E-Mail";
            // 
            // numericUpDown_MailIntervalHour
            // 
            this.numericUpDown_MailIntervalHour.Location = new System.Drawing.Point(336, 152);
            this.numericUpDown_MailIntervalHour.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.numericUpDown_MailIntervalHour.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_MailIntervalHour.Name = "numericUpDown_MailIntervalHour";
            this.numericUpDown_MailIntervalHour.Size = new System.Drawing.Size(150, 28);
            this.numericUpDown_MailIntervalHour.TabIndex = 64;
            this.numericUpDown_MailIntervalHour.TabStop = false;
            this.numericUpDown_MailIntervalHour.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 154);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(318, 21);
            this.label10.TabIndex = 67;
            this.label10.Text = "Sending Mail Interval(hour):";
            // 
            // textBox_ToEMailAddress
            // 
            this.textBox_ToEMailAddress.Location = new System.Drawing.Point(244, 107);
            this.textBox_ToEMailAddress.Name = "textBox_ToEMailAddress";
            this.textBox_ToEMailAddress.Size = new System.Drawing.Size(548, 28);
            this.textBox_ToEMailAddress.TabIndex = 66;
            this.textBox_ToEMailAddress.TabStop = false;
            // 
            // textBox_GMailAppPassword
            // 
            this.textBox_GMailAppPassword.Location = new System.Drawing.Point(244, 70);
            this.textBox_GMailAppPassword.Name = "textBox_GMailAppPassword";
            this.textBox_GMailAppPassword.Size = new System.Drawing.Size(548, 28);
            this.textBox_GMailAppPassword.TabIndex = 65;
            this.textBox_GMailAppPassword.TabStop = false;
            // 
            // textBox_GMailAddress
            // 
            this.textBox_GMailAddress.Location = new System.Drawing.Point(244, 35);
            this.textBox_GMailAddress.Name = "textBox_GMailAddress";
            this.textBox_GMailAddress.Size = new System.Drawing.Size(548, 28);
            this.textBox_GMailAddress.TabIndex = 64;
            this.textBox_GMailAddress.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 110);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(208, 21);
            this.label7.TabIndex = 66;
            this.label7.Text = "To E-Mail Address:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(219, 21);
            this.label5.TabIndex = 65;
            this.label5.Text = "Gmail App Password:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(219, 21);
            this.label1.TabIndex = 64;
            this.label1.Text = "Your Gmail Address:";
            // 
            // groupBox_TcpSocketCom
            // 
            this.groupBox_TcpSocketCom.Controls.Add(this.label6);
            this.groupBox_TcpSocketCom.Controls.Add(this.radioButton_Server);
            this.groupBox_TcpSocketCom.Controls.Add(this.textBox_ServerIpAddr);
            this.groupBox_TcpSocketCom.Controls.Add(this.radioButton_Client);
            this.groupBox_TcpSocketCom.Controls.Add(this.label_ServerIpAddr);
            this.groupBox_TcpSocketCom.Location = new System.Drawing.Point(12, 215);
            this.groupBox_TcpSocketCom.Name = "groupBox_TcpSocketCom";
            this.groupBox_TcpSocketCom.Size = new System.Drawing.Size(804, 140);
            this.groupBox_TcpSocketCom.TabIndex = 62;
            this.groupBox_TcpSocketCom.TabStop = false;
            this.groupBox_TcpSocketCom.Text = "TCP Socket Communication";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 21);
            this.label6.TabIndex = 0;
            this.label6.Text = "PicoW is:";
            // 
            // radioButton_BLE
            // 
            this.radioButton_BLE.AutoSize = true;
            this.radioButton_BLE.Checked = true;
            this.radioButton_BLE.Location = new System.Drawing.Point(12, 19);
            this.radioButton_BLE.Name = "radioButton_BLE";
            this.radioButton_BLE.Size = new System.Drawing.Size(68, 25);
            this.radioButton_BLE.TabIndex = 64;
            this.radioButton_BLE.TabStop = true;
            this.radioButton_BLE.Text = "BLE";
            this.radioButton_BLE.UseVisualStyleBackColor = true;
            // 
            // radioButton_Wifi
            // 
            this.radioButton_Wifi.AutoSize = true;
            this.radioButton_Wifi.Location = new System.Drawing.Point(101, 19);
            this.radioButton_Wifi.Name = "radioButton_Wifi";
            this.radioButton_Wifi.Size = new System.Drawing.Size(90, 25);
            this.radioButton_Wifi.TabIndex = 65;
            this.radioButton_Wifi.Text = "Wi-Fi";
            this.radioButton_Wifi.UseVisualStyleBackColor = true;
            // 
            // FormNwConfig
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(851, 759);
            this.Controls.Add(this.radioButton_Wifi);
            this.Controls.Add(this.radioButton_BLE);
            this.Controls.Add(this.groupBox_WiFi);
            this.Controls.Add(this.button_SetConfig);
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormNwConfig";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NwConfig";
            this.Load += new System.EventHandler(this.FormNwConfig_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox_WiFi.ResumeLayout(false);
            this.groupBox_WiFi.PerformLayout();
            this.groupBox_EMail.ResumeLayout(false);
            this.groupBox_EMail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_MailIntervalHour)).EndInit();
            this.groupBox_TcpSocketCom.ResumeLayout(false);
            this.groupBox_TcpSocketCom.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox_IpAddr;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button_SetConfig;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_SSID;
        private System.Windows.Forms.TextBox textBox_Password;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton_Server;
        private System.Windows.Forms.RadioButton radioButton_Client;
        private System.Windows.Forms.TextBox textBox_ServerIpAddr;
        private System.Windows.Forms.Label label_ServerIpAddr;
        private System.Windows.Forms.GroupBox groupBox_WiFi;
        private System.Windows.Forms.GroupBox groupBox_TcpSocketCom;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox_EMail;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox_ToEMailAddress;
        private System.Windows.Forms.TextBox textBox_GMailAppPassword;
        private System.Windows.Forms.TextBox textBox_GMailAddress;
        private System.Windows.Forms.NumericUpDown numericUpDown_MailIntervalHour;
        private System.Windows.Forms.RadioButton radioButton_BLE;
        private System.Windows.Forms.RadioButton radioButton_Wifi;
    }
}