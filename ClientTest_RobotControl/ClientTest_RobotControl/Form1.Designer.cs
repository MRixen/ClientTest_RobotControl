namespace ClientTest_RobotControl
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
            this.button_Motor1_ok = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_Motor1_ok
            // 
            this.button_Motor1_ok.Location = new System.Drawing.Point(28, 25);
            this.button_Motor1_ok.Name = "button_Motor1_ok";
            this.button_Motor1_ok.Size = new System.Drawing.Size(75, 23);
            this.button_Motor1_ok.TabIndex = 0;
            this.button_Motor1_ok.Text = "Motor 1 ok";
            this.button_Motor1_ok.UseVisualStyleBackColor = true;
            this.button_Motor1_ok.Click += new System.EventHandler(this.button_Motor1_ok_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(28, 54);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Motor 2 ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button_Motor2_ok_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 585);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button_Motor1_ok);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_Motor1_ok;
        private System.Windows.Forms.Button button1;
    }
}

