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
            this.textBox_CountryCode = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox_IpAddr = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.button_SetConfig = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_SSID = new System.Windows.Forms.TextBox();
            this.textBox_Password = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.radioButton_Server = new System.Windows.Forms.RadioButton();
            this.radioButton_Client = new System.Windows.Forms.RadioButton();
            this.textBox_ServerIpAddr = new System.Windows.Forms.TextBox();
            this.label_ServerIpAddr = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_CountryCode
            // 
            this.textBox_CountryCode.Location = new System.Drawing.Point(242, 135);
            this.textBox_CountryCode.Name = "textBox_CountryCode";
            this.textBox_CountryCode.Size = new System.Drawing.Size(200, 28);
            this.textBox_CountryCode.TabIndex = 44;
            this.textBox_CountryCode.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(30, 138);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(153, 21);
            this.label9.TabIndex = 46;
            this.label9.Text = "Country Code:";
            // 
            // textBox_IpAddr
            // 
            this.textBox_IpAddr.Location = new System.Drawing.Point(243, 182);
            this.textBox_IpAddr.Name = "textBox_IpAddr";
            this.textBox_IpAddr.Size = new System.Drawing.Size(200, 28);
            this.textBox_IpAddr.TabIndex = 43;
            this.textBox_IpAddr.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(30, 185);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(208, 21);
            this.label8.TabIndex = 45;
            this.label8.Text = "Pico W IP Address:";
            // 
            // button_SetConfig
            // 
            this.button_SetConfig.Location = new System.Drawing.Point(343, 454);
            this.button_SetConfig.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button_SetConfig.Name = "button_SetConfig";
            this.button_SetConfig.Size = new System.Drawing.Size(174, 50);
            this.button_SetConfig.TabIndex = 47;
            this.button_SetConfig.TabStop = false;
            this.button_SetConfig.Text = "setting change";
            this.button_SetConfig.UseVisualStyleBackColor = true;
            this.button_SetConfig.Click += new System.EventHandler(this.button_SetConfig_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(490, 21);
            this.label1.TabIndex = 48;
            this.label1.Text = "Network Settings of Raspberry Pi Pico W:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(98, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 21);
            this.label2.TabIndex = 49;
            this.label2.Text = "SSID:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(54, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 21);
            this.label3.TabIndex = 50;
            this.label3.Text = "Password:";
            // 
            // textBox_SSID
            // 
            this.textBox_SSID.Location = new System.Drawing.Point(170, 39);
            this.textBox_SSID.Name = "textBox_SSID";
            this.textBox_SSID.Size = new System.Drawing.Size(640, 28);
            this.textBox_SSID.TabIndex = 51;
            this.textBox_SSID.TabStop = false;
            // 
            // textBox_Password
            // 
            this.textBox_Password.Location = new System.Drawing.Point(169, 82);
            this.textBox_Password.Name = "textBox_Password";
            this.textBox_Password.Size = new System.Drawing.Size(640, 28);
            this.textBox_Password.TabIndex = 52;
            this.textBox_Password.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_SSID);
            this.groupBox1.Controls.Add(this.textBox_Password);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(19, 291);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(823, 135);
            this.groupBox1.TabIndex = 54;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "WPA2(AES)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(448, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(230, 21);
            this.label4.TabIndex = 55;
            this.label4.Text = "e.g:Japan=JP  USA=US";
            // 
            // radioButton_Server
            // 
            this.radioButton_Server.AutoSize = true;
            this.radioButton_Server.Checked = true;
            this.radioButton_Server.Location = new System.Drawing.Point(34, 81);
            this.radioButton_Server.Name = "radioButton_Server";
            this.radioButton_Server.Size = new System.Drawing.Size(101, 25);
            this.radioButton_Server.TabIndex = 56;
            this.radioButton_Server.Text = "Server";
            this.radioButton_Server.UseVisualStyleBackColor = true;
            this.radioButton_Server.CheckedChanged += new System.EventHandler(this.radioButton_Server_CheckedChanged);
            // 
            // radioButton_Client
            // 
            this.radioButton_Client.AutoSize = true;
            this.radioButton_Client.Location = new System.Drawing.Point(159, 81);
            this.radioButton_Client.Name = "radioButton_Client";
            this.radioButton_Client.Size = new System.Drawing.Size(101, 25);
            this.radioButton_Client.TabIndex = 57;
            this.radioButton_Client.Text = "Client";
            this.radioButton_Client.UseVisualStyleBackColor = true;
            // 
            // textBox_ServerIpAddr
            // 
            this.textBox_ServerIpAddr.Location = new System.Drawing.Point(243, 229);
            this.textBox_ServerIpAddr.Name = "textBox_ServerIpAddr";
            this.textBox_ServerIpAddr.Size = new System.Drawing.Size(200, 28);
            this.textBox_ServerIpAddr.TabIndex = 58;
            this.textBox_ServerIpAddr.TabStop = false;
            // 
            // label_ServerIpAddr
            // 
            this.label_ServerIpAddr.AutoSize = true;
            this.label_ServerIpAddr.Location = new System.Drawing.Point(30, 232);
            this.label_ServerIpAddr.Name = "label_ServerIpAddr";
            this.label_ServerIpAddr.Size = new System.Drawing.Size(208, 21);
            this.label_ServerIpAddr.TabIndex = 59;
            this.label_ServerIpAddr.Text = "Server IP Address:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(351, 21);
            this.label5.TabIndex = 60;
            this.label5.Text = "Is Pico W a Server or a Client?";
            // 
            // FormNwConfig
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(862, 526);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_ServerIpAddr);
            this.Controls.Add(this.label_ServerIpAddr);
            this.Controls.Add(this.radioButton_Client);
            this.Controls.Add(this.radioButton_Server);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_SetConfig);
            this.Controls.Add(this.textBox_CountryCode);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBox_IpAddr);
            this.Controls.Add(this.label8);
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormNwConfig";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NwConfig";
            this.Load += new System.EventHandler(this.FormNetwork_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_CountryCode;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox_IpAddr;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button_SetConfig;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_SSID;
        private System.Windows.Forms.TextBox textBox_Password;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radioButton_Server;
        private System.Windows.Forms.RadioButton radioButton_Client;
        private System.Windows.Forms.TextBox textBox_ServerIpAddr;
        private System.Windows.Forms.Label label_ServerIpAddr;
        private System.Windows.Forms.Label label5;
    }
}