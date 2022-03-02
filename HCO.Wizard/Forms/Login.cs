using HCO.Wizard.Common;
using HCO.Wizard.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HCO.Wizard.BL;

namespace HCO.Wizard.Forms
{
    public partial class Login : Form
    {
        Sb1ConnectionDTO _sb1ConnectionDTO;

        public Login()
        {
            InitializeComponent();
        }

        private void Connect()
        {
            DIAPIHelper.Connect(_sb1ConnectionDTO.Server,
                               _sb1ConnectionDTO.DbServerType,
                               _sb1ConnectionDTO.CompanyDB,
                               _sb1ConnectionDTO.LicenseServer,
                               _sb1ConnectionDTO.DbUserName,
                               _sb1ConnectionDTO.DbPassword,
                               this.txtUser.Text,
                               this.txtPassword.Text);

            this.Close();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            try
            {
                _sb1ConnectionDTO = Utilities.GetConfigFile();


                if (!string.IsNullOrEmpty(_sb1ConnectionDTO.CompanyName))
                    this.txtCompanyName.Text = _sb1ConnectionDTO.CompanyName;

                if (!string.IsNullOrEmpty(_sb1ConnectionDTO.CompanyDB))
                    this.txtCompanyDb.Text = _sb1ConnectionDTO.CompanyDB;

            }
            catch (Exception ex)
            {
                MessageUI.Error(ex.Message, ex);
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                Connect();
            }
            catch (Exception ex)
            {
                MessageUI.Error(ex.Message, ex);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void lblSettings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();
        }

    }
}
