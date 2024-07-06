
namespace JigApp
{
    partial class FormAdc
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
            this.label_Adc0 = new System.Windows.Forms.Label();
            this.label_Adc1 = new System.Windows.Forms.Label();
            this.label_Adc2 = new System.Windows.Forms.Label();
            this.label_Temp = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label_Adc0
            // 
            this.label_Adc0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_Adc0.Location = new System.Drawing.Point(15, 34);
            this.label_Adc0.Name = "label_Adc0";
            this.label_Adc0.Size = new System.Drawing.Size(150, 25);
            this.label_Adc0.TabIndex = 0;
            this.label_Adc0.Text = "---";
            this.label_Adc0.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_Adc1
            // 
            this.label_Adc1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_Adc1.Location = new System.Drawing.Point(15, 97);
            this.label_Adc1.Name = "label_Adc1";
            this.label_Adc1.Size = new System.Drawing.Size(150, 25);
            this.label_Adc1.TabIndex = 1;
            this.label_Adc1.Text = "---";
            this.label_Adc1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_Adc2
            // 
            this.label_Adc2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_Adc2.Location = new System.Drawing.Point(15, 163);
            this.label_Adc2.Name = "label_Adc2";
            this.label_Adc2.Size = new System.Drawing.Size(150, 25);
            this.label_Adc2.TabIndex = 2;
            this.label_Adc2.Text = "---";
            this.label_Adc2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_Temp
            // 
            this.label_Temp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_Temp.Location = new System.Drawing.Point(15, 233);
            this.label_Temp.Name = "label_Temp";
            this.label_Temp.Size = new System.Drawing.Size(150, 25);
            this.label_Temp.TabIndex = 3;
            this.label_Temp.Text = "---";
            this.label_Temp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 21);
            this.label1.TabIndex = 4;
            this.label1.Text = "ADC0:[V]";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 21);
            this.label2.TabIndex = 5;
            this.label2.Text = "ADC1:[V]";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 212);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(472, 21);
            this.label4.TabIndex = 7;
            this.label4.Text = "ADC4(temperature sensor):[degrees Celsius]";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 21);
            this.label3.TabIndex = 8;
            this.label3.Text = "ADC2:[V]";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormAdc
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(866, 277);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label_Temp);
            this.Controls.Add(this.label_Adc2);
            this.Controls.Add(this.label_Adc1);
            this.Controls.Add(this.label_Adc0);
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormAdc";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ADC";
            this.Load += new System.EventHandler(this.FormAdc_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_Adc0;
        private System.Windows.Forms.Label label_Adc1;
        private System.Windows.Forms.Label label_Adc2;
        private System.Windows.Forms.Label label_Temp;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}