
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

    public partial class PnlProcess : HCO.Wizard.Forms.Panels.PnlBase
    {
        #region Declaraciones

        private List<IndicadorDocumentoDTO> _indicadores;

        #endregion

        #region Constructor

        public PnlProcess()
        {
            InitializeComponent();

            LoadMasterData();
        }

        #endregion

        #region Métodos privados

        public void LoadMasterData()
        {
            try
            {
                IndicadorDocumentoBL indicadorBL = new IndicadorDocumentoBL();

                _indicadores = indicadorBL.GetIndicadoresDocumentos();

                this.cboDocumentType.DataSource = _indicadores;
                this.cboDocumentType.DisplayMember = "Descripcion";
                this.cboDocumentType.ValueMember = "Formulario";

                this.cboCondVenta.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageUI.Error(ex.Message, ex);
            }
        }

        public void EnabledControls (bool enabled)
        {
            this.cboCondVenta.Enabled = enabled;
            this.cboDocumentType.Enabled = enabled;
            this.tcResults.Enabled = enabled;
        }

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

        #endregion

        #region Propiedades

        #endregion

        #region Eventos

        #endregion

    }
}
