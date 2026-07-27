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
            _game = await _client.Shake() ?? new();

            _game.Players.Add(new Player(_username));

            var g = _client.Send(_game);

            _timer.Start();
        }

        private async void _timer_Tick(object? sender, EventArgs e)
        {
            _timer.Stop();

            try
            {
                var game = await _client.Shake();
               
                if (game != null)
                {
                    _game = game;
                    _updateUI();
                    if (!_is_Server
                        && _game.IsStarted)
                    {
                        btnStart_Click(this, EventArgs.Empty);
                    }
                }
            }
            finally
            {
                _timer.Start();
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
                p.Status = Status.Wainting;
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

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_game.Deck.Count < 12)
            {
                MessageBox.Show("Il faut au moins 12 cartes pour distribuer 3 cartes à 4 joueurs.");
                return;
            }

            Gameplay gameplay = new Gameplay(_game, _username);

            this.Hide();

            gameplay.FormClosed += (s, args) => this.Close();

            gameplay.Show();

            if (_is_Server)
            {
                _game.IsStarted = true;
                _game.ShareCards();

                _ = _client.Send(_game);
            }
        }
    }
}
