using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCO.Wizard.DTO
{
    public class InvoiceDTO
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string DocStatus { get; set; }
        public DateTime TaxDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string LicTradNum { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public int GroupCode { get; set; }
        public string GroupName { get; set; }
        public int GroupNum { get; set; }
        public string PymntGroup { get; set; }
        public int SlpCode { get; set; }
        public string SlpName { get; set; }
        public double DocTotal{ get; set; }

        public string FolioPref { get; set; }
        public string FolionNum { get; set; }
        public bool Print { get; set; }
        public int EstadoAut { get; set; }
        public string MensajeFacElectronica { get; set; }
        public string Form { get; set; }
    }
}
