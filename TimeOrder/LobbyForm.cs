using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TimeOrder
{
    public partial class LobbyForm : Form
    {
        public string _username { get; private set; }
        private Deck _deck;     
        private string _ip_address;
        private bool _is_Server;
        private Dictionary<string, bool> _playersReady = new Dictionary<string, bool>();
        private List<Card> _cards;
        public LobbyForm(string username, string ip_address, bool isServer, Deck deck)
        {
            InitializeComponent();

            _username = username;
            _ip_address = ip_address;
            _is_Server = isServer;
            _deck = deck;

            btnReady.Text = "Ready";
            lblPlayer.Text = "Player 1 : " + _username;
            _playersReady.Add(_username, false);
            btnStart.Visible = false;
            chkReady.Checked = false;

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
        }

        private void btnReady_Click(object sender, EventArgs e)
        {
            if (btnReady.Text == "Ready")
            {
                btnReady.Text = "Unready";
                btnReady.Enabled = true;
                _playersReady[_username] = true;
                chkReady.Checked = true;

            }
            else
            {
                btnReady.Text = "Ready";
                btnReady.Enabled = true;
                _playersReady[_username] = false;
                chkReady.Checked = false;
            }
            UpdateStartButton();
        }


        private void UpdateStartButton()
        {
            bool allPlayersReady = _playersReady.All(player => player.Value == true);

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
            if (_deck.Count < 12)
            {
                MessageBox.Show("Il faut au moins 12 cartes pour distribuer 3 cartes à 4 joueurs.");
                return;
            }

            Gameplay gameplay = new Gameplay(_deck, _username);

            this.Hide();

            gameplay.FormClosed += (s, args) => this.Close();

            gameplay.Show();
        }
    }
}
