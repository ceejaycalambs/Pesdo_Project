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
    public partial class frm_addJobseeker : Form
    {
        private bool isDeleteMode = false;
        private bool isViewOnly = false;
        bool isUpdateMode = false;
        private int? jobseekerId = null;
        private frm_jobseekers jobseekerForm;
        
        public frm_addJobseeker(frm_jobseekers form)
        {
           
            InitializeComponent();
            
            jobseekerForm = form;

          

            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }
        public frm_addJobseeker(int id, frm_jobseekers form, bool isViewOnly, bool isDeleteMode = false, bool isUpdateMode = false)
        {
            InitializeComponent();
            jobseekerId = id;
            jobseekerForm = form;
            this.isViewOnly = isViewOnly;
            this.isDeleteMode = isDeleteMode;

            LoadJobseekerData(); // load data from database
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
            Status_switch.Enabled = false;
            btnAdd.Enabled = false;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }
        private void LoadJobseekerData()
        {
            try
            {
                using (SqlConnection conn = connection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM tbl_jobseeker WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", jobseekerId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtFirstname.Text = reader["FirstName"].ToString();
                        txtLastname.Text = reader["LastName"].ToString();
                        txtMiddlename.Text = reader["MiddleName"].ToString();
                        txtAge.Text = reader["Age"].ToString();
                        txtEmail.Text = reader["Email"].ToString();
                        lblStatus.Text = reader["Status"].ToString();
                        cbGender.Text = reader["Gender"].ToString();
                        cbEducAttain.Text = reader["Education_attainment"].ToString();
                        txtWorkExp.Text = reader["WorkyearExp"].ToString();
                        txt_pref1.Text = reader["preferred_job1"].ToString();
                        txt_pref2.Text = reader["preferred_job2"].ToString();
                        txt_pref3.Text = reader["preferred_job3"].ToString();
                        txtContact.Text = reader["Contact_no"].ToString();
                        txtAddress.Text = reader["Address"].ToString();
                        dtBdate.Value = Convert.ToDateTime(reader["Birthdate"]);
                        txtSkills.Text = reader["Skills"].ToString();

                        // Optional: disable btnAdd and enable btnUpdate
                        btnAdd.Enabled = false;
                        btnUpdate.Enabled = true;
                        btnDelete.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading jobseeker: " + ex.Message);
            }
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = connection.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO tbl_jobseeker
                    (FirstName, LastName, MiddleName, Age,Gender,Status, Email, Education_attainment, WorkyearExp, preferred_job1,preferred_job2, preferred_job3, Contact_no, Address, Birthdate, Skills,DateAdded)
                     VALUES
                    (@FirstName, @LastName, @MiddleName, @Age,@Gender,@Status, @Email, @Education_attainment, @WorkyearExp,@preferred_job1,@preferred_job2, @preferred_job3, @Contact_no, @Address, @Birthdate, @Skills,@DateAdded)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@FirstName", txtFirstname.Text.Trim());
                    cmd.Parameters.AddWithValue("@LastName", txtLastname.Text.Trim());
                    cmd.Parameters.AddWithValue("@MiddleName", txtMiddlename.Text.Trim());
                    cmd.Parameters.AddWithValue("@Age", Convert.ToInt32(txtAge.Text.Trim()));
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@DateAdded", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Status", lblStatus.Text);
                    cmd.Parameters.AddWithValue("@Gender", cbGender.Text);
                    cmd.Parameters.AddWithValue("@Education_attainment", cbEducAttain.Text);
                    cmd.Parameters.AddWithValue("@WorkyearExp", Convert.ToInt32(txtWorkExp.Text.Trim()));
                    cmd.Parameters.AddWithValue("@preferred_job1", txt_pref1.Text.Trim());
                    cmd.Parameters.AddWithValue("@preferred_job2", txt_pref2.Text.Trim());
                    cmd.Parameters.AddWithValue("@preferred_job3", txt_pref3.Text.Trim());
                    cmd.Parameters.AddWithValue("@Contact_no", txtContact.Text.Trim());
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@Birthdate", dtBdate.Value.Date);
                    cmd.Parameters.AddWithValue("@Skills", txtSkills.Text);



                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Jobseeker added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    jobseekerForm.LoadRecords();
                }
            }catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (jobseekerId == null)
            {
                MessageBox.Show("No jobseeker selected.");
                return;
            }

            try
            {
                using (SqlConnection conn = connection.GetConnection())
                {
                    conn.Open();
                    string updateQuery = @"
                UPDATE tbl_jobseeker SET
                    Lastname = @Lastname,
                    Firstname = @Firstname,
                    MiddleName = @MiddleName,
                    Age = @Age,
                    Gender = @Gender,
                    Status = @Status,
                    Email = @Email,
                    Contact_no = @Contact,
                    Address = @Address,
                    Birthdate = @Birthdate,
                    WorkyearExp = @WorkyearExp,
                    preferred_job1 = @preferred_job1,
                    preferred_job2 = @preferred_job2,
                    preferred_job3 = @preferred_job3,
                    Skills = @Skills
                     WHERE Id = @Id";

                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@Id", jobseekerId);
                    cmd.Parameters.AddWithValue("@Lastname", txtLastname.Text);
                    cmd.Parameters.AddWithValue("@Firstname", txtFirstname.Text);
                    cmd.Parameters.AddWithValue("@MiddleName", txtMiddlename.Text);
                    cmd.Parameters.AddWithValue("@Age", txtAge.Text);
                    cmd.Parameters.AddWithValue("@Gender", cbGender.Text);
                    cmd.Parameters.AddWithValue("@Status", lblStatus.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@Contact", txtContact.Text);
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@Birthdate", dtBdate.Value);
                    cmd.Parameters.AddWithValue("@WorkyearExp", txtWorkExp.Text);
                    cmd.Parameters.AddWithValue("@preferred_job1", txt_pref1.Text);
                    cmd.Parameters.AddWithValue("@preferred_job2", txt_pref2.Text);
                    cmd.Parameters.AddWithValue("@preferred_job3", txt_pref3.Text);
                    cmd.Parameters.AddWithValue("@Skills", txtSkills.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Jobseeker updated successfully!", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    jobseekerForm.LoadRecords();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating jobseeker: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (jobseekerId == null)
            {
                MessageBox.Show("No jobseeker selected.");
                return;
            }

            DialogResult confirm = MessageBox.Show("Are you sure you want to delete this jobseeker?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = connection.GetConnection())
                    {
                        conn.Open();
                        string deleteQuery = "DELETE FROM tbl_jobseeker WHERE Id = @Id";
                        SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                        cmd.Parameters.AddWithValue("@Id", jobseekerId);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Jobseeker deleted successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        jobseekerForm.LoadRecords(); // Refresh grid
                        this.Close(); // Close this form
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting jobseeker: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_addJobseeker_Load(object sender, EventArgs e)
        {

        }

        private void Status_switch_CheckedChanged(object sender, EventArgs e)
        {
            if (Status_switch.Checked)
            {
                lblStatus.Text = "In Field";
            }
            else
            {
                lblStatus.Text = "Available";
            }
        }
    }
}
