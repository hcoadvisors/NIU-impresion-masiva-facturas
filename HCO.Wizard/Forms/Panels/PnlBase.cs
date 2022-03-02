
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
    /// Plantilla base para el asistente de configuración de escenarios
    /// </summary>
    public partial class PnlBase : UserControl
    {
        #region Declaraciones

        #endregion

        #region Constructor

        public PnlBase()
        {
            InitializeComponent();
        }



        #endregion

        #region Métodos privados

        #endregion

        #region Métodos públicos

        public virtual void SetFocus()
        {
          
        }

        public virtual bool CheckInputData()
        {
            return true;
        }

        #endregion

        #region Propiedades

        public int StepNumber { get; set; }

        #endregion

        #region Eventos

        #endregion

    }
}
