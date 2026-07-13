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
        private Card _card1;
        private Card _card2;
        private Card _card3;
        private List<Card> _timelineCards = new List<Card>();

        Image Card;
        Point position = new Point(200, 100);
        Point dragOffset;
        bool dragging;
        Rectangle rectangle;
        int height = 200;
        int width = 100;


        public Gameplay(Card card1, Card card2, Card card3, String username)
        {
            InitializeComponent();

            lblComment.Visible = false;

            _card1 = card1;
            _card2 = card2;
            _card3 = card3;

            DisplaySmallCard(_card1, pnlSlot1);
            DisplaySmallCard(_card2, pnlSlot2);
            DisplaySmallCard(_card3, pnlSlot3);

            Card = Image.FromFile("card.png");

            rectangle = new Rectangle(
                position.X,
                position.Y,
                width,
                height
            );

            pnlPlayer1.Text = username;
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
                try
                {
                    picture.Image = Image.FromFile(card.ImagePath);
                }
                catch
                {
                    MessageBox.Show(
                        "Impossible de charger l'image : "
                        + card.ImagePath
                    );
                }
            }
            else
            {
                MessageBox.Show(
                    "Image introuvable : "
                    + card.ImagePath
                );
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
                    lblDate.Text =
                        card.Date_Birth
                        + " - "
                        + card.Date_Death;
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


        private void AddCardToTimeline(Card card, bool checkOrder = true)
        {
            if (_timelineCards.Contains(card))
            {
                lblComment.Visible = true;
                lblComment.Text =
                    "Cette carte est déjà sur la timeline.";

                return;
            }

            if (checkOrder == true && CheckOrder(card) == false)
            {
                return;
            }

            _timelineCards.Add(card);

            Panel cardPanel = new Panel();
            cardPanel.Width = 81;
            cardPanel.Height = 100;
            cardPanel.BackColor = Color.Bisque;
            cardPanel.BorderStyle = BorderStyle.FixedSingle;

            PictureBox picture = new PictureBox();
            picture.Image = Image.FromFile(card.ImagePath);
            picture.SizeMode = PictureBoxSizeMode.Zoom;
            picture.Dock = DockStyle.Fill;

            cardPanel.Controls.Add(picture);

            int spacing = 95;
            int index = _timelineCards.Count - 1;

            /*
            int startX = pnlTimelineLine.Left + 3;

            cardPanel.Left = startX + index * spacing;
            cardPanel.Top =
                pnlTimelineLine.Top
                + pnlTimelineLine.Height / 2
                - cardPanel.Height / 2;
            */

            this.Controls.Add(cardPanel);
            cardPanel.BringToFront();
        }


        public bool CheckOrder(Card card)
        {
            int cardYear = int.Parse(card.Date_Birth);

            for (int i = 0; i < _timelineCards.Count; i++)
            {
                int timelineYear =
                    int.Parse(_timelineCards[i].Date_Birth);

                if (cardYear < timelineYear)
                {
                    lblComment.Visible = true;
                    lblComment.Text =
                        "La carte: "
                        + card.Name
                        + " est dans le mauvais ordre !";

                    return false;
                }
            }

            return true;
        }


        private void FormMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (rectangle.Contains(e.Location))
                {
                    dragging = true;

                    dragOffset.X = e.X - rectangle.X;
                    dragOffset.Y = e.Y - rectangle.Y;

                    lblComment.Visible = true;
                    lblComment.Text =
                        "Vous avez cliqué sur la carte !";
                }
            }
        }


        private void FormMouseMove(object sender, MouseEventArgs e)
        {
            if (dragging == true)
            {
                position.X = e.X - dragOffset.X;
                position.Y = e.Y - dragOffset.Y;
            }
        }


        private void FormMouseUp(object sender, MouseEventArgs e)
        {
            if (dragging == true)
            {
                dragging = false;

                lblComment.Visible = true;
                lblComment.Text =
                    "Vous avez relâché la carte à "
                    + rectangle.Location.ToString();
            }
        }


        private void FormPaintEvent(object sender, PaintEventArgs e)
        {
            Pen outline;

            if (dragging == true)
            {
                outline = new Pen(Color.Red, 3);
            }
            else
            {
                outline = new Pen(Color.Black, 3);
            }

            e.Graphics.DrawImage(
                Card,
                rectangle.X,
                rectangle.Y,
                rectangle.Width,
                rectangle.Height
            );

            e.Graphics.DrawRectangle(
                outline,
                rectangle
            );

            outline.Dispose();
        }


        private void FormTimerEvent(object sender, EventArgs e)
        {
            rectangle.X = position.X;
            rectangle.Y = position.Y;

            panel2.Invalidate();
        }
    }
}