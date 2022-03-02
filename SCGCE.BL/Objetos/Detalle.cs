using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SCGCE.BL.Objetos
{
    public class Detalle
    {
        public string NroLinDet { get; set; }
        public CdgItem CdgItem { get; set; }
        public string IndExe { get; set; }
        public Retenedor Retenedor { get; set; }
        public string NmbItem { get; set; }
        public string DscItem { get; set; }
        public string QtyRef { get; set; }
        public string UnmdRef { get; set; }
        public string PrcRef { get; set; }
        public string QtyItem { get; set; }
        public Subcantidad Subcantidad { get; set; }
        public string FchElabor { get; set; }
        public string FchVencim { get; set; }
        public string UnmdItem { get; set; }
        public string PrcItem { get; set; }
        public OtrMnda OtrMnda { get; set; }
        public string DescuentoPct { get; set; }
        public string DescuentoMonto { get; set; }
        public SubDscto SubDscto { get; set; }
        public string RecargoPct { get; set; }
        public string RecargoMonto { get; set; }
        public SubRecargo SubRecargo { get; set; }
        public string CodImpAdic { get; set; }
        public string MontoItem { get; set; }
    }
}
