namespace Client
{
    partial class Register
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
            this.btn_register = new System.Windows.Forms.Button();
            this.btn_switch_login = new System.Windows.Forms.Button();
            this.tb_username = new System.Windows.Forms.TextBox();
            this.tb_password = new System.Windows.Forms.TextBox();
            this.lb_username = new System.Windows.Forms.Label();
            this.lb_password = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_register
            // 
            this.btn_register.Location = new System.Drawing.Point(156, 167);
            this.btn_register.Name = "btn_register";
            this.btn_register.Size = new System.Drawing.Size(108, 47);
            this.btn_register.TabIndex = 0;
            this.btn_register.Text = "Registrati";
            this.btn_register.UseVisualStyleBackColor = true;
            this.btn_register.Click += new System.EventHandler(this.btn_register_Click);
            // 
            // btn_switch_login
            // 
            this.btn_switch_login.Location = new System.Drawing.Point(328, 223);
            this.btn_switch_login.Name = "btn_switch_login";
            this.btn_switch_login.Size = new System.Drawing.Size(75, 23);
            this.btn_switch_login.TabIndex = 1;
            this.btn_switch_login.Text = "login";
            this.btn_switch_login.UseVisualStyleBackColor = true;
            this.btn_switch_login.Click += new System.EventHandler(this.btn_switch_login_Click);
            // 
            // tb_username
            // 
            this.tb_username.Location = new System.Drawing.Point(132, 27);
            this.tb_username.Name = "tb_username";
            this.tb_username.Size = new System.Drawing.Size(247, 20);
            this.tb_username.TabIndex = 2;
            // 
            // tb_password
            // 
            this.tb_password.Location = new System.Drawing.Point(132, 94);
            this.tb_password.Name = "tb_password";
            this.tb_password.Size = new System.Drawing.Size(247, 20);
            this.tb_password.TabIndex = 3;
            // 
            // lb_username
            // 
            this.lb_username.AutoSize = true;
            this.lb_username.Location = new System.Drawing.Point(30, 30);
            this.lb_username.Name = "lb_username";
            this.lb_username.Size = new System.Drawing.Size(55, 13);
            this.lb_username.TabIndex = 4;
            this.lb_username.Text = "Username";
            // 
            // lb_password
            // 
            this.lb_password.AutoSize = true;
            this.lb_password.Location = new System.Drawing.Point(30, 97);
            this.lb_password.Name = "lb_password";
            this.lb_password.Size = new System.Drawing.Size(53, 13);
            this.lb_password.TabIndex = 5;
            this.lb_password.Text = "Password";
            // 
            // Register
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 255);
            this.Controls.Add(this.lb_password);
            this.Controls.Add(this.lb_username);
            this.Controls.Add(this.tb_password);
            this.Controls.Add(this.tb_username);
            this.Controls.Add(this.btn_switch_login);
            this.Controls.Add(this.btn_register);
            this.Name = "Register";
            this.Text = "Register";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_register;
        private System.Windows.Forms.Button btn_switch_login;
        private System.Windows.Forms.TextBox tb_username;
        private System.Windows.Forms.TextBox tb_password;
        private System.Windows.Forms.Label lb_username;
        private System.Windows.Forms.Label lb_password;
    }
}