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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox_Gateway = new System.Windows.Forms.TextBox();
            this.label_Gateway = new System.Windows.Forms.Label();
            this.textBox_Subnet = new System.Windows.Forms.TextBox();
            this.label_Subnet = new System.Windows.Forms.Label();
            this.groupBox_TcpSocketCom = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.radioButton_BLE = new System.Windows.Forms.RadioButton();
            this.radioButton_Wifi = new System.Windows.Forms.RadioButton();
            this.groupBox_Mode = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox_WiFi.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox_TcpSocketCom.SuspendLayout();
            this.groupBox_Mode.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_IpAddr
            // 
            this.textBox_IpAddr.Location = new System.Drawing.Point(166, 34);
            this.textBox_IpAddr.Name = "textBox_IpAddr";
            this.textBox_IpAddr.Size = new System.Drawing.Size(200, 28);
            this.textBox_IpAddr.TabIndex = 43;
            this.textBox_IpAddr.TabStop = false;
            this.textBox_IpAddr.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_HalfWidth_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(29, 37);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(131, 21);
            this.label8.TabIndex = 45;
            this.label8.Text = "IP Address:";
            // 
            // button_SetConfig
            // 
            this.button_SetConfig.Location = new System.Drawing.Point(298, 693);
            this.button_SetConfig.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button_SetConfig.Name = "button_SetConfig";
            this.button_SetConfig.Size = new System.Drawing.Size(238, 50);
            this.button_SetConfig.TabIndex = 47;
            this.button_SetConfig.TabStop = false;
            this.button_SetConfig.Text = "Change Settings";
            this.button_SetConfig.UseVisualStyleBackColor = true;
            this.button_SetConfig.Click += new System.EventHandler(this.button_SetConfig_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(63, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 21);
            this.label2.TabIndex = 49;
            this.label2.Text = "SSID:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 21);
            this.label3.TabIndex = 50;
            this.label3.Text = "Password:";
            // 
            // textBox_SSID
            // 
            this.textBox_SSID.Location = new System.Drawing.Point(134, 34);
            this.textBox_SSID.Name = "textBox_SSID";
            this.textBox_SSID.Size = new System.Drawing.Size(590, 28);
            this.textBox_SSID.TabIndex = 51;
            this.textBox_SSID.TabStop = false;
            this.textBox_SSID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_HalfWidth_KeyPress);
            // 
            // textBox_Password
            // 
            this.textBox_Password.Location = new System.Drawing.Point(135, 75);
            this.textBox_Password.Name = "textBox_Password";
            this.textBox_Password.PasswordChar = '*';
            this.textBox_Password.Size = new System.Drawing.Size(590, 28);
            this.textBox_Password.TabIndex = 52;
            this.textBox_Password.TabStop = false;
            this.textBox_Password.UseSystemPasswordChar = true;
            this.textBox_Password.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_HalfWidth_KeyPress);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_SSID);
            this.groupBox1.Controls.Add(this.textBox_Password);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(22, 221);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(744, 126);
            this.groupBox1.TabIndex = 54;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Access Point (2.4GHz, WPA2)";
            // 
            // radioButton_Server
            // 
            this.radioButton_Server.AutoSize = true;
            this.radioButton_Server.Checked = true;
            this.radioButton_Server.Location = new System.Drawing.Point(22, 71);
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
            this.radioButton_Client.Location = new System.Drawing.Point(22, 107);
            this.radioButton_Client.Name = "radioButton_Client";
            this.radioButton_Client.Size = new System.Drawing.Size(101, 25);
            this.radioButton_Client.TabIndex = 57;
            this.radioButton_Client.Text = "Client";
            this.radioButton_Client.UseVisualStyleBackColor = true;
            // 
            // textBox_ServerIpAddr
            // 
            this.textBox_ServerIpAddr.Location = new System.Drawing.Point(376, 106);
            this.textBox_ServerIpAddr.Name = "textBox_ServerIpAddr";
            this.textBox_ServerIpAddr.Size = new System.Drawing.Size(200, 28);
            this.textBox_ServerIpAddr.TabIndex = 58;
            this.textBox_ServerIpAddr.TabStop = false;
            this.textBox_ServerIpAddr.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_HalfWidth_KeyPress);
            // 
            // label_ServerIpAddr
            // 
            this.label_ServerIpAddr.AutoSize = true;
            this.label_ServerIpAddr.Location = new System.Drawing.Point(162, 109);
            this.label_ServerIpAddr.Name = "label_ServerIpAddr";
            this.label_ServerIpAddr.Size = new System.Drawing.Size(208, 21);
            this.label_ServerIpAddr.TabIndex = 59;
            this.label_ServerIpAddr.Text = "Server IP Address:";
            // 
            // groupBox_WiFi
            // 
            this.groupBox_WiFi.Controls.Add(this.groupBox2);
            this.groupBox_WiFi.Controls.Add(this.groupBox1);
            this.groupBox_WiFi.Location = new System.Drawing.Point(18, 121);
            this.groupBox_WiFi.Name = "groupBox_WiFi";
            this.groupBox_WiFi.Size = new System.Drawing.Size(791, 372);
            this.groupBox_WiFi.TabIndex = 61;
            this.groupBox_WiFi.TabStop = false;
            this.groupBox_WiFi.Text = "Wi-Fi";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_Gateway);
            this.groupBox2.Controls.Add(this.label_Gateway);
            this.groupBox2.Controls.Add(this.textBox_Subnet);
            this.groupBox2.Controls.Add(this.label_Subnet);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.textBox_IpAddr);
            this.groupBox2.Location = new System.Drawing.Point(23, 31);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(743, 171);
            this.groupBox2.TabIndex = 68;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "IP Config";
            // 
            // textBox_Gateway
            // 
            this.textBox_Gateway.Location = new System.Drawing.Point(166, 122);
            this.textBox_Gateway.Name = "textBox_Gateway";
            this.textBox_Gateway.Size = new System.Drawing.Size(200, 28);
            this.textBox_Gateway.TabIndex = 73;
            this.textBox_Gateway.TabStop = false;
            this.textBox_Gateway.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_HalfWidth_KeyPress);
            // 
            // label_Gateway
            // 
            this.label_Gateway.AutoSize = true;
            this.label_Gateway.Location = new System.Drawing.Point(62, 124);
            this.label_Gateway.Name = "label_Gateway";
            this.label_Gateway.Size = new System.Drawing.Size(98, 21);
            this.label_Gateway.TabIndex = 72;
            this.label_Gateway.Text = "Gateway:";
            // 
            // textBox_Subnet
            // 
            this.textBox_Subnet.Location = new System.Drawing.Point(166, 79);
            this.textBox_Subnet.Name = "textBox_Subnet";
            this.textBox_Subnet.Size = new System.Drawing.Size(200, 28);
            this.textBox_Subnet.TabIndex = 71;
            this.textBox_Subnet.TabStop = false;
            this.textBox_Subnet.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_HalfWidth_KeyPress);
            // 
            // label_Subnet
            // 
            this.label_Subnet.AutoSize = true;
            this.label_Subnet.Location = new System.Drawing.Point(18, 82);
            this.label_Subnet.Name = "label_Subnet";
            this.label_Subnet.Size = new System.Drawing.Size(142, 21);
            this.label_Subnet.TabIndex = 70;
            this.label_Subnet.Text = "Subnet Mask:";
            // 
            // groupBox_TcpSocketCom
            // 
            this.groupBox_TcpSocketCom.Controls.Add(this.label6);
            this.groupBox_TcpSocketCom.Controls.Add(this.radioButton_Server);
            this.groupBox_TcpSocketCom.Controls.Add(this.textBox_ServerIpAddr);
            this.groupBox_TcpSocketCom.Controls.Add(this.radioButton_Client);
            this.groupBox_TcpSocketCom.Controls.Add(this.label_ServerIpAddr);
            this.groupBox_TcpSocketCom.Location = new System.Drawing.Point(18, 512);
            this.groupBox_TcpSocketCom.Name = "groupBox_TcpSocketCom";
            this.groupBox_TcpSocketCom.Size = new System.Drawing.Size(791, 155);
            this.groupBox_TcpSocketCom.TabIndex = 62;
            this.groupBox_TcpSocketCom.TabStop = false;
            this.groupBox_TcpSocketCom.Text = "TCP Socket Communication";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 21);
            this.label6.TabIndex = 0;
            this.label6.Text = "Role:";
            // 
            // radioButton_BLE
            // 
            this.radioButton_BLE.AutoSize = true;
            this.radioButton_BLE.Location = new System.Drawing.Point(166, 41);
            this.radioButton_BLE.Name = "radioButton_BLE";
            this.radioButton_BLE.Size = new System.Drawing.Size(68, 25);
            this.radioButton_BLE.TabIndex = 64;
            this.radioButton_BLE.Text = "BLE";
            this.radioButton_BLE.UseVisualStyleBackColor = true;
            this.radioButton_BLE.CheckedChanged += new System.EventHandler(this.radioButton_BLE_CheckedChanged);
            // 
            // radioButton_Wifi
            // 
            this.radioButton_Wifi.AutoSize = true;
            this.radioButton_Wifi.Checked = true;
            this.radioButton_Wifi.Location = new System.Drawing.Point(22, 41);
            this.radioButton_Wifi.Name = "radioButton_Wifi";
            this.radioButton_Wifi.Size = new System.Drawing.Size(90, 25);
            this.radioButton_Wifi.TabIndex = 65;
            this.radioButton_Wifi.TabStop = true;
            this.radioButton_Wifi.Text = "Wi-Fi";
            this.radioButton_Wifi.UseVisualStyleBackColor = true;
            // 
            // groupBox_Mode
            // 
            this.groupBox_Mode.Controls.Add(this.radioButton_Wifi);
            this.groupBox_Mode.Controls.Add(this.radioButton_BLE);
            this.groupBox_Mode.Location = new System.Drawing.Point(18, 15);
            this.groupBox_Mode.Name = "groupBox_Mode";
            this.groupBox_Mode.Size = new System.Drawing.Size(791, 88);
            this.groupBox_Mode.TabIndex = 66;
            this.groupBox_Mode.TabStop = false;
            this.groupBox_Mode.Text = "Mode";
            // 
            // FormNwConfig
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(834, 766);
            this.Controls.Add(this.groupBox_Mode);
            this.Controls.Add(this.groupBox_TcpSocketCom);
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
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox_TcpSocketCom.ResumeLayout(false);
            this.groupBox_TcpSocketCom.PerformLayout();
            this.groupBox_Mode.ResumeLayout(false);
            this.groupBox_Mode.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.RadioButton radioButton_BLE;
        private System.Windows.Forms.RadioButton radioButton_Wifi;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label_Subnet;
        private System.Windows.Forms.TextBox textBox_Gateway;
        private System.Windows.Forms.Label label_Gateway;
        private System.Windows.Forms.TextBox textBox_Subnet;
        private System.Windows.Forms.GroupBox groupBox_Mode;
    }
}