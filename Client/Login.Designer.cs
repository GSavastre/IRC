namespace Client
{
    partial class Login
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
            this.btn_login = new System.Windows.Forms.Button();
            this.btn_switch_reg = new System.Windows.Forms.Button();
            this.tb_log_username = new System.Windows.Forms.TextBox();
            this.tb_log_password = new System.Windows.Forms.TextBox();
            this.lb_username = new System.Windows.Forms.Label();
            this.lb_password = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_login
            // 
            this.btn_login.Location = new System.Drawing.Point(157, 125);
            this.btn_login.Name = "btn_login";
            this.btn_login.Size = new System.Drawing.Size(115, 59);
            this.btn_login.TabIndex = 0;
            this.btn_login.Text = "Login";
            this.btn_login.UseVisualStyleBackColor = true;
            this.btn_login.Click += new System.EventHandler(this.btn_login_Click);
            // 
            // btn_switch_reg
            // 
            this.btn_switch_reg.Location = new System.Drawing.Point(318, 181);
            this.btn_switch_reg.Name = "btn_switch_reg";
            this.btn_switch_reg.Size = new System.Drawing.Size(83, 27);
            this.btn_switch_reg.TabIndex = 1;
            this.btn_switch_reg.Text = "Registrazione";
            this.btn_switch_reg.UseVisualStyleBackColor = true;
            this.btn_switch_reg.Click += new System.EventHandler(this.btn_switch_reg_Click);
            // 
            // tb_log_username
            // 
            this.tb_log_username.Location = new System.Drawing.Point(108, 35);
            this.tb_log_username.Name = "tb_log_username";
            this.tb_log_username.Size = new System.Drawing.Size(240, 20);
            this.tb_log_username.TabIndex = 2;
            this.tb_log_username.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tb_log_password
            // 
            this.tb_log_password.Location = new System.Drawing.Point(108, 77);
            this.tb_log_password.Name = "tb_log_password";
            this.tb_log_password.Size = new System.Drawing.Size(240, 20);
            this.tb_log_password.TabIndex = 3;
            this.tb_log_password.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tb_log_password.UseSystemPasswordChar = true;
            // 
            // lb_username
            // 
            this.lb_username.AutoSize = true;
            this.lb_username.Location = new System.Drawing.Point(34, 38);
            this.lb_username.Name = "lb_username";
            this.lb_username.Size = new System.Drawing.Size(55, 13);
            this.lb_username.TabIndex = 4;
            this.lb_username.Text = "Username";
            // 
            // lb_password
            // 
            this.lb_password.AutoSize = true;
            this.lb_password.Location = new System.Drawing.Point(34, 80);
            this.lb_password.Name = "lb_password";
            this.lb_password.Size = new System.Drawing.Size(53, 13);
            this.lb_password.TabIndex = 5;
            this.lb_password.Text = "Password";
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 220);
            this.Controls.Add(this.lb_password);
            this.Controls.Add(this.lb_username);
            this.Controls.Add(this.tb_log_password);
            this.Controls.Add(this.tb_log_username);
            this.Controls.Add(this.btn_switch_reg);
            this.Controls.Add(this.btn_login);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_login;
        private System.Windows.Forms.Button btn_switch_reg;
        private System.Windows.Forms.TextBox tb_log_username;
        private System.Windows.Forms.TextBox tb_log_password;
        private System.Windows.Forms.Label lb_username;
        private System.Windows.Forms.Label lb_password;
    }
}