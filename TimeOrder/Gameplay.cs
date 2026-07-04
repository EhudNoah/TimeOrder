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

namespace TimeOrder
{
    public partial class Gameplay : Form
    {
        private Card _card;

        public Gameplay(Card card)
        {
            InitializeComponent();
            _card = card;
            DisplayCard(_card);
        }

        public void DisplayCard(Card card)
        {
            Panel panelCard = new Panel();
            panelCard.Size = new Size(250, 350);
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

            Label lblDescription = new Label();
            lblDescription.Text = card.Description;
            lblDescription.Location = new Point(25, 250);
            lblDescription.Size = new Size(200, 70);

            panelCard.Controls.Add(picture);
            panelCard.Controls.Add(lblName);
            panelCard.Controls.Add(lblType);
            panelCard.Controls.Add(lblDescription);

            this.Controls.Add(panelCard);
        }
    }
}
