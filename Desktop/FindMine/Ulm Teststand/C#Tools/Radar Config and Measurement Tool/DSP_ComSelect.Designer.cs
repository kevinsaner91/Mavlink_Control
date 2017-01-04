namespace Radar_Config_and_Measurement_Tool
{
    partial class DSP_ComSelect
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DSP_ComSelect));
            this.cB_Com = new System.Windows.Forms.ComboBox();
            this.btn_save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cB_Com
            // 
            this.cB_Com.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cB_Com.FormattingEnabled = true;
            this.cB_Com.Location = new System.Drawing.Point(40, 12);
            this.cB_Com.Name = "cB_Com";
            this.cB_Com.Size = new System.Drawing.Size(139, 21);
            this.cB_Com.TabIndex = 0;
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(72, 44);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(75, 23);
            this.btn_save.TabIndex = 1;
            this.btn_save.Text = "save";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // DSP_ComSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(218, 79);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.cB_Com);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DSP_ComSelect";
            this.Text = "Select COM Port";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cB_Com;
        private System.Windows.Forms.Button btn_save;
    }
}