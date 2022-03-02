using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCO.Wizard.DTO
{
    public class IndicadorDocumentoDTO
    {
        //SELECT "U_IdSerS", "U_Desc", "U_Term" FROM "@SCGCE_CONFS" WHERE "U_Term" IN ('133','65302','65304','65305')

        public string Descripcion { get; set; }

        public string Indicador { get; set; }

        public string Formulario { get; set; }
    }
}
