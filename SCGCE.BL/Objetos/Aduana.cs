using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCGCE.BL.Objetos
{
    public class Aduana
    {
        public string CodModVenta { get; set; }
        public string CodClauVenta { get; set; }
        public string TotClauVenta { get; set; }
        public string CodViaTransp { get; set; }
        public string NombreTransp { get; set; }
        public string RUTCiaTransp { get; set; }
        public string NomCiaTransp { get; set; }
        public string IdAdicTransp { get; set; }
        public string Booking { get; set; }
        public string Operador { get; set; }
        public string CodPtoEmbarque { get; set; }
        public string IdAdicPtoEmb { get; set; }
        public string CodPtoDesemb { get; set; }
        public string IdAdicPtoDesemb { get; set; }
        public string Tara { get; set; }
        public string CodUnidMedTara { get; set; }
        public string PesoBruto { get; set; }
        public string CodUnidPesoBruto { get; set; }
        public string PesoNeto { get; set; }
        public string CodUnidPesoNeto { get; set; }
        public string TotItems { get; set; }
        public string TotBultos { get; set; }
        public TipoBultos TipoBultos { get; set; }
        public string MntFlete { get; set; }
        public string MntSeguro { get; set; }
        public string CodPaisRecep { get; set; }
        public string CodPaisDestin { get; set; }
    }
}
