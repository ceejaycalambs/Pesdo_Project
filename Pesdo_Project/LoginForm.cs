using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pesdo_Project
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            this.AcceptButton = btnLogin;

            // Optional: Start hidden
            lblMessage.Text = "";
            lblMessage.Visible = false;

            // Timer setup
            messageTimer.Interval = 1500; // 1.5 seconds
            messageTimer.Tick += messageTimer_Tick;
        }

        private void messageTimer_Tick(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            lblMessage.Visible = false;
            messageTimer.Stop();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            using (SqlConnection conn = connection.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM tbl_Login WHERE username = @username AND password = @password";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        // Login successful
                        frm_main mainForm = new frm_main();
                        this.Hide();
                        mainForm.Show();
                    }
                    else
                    {
                        // Login failed
                        lblMessage.Text = "Invalid username or password.";
                        lblMessage.ForeColor = Color.Red;
                        lblMessage.Visible = true;

                        // Always restart the timer
                        if (messageTimer.Enabled)
                            messageTimer.Stop();
                        messageTimer.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error connecting to database:\n" + ex.Message);
                }
            }
        }
    }
}
