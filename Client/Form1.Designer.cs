namespace Client
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.tb_test = new System.Windows.Forms.TextBox();
            this.btn_test = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tb_test
            // 
            this.tb_test.Location = new System.Drawing.Point(51, 36);
            this.tb_test.Name = "tb_test";
            this.tb_test.Size = new System.Drawing.Size(100, 20);
            this.tb_test.TabIndex = 0;
            this.tb_test.Text = "Hi ! Dax here.";
            // 
            // btn_test
            // 
            this.btn_test.Location = new System.Drawing.Point(184, 34);
            this.btn_test.Name = "btn_test";
            this.btn_test.Size = new System.Drawing.Size(40, 23);
            this.btn_test.TabIndex = 1;
            this.btn_test.Text = "Send";
            this.btn_test.UseVisualStyleBackColor = true;
            this.btn_test.Click += new System.EventHandler(this.btn_test_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 348);
            this.Controls.Add(this.btn_test);
            this.Controls.Add(this.tb_test);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_test;
        private System.Windows.Forms.Button btn_test;
    }
}

