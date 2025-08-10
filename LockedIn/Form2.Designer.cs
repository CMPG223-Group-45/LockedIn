namespace LockedIn
{
    partial class frmAdministrator
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpMain = new System.Windows.Forms.TabPage();
            this.tpPrisoner = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tpMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpMain);
            this.tabControl1.Controls.Add(this.tpPrisoner);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(801, 451);
            this.tabControl1.TabIndex = 0;
            // 
            // tpMain
            // 
            this.tpMain.Controls.Add(this.button5);
            this.tpMain.Controls.Add(this.button4);
            this.tpMain.Controls.Add(this.button3);
            this.tpMain.Controls.Add(this.button2);
            this.tpMain.Controls.Add(this.button1);
            this.tpMain.Location = new System.Drawing.Point(4, 22);
            this.tpMain.Name = "tpMain";
            this.tpMain.Padding = new System.Windows.Forms.Padding(3);
            this.tpMain.Size = new System.Drawing.Size(793, 425);
            this.tpMain.TabIndex = 0;
            this.tpMain.Text = "tabPage1";
            this.tpMain.UseVisualStyleBackColor = true;
            // 
            // tpPrisoner
            // 
            this.tpPrisoner.Location = new System.Drawing.Point(4, 22);
            this.tpPrisoner.Name = "tpPrisoner";
            this.tpPrisoner.Padding = new System.Windows.Forms.Padding(3);
            this.tpPrisoner.Size = new System.Drawing.Size(793, 425);
            this.tpPrisoner.TabIndex = 1;
            this.tpPrisoner.Text = "tabPage2";
            this.tpPrisoner.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(64, 90);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(64, 119);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(64, 148);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(64, 177);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 3;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(64, 206);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 4;
            this.button5.Text = "button5";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // frmAdministrator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.Name = "frmAdministrator";
            this.Text = "Administrator";
            this.tabControl1.ResumeLayout(false);
            this.tpMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpMain;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabPage tpPrisoner;
    }
}