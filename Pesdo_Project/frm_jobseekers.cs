using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Pesdo_Project
{
    public partial class frm_jobseekers : Form
    {


        public frm_jobseekers()
        {
            InitializeComponent();
            LoadRecords();
        }
        public void LoadRecords()
        {
            try
            {
                using (SqlConnection conn = connection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT Id, Lastname, Firstname, MiddleName, Age, Gender, Status, Email, Contact_no, Address, Birthdate, WorkyearExp FROM tbl_jobseeker";
    
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.Columns["FirstName"].HeaderText = "First Name";
                    dataGridView1.Columns["LastName"].HeaderText = "Last Name";
                    dataGridView1.Columns["MiddleName"].HeaderText = "Middle Name";
                    dataGridView1.Columns["Age"].HeaderText = "Age";
                    dataGridView1.Columns["Status"].HeaderText = "Status";
                    dataGridView1.Columns["Email"].HeaderText = "Email";
                    dataGridView1.Columns["Contact_no"].HeaderText = "Contact no.";
                    dataGridView1.Columns["Address"].HeaderText = "Address";
                    dataGridView1.Columns["Birthdate"].HeaderText = "Birthdate";
                    dataGridView1.Columns["WorkyearExp"].HeaderText = "Work Year Experience";

                    dataGridView1.Columns["colView"].DisplayIndex = dataGridView1.Columns.Count - 1;
                    dataGridView1.Columns["colDelete"].DisplayIndex = dataGridView1.Columns.Count -1;
                    dataGridView1.Columns["colUpdate"].DisplayIndex = dataGridView1.Columns.Count - 1;
                    dataGridView1.Columns["Id"].Visible = false;

                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Loading data" + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frm_addJobseeker addjobseeker = new frm_addJobseeker(this);
            addjobseeker.Show();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "colView")
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Id"].Value);
                frm_addJobseeker viewForm = new frm_addJobseeker(id, this, true); // true = viewOnly
                viewForm.ShowDialog();
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "ColDelete")
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Id"].Value);
                frm_addJobseeker deleteForm = new frm_addJobseeker(id, this, isViewOnly: false, isDeleteMode: true);
                deleteForm.ShowDialog();
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "colUpdate")
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Id"].Value);
                frm_addJobseeker updateForm = new frm_addJobseeker(id, this, isViewOnly: false, isDeleteMode: false, isUpdateMode: true);
                updateForm.ShowDialog();
            }
        }
    }
}
