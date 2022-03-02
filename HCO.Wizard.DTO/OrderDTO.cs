using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCO.Wizard.DTO
{
    public class OrderDTO
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

        public int LineNum { get; set; }

        public string ItemCode { get; set; }

        public string Dscription { get; set; }

        public string LineStatus { get; set; }

        public double Quantity { get; set; }

        public double OpenQty { get; set; }


        public bool InvoiceCreated { get; set; }

        public string InvoiceDocNumCreated { get; set; }

        public string InvoiceDocEntryCreated { get; set; }

        public string FolioPref { get; set; }

        public string FolioNum { get; set; }

        public string InvoiceType { get; set; }

        public string  Message { get; set; }

        public bool Selected { get; set; }

        public int EstadoAut { get; set; }

        public string MensajeFacElectronica { get; set; }


    }
}
