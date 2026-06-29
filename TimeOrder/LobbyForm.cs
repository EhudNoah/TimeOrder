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

namespace TimeOrder
{
    public partial class LobbyForm : Form
    {
        private string _username;
        private string _ip_address;
        private bool _is_Server;
        private Dictionary<string, bool> _playersReady = new Dictionary<string, bool>();
        public LobbyForm(string username, string ip_address, bool isServer)
        {
            InitializeComponent();
            _username = username;
            _ip_address = ip_address;
            _is_Server = isServer;
            btnReady.Text = "Ready";
            lblPlayer.Text = "Player 1 : " + _username;
            _playersReady.Add(_username, false);
            btnStart.Visible = false;
            chkReady.Checked = false;


            if (isServer)
            {
                MessageBox.Show($"Lobby created successfuly");
                lblTitle.Text = _username + "'s Lobby";
                lblMode.Text = "Server Mode";
            }
            else
            {
                MessageBox.Show($"Welcome to {username}'s Lobby");
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

    }
}
