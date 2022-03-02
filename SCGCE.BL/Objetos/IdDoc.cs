using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SCGCE.BL.Objetos
{
    public class IdDoc
    {
        public string TipoDTE { get; set; }
        public string Folio { get; set; }
        public string FchEmis { get; set; }
        public string IndNoRebaja { get; set; }
        public string TipoDespacho { get; set; }
        public string IndTraslado { get; set; }
        public string TpoImpresion { get; set; }
        public string IndServicio { get; set; }
        public string IndMntNeto { get; set; }
        public string MntBruto { get; set; }
        public string FmaPago { get; set; }
        public string FmaPagExp { get; set; }
        public string FchCancel { get; set; }
        public string MntCancel { get; set; }
        public string SaldoInsol { get; set; }
        public MntPagos MntPagos { get; set; }
        public string PeriodoDesde { get; set; }
        public string PeriodoHasta { get; set; }
        public string MedioPago { get; set; }
        public string TpoCtaPago { get; set; }
        public string NumCtaPago { get; set; }
        public string BcoPago { get; set; }
        public string TermPagoCdg { get; set; }
        public string TermPagoGlosa { get; set; }
        public string TermPagoDias { get; set; }
        public string FchVenc { get; set; }
    }
}
