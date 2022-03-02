
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HCO.Wizard.Forms.Panels
{
    /// <summary>
    /// Panel para definir el nombre del escenario
    /// </summary>
    public partial class PnlWelcome : HCO.Wizard.Forms.Panels.PnlBase
    {
        #region Declaraciones

        #endregion

        #region Constructor

        public PnlWelcome()
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
            return this.controlsValidator.Valida(true);
        }

        #endregion

        #region Propiedades

        #endregion

        #region Eventos

        #endregion

    }
}
