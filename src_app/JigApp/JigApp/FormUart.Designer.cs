
namespace JigApp
{
    partial class FormUart
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
            this.button_Send = new System.Windows.Forms.Button();
            this.textBox_Log = new System.Windows.Forms.TextBox();
            this.textBox_SendData = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button_SetConfig = new System.Windows.Forms.Button();
            this.comboBox_Parity = new System.Windows.Forms.ComboBox();
            this.comboBox_StopBits = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDown_Baudrate = new System.Windows.Forms.NumericUpDown();
            this.label_sendData = new System.Windows.Forms.Label();
            this.label_log = new System.Windows.Forms.Label();
            this.button_Clear = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.label_sendSize = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Baudrate)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Send
            // 
            this.button_Send.Location = new System.Drawing.Point(684, 367);
            this.button_Send.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button_Send.Name = "button_Send";
            this.button_Send.Size = new System.Drawing.Size(150, 50);
            this.button_Send.TabIndex = 17;
            this.button_Send.TabStop = false;
            this.button_Send.Text = "send";
            this.button_Send.UseVisualStyleBackColor = true;
            this.button_Send.Click += new System.EventHandler(this.button_Send_Click);
            // 
            // textBox_Log
            // 
            this.textBox_Log.Location = new System.Drawing.Point(18, 485);
            this.textBox_Log.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.textBox_Log.Multiline = true;
            this.textBox_Log.Name = "textBox_Log";
            this.textBox_Log.ReadOnly = true;
            this.textBox_Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Log.Size = new System.Drawing.Size(640, 170);
            this.textBox_Log.TabIndex = 16;
            this.textBox_Log.TabStop = false;
            // 
            // textBox_SendData
            // 
            this.textBox_SendData.Location = new System.Drawing.Point(16, 327);
            this.textBox_SendData.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.textBox_SendData.Multiline = true;
            this.textBox_SendData.Name = "textBox_SendData";
            this.textBox_SendData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_SendData.Size = new System.Drawing.Size(640, 120);
            this.textBox_SendData.TabIndex = 15;
            this.textBox_SendData.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 21);
            this.label1.TabIndex = 18;
            this.label1.Text = "baud rate(bps):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 172);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 21);
            this.label2.TabIndex = 19;
            this.label2.Text = "parity:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 133);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 21);
            this.label3.TabIndex = 20;
            this.label3.Text = "stop bit:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 21);
            this.label4.TabIndex = 24;
            this.label4.Text = "data bit:8";
            // 
            // button_SetConfig
            // 
            this.button_SetConfig.Location = new System.Drawing.Point(628, 178);
            this.button_SetConfig.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button_SetConfig.Name = "button_SetConfig";
            this.button_SetConfig.Size = new System.Drawing.Size(190, 50);
            this.button_SetConfig.TabIndex = 27;
            this.button_SetConfig.TabStop = false;
            this.button_SetConfig.Text = "setting change";
            this.button_SetConfig.UseVisualStyleBackColor = true;
            this.button_SetConfig.Click += new System.EventHandler(this.button_SetConfig_Click);
            // 
            // comboBox_Parity
            // 
            this.comboBox_Parity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Parity.FormattingEnabled = true;
            this.comboBox_Parity.Location = new System.Drawing.Point(128, 169);
            this.comboBox_Parity.MaxDropDownItems = 100;
            this.comboBox_Parity.Name = "comboBox_Parity";
            this.comboBox_Parity.Size = new System.Drawing.Size(90, 29);
            this.comboBox_Parity.TabIndex = 33;
            this.comboBox_Parity.TabStop = false;
            // 
            // comboBox_StopBits
            // 
            this.comboBox_StopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_StopBits.FormattingEnabled = true;
            this.comboBox_StopBits.Location = new System.Drawing.Point(128, 130);
            this.comboBox_StopBits.MaxDropDownItems = 100;
            this.comboBox_StopBits.Name = "comboBox_StopBits";
            this.comboBox_StopBits.Size = new System.Drawing.Size(90, 29);
            this.comboBox_StopBits.TabIndex = 34;
            this.comboBox_StopBits.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.numericUpDown_Baudrate);
            this.groupBox1.Controls.Add(this.button_SetConfig);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBox_StopBits);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.comboBox_Parity);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(16, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(830, 246);
            this.groupBox1.TabIndex = 36;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Communication Setting";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(173, 60);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(142, 21);
            this.label8.TabIndex = 64;
            this.label8.Text = "4800 or more";
            // 
            // numericUpDown_Baudrate
            // 
            this.numericUpDown_Baudrate.Location = new System.Drawing.Point(17, 57);
            this.numericUpDown_Baudrate.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericUpDown_Baudrate.Minimum = new decimal(new int[] {
            4800,
            0,
            0,
            0});
            this.numericUpDown_Baudrate.Name = "numericUpDown_Baudrate";
            this.numericUpDown_Baudrate.Size = new System.Drawing.Size(150, 28);
            this.numericUpDown_Baudrate.TabIndex = 41;
            this.numericUpDown_Baudrate.TabStop = false;
            this.numericUpDown_Baudrate.Value = new decimal(new int[] {
            9600,
            0,
            0,
            0});
            // 
            // label_sendData
            // 
            this.label_sendData.AutoSize = true;
            this.label_sendData.Location = new System.Drawing.Point(15, 280);
            this.label_sendData.Name = "label_sendData";
            this.label_sendData.Size = new System.Drawing.Size(505, 42);
            this.label_sendData.TabIndex = 37;
            this.label_sendData.Text = "Send Data:Hex(00-FF) separator:space or comma\r\ne.g. 00,01,FE,FF\r\n";
            // 
            // label_log
            // 
            this.label_log.AutoSize = true;
            this.label_log.Location = new System.Drawing.Point(12, 459);
            this.label_log.Name = "label_log";
            this.label_log.Size = new System.Drawing.Size(197, 21);
            this.label_log.TabIndex = 38;
            this.label_log.Text = "Send/Receive Log:";
            // 
            // button_Clear
            // 
            this.button_Clear.Location = new System.Drawing.Point(684, 539);
            this.button_Clear.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(150, 50);
            this.button_Clear.TabIndex = 39;
            this.button_Clear.TabStop = false;
            this.button_Clear.Text = "clear";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // label_sendSize
            // 
            this.label_sendSize.AutoSize = true;
            this.label_sendSize.Location = new System.Drawing.Point(671, 330);
            this.label_sendSize.Name = "label_sendSize";
            this.label_sendSize.Size = new System.Drawing.Size(175, 21);
            this.label_sendSize.TabIndex = 70;
            this.label_sendSize.Text = "send size:1-256";
            // 
            // FormUart
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(866, 671);
            this.Controls.Add(this.label_sendSize);
            this.Controls.Add(this.button_Clear);
            this.Controls.Add(this.label_log);
            this.Controls.Add(this.label_sendData);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_Send);
            this.Controls.Add(this.textBox_Log);
            this.Controls.Add(this.textBox_SendData);
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormUart";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UART";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormUart_FormClosing);
            this.Load += new System.EventHandler(this.FormUart_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Baudrate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Send;
        private System.Windows.Forms.TextBox textBox_Log;
        private System.Windows.Forms.TextBox textBox_SendData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button_SetConfig;
        private System.Windows.Forms.ComboBox comboBox_Parity;
        private System.Windows.Forms.ComboBox comboBox_StopBits;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label_sendData;
        private System.Windows.Forms.Label label_log;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.NumericUpDown numericUpDown_Baudrate;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label_sendSize;
    }
}