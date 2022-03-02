using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SCGCE.BL.Objetos
{
    public class Exportaciones
    {
        public Encabezado Encabezado { get; set; }
        [XmlElement("Detalle")]
        public List<Detalle> Detalle { get; set; }
        [XmlElement("SubTotInfo")]
        public List<SubTotInfo> SubTotInfo { get; set; }
        [XmlElement("DscRcgGlobal")]
        public List<DscRcgGlobal> DscRcgGlobal { get; set; }
        [XmlElement("Referencia")]
        public List<Referencia> Referencia { get; set; }
        [XmlElement("Comisiones")]
        public List<Comisiones> Comisiones { get; set; }
    }
}
