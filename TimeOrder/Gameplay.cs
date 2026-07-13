using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TimeOrder
{
    public partial class Gameplay : Form
    {
        private Card _card1;
        private Card _card2;
        private Card _card3;

        private List<Card> _timelineCards = new List<Card>();

        private Panel selectedCardPanel;
        private Card selectedCard;

        private Point dragOffset;

        private bool dragging;


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


            AddCardToTimeline(selectedCard);
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