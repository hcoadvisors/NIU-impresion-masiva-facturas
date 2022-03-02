using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HCO.Wizard.DTO;
using HCO.Wizard.Common;
using HCO.Wizard.BL;

namespace HCO.Wizard.Forms
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void EnabledControls()
        {
            if (rbtnSL.Checked)
            {
                this.txtSL.Enabled = true;
                this.controlsValidator.SetRequerido(this.txtSL, true);

                this.txtDbServer.Enabled = false;
                this.txtLicenseServer.Enabled = false;
                this.txtDbUser.Enabled = false;
                this.txtDbPassword.Enabled = false;

                this.txtDbServer.Clear();
                this.txtLicenseServer.Clear();
                this.txtDbUser.Clear();
                this.txtDbPassword.Clear();

                this.controlsValidator.SetRequerido(this.txtDbServer, false);
                this.controlsValidator.SetRequerido(this.txtLicenseServer, false);
                this.controlsValidator.SetRequerido(this.txtDbUser, false);
                this.controlsValidator.SetRequerido(this.txtDbPassword, false);
            }
            else
            {
                this.txtSL.Enabled = false;

                this.controlsValidator.SetRequerido(this.txtSL, false);

                this.txtSL.Clear();

                this.txtDbServer.Enabled = true;
                this.txtLicenseServer.Enabled = true;
                this.txtDbUser.Enabled = true;
                this.txtDbPassword.Enabled = true;

                this.controlsValidator.SetRequerido(this.txtDbServer, true);
                this.controlsValidator.SetRequerido(this.txtLicenseServer, true);

            }
        }

        private void SaveConfigFile()
        {
            if (!this.controlsValidator.Valida(true))
                return;

            Sb1ConnectionDTO sb1ConnectionDTO = new Sb1ConnectionDTO();
            sb1ConnectionDTO.UseSL = this.rbtnSL.Checked;
            sb1ConnectionDTO.Server = this.txtDbServer.Text;
            sb1ConnectionDTO.LicenseServer = this.txtLicenseServer.Text;
            sb1ConnectionDTO.DbServerType = this.cboServerType.Text;
            sb1ConnectionDTO.CompanyDB = this.txtCompanyDb.Text;
            sb1ConnectionDTO.CompanyName = DIAPIHelper.Sb1CompanyName;
            sb1ConnectionDTO.PDFPath = this.txtPdfPath.Text;

            if (this.txtDbUser.Text.Length > 0)
                sb1ConnectionDTO.DbUserName = this.txtDbUser.Text;

            if (this.txtDbPassword.Text.Length > 0)
                sb1ConnectionDTO.DbPassword = CryptoEngine.Encrypt(this.txtDbPassword.Text, CryptoEngine.K);
            //sb1ConnectionDTO.UserName = this.txtB1User.Text;
            //sb1ConnectionDTO.Password = this.txtB1Password.Text;


            if (MessageUI.Question("¿Esta seguro de guardar los cambios?") == DialogResult.Yes)
            {
                Utilities.SaveConfigFile(sb1ConnectionDTO);
                Close();
            }
        }


        private void Test()
        {

            try
            {
                if (!this.controlsValidator.Valida(true))
                    return;

                DIAPIHelper.Connect(this.txtDbServer.Text, 
                                    this.cboServerType.Text, 
                                    this.txtCompanyDb.Text, 
                                    this.txtLicenseServer.Text, 
                                    this.txtDbUser.Text, 
                                    this.txtDbPassword.Text,
                                    this.txtB1User.Text,
                                    this.txtB1Password.Text);

                MessageUI.Show("Conexión exitosa con " + DIAPIHelper.Sb1CompanyName);

                DIAPIHelper.Disconnect();
            }
            catch (Exception ex)
            {
                MessageUI.Error(ex.Message, ex);
            }


        }

        private void LoadConfigFile()
        {
            Sb1ConnectionDTO sb1ConnectionDTO = Utilities.GetConfigFile();

            if (sb1ConnectionDTO == null)
            {
                this.rbtnDIAPI.Checked = true;
            }
            else
            {
                this.rbtnSL.Checked = sb1ConnectionDTO.UseSL;
                this.rbtnDIAPI.Checked = !sb1ConnectionDTO.UseSL;
                this.txtDbServer.Text = sb1ConnectionDTO.Server;
                this.txtLicenseServer.Text = sb1ConnectionDTO.LicenseServer;
                this.cboServerType.Text = sb1ConnectionDTO.DbServerType;
                this.txtCompanyDb.Text = sb1ConnectionDTO.CompanyDB;

                if (!string.IsNullOrEmpty(sb1ConnectionDTO.DbUserName))
                    this.txtDbUser.Text = sb1ConnectionDTO.DbUserName;

                if (!string.IsNullOrEmpty(sb1ConnectionDTO.DbPassword))
                    this.txtDbPassword.Text = CryptoEngine.Decrypt(sb1ConnectionDTO.DbPassword, CryptoEngine.K);
                //this.txtB1User.Text = sb1ConnectionDTO.UserName;
                //this.txtB1Password.Text = sb1ConnectionDTO.Password;

                this.txtPdfPath.Text = sb1ConnectionDTO.PDFPath;
            }

            this.EnabledControls();
        }

        private void rbtnSL_CheckedChanged(object sender, EventArgs e)
        {
            EnabledControls();
        }

        private void rbtnDIAPI_CheckedChanged(object sender, EventArgs e)
        {
            EnabledControls();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveConfigFile();

            }
            catch (Exception ex)
            {
                MessageUI.Error(ex.Message, ex);
            }
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            try
            {
                LoadConfigFile();
            }
            catch (Exception ex)
            {
                MessageUI.Error(ex.Message, ex);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            Test();
        }

        private void btnOpenFolderDialog_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    this.txtPdfPath.Text = fbd.SelectedPath;
                }
            }
        }
    }
}
