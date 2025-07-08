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
    public partial class frm_jobvacancy : Form
    {
        public frm_jobvacancy()
        {
            InitializeComponent();
            LoadJobVacancy();
        }

        private void btnAddjobVacancy_Click(object sender, EventArgs e)
        {
            frm_AddJobVacancy frm_AddJobVacancy = new frm_AddJobVacancy(this);
            frm_AddJobVacancy.ShowDialog();
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
                            dataGridView1.Columns["Vacancy_Count"].HeaderText = "Vacancy Count";
                            dataGridView1.Columns["Job_Description"].HeaderText = "Job Description";

                            dataGridView1.Columns["colView"].DisplayIndex = dataGridView1.Columns.Count - 1;
                            dataGridView1.Columns["colDelete"].DisplayIndex = dataGridView1.Columns.Count - 1;
                            dataGridView1.Columns["colUpdate"].DisplayIndex = dataGridView1.Columns.Count - 1;
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
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "colView")
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Id"].Value);
                frm_AddJobVacancy viewForm = new frm_AddJobVacancy(id, this, true); // true = viewOnly
                viewForm.ShowDialog();
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "ColDelete")
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Id"].Value);
                frm_AddJobVacancy deleteForm = new frm_AddJobVacancy(id, this, isViewOnly: false, isDeleteMode: true);
                deleteForm.ShowDialog();
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "colUpdate")
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Id"].Value);
                frm_AddJobVacancy updateForm = new frm_AddJobVacancy(id, this, isViewOnly: false, isDeleteMode: false, isUpdateMode: true);
                updateForm.ShowDialog();
            }
            
        }
    }
}
