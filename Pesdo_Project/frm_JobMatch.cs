// UPDATED frm_JobMatch.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Pesdo_Project
{
    public partial class frm_JobMatch : Form
    {
        private string VacancyId;
        private int maxSelectableApplicants = 0;
        private HashSet<string> checkedApplicantIds = new HashSet<string>();

        public frm_JobMatch(string Id)
        {
            InitializeComponent();
            VacancyId = Id;

            LvApplicants.CheckBoxes = true;
            LvApplicants.View = View.Details;
            LvApplicants.ItemCheck += LvApplicants_ItemCheck;

            LoadVacancy();
            LoadApplicants();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetControlsReadOnly()
        {
            foreach (Control c in this.Controls)
            {
                if (c is TextBox) ((TextBox)c).ReadOnly = true;
                if (c is ComboBox) ((ComboBox)c).Enabled = false;
                if (c is DateTimePicker) ((DateTimePicker)c).Enabled = false;
            }
        }

        private void LoadVacancy()
        {
            using (SqlConnection conn = connection.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM tbl_Job_Vacancy WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", VacancyId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtJobTitle.Text = reader["Job_Title"].ToString();
                            txtEmpName.Text = reader["Employer_Name"].ToString();
                            txtLocation.Text = reader["Location"].ToString();
                            cbJobType.Text = reader["Job_Type"].ToString();
                            txtVacancyCount.Text = reader["Vacancy_Count"].ToString();
                            txtJobDescription.Text = reader["Job_Description"].ToString();

                            if (int.TryParse(reader["Vacancy_Count"].ToString(), out int parsedCount))
                                maxSelectableApplicants = parsedCount;

                            SetControlsReadOnly();
                        }
                    }
                }
            }
        }

        private void LoadApplicants()
        {
            try
            {
                using (SqlConnection conn = connection.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT * FROM tbl_jobseeker
                        WHERE Status != 'Hired'
                        AND Id NOT IN (
                            SELECT js.Id
                            FROM tbl_jobseeker js
                            INNER JOIN tbl_Jobplacement jp
                                ON js.Firstname = jp.Firstname AND js.Lastname = jp.Lastname
                                AND jp.Company_name = @CompanyName AND jp.Position = @Position)
                    ";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CompanyName", txtEmpName.Text);
                        cmd.Parameters.AddWithValue("@Position", txtJobTitle.Text);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            LvApplicants.Items.Clear();
                            LvApplicants.Columns.Clear();

                            LvApplicants.Columns.Add("Full Name", 250);
                            LvApplicants.Columns.Add("Age", 50);
                            LvApplicants.Columns.Add("Sex", 50);
                            LvApplicants.Columns.Add("Email", 100);
                            LvApplicants.Columns.Add("Contact", 100);
                            LvApplicants.Columns.Add("Education", 100);
                            LvApplicants.Columns.Add("Skills", 150);

                            while (reader.Read())
                            {
                                string fullName = reader["Firstname"] + " " + reader["MiddleName"] + " " + reader["Lastname"];
                                ListViewItem item = new ListViewItem(fullName);
                                item.Tag = reader["Id"].ToString();

                                item.SubItems.Add(reader["Age"].ToString());
                                item.SubItems.Add(reader["Gender"].ToString());
                                item.SubItems.Add(reader["Email"].ToString());
                                item.SubItems.Add(reader["Contact_no"].ToString());
                                item.SubItems.Add(reader["Education_attainment"].ToString());
                                item.SubItems.Add(reader["Skills"].ToString());

                                LvApplicants.Items.Add(item);

                                if (checkedApplicantIds.Contains(item.Tag.ToString()))
                                    item.Checked = true;
                            }

                            UpdateCheckboxAvailability();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading applicants: " + ex.Message);
            }
        }

        private void LvApplicants_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string applicantId = LvApplicants.Items[e.Index].Tag.ToString();

            this.BeginInvoke(new Action(() =>
            {
                if (e.NewValue == CheckState.Checked)
                    checkedApplicantIds.Add(applicantId);
                else
                    checkedApplicantIds.Remove(applicantId);

                UpdateCheckboxAvailability();
            }));

            int checkedCount = LvApplicants.CheckedItems.Count;

            if (e.NewValue == CheckState.Checked && checkedCount + 1 > maxSelectableApplicants)
            {
                MessageBox.Show("Only " + maxSelectableApplicants + " applicant(s) can be selected.");
                e.NewValue = CheckState.Unchecked;
            }
        }

        private void UpdateCheckboxAvailability()
        {
            int checkedCount = LvApplicants.CheckedItems.Count;

            foreach (ListViewItem item in LvApplicants.Items)
            {
                item.ForeColor = (!item.Checked && checkedCount >= maxSelectableApplicants) ? Color.Gray : Color.Black;
                item.BackColor = (!item.Checked && checkedCount >= maxSelectableApplicants) ? Color.LightGray : Color.White;
            }
        }

        private void btnMatch_Click(object sender, EventArgs e)
        {
            if (LvApplicants.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one applicant.");
                return;
            }

            int matchedCount = 0;

            try
            {
                using (SqlConnection conn = connection.GetConnection())
                {
                    conn.Open();

                    foreach (ListViewItem item in LvApplicants.CheckedItems)
                    {
                        string applicantId = item.Tag.ToString();

                        string getApplicantQuery = "SELECT * FROM tbl_jobseeker WHERE Id = @Id";

                        using (SqlCommand cmd = new SqlCommand(getApplicantQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", applicantId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string firstName = reader["Firstname"].ToString();
                                    string middleName = reader["MiddleName"].ToString();
                                    string lastName = reader["Lastname"].ToString();
                                    string gender = reader["Gender"].ToString();
                                    string middleInitial = string.IsNullOrWhiteSpace(middleName) ? "" : middleName.Substring(0, 1);

                                    reader.Close();

                                    // Check if already placed
                                    string checkQuery = @"SELECT COUNT(*) FROM tbl_Jobplacement
                                        WHERE Firstname = @Firstname AND Lastname = @Lastname
                                        AND Company_name = @Company AND Position = @Position";

                                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                                    {
                                        checkCmd.Parameters.AddWithValue("@Firstname", firstName);
                                        checkCmd.Parameters.AddWithValue("@Lastname", lastName);
                                        checkCmd.Parameters.AddWithValue("@Company", txtEmpName.Text);
                                        checkCmd.Parameters.AddWithValue("@Position", txtJobTitle.Text);

                                        int exists = (int)checkCmd.ExecuteScalar();

                                        if (exists == 0)
                                        {
                                            // Insert
                                            string insertQuery = @"INSERT INTO tbl_Jobplacement
                                            (Lastname, Firstname, Middle_Initial, Gender, Company_name, Employment_type, Position, Date_Hired)
                                            VALUES (@Lastname, @Firstname, @Middle_Initial, @Gender, @Company, @Employment_type, @Position, @Date_Hired)";

                                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                                            {
                                                insertCmd.Parameters.AddWithValue("@Lastname", lastName);
                                                insertCmd.Parameters.AddWithValue("@Firstname", firstName);
                                                insertCmd.Parameters.AddWithValue("@Middle_Initial", middleInitial);
                                                insertCmd.Parameters.AddWithValue("@Gender", gender);
                                                insertCmd.Parameters.AddWithValue("@Company", txtEmpName.Text);
                                                insertCmd.Parameters.AddWithValue("@Employment_type", cbJobType.Text);
                                                insertCmd.Parameters.AddWithValue("@Position", txtJobTitle.Text);
                                                insertCmd.Parameters.AddWithValue("@Date_Hired", DateTime.Now);

                                                insertCmd.ExecuteNonQuery();
                                            }

                                            matchedCount++;
                                        }

                                        // Update Status
                                        string updateStatus = "UPDATE tbl_jobseeker SET Status = 'Hired' WHERE Id = @Id";

                                        using (SqlCommand updateCmd = new SqlCommand(updateStatus, conn))
                                        {
                                            updateCmd.Parameters.AddWithValue("@Id", applicantId);
                                            updateCmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (matchedCount > 0)
                    {
                        string updateVacancyQuery = "UPDATE tbl_Job_Vacancy SET Vacancy_Count = Vacancy_Count - @Count WHERE Id = @Id";
                        using (SqlCommand cmd = new SqlCommand(updateVacancyQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Count", matchedCount);
                            cmd.Parameters.AddWithValue("@Id", VacancyId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show($"{matchedCount} applicant(s) matched successfully.");
                    LoadVacancy();
                    LoadApplicants();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadApplicants(); // Optionally filter here too
        }
    }
}
