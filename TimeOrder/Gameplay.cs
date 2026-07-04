using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TimeOrder
{
    public partial class Gameplay : Form
    {
        private Card _card;

        public Gameplay(Card card)
        {
            InitializeComponent();
            _card = card;
            DisplaySmallCard(_card, pnlSlot1);
        }

        public void DisplayCard(Card card)
        {
            Panel panelCard = new Panel();
            panelCard.Size = new Size(250, 400);
            panelCard.Location = new Point(275, 50);
            panelCard.BackColor = Color.White;
            panelCard.BorderStyle = BorderStyle.FixedSingle;

            PictureBox picture = new PictureBox();
            picture.Size = new Size(200, 150);
            picture.Location = new Point(25, 20);
            picture.SizeMode = PictureBoxSizeMode.StretchImage;

            if (File.Exists(card.ImagePath))
            {
                picture.Image = Image.FromFile(card.ImagePath);
            }
            else
            {
                MessageBox.Show("Image introuvable : " + card.ImagePath);
            }

            Label lblName = new Label();
            lblName.Text = card.Name;
            lblName.Location = new Point(25, 185);
            lblName.Size = new Size(200, 25);
            lblName.Font = new Font("Arial", 12, FontStyle.Bold);
            lblName.TextAlign = ContentAlignment.MiddleCenter;

            Label lblType = new Label();
            lblType.Text = card.Type;
            lblType.Location = new Point(25, 215);
            lblType.Size = new Size(200, 25);
            lblType.TextAlign = ContentAlignment.MiddleCenter;

            Label lblDate = new Label();
            lblDate.Location = new Point(25, 245);
            lblDate.Size = new Size(200, 25);
            lblDate.TextAlign = ContentAlignment.MiddleCenter;
            lblDate.Font = new Font("Arial", 10, FontStyle.Bold);

            switch (card.IsDateVisible)
            {
                case true:
                    lblDate.Text = card.Date_Birth + " - " + card.Date_Death;
                    break;

                case false:
                    lblDate.Text = "???? - ????";
                    break;
            }

            Label lblDescription = new Label();
            lblDescription.Text = card.Description;
            lblDescription.Location = new Point(25, 285);
            lblDescription.Size = new Size(200, 90);
            lblDescription.TextAlign = ContentAlignment.TopLeft;

            panelCard.Controls.Add(picture);
            panelCard.Controls.Add(lblName);
            panelCard.Controls.Add(lblType);
            panelCard.Controls.Add(lblDate);
            panelCard.Controls.Add(lblDescription);

            this.Controls.Add(panelCard);
        }
        public void DisplaySmallCard(Card card, Panel slot)
        {
            slot.Controls.Clear();

            Panel panelCard = new Panel();
            panelCard.Dock = DockStyle.Fill;
            panelCard.BackColor = Color.White;
            panelCard.BorderStyle = BorderStyle.FixedSingle;

            PictureBox picture = new PictureBox();
            picture.Size = new Size(slot.Width - 20, 45);
            picture.Location = new Point(10, 5);
            picture.SizeMode = PictureBoxSizeMode.Zoom;

            if (File.Exists(card.ImagePath))
            {
                picture.Image = Image.FromFile(card.ImagePath);
            }
            else
            {
                MessageBox.Show("Image introuvable : " + card.ImagePath);
            }

            Label lblName = new Label();
            lblName.Text = card.Name;
            lblName.Location = new Point(5, 52);
            lblName.Size = new Size(slot.Width - 10, 20);
            lblName.Font = new Font("Arial", 8, FontStyle.Bold);
            lblName.TextAlign = ContentAlignment.MiddleCenter;

            Label lblDate = new Label();
            lblDate.Location = new Point(5, 72);
            lblDate.Size = new Size(slot.Width - 10, 18);
            lblDate.Font = new Font("Arial", 8, FontStyle.Regular);
            lblDate.TextAlign = ContentAlignment.MiddleCenter;

            switch (card.IsDateVisible)
            {
                case true:
                    lblDate.Text = card.Date_Birth + " - " + card.Date_Death;
                    break;

                case false:
                    lblDate.Text = "???? - ????";
                    break;
            }

            panelCard.Controls.Add(picture);
            panelCard.Controls.Add(lblName);
            panelCard.Controls.Add(lblDate);

            slot.Controls.Add(panelCard);
        }
    }
}
