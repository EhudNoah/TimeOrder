using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        public LobbyForm(string username, string ip_address, bool isServer)
        {
            InitializeComponent();
            _username = username;
            _ip_address = ip_address;
            _is_Server = isServer;
            btnReady.Text = "Ready";
            lblPlayer.Text = _username;
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
            }
        }

        private void btnReady_Click(object sender, EventArgs e)
        {
            if (btnReady.Text == "Ready")
            {
                btnReady.Text = "Unready";
                btnReady.Enabled = true;
            }
            else
            {
                btnReady.Text = "Ready";
                btnReady.Enabled = true;
            }
            }

        private void btnReady_Check(object sender, EventArgs e)
        {

        }

    }
}
