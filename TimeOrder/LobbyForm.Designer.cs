namespace TimeOrder
{
    partial class LobbyForm
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
            lblTitle = new Label();
            lblMode = new Label();
            lblPlayer = new Label();
            btnReady = new Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Arial", 11.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(420, 12);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(52, 17);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Lobby";
            // 
            // lblMode
            // 
            lblMode.AutoSize = true;
            lblMode.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            lblMode.Location = new Point(792, 498);
            lblMode.Name = "lblMode";
            lblMode.Size = new Size(36, 15);
            lblMode.TabIndex = 1;
            lblMode.Text = "Mode";
            // 
            // lblPlayer
            // 
            lblPlayer.AutoSize = true;
            lblPlayer.Location = new Point(24, 119);
            lblPlayer.Name = "lblPlayer";
            lblPlayer.Size = new Size(45, 15);
            lblPlayer.TabIndex = 2;
            lblPlayer.Text = "Player1";
            // 
            // btnReady
            // 
            btnReady.Location = new Point(132, 111);
            btnReady.Name = "btnReady";
            btnReady.Size = new Size(75, 23);
            btnReady.TabIndex = 3;
            btnReady.Text = "Ready";
            btnReady.UseVisualStyleBackColor = true;
            btnReady.Click += btnReady_Click;
            // 
            // LobbyForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 195, 96);
            ClientSize = new Size(868, 522);
            Controls.Add(btnReady);
            Controls.Add(lblPlayer);
            Controls.Add(lblMode);
            Controls.Add(lblTitle);
            Name = "LobbyForm";
            Text = "LobbyForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Label lblMode;
        private Label lblPlayer;
        private Button btnReady;
    }
}