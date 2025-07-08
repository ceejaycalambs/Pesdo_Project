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
using Microsoft.ReportingServices.Diagnostics.Internal;

namespace Pesdo_Project
{
    public partial class frm_Dashboard : Form
    {
        public frm_Dashboard()
        {
            InitializeComponent();
            LoadTotalJobseekers();
            LoadTotalEmployers();
            LoadTotaVacancy();
        }
        private void LoadTotalJobseekers()
        {
           
            using (SqlConnection con =  connection.GetConnection())
            {
                try
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM tbl_jobseeker";
                    SqlCommand cmd = new SqlCommand(query, con);
                    int count = (int)cmd.ExecuteScalar();
                    lblTotalApplicants.Text = count.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading total jobseekers: " + ex.Message);
                }
            }
        }
        private void LoadTotalEmployers()
        {
           
            using (SqlConnection con = connection.GetConnection())
            {
                try
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM tbl_employers";
                    SqlCommand cmd = new SqlCommand(query, con);
                    int count = (int)cmd.ExecuteScalar();
                    lblTotalEmployers.Text = count.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading total employers: " + ex.Message);
                }
            }
        }
        private void LoadTotaVacancy()
        {

            using (SqlConnection con = connection.GetConnection())
            {
                try
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM tbl_Job_Vacancy";
                    SqlCommand cmd = new SqlCommand(query, con);
                    int count = (int)cmd.ExecuteScalar();
                    lblTotalVacancy.Text = count.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading total employers: " + ex.Message);
                }
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
