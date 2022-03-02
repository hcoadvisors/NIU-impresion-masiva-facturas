using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SCGCE.BL.Objetos
{
    public class OtraMoneda
    {
        public string TpoMoneda { get; set; }
        public string TpoCambio { get; set; }
        public string MntNetoOtrMnda { get; set; }
        public string MntExeOtrMnda { get; set; }
        public string MntFaeCarneOtrMnda { get; set; }
        public string MntMargComOtrMnda { get; set; }
        public string IVAOtrMnda { get; set; }
        public ImpRetOtrMnda ImpRetOtrMnda { get; set; }
        public string IVANoRetOtrMnda { get; set; }
        public string MntTotOtrMnda { get; set; }
    }
}
