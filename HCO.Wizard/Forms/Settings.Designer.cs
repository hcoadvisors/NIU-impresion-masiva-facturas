
namespace HCO.Wizard.Forms
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtSL = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.rbtnDIAPI = new System.Windows.Forms.RadioButton();
            this.rbtnSL = new System.Windows.Forms.RadioButton();
            this.cboServerType = new System.Windows.Forms.ComboBox();
            this.txtLicenseServer = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.txtDbUser = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDbPassword = new System.Windows.Forms.TextBox();
            this.txtDbServer = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCompanyDb = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtB1Password = new System.Windows.Forms.TextBox();
            this.txtB1User = new System.Windows.Forms.TextBox();
            this.controlsValidator = new SCG.UX.Windows.SCGControlsValidator();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPdfPath = new System.Windows.Forms.TextBox();
            this.btnOpenFolderDialog = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtSL);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.rbtnDIAPI);
            this.groupBox2.Controls.Add(this.rbtnSL);
            this.groupBox2.Controls.Add(this.cboServerType);
            this.groupBox2.Controls.Add(this.txtLicenseServer);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.btnTest);
            this.groupBox2.Controls.Add(this.txtDbUser);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtDbPassword);
            this.groupBox2.Controls.Add(this.txtDbServer);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtCompanyDb);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtB1Password);
            this.groupBox2.Controls.Add(this.txtB1User);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.controlsValidator.SetOrden(this.groupBox2, 0);
            this.controlsValidator.SetPuedeSerVacio(this.groupBox2, false);
            this.controlsValidator.SetRequerido(this.groupBox2, false);
            this.groupBox2.Size = new System.Drawing.Size(627, 341);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Parámetros de conexión con SAP Business One";
            // 
            // txtSL
            // 
            this.controlsValidator.SetDescripcion(this.txtSL, "Campo requerido");
            this.txtSL.Location = new System.Drawing.Point(294, 64);
            this.txtSL.Name = "txtSL";
            this.controlsValidator.SetOrden(this.txtSL, 0);
            this.controlsValidator.SetPuedeSerVacio(this.txtSL, false);
            this.controlsValidator.SetRequerido(this.txtSL, false);
            this.txtSL.Size = new System.Drawing.Size(309, 21);
            this.txtSL.TabIndex = 2;
            this.txtSL.Tag = "9999";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(22, 67);
            this.label11.Name = "label11";
            this.controlsValidator.SetOrden(this.label11, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label11, false);
            this.controlsValidator.SetRequerido(this.label11, false);
            this.label11.Size = new System.Drawing.Size(139, 13);
            this.label11.TabIndex = 72;
            this.label11.Text = "Endpoint Service Layer";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(22, 43);
            this.label10.Name = "label10";
            this.controlsValidator.SetOrden(this.label10, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label10, false);
            this.controlsValidator.SetRequerido(this.label10, false);
            this.label10.Size = new System.Drawing.Size(104, 13);
            this.label10.TabIndex = 70;
            this.label10.Text = "Tipo de conexión";
            // 
            // rbtnDIAPI
            // 
            this.rbtnDIAPI.AutoSize = true;
            this.rbtnDIAPI.Checked = true;
            this.rbtnDIAPI.Location = new System.Drawing.Point(413, 41);
            this.rbtnDIAPI.Name = "rbtnDIAPI";
            this.controlsValidator.SetOrden(this.rbtnDIAPI, 0);
            this.controlsValidator.SetPuedeSerVacio(this.rbtnDIAPI, false);
            this.controlsValidator.SetRequerido(this.rbtnDIAPI, false);
            this.rbtnDIAPI.Size = new System.Drawing.Size(63, 17);
            this.rbtnDIAPI.TabIndex = 1;
            this.rbtnDIAPI.TabStop = true;
            this.rbtnDIAPI.Text = "DI API";
            this.rbtnDIAPI.UseVisualStyleBackColor = true;
            this.rbtnDIAPI.CheckedChanged += new System.EventHandler(this.rbtnDIAPI_CheckedChanged);
            // 
            // rbtnSL
            // 
            this.rbtnSL.AutoSize = true;
            this.rbtnSL.Location = new System.Drawing.Point(294, 41);
            this.rbtnSL.Name = "rbtnSL";
            this.controlsValidator.SetOrden(this.rbtnSL, 0);
            this.controlsValidator.SetPuedeSerVacio(this.rbtnSL, false);
            this.controlsValidator.SetRequerido(this.rbtnSL, false);
            this.rbtnSL.Size = new System.Drawing.Size(104, 17);
            this.rbtnSL.TabIndex = 0;
            this.rbtnSL.Text = "Service Layer";
            this.rbtnSL.UseVisualStyleBackColor = true;
            this.rbtnSL.CheckedChanged += new System.EventHandler(this.rbtnSL_CheckedChanged);
            // 
            // cboServerType
            // 
            this.controlsValidator.SetDescripcion(this.cboServerType, "Campo requerido");
            this.cboServerType.FormattingEnabled = true;
            this.cboServerType.Items.AddRange(new object[] {
            "dst_MSSQL2008",
            "dst_MSSQL2012",
            "dst_MSSQL2014",
            "dst_HANADB",
            "dst_MSSQL2016",
            "dst_MSSQL2017",
            "dst_MSSQL2019"});
            this.cboServerType.Location = new System.Drawing.Point(294, 145);
            this.cboServerType.Name = "cboServerType";
            this.controlsValidator.SetOrden(this.cboServerType, 3);
            this.controlsValidator.SetPuedeSerVacio(this.cboServerType, false);
            this.controlsValidator.SetRequerido(this.cboServerType, false);
            this.cboServerType.Size = new System.Drawing.Size(309, 21);
            this.cboServerType.TabIndex = 5;
            // 
            // txtLicenseServer
            // 
            this.controlsValidator.SetDescripcion(this.txtLicenseServer, "Campo requerido");
            this.txtLicenseServer.Location = new System.Drawing.Point(294, 172);
            this.txtLicenseServer.Name = "txtLicenseServer";
            this.controlsValidator.SetOrden(this.txtLicenseServer, 4);
            this.controlsValidator.SetPuedeSerVacio(this.txtLicenseServer, false);
            this.controlsValidator.SetRequerido(this.txtLicenseServer, false);
            this.txtLicenseServer.Size = new System.Drawing.Size(309, 21);
            this.txtLicenseServer.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(22, 175);
            this.label7.Name = "label7";
            this.controlsValidator.SetOrden(this.label7, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label7, false);
            this.controlsValidator.SetRequerido(this.label7, false);
            this.label7.Size = new System.Drawing.Size(126, 13);
            this.label7.TabIndex = 45;
            this.label7.Text = "Servidor de licencias";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 148);
            this.label6.Name = "label6";
            this.controlsValidator.SetOrden(this.label6, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label6, false);
            this.controlsValidator.SetRequerido(this.label6, false);
            this.label6.Size = new System.Drawing.Size(100, 13);
            this.label6.TabIndex = 44;
            this.label6.Text = "Tipo de servidor";
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(471, 307);
            this.btnTest.Name = "btnTest";
            this.controlsValidator.SetOrden(this.btnTest, 0);
            this.controlsValidator.SetPuedeSerVacio(this.btnTest, false);
            this.controlsValidator.SetRequerido(this.btnTest, false);
            this.btnTest.Size = new System.Drawing.Size(132, 23);
            this.btnTest.TabIndex = 11;
            this.btnTest.Text = "Probar conexión";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // txtDbUser
            // 
            this.controlsValidator.SetDescripcion(this.txtDbUser, "Campo requerido");
            this.txtDbUser.Location = new System.Drawing.Point(294, 253);
            this.txtDbUser.Name = "txtDbUser";
            this.controlsValidator.SetOrden(this.txtDbUser, 7);
            this.controlsValidator.SetPuedeSerVacio(this.txtDbUser, false);
            this.controlsValidator.SetRequerido(this.txtDbUser, false);
            this.txtDbUser.Size = new System.Drawing.Size(309, 21);
            this.txtDbUser.TabIndex = 9;
            this.txtDbUser.Tag = "9999";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 256);
            this.label5.Name = "label5";
            this.controlsValidator.SetOrden(this.label5, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label5, false);
            this.controlsValidator.SetRequerido(this.label5, false);
            this.label5.Size = new System.Drawing.Size(134, 13);
            this.label5.TabIndex = 39;
            this.label5.Text = "Usuario base de datos";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 121);
            this.label4.Name = "label4";
            this.controlsValidator.SetOrden(this.label4, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label4, false);
            this.controlsValidator.SetRequerido(this.label4, false);
            this.label4.Size = new System.Drawing.Size(244, 13);
            this.label4.TabIndex = 38;
            this.label4.Text = "Nombre base de datos SAP Business One";
            // 
            // txtDbPassword
            // 
            this.controlsValidator.SetDescripcion(this.txtDbPassword, "Campo requerido");
            this.txtDbPassword.Location = new System.Drawing.Point(294, 280);
            this.txtDbPassword.Name = "txtDbPassword";
            this.controlsValidator.SetOrden(this.txtDbPassword, 8);
            this.txtDbPassword.PasswordChar = '*';
            this.controlsValidator.SetPuedeSerVacio(this.txtDbPassword, false);
            this.controlsValidator.SetRequerido(this.txtDbPassword, false);
            this.txtDbPassword.Size = new System.Drawing.Size(309, 21);
            this.txtDbPassword.TabIndex = 10;
            this.txtDbPassword.Tag = "9999";
            // 
            // txtDbServer
            // 
            this.controlsValidator.SetDescripcion(this.txtDbServer, "Campo requerido");
            this.txtDbServer.Location = new System.Drawing.Point(294, 91);
            this.txtDbServer.Name = "txtDbServer";
            this.controlsValidator.SetOrden(this.txtDbServer, 1);
            this.controlsValidator.SetPuedeSerVacio(this.txtDbServer, false);
            this.controlsValidator.SetRequerido(this.txtDbServer, false);
            this.txtDbServer.Size = new System.Drawing.Size(309, 21);
            this.txtDbServer.TabIndex = 3;
            this.txtDbServer.Tag = "9999";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 283);
            this.label2.Name = "label2";
            this.controlsValidator.SetOrden(this.label2, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label2, false);
            this.controlsValidator.SetRequerido(this.label2, false);
            this.label2.Size = new System.Drawing.Size(203, 13);
            this.label2.TabIndex = 40;
            this.label2.Text = "Contraseña usuario base de datos";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 94);
            this.label3.Name = "label3";
            this.controlsValidator.SetOrden(this.label3, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label3, false);
            this.controlsValidator.SetRequerido(this.label3, false);
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 37;
            this.label3.Text = "Nombre servidor";
            // 
            // txtCompanyDb
            // 
            this.controlsValidator.SetDescripcion(this.txtCompanyDb, "Campo requerido");
            this.txtCompanyDb.Location = new System.Drawing.Point(294, 118);
            this.txtCompanyDb.Name = "txtCompanyDb";
            this.controlsValidator.SetOrden(this.txtCompanyDb, 2);
            this.controlsValidator.SetPuedeSerVacio(this.txtCompanyDb, false);
            this.controlsValidator.SetRequerido(this.txtCompanyDb, false);
            this.txtCompanyDb.Size = new System.Drawing.Size(309, 21);
            this.txtCompanyDb.TabIndex = 4;
            this.txtCompanyDb.Tag = "9999";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(22, 202);
            this.label9.Name = "label9";
            this.controlsValidator.SetOrden(this.label9, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label9, false);
            this.controlsValidator.SetRequerido(this.label9, false);
            this.label9.Size = new System.Drawing.Size(176, 13);
            this.label9.TabIndex = 42;
            this.label9.Text = "Usuario de SAP Business One";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(22, 229);
            this.label8.Name = "label8";
            this.controlsValidator.SetOrden(this.label8, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label8, false);
            this.controlsValidator.SetRequerido(this.label8, false);
            this.label8.Size = new System.Drawing.Size(263, 13);
            this.label8.TabIndex = 43;
            this.label8.Text = "Contraseña de usuario de SAP Business One";
            // 
            // txtB1Password
            // 
            this.controlsValidator.SetDescripcion(this.txtB1Password, "Campo requerido");
            this.txtB1Password.Location = new System.Drawing.Point(294, 226);
            this.txtB1Password.Name = "txtB1Password";
            this.controlsValidator.SetOrden(this.txtB1Password, 6);
            this.txtB1Password.PasswordChar = '*';
            this.controlsValidator.SetPuedeSerVacio(this.txtB1Password, false);
            this.controlsValidator.SetRequerido(this.txtB1Password, false);
            this.txtB1Password.Size = new System.Drawing.Size(309, 21);
            this.txtB1Password.TabIndex = 8;
            this.txtB1Password.Tag = "9999";
            // 
            // txtB1User
            // 
            this.controlsValidator.SetDescripcion(this.txtB1User, "Campo requerido");
            this.txtB1User.Location = new System.Drawing.Point(294, 199);
            this.txtB1User.Name = "txtB1User";
            this.controlsValidator.SetOrden(this.txtB1User, 5);
            this.controlsValidator.SetPuedeSerVacio(this.txtB1User, false);
            this.controlsValidator.SetRequerido(this.txtB1User, false);
            this.txtB1User.Size = new System.Drawing.Size(309, 21);
            this.txtB1User.TabIndex = 7;
            this.txtB1User.Tag = "9999";
            // 
            // controlsValidator
            // 
            this.controlsValidator.ColorError = System.Drawing.Color.Beige;
            this.controlsValidator.Duracion = 5000;
            this.controlsValidator.ErrorSub = null;
            this.controlsValidator.FormularioPadre = null;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(445, 408);
            this.btnSave.Name = "btnSave";
            this.controlsValidator.SetOrden(this.btnSave, 0);
            this.controlsValidator.SetPuedeSerVacio(this.btnSave, false);
            this.controlsValidator.SetRequerido(this.btnSave, false);
            this.btnSave.Size = new System.Drawing.Size(82, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Guardar";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(533, 408);
            this.btnCerrar.Name = "btnCerrar";
            this.controlsValidator.SetOrden(this.btnCerrar, 0);
            this.controlsValidator.SetPuedeSerVacio(this.btnCerrar, false);
            this.controlsValidator.SetRequerido(this.btnCerrar, false);
            this.btnCerrar.Size = new System.Drawing.Size(82, 23);
            this.btnCerrar.TabIndex = 4;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 369);
            this.label1.Name = "label1";
            this.controlsValidator.SetOrden(this.label1, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label1, false);
            this.controlsValidator.SetRequerido(this.label1, false);
            this.label1.Size = new System.Drawing.Size(158, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Ruta carpeta archivos PDF";
            // 
            // txtPdfPath
            // 
            this.controlsValidator.SetDescripcion(this.txtPdfPath, "Campo mandatorio");
            this.txtPdfPath.Location = new System.Drawing.Point(306, 361);
            this.txtPdfPath.Name = "txtPdfPath";
            this.controlsValidator.SetOrden(this.txtPdfPath, 0);
            this.controlsValidator.SetPuedeSerVacio(this.txtPdfPath, false);
            this.txtPdfPath.ReadOnly = true;
            this.controlsValidator.SetRequerido(this.txtPdfPath, true);
            this.txtPdfPath.Size = new System.Drawing.Size(275, 21);
            this.txtPdfPath.TabIndex = 1;
            // 
            // btnOpenFolderDialog
            // 
            this.btnOpenFolderDialog.Location = new System.Drawing.Point(587, 360);
            this.btnOpenFolderDialog.Name = "btnOpenFolderDialog";
            this.controlsValidator.SetOrden(this.btnOpenFolderDialog, 0);
            this.controlsValidator.SetPuedeSerVacio(this.btnOpenFolderDialog, false);
            this.controlsValidator.SetRequerido(this.btnOpenFolderDialog, false);
            this.btnOpenFolderDialog.Size = new System.Drawing.Size(28, 23);
            this.btnOpenFolderDialog.TabIndex = 2;
            this.btnOpenFolderDialog.Text = "...";
            this.btnOpenFolderDialog.UseVisualStyleBackColor = true;
            this.btnOpenFolderDialog.Click += new System.EventHandler(this.btnOpenFolderDialog_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 444);
            this.Controls.Add(this.btnOpenFolderDialog);
            this.Controls.Add(this.txtPdfPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox2);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.controlsValidator.SetOrden(this, 0);
            this.controlsValidator.SetPuedeSerVacio(this, false);
            this.controlsValidator.SetRequerido(this, false);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.RadioButton rbtnDIAPI;
        public System.Windows.Forms.RadioButton rbtnSL;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private SCG.UX.Windows.SCGControlsValidator controlsValidator;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.TextBox txtSL;
        private System.Windows.Forms.ComboBox cboServerType;
        private System.Windows.Forms.TextBox txtLicenseServer;
        private System.Windows.Forms.TextBox txtDbUser;
        private System.Windows.Forms.TextBox txtDbPassword;
        private System.Windows.Forms.TextBox txtDbServer;
        private System.Windows.Forms.TextBox txtCompanyDb;
        private System.Windows.Forms.TextBox txtB1Password;
        private System.Windows.Forms.TextBox txtB1User;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPdfPath;
        private System.Windows.Forms.Button btnOpenFolderDialog;
    }
}