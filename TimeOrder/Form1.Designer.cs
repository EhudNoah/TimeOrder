namespace TimeOrder
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelMain = new Panel();
            btnJoin = new Button();
            rbClient = new RadioButton();
            rbServer = new RadioButton();
            txtIpAddress = new TextBox();
            lblIpAddress = new Label();
            txtUsername = new TextBox();
            lblUsername = new Label();
            lblTitle = new Label();
            panelMain.SuspendLayout();
            SuspendLayout();
            // 
            // panelMain
            // 
            panelMain.BackColor = Color.FromArgb(248, 195, 96);
            panelMain.Controls.Add(btnJoin);
            panelMain.Controls.Add(rbClient);
            panelMain.Controls.Add(rbServer);
            panelMain.Controls.Add(txtIpAddress);
            panelMain.Controls.Add(lblIpAddress);
            panelMain.Controls.Add(txtUsername);
            panelMain.Controls.Add(lblUsername);
            panelMain.Controls.Add(lblTitle);
            panelMain.Location = new Point(12, 8);
            panelMain.Name = "panelMain";
            panelMain.Size = new Size(860, 541);
            panelMain.TabIndex = 0;
            // 
            // btnJoin
            // 
            btnJoin.Location = new Point(405, 365);
            btnJoin.Name = "btnJoin";
            btnJoin.Size = new Size(120, 30);
            btnJoin.TabIndex = 7;
            btnJoin.Text = "Join";
            btnJoin.UseVisualStyleBackColor = true;
            btnJoin.Click += btnJoin_Click;
            // 
            // rbClient
            // 
            rbClient.AutoSize = true;
            rbClient.Location = new Point(470, 320);
            rbClient.Name = "rbClient";
            rbClient.Size = new Size(90, 19);
            rbClient.TabIndex = 6;
            rbClient.TabStop = true;
            rbClient.Text = "Client Mode";
            rbClient.UseVisualStyleBackColor = true;
            rbClient.CheckedChanged += rbClient_CheckedChanged;
            // 
            // rbServer
            // 
            rbServer.AutoSize = true;
            rbServer.Location = new Point(285, 320);
            rbServer.Name = "rbServer";
            rbServer.Size = new Size(91, 19);
            rbServer.TabIndex = 5;
            rbServer.TabStop = true;
            rbServer.Text = "Server mode";
            rbServer.UseVisualStyleBackColor = true;
            rbServer.CheckedChanged += rbServer_CheckedChanged_1;
            // 
            // txtIpAddress
            // 
            txtIpAddress.Location = new Point(386, 260);
            txtIpAddress.Name = "txtIpAddress";
            txtIpAddress.Size = new Size(198, 23);
            txtIpAddress.TabIndex = 4;
            // 
            // lblIpAddress
            // 
            lblIpAddress.AutoSize = true;
            lblIpAddress.Location = new Point(273, 265);
            lblIpAddress.Name = "lblIpAddress";
            lblIpAddress.Size = new Size(68, 15);
            lblIpAddress.TabIndex = 3;
            lblIpAddress.Text = "IP Address :";
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(386, 200);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(198, 23);
            txtUsername.TabIndex = 2;
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(273, 205);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(66, 15);
            lblUsername.TabIndex = 1;
            lblUsername.Text = "Username :";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(25, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(64, 15);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "TimeOrder";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(884, 561);
            Controls.Add(panelMain);
            Name = "Form1";
            Text = "TimeOrder";
            Load += Form1_Load;
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelMain;
        private TextBox txtIpAddress;
        private Label lblIpAddress;
        private TextBox txtUsername;
        private Label lblUsername;
        private Label lblTitle;
        private Button btnJoin;
        private RadioButton rbClient;
        private RadioButton rbServer;
    }
}
