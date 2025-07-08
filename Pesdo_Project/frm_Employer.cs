using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Pesdo_Project
{
    public partial class frm_Employer : Form
    {
        public frm_Employer()
        {
            InitializeComponent();
            LoadRecords();
        }

        private void btnAddEmployer_Click(object sender, EventArgs e)
        {
            frm_addEmployer frm_AddEmployer = new frm_addEmployer(this);
            frm_AddEmployer.ShowDialog();
        }

        public void LoadRecords()
        {
            try
            {
                using (SqlConnection conn = connection.GetConnection())
                {
                    conn.Open();

                    // ✅ Include Employment_Type in your SELECT
                    string query = @"SELECT 
                        Id, 
                        Employer_Name, 
                        Location, 
                        Email, 
                        Contact_no, 
                        Company_Description, 
                        Employment_Type 
                    FROM tbl_employers";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;

                    // ✅ Set header texts
                    dataGridView1.Columns["Employer_Name"].HeaderText = "Employer Name";
                    dataGridView1.Columns["Location"].HeaderText = "Location";
                    dataGridView1.Columns["Email"].HeaderText = "Email";
                    dataGridView1.Columns["Contact_no"].HeaderText = "Contact No.";
                    dataGridView1.Columns["Company_Description"].HeaderText = "Company Description";
                    dataGridView1.Columns["Employment_Type"].HeaderText = "Employment Type";

                    // ✅ Adjust column visibility/order
                    dataGridView1.Columns["Id"].Visible = false;
                    dataGridView1.Columns["colView"].DisplayIndex = dataGridView1.Columns.Count - 1;
                    dataGridView1.Columns["colDelete"].DisplayIndex = dataGridView1.Columns.Count - 1;
                    dataGridView1.Columns["colUpdate"].DisplayIndex = dataGridView1.Columns.Count - 1;

                    // ✅ Optional: auto size
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Loading data: " + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string columnName = dataGridView1.Columns[e.ColumnIndex].Name;
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Id"].Value);

                if (columnName == "colView")
                {
                    frm_addEmployer viewForm = new frm_addEmployer(id, this, isViewOnly: true);
                    viewForm.ShowDialog();
                }
                else if (columnName == "colDelete")
                {
                    frm_addEmployer deleteForm = new frm_addEmployer(id, this, isViewOnly: false, isDeleteMode: true);
                    deleteForm.ShowDialog();
                }
                else if (columnName == "colUpdate")
                {
                    frm_addEmployer updateForm = new frm_addEmployer(id, this, isViewOnly: false, isDeleteMode: false, isUpdateMode: true);
                    updateForm.ShowDialog();
                }
            }
        }
    }
}
