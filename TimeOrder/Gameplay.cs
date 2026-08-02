using Interop_Test.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace TimeOrder
{
    public partial class Gameplay : Form
    {
        private Game _game;
        private Player _player;
        Server<Game>? _server;
        Client<Game> _client;
        bool _isServer;
        private List<Card> _timelineCards = new List<Card>();
        private Timer _timer;

        private Panel selectedCardPanel;
        private Card selectedCard;

        private Point dragOffset;

        private bool dragging;

        private PictureBox pictureCardPreview;


        public Gameplay(Game game, string username, Server<Game>? server, Client<Game> client)
        {
            InitializeComponent();
            pnlBottom.Resize += PnlBottomResize;
            panel1.Resize += Panel1Resize;
            Resize += GameplayResize;

            ResizeBottomSlots();
            ResizePlayerLabels();

            pictureCardPreview = new PictureBox();

            pictureCardPreview.Size = new Size(180, 230);
            pictureCardPreview.SizeMode = PictureBoxSizeMode.Zoom;
            pictureCardPreview.BorderStyle = BorderStyle.FixedSingle;
            pictureCardPreview.Visible = false;

            this.Controls.Add(pictureCardPreview);
            pictureCardPreview.BringToFront();

            _game = game;
            _player = _game.Players.First(p => p.Name == username);
            _server = server;
            _client = client;
            _isServer = _server is not null;

            _timer = new Timer();
            _timer.Interval = 500;
            _timer.Tick += _timer_Tick;

            if (_player.Status != Status.Active)
            {
                _timer.Start();
            }

            lblComment.Visible = false;

            //CreatePlayers(username);

            //DealCards(3);

            DisplayCurrentPlayerHand();
            UpdatePlayerLabels();
            PrintDistributedCards();
        }
        private void GameplayResize(object sender, EventArgs e)
        {
            PositionCommentLabel();
        }
        public void DisplayPlayers()
        {
            if (_game.Players.Count >= 1)
            {
                if (_game.Players[0].Status == Status.Active)
                {
                    pnlPlayer1.Text = _game.Players[0].Name + " 🟢";
                }
            }

            if (_game.Players.Count >= 2)
            {
                if (_game.Players[1].Status == Status.Active)
                {
                    pnlPlayer2.Text = _game.Players[1].Name + " 🟢";
                }
            }

            if (_game.Players.Count >= 3)
            {
                if (_game.Players[2].Status == Status.Active)
                {
                    pnlPlayer3.Text = _game.Players[2].Name + " 🟢";
                }
            }

            if (_game.Players.Count >= 4)
            {
                if (_game.Players[3].Status == Status.Active)
                {
                    pnlPlayer4.Text = _game.Players[3].Name + " 🟢";
                }
            }

            ResizePlayerLabels();
        }
        private void PositionCommentLabel()
        {
            int rightMargin = 10;
            int bottomPanelMargin = 10;

            lblComment.Left =
                ClientSize.Width
                - lblComment.Width
                - rightMargin;

            lblComment.Top =
                pnlBottom.Top
                - lblComment.Height
                - bottomPanelMargin;
        }
        private void PnlBottomResize(object sender, EventArgs e)
        {
            ResizeBottomSlots();
        }

        private void ResizeBottomSlots()
        {
            int slotWidth = pnlBottom.ClientSize.Width * 12 / 100;
            int sectionWidth = pnlBottom.ClientSize.Width / 3;
            int topPosition = 9;

            pnlSlot1.Width = slotWidth;
            pnlSlot2.Width = slotWidth;
            pnlSlot3.Width = slotWidth;

            pnlSlot1.Left = (sectionWidth - pnlSlot1.Width) / 2;

            pnlSlot2.Left = sectionWidth
                + (sectionWidth - pnlSlot2.Width) / 2;

            pnlSlot3.Left = sectionWidth * 2
                + (sectionWidth - pnlSlot3.Width) / 2;

            pnlSlot1.Top = topPosition;
            pnlSlot2.Top = topPosition;
            pnlSlot3.Top = topPosition;
        }
        private void Panel1Resize(object sender, EventArgs e)
        {
            ResizePlayerLabels();
        }

        private void ResizePlayerLabels()
        {
            int sectionWidth = panel1.ClientSize.Width / 4;
            int spaceBetweenLabels = 15;
            int topPosition = 12;

            PositionPlayerLabels(
                pnlPlayer1,
                pnlPlayer1Cards,
                0,
                sectionWidth,
                spaceBetweenLabels,
                topPosition
            );

            PositionPlayerLabels(
                pnlPlayer2,
                pnlPlayer2Cards,
                sectionWidth,
                sectionWidth,
                spaceBetweenLabels,
                topPosition
            );

            PositionPlayerLabels(
                pnlPlayer3,
                pnlPlayer3Cards,
                sectionWidth * 2,
                sectionWidth,
                spaceBetweenLabels,
                topPosition
            );

            PositionPlayerLabels(
                pnlPlayer4,
                pnlPlayer4Cards,
                sectionWidth * 3,
                sectionWidth,
                spaceBetweenLabels,
                topPosition
            );
        }

        private void PositionPlayerLabels(Label playerLabel, Label cardsLabel, int sectionLeft, int sectionWidth, int spaceBetweenLabels, int topPosition)
        {
            int totalWidth = playerLabel.Width
                + spaceBetweenLabels
                + cardsLabel.Width;

            int groupLeft = sectionLeft
                + (sectionWidth - totalWidth) / 2;

            playerLabel.Left = groupLeft;
            playerLabel.Top = topPosition;

            cardsLabel.Left = playerLabel.Right + spaceBetweenLabels;
            cardsLabel.Top = topPosition + 2;
        }

        private async void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();

            Game game = await _client.Shake();

            if (game != null)
            {
                _game = game;
                _player = _game.Players.First(p => p.Name == _player.Name);
                _drawGamePlan();
                DisplayCurrentPlayerHand();
                UpdatePlayerLabels();
                DisplayPlayers();
                if (_player.Status == Status.Active)
                {
                    DisplayPlayers();
                    return;
                }
            }

            _timer.Start();
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
            if (_player.Status != Status.Active
                || e.Button != MouseButtons.Left)
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
            PositionCommentLabel();
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
            PositionCommentLabel();
            lblComment.BringToFront();
        }


        private void CardMouseUp(object sender, MouseEventArgs e)
        {
            Point cardLocation = ((Control)sender).Location;
            Point mouseLocation = new Point(cardLocation.X + e.X, cardLocation.Y + e.Y);

            if (!timePlan.DisplayRectangle.Contains(mouseLocation))
            {
                dragging = false;
                return;
            }

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
            PositionCommentLabel();
            lblComment.BringToFront();


            Card playedCard = selectedCard;

            AddCardToTimeline(playedCard);
            _player.RemoveCard(playedCard);
            _game.PlayedCards.Add(playedCard);

            playedCard.Height = (double)selectedCardPanel.Top / timePlan.Height;

            Card drawnCard = _player.DrawCard(_game.Deck);

            Console.WriteLine(_player.Name + " a joué : " + playedCard.Name);

            if (drawnCard != null)
            {
                Console.WriteLine(_player.Name + " a pioché : " + drawnCard.Name);
            }
            else
            {
                Console.WriteLine("La pioche est vide.");
            }

            Console.WriteLine("Cartes restantes dans la pioche : " + _game.Deck.Count);
            Console.WriteLine();

            selectedCardPanel.Dispose();

            selectedCardPanel = null;
            selectedCard = null;

            _drawGamePlan();
            DisplayCurrentPlayerHand();
            UpdatePlayerLabels();

            _player.Status = Status.Waiting;
            int index = (_game.Players.FindIndex(p => p.Name == _player.Name) + 1) % _game.Players.Count;

            _game.Players[index].Status = Status.Active;

            _ = _client.Send(_game);

            _timer.Start();
        }

        private void _drawGamePlan()
        {
            timePlan.Controls.Clear();

            foreach (Card card in _game.PlayedCards)
            {
                // calculer x
                //  1955 - 1596 = 359
                int firstYear = _game.Deck.Min;
                int lastYear = _game.Deck.Max;

                int birthYear = card.Date_Birth;
                int deathYear = card.Date_Death;

                int totalYears = lastYear - firstYear;
                int lifeDuration = deathYear - birthYear;

                float pixelsPerYear = (float)timePlan.Width / totalYears;

                int textBoxLeft = (int)((birthYear - firstYear) * pixelsPerYear);
                int textBoxWidth = (int)(lifeDuration * pixelsPerYear);

                if (textBoxWidth < 40)
                {
                    textBoxWidth = 40;
                }

                TextBox cardTB = new TextBox();

                cardTB.Text = card.Name;
                cardTB.Size = new Size(textBoxWidth, 20);
                cardTB.Left = textBoxLeft;
                cardTB.Top = (int)(card.Height * timePlan.Height);
                cardTB.ReadOnly = true;
                cardTB.TextAlign = HorizontalAlignment.Center;

                cardTB.Tag = card;

                cardTB.MouseEnter += TextBoxCardMouseEnter;
                cardTB.MouseLeave += TextBoxCardMouseLeave;

                timePlan.Controls.Add(cardTB);
            }
        }


        private void TextBoxCardMouseEnter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox == null)
            {
                return;
            }

            Card card = textBox.Tag as Card;

            if (card == null)
            {
                return;
            }

            if (pictureCardPreview == null)
            {
                return;
            }

            if (card.ImagePath == null || card.ImagePath.Trim() == "")
            {
                return;
            }

            if (File.Exists(card.ImagePath) == false)
            {
                Console.WriteLine("Image introuvable : " + card.ImagePath);
                return;
            }

            if (pictureCardPreview.Image != null)
            {
                pictureCardPreview.Image.Dispose();
                pictureCardPreview.Image = null;
            }

            using (Image originalImage = Image.FromFile(card.ImagePath))
            {
                pictureCardPreview.Image = new Bitmap(originalImage);
            }

            Point position = this.PointToClient(Cursor.Position);

            pictureCardPreview.Left = position.X + 15;
            pictureCardPreview.Top = position.Y + 15;

            pictureCardPreview.Visible = true;
            pictureCardPreview.BringToFront();
        }


        private void TextBoxCardMouseLeave(object sender, EventArgs e)
        {
            if (pictureCardPreview == null)
            {
                return;
            }

            pictureCardPreview.Visible = false;
        }


        private void AddCardToTimeline(Card card, bool checkOrder = true)
        {
            if (_timelineCards.Contains(card))
            {
                lblComment.Visible = true;

                lblComment.Text =
                    "Cette carte est déjà sur la timeline.";
                PositionCommentLabel();
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
            int cardYear = card.Date_Birth;


            for (int i = 0; i < _timelineCards.Count; i++)
            {
                int timelineYear = _timelineCards[i].Date_Birth;


                if (cardYear < timelineYear)
                {
                    lblComment.Visible = true;

                    lblComment.Text =
                        "La carte : "
                        + card.Name
                        + " est dans le mauvais ordre !";
                    PositionCommentLabel();
                    return false;
                }
            }


            return true;
        }
        private void PrintDistributedCards()
        {
            Console.WriteLine();
            Console.WriteLine("========== CARTES DISTRIBUÉES ==========");

            for (int i = 0; i < _game.Players.Count; i++)
            {
                Console.Write("Player " + (i + 1) + " (" + _game.Players[i].Name + ") : ");

                for (int j = 0; j < _game.Players[i].Hand.Count; j++)
                {
                    Console.Write(_game.Players[i].Hand[j].Name);

                    if (j < _game.Players[i].Hand.Count - 1)
                    {
                        Console.Write(", ");
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine("Cartes restantes dans la pioche : " + _game.Deck.Count);
            Console.WriteLine("========================================");
            Console.WriteLine();
        }

        public void DisplayCurrentPlayerHand()
        {
            ClearSlot(pnlSlot1);
            ClearSlot(pnlSlot2);
            ClearSlot(pnlSlot3);

            if (_player.Hand.Count > 0)
            {
                DisplaySmallCard(_player.Hand[0], pnlSlot1);
            }

            if (_player.Hand.Count > 1)
            {
                DisplaySmallCard(_player.Hand[1], pnlSlot2);
            }

            if (_player.Hand.Count > 2)
            {
                DisplaySmallCard(_player.Hand[2], pnlSlot3);
            }
        }

        private void ClearSlot(Panel slot)
        {
            while (slot.Controls.Count > 0)
            {
                slot.Controls[0].Dispose();
            }
        }
        private void UpdatePlayerLabels()
        {
            pnlPlayer1.Text = _game.Players[0].Name;
            pnlPlayer1Cards.Text = _game.Players[0].Score + " points |";

            if (_game.Players.Count >= 2)
            {
                pnlPlayer2.Text = _game.Players[1].Name;
                pnlPlayer2Cards.Text = _game.Players[1].Score + " points |";
            }

            if (_game.Players.Count >= 3)
            {
                pnlPlayer3.Text = _game.Players[2].Name;
                pnlPlayer3Cards.Text = _game.Players[2].Score + " points |";
            }

            if (_game.Players.Count >= 4)
            {
                pnlPlayer4.Text = _game.Players[3].Name;
                pnlPlayer4Cards.Text = _game.Players[3].Score + " points";
            }
        }
    }
}