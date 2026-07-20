namespace TimeOrder
{
    public partial class Form1 : Form
    {
        private List<Card> _cards;

        public Form1(List<Card> cards)
        {
            InitializeComponent();

            _cards = cards;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            rbServer.Checked = true;
            UpdateIpTextBox();
        }

        private void UpdateIpTextBox()
        {
            if (rbServer.Checked)
            {
                txtIpAddress.Text = "";
                txtIpAddress.ReadOnly = true;
                txtIpAddress.Text = "192.168.001.001";
                btnJoin.Text = "Start";
            }
            else
            {
                txtIpAddress.ReadOnly = false;
                btnJoin.Text = "Join";
                txtIpAddress.Text = "   .   .   .   ";
            }
        }

        private void rbServer_CheckedChanged_1(object sender, EventArgs e)
        {
            UpdateIpTextBox();
        }

        private void rbClient_CheckedChanged(object sender, EventArgs e)
        {
            UpdateIpTextBox();
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string ipAddress = txtIpAddress.Text.Trim().Replace(" ", "");

            if (username == "")
            {
                MessageBox.Show("Please enter a username.");
                return;
            }

            if (rbServer.Checked)
            {
                LobbyForm lobby = new LobbyForm(username, ipAddress, true, _cards);

                this.Hide();
                lobby.FormClosed += (s, args) => this.Close();
                lobby.Show();
            }
            else if (rbClient.Checked)
            {
                if (ipAddress == "")
                {
                    MessageBox.Show("Please enter an IP address.");
                    return;
                }

                if (!IsValidIpAddress(ipAddress))
                {
                    MessageBox.Show("Please enter a valid IP address like 192.168.001.001");
                    return;
                }

                LobbyForm lobby = new LobbyForm(username, ipAddress, false, _cards);

                this.Hide();
                lobby.FormClosed += (s, args) => this.Close();
                lobby.Show();
            }
            else
            {
                MessageBox.Show("Please select Server mode or Client mode.");
            }
        }
        public bool IsValidIpAddress(string ipAddress)
        {
            string[] parts = ipAddress.Split('.');

            if (parts.Length != 4)
            {
                return false;
            }

            foreach (string part in parts)
            {
                if (part.Length != 3)
                {
                    return false;
                }

                if (!int.TryParse(part, out int number))
                {
                    return false;
                }

                if (number < 0 || number > 255)
                {
                    return false;
                }
            }

            return true;
        }


    }
}