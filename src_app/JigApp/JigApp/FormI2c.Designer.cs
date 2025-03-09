
namespace JigApp
{
    partial class FormI2c
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
            this.button_Send = new System.Windows.Forms.Button();
            this.textBox_Log = new System.Windows.Forms.TextBox();
            this.textBox_SendData = new System.Windows.Forms.TextBox();
            this.button_Recv = new System.Windows.Forms.Button();
            this.button_SetConfig = new System.Windows.Forms.Button();
            this.numericUpDown_Freq = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown_RecvSize = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDown_SlaveAddr = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button_Clear = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Freq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_RecvSize)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_SlaveAddr)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Send
            // 
            this.button_Send.Location = new System.Drawing.Point(679, 127);
            this.button_Send.Name = "button_Send";
            this.button_Send.Size = new System.Drawing.Size(150, 50);
            this.button_Send.TabIndex = 19;
            this.button_Send.TabStop = false;
            this.button_Send.Text = "send";
            this.button_Send.UseVisualStyleBackColor = true;
            this.button_Send.Click += new System.EventHandler(this.button_Send_Click);
            // 
            // textBox_Log
            // 
            this.textBox_Log.Location = new System.Drawing.Point(15, 593);
            this.textBox_Log.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.textBox_Log.Multiline = true;
            this.textBox_Log.Name = "textBox_Log";
            this.textBox_Log.ReadOnly = true;
            this.textBox_Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Log.Size = new System.Drawing.Size(658, 170);
            this.textBox_Log.TabIndex = 18;
            this.textBox_Log.TabStop = false;
            // 
            // textBox_SendData
            // 
            this.textBox_SendData.Location = new System.Drawing.Point(18, 90);
            this.textBox_SendData.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.textBox_SendData.Multiline = true;
            this.textBox_SendData.Name = "textBox_SendData";
            this.textBox_SendData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_SendData.Size = new System.Drawing.Size(640, 120);
            this.textBox_SendData.TabIndex = 17;
            this.textBox_SendData.TabStop = false;
            // 
            // button_Recv
            // 
            this.button_Recv.Location = new System.Drawing.Point(679, 33);
            this.button_Recv.Name = "button_Recv";
            this.button_Recv.Size = new System.Drawing.Size(150, 50);
            this.button_Recv.TabIndex = 20;
            this.button_Recv.TabStop = false;
            this.button_Recv.Text = "receive";
            this.button_Recv.UseVisualStyleBackColor = true;
            this.button_Recv.Click += new System.EventHandler(this.button_Recv_Click);
            // 
            // button_SetConfig
            // 
            this.button_SetConfig.Location = new System.Drawing.Point(662, 31);
            this.button_SetConfig.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button_SetConfig.Name = "button_SetConfig";
            this.button_SetConfig.Size = new System.Drawing.Size(174, 50);
            this.button_SetConfig.TabIndex = 48;
            this.button_SetConfig.TabStop = false;
            this.button_SetConfig.Text = "setting change";
            this.button_SetConfig.UseVisualStyleBackColor = true;
            this.button_SetConfig.Click += new System.EventHandler(this.button_SetConfig_Click);
            // 
            // numericUpDown_Freq
            // 
            this.numericUpDown_Freq.Location = new System.Drawing.Point(18, 53);
            this.numericUpDown_Freq.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericUpDown_Freq.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown_Freq.Name = "numericUpDown_Freq";
            this.numericUpDown_Freq.Size = new System.Drawing.Size(150, 28);
            this.numericUpDown_Freq.TabIndex = 55;
            this.numericUpDown_Freq.TabStop = false;
            this.numericUpDown_Freq.Value = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 130);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(219, 21);
            this.label1.TabIndex = 56;
            this.label1.Text = "7bit slave address:";
            // 
            // numericUpDown_RecvSize
            // 
            this.numericUpDown_RecvSize.Location = new System.Drawing.Point(18, 57);
            this.numericUpDown_RecvSize.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDown_RecvSize.Name = "numericUpDown_RecvSize";
            this.numericUpDown_RecvSize.Size = new System.Drawing.Size(150, 28);
            this.numericUpDown_RecvSize.TabIndex = 58;
            this.numericUpDown_RecvSize.TabStop = false;
            this.numericUpDown_RecvSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(142, 21);
            this.label3.TabIndex = 59;
            this.label3.Text = "receive size";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.numericUpDown_Freq);
            this.groupBox1.Controls.Add(this.button_SetConfig);
            this.groupBox1.Location = new System.Drawing.Point(15, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(850, 98);
            this.groupBox1.TabIndex = 60;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Communication Setting";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(176, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(164, 21);
            this.label5.TabIndex = 71;
            this.label5.Text = "100000 or more";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(164, 21);
            this.label4.TabIndex = 64;
            this.label4.Text = "frequency(Hz):";
            // 
            // numericUpDown_SlaveAddr
            // 
            this.numericUpDown_SlaveAddr.Hexadecimal = true;
            this.numericUpDown_SlaveAddr.Location = new System.Drawing.Point(15, 152);
            this.numericUpDown_SlaveAddr.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.numericUpDown_SlaveAddr.Name = "numericUpDown_SlaveAddr";
            this.numericUpDown_SlaveAddr.Size = new System.Drawing.Size(120, 28);
            this.numericUpDown_SlaveAddr.TabIndex = 61;
            this.numericUpDown_SlaveAddr.TabStop = false;
            this.numericUpDown_SlaveAddr.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 34);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(505, 42);
            this.label6.TabIndex = 62;
            this.label6.Text = "send data:Hex(00-FF) separator:space or comma\r\ne.g. 00,01,FE,FF";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 567);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(186, 21);
            this.label7.TabIndex = 63;
            this.label7.Text = "Send/Receive Log";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(141, 155);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 21);
            this.label2.TabIndex = 66;
            this.label2.Text = "Hex(00-7F)";
            // 
            // button_Clear
            // 
            this.button_Clear.Location = new System.Drawing.Point(694, 644);
            this.button_Clear.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(150, 50);
            this.button_Clear.TabIndex = 67;
            this.button_Clear.TabStop = false;
            this.button_Clear.Text = "clear";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(665, 90);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(175, 21);
            this.label8.TabIndex = 68;
            this.label8.Text = "send size:1-256";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.textBox_SendData);
            this.groupBox2.Controls.Add(this.button_Send);
            this.groupBox2.Location = new System.Drawing.Point(15, 201);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(850, 230);
            this.groupBox2.TabIndex = 69;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Send";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.numericUpDown_RecvSize);
            this.groupBox3.Controls.Add(this.button_Recv);
            this.groupBox3.Location = new System.Drawing.Point(15, 449);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(850, 104);
            this.groupBox3.TabIndex = 70;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Receive";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(174, 59);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 21);
            this.label9.TabIndex = 60;
            this.label9.Text = "1-256";
            // 
            // FormI2c
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(881, 779);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button_Clear);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.numericUpDown_SlaveAddr);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_Log);
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormI2c";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "I2C";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormI2c_FormClosing);
            this.Load += new System.EventHandler(this.FormI2c_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Freq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_RecvSize)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_SlaveAddr)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Send;
        private System.Windows.Forms.TextBox textBox_Log;
        private System.Windows.Forms.TextBox textBox_SendData;
        private System.Windows.Forms.Button button_Recv;
        private System.Windows.Forms.Button button_SetConfig;
        private System.Windows.Forms.NumericUpDown numericUpDown_Freq;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown_RecvSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numericUpDown_SlaveAddr;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
    }
}