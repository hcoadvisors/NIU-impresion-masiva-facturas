using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCO.Wizard.DTO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SAPbobsCOM;
using Spire.Pdf;

namespace HCO.Wizard.BL
{
    public class InvoiceBL
    {

        public void CreateInvoices(List<OrderDTO> orders, string form, string indicator, string codVenta, DateTime docDate, ref List<OrderDTO> resultOfProcess, ref List<InvoiceDTO> invoicesCreated)
        {

            EmisionDocumentoBL emisionDocumentoBL = new EmisionDocumentoBL();
            string message = string.Empty;

            //Obtiene un listado  de clientes
            var businessPartnersQuery = (from o in orders
                                         where o.Selected == true
                                         group o by new { o.DocEntry, o.CardCode, o.DocDueDate } into g
                                         select new { CardCode = g.Key.CardCode, DocEntry = g.Key.DocEntry, DocDueDate = g.Key.DocDueDate }).ToList();


            Documents invoice = null;
            Recordset recordset = (Recordset)DIAPIHelper.Sb1Company.GetBusinessObject(BoObjectTypes.BoRecordset); //Para consultar el DocNum de la factura creada

            //Inicia el proceso de facturación agrupando por cliente
            foreach (var businessPartner in  businessPartnersQuery)
            {
                string documentTypeName = string.Empty;

                //Setea  el encabezado de la factura para un cliente especifico
                invoice = (Documents)DIAPIHelper.Sb1Company.GetBusinessObject(BoObjectTypes.oInvoices);
                invoice.CardCode = businessPartner.CardCode;
                invoice.DocumentSubType = GetDocumentSubType(form, out documentTypeName);
                invoice.DocDate = docDate;
                invoice.TaxDate = docDate;
                invoice.Indicator = indicator;
                invoice.UserFields.Fields.Item("U_SCGCE_CondVent").Value = codVenta;
                invoice.UserFields.Fields.Item("U_HCO_ShipDate").Value = businessPartner.DocDueDate;

                //Consulta las líneas de los pedidos para un cliente especifico
                var invoiceLines = from o in orders
                                   where o.CardCode == businessPartner.CardCode && o.Selected == true && o.DocEntry == businessPartner.DocEntry
                                   select o;

                //Setea las líneas de los pedidos en la factura del cliente
                bool firstLine = true;
                foreach (var line in invoiceLines)
                {
                    resultOfProcess.Add(line);

                    if (!firstLine)
                        invoice.Lines.Add();

                    invoice.Lines.BaseEntry = line.DocEntry;
                    invoice.Lines.BaseLine = line.LineNum;
                    invoice.Lines.BaseType = (int)SAPbobsCOM.BoObjectTypes.oOrders;


                    if (form == "65302" ||  form == "65305")
                        invoice.Lines.UserFields.Fields.Item("U_SCGCE_IndExe").Value = "1";

                    firstLine = false;

                    //Lanza enveto de línea de pedido procesada
                    OrderLineReachedEventArgs args = new OrderLineReachedEventArgs();
                    args.DocEntry = line.DocEntry;
                    args.DocNum = line.DocNum;
                    args.LineNum = line.LineNum;

                    OnOrderLineReached(args);
                }

                int result = invoice.Add();

                if (result != 0)
                {
                    message = DIAPIHelper.Sb1Company.GetLastErrorCode() + " - " + DIAPIHelper.Sb1Company.GetLastErrorDescription();
                    SetResult(ref resultOfProcess, businessPartner.CardCode, string.Empty, string.Empty, string.Empty, string.Empty, 0, string.Empty, documentTypeName, false, "No se creo el documento causa: " +  message, businessPartner.DocEntry);
                }
                else
                {
                    string invoiceDocEntryCreated = DIAPIHelper.Sb1Company.GetNewObjectKey();
                    string invoiceDocNumCreated = "";
                    string folioPref = "";
                    string folioNum = "";
                    int estadoAut = 0;
                    string u_scgce_des = "";

                    message = emisionDocumentoBL.Emitir(form, invoiceDocEntryCreated, "");

                    recordset.DoQuery("SELECT \"DocNum\", \"FolioPref\", \"FolioNum\", \"U_SCGCE_EstadoAut\", \"U_SCGCE_Des\" FROM \"OINV\" WHERE \"DocEntry\" = " + invoiceDocEntryCreated);

                    invoiceDocNumCreated = recordset.Fields.Item("DocNum").Value.ToString();
                    folioPref = recordset.Fields.Item("FolioPref").Value.ToString();
                    folioNum = recordset.Fields.Item("FolioNum").Value.ToString();
                    estadoAut = (int)recordset.Fields.Item("U_SCGCE_EstadoAut").Value;
                    u_scgce_des  = recordset.Fields.Item("U_SCGCE_Des").Value.ToString();

                    SetResult(ref resultOfProcess, businessPartner.CardCode, invoiceDocEntryCreated, invoiceDocNumCreated, folioPref, folioNum, estadoAut, u_scgce_des,  documentTypeName, true, "Documento creado exitosamente", businessPartner.DocEntry);

                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(invoice);
                invoice = null;
            }

            SetInvoicesCreated(ref resultOfProcess, ref invoicesCreated);

            System.Runtime.InteropServices.Marshal.ReleaseComObject(recordset);
            recordset = null;

        }

        private void SetInvoicesCreated (ref List<OrderDTO> resultOfProcess, ref List<InvoiceDTO> invoices)
        {
            invoices = new List<InvoiceDTO>();

            var invoicesQuery =
                    (from o in resultOfProcess
                     where o.InvoiceCreated == true
                    group o by new
                    {
                        o.InvoiceDocEntryCreated,
                        o.InvoiceDocNumCreated,
                        o.FolioPref,
                        o.FolioNum,
                        o.InvoiceType,
                        o.CardCode,
                        o.CardName,
                        o.EstadoAut,
                        o.MensajeFacElectronica,
                    } into g
                    select new 
                    {
                        InvoiceDocEntryCreated = g.Key.InvoiceDocEntryCreated,
                        InvoiceDocNumCreated = g.Key.InvoiceDocNumCreated,
                        InvoiceType = g.Key.InvoiceType,
                        FolioPref = g.Key.FolioPref,
                        FolioNum = g.Key.FolioNum,
                        CardCode = g.Key.CardCode,
                        CardName = g.Key.CardName,
                        EstadoAut = g.Key.EstadoAut,
                        MensajeFacElectronica = g.Key.MensajeFacElectronica
                    }).ToList();

            foreach (var invoice in invoicesQuery)
            {
                invoices.Add(new InvoiceDTO
                {
                    DocEntry = int.Parse(invoice.InvoiceDocEntryCreated),
                    DocNum = int.Parse(invoice.InvoiceDocNumCreated),
                    FolioPref = invoice.FolioPref,
                    FolionNum = invoice.FolioNum,
                    CardCode = invoice.CardCode,
                    CardName = invoice.CardName,
                    EstadoAut = invoice.EstadoAut,
                    MensajeFacElectronica = invoice.MensajeFacElectronica,
                    Print = false
                });
            }
        }

        private void SetResult(ref List<OrderDTO> orders, string cardCode, string invoiceDocEntryCreated, string invoiceDocNumCreated, string folioPref, string folioNum, int estadoAut, string u_scgce_des, string documentTypeName, bool invoiceCreated, string message, int DocEntry)
        {

            var query = orders.Where(t => t.CardCode == cardCode && t.DocEntry == DocEntry).ToList();

            foreach (var order in query)
            {
                order.InvoiceDocEntryCreated = invoiceDocEntryCreated;
                order.InvoiceDocNumCreated = invoiceDocNumCreated;
                order.InvoiceType = documentTypeName;
                order.InvoiceCreated = invoiceCreated;
                order.FolioPref = folioPref;
                order.FolioNum = folioNum;
                order.Message = message;
                order.EstadoAut = estadoAut;
                order.MensajeFacElectronica = u_scgce_des;
            }

        }

        public SAPbobsCOM.BoDocumentSubType GetDocumentSubType(string form, out string description )
        {
            SAPbobsCOM.BoDocumentSubType documentSubType;

            switch (form)
            {
                case "65302": //65302	Factura exenta de deudores	IE
                    documentSubType = BoDocumentSubType.bod_InvoiceExempt;
                    description = "Facturas Exenta Electrónica";
                    break;

                case "65304": //65304	Boleta	IB
                    documentSubType = BoDocumentSubType.bod_Bill;
                    description = "Boleta Electrónica";
                    break;

                case "65305": //65305	Boleta exenta	EB
                    documentSubType = BoDocumentSubType.bod_ExemptBill;
                    description = "Boleta Exenta Electrónica";
                    break;

                default: // 133	Factura de deudores
                    documentSubType = BoDocumentSubType.bod_None;
                    description = "Facturas Electrónica";
                    break;
            }

            return documentSubType;
        }

        public void printInvoices(List<InvoiceDTO> invoices, string form, string print)
        {
            new LogTxt("Iniciando Impresion");

            Recordset recordset = (Recordset)DIAPIHelper.Sb1Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            var strConsulta = "SELECT \"U_IdNum\",\"U_RutaDTE\",\"U_cedibles\",\"U_CopiasCed\" FROM \"@SCGCE_CONF\" ";
            recordset.DoQuery(strConsulta);
            var RutaDTE = recordset.Fields.Item("U_RutaDTE").Value.ToString();

            new LogTxt("Ruta de tabla de configuracion" + RutaDTE);

            string path = (string.IsNullOrEmpty(RutaDTE) ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString() : RutaDTE) + "\\DocumentosElectronicos\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            new LogTxt("Ruta final" + path);

            string archivoFinal = path + "Impresion " + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
            List<string> archivos = new List<string>();

            for (int i = 0; i < invoices.Count; i++)
            {
                new LogTxt("Imprimiendo primer factura");

                string description = "";

                GetDocumentSubType(string.IsNullOrEmpty(form) ? invoices[i].Form : form, out description);
                string archivo = path + description + " Folio " + invoices[i].FolionNum + ".pdf";
                new LogTxt("Archivo " + archivo);
                string[] files = Directory.GetFiles(path);
                if (!files.Contains(archivo))
                {
                    new LogTxt("No existe el archivo " + archivo);
                    EmisionDocumentoBL emisionDocumentoBL = new EmisionDocumentoBL();
                    emisionDocumentoBL.ImprimirPDF(string.IsNullOrEmpty(form) ? invoices[i].Form : form, invoices[i].DocEntry.ToString(), invoices[i].FolionNum, "");
                    new LogTxt("Archivo generado");
                }

                new LogTxt("Intanciando impresión");

                archivos.Add(archivo);

                new LogTxt("Archivo impreso");

                OrderLineReachedEventArgs args = new OrderLineReachedEventArgs();
                args.DocEntry = invoices[i].DocEntry;
                args.DocNum = invoices[i].DocNum;
                args.Folio = invoices[i].FolionNum;
                args.Print = true;
                OnOrderLineReached(args);
            }

            try
            {
                //PdfDocument pdfdocument = new PdfDocument();
                //new LogTxt("Instancia objeto PDF");
                //pdfdocument.LoadFromFile(archivo);
                //new LogTxt("Carga archivo");
                //if (!string.IsNullOrEmpty(print))
                //{
                //    pdfdocument.PrintDocument.PrinterSettings.PrinterName = print;
                //    new LogTxt("Asigna impresora " + print);

                //}
                //pdfdocument.PrintDocument.Print();
                //pdfdocument.Dispose();

                Merge(archivos, archivoFinal);

                PdfPrintingNet.PdfPrint pdfPrint = new PdfPrintingNet.PdfPrint("demoCompany", "demoKey");
                pdfPrint.PrinterName = print;
                var status = pdfPrint.Print(archivoFinal);
                if (status != PdfPrintingNet.PdfPrint.Status.OK)
                    new LogTxt("Error imprimiendo archivo " + archivoFinal + " Impresora " + print + " Error " + status.ToString());
            }
            catch (Exception ex)
            {
                new LogTxt("Error imprimiendo archivo " + archivoFinal + " Impresora " + print + " Error " + ex.Message);
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(recordset);
            recordset = null;
            GC.Collect();
        }

        public static void Merge(List<String> InFiles, String OutFile)
        {

            using (FileStream stream = new FileStream(OutFile, FileMode.Create))
            using (Document doc = new Document())
            using (PdfCopy pdf = new PdfCopy(doc, stream))
            {
                doc.Open();

                PdfReader reader = null;
                PdfImportedPage page = null;

                //fixed typo
                InFiles.ForEach(file =>
                {
                    reader = new PdfReader(file);

                    for (int i = 0; i < reader.NumberOfPages; i++)
                    {
                        page = pdf.GetImportedPage(reader, i + 1);
                        pdf.AddPage(page);
                    }

                    pdf.FreeReader(reader);
                    reader.Close();
                });
            }
        }

        protected virtual void OnOrderLineReached(OrderLineReachedEventArgs e)
        {
            EventHandler<OrderLineReachedEventArgs> handler = OrderLineReached;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<OrderLineReachedEventArgs> OrderLineReached;

        public List<InvoiceDTO> GetInvoices(DateTime? taxDateFrom, DateTime? taxDateTo, DateTime? docDueDateFrom, DateTime? docDueDateTo, string groupNum, string slpCode, string groupCode, string deliveryType)
        {
            string filterTaxDate = taxDateFrom.HasValue ? " AND T0.\"TaxDate\" BETWEEN '" + taxDateFrom.Value.ToString("yyyyMMdd") + "' AND '" + taxDateTo.Value.ToString("yyyyMMdd") + "'" : string.Empty;
            string filterDocDueDate = docDueDateFrom.HasValue ? " AND T0.\"U_HCO_ShipDate\" BETWEEN '" + docDueDateFrom.Value.ToString("yyyyMMdd") + "' AND '" + docDueDateTo.Value.ToString("yyyyMMdd") + "'" : string.Empty;
            string filterGroupNum = !string.IsNullOrEmpty(groupNum) ? " AND T0.\"GroupNum\" = " + groupNum : string.Empty;
            string filterSlpCode = !string.IsNullOrEmpty(slpCode) ? " AND T0.\"SlpCode\" = " + slpCode : string.Empty;
            string filterGroupCode = !string.IsNullOrEmpty(groupCode) ? " AND T2.\"GroupCode\" = " + groupCode : string.Empty;
            string filterDeliveryType = !string.IsNullOrEmpty(deliveryType) ? " AND T0.\"U_TDespacho\" = '" + deliveryType + "'" : string.Empty;

            string filter = filterTaxDate + filterDocDueDate + filterGroupNum + filterSlpCode + filterGroupCode + filterDeliveryType;

            filter = " WHERE T0.\"U_SCGCE_EstadoAut\" = 1 AND T0.\"FolioNum\" IS NOT NULL " + filter;

            List<InvoiceDTO> invoices = new List<InvoiceDTO>();
            string query = "SELECT T0.\"DocEntry\", " +
                    "	   T0.\"DocNum\", " +
                    "	   T0.\"DocStatus\", " +
                    "	   T0.\"TaxDate\", " +
                    "	   T0.\"DocDueDate\", " +
                    "	   T2.\"LicTradNum\", " +
                    "	   T2.\"CardCode\", " +
                    "	   T2.\"CardName\", " +
                    "	   T2.\"GroupCode\", " +
                    "	   T5.\"GroupName\", " +
                    "	   T0.\"GroupNum\", " +
                    "	   T3.\"PymntGroup\", " +
                    "	   T0.\"SlpCode\",  " +
                    "	   T4.\"SlpName\", " +
                    "	   T0.\"FolioNum\", " +
                    "	   T0.\"DocTotal\", " +
                    "	   CASE T0.\"DocSubType\"" +
                    "           WHEN  '--' THEN '133'" +
                    "           WHEN  'IE' THEN '65302'" +
                    "           WHEN  'IB' THEN '65304'" +
                    "           WHEN  'EB' THEN '65305' END \"Form\"" +
                    "FROM OINV T0 " +
                    "INNER JOIN OCRD T2 ON T0.\"CardCode\" = T2.\"CardCode\" " +
                    "INNER JOIN OCTG T3 ON T0.\"GroupNum\" = T3.\"GroupNum\" " +
                    "INNER JOIN OSLP T4 ON T0.\"SlpCode\" = T4.\"SlpCode\" " +
                    "INNER JOIN OCRG T5 ON T2.\"GroupCode\" = T5.\"GroupCode\" " + filter;

            Recordset recordset = (Recordset)DIAPIHelper.Sb1Company.GetBusinessObject(BoObjectTypes.BoRecordset);

            recordset.DoQuery(query);


            while (!recordset.EoF)
            {
                InvoiceDTO invoiceDTO = new InvoiceDTO();

                invoiceDTO.DocEntry = (int)recordset.Fields.Item("DocEntry").Value;
                invoiceDTO.DocNum = (int)recordset.Fields.Item("DocNum").Value;
                invoiceDTO.DocStatus = recordset.Fields.Item("DocStatus").Value.ToString();
                invoiceDTO.TaxDate = (DateTime)recordset.Fields.Item("TaxDate").Value;
                invoiceDTO.DocDueDate = (DateTime)recordset.Fields.Item("DocDueDate").Value;
                invoiceDTO.LicTradNum = recordset.Fields.Item("LicTradNum").Value.ToString();
                invoiceDTO.CardCode = recordset.Fields.Item("CardCode").Value.ToString();
                invoiceDTO.CardName = recordset.Fields.Item("CardName").Value.ToString();
                invoiceDTO.GroupCode = (int)recordset.Fields.Item("GroupCode").Value;
                invoiceDTO.GroupName = recordset.Fields.Item("GroupName").Value.ToString();
                invoiceDTO.GroupNum = (int)recordset.Fields.Item("GroupNum").Value;
                invoiceDTO.PymntGroup = recordset.Fields.Item("PymntGroup").Value.ToString();
                invoiceDTO.SlpCode = (int)recordset.Fields.Item("SlpCode").Value;
                invoiceDTO.SlpName = recordset.Fields.Item("SlpName").Value.ToString();
                invoiceDTO.FolionNum = recordset.Fields.Item("FolioNum").Value.ToString();
                invoiceDTO.DocTotal = double.Parse(recordset.Fields.Item("DocTotal").Value.ToString());
                invoiceDTO.Form = recordset.Fields.Item("Form").Value.ToString();
                invoiceDTO.EstadoAut = 1;
                invoiceDTO.Print = false;

                invoices.Add(invoiceDTO);

                recordset.MoveNext();
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(recordset);
            recordset = null;

            return invoices;

        }
    }

    public class OrderLineReachedEventArgs : EventArgs
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public int LineNum { get; set; }
        public string Folio { get; set; }
        public bool Print { get; set; }

    }

}
