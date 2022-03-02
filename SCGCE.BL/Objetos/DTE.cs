using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SCGCE.BL.Objetos
{
    public class DTE
    {
        [XmlElement("Documento")]
        public Documento Documento { get; set; }
        [XmlElement("Exportaciones")]
        public Exportaciones Exportaciones { get; set; }
    }
}
