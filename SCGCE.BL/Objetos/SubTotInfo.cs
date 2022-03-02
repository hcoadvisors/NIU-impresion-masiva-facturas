using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SCGCE.BL.Objetos
{
    public class SubTotInfo
    {
        public string NroSTI { get; set; }
        public string GlosaSTI { get; set; }
        public string OrdenSTI { get; set; }
        public string SubTotNetoSTI { get; set; }
        public string SubTotIVASTI { get; set; }
        public string SubTotAdicSTI { get; set; }
        public string SubTotExeSTI { get; set; }
        public string ValSubtotSTI { get; set; }
        public string LineasDeta { get; set; }
    }
}
