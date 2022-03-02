
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HCO.Wizard.DTO;

namespace HCO.Wizard.Forms.Panels
{
    /// <summary>
    /// Panel para definir el nombre del escenario
    /// </summary>
    public partial class PnlChooseDocuments : HCO.Wizard.Forms.Panels.PnlBase
    {
        #region Declaraciones

        #endregion

        #region Constructor

        public PnlChooseDocuments()
        {
            InitializeComponent();
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
            bool isSelected = false;

            foreach (DataGridViewRow row in dgvOrders.Rows)
            {
                isSelected = isSelected || Convert.ToBoolean(row.Cells["Selected"].Value);
            }

            if (!isSelected)
                MessageUI.Show("Debe escoger al menos un pedido");

            return isSelected;
        }

        #endregion

        #region Propiedades

        #endregion

        #region Eventos

        private void chkSel_CheckedChanged(object sender, EventArgs e)
        {

            foreach (DataGridViewRow row in dgvOrders.Rows)
            {
                dgvOrders.Rows[row.Index].SetValues(chkSel.Checked);
            }
        }

        #endregion


    }
}
