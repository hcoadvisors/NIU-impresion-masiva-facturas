using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SCGCE.BL.Objetos
{
    public class Comisiones
    {
        public string NroLinCom { get; set; }
        public string TipoMovim { get; set; }
        public string Glosa { get; set; }
        public string TasaComision { get; set; }
        public string ValComNeto { get; set; }
        public string ValComExe { get; set; }
        public string ValComIVA { get; set; }
    }
}
