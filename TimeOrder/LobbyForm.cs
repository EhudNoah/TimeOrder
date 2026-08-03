using Interop_Test.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Quic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Timer = System.Windows.Forms.Timer;

namespace TimeOrder
{
    public partial class LobbyForm : Form
    {
        public string _username { get; private set; }
        private bool _gameplayOpened;
        private Game _game;
        private string _ip_address;
        private ushort _port;
        private bool _is_Server;
        private List<Card> _cards;

        private Server<Game>? _server;
        private Client<Game> _client;
        private Timer _timer;

        public LobbyForm(string username, string ip_address, ushort port, bool isServer)
        {
            InitializeComponent();

            Text += " - " + username;

            _client = new Client<Game>(ip_address, port);

            if (isServer)
            {
                _server = new Server<Game>(ip_address, port);
                _server.Run();
            }

            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += _timer_Tick;

            _username = username;
            _ip_address = ip_address;
            _port = port;
            _is_Server = isServer;

            btnReady.Text = "Ready";
            btnStart.Visible = false;

            _initGame();

            if (isServer)
            {
                MessageBox.Show("Lobby created successfully");
                lblTitle.Text = _username + "'s Lobby";
                lblMode.Text = "Server Mode";
            }
            else
            {
                MessageBox.Show("Welcome to " + username + "'s Lobby");
                lblTitle.Text = _username + "'s Lobby";
                lblMode.Text = "Client Mode";
                lblIpLobby.Text = "Ip Address: " + _ip_address.Trim();
            }

            btnReady.Click += btnReady_Click;
        }

        private async void _initGame()
        {
            _game = await _client.Shake() ?? new Game();

            _game.Players.Add(new Player(_username));

            await _client.Send(_game);

            _timer.Start();
        }

        private async void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();

            try
            {
                Game game = await _client.Shake();

                if (game != null)
                {
                    _game = game;

                    _updateUI();

                    if (_is_Server == false && _game.IsStarted)
                    {
                        OpenGameplay();
                    }
                }
            }
            finally
            {
                if (_gameplayOpened == false)
                {
                    _timer.Start();
                }
            }
        }

        private void btnReady_Click(object sender, EventArgs e)
        {
            if (btnReady.Text == "Ready")
            {
                btnReady.Text = "Unready";
                btnReady.Enabled = true;
                Player p = _game.Players.First(player => player.Name == _username);
                p.Status = Status.Active;
            }
            else
            {
                btnReady.Text = "Ready";
                btnReady.Enabled = true;
                Player p = _game.Players.First(player => player.Name == _username);
                p.Status = Status.Waiting;
            }

            _ = _client.Send(_game);
        }

        private void _updateUI()
        {
            playerList.Items.Clear();

            foreach (var player in _game.Players)
            {
                playerList.Items.Add(player.Name, player.Status == Status.Active);
            }

            UpdateStartButton();
        }


        private void UpdateStartButton()
        {
            bool allPlayersReady = _game.Players.All(player => player.Status == Status.Active);

            if (_is_Server && allPlayersReady)
            {
                btnStart.Visible = true;
            }
            else
            {
                btnStart.Visible = false;
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (_is_Server == false)
            {
                return;
            }

            int cardsPerPlayer = 3;
            int requiredCards = _game.Players.Count * cardsPerPlayer;

            if (_game.Deck.Count < requiredCards)
            {
                MessageBox.Show(
                    "Il faut au moins "
                    + requiredCards
                    + " cartes pour distribuer "
                    + cardsPerPlayer
                    + " cartes à "
                    + _game.Players.Count
                    + " joueurs."
                );

                return;
            }

            _timer.Stop();

            _game.ShareCards(cardsPerPlayer);
            _game.Players.ForEach(player => player.Status = Status.Waiting);
            _game.Players.First(player => player.Name == _username).Status = Status.Active;

            _game.IsStarted = true;

            await _client.Send(_game);

            OpenGameplay();
        }

        private void OpenGameplay()
        {
            if (_gameplayOpened == true)
            {
                return;
            }

            _gameplayOpened = true;

            Gameplay gameplay = new Gameplay(_game, _username, _server, _client);

            this.Hide();

            gameplay.FormClosed += (sender, e) => this.Close();

            gameplay.Show();
        }
    }
}
