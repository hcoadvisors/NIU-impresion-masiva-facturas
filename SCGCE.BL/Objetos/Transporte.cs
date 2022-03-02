using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCGCE.BL.Objetos
{
    public class Transporte
    {
        public string Patente { get; set; }
        public string RUTTrans { get; set; }
        public Chofer Chofer { get; set; }
        public string DirDest { get; set; }
        public string CmnaDest { get; set; }
        public string CiudadDest { get; set; }
        public Aduana Aduana { get; set; }
    }
}
