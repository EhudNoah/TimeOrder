namespace TimeOrder
{
    partial class Gameplay
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
            pnlBottom = new Panel();
            pnlSlot1 = new Panel();
            pnlBottom.SuspendLayout();
            SuspendLayout();
            // 
            // pnlBottom
            // 
            pnlBottom.BackColor = Color.Silver;
            pnlBottom.Controls.Add(pnlSlot1);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 383);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new Size(868, 139);
            pnlBottom.TabIndex = 0;
            // 
            // pnlSlot1
            // 
            pnlSlot1.BackColor = Color.FromArgb(255, 224, 192);
            pnlSlot1.BorderStyle = BorderStyle.FixedSingle;
            pnlSlot1.Location = new Point(60, 15);
            pnlSlot1.Name = "pnlSlot1";
            pnlSlot1.Size = new Size(180, 100);
            pnlSlot1.TabIndex = 0;
            // 
            // Gameplay
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(868, 522);
            Controls.Add(pnlBottom);
            Name = "Gameplay";
            Text = "Gameplay";
            pnlBottom.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlBottom;
        private Panel pnlSlot1;
    }
}