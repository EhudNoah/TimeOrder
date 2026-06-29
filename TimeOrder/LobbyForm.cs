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
        public LobbyForm(string username, string ip_address, bool isServer)
        {
            InitializeComponent();
            if (isServer)
            {
                MessageBox.Show($"Lobby created successfuly");
            }
            else
            {
                MessageBox.Show($"Welcome to {username}'s Lobby");
            }
        }
    }
}
