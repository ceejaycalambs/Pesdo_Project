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
    public partial class frm_addEmployer : Form
    {
        private frm_Employer frm_Employer;
        private bool isDeleteMode = false;
        private bool isViewOnly = false;
        private bool isUpdateMode = false;
        private int? EmployerId = null;

        public frm_addEmployer(frm_Employer form)
        {
            InitializeComponent();
            this.frm_Employer = form;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        public frm_addEmployer(int id, frm_Employer form, bool isViewOnly, bool isDeleteMode = false, bool isUpdateMode = false)
        {
            InitializeComponent();
            EmployerId = id;
            frm_Employer = form;
            this.isViewOnly = isViewOnly;
            this.isDeleteMode = isDeleteMode;
            this.isUpdateMode = isUpdateMode;

            LoadEmployerData();

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
            foreach (Control c in this.Controls)
            {
                if (c is TextBox) ((TextBox)c).ReadOnly = true;
                if (c is ComboBox) ((ComboBox)c).Enabled = false;
                if (c is DateTimePicker) ((DateTimePicker)c).Enabled = false;
                if (c is RadioButton) ((RadioButton)c).Enabled = false;
            }

            btnAdd.Enabled = false;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void LoadEmployerData()
        {
            try
            {
                using (SqlConnection conn = connection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM tbl_employers WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", EmployerId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtEmpName.Text = reader["Employer_Name"].ToString();
                        txtLoc.Text = reader["Location"].ToString();
                        txtContactNo.Text = reader["Contact_no"].ToString();
                        txtComDes.Text = reader["Company_Description"].ToString();
                        txtEmail.Text = reader["Email"].ToString();

                        // Set radio buttons based on Employment_Type
                        string empType = reader["Employment_Type"].ToString();
                        if (empType == "Local")
                            RdLocal.Checked = true;
                        else if (empType == "Overseas")
                            RdOverseas.Checked = true;
                        else if (empType == "Government")
                            RdGovernment.Checked = true;

                        btnAdd.Enabled = false;
                        btnUpdate.Enabled = true;
                        btnDelete.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading employer:\n" + ex.Message + "\n\n" + ex.StackTrace, "Debug Info");
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = connection.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO tbl_employers
                        (Employer_Name, Location, Email, Contact_no, Company_Description, Date_Added, Employment_Type)
                        VALUES
                        (@EmployerName, @Location, @Email, @ContactNo, @CompanyDescription, @DateAdded, @EmploymentType)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@EmployerName", txtEmpName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Location", txtLoc.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@DateAdded", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ContactNo", txtContactNo.Text);
                    cmd.Parameters.AddWithValue("@CompanyDescription", txtComDes.Text);
                    cmd.Parameters.AddWithValue("@EmploymentType", GetSelectedEmploymentType());

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Employer added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frm_Employer.LoadRecords();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (EmployerId == null)
            {
                MessageBox.Show("No Employer selected.");
                return;
            }

            try
            {
                using (SqlConnection conn = connection.GetConnection())
                {
                    conn.Open();
                    string updateQuery = @"
                        UPDATE tbl_Employers SET
                            Employer_Name = @EmployerName,
                            Location = @Location,
                            Contact_no = @ContactNo,
                            Company_Description = @CompanyDescription,
                            Email = @Email,
                            Employment_Type = @EmploymentType
                         WHERE Id = @Id";

                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@Id", EmployerId);
                    cmd.Parameters.AddWithValue("@EmployerName", txtEmpName.Text);
                    cmd.Parameters.AddWithValue("@Location", txtLoc.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@ContactNo", txtContactNo.Text);
                    cmd.Parameters.AddWithValue("@CompanyDescription", txtComDes.Text);
                    cmd.Parameters.AddWithValue("@EmploymentType", GetSelectedEmploymentType());

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Employer updated successfully!", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frm_Employer.LoadRecords();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating Employer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (EmployerId == null)
            {
                MessageBox.Show("No Employer selected.");
                return;
            }

            DialogResult confirm = MessageBox.Show("Are you sure you want to delete this Employer?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = connection.GetConnection())
                    {
                        conn.Open();
                        string deleteQuery = "DELETE FROM tbl_employers WHERE Id = @Id";
                        SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                        cmd.Parameters.AddWithValue("@Id", EmployerId);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Employer deleted successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        frm_Employer.LoadRecords();
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting Employer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string GetSelectedEmploymentType()
        {
            if (RdLocal.Checked)
                return "Local";
            else if (RdOverseas.Checked)
                return "Overseas";
            else if (RdGovernment.Checked)
                return "Government";
            else
                return string.Empty;
        }
    }
}
