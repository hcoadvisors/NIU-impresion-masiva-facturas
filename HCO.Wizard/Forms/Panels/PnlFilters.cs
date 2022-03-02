
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HCO.Wizard.BL;
using HCO.Wizard.DTO;

namespace HCO.Wizard.Forms.Panels
{
    /// <summary>
    /// Panel para definir el nombre del escenario
    /// </summary>
    public partial class PnlFilters : HCO.Wizard.Forms.Panels.PnlBase
    {
        #region Declaraciones

        private List<SaleEmployeeDTO> _salesEmployees;
        private List<BusinessPartnerGroupDTO> _bpGroups;
        private List<ValidValueUFDTO> _DeliveryType;
        private List<ValidValueUFDTO> _PaymentTerms;

        #endregion

        #region Constructor

        public PnlFilters()
        {
            InitializeComponent();
            LoadMasterData();
        }

        #endregion

        #region Métodos privados

        #endregion

        #region Métodos públicos

        public override void SetFocus()
        {
            base.SetFocus();
     
        }

        public override bool CheckInputData()
        {
            base.CheckInputData();
            return this.controlsValidator.Valida(true);
        }

        public void LoadMasterData ()
        {
            try
            {
                SalesEmployeeBL salesEmployeeBL = new SalesEmployeeBL();
                BusinessPartnerGroupBL businessPartnerGroupBL = new BusinessPartnerGroupBL();
                ValidValueUFBL validValuesUFBL = new ValidValueUFBL();

                _salesEmployees = salesEmployeeBL.GetSalesEmployees().OrderBy(o => o.SlpName).ToList();
                _bpGroups = businessPartnerGroupBL.GetBusinessPartnerGroups().OrderBy(o => o.GroupName).ToList();
                _DeliveryType = validValuesUFBL.GetValidValues("TDespacho", "ORDR");
                _PaymentTerms = validValuesUFBL.GetValidValues("SCGCE_CondVent", "ORDR");

                this.cboSalesEmployee.DataSource = _salesEmployees;
                this.cboSalesEmployee.DisplayMember = "SlpName";
                this.cboSalesEmployee.ValueMember = "SlpCode";

                this.cboBusinessPartnerGroup.DataSource = _bpGroups;
                this.cboBusinessPartnerGroup.DisplayMember = "GroupName";
                this.cboBusinessPartnerGroup.ValueMember = "GroupCode";

                this.cboDeliveryType.DataSource = _DeliveryType;
                this.cboDeliveryType.DisplayMember = "Descr";
                this.cboDeliveryType.ValueMember = "FldValue";

                this.cboPaymentTerms.DataSource = _PaymentTerms;
                this.cboPaymentTerms.DisplayMember = "Descr";
                this.cboPaymentTerms.ValueMember = "FldValue";
            }
            catch (Exception ex)
            {
                MessageUI.Error(ex.Message, ex);
            }
        }

        #endregion

        #region Propiedades

        #endregion

        #region Eventos

        private void chkTaxDate_CheckedChanged(object sender, EventArgs e)
        {
            this.gbTaxDate.Enabled = chkTaxDate.Checked;
        }

        private void chkDueDate_CheckedChanged(object sender, EventArgs e)
        {
            this.gbDueDate.Enabled = chkDueDate.Checked;
        }

        private void chkDelivery_CheckedChanged(object sender, EventArgs e)
        {
            this.gbDelivery.Enabled = chkDelivery.Checked;
        }

        private void chkPaymentTerms_CheckedChanged(object sender, EventArgs e)
        {
            this.gbPaymentTerm.Enabled = chkPaymentTerms.Checked;
        }

        private void chkSalesEmployees_CheckedChanged(object sender, EventArgs e)
        {
            this.gbSalesEmp.Enabled = chkSalesEmployees.Checked;
        }

        private void chkBusinessPartnerGroup_CheckedChanged(object sender, EventArgs e)
        {
            this.gbBusinessPartnerGroup.Enabled = chkBusinessPartnerGroup.Checked;
        }

        #endregion

    }
}
