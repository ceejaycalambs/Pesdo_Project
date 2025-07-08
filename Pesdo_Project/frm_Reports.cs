using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace Pesdo_Project
{
    public partial class frm_Reports : Form
    {
        public frm_Reports()
        {
            InitializeComponent();
            this.Load += new EventHandler(frm_Reports_Load); // Attach load event
        }

        private void frm_Reports_Load(object sender, EventArgs e)
        {
            LoadReportData();
        }

        private void LoadReportData()
        {
            try
            {
                using (SqlConnection conn = connection.GetConnection()) // Replace with your actual DB connection method
                {
                    conn.Open();

                    // 📅 July 2025 filter
                    string selectedMonth = "2025-07";

                    string query = @"
                        SELECT 
                            jp.FullName,
                            js.Gender,
                            jp.Company_name,
                            jp.Position,
                            CONVERT(date, jp.Date_Hired) AS Date_Hired
                        FROM tbl_Jobplacement jp
                        INNER JOIN tbl_Jobseeker js
                            ON jp.Firstname = js.Firstname 
                            AND jp.Lastname = js.Lastname
                        WHERE FORMAT(jp.Date_Hired, 'yyyy-MM') = @MonthFilter";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@MonthFilter", selectedMonth);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("No job placements found for July 2025.");
                        return;
                    }

                    string reportPath = Path.Combine(Application.StartupPath, "rptJobPlacement.rdlc");

                    if (!File.Exists(reportPath))
                    {
                        MessageBox.Show("Report file not found:\n" + reportPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    ReportDataSource rds = new ReportDataSource("JobPlacementDataSet", dt);
                    reportViewer1.LocalReport.ReportPath = reportPath;
                    reportViewer1.LocalReport.DataSources.Clear();
                    reportViewer1.LocalReport.DataSources.Add(rds);
                    reportViewer1.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
