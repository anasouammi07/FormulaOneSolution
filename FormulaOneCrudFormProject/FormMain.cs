using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FormulaOneDll;

namespace FormulaOneCrudFormProject
{
    public partial class FormMain : Form
    {
        DbTools db;
        BindingList<Team> teams;
        
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            db = new DbTools();
            db.GetCountries();
            db.GetDrivers();
            teams = new BindingList<Team>(db.LoadTeams());
            listBoxTeam.DataSource = teams;
        }
        private void listBoxTeam_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Team t = teams.ElementAt(listBoxTeam.SelectedIndex);
                txtFullName.Text = t.FullTeamName.ToString();
                txtPowerUnit.Text = t.PowerUnit.ToString();
                txtCountry.Text = t.Country.CountryName.ToString();
                txtFirstDriver.Text = t.FirstDriver.ToString();
                txtSecondDriver.Text = t.SecondDriver.ToString();
                txtTechChief.Text = t.TechnicalChief.ToString();
                txtChassis.Text = t.Chassis.ToString();
            }
            catch(Exception ex)
            {
                
            }

        }

        private void stampaToolStripButton_Click(object sender, EventArgs e)
        {
            Utils.SerializeToCsv(teams, @".\Teams.csv"); 
            Utils.SerializeToCsv(teams, @".\Teams.json");
        }
    }
}
