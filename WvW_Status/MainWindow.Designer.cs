namespace WvW_Status
{
    partial class MainWindow
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
            this.radioButtonNA = new System.Windows.Forms.RadioButton();
            this.radioButtonEU = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // radioButtonNA
            // 
            this.radioButtonNA.AutoSize = true;
            this.radioButtonNA.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonNA.ForeColor = System.Drawing.Color.LightGray;
            this.radioButtonNA.Location = new System.Drawing.Point(18, 18);
            this.radioButtonNA.Name = "radioButtonNA";
            this.radioButtonNA.Size = new System.Drawing.Size(105, 23);
            this.radioButtonNA.TabIndex = 0;
            this.radioButtonNA.Text = "NA Servers";
            this.radioButtonNA.UseVisualStyleBackColor = true;
            this.radioButtonNA.CheckedChanged += new System.EventHandler(this.radioButtonNA_CheckedChanged);
            // 
            // radioButtonEU
            // 
            this.radioButtonEU.AutoSize = true;
            this.radioButtonEU.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonEU.ForeColor = System.Drawing.Color.LightGray;
            this.radioButtonEU.Location = new System.Drawing.Point(129, 18);
            this.radioButtonEU.Name = "radioButtonEU";
            this.radioButtonEU.Size = new System.Drawing.Size(103, 23);
            this.radioButtonEU.TabIndex = 1;
            this.radioButtonEU.Text = "EU Servers";
            this.radioButtonEU.UseVisualStyleBackColor = true;
            this.radioButtonEU.CheckedChanged += new System.EventHandler(this.radioButtonEU_CheckedChanged);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(242, 58);
            this.Controls.Add(this.radioButtonEU);
            this.Controls.Add(this.radioButtonNA);
            this.Font = new System.Drawing.Font("Cambria", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Text = "Guild Wars 2 WvW Status";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonNA;
        private System.Windows.Forms.RadioButton radioButtonEU;
    }
}

