namespace TimeOrder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
                txtIpAddress.Enabled = false;
                btnJoin.Text = "Start";
            }
            else
            {
                txtIpAddress.Enabled = true;
                btnJoin.Text = "Join";
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
            string ipAddress = txtIpAddress.Text.Trim();

            if (username == "")
            {
                MessageBox.Show("Please enter a username.");
                return;
            }

            if (rbServer.Checked)
            {
                MessageBox.Show("Server mode selected\nUsername: " + username);
            }
            else if (rbClient.Checked)
            {
                if (ipAddress == "")
                {
                    MessageBox.Show("Please enter an IP address.");
                    return;
                }

                MessageBox.Show("Client mode selected\nUsername: " + username + "\nIP: " + ipAddress);
            }
            else
            {
                MessageBox.Show("Please select Server mode or Client mode.");
            }
        }

        
    }
}