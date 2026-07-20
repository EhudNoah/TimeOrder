using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TimeOrder
{
    public partial class Gameplay : Form
    {
        private List<Card> _cards;


        private List<Card> _timelineCards = new List<Card>();

        private Panel selectedCardPanel;
        private Card selectedCard;

        private Point dragOffset;

        private bool dragging;

        private PictureBox pictureCardPreview;


        public Gameplay(List<Card> cards, String username)
        {
            InitializeComponent();

            pictureCardPreview = new PictureBox();

            pictureCardPreview.Size = new Size(80, 70);
            pictureCardPreview.SizeMode = PictureBoxSizeMode.Zoom;
            pictureCardPreview.BorderStyle = BorderStyle.FixedSingle;
            pictureCardPreview.BackColor = Color.White;
            pictureCardPreview.Visible = false;

            this.Controls.Add(pictureCardPreview);

            lblComment.Visible = false;

            _cards = cards;

            if (_cards.Count < 3)
            {
                MessageBox.Show("Il faut au moins 3 cartes dans le fichier JSON.");
                return;
            }

            DisplaySmallCard(_cards[0], pnlSlot1);
            DisplaySmallCard(_cards[1], pnlSlot2);
            DisplaySmallCard(_cards[2], pnlSlot3);

            pnlPlayer1.Text = username;
        }


        public void DisplaySmallCard(Card card, Panel slot)
        {
            Panel panelCard = new Panel();

            panelCard.Dock = DockStyle.Fill;
            panelCard.BackColor = Color.White;
            panelCard.BorderStyle = BorderStyle.FixedSingle;
            panelCard.Tag = card;


            PictureBox picture = new PictureBox();

            picture.Size = new Size(
                slot.Width - 20,
                45
            );

            picture.Location = new Point(10, 5);
            picture.SizeMode = PictureBoxSizeMode.Zoom;


            if (File.Exists(card.ImagePath))
            {
                try
                {
                    picture.Image = Image.FromFile(
                        card.ImagePath
                    );
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

            lblName.Size = new Size(
                slot.Width - 10,
                20
            );

            lblName.Font = new Font(
                "Arial",
                8,
                FontStyle.Bold
            );

            lblName.TextAlign = ContentAlignment.MiddleCenter;


            Label lblDate = new Label();

            lblDate.Location = new Point(5, 72);

            lblDate.Size = new Size(
                slot.Width - 10,
                18
            );

            lblDate.Font = new Font(
                "Arial",
                8,
                FontStyle.Regular
            );

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


            AddDragEvents(panelCard);
            AddDragEvents(picture);
            AddDragEvents(lblName);
            AddDragEvents(lblDate);


            slot.Controls.Add(panelCard);
        }


        private void AddDragEvents(Control control)
        {
            control.MouseDown += CardMouseDown;
            control.MouseMove += CardMouseMove;
            control.MouseUp += CardMouseUp;
        }


        private void CardMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }


            Control clickedControl = sender as Control;

            if (clickedControl == null)
            {
                return;
            }


            if (clickedControl is Panel)
            {
                selectedCardPanel = clickedControl as Panel;
            }
            else
            {
                selectedCardPanel = clickedControl.Parent as Panel;
            }


            if (selectedCardPanel == null)
            {
                return;
            }


            selectedCard = selectedCardPanel.Tag as Card;

            if (selectedCard == null)
            {
                return;
            }


            Point cardPosition = this.PointToClient(
                selectedCardPanel.PointToScreen(
                    Point.Empty
                )
            );


            Size cardSize = selectedCardPanel.Size;


            selectedCardPanel.Dock = DockStyle.None;

            selectedCardPanel.Parent = this;

            selectedCardPanel.Size = cardSize;

            selectedCardPanel.Location = cardPosition;

            selectedCardPanel.BringToFront();


            selectedCardPanel.Capture = true;


            Point mousePosition = this.PointToClient(
                Cursor.Position
            );


            dragOffset.X =
                mousePosition.X
                - selectedCardPanel.Left;

            dragOffset.Y =
                mousePosition.Y
                - selectedCardPanel.Top;


            dragging = true;


            lblComment.Visible = true;

            lblComment.Text =
                "Vous déplacez la carte : "
                + selectedCard.Name;

            lblComment.BringToFront();
        }


        private void CardMouseMove(object sender, MouseEventArgs e)
        {
            if (dragging == false)
            {
                return;
            }


            if (selectedCardPanel == null)
            {
                return;
            }


            Point mousePosition = this.PointToClient(
                Cursor.Position
            );


            selectedCardPanel.Left =
                mousePosition.X
                - dragOffset.X;

            selectedCardPanel.Top =
                mousePosition.Y
                - dragOffset.Y;


            lblComment.Visible = true;

            lblComment.Text =
                "Carte : "
                + selectedCard.Name
                + " | X : "
                + selectedCardPanel.Left
                + " | Y : "
                + selectedCardPanel.Top;

            lblComment.BringToFront();
        }


        private void CardMouseUp(object sender, MouseEventArgs e)
        {
            if (dragging == false)
            {
                return;
            }


            dragging = false;


            if (selectedCardPanel != null)
            {
                selectedCardPanel.Capture = false;
            }


            if (selectedCard == null || selectedCardPanel == null)
            {
                return;
            }


            lblComment.Visible = true;

            lblComment.Text =
                "Carte déposée : "
                + selectedCard.Name
                + " | X : "
                + selectedCardPanel.Left
                + " | Y : "
                + selectedCardPanel.Top;

            lblComment.BringToFront();

            // calculer x
            //  1955 - 1596 = 359
            int firstYear = 1596;
            int lastYear = 1955;

            int birthYear = int.Parse(selectedCard.Date_Birth);
            int deathYear = int.Parse(selectedCard.Date_Death);

            int totalYears = lastYear - firstYear;
            int lifeDuration = deathYear - birthYear;

            float pixelsPerYear = (float)panel2.Width / totalYears;

            int textBoxLeft = (int)((birthYear - firstYear) * pixelsPerYear);
            int textBoxWidth = (int)(lifeDuration * pixelsPerYear);

            if (textBoxWidth < 40)
            {
                textBoxWidth = 40;
            }

            TextBox card = new TextBox();

            card.Text = selectedCard.Name;
            card.Size = new Size(textBoxWidth, 20);
            card.Left = textBoxLeft;
            card.Top = selectedCardPanel.Top;
            card.ReadOnly = true;
            card.TextAlign = HorizontalAlignment.Center;

            card.Tag = selectedCard;

            card.MouseEnter += TextBoxCardMouseEnter;
            card.MouseLeave += TextBoxCardMouseLeave;

            panel2.Controls.Add(card);

            selectedCardPanel.Visible = false;

            AddCardToTimeline(selectedCard);
        }


        private void TextBoxCardMouseEnter(object sender, EventArgs e)
        {
            TextBox textBoxCard = sender as TextBox;

            if (textBoxCard == null)
            {
                return;
            }

            Card card = textBoxCard.Tag as Card;

            if (card == null)
            {
                return;
            }

            if (File.Exists(card.ImagePath) == false)
            {
                return;
            }

            if (pictureCardPreview.Image != null)
            {
                pictureCardPreview.Image.Dispose();
                pictureCardPreview.Image = null;
            }

            try
            {
                using (Image image = Image.FromFile(card.ImagePath))
                {
                    pictureCardPreview.Image = new Bitmap(image);
                }
            }
            catch
            {
                return;
            }

            Point textBoxPosition = this.PointToClient(
                textBoxCard.PointToScreen(
                    Point.Empty
                )
            );

            pictureCardPreview.Left =
                textBoxPosition.X
                + textBoxCard.Width
                + 10;

            pictureCardPreview.Top =
                selectedCardPanel.Top;

            if (pictureCardPreview.Top < 0)
            {
                pictureCardPreview.Top =
                    selectedCardPanel.Top;
            }

            if (pictureCardPreview.Right > this.ClientSize.Width)
            {
                pictureCardPreview.Left =
                    textBoxPosition.X
                    - pictureCardPreview.Width
                    - 10;
            }

            pictureCardPreview.Visible = true;
            pictureCardPreview.BringToFront();
        }


        private void TextBoxCardMouseLeave(object sender, EventArgs e)
        {
            pictureCardPreview.Visible = false;

            if (pictureCardPreview.Image != null)
            {
                pictureCardPreview.Image.Dispose();
                pictureCardPreview.Image = null;
            }
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
        }


        public bool CheckOrder(Card card)
        {
            int cardYear = int.Parse(
                card.Date_Birth
            );


            for (int i = 0; i < _timelineCards.Count; i++)
            {
                int timelineYear = int.Parse(
                    _timelineCards[i].Date_Birth
                );


                if (cardYear < timelineYear)
                {
                    lblComment.Visible = true;

                    lblComment.Text =
                        "La carte : "
                        + card.Name
                        + " est dans le mauvais ordre !";

                    return false;
                }
            }


            return true;
        }
    }
}