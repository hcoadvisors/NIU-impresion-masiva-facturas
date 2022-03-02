using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SCGCE.BL.Objetos
{
    public class Referencia
    {
        public string NroLinRef { get; set; }
        public string TpoDocRef { get; set; }
        public string IndGlobal { get; set; }
        public string FolioRef { get; set; }
        public string RUTOtr { get; set; }
        public string FchRef { get; set; }
        public string CodRef { get; set; }
        public string RazonRef { get; set; }
    }
}
