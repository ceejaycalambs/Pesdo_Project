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
    public partial class frm_JobMatching : Form
    {
        private string vacancyID;
        public frm_JobMatching(string id)
        {
            InitializeComponent();
            this.vacancyID = id;
            LoadJobVacancy();
        }
        public frm_JobMatching()
        {
            InitializeComponent();
            LoadJobVacancy(); // If you still want to load the list even without an ID
        }
        public void LoadJobVacancy()
        {
            try
            {
                using (SqlConnection conn = connection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM tbl_Job_Vacancy ORDER BY DateAdded DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dataGridView1.DataSource = dt;
                            dataGridView1.DataSource = dt;
                            dataGridView1.Columns["Employer_Name"].HeaderText = "Employer Name";
                            dataGridView1.Columns["Location"].HeaderText = "Location";
                            dataGridView1.Columns["Job_Type"].HeaderText = "Job Type";
                            dataGridView1.Columns["Job_Title"].HeaderText = "Job Title";

                            dataGridView1.Columns["Job_Description"].HeaderText = "Job Description";
                            dataGridView1.Columns["coljobMatch"].DisplayIndex = dataGridView1.Columns.Count - 1;
                       
                            dataGridView1.Columns["Id"].Visible = false;
                            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load job vacancies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Make sure the clicked column is 'coljobMatch'
                if (dataGridView1.Columns[e.ColumnIndex].Name == "coljobMatch")
                {
                    // Get ID from the row, change "Id" to your actual column name
                    string VacancyId = dataGridView1.Rows[e.RowIndex].Cells["Id"].Value.ToString();

                    // Open the JobMatch form
                    frm_JobMatch jobMatchForm = new frm_JobMatch(VacancyId);
                    jobMatchForm.ShowDialog();
                }
            }
        }
    }
}
