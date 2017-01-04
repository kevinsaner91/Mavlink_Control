namespace Radar_Config_and_Measurement_Tool
{
    partial class NumericInputDialog
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
            this.lbl_Text = new System.Windows.Forms.Label();
            this.lbl_unit = new System.Windows.Forms.Label();
            this.num_Value = new System.Windows.Forms.NumericUpDown();
            this.btn_Save = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.num_Value)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_Text
            // 
            this.lbl_Text.AutoSize = true;
            this.lbl_Text.Location = new System.Drawing.Point(16, 17);
            this.lbl_Text.Name = "lbl_Text";
            this.lbl_Text.Size = new System.Drawing.Size(211, 13);
            this.lbl_Text.TabIndex = 0;
            this.lbl_Text.Text = "Input waiting time for wall swing prevention:";
            // 
            // lbl_unit
            // 
            this.lbl_unit.AutoSize = true;
            this.lbl_unit.Location = new System.Drawing.Point(320, 17);
            this.lbl_unit.Name = "lbl_unit";
            this.lbl_unit.Size = new System.Drawing.Size(20, 13);
            this.lbl_unit.TabIndex = 1;
            this.lbl_unit.Text = "ms";
            // 
            // num_Value
            // 
            this.num_Value.Location = new System.Drawing.Point(233, 15);
            this.num_Value.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.num_Value.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.num_Value.Name = "num_Value";
            this.num_Value.Size = new System.Drawing.Size(81, 20);
            this.num_Value.TabIndex = 2;
            this.num_Value.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(141, 47);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(75, 23);
            this.btn_Save.TabIndex = 3;
            this.btn_Save.Text = "save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // NumericInputDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 83);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.num_Value);
            this.Controls.Add(this.lbl_unit);
            this.Controls.Add(this.lbl_Text);
            this.Name = "NumericInputDialog";
            this.Text = "Automation Setting";
            ((System.ComponentModel.ISupportInitialize)(this.num_Value)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lbl_Text;
        public System.Windows.Forms.Label lbl_unit;
        public System.Windows.Forms.NumericUpDown num_Value;
        private System.Windows.Forms.Button btn_Save;
    }
}