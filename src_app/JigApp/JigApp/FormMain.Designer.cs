
namespace JigApp
{
    partial class FormMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label_BoardId = new System.Windows.Forms.Label();
            this.button_Connect = new System.Windows.Forms.Button();
            this.comboBox_Port = new System.Windows.Forms.ComboBox();
            this.button_Spi = new System.Windows.Forms.Button();
            this.button_Uart = new System.Windows.Forms.Button();
            this.button_I2c = new System.Windows.Forms.Button();
            this.button_Pwm = new System.Windows.Forms.Button();
            this.button_Gpio = new System.Windows.Forms.Button();
            this.button_Adc = new System.Windows.Forms.Button();
            this.label_FwName = new System.Windows.Forms.Label();
            this.label_FwVer = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton_UsbMode = new System.Windows.Forms.RadioButton();
            this.radioButton_Wifi = new System.Windows.Forms.RadioButton();
            this.textBox_ServerIpAddr = new System.Windows.Forms.TextBox();
            this.label_ConnectStatus = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label_AppVer = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label_AppName = new System.Windows.Forms.Label();
            this.button_EraseFlash = new System.Windows.Forms.Button();
            this.textBox_FwErr = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.button_ClearFwErr = new System.Windows.Forms.Button();
            this.button_NwConfig = new System.Windows.Forms.Button();
            this.textBox_AppLog = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.button_ClearAppLog = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_BoardId
            // 
            this.label_BoardId.AutoSize = true;
            this.label_BoardId.Location = new System.Drawing.Point(219, 157);
            this.label_BoardId.Name = "label_BoardId";
            this.label_BoardId.Size = new System.Drawing.Size(43, 21);
            this.label_BoardId.TabIndex = 11;
            this.label_BoardId.Text = "---";
            // 
            // button_Connect
            // 
            this.button_Connect.Location = new System.Drawing.Point(254, 254);
            this.button_Connect.Name = "button_Connect";
            this.button_Connect.Size = new System.Drawing.Size(170, 50);
            this.button_Connect.TabIndex = 10;
            this.button_Connect.TabStop = false;
            this.button_Connect.Text = "connect";
            this.button_Connect.UseVisualStyleBackColor = true;
            this.button_Connect.Click += new System.EventHandler(this.button_Connect_Click);
            // 
            // comboBox_Port
            // 
            this.comboBox_Port.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Port.FormattingEnabled = true;
            this.comboBox_Port.Location = new System.Drawing.Point(46, 92);
            this.comboBox_Port.MaxDropDownItems = 100;
            this.comboBox_Port.Name = "comboBox_Port";
            this.comboBox_Port.Size = new System.Drawing.Size(154, 29);
            this.comboBox_Port.TabIndex = 9;
            this.comboBox_Port.TabStop = false;
            // 
            // button_Spi
            // 
            this.button_Spi.Location = new System.Drawing.Point(627, 240);
            this.button_Spi.Name = "button_Spi";
            this.button_Spi.Size = new System.Drawing.Size(100, 90);
            this.button_Spi.TabIndex = 17;
            this.button_Spi.TabStop = false;
            this.button_Spi.Text = "SPI";
            this.button_Spi.UseVisualStyleBackColor = true;
            this.button_Spi.Click += new System.EventHandler(this.button_Spi_Click);
            // 
            // button_Uart
            // 
            this.button_Uart.Location = new System.Drawing.Point(512, 240);
            this.button_Uart.Name = "button_Uart";
            this.button_Uart.Size = new System.Drawing.Size(100, 90);
            this.button_Uart.TabIndex = 18;
            this.button_Uart.TabStop = false;
            this.button_Uart.Text = "UART";
            this.button_Uart.UseVisualStyleBackColor = true;
            this.button_Uart.Click += new System.EventHandler(this.button_Uart_Click);
            // 
            // button_I2c
            // 
            this.button_I2c.Location = new System.Drawing.Point(742, 243);
            this.button_I2c.Name = "button_I2c";
            this.button_I2c.Size = new System.Drawing.Size(100, 90);
            this.button_I2c.TabIndex = 25;
            this.button_I2c.TabStop = false;
            this.button_I2c.Text = "I2C";
            this.button_I2c.UseVisualStyleBackColor = true;
            this.button_I2c.Click += new System.EventHandler(this.button_I2c_Click);
            // 
            // button_Pwm
            // 
            this.button_Pwm.Location = new System.Drawing.Point(742, 132);
            this.button_Pwm.Name = "button_Pwm";
            this.button_Pwm.Size = new System.Drawing.Size(100, 90);
            this.button_Pwm.TabIndex = 26;
            this.button_Pwm.TabStop = false;
            this.button_Pwm.Text = "PWM";
            this.button_Pwm.UseVisualStyleBackColor = true;
            this.button_Pwm.Click += new System.EventHandler(this.button_Pwm_Click);
            // 
            // button_Gpio
            // 
            this.button_Gpio.Location = new System.Drawing.Point(512, 129);
            this.button_Gpio.Name = "button_Gpio";
            this.button_Gpio.Size = new System.Drawing.Size(100, 90);
            this.button_Gpio.TabIndex = 27;
            this.button_Gpio.TabStop = false;
            this.button_Gpio.Text = "GPIO";
            this.button_Gpio.UseVisualStyleBackColor = true;
            this.button_Gpio.Click += new System.EventHandler(this.button_Gpio_Click);
            // 
            // button_Adc
            // 
            this.button_Adc.Location = new System.Drawing.Point(627, 129);
            this.button_Adc.Name = "button_Adc";
            this.button_Adc.Size = new System.Drawing.Size(100, 90);
            this.button_Adc.TabIndex = 28;
            this.button_Adc.TabStop = false;
            this.button_Adc.Text = "ADC";
            this.button_Adc.UseVisualStyleBackColor = true;
            this.button_Adc.Click += new System.EventHandler(this.button_Adc_Click);
            // 
            // label_FwName
            // 
            this.label_FwName.AutoSize = true;
            this.label_FwName.Location = new System.Drawing.Point(219, 94);
            this.label_FwName.Name = "label_FwName";
            this.label_FwName.Size = new System.Drawing.Size(43, 21);
            this.label_FwName.TabIndex = 29;
            this.label_FwName.Text = "---";
            // 
            // label_FwVer
            // 
            this.label_FwVer.AutoSize = true;
            this.label_FwVer.Location = new System.Drawing.Point(219, 125);
            this.label_FwVer.Name = "label_FwVer";
            this.label_FwVer.Size = new System.Drawing.Size(43, 21);
            this.label_FwVer.TabIndex = 30;
            this.label_FwVer.Text = "---";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton_UsbMode);
            this.groupBox1.Controls.Add(this.radioButton_Wifi);
            this.groupBox1.Controls.Add(this.textBox_ServerIpAddr);
            this.groupBox1.Controls.Add(this.label_ConnectStatus);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.button_Connect);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.comboBox_Port);
            this.groupBox1.Location = new System.Drawing.Point(14, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(477, 323);
            this.groupBox1.TabIndex = 31;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connect";
            // 
            // radioButton_UsbMode
            // 
            this.radioButton_UsbMode.AutoSize = true;
            this.radioButton_UsbMode.Checked = true;
            this.radioButton_UsbMode.Location = new System.Drawing.Point(18, 33);
            this.radioButton_UsbMode.Name = "radioButton_UsbMode";
            this.radioButton_UsbMode.Size = new System.Drawing.Size(123, 25);
            this.radioButton_UsbMode.TabIndex = 40;
            this.radioButton_UsbMode.TabStop = true;
            this.radioButton_UsbMode.Text = "USB Mode";
            this.radioButton_UsbMode.UseVisualStyleBackColor = true;
            this.radioButton_UsbMode.CheckedChanged += new System.EventHandler(this.radioButton_UsbMode_CheckedChanged);
            // 
            // radioButton_Wifi
            // 
            this.radioButton_Wifi.AutoSize = true;
            this.radioButton_Wifi.Location = new System.Drawing.Point(18, 141);
            this.radioButton_Wifi.Name = "radioButton_Wifi";
            this.radioButton_Wifi.Size = new System.Drawing.Size(277, 25);
            this.radioButton_Wifi.TabIndex = 41;
            this.radioButton_Wifi.Text = "Wi-Fi Mode(PicoW Only)";
            this.radioButton_Wifi.UseVisualStyleBackColor = true;
            // 
            // textBox_ServerIpAddr
            // 
            this.textBox_ServerIpAddr.Location = new System.Drawing.Point(46, 200);
            this.textBox_ServerIpAddr.Name = "textBox_ServerIpAddr";
            this.textBox_ServerIpAddr.Size = new System.Drawing.Size(200, 28);
            this.textBox_ServerIpAddr.TabIndex = 39;
            this.textBox_ServerIpAddr.TabStop = false;
            this.textBox_ServerIpAddr.Text = "192.168.10.100";
            // 
            // label_ConnectStatus
            // 
            this.label_ConnectStatus.BackColor = System.Drawing.SystemColors.Control;
            this.label_ConnectStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_ConnectStatus.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_ConnectStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label_ConnectStatus.Location = new System.Drawing.Point(70, 254);
            this.label_ConnectStatus.Name = "label_ConnectStatus";
            this.label_ConnectStatus.Size = new System.Drawing.Size(154, 50);
            this.label_ConnectStatus.TabIndex = 39;
            this.label_ConnectStatus.Text = "disconnected";
            this.label_ConnectStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(42, 176);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(417, 21);
            this.label8.TabIndex = 41;
            this.label8.Text = "IP address of the destination server:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(42, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 21);
            this.label4.TabIndex = 32;
            this.label4.Text = "COM Port:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label_AppVer);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label_FwVer);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label_AppName);
            this.groupBox2.Controls.Add(this.label_FwName);
            this.groupBox2.Controls.Add(this.label_BoardId);
            this.groupBox2.Location = new System.Drawing.Point(14, 343);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(477, 193);
            this.groupBox2.TabIndex = 32;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "App/FW Information";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 63);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(142, 21);
            this.label7.TabIndex = 39;
            this.label7.Text = "App Version:";
            // 
            // label_AppVer
            // 
            this.label_AppVer.AutoSize = true;
            this.label_AppVer.Location = new System.Drawing.Point(219, 61);
            this.label_AppVer.Name = "label_AppVer";
            this.label_AppVer.Size = new System.Drawing.Size(43, 21);
            this.label_AppVer.TabIndex = 38;
            this.label_AppVer.Text = "---";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 21);
            this.label5.TabIndex = 38;
            this.label5.Text = "App Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 157);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(186, 21);
            this.label3.TabIndex = 35;
            this.label3.Text = "Unique Board ID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 21);
            this.label2.TabIndex = 34;
            this.label2.Text = "FW Version:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 21);
            this.label1.TabIndex = 33;
            this.label1.Text = "FW Name:";
            // 
            // label_AppName
            // 
            this.label_AppName.AutoSize = true;
            this.label_AppName.Location = new System.Drawing.Point(219, 32);
            this.label_AppName.Name = "label_AppName";
            this.label_AppName.Size = new System.Drawing.Size(43, 21);
            this.label_AppName.TabIndex = 32;
            this.label_AppName.Text = "---";
            // 
            // button_EraseFlash
            // 
            this.button_EraseFlash.Location = new System.Drawing.Point(512, 404);
            this.button_EraseFlash.Name = "button_EraseFlash";
            this.button_EraseFlash.Size = new System.Drawing.Size(330, 65);
            this.button_EraseFlash.TabIndex = 34;
            this.button_EraseFlash.TabStop = false;
            this.button_EraseFlash.Text = "erase setting data in flash memory";
            this.button_EraseFlash.UseVisualStyleBackColor = true;
            this.button_EraseFlash.Click += new System.EventHandler(this.button_EraseFlash_Click);
            // 
            // textBox_FwErr
            // 
            this.textBox_FwErr.Location = new System.Drawing.Point(14, 768);
            this.textBox_FwErr.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.textBox_FwErr.Multiline = true;
            this.textBox_FwErr.Name = "textBox_FwErr";
            this.textBox_FwErr.ReadOnly = true;
            this.textBox_FwErr.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_FwErr.Size = new System.Drawing.Size(692, 84);
            this.textBox_FwErr.TabIndex = 35;
            this.textBox_FwErr.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 742);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 21);
            this.label6.TabIndex = 36;
            this.label6.Text = "FW Error:";
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // button_ClearFwErr
            // 
            this.button_ClearFwErr.Location = new System.Drawing.Point(721, 784);
            this.button_ClearFwErr.Name = "button_ClearFwErr";
            this.button_ClearFwErr.Size = new System.Drawing.Size(121, 50);
            this.button_ClearFwErr.TabIndex = 37;
            this.button_ClearFwErr.TabStop = false;
            this.button_ClearFwErr.Text = "clear";
            this.button_ClearFwErr.UseVisualStyleBackColor = true;
            this.button_ClearFwErr.Click += new System.EventHandler(this.button_ClearFwErr_Click);
            // 
            // button_NwConfig
            // 
            this.button_NwConfig.Location = new System.Drawing.Point(512, 17);
            this.button_NwConfig.Name = "button_NwConfig";
            this.button_NwConfig.Size = new System.Drawing.Size(100, 90);
            this.button_NwConfig.TabIndex = 38;
            this.button_NwConfig.TabStop = false;
            this.button_NwConfig.Text = "NW Config";
            this.button_NwConfig.UseVisualStyleBackColor = true;
            this.button_NwConfig.Click += new System.EventHandler(this.button_NwConfig_Click);
            // 
            // textBox_AppLog
            // 
            this.textBox_AppLog.Location = new System.Drawing.Point(14, 571);
            this.textBox_AppLog.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.textBox_AppLog.Multiline = true;
            this.textBox_AppLog.Name = "textBox_AppLog";
            this.textBox_AppLog.ReadOnly = true;
            this.textBox_AppLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_AppLog.Size = new System.Drawing.Size(692, 162);
            this.textBox_AppLog.TabIndex = 39;
            this.textBox_AppLog.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 545);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(98, 21);
            this.label9.TabIndex = 40;
            this.label9.Text = "App Log:";
            // 
            // button_ClearAppLog
            // 
            this.button_ClearAppLog.Location = new System.Drawing.Point(721, 627);
            this.button_ClearAppLog.Name = "button_ClearAppLog";
            this.button_ClearAppLog.Size = new System.Drawing.Size(121, 50);
            this.button_ClearAppLog.TabIndex = 41;
            this.button_ClearAppLog.TabStop = false;
            this.button_ClearAppLog.Text = "clear";
            this.button_ClearAppLog.UseVisualStyleBackColor = true;
            this.button_ClearAppLog.Click += new System.EventHandler(this.button_ClearAppLog_Click);
            // 
            // FormMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(856, 864);
            this.Controls.Add(this.button_ClearAppLog);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBox_AppLog);
            this.Controls.Add(this.button_NwConfig);
            this.Controls.Add(this.button_ClearFwErr);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox_FwErr);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button_EraseFlash);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_Adc);
            this.Controls.Add(this.button_Gpio);
            this.Controls.Add(this.button_Pwm);
            this.Controls.Add(this.button_I2c);
            this.Controls.Add(this.button_Uart);
            this.Controls.Add(this.button_Spi);
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label_BoardId;
        private System.Windows.Forms.Button button_Connect;
        private System.Windows.Forms.ComboBox comboBox_Port;
        private System.Windows.Forms.Button button_Spi;
        private System.Windows.Forms.Button button_Uart;
        private System.Windows.Forms.Button button_I2c;
        private System.Windows.Forms.Button button_Pwm;
        private System.Windows.Forms.Button button_Gpio;
        private System.Windows.Forms.Button button_Adc;
        private System.Windows.Forms.Label label_FwName;
        private System.Windows.Forms.Label label_FwVer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label_AppName;
        private System.Windows.Forms.Button button_EraseFlash;
        private System.Windows.Forms.TextBox textBox_FwErr;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button button_ClearFwErr;
        private System.Windows.Forms.Label label_ConnectStatus;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label_AppVer;
        private System.Windows.Forms.RadioButton radioButton_UsbMode;
        private System.Windows.Forms.RadioButton radioButton_Wifi;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox_ServerIpAddr;
        private System.Windows.Forms.Button button_NwConfig;
        private System.Windows.Forms.TextBox textBox_AppLog;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button_ClearAppLog;
    }
}

