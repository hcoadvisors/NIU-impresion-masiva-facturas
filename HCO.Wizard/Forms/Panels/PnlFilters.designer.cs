namespace HCO.Wizard.Forms.Panels
{
    partial class PnlFilters
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpFromTaxDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToTaxDate = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dtpToDocDueDate = new System.Windows.Forms.DateTimePicker();
            this.dtpFromDocDueDate = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cboDeliveryType = new System.Windows.Forms.ComboBox();
            this.cboPaymentTerms = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cboSalesEmployee = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cboBusinessPartnerGroup = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.chkTaxDate = new System.Windows.Forms.CheckBox();
            this.gbTaxDate = new System.Windows.Forms.GroupBox();
            this.gbDueDate = new System.Windows.Forms.GroupBox();
            this.chkDueDate = new System.Windows.Forms.CheckBox();
            this.gbDelivery = new System.Windows.Forms.GroupBox();
            this.chkDelivery = new System.Windows.Forms.CheckBox();
            this.gbPaymentTerm = new System.Windows.Forms.GroupBox();
            this.chkPaymentTerms = new System.Windows.Forms.CheckBox();
            this.gbSalesEmp = new System.Windows.Forms.GroupBox();
            this.chkSalesEmployees = new System.Windows.Forms.CheckBox();
            this.chkBusinessPartnerGroup = new System.Windows.Forms.CheckBox();
            this.gbBusinessPartnerGroup = new System.Windows.Forms.GroupBox();
            this.pnlFondo.SuspendLayout();
            this.gbTaxDate.SuspendLayout();
            this.gbDueDate.SuspendLayout();
            this.gbDelivery.SuspendLayout();
            this.gbPaymentTerm.SuspendLayout();
            this.gbSalesEmp.SuspendLayout();
            this.gbBusinessPartnerGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlFondo
            // 
            this.pnlFondo.Controls.Add(this.chkBusinessPartnerGroup);
            this.pnlFondo.Controls.Add(this.chkSalesEmployees);
            this.pnlFondo.Controls.Add(this.chkPaymentTerms);
            this.pnlFondo.Controls.Add(this.chkDelivery);
            this.pnlFondo.Controls.Add(this.chkDueDate);
            this.pnlFondo.Controls.Add(this.chkTaxDate);
            this.pnlFondo.Controls.Add(this.gbBusinessPartnerGroup);
            this.pnlFondo.Controls.Add(this.gbSalesEmp);
            this.pnlFondo.Controls.Add(this.gbPaymentTerm);
            this.pnlFondo.Controls.Add(this.gbDelivery);
            this.pnlFondo.Controls.Add(this.gbDueDate);
            this.pnlFondo.Controls.Add(this.gbTaxDate);
            this.pnlFondo.Controls.Add(this.label2);
            this.pnlFondo.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(20, 34);
            this.label2.Name = "label2";
            this.controlsValidator.SetOrden(this.label2, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label2, false);
            this.controlsValidator.SetRequerido(this.label2, false);
            this.label2.Size = new System.Drawing.Size(254, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Ingrese los parámetros de búsqueda de los pedidos";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(263, 26);
            this.label3.Name = "label3";
            this.controlsValidator.SetOrden(this.label3, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label3, false);
            this.controlsValidator.SetRequerido(this.label3, false);
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Hasta";
            // 
            // dtpFromTaxDate
            // 
            this.dtpFromTaxDate.CustomFormat = "dd/MM/yyyy";
            this.dtpFromTaxDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromTaxDate.Location = new System.Drawing.Point(168, 20);
            this.dtpFromTaxDate.Name = "dtpFromTaxDate";
            this.controlsValidator.SetOrden(this.dtpFromTaxDate, 0);
            this.controlsValidator.SetPuedeSerVacio(this.dtpFromTaxDate, false);
            this.controlsValidator.SetRequerido(this.dtpFromTaxDate, false);
            this.dtpFromTaxDate.Size = new System.Drawing.Size(85, 21);
            this.dtpFromTaxDate.TabIndex = 1;
            this.dtpFromTaxDate.Tag = "9999";
            // 
            // dtpToTaxDate
            // 
            this.dtpToTaxDate.CustomFormat = "dd/MM/yyyy";
            this.dtpToTaxDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToTaxDate.Location = new System.Drawing.Point(304, 20);
            this.dtpToTaxDate.Name = "dtpToTaxDate";
            this.controlsValidator.SetOrden(this.dtpToTaxDate, 0);
            this.controlsValidator.SetPuedeSerVacio(this.dtpToTaxDate, false);
            this.controlsValidator.SetRequerido(this.dtpToTaxDate, false);
            this.dtpToTaxDate.Size = new System.Drawing.Size(85, 21);
            this.dtpToTaxDate.TabIndex = 2;
            this.dtpToTaxDate.Tag = "9999";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(125, 26);
            this.label4.Name = "label4";
            this.controlsValidator.SetOrden(this.label4, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label4, false);
            this.controlsValidator.SetRequerido(this.label4, false);
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Desde";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 26);
            this.label1.Name = "label1";
            this.controlsValidator.SetOrden(this.label1, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label1, false);
            this.controlsValidator.SetRequerido(this.label1, false);
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Fecha de emisión";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 26);
            this.label5.Name = "label5";
            this.controlsValidator.SetOrden(this.label5, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label5, false);
            this.controlsValidator.SetRequerido(this.label5, false);
            this.label5.Size = new System.Drawing.Size(100, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Fecha de despacho";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(125, 26);
            this.label6.Name = "label6";
            this.controlsValidator.SetOrden(this.label6, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label6, false);
            this.controlsValidator.SetRequerido(this.label6, false);
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Desde";
            // 
            // dtpToDocDueDate
            // 
            this.dtpToDocDueDate.CustomFormat = "dd/MM/yyyy";
            this.dtpToDocDueDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDocDueDate.Location = new System.Drawing.Point(304, 20);
            this.dtpToDocDueDate.Name = "dtpToDocDueDate";
            this.controlsValidator.SetOrden(this.dtpToDocDueDate, 0);
            this.controlsValidator.SetPuedeSerVacio(this.dtpToDocDueDate, false);
            this.controlsValidator.SetRequerido(this.dtpToDocDueDate, false);
            this.dtpToDocDueDate.Size = new System.Drawing.Size(85, 21);
            this.dtpToDocDueDate.TabIndex = 2;
            this.dtpToDocDueDate.Tag = "9999";
            // 
            // dtpFromDocDueDate
            // 
            this.dtpFromDocDueDate.CustomFormat = "dd/MM/yyyy";
            this.dtpFromDocDueDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDocDueDate.Location = new System.Drawing.Point(168, 20);
            this.dtpFromDocDueDate.Name = "dtpFromDocDueDate";
            this.controlsValidator.SetOrden(this.dtpFromDocDueDate, 0);
            this.controlsValidator.SetPuedeSerVacio(this.dtpFromDocDueDate, false);
            this.controlsValidator.SetRequerido(this.dtpFromDocDueDate, false);
            this.dtpFromDocDueDate.Size = new System.Drawing.Size(85, 21);
            this.dtpFromDocDueDate.TabIndex = 1;
            this.dtpFromDocDueDate.Tag = "9999";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(263, 26);
            this.label7.Name = "label7";
            this.controlsValidator.SetOrden(this.label7, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label7, false);
            this.controlsValidator.SetRequerido(this.label7, false);
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Hasta";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 23);
            this.label8.Name = "label8";
            this.controlsValidator.SetOrden(this.label8, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label8, false);
            this.controlsValidator.SetRequerido(this.label8, false);
            this.label8.Size = new System.Drawing.Size(91, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Tipo de despacho";
            // 
            // cboDeliveryType
            // 
            this.cboDeliveryType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDeliveryType.FormattingEnabled = true;
            this.cboDeliveryType.Location = new System.Drawing.Point(168, 20);
            this.cboDeliveryType.Name = "cboDeliveryType";
            this.controlsValidator.SetOrden(this.cboDeliveryType, 0);
            this.controlsValidator.SetPuedeSerVacio(this.cboDeliveryType, false);
            this.controlsValidator.SetRequerido(this.cboDeliveryType, false);
            this.cboDeliveryType.Size = new System.Drawing.Size(221, 21);
            this.cboDeliveryType.TabIndex = 1;
            // 
            // cboPaymentTerms
            // 
            this.cboPaymentTerms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPaymentTerms.FormattingEnabled = true;
            this.cboPaymentTerms.Location = new System.Drawing.Point(168, 20);
            this.cboPaymentTerms.Name = "cboPaymentTerms";
            this.controlsValidator.SetOrden(this.cboPaymentTerms, 0);
            this.controlsValidator.SetPuedeSerVacio(this.cboPaymentTerms, false);
            this.controlsValidator.SetRequerido(this.cboPaymentTerms, false);
            this.cboPaymentTerms.Size = new System.Drawing.Size(221, 21);
            this.cboPaymentTerms.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 23);
            this.label9.Name = "label9";
            this.controlsValidator.SetOrden(this.label9, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label9, false);
            this.controlsValidator.SetRequerido(this.label9, false);
            this.label9.Size = new System.Drawing.Size(99, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Condición de venta";
            // 
            // cboSalesEmployee
            // 
            this.cboSalesEmployee.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSalesEmployee.FormattingEnabled = true;
            this.cboSalesEmployee.Location = new System.Drawing.Point(169, 20);
            this.cboSalesEmployee.Name = "cboSalesEmployee";
            this.controlsValidator.SetOrden(this.cboSalesEmployee, 0);
            this.controlsValidator.SetPuedeSerVacio(this.cboSalesEmployee, false);
            this.controlsValidator.SetRequerido(this.cboSalesEmployee, false);
            this.cboSalesEmployee.Size = new System.Drawing.Size(221, 21);
            this.cboSalesEmployee.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 23);
            this.label10.Name = "label10";
            this.controlsValidator.SetOrden(this.label10, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label10, false);
            this.controlsValidator.SetRequerido(this.label10, false);
            this.label10.Size = new System.Drawing.Size(99, 13);
            this.label10.TabIndex = 23;
            this.label10.Text = "Empleado de venta";
            // 
            // cboBusinessPartnerGroup
            // 
            this.cboBusinessPartnerGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusinessPartnerGroup.FormattingEnabled = true;
            this.cboBusinessPartnerGroup.Location = new System.Drawing.Point(169, 20);
            this.cboBusinessPartnerGroup.Name = "cboBusinessPartnerGroup";
            this.controlsValidator.SetOrden(this.cboBusinessPartnerGroup, 0);
            this.controlsValidator.SetPuedeSerVacio(this.cboBusinessPartnerGroup, false);
            this.controlsValidator.SetRequerido(this.cboBusinessPartnerGroup, false);
            this.cboBusinessPartnerGroup.Size = new System.Drawing.Size(221, 21);
            this.cboBusinessPartnerGroup.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(9, 23);
            this.label11.Name = "label11";
            this.controlsValidator.SetOrden(this.label11, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label11, false);
            this.controlsValidator.SetRequerido(this.label11, false);
            this.label11.Size = new System.Drawing.Size(85, 13);
            this.label11.TabIndex = 25;
            this.label11.Text = "Grupo de cliente";
            // 
            // chkTaxDate
            // 
            this.chkTaxDate.AutoSize = true;
            this.chkTaxDate.Location = new System.Drawing.Point(26, 68);
            this.chkTaxDate.Name = "chkTaxDate";
            this.controlsValidator.SetOrden(this.chkTaxDate, 0);
            this.controlsValidator.SetPuedeSerVacio(this.chkTaxDate, false);
            this.controlsValidator.SetRequerido(this.chkTaxDate, false);
            this.chkTaxDate.Size = new System.Drawing.Size(156, 17);
            this.chkTaxDate.TabIndex = 0;
            this.chkTaxDate.Text = "Filtrar por fecha de emisión";
            this.chkTaxDate.UseVisualStyleBackColor = true;
            this.chkTaxDate.CheckedChanged += new System.EventHandler(this.chkTaxDate_CheckedChanged);
            // 
            // gbTaxDate
            // 
            this.gbTaxDate.Controls.Add(this.label1);
            this.gbTaxDate.Controls.Add(this.label4);
            this.gbTaxDate.Controls.Add(this.dtpToTaxDate);
            this.gbTaxDate.Controls.Add(this.dtpFromTaxDate);
            this.gbTaxDate.Controls.Add(this.label3);
            this.gbTaxDate.Enabled = false;
            this.gbTaxDate.Location = new System.Drawing.Point(23, 78);
            this.gbTaxDate.Name = "gbTaxDate";
            this.controlsValidator.SetOrden(this.gbTaxDate, 0);
            this.controlsValidator.SetPuedeSerVacio(this.gbTaxDate, false);
            this.controlsValidator.SetRequerido(this.gbTaxDate, false);
            this.gbTaxDate.Size = new System.Drawing.Size(408, 55);
            this.gbTaxDate.TabIndex = 0;
            this.gbTaxDate.TabStop = false;
            // 
            // gbDueDate
            // 
            this.gbDueDate.Controls.Add(this.label5);
            this.gbDueDate.Controls.Add(this.label7);
            this.gbDueDate.Controls.Add(this.dtpFromDocDueDate);
            this.gbDueDate.Controls.Add(this.dtpToDocDueDate);
            this.gbDueDate.Controls.Add(this.label6);
            this.gbDueDate.Enabled = false;
            this.gbDueDate.Location = new System.Drawing.Point(23, 146);
            this.gbDueDate.Name = "gbDueDate";
            this.controlsValidator.SetOrden(this.gbDueDate, 0);
            this.controlsValidator.SetPuedeSerVacio(this.gbDueDate, false);
            this.controlsValidator.SetRequerido(this.gbDueDate, false);
            this.gbDueDate.Size = new System.Drawing.Size(408, 55);
            this.gbDueDate.TabIndex = 1;
            this.gbDueDate.TabStop = false;
            // 
            // chkDueDate
            // 
            this.chkDueDate.AutoSize = true;
            this.chkDueDate.Location = new System.Drawing.Point(26, 136);
            this.chkDueDate.Name = "chkDueDate";
            this.controlsValidator.SetOrden(this.chkDueDate, 0);
            this.controlsValidator.SetPuedeSerVacio(this.chkDueDate, false);
            this.controlsValidator.SetRequerido(this.chkDueDate, false);
            this.chkDueDate.Size = new System.Drawing.Size(167, 17);
            this.chkDueDate.TabIndex = 0;
            this.chkDueDate.Text = "Filtrar por fecha de despacho";
            this.chkDueDate.UseVisualStyleBackColor = true;
            this.chkDueDate.CheckedChanged += new System.EventHandler(this.chkDueDate_CheckedChanged);
            // 
            // gbDelivery
            // 
            this.gbDelivery.Controls.Add(this.cboDeliveryType);
            this.gbDelivery.Controls.Add(this.label8);
            this.gbDelivery.Enabled = false;
            this.gbDelivery.Location = new System.Drawing.Point(23, 213);
            this.gbDelivery.Name = "gbDelivery";
            this.controlsValidator.SetOrden(this.gbDelivery, 0);
            this.controlsValidator.SetPuedeSerVacio(this.gbDelivery, false);
            this.controlsValidator.SetRequerido(this.gbDelivery, false);
            this.gbDelivery.Size = new System.Drawing.Size(408, 55);
            this.gbDelivery.TabIndex = 2;
            this.gbDelivery.TabStop = false;
            // 
            // chkDelivery
            // 
            this.chkDelivery.AutoSize = true;
            this.chkDelivery.Location = new System.Drawing.Point(26, 203);
            this.chkDelivery.Name = "chkDelivery";
            this.controlsValidator.SetOrden(this.chkDelivery, 0);
            this.controlsValidator.SetPuedeSerVacio(this.chkDelivery, false);
            this.controlsValidator.SetRequerido(this.chkDelivery, false);
            this.chkDelivery.Size = new System.Drawing.Size(158, 17);
            this.chkDelivery.TabIndex = 0;
            this.chkDelivery.Text = "Filtrar por tipo de despacho";
            this.chkDelivery.UseVisualStyleBackColor = true;
            this.chkDelivery.CheckedChanged += new System.EventHandler(this.chkDelivery_CheckedChanged);
            // 
            // gbPaymentTerm
            // 
            this.gbPaymentTerm.Controls.Add(this.cboPaymentTerms);
            this.gbPaymentTerm.Controls.Add(this.label9);
            this.gbPaymentTerm.Enabled = false;
            this.gbPaymentTerm.Location = new System.Drawing.Point(23, 281);
            this.gbPaymentTerm.Name = "gbPaymentTerm";
            this.controlsValidator.SetOrden(this.gbPaymentTerm, 0);
            this.controlsValidator.SetPuedeSerVacio(this.gbPaymentTerm, false);
            this.controlsValidator.SetRequerido(this.gbPaymentTerm, false);
            this.gbPaymentTerm.Size = new System.Drawing.Size(408, 55);
            this.gbPaymentTerm.TabIndex = 3;
            this.gbPaymentTerm.TabStop = false;
            // 
            // chkPaymentTerms
            // 
            this.chkPaymentTerms.AutoSize = true;
            this.chkPaymentTerms.Location = new System.Drawing.Point(26, 271);
            this.chkPaymentTerms.Name = "chkPaymentTerms";
            this.controlsValidator.SetOrden(this.chkPaymentTerms, 0);
            this.controlsValidator.SetPuedeSerVacio(this.chkPaymentTerms, false);
            this.controlsValidator.SetRequerido(this.chkPaymentTerms, false);
            this.chkPaymentTerms.Size = new System.Drawing.Size(166, 17);
            this.chkPaymentTerms.TabIndex = 0;
            this.chkPaymentTerms.Text = "Filtrar por condición de venta";
            this.chkPaymentTerms.UseVisualStyleBackColor = true;
            this.chkPaymentTerms.CheckedChanged += new System.EventHandler(this.chkPaymentTerms_CheckedChanged);
            // 
            // gbSalesEmp
            // 
            this.gbSalesEmp.Controls.Add(this.cboSalesEmployee);
            this.gbSalesEmp.Controls.Add(this.label10);
            this.gbSalesEmp.Enabled = false;
            this.gbSalesEmp.Location = new System.Drawing.Point(23, 349);
            this.gbSalesEmp.Name = "gbSalesEmp";
            this.controlsValidator.SetOrden(this.gbSalesEmp, 0);
            this.controlsValidator.SetPuedeSerVacio(this.gbSalesEmp, false);
            this.controlsValidator.SetRequerido(this.gbSalesEmp, false);
            this.gbSalesEmp.Size = new System.Drawing.Size(408, 55);
            this.gbSalesEmp.TabIndex = 4;
            this.gbSalesEmp.TabStop = false;
            // 
            // chkSalesEmployees
            // 
            this.chkSalesEmployees.AutoSize = true;
            this.chkSalesEmployees.Location = new System.Drawing.Point(26, 339);
            this.chkSalesEmployees.Name = "chkSalesEmployees";
            this.controlsValidator.SetOrden(this.chkSalesEmployees, 0);
            this.controlsValidator.SetPuedeSerVacio(this.chkSalesEmployees, false);
            this.controlsValidator.SetRequerido(this.chkSalesEmployees, false);
            this.chkSalesEmployees.Size = new System.Drawing.Size(168, 17);
            this.chkSalesEmployees.TabIndex = 0;
            this.chkSalesEmployees.Text = "Filtrar por empleado de venta";
            this.chkSalesEmployees.UseVisualStyleBackColor = true;
            this.chkSalesEmployees.CheckedChanged += new System.EventHandler(this.chkSalesEmployees_CheckedChanged);
            // 
            // chkBusinessPartnerGroup
            // 
            this.chkBusinessPartnerGroup.AutoSize = true;
            this.chkBusinessPartnerGroup.Location = new System.Drawing.Point(26, 409);
            this.chkBusinessPartnerGroup.Name = "chkBusinessPartnerGroup";
            this.controlsValidator.SetOrden(this.chkBusinessPartnerGroup, 0);
            this.controlsValidator.SetPuedeSerVacio(this.chkBusinessPartnerGroup, false);
            this.controlsValidator.SetRequerido(this.chkBusinessPartnerGroup, false);
            this.chkBusinessPartnerGroup.Size = new System.Drawing.Size(153, 17);
            this.chkBusinessPartnerGroup.TabIndex = 0;
            this.chkBusinessPartnerGroup.Text = "Filtrar por grupo de cliente";
            this.chkBusinessPartnerGroup.UseVisualStyleBackColor = true;
            this.chkBusinessPartnerGroup.CheckedChanged += new System.EventHandler(this.chkBusinessPartnerGroup_CheckedChanged);
            // 
            // gbBusinessPartnerGroup
            // 
            this.gbBusinessPartnerGroup.Controls.Add(this.cboBusinessPartnerGroup);
            this.gbBusinessPartnerGroup.Controls.Add(this.label11);
            this.gbBusinessPartnerGroup.Enabled = false;
            this.gbBusinessPartnerGroup.Location = new System.Drawing.Point(23, 419);
            this.gbBusinessPartnerGroup.Name = "gbBusinessPartnerGroup";
            this.controlsValidator.SetOrden(this.gbBusinessPartnerGroup, 0);
            this.controlsValidator.SetPuedeSerVacio(this.gbBusinessPartnerGroup, false);
            this.controlsValidator.SetRequerido(this.gbBusinessPartnerGroup, false);
            this.gbBusinessPartnerGroup.Size = new System.Drawing.Size(408, 55);
            this.gbBusinessPartnerGroup.TabIndex = 5;
            this.gbBusinessPartnerGroup.TabStop = false;
            // 
            // PnlFilters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "PnlFilters";
            this.controlsValidator.SetOrden(this, 0);
            this.controlsValidator.SetPuedeSerVacio(this, false);
            this.controlsValidator.SetRequerido(this, false);
            this.pnlFondo.ResumeLayout(false);
            this.pnlFondo.PerformLayout();
            this.gbTaxDate.ResumeLayout(false);
            this.gbTaxDate.PerformLayout();
            this.gbDueDate.ResumeLayout(false);
            this.gbDueDate.PerformLayout();
            this.gbDelivery.ResumeLayout(false);
            this.gbDelivery.PerformLayout();
            this.gbPaymentTerm.ResumeLayout(false);
            this.gbPaymentTerm.PerformLayout();
            this.gbSalesEmp.ResumeLayout(false);
            this.gbSalesEmp.PerformLayout();
            this.gbBusinessPartnerGroup.ResumeLayout(false);
            this.gbBusinessPartnerGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.DateTimePicker dtpToTaxDate;
        public System.Windows.Forms.DateTimePicker dtpFromTaxDate;
        public System.Windows.Forms.DateTimePicker dtpToDocDueDate;
        public System.Windows.Forms.DateTimePicker dtpFromDocDueDate;
        public System.Windows.Forms.ComboBox cboBusinessPartnerGroup;
        public System.Windows.Forms.ComboBox cboSalesEmployee;
        public System.Windows.Forms.ComboBox cboPaymentTerms;
        public System.Windows.Forms.ComboBox cboDeliveryType;
        private System.Windows.Forms.GroupBox gbBusinessPartnerGroup;
        private System.Windows.Forms.GroupBox gbSalesEmp;
        private System.Windows.Forms.GroupBox gbPaymentTerm;
        private System.Windows.Forms.GroupBox gbDelivery;
        private System.Windows.Forms.GroupBox gbDueDate;
        private System.Windows.Forms.GroupBox gbTaxDate;
        public System.Windows.Forms.CheckBox chkBusinessPartnerGroup;
        public System.Windows.Forms.CheckBox chkSalesEmployees;
        public System.Windows.Forms.CheckBox chkPaymentTerms;
        public System.Windows.Forms.CheckBox chkDelivery;
        public System.Windows.Forms.CheckBox chkDueDate;
        public System.Windows.Forms.CheckBox chkTaxDate;
    }
}
