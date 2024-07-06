
namespace JigApp
{
    partial class FormPwm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button_Start = new System.Windows.Forms.Button();
            this.button_Stop = new System.Windows.Forms.Button();
            this.numericUpDown_Wrap = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_Level = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDown_Divider = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Wrap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Level)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Divider)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 141);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "clkdiv:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 204);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 21);
            this.label2.TabIndex = 2;
            this.label2.Text = "wrap:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 267);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(142, 21);
            this.label3.TabIndex = 5;
            this.label3.Text = "high_period:";
            // 
            // button_Start
            // 
            this.button_Start.Location = new System.Drawing.Point(269, 357);
            this.button_Start.Name = "button_Start";
            this.button_Start.Size = new System.Drawing.Size(150, 50);
            this.button_Start.TabIndex = 8;
            this.button_Start.TabStop = false;
            this.button_Start.Text = "PWM start";
            this.button_Start.UseVisualStyleBackColor = true;
            this.button_Start.Click += new System.EventHandler(this.button_Start_Click);
            // 
            // button_Stop
            // 
            this.button_Stop.Location = new System.Drawing.Point(452, 357);
            this.button_Stop.Name = "button_Stop";
            this.button_Stop.Size = new System.Drawing.Size(150, 50);
            this.button_Stop.TabIndex = 9;
            this.button_Stop.TabStop = false;
            this.button_Stop.Text = "PWM stop";
            this.button_Stop.UseVisualStyleBackColor = true;
            this.button_Stop.Click += new System.EventHandler(this.button_Stop_Click);
            // 
            // numericUpDown_Wrap
            // 
            this.numericUpDown_Wrap.Location = new System.Drawing.Point(40, 228);
            this.numericUpDown_Wrap.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDown_Wrap.Name = "numericUpDown_Wrap";
            this.numericUpDown_Wrap.Size = new System.Drawing.Size(170, 28);
            this.numericUpDown_Wrap.TabIndex = 56;
            this.numericUpDown_Wrap.TabStop = false;
            this.numericUpDown_Wrap.Value = new decimal(new int[] {
            4999,
            0,
            0,
            0});
            // 
            // numericUpDown_Level
            // 
            this.numericUpDown_Level.Location = new System.Drawing.Point(39, 291);
            this.numericUpDown_Level.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDown_Level.Name = "numericUpDown_Level";
            this.numericUpDown_Level.Size = new System.Drawing.Size(170, 28);
            this.numericUpDown_Level.TabIndex = 57;
            this.numericUpDown_Level.TabStop = false;
            this.numericUpDown_Level.Value = new decimal(new int[] {
            2500,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(215, 230);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 21);
            this.label4.TabIndex = 58;
            this.label4.Text = "0-65535";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(215, 293);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 21);
            this.label5.TabIndex = 59;
            this.label5.Text = "0-65535";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(215, 167);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(142, 21);
            this.label6.TabIndex = 60;
            this.label6.Text = "1-259.999999";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(34, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(494, 21);
            this.label7.TabIndex = 61;
            this.label7.Text = "PWM frequency = 125MHz / ((wrap+1) * clkdiv)";
            // 
            // numericUpDown_Divider
            // 
            this.numericUpDown_Divider.DecimalPlaces = 6;
            this.numericUpDown_Divider.Location = new System.Drawing.Point(41, 165);
            this.numericUpDown_Divider.Maximum = new decimal(new int[] {
            259999999,
            0,
            0,
            393216});
            this.numericUpDown_Divider.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_Divider.Name = "numericUpDown_Divider";
            this.numericUpDown_Divider.Size = new System.Drawing.Size(170, 28);
            this.numericUpDown_Divider.TabIndex = 62;
            this.numericUpDown_Divider.TabStop = false;
            this.numericUpDown_Divider.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(32, 51);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(395, 21);
            this.label8.TabIndex = 63;
            this.label8.Text = "duty ratio = high_period / (wrap+1)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(37, 104);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(494, 21);
            this.label9.TabIndex = 64;
            this.label9.Text = "e.g. PWM frequency = 100Hz, duty ratio = 50%";
            // 
            // FormPwm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(866, 435);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.numericUpDown_Divider);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericUpDown_Level);
            this.Controls.Add(this.numericUpDown_Wrap);
            this.Controls.Add(this.button_Stop);
            this.Controls.Add(this.button_Start);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormPwm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PWM";
            this.Load += new System.EventHandler(this.FormPwm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Wrap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Level)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Divider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_Start;
        private System.Windows.Forms.Button button_Stop;
        private System.Windows.Forms.NumericUpDown numericUpDown_Wrap;
        private System.Windows.Forms.NumericUpDown numericUpDown_Level;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericUpDown_Divider;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
    }
}