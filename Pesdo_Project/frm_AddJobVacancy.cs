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
    public partial class frm_AddJobVacancy : Form
    {      
        private frm_jobvacancy frm_Jobvacancy;
        private bool isDeleteMode = false;
        private bool isViewOnly = false;
        bool isUpdateMode = false;
        private int? JobId = null;
        public frm_AddJobVacancy(frm_jobvacancy form)
        {
            this.frm_Jobvacancy = form;
           
            InitializeComponent();
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            LoadAutoCompleteData();
        }
        public frm_AddJobVacancy(int id, frm_jobvacancy form, bool isViewOnly, bool isDeleteMode = false, bool isUpdateMode = false)
        {
            InitializeComponent();
            JobId = id;
            frm_Jobvacancy = form;
            this.isViewOnly = isViewOnly;
            this.isDeleteMode = isDeleteMode;


            LoadJobVacancy(); // load data from database
            if (isViewOnly)
            {
                SetControlsReadOnly();
            }
            else if (isDeleteMode)
            {
                SetControlsReadOnly();
                btnAdd.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = true;
            }
            else if (isUpdateMode)
            {
                btnAdd.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = false;
            }
        }
        private void SetControlsReadOnly()
        {
            // Disable all input controls
            foreach (Control c in this.Controls)
            {
                if (c is TextBox) ((TextBox)c).ReadOnly = true;
                if (c is ComboBox) ((ComboBox)c).Enabled = false;
                if (c is DateTimePicker) ((DateTimePicker)c).Enabled = false;
            }

            // Disable status switch and buttons

            btnAdd.Enabled = false;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void LoadJobVacancy()
        {
            try
            {
                using (SqlConnection conn = connection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM tbl_Job_Vacancy WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", JobId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtEmpName.Text = reader["Employer_Name"].ToString();
                        txtVacancyCount.Text = reader["Vacancy_Count"].ToString();
                        txtLocation.Text = reader["Location"].ToString();
                        txtJobTitle.Text = reader["Job_Title"].ToString();
                        cbJobType.Text = reader["Job_Type"].ToString();
                        txtJobDescription.Text = reader["Job_Description"].ToString();


                        // Optional: disable btnAdd and enable btnUpdate
                        btnAdd.Enabled = false;
                        btnUpdate.Enabled = true;
                        btnDelete.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Job Vacancy:\n" + ex.Message + "\n\n" + ex.StackTrace, "Debug Info");
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    private void LoadAutoCompleteData()
        {
            AutoCompleteStringCollection autoComplete = new AutoCompleteStringCollection();

            // e.g. Data Source=.\SQLEXPRESS;Initial Catalog=YourDB;Integrated Security=True;
            using (SqlConnection conn = connection.GetConnection()) {
                {
                    string query = "SELECT Employer_Name FROM tbl_employers"; // or your own table/column
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        try
                        {
                            conn.Open();
                            SqlDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                autoComplete.Add(reader["Employer_Name"].ToString());
                            }
                            reader.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error loading autocomplete data: " + ex.Message);
                        }
                    }
                }

                txtEmpName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtEmpName.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txtEmpName.AutoCompleteCustomSource = autoComplete;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = connection.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO tbl_Job_Vacancy
                    (Employer_Name, Location, Job_Title, Job_Type,Job_Description,Vacancy_Count,DateAdded)
                     VALUES
                    (@EmployerName, @Location, @JobTitle, @JobType,@JobDescription,@VacancyCount,@DateAdded)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@EmployerName", txtEmpName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Location", txtLocation.Text.Trim());
                    cmd.Parameters.AddWithValue("@JobTitle", txtJobTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@DateAdded", DateTime.Now);
                    cmd.Parameters.AddWithValue("@JobType", cbJobType.Text);
                    cmd.Parameters.AddWithValue("@VacancyCount", txtVacancyCount.Text.Trim());
                    cmd.Parameters.AddWithValue("@JobDescription", txtJobDescription.Text);




                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Job Vacancy added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frm_Jobvacancy.LoadJobVacancy();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            if (JobId == null)
            {
                MessageBox.Show("No Job Vacancy selected.");
                return;
            }

            try
            {
                using (SqlConnection conn = connection.GetConnection())
                {
                    conn.Open();
                    string updateQuery = @"
                UPDATE tbl_Job_Vacancy SET
                    Employer_Name = @EmployerName,
                    Location = @Location,
                    Job_Title = @JobTitle,
                    Job_Type = @JobType,
                    Vacancy_Count = @VacancyCount,
                    Job_Description = @JobDescription
                WHERE Id = @Id";

                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@Id", JobId);
                    cmd.Parameters.AddWithValue("@EmployerName", txtEmpName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Location", txtLocation.Text.Trim());
                    cmd.Parameters.AddWithValue("@JobTitle", txtJobTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@VacancyCount", txtVacancyCount.Text.Trim());
                    cmd.Parameters.AddWithValue("@JobType", cbJobType.Text);
                    cmd.Parameters.AddWithValue("@JobDescription", txtJobDescription.Text.Trim());

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Job vacancy updated successfully!", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating job vacancy: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (JobId == null)
            {
                MessageBox.Show("No Job Vacancy selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this job vacancy?",
                                                  "Confirm Delete",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = connection.GetConnection())
                    {
                        conn.Open();
                        string deleteQuery = "DELETE FROM tbl_Job_Vacancy WHERE Id = @Id";

                        SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                        cmd.Parameters.AddWithValue("@Id", JobId);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Job vacancy deleted successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadJobVacancy(); // Refresh the DataGridView
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting job vacancy: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
