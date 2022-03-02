using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SCGCE.BL.Objetos
{
    public class Emisor
    {
        public string RUTEmisor { get; set; }
        public string RznSoc { get; set; }
        public string GiroEmis { get; set; }
        public string Telefono { get; set; }
        public string CorreoEmisor { get; set; }

        [System.Xml.Serialization.XmlElement]
        public List<string> Acteco { get; set; }
        public GuiaExport GuiaExport { get; set; }
        public string Sucursal { get; set; }
        public string CdgSIISucur { get; set; }
        public string DirOrigen { get; set; }
        public string CmnaOrigen { get; set; }
        public string CiudadOrigen { get; set; }
        public string CdgVendedor { get; set; }
        public string IdAdicEmisor { get; set; }
    }
}
