using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pesdo_Project
{
    public partial class frm_main : Form
    {
        public frm_main()
        {
            InitializeComponent();
          
        }

        private void btnManageJobseekers_Click(object sender, EventArgs e)
        {
            frm_jobseekers frm_Jobseekers = new frm_jobseekers();
            frm_Jobseekers.TopLevel = false;
            panelDisplay.Controls.Add(frm_Jobseekers);
            frm_Jobseekers.BringToFront();
            frm_Jobseekers.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frm_Employer frm_Employer = new frm_Employer();
            frm_Employer.TopLevel = false;
            panelDisplay.Controls.Add(frm_Employer);
            frm_Employer.BringToFront();
            frm_Employer.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frm_jobvacancy frm_Jobvacancy = new frm_jobvacancy();
            frm_Jobvacancy.TopLevel = false;
            panelDisplay.Controls.Add (frm_Jobvacancy);
            frm_Jobvacancy .BringToFront();
            frm_Jobvacancy .Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
          
            frm_JobMatching frm_JobMatching = new frm_JobMatching();
            frm_JobMatching.TopLevel = false;   
            panelDisplay.Controls .Add(frm_JobMatching);
            frm_JobMatching .BringToFront();
            frm_JobMatching .Show();
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {

            frm_Dashboard frm_Dashboard = new frm_Dashboard();
            frm_Dashboard.TopLevel = false;
            panelDisplay .Controls.Add(frm_Dashboard);
            frm_Dashboard .BringToFront();
            frm_Dashboard .Show();
        }

        private void panelDisplay_Paint(object sender, PaintEventArgs e)
        {
            frm_Dashboard frm_Dashboard = new frm_Dashboard();
            frm_Dashboard.TopLevel = false;
            panelDisplay.Controls.Add(frm_Dashboard);
            frm_Dashboard.BringToFront();
            frm_Dashboard.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            frm_Reports frm_Report = new frm_Reports();
            frm_Report.TopLevel = false;
            panelDisplay.Controls.Add(frm_Report);
            frm_Report.BringToFront();
            frm_Report.Show();
        }
    }
}
