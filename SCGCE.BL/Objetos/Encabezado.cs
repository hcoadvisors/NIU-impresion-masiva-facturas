using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCGCE.BL.Objetos
{
    public class Encabezado
    {
        public IdDoc IdDoc { get; set; }
        public Emisor Emisor { get; set; }
        public string RUTMandante { get; set; }
        public Receptor Receptor { get; set; }
        public string RUTSolicita { get; set; }
        public Transporte Transporte { get; set; }
        public Totales Totales { get; set; }
        public OtraMoneda OtraMoneda { get; set; }
    }
}
