
namespace JigApp
{
    partial class FormSpi
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
            this.textBox_Log = new System.Windows.Forms.TextBox();
            this.textBox_SendData = new System.Windows.Forms.TextBox();
            this.button_Send = new System.Windows.Forms.Button();
            this.button_Clear = new System.Windows.Forms.Button();
            this.button_SetConfig = new System.Windows.Forms.Button();
            this.comboBox_Mode = new System.Windows.Forms.ComboBox();
            this.numericUpDown_Freq = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Freq)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_Log
            // 
            this.textBox_Log.Location = new System.Drawing.Point(17, 480);
            this.textBox_Log.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.textBox_Log.Multiline = true;
            this.textBox_Log.Name = "textBox_Log";
            this.textBox_Log.ReadOnly = true;
            this.textBox_Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Log.Size = new System.Drawing.Size(640, 170);
            this.textBox_Log.TabIndex = 15;
            this.textBox_Log.TabStop = false;
            // 
            // textBox_SendData
            // 
            this.textBox_SendData.Location = new System.Drawing.Point(15, 319);
            this.textBox_SendData.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.textBox_SendData.Multiline = true;
            this.textBox_SendData.Name = "textBox_SendData";
            this.textBox_SendData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_SendData.Size = new System.Drawing.Size(640, 120);
            this.textBox_SendData.TabIndex = 14;
            this.textBox_SendData.TabStop = false;
            // 
            // button_Send
            // 
            this.button_Send.Location = new System.Drawing.Point(682, 356);
            this.button_Send.Name = "button_Send";
            this.button_Send.Size = new System.Drawing.Size(150, 50);
            this.button_Send.TabIndex = 16;
            this.button_Send.TabStop = false;
            this.button_Send.Text = "send";
            this.button_Send.UseVisualStyleBackColor = true;
            this.button_Send.Click += new System.EventHandler(this.button_Send_Click);
            // 
            // button_Clear
            // 
            this.button_Clear.Location = new System.Drawing.Point(682, 535);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(150, 50);
            this.button_Clear.TabIndex = 17;
            this.button_Clear.TabStop = false;
            this.button_Clear.Text = "clear";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // button_SetConfig
            // 
            this.button_SetConfig.Location = new System.Drawing.Point(633, 176);
            this.button_SetConfig.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button_SetConfig.Name = "button_SetConfig";
            this.button_SetConfig.Size = new System.Drawing.Size(180, 50);
            this.button_SetConfig.TabIndex = 46;
            this.button_SetConfig.TabStop = false;
            this.button_SetConfig.Text = "setting change";
            this.button_SetConfig.UseVisualStyleBackColor = true;
            this.button_SetConfig.Click += new System.EventHandler(this.button_SetConfig_Click);
            // 
            // comboBox_Mode
            // 
            this.comboBox_Mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Mode.FormattingEnabled = true;
            this.comboBox_Mode.Location = new System.Drawing.Point(20, 119);
            this.comboBox_Mode.MaxDropDownItems = 100;
            this.comboBox_Mode.Name = "comboBox_Mode";
            this.comboBox_Mode.Size = new System.Drawing.Size(260, 29);
            this.comboBox_Mode.TabIndex = 48;
            this.comboBox_Mode.TabStop = false;
            // 
            // numericUpDown_Freq
            // 
            this.numericUpDown_Freq.Location = new System.Drawing.Point(20, 57);
            this.numericUpDown_Freq.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericUpDown_Freq.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDown_Freq.Name = "numericUpDown_Freq";
            this.numericUpDown_Freq.Size = new System.Drawing.Size(150, 28);
            this.numericUpDown_Freq.TabIndex = 54;
            this.numericUpDown_Freq.TabStop = false;
            this.numericUpDown_Freq.Value = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.button_SetConfig);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.numericUpDown_Freq);
            this.groupBox1.Controls.Add(this.comboBox_Mode);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(19, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(828, 242);
            this.groupBox1.TabIndex = 55;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Communication setting";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(176, 61);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(175, 21);
            this.label10.TabIndex = 68;
            this.label10.Text = "1000000 or more";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(164, 21);
            this.label4.TabIndex = 55;
            this.label4.Text = "frequency(Hz):";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 95);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 21);
            this.label7.TabIndex = 59;
            this.label7.Text = "Mode:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 191);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(230, 21);
            this.label9.TabIndex = 61;
            this.label9.Text = "byte order:MSB First";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 161);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 21);
            this.label5.TabIndex = 56;
            this.label5.Text = "data bit:8";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 268);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(505, 42);
            this.label6.TabIndex = 56;
            this.label6.Text = "send data:Hex(00-FF) separator:space or comma\r\ne.g. 00,01,FE,FF\r\n";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 454);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(197, 21);
            this.label3.TabIndex = 58;
            this.label3.Text = "send/receive log:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(668, 319);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(175, 21);
            this.label11.TabIndex = 69;
            this.label11.Text = "send size:1-256";
            // 
            // FormSpi
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(866, 665);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_Clear);
            this.Controls.Add(this.button_Send);
            this.Controls.Add(this.textBox_Log);
            this.Controls.Add(this.textBox_SendData);
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormSpi";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SPI";
            this.Load += new System.EventHandler(this.FormSpi_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Freq)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_Log;
        private System.Windows.Forms.TextBox textBox_SendData;
        private System.Windows.Forms.Button button_Send;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.Button button_SetConfig;
        private System.Windows.Forms.ComboBox comboBox_Mode;
        private System.Windows.Forms.NumericUpDown numericUpDown_Freq;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
    }
}