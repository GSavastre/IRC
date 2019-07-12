namespace Client
{
    partial class Chat
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
            this.tb_msg = new System.Windows.Forms.TextBox();
            this.btn_send = new System.Windows.Forms.Button();
            this.lb_chat = new System.Windows.Forms.ListBox();
            this.l_partner = new System.Windows.Forms.Label();
            this.l_chatting_with = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tb_msg
            // 
            this.tb_msg.Location = new System.Drawing.Point(12, 235);
            this.tb_msg.Multiline = true;
            this.tb_msg.Name = "tb_msg";
            this.tb_msg.Size = new System.Drawing.Size(288, 52);
            this.tb_msg.TabIndex = 0;
            // 
            // btn_send
            // 
            this.btn_send.Location = new System.Drawing.Point(306, 235);
            this.btn_send.Name = "btn_send";
            this.btn_send.Size = new System.Drawing.Size(67, 52);
            this.btn_send.TabIndex = 1;
            this.btn_send.Text = "Send";
            this.btn_send.UseVisualStyleBackColor = true;
            this.btn_send.Click += new System.EventHandler(this.btn_test_Click);
            // 
            // lb_chat
            // 
            this.lb_chat.FormattingEnabled = true;
            this.lb_chat.Location = new System.Drawing.Point(12, 29);
            this.lb_chat.Name = "lb_chat";
            this.lb_chat.Size = new System.Drawing.Size(361, 186);
            this.lb_chat.TabIndex = 2;
            // 
            // l_partner
            // 
            this.l_partner.AutoSize = true;
            this.l_partner.Location = new System.Drawing.Point(95, 10);
            this.l_partner.Name = "l_partner";
            this.l_partner.Size = new System.Drawing.Size(35, 13);
            this.l_partner.TabIndex = 3;
            this.l_partner.Text = "label1";
            // 
            // l_chatting_with
            // 
            this.l_chatting_with.AutoSize = true;
            this.l_chatting_with.Location = new System.Drawing.Point(12, 10);
            this.l_chatting_with.Name = "l_chatting_with";
            this.l_chatting_with.Size = new System.Drawing.Size(77, 13);
            this.l_chatting_with.TabIndex = 4;
            this.l_chatting_with.Text = "Chatting with : ";
            // 
            // Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 299);
            this.Controls.Add(this.l_chatting_with);
            this.Controls.Add(this.l_partner);
            this.Controls.Add(this.lb_chat);
            this.Controls.Add(this.btn_send);
            this.Controls.Add(this.tb_msg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Chat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_msg;
        private System.Windows.Forms.Button btn_send;
        private System.Windows.Forms.ListBox lb_chat;
        private System.Windows.Forms.Label l_partner;
        private System.Windows.Forms.Label l_chatting_with;
    }
}

