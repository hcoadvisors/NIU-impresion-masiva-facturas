using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SCGCE.BL.Objetos
{
    public class Totales
    {
        public string TpoMoneda { get; set; }
        public string MntNeto { get; set; }
        public string MntExe { get; set; }
        public string MntBase { get; set; }
        public string MntMargenCom { get; set; }
        public string TasaIVA { get; set; }
        public string IVA { get; set; }
        public string IVAProp { get; set; }
        public string IVATerc { get; set; }
        public ImptoReten ImptoReten { get; set; }
        public string IVANoRet { get; set; }
        public string CredEC { get; set; }
        public string GrntDep { get; set; }
        public Comisiones Comisiones { get; set; }
        public string MntTotal { get; set; }
        public string MontoNF { get; set; }
        public string MontoPeriodo { get; set; }
        public string SaldoAnterior { get; set; }
        public string VlrPagar { get; set; }
    }
}
