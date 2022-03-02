using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbobsCOM;
using SCGCE.BL;

namespace HCO.Wizard.BL
{
    public class EmisionDocumentoBL
    {

        private string GetQuery(string form, string docEntry)
        {
            string query;

            switch (form)
            {
                case "65302": //65302	Factura exenta de deudores	IE
                    query = " SELECT '02' AS \"TipoDocumento\", 'Factura Exenta Electrónica' AS \"DesDocumento\", \"DocEntry\", \"DocNum\",  " +
                                " \"FolioNum\", \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
                                " FROM \"OINV\" WHERE  \"U_SCGCE_EstadoAut\" = 0 AND \"Indicator\" = '34' And \"DocSubType\" = 'IE' AND(\"FolioNum\" is null OR \"FolioNum\" =0) " +
                                " AND \"DocEntry\" = " + docEntry;
                    break;

                case "65304": //65304	Boleta	IB
                    query = " SELECT '03' AS \"TipoDocumento\", 'Boleta Electrónica' AS \"DesDocumento\", \"DocEntry\", \"DocNum\", \"FolioNum\", " +
                                " \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
                                " FROM \"OINV\" WHERE \"U_SCGCE_EstadoAut\" = 0 AND \"Indicator\" = '39' And \"DocSubType\" = 'IB' AND (\"FolioNum\" is null OR \"FolioNum\" =0) " +
                                " AND \"DocEntry\" = " + docEntry;
                    break;

                case "65305": //65305	Boleta exenta	EB
                    query = " SELECT '04' AS \"TipoDocumento\", 'Boleta Exenta Electrónica' AS \"DesDocumento\", \"DocEntry\", \"DocNum\", \"FolioNum\", " +
                                " \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
                                " FROM \"OINV\" WHERE \"U_SCGCE_EstadoAut\" = 0 AND \"Indicator\" = '41' And \"DocSubType\" = 'EB' AND (\"FolioNum\" is null OR \"FolioNum\" =0) " +
                                " AND \"DocEntry\" = " + docEntry;
                    break;

                default: // 133	Factura de deudores
                    query = "SELECT '01' AS \"TipoDocumento\", 'Factura Electrónica' AS \"DesDocumento\", \"DocEntry\", \"DocNum\", " +
                                " \"FolioNum\", \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR) AS \"U_SCGCE_Des\" " +
                                " FROM \"OINV\" WHERE  \"U_SCGCE_EstadoAut\" = 0 AND \"Indicator\" = '33' And \"DocSubType\" = '--' And \"isIns\" = 'N' AND (\"FolioNum\" is null OR \"FolioNum\" =0) " +
                                " AND \"DocEntry\" = " + docEntry;
                    break;
            }

            return query;

        }


        public string Emitir(string form, string docEntry, string rutaArchivos)
        {
            Recordset recordset = null;
            string message = string.Empty;
            string query = GetQuery(form, docEntry);

            recordset = (Recordset)DIAPIHelper.Sb1Company.GetBusinessObject(BoObjectTypes.BoRecordset);

            recordset.DoQuery(query);

            try
            {
                string tipoDocumento = recordset.Fields.Item("TipoDocumento").Value.ToString();
                string desDocumento = recordset.Fields.Item("DesDocumento").Value.ToString();
                //int docEntry = (int)recordset.Fields.Item("DocEntry").Value;
                int docNum = (int)recordset.Fields.Item("DocNum").Value;
                string formTypeEx;
                string indicador;
                string almacen;

                BLCEEmision oBLCEEmision = new BLCEEmision(rutaArchivos);
                BLCEGeneral oBLCEGeneral = new BLCEGeneral(DIAPIHelper.Sb1Company);
                string objType = string.Empty;

                formTypeEx = oBLCEEmision.ObtenerFormTypeEx(tipoDocumento, desDocumento);

                oBLCEGeneral.GetFacturaInformacionFE(docEntry.ToString(), out indicador, out almacen, formTypeEx, out objType);

                string create = oBLCEEmision.EnviarDocumentoElectronico(formTypeEx, indicador, almacen, docEntry.ToString(), false, docNum, DIAPIHelper.Sb1Company);

                if (create == "OK")
                    message = "OK";
                else if (create == "Error")
                    message = oBLCEEmision.WSdescripcion;
                else
                    message = oBLCEEmision.WSdescripcion;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }


            if (recordset != null)
                System.Runtime.InteropServices.Marshal.ReleaseComObject(recordset);

            return message;

        }


        public void ImprimirPDF (string form, string docEntry, string Folio, string rutaArchivos)
        {
            BLCEEmision oBLCEEmision = new BLCEEmision(rutaArchivos);
            string NombrePDF = oBLCEEmision.ObtenerNombrePDF(DIAPIHelper.Sb1Company);
            IndicadorDocumentoBL indicadorDocumentoBL = new IndicadorDocumentoBL();
            InvoiceBL invoiceBL = new InvoiceBL();
            string tipoDocumento = string.Empty;
            int intFolio = int.Parse(Folio);

            int tipoDte =  int.Parse(indicadorDocumentoBL.GetIndicador(form));

            invoiceBL.GetDocumentSubType(form, out tipoDocumento);

            oBLCEEmision.OptieneDte(null, 
                                    tipoDte,
                                    intFolio, 
                                    SCGCE.BL.facele.obtieneDTEFormato.PDF, 
                                    DIAPIHelper.Sb1Company,
                                    tipoDocumento, 
                                    false,
                                    NombrePDF, 
                                    BoObjectTypes.oInvoices,
                                    docEntry
                                    );



        }
    }
}
