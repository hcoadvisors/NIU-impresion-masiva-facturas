using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCGCE.BL.Objetos
{
    public class Receptor
    {
        public string RUTRecep { get; set; }
        public string CdgIntRecep { get; set; }
        public string RznSocRecep { get; set; }
        public Extranjero Extranjero { get; set; }
        public string GiroRecep { get; set; }
        public string Contacto { get; set; }
        public string CorreoRecep { get; set; }
        public string DirRecep { get; set; }
        public string CmnaRecep { get; set; }
        public string CiudadRecep { get; set; }
        public string DirPostal { get; set; }
        public string CmnaPostal { get; set; }
        public string CiudadPostal { get; set; }
    }
}
