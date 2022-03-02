using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCO.Wizard.BL;
using HCO.Wizard.DTO;
using HCO.SB1ServiceLayerSDK;
    
namespace Test
{

    class BusinessPartnerEx : HCO.SB1ServiceLayerSDK.SAPB1.BusinessPartner
    {
        public string U_SCGCE_GiroNeg { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {

            DIAPIHelper.Connect("hanades.scgsrvr.com:30015", "dst_HANADB", "SBODEMOCL", "hanades.scgsrvr.com:40000", null, null, "ncubillos", "B1Admin");

            InvoiceBL invoiceBL = new InvoiceBL();
            var invoices = invoiceBL.GetInvoices(new DateTime(2021, 09, 02), new DateTime(2021, 09, 02), null, null, null, null, null, null);
            invoices[0].Print = true;
            invoiceBL.printInvoices(invoices.Where(i => i.Print == true).ToList(), "133", "");

            DIAPIHelper.Disconnect();



            //foreach (string printerName in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            //{
            //    Console.WriteLine(printerName);
            //}

            //Console.ReadLine();

            //string file = @"C:\LogFacele\DocumentosElectronicos\Factura de deudores Folio 3220.pdf";
            //HCO.Wizard.Printer.PrinterHelper.PrintPDFWithAcrobat(file);

            //string printer = "Microsoft XPS Document Writer";

            //ImportCustomers();

            //ImportOrders();
        }


        private static void ImportOrders()
        {

            //Cliente que facilita el consumo de SAP Business One Service Layer
            ServiceLayerClient slClient = null;

            try
            {
                //URL del Service Layer
                string endpoint = "https://192.168.0.9:50000/b1s/v1";

                //Se instancia el cliente
                slClient = new ServiceLayerClient(endpoint);

                //Login al Service Layer
                slClient.Login("SBODEMOCL", "wflores", "B1Admin");

                string cardCode;


                for (int i = 1; i <= 20; i++)
                {
                    cardCode = "CP" + i.ToString().PadLeft(4, '0');


                    //Crea el encabezado del documento
                    HCO.SB1ServiceLayerSDK.SAPB1.Document order = new HCO.SB1ServiceLayerSDK.SAPB1.Document();
                    order.CardCode = cardCode;
                    order.DocDueDate = DateTime.Now;

                    //Agrega las líneas del documento
                    order.DocumentLines = new List<HCO.SB1ServiceLayerSDK.SAPB1.DocumentLine>();
                    order.DocumentLines.Add(new HCO.SB1ServiceLayerSDK.SAPB1.DocumentLine { ItemCode = "A00002", Quantity = 1, TaxCode = "IVA" });

                    string docEntryCreated = slClient.AddAndGetOnlyKey<HCO.SB1ServiceLayerSDK.SAPB1.Document>(order, HCO.SB1ServiceLayerSDK.SAPB1.BoObjectTypes.oOrders);

                }



            }
            catch (ServiceLayerException ex) //Si alguna operación falla se captura el error mediante la excepción ServiceLayerException
            {
                Console.WriteLine("Error code {0} - Message {1}", ex.ErrorCode, ex.Message);
            }
            finally
            {
                if (slClient != null)
                    slClient.Logout();  //Cierra la sesión
            }


        }

        private static void ImportCustomers ()
        {

            //Cliente que facilita el consumo de SAP Business One Service Layer
            ServiceLayerClient slClient = null;

            try
            {
                //URL del Service Layer
                string endpoint = "https://192.168.0.9:50000/b1s/v1";

                //Se instancia el cliente
                slClient = new ServiceLayerClient(endpoint);

                //Login al Service Layer
                slClient.Login("SBODEMOCL", "wflores", "B1Admin");

                string cardCode;
                string cardName;

                for (int i = 1; i <= 1000; i++)
                {
                    cardCode = "CP" + i.ToString().PadLeft(4, '0');
                    cardName = "Cliente " + cardCode;

                    BusinessPartnerEx businessPartner = new BusinessPartnerEx();
                    businessPartner.CardCode = cardCode;
                    businessPartner.CardName = cardName;
                    businessPartner.FederalTaxID = "11111111-1";
                    businessPartner.U_SCGCE_GiroNeg = "Cliente";

                    string cardCodeCreated = slClient.AddAndGetOnlyKey<HCO.SB1ServiceLayerSDK.SAPB1.BusinessPartner>(businessPartner,  HCO.SB1ServiceLayerSDK.SAPB1.BoObjectTypes.oBusinessPartners);

                }



            }
            catch (ServiceLayerException ex) //Si alguna operación falla se captura el error mediante la excepción ServiceLayerException
            {
                Console.WriteLine("Error code {0} - Message {1}", ex.ErrorCode, ex.Message);
            }
            finally
            {
                if (slClient != null)
                    slClient.Logout();  //Cierra la sesión
            }


        }

        private static void InvoiceDA_OrderLineReached(object sender, OrderLineReachedEventArgs e)
        {
            Console.WriteLine("DocEntry: {0}, DocNum: {1}, LineNum: {2}", e.DocEntry, e.DocNum, e.LineNum);
        }
    }
}
