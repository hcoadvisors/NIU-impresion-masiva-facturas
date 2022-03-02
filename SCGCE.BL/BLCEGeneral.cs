using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCG.DAC;
using SAPbobsCOM;
using SAPbouiCOM;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using System.Globalization;
using PDSA.PDSACryptography;

namespace SCGCE.BL
{
    public class BLCEGeneral
    {
        #region Declaracion

        private SAPbobsCOM.Company oCompany;
        private ServerType oServerType;
        private DbHelper oDbHelper = new DbHelper();
        private Application oSboAplication;
        private PDSASymmetric manageSecurity;
        private const string keyEncrypt = "uOy+T/osNvg=";
        private const string IVEncrypt = "msJOriaZmuc=";
        //private SCG.CE.SyncFramework.SyncManager _SyncManager = null;
        //private SCG.CE.SyncFramework.Config _Config = null;
        private NumberFormatInfo numberFormat;

        #endregion

        public BLCEGeneral(SAPbobsCOM.Company poCompany, ref Application sboAplication)
        {
            oCompany = poCompany;
            oServerType = oCompany.DbServerType == BoDataServerTypes.dst_HANADB ? ServerType.HANA : ServerType.SQLServer;
            oSboAplication = sboAplication;
            numberFormat = GetNumberFormatInfo(oCompany);
            manageSecurity = new PDSASymmetric();
            manageSecurity.KeyString = keyEncrypt;
            manageSecurity.IVString = IVEncrypt;
        }

        public BLCEGeneral(SAPbobsCOM.Company poCompany)
        {
            oCompany = poCompany;
            oServerType = oCompany.DbServerType == BoDataServerTypes.dst_HANADB ? ServerType.HANA : ServerType.SQLServer;
            //oSboAplication = sboAplication;
            numberFormat = GetNumberFormatInfo(oCompany);
            manageSecurity = new PDSASymmetric();
            manageSecurity.KeyString = keyEncrypt;
            manageSecurity.IVString = IVEncrypt;
        }

        public NumberFormatInfo GetNumberFormatInfo(SAPbobsCOM.Company company)
        {
            var companyService = company.GetCompanyService();
            var adminInfo = companyService.GetAdminInfo();
            var numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.CurrencyDecimalSeparator = adminInfo.DecimalSeparator;
            numberFormatInfo.CurrencyGroupSeparator = adminInfo.ThousandsSeparator;
            numberFormatInfo.CurrencyDecimalDigits = adminInfo.PriceAccuracy;
            numberFormatInfo.NumberDecimalDigits = adminInfo.AccuracyofQuantities;
            return numberFormatInfo;
        }

        //public void ProcesarDocumentosManual()
        //{
        //    _Config = new SCG.CE.SyncFramework.Config
        //    {
        //        Servidor = oCompany.Server,
        //        AutWindows = oCompany.UseTrusted,
        //        ServidorLicencias = oCompany.LicenseServer,
        //        BaseDatos = oCompany.CompanyDB,
        //        UsuarioBD = oCompany.DbUserName,
        //        ClaveBD = manageSecurity.Encrypt(oCompany.DbPassword),
        //        UsuarioSBO = oCompany.UserName,
        //        ClaveSBO = manageSecurity.Encrypt(oCompany.Password),
        //        MotorBD = oCompany.DbServerType == BoDataServerTypes.dst_HANADB ? "HANA" : "SQLServer"

        //        //Servidor = oCompany.
        //        //AutWindows = Boolean.Parse(drTienda.UsaAutenticacionWindows),
        //        //ServidorLicencias = drTienda.ServidorLicencias,
        //        //BaseDatos = drTienda.BaseDatos,
        //        //UsuarioBD = drTienda.UsuarioBD,
        //        //ClaveBD = drTienda.ClaveBD,
        //        //UsuarioSBO = drTienda.UsuarioSBO,
        //        //ClaveSBO = drTienda.ClaveSBO,
        //        //MotorBD = drTienda.MotorBD
        //    };

        //    _SyncManager = new SCG.CE.SyncFramework.SyncManager(3600, 100, oCompany);

        //    _SyncManager.SincFactSap();
        //    _SyncManager.SincNCredSap();
        //    _SyncManager.SincNDebiSap();

        //    _Config = null;
        //    _SyncManager = null;

        //}

        public bool EstaHabiltiadoProcesaDocumentoManual()
        {
            try
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string strSeries = string.Empty;

                strSeries = @"SELECT TOP 1 ""U_HabProM"" FROM " + oDbHelper.SetDB("@SCGCE_CONF");

                oRecordset.DoQuery(strSeries);

                if (oRecordset.Fields.Item(0).Value.ToString() != string.Empty)
                {
                    if (oRecordset.Fields.Item(0).Value.ToString() == "Y")
                    {
                        return true;
                    }
                    else { return false; }
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        public bool EsDocumentoElectronico(string pIndicador, out string pTipoDoc, string pFormTypeEx)
        {
            bool serieConfigurada = false;
            try
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                pTipoDoc = string.Empty;

                string strIndicador = string.Empty;

                strIndicador = @"SELECT ""U_IdSerS"", ""U_TipDoc"" FROM " + oDbHelper.SetDB("@SCGCE_CONFS") + @" WHERE  ""U_Term"" = '" + pFormTypeEx + "'";

                oRecordset.DoQuery(strIndicador);

                for (int i = 0; i < oRecordset.RecordCount; i++)
                {
                    if (oRecordset.Fields.Item(0).Value.ToString() == pIndicador)
                    {
                        pTipoDoc = oRecordset.Fields.Item(1).Value.ToString();
                        serieConfigurada = true;
                    }
                    else
                    {
                        pTipoDoc = string.Empty;
                        serieConfigurada = false;
                    }
                    oRecordset.MoveNext();
                }
                return serieConfigurada;
            }
            catch (Exception ex)
            {
                serieConfigurada = false;
                throw ex;
            }
        }

        /// <summary>
        /// Devuelve un arreglo de las series que estan configuradas
        /// </summary>
        /// <param name="pTipoDoc">Tipo de documento a consultar</param>
        /// <returns>arreglo de string de las series configuradas</returns>
        public string SeriesConfiguradasPorDocumento(string pTipoDoc)
        {
            string seriesConfiguradas = string.Empty;
            try
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string strSeries = string.Empty;

                strSeries = @"SELECT ""U_IdSerS"" FROM " + oDbHelper.SetDB("@SCGCE_CONFS") + @" WHERE ""U_TipDoc"" = '" + pTipoDoc + "'";

                oRecordset.DoQuery(strSeries);

                if (!oRecordset.EoF)
                {
                    oRecordset.MoveFirst();
                    for (int i = 0; i < oRecordset.RecordCount; i++)
                    {
                        seriesConfiguradas = seriesConfiguradas + oRecordset.Fields.Item(0).Value.ToString() + ",";
                        oRecordset.MoveNext();
                    }
                    seriesConfiguradas = seriesConfiguradas.Remove(seriesConfiguradas.Length - 1);
                }
            }
            catch (Exception ex)
            {
                return seriesConfiguradas;
                throw ex;
            }
            return seriesConfiguradas;
        }

        public void VerificarConfiguracionImpuestos(string codImpuesto, out double pMontoImpuesto, out double pPorcentajeImpuesto)
        {
            try
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                pMontoImpuesto = 0;
                pPorcentajeImpuesto = 0;
                string strSeries = string.Empty;

                strSeries = @"SELECT TOP 1 ""U_MntImpPad"",""U_PorImpPad"" FROM " + oDbHelper.SetDB("@SCGCE_TIPIMP") + @" Where ""Code"" = '" + codImpuesto + "'";

                oRecordset.DoQuery(strSeries);

                if (oRecordset.Fields.Item(0).Value.ToString() != string.Empty)
                {
                    pMontoImpuesto = double.Parse(oRecordset.Fields.Item(0).Value.ToString(), numberFormat);
                    pPorcentajeImpuesto = double.Parse(oRecordset.Fields.Item(1).Value.ToString(), numberFormat);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ExisteConfiguracionHacienda()
        {
            try
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string strSeries = string.Empty;

                strSeries = @"SELECT TOP 1 ""Code"" FROM " + oDbHelper.SetDB("@SCGCE_SUC");

                oRecordset.DoQuery(strSeries);

                if (oRecordset.Fields.Item(0).Value.ToString() != string.Empty)
                {
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return true;
                throw ex;
            }
        }

        public string tipoDeDocumentoPorSerie(string pSerie)
        {
            string tipoVacaciones = string.Empty;

            try
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string strSeries = string.Empty;

                strSeries = @"SELECT TOP 1 ""U_TipDoc"" FROM " + oDbHelper.SetDB("@SCGCE_CONFS") + @" WHERE ""U_IdSerS"" = '" + pSerie + "'";

                oRecordset.DoQuery(strSeries);

                if (oRecordset.Fields.Item(0).Value.ToString() != string.Empty)
                {
                    tipoVacaciones = oRecordset.Fields.Item(0).Value.ToString();
                }
            }
            catch (Exception ex)
            {
                tipoVacaciones = string.Empty;
                throw ex;
            }
            return tipoVacaciones;
        }


        public bool facturacionEnLineaActivado()
        {
            try
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string strSeries = string.Empty;

                strSeries = @"SELECT TOP 1 ""U_EmitCe"" FROM " + oDbHelper.SetDB("@SCGCE_CONF");

                oRecordset.DoQuery(strSeries);

                if (oRecordset.Fields.Item(0).Value.ToString() != string.Empty)
                {
                    if (oRecordset.Fields.Item(0).Value.ToString() == "Y")
                    {
                        return true;
                    }
                    else { return false; }
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
        //public void registrarInformacionCE(string pDocEntry, int pDocType, string pTipoDocHacienda, string pEstadoSituacion)
        //{
        //    try
        //    {
        //        SAPbobsCOM.Documents oDocuments = null;
        //        oDocuments = oCompany.GetBusinessObject((SAPbobsCOM.BoObjectTypes)pDocType);
        //        oDocuments.GetByKey(Convert.ToInt32(pDocEntry));

        //        if (string.IsNullOrEmpty(oDocuments.UserFields.Fields.Item("U_SCGCE_ClaveNum").Value) || string.IsNullOrWhiteSpace(oDocuments.UserFields.Fields.Item("U_SCGCE_ClaveNum").Value) || oDocuments.UserFields.Fields.Item("U_SCGCE_ClaveNum").Size != 50)
        //        {
        //            SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
        //            SAPbobsCOM.Recordset oRecordset2 = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

        //            string strSeries = string.Empty;
        //            string strEncabezado = string.Empty;
        //            string strSucursal = string.Empty;
        //            string strTerminal = string.Empty;
        //            string strSecuencia = string.Empty;

        //            strSeries = @"SELECT ""Code"", ""LineId"", ""U_IdSerS"", ""U_TipDoc"", ""U_Desc"", ""U_Term"", ""U_Term"",""U_Suc"" FROM " + oDbHelper.SetDB("@SCGCE_CONFS") + @" WHERE ""U_IdSerS"" = '" + oDocuments.Series + @"' AND ""U_TipDoc"" = '" + pTipoDocHacienda + "'";

        //            oRecordset.DoQuery(strSeries);

        //            if (!oRecordset.EoF)
        //            {
        //                string strCodPais = "506";
        //                string strDia = oDocuments.DocDate.Day.ToString().PadLeft(2, '0');
        //                string strMes = oDocuments.DocDate.Month.ToString().PadLeft(2, '0');
        //                string strAnno = oDocuments.DocDate.ToString("yy").PadLeft(2, '0');
        //                string strCed = string.Empty;

        //                strEncabezado = @"SELECT TOP 1 ""U_IdNum"" FROM " + oDbHelper.SetDB("@SCGCE_CONF");

        //                oRecordset2.DoQuery(strEncabezado);
        //                if (!oRecordset2.EoF)
        //                {
        //                    strCed = oRecordset2.Fields.Item(0).Value.ToString();
        //                    strCed = strCed.PadLeft(12, '0');
        //                }

        //                strSucursal = oRecordset.Fields.Item(7).Value.ToString();
        //                strSucursal = strSucursal.PadLeft(3, '0');

        //                strTerminal = oRecordset.Fields.Item(6).Value.ToString();
        //                strTerminal = strTerminal.PadLeft(5, '0');

        //                pTipoDocHacienda = pTipoDocHacienda.PadLeft(2, '0');

        //                string strCons = oDocuments.DocNum.ToString();
        //                strCons = strCons.Substring(1,strCons.Length-1);
        //                strCons = strCons.PadLeft(10, '0');

        //                strSecuencia = strSucursal + strTerminal + pTipoDocHacienda + strCons;


        //                string strSecKey = FacturaElectronica_Cod_Seguridad();
        //                oDocuments.UserFields.Fields.Item("U_SCGCE_Secuen").Value = strSecuencia;
        //                oDocuments.UserFields.Fields.Item("U_SCGCE_ClaveNum").Value = strCodPais + strDia + strMes + strAnno + strCed + strSecuencia + pEstadoSituacion + strSecKey;

        //                //int numLinea = int.Parse(oRecordset.Fields.Item(1).Value.ToString());
        //                //int numSec = int.Parse(oRecordset.Fields.Item(6).Value.ToString());

        //                if (string.IsNullOrEmpty(oDocuments.UserFields.Fields.Item("U_SCGCE_CondVent").Value) || string.IsNullOrWhiteSpace(oDocuments.UserFields.Fields.Item("U_SCGCE_CondVent").Value))
        //                    oDocuments.UserFields.Fields.Item("U_SCGCE_CondVent").Value = "01"; //Condicion de venta de contado

        //                if (string.IsNullOrEmpty(oDocuments.UserFields.Fields.Item("U_SCGCE_MedPag1").Value) || string.IsNullOrWhiteSpace(oDocuments.UserFields.Fields.Item("U_SCGCE_MedPag1").Value))
        //                    oDocuments.UserFields.Fields.Item("U_SCGCE_MedPag1").Value = "01"; // Medio de pago 1 efectivo

        //                if (string.IsNullOrEmpty(oDocuments.UserFields.Fields.Item("U_SCGCE_TipFac").Value) || string.IsNullOrWhiteSpace(oDocuments.UserFields.Fields.Item("U_SCGCE_TipFac").Value))
        //                    oDocuments.UserFields.Fields.Item("U_SCGCE_TipFac").Value = "TE"; // Medio de pago 1 efectivo

        //                oCompany.StartTransaction();
        //                //actualizarSecuenciaNumeroUDT(oDocuments.Series.ToString(), numLinea, numSec + 1);
        //                oDocuments.Update();
        //                if (oCompany.InTransaction)
        //                    oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
        //            }
        //        }// CompletarUDF(ref oDocuments, tipoDoc);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (oCompany.InTransaction)
        //            oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

        //        throw ex;
        //    }

        //}

        public void GuardarRespuestaFE(int pDocEntry, string pTipoDoc, string pMensaje, string pEstadoFE)
        {
            try
            {
                SAPbobsCOM.Documents oDocuments = null;
                int iRetCode = 0;
                int iErrCode = 0;
                string sErrMsg;
                switch (pTipoDoc)
                {
                    case "03":
                        oDocuments = (Documents)oCompany.GetBusinessObject((SAPbobsCOM.BoObjectTypes.oCreditNotes));
                        break;
                    default: //"TQ","FC","ND"
                        oDocuments = (Documents)oCompany.GetBusinessObject((SAPbobsCOM.BoObjectTypes.oInvoices));
                        break;
                }

                oDocuments.GetByKey(Convert.ToInt32(pDocEntry));

                oDocuments.UserFields.Fields.Item("U_SCGCE_EstadoAut").Value = pEstadoFE;
                oDocuments.UserFields.Fields.Item("U_SCGCE_Des").Value = pMensaje;

                iRetCode = oDocuments.Update();

                if (iRetCode != 0)
                {
                    oCompany.GetLastError(out iErrCode, out sErrMsg);
                    throw new Exception("Error en GuardarRespuestaFE: " + sErrMsg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///// <summary>
        ///// Se crea la clave numerica requerida por el ministerio de hacienda
        ///// </summary>
        ///// <param name="oDocuments">Se envia el documentos con el cual se esta trabajando</param>
        ///// <param name="pCodTipoDoc">Se especifica el numero de documento segun hacienda, 01 Factura de cliente, 02: Nota de debito, 03 Nota de credito, 04 Tiquete Electronico</param>
        ///// <returns></returns>
        //private void CompletarUDF(ref SAPbobsCOM.Documents oDocuments, string pCodTipoDoc)
        //{
        //    int result = -1;

        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        string error;
        //        error = ex.InnerException.Message.Length > 0 ?ex.InnerException.Message: ex.Message;
        //        if (oCompany.InTransaction)
        //            oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack); 

        //        throw new Exception("Error en Crear clave numerica " + error);
        //    }          
        //}

        private void actualizarSecuenciaNumeroUDT(string pIdSers, int pNumLinea, int pUltimoValor)
        {
            try
            {
                GeneralService oGeneralService;
                GeneralData oGeneralData;
                GeneralDataParams oGeneralParams;
                GeneralDataCollection oGeneralDataCollection;
                GeneralData oGeneralDataLinea;
                int numLinea;

                CompanyService oCompanyService;
                oCompanyService = oCompany.GetCompanyService();

                oGeneralService = oCompanyService.GetGeneralService("SCGCE_CONF");

                oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);
                oGeneralParams.SetProperty("Code", "1");
                oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                oGeneralDataCollection = oGeneralData.Child("SCGCE_CONFS");

                for (int i = 0; i < oGeneralDataCollection.Count - 1; i++)
                {
                    oGeneralDataLinea = oGeneralDataCollection.Item(i);

                    numLinea = Convert.ToInt32(oGeneralDataLinea.GetProperty("LineId"));

                    if (numLinea == pNumLinea)
                    {
                        oGeneralDataLinea.SetProperty("U_Num", pUltimoValor.ToString());
                        break;
                    }
                }
                oGeneralService.Update(oGeneralData);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public int GetFacturaInformacionFE(string pDocEntry, out string pIndicador, out string pAlmacen, string pFormTypeEx, out string objType)
        {
            try
            {
                string Control = string.Empty;
                int DocNum = 0;
                SAPbobsCOM.Documents oDocuments = null;
                SAPbobsCOM.StockTransfer oStockTransfer = null;
                switch (pFormTypeEx)
                {
                    case "133"://Factura
                    case "65302"://Factura Exenta
                    case "65304": //Boleta
                    case "65305": //Boleta Exenta
                    case "60091": //Factura Reserva
                    case "65307": //Factura Exportación                     
                        {
                            oDocuments = oDocuments = (Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                            break;
                        }
                    case "179": // Nota de credito
                        {
                            oDocuments = oDocuments = (Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes);
                            break;
                        }
                    case "65303":   // Nota de debito
                        {
                            oDocuments = oDocuments = (Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                            break;
                        }
                    case "141":   //Factura Compra
                        {
                            oDocuments = oDocuments = (Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
                            break;
                        }
                    case "940":   //Guia Despacho
                        {
                            oStockTransfer = oStockTransfer = (StockTransfer)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer);
                            break;
                        }
                    case "140":   //Guia Despacho
                        {
                            oDocuments = oDocuments = (Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDeliveryNotes);
                            break;
                        }
                    case "182":   //Guia Despacho
                        {
                            oDocuments = oDocuments = (Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseReturns);
                            break;
                        }
                    case "65300": //Factura Anticipo
                        {
                            oDocuments = oDocuments = (Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDownPayments);
                            break;
                        }
                }

                if (pFormTypeEx != "940")
                {
                    oDocuments.GetByKey(Convert.ToInt32(pDocEntry));
                    pIndicador = oDocuments.Indicator.ToString();
                    pAlmacen = string.Empty;
                    DocNum = oDocuments.DocNum;
                    objType = ((int)oDocuments.DocObjectCode).ToString();

                    if (oDocuments != null)
                    {
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oDocuments);
                        oDocuments = null;
                    }
                    return DocNum;
                }
                else
                {
                    oStockTransfer.GetByKey(Convert.ToInt32(pDocEntry));
                    Control = oStockTransfer.UserFields.Fields.Item("U_SCGCE_EnviaTranf").Value.ToString();
                    if (Control == "Y")
                    {
                        pIndicador = "52";
                    }
                    else
                    {
                        pIndicador = "50";
                    }
                    DocNum = oStockTransfer.DocNum;
                    pAlmacen = oStockTransfer.ToWarehouse;
                    objType = ((int)oStockTransfer.DocObjectCode).ToString();

                    if (oStockTransfer != null)
                    {
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oStockTransfer);
                        oStockTransfer = null;
                    }
                    return DocNum;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string FacturaElectronica_Cod_Seguridad()
        {
            Random rndRandom = new Random();
            string Salida = string.Empty;
            for (int value = 0; value <= 7; value++)
                Salida += rndRandom.Next(0, 9).ToString();
            return Salida;
        }

        public string SacarValorObjectKey(string seccion, string clave, ref System.Xml.XmlDocument p_configXML)
        {
            string strEntry = "";

            System.Xml.XmlNode n = null;
            n = p_configXML.SelectSingleNode(seccion);

            if (n != null)
                strEntry = (n.InnerText);

            return strEntry;
        }

        public void CargarInformacionBitacora(ref Form pForm)
        {
            string sQuery = string.Empty;
            string selectTop;
            DataTable oDataTable = null;
            DateTime fechaDes, fechaHas;
            try
            {
                Grid oGrid;
                Item oItemG;

                oItemG = pForm.Items.Item("dgvBit");
                oGrid = (Grid)oItemG.Specific;
                oGrid.DataTable = null;

                for (int i = 0; i < pForm.DataSources.DataTables.Count; i++)
                {
                    if (pForm.DataSources.DataTables.Item(i).UniqueID == "dt")
                    {
                        oDataTable = pForm.DataSources.DataTables.Item(i);
                        break;
                    }
                }

                if (oDataTable == null)
                {
                    oDataTable = pForm.DataSources.DataTables.Add("dt");
                    oDataTable.Columns.Add("Tipo Doc", BoFieldsType.ft_AlphaNumeric, 2);
                    oDataTable.Columns.Add("Des Doc", BoFieldsType.ft_AlphaNumeric, 30);
                    oDataTable.Columns.Add("ID interno", BoFieldsType.ft_AlphaNumeric, 254);
                    oDataTable.Columns.Add("Num Doc", BoFieldsType.ft_AlphaNumeric, 254);
                    oDataTable.Columns.Add("Folio", BoFieldsType.ft_AlphaNumeric, 254);
                    oDataTable.Columns.Add("ID Cliente", BoFieldsType.ft_AlphaNumeric, 254);
                    oDataTable.Columns.Add("Nombre", BoFieldsType.ft_AlphaNumeric, 254);
                    oDataTable.Columns.Add("Cod Resp", BoFieldsType.ft_AlphaNumeric, 254);
                    oDataTable.Columns.Add("Mensaje", BoFieldsType.ft_AlphaNumeric, 254);
                }
                else
                {
                    oDataTable.Clear();
                    oDataTable.Columns.Add("Tipo Doc", BoFieldsType.ft_AlphaNumeric, 2);
                    oDataTable.Columns.Add("Des Doc", BoFieldsType.ft_AlphaNumeric, 30);
                    oDataTable.Columns.Add("ID interno", BoFieldsType.ft_AlphaNumeric, 254);
                    oDataTable.Columns.Add("Num Doc", BoFieldsType.ft_AlphaNumeric, 254);
                    oDataTable.Columns.Add("Folio", BoFieldsType.ft_AlphaNumeric, 254);
                    oDataTable.Columns.Add("ID Cliente", BoFieldsType.ft_AlphaNumeric, 254);
                    oDataTable.Columns.Add("Nombre", BoFieldsType.ft_AlphaNumeric, 254);
                    oDataTable.Columns.Add("Cod Resp", BoFieldsType.ft_AlphaNumeric, 254);
                    oDataTable.Columns.Add("Mensaje", BoFieldsType.ft_AlphaNumeric, 254);
                }

                Item oItemFec; EditText oEditFec;
                oItemFec = oSboAplication.Forms.Item(oSboAplication.Forms.ActiveForm.UniqueID).Items.Item("txtDesde");
                oEditFec = (EditText)oItemFec.Specific;

                if (string.IsNullOrEmpty(oEditFec.Value) || string.IsNullOrWhiteSpace(oEditFec.Value))
                {
                    throw new Exception("Favor definir la fecha Desde");
                }
                else
                {
                    fechaDes = DateTime.ParseExact(oEditFec.Value, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                }

                oItemFec = oSboAplication.Forms.Item(oSboAplication.Forms.ActiveForm.UniqueID).Items.Item("txtHasta");
                oEditFec = (EditText)oItemFec.Specific;

                if (string.IsNullOrEmpty(oEditFec.Value) || string.IsNullOrWhiteSpace(oEditFec.Value))
                {
                    throw new Exception("Favor definir la fecha Hasta");
                }
                else
                {
                    fechaHas = DateTime.ParseExact(oEditFec.Value, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                }

                ComboBox cmbDoc = (ComboBox)oSboAplication.Forms.Item(oSboAplication.Forms.ActiveForm.UniqueID).Items.Item("CmbEstDoc").Specific;

                if (!string.IsNullOrEmpty(cmbDoc.Value) || !string.IsNullOrEmpty(cmbDoc.Value))
                {
                    sQuery = sQuery.Length > 0 ? sQuery + @" AND ""U_SCGCE_EstadoAut"" = '" + cmbDoc.Value + "' " : sQuery + @" ""U_SCGCE_EstadoAut"" = '" + cmbDoc.Value + "' ";
                    sQuery = Convert.ToInt32(cmbDoc.Value) == 0 ? sQuery + @" AND (""FolioNum"" is null OR ""FolioNum"" =0) " : sQuery + @" AND ""FolioNum"" is not null ";
                }

                Item oItem = oSboAplication.Forms.Item(oSboAplication.Forms.ActiveForm.UniqueID).Items.Item("txtFilas");
                EditText oEdit = (EditText)oItem.Specific;

                if (string.IsNullOrEmpty(oEdit.Value) && string.IsNullOrWhiteSpace(oEdit.Value))
                {
                    selectTop = "100";
                }
                else
                {
                    selectTop = oEdit.Value;
                }


                sQuery = sQuery.Length > 0 ? sQuery + @" AND ""TaxDate"" BETWEEN " + CToSQLAnsiDate(fechaDes) +
                    @" AND  " + CToSQLAnsiDate(fechaHas) + " " : @" ""TaxDate""  BETWEEN " + CToSQLAnsiDate(fechaDes) +
                    @" AND  " + CToSQLAnsiDate(fechaHas) + " ";

                cmbDoc = (ComboBox)oSboAplication.Forms.Item(oSboAplication.Forms.ActiveForm.UniqueID).Items.Item("cmbTipoDoc").Specific;

                if (cmbDoc.Value != "" & cmbDoc.Value != "00")
                {
                    switch (cmbDoc.Value)
                    {
                        case "01":
                            {
                                //sQuery += @" AND ""Indicator"" = '33' And ""DocSubType"" = '--' ";
                                consultarDocumentoFA(sQuery, ref oDataTable, selectTop, "01", "Factura Electrónica", "OINV");//Consulta las facturas electronicas
                                break;
                            }
                        case "02":
                            {
                                sQuery += @" AND ""Indicator"" = '34' And ""DocSubType"" = 'IE' ";
                                consultarDocumento(sQuery, ref oDataTable, selectTop, "02", "Factura Exenta Electrónica", "OINV");//Consulta las facturas exenta electronicas
                                break;
                            }
                        case "03":
                            {
                                sQuery += @" AND ""Indicator"" = '39' And ""DocSubType"" = 'IB' ";
                                consultarDocumento(sQuery, ref oDataTable, selectTop, "03", "Boleta Electrónica", "OINV");//Consulta las Boletas electronicas
                                break;
                            }
                        case "04":
                            {
                                sQuery += @" AND ""Indicator"" = '41' And ""DocSubType"" = 'EB' ";
                                consultarDocumento(sQuery, ref oDataTable, selectTop, "04", "Boleta Exenta Electrónica", "OINV");//Consulta las Boletas Exentas electronicas
                                break;
                            }
                        case "05":
                            {
                                sQuery += @" AND ""Indicator"" = '46' And ""DocSubType"" = '--' ";
                                consultarDocumento(sQuery, ref oDataTable, selectTop, "05", "Factura Compra Electrónica", "OPCH");//Consulta las facturas compra electronicas
                                break;
                            }
                        case "06":
                            {
                                sQuery += @" AND (""Indicator"" = '56' OR ""Indicator"" = '11') And ""DocSubType"" = 'DN' ";
                                consultarDocumento(sQuery, ref oDataTable, selectTop, "06", "Nota Débito Electrónica", "OINV");//Consulta las Nota Debito electronicas
                                break;
                            }
                        case "07":
                            {
                                sQuery += @" AND (""Indicator"" = '61' OR ""Indicator"" = '13') And ""DocSubType"" = '--' ";
                                consultarDocumento(sQuery, ref oDataTable, selectTop, "07", "Nota Crédito Electrónica", "ORIN");//Consulta las Nota Credito electronicas
                                break;
                            }
                        case "08":
                            {
                                //sQuery += @" AND ""U_SCGCE_EnviaTranf"" = 'Y' And ""DocSubType"" = '--' ";
                                //consultarDocumento(sQuery, ref oDataTable, selectTop, "08", "Guía Despacho Electrónica", "OWTR");//Consulta las Guia Despacho electronicas
                                //sQuery += @" AND ""Indicator"" = '52' And ""DocSubType"" = '--' ";
                                //consultarDocumento(sQuery, ref oDataTable, selectTop, "08", "Guía Despacho Electrónica", "ODLN");//Consulta las Guia Despacho electronicas
                                //sQuery += @" AND ""Indicator"" = '52' And ""DocSubType"" = '--' ";
                                consultarDocumentoGD(sQuery, ref oDataTable, selectTop, "08", "Guía Despacho Electrónica", "ORPD");//Consulta las Guia Despacho electronicas
                                break;
                            }
                    }
                }
                else
                {
                    consultarDocumentosTodos(sQuery, ref oDataTable, selectTop);//Consulta las facturas electronicas
                }

                oGrid.DataTable = oDataTable;
                //oGrid.CollapseLevel = 1;
                oGrid.AutoResizeColumns();
            }
            catch (Exception ex)
            {
                ManejoErrores(ex);
            }
        }

        /// <summary>
        /// Query para obtener los documentos segun los criterios de busqueda, 
        /// </summary>
        /// <param name="pSquery">Query con lo parametros</param>
        private void consultarDocumentoFacturaCondicional(DBDataSource pdsDocumentos, string pDocEntry, string pClaveNum, string pTipoDoc, string pEstadoDoc, int pTopReg)
        {
            //SAPbouiCOM.DBDataSource dsExpedientes;
            SAPbouiCOM.Conditions oConditions;
            SAPbouiCOM.Condition oCondition;
            Matrix mtxBit = null;
            DBDataSource bdDataSource = null;

            try
            {

                mtxBit = (Matrix)oSboAplication.Forms.Item(oSboAplication.Forms.ActiveForm.UniqueID).Items.Item("mtxBit").Specific;


                //Verifica que al menos uno de los filtros se haya ingresado
                if (!string.IsNullOrEmpty(pTipoDoc))
                {
                    switch (pTipoDoc)
                    {
                        case "01":
                            oSboAplication.Forms.Item(oSboAplication.Forms.ActiveForm.UniqueID).DataSources.DBDataSources.Add("OINV");
                            pdsDocumentos = oSboAplication.Forms.Item(oSboAplication.Forms.ActiveForm.UniqueID).DataSources.DBDataSources.Item("OINV");

                            oSboAplication.Forms.Item(oSboAplication.Forms.ActiveForm.UniqueID).DataSources.DBDataSources.Add("ORIN");
                            bdDataSource = oSboAplication.Forms.Item(oSboAplication.Forms.ActiveForm.UniqueID).DataSources.DBDataSources.Item("ORIN");

                            break;

                        case "02":

                            oSboAplication.Forms.Item(oSboAplication.Forms.ActiveForm.UniqueID).DataSources.DBDataSources.Add("ORIN");
                            bdDataSource = oSboAplication.Forms.Item(oSboAplication.Forms.ActiveForm.UniqueID).DataSources.DBDataSources.Item("ORIN");

                            break;

                        case "03":

                            break;

                        case "04":
                            oSboAplication.Forms.Item(oSboAplication.Forms.ActiveForm.UniqueID).DataSources.DBDataSources.Add("ORIN");
                            bdDataSource = oSboAplication.Forms.Item(oSboAplication.Forms.ActiveForm.UniqueID).DataSources.DBDataSources.Item("ORIN");

                            break;
                    }


                    //Obtiene el DataSource de la tabla de ofertas de ventas
                    //    dsExpedientes = FormularioSBO.DatSources.DBDataSources.Item("OQUT");


                    //Crea un objeto SAPbouiCOM.Conditions, que se encarga de generar un query sobre la tabla OQUT y obtener los datos de los expedientes
                    oConditions = (SAPbouiCOM.Conditions)oSboAplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_Conditions);
                    oCondition = oConditions.Add();

                    oCondition.BracketOpenNum = 1;

                    oCondition.Alias = "U_SCGCE_ClaveNum";
                    oCondition.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_NULL;
                    //oCondition.CondVal = p_strCodigoUnidad;


                    //if (!string.IsNullOrEmpty(p_strCodigoUnidad) && !string.IsNullOrEmpty(p_strNumeroExpediente))
                    //{
                    //    oCondition.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;
                    //    oCondition = oConditions.Add();
                    //}

                    //if (!string.IsNullOrEmpty(p_strNumeroExpediente))
                    //{
                    //    oCondition.Alias = "U_SCGD_Exp";
                    //    oCondition.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                    //    oCondition.CondVal = p_strNumeroExpediente;
                    //}

                    oCondition.BracketCloseNum = 1;

                    Matrix omatrix;
                    Grid ogrid;

                    DataTable a;

                    pdsDocumentos.Query(oConditions);
                    EnlazaColumnasMatrizADatasource(mtxBit, out omatrix, "OINV");
                    mtxBit.LoadFromDataSource();
                    mtxBit.FlushToDataSource();

                    bdDataSource.Query(oConditions);

                    EnlazaColumnasMatrizADatasource(mtxBit, out omatrix, "ORIN");

                    mtxBit.LoadFromDataSource();
                    mtxBit.FlushToDataSource();
                    mtxBit.AutoResizeColumns();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void EnlazaColumnasMatrizADatasource(SAPbouiCOM.Matrix oMatrix, out Matrix oMatrixCop, string pTable)
        {
            SAPbouiCOM.Column oColumna;

            oMatrixCop = null;
            try
            {
                //DocEntry de la unidad
                oColumna = oMatrix.Columns.Item("DocNum");
                oColumna.DataBind.SetBound(true, pTable, "DocNum");
                //Número de unidad
                oColumna = oMatrix.Columns.Item("ClavNum");
                oColumna.DataBind.SetBound(true, pTable, "U_SCGCE_ClaveNum");
                //Marca
                oColumna = oMatrix.Columns.Item("FecDoc");
                oColumna.DataBind.SetBound(true, pTable, "TaxDate");
                ////Estilo
                //oColumna = oMatrix.Columns.Item("TipDoc");
                //oColumna.DataBind.SetBound(true, pTable, "U_SCGD_Des_Esti");
                //Modelo
                oColumna = oMatrix.Columns.Item("EstDoc");
                oColumna.DataBind.SetBound(true, pTable, "U_SCGCE_EstadoAut");
                //Expediente
                oColumna = oMatrix.Columns.Item("Desc");
                oColumna.DataBind.SetBound(true, pTable, "U_SCGCE_Des");

                //oMatrix.LoadFromDataSource();
                //oMatrix.FlushToDataSource();

                oMatrixCop = oMatrix;

            }
            catch (Exception ex)
            {
                oMatrixCop = null;
            }

            oMatrixCop = oMatrix;

        }

        /// <summary>
        /// Query para obtener los documentos segun los criterios de busqueda, 
        /// </summary>
        /// <param name="pSquery">Query con lo parametros</param>
        private void consultarDocumento(string pSquery, ref DataTable oDataTable, string pTop, string pTipoDoc, string pDesDoc, string pTablaBD)
        {
            SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            string strSeries = string.Empty;
            string status = string.Empty;
            string Mensaje = string.Empty;
            //DataTable odtable = null;

            strSeries = "SELECT TOP " + pTop + " '" + pTipoDoc + "' AS \"TipoDocumento\", '" + pDesDoc + "' AS \"DesDocumento\", \"DocEntry\", \"DocNum\", \"FolioNum\", " +
                " \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", \"U_SCGCE_Des\" FROM \"" + pTablaBD + "\" " +
                " WHERE" + pSquery;

            oRecordset.DoQuery(strSeries);


            if (!oRecordset.EoF)
            {
                oRecordset.MoveFirst();
                for (int i = 0; i < oRecordset.RecordCount; i++)
                {
                    oDataTable.Rows.Add();
                    int index = oDataTable.Rows.Count - 1;
                    oDataTable.SetValue("Tipo Doc", index, oRecordset.Fields.Item("TipoDocumento").Value.ToString());
                    oDataTable.SetValue("Des Doc", index, oRecordset.Fields.Item("DesDocumento").Value.ToString());
                    oDataTable.SetValue("ID interno", index, oRecordset.Fields.Item("DocEntry").Value.ToString());
                    oDataTable.SetValue("Num Doc", index, oRecordset.Fields.Item("DocNum").Value.ToString());
                    oDataTable.SetValue("Folio", index, oRecordset.Fields.Item("FolioNum").Value.ToString());
                    oDataTable.SetValue("ID Cliente", index, oRecordset.Fields.Item("CardCode").Value.ToString());
                    oDataTable.SetValue("Nombre", index, oRecordset.Fields.Item("CardName").Value.ToString());
                    oDataTable.SetValue("Cod Resp", index, oRecordset.Fields.Item("U_SCGCE_EstadoAut").Value.ToString());
                    Mensaje = oRecordset.Fields.Item("U_SCGCE_Des").Value.ToString();
                    if (Mensaje.Length > 254)
                    {
                        Mensaje = Mensaje.Substring(0, 254);
                    }
                    oDataTable.SetValue("Mensaje", index, Mensaje);
                    oRecordset.MoveNext();
                }
            }
        }

        private void consultarDocumentoGD(string pSquery, ref DataTable oDataTable, string pTop, string pTipoDoc, string pDesDoc, string pTablaBD)
        {
            SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            string strSeries = string.Empty;
            string status = string.Empty;
            string Mensaje = string.Empty;
            //DataTable odtable = null;

            strSeries = " SELECT TOP " + pTop + " '08' AS \"TipoDocumento\", 'Guía Despacho Electrónica TS' AS \"DesDocumento\",  \"DocEntry\", \"DocNum\", \"FolioNum\", " +
            " \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"OWTR\" WHERE " + pSquery + " AND \"U_SCGCE_EnviaTranf\" = 'Y' And \"DocSubType\" = '--' UNION " +
            " SELECT TOP " + pTop + " '08' AS \"TipoDocumento\", 'Guía Despacho Electrónica EN' AS \"DesDocumento\",  \"DocEntry\", \"DocNum\", \"FolioNum\", " +
            " \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"ODLN\" WHERE " + pSquery + " AND \"Indicator\" = '52' And \"DocSubType\" = '--' UNION " +
            " SELECT TOP " + pTop + " '08' AS \"TipoDocumento\", 'Guía Despacho Electrónica DM' AS \"DesDocumento\",  \"DocEntry\", \"DocNum\", \"FolioNum\", " +
            " \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"ORPD\" WHERE " + pSquery + " AND \"Indicator\" = '52' And \"DocSubType\" = '--'";

            oRecordset.DoQuery(strSeries);


            if (!oRecordset.EoF)
            {
                oRecordset.MoveFirst();
                for (int i = 0; i < oRecordset.RecordCount; i++)
                {
                    oDataTable.Rows.Add();
                    int index = oDataTable.Rows.Count - 1;
                    oDataTable.SetValue("Tipo Doc", index, oRecordset.Fields.Item("TipoDocumento").Value.ToString());
                    oDataTable.SetValue("Des Doc", index, oRecordset.Fields.Item("DesDocumento").Value.ToString());
                    oDataTable.SetValue("ID interno", index, oRecordset.Fields.Item("DocEntry").Value.ToString());
                    oDataTable.SetValue("Num Doc", index, oRecordset.Fields.Item("DocNum").Value.ToString());
                    oDataTable.SetValue("Folio", index, oRecordset.Fields.Item("FolioNum").Value.ToString());
                    oDataTable.SetValue("ID Cliente", index, oRecordset.Fields.Item("CardCode").Value.ToString());
                    oDataTable.SetValue("Nombre", index, oRecordset.Fields.Item("CardName").Value.ToString());
                    oDataTable.SetValue("Cod Resp", index, oRecordset.Fields.Item("U_SCGCE_EstadoAut").Value.ToString());
                    Mensaje = oRecordset.Fields.Item("U_SCGCE_Des").Value.ToString();
                    if (Mensaje.Length > 254)
                    {
                        Mensaje = Mensaje.Substring(0, 254);
                    }
                    oDataTable.SetValue("Mensaje", index, Mensaje);
                    oRecordset.MoveNext();
                }
            }
        }

        private void consultarDocumentoFA(string pSquery, ref DataTable oDataTable, string pTop, string pTipoDoc, string pDesDoc, string pTablaBD)
        {
            SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            string strSeries = string.Empty;
            string status = string.Empty;
            string Mensaje = string.Empty;
            //DataTable odtable = null;

            strSeries = "SELECT TOP " + pTop + " '01' AS \"TipoDocumento\", 'Factura Electrónica' AS \"DesDocumento\", \"DocEntry\", \"DocNum\", " +
            " \"FolioNum\", \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR) AS \"U_SCGCE_Des\" " +
            " FROM \"OINV\" WHERE " + pSquery + " AND \"Indicator\" = '33' And \"DocSubType\" = '--' And \"isIns\" = 'N' UNION " +
             " SELECT TOP " + pTop + " '01' AS \"TipoDocumento\", 'Factura Reserva Electrónica' AS \"DesDocumento\", \"DocEntry\", \"DocNum\",  " +
            " \"FolioNum\", \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"OINV\" WHERE " + pSquery + " AND \"Indicator\" = '33' And \"DocSubType\" = '--' And \"isIns\" = 'Y' UNION " +
            " SELECT TOP " + pTop + " '01' AS \"TipoDocumento\", 'Factura Anticipo Electrónica' AS \"DesDocumento\", \"DocEntry\", \"DocNum\",  " +
            " \"FolioNum\", \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"ODPI\" WHERE " + pSquery + " AND \"Indicator\" = '33' And \"DocSubType\" = '--' UNION " +
            " SELECT TOP " + pTop + " '01' AS \"TipoDocumento\", 'Factura Exportación' AS \"DesDocumento\", \"DocEntry\", \"DocNum\",  " +
            " \"FolioNum\", \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"OINV\" WHERE " + pSquery + " AND \"Indicator\" = '10' And \"DocSubType\" = 'IX' ";

            oRecordset.DoQuery(strSeries);


            if (!oRecordset.EoF)
            {
                oRecordset.MoveFirst();
                for (int i = 0; i < oRecordset.RecordCount; i++)
                {
                    oDataTable.Rows.Add();
                    int index = oDataTable.Rows.Count - 1;
                    oDataTable.SetValue("Tipo Doc", index, oRecordset.Fields.Item("TipoDocumento").Value.ToString());
                    oDataTable.SetValue("Des Doc", index, oRecordset.Fields.Item("DesDocumento").Value.ToString());
                    oDataTable.SetValue("ID interno", index, oRecordset.Fields.Item("DocEntry").Value.ToString());
                    oDataTable.SetValue("Num Doc", index, oRecordset.Fields.Item("DocNum").Value.ToString());
                    oDataTable.SetValue("Folio", index, oRecordset.Fields.Item("FolioNum").Value.ToString());
                    oDataTable.SetValue("ID Cliente", index, oRecordset.Fields.Item("CardCode").Value.ToString());
                    oDataTable.SetValue("Nombre", index, oRecordset.Fields.Item("CardName").Value.ToString());
                    oDataTable.SetValue("Cod Resp", index, oRecordset.Fields.Item("U_SCGCE_EstadoAut").Value.ToString());
                    Mensaje = oRecordset.Fields.Item("U_SCGCE_Des").Value.ToString();
                    if (Mensaje.Length > 254)
                    {
                        Mensaje = Mensaje.Substring(0, 254);
                    }
                    oDataTable.SetValue("Mensaje", index, Mensaje);
                    oRecordset.MoveNext();
                }
            }
        }

        private void consultarDocumentosTodos(string pSquery, ref DataTable oDataTable, string pTop)
        {
            SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            string strSeries = string.Empty;
            string status = string.Empty;
            string Mensaje = string.Empty;
            //DataTable odtable = null;

            strSeries = "SELECT TOP " + pTop + " '01' AS \"TipoDocumento\", 'Factura Electrónica' AS \"DesDocumento\", \"DocEntry\", \"DocNum\", " +
            " \"FolioNum\", \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR) AS \"U_SCGCE_Des\" " +
            " FROM \"OINV\" WHERE " + pSquery + " AND \"Indicator\" = '33' And \"DocSubType\" = '--' And \"isIns\" = 'N' UNION " +
             " SELECT TOP " + pTop + " '01' AS \"TipoDocumento\", 'Factura Reserva Electrónica' AS \"DesDocumento\", \"DocEntry\", \"DocNum\",  " +
            " \"FolioNum\", \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"OINV\" WHERE " + pSquery + " AND \"Indicator\" = '33' And \"DocSubType\" = '--' And \"isIns\" = 'Y' UNION " +
            " SELECT TOP " + pTop + " '01' AS \"TipoDocumento\", 'Factura Anticipo Electrónica' AS \"DesDocumento\", \"DocEntry\", \"DocNum\",  " +
            " \"FolioNum\", \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"ODPI\" WHERE " + pSquery + " AND \"Indicator\" = '33' And \"DocSubType\" = '--' UNION " +
            " SELECT TOP " + pTop + " '01' AS \"TipoDocumento\", 'Factura Exportación' AS \"DesDocumento\", \"DocEntry\", \"DocNum\",  " +
            " \"FolioNum\", \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"OINV\" WHERE " + pSquery + " AND \"Indicator\" = '10' And \"DocSubType\" = 'IX' UNION " +
            " SELECT TOP " + pTop + " '02' AS \"TipoDocumento\", 'Factura Exenta Electrónica' AS \"DesDocumento\", \"DocEntry\", \"DocNum\",  " +
            " \"FolioNum\", \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"OINV\" WHERE " + pSquery + " AND \"Indicator\" = '34' And \"DocSubType\" = 'IE' UNION " +
            " SELECT TOP " + pTop + " '03' AS \"TipoDocumento\", 'Boleta Electrónica' AS \"DesDocumento\", \"DocEntry\", \"DocNum\", \"FolioNum\", " +
            " \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"OINV\" WHERE " + pSquery + " AND \"Indicator\" = '39' And \"DocSubType\" = 'IB' UNION " +
            " SELECT TOP " + pTop + " '04' AS \"TipoDocumento\", 'Boleta Exenta Electrónica' AS \"DesDocumento\", \"DocEntry\", \"DocNum\", \"FolioNum\", " +
            " \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"OINV\" WHERE " + pSquery + " AND \"Indicator\" = '41' And \"DocSubType\" = 'EB' UNION " +
            " SELECT TOP " + pTop + " '06' AS \"TipoDocumento\", 'Nota Débito Electrónica' AS \"DesDocumento\",  \"DocEntry\", \"DocNum\", \"FolioNum\", " +
            " \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"OINV\" WHERE " + pSquery + " AND (\"Indicator\" = '56' OR \"Indicator\" = '11') And \"DocSubType\" = 'DN' UNION " +
            " SELECT TOP " + pTop + " '05' AS \"TipoDocumento\", 'Factura Compra Electrónica' AS \"DesDocumento\",  \"DocEntry\", \"DocNum\", \"FolioNum\", " +
            " \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"OPCH\" WHERE " + pSquery + " AND \"Indicator\" = '46' And \"DocSubType\" = '--' UNION " +
            " SELECT TOP " + pTop + " '07' AS \"TipoDocumento\", 'Nota Crédito Electrónica' AS \"DesDocumento\", \"DocEntry\", \"DocNum\", \"FolioNum\", " +
            " \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"ORIN\" WHERE " + pSquery + " AND (\"Indicator\" = '61' OR \"Indicator\" = '12') And \"DocSubType\" = '--' UNION " +
            " SELECT TOP " + pTop + " '08' AS \"TipoDocumento\", 'Guía Despacho Electrónica TS' AS \"DesDocumento\",  \"DocEntry\", \"DocNum\", \"FolioNum\", " +
            " \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"OWTR\" WHERE " + pSquery + " AND \"U_SCGCE_EnviaTranf\" = 'Y' And \"DocSubType\" = '--' UNION " +
            " SELECT TOP " + pTop + " '08' AS \"TipoDocumento\", 'Guía Despacho Electrónica EN' AS \"DesDocumento\",  \"DocEntry\", \"DocNum\", \"FolioNum\", " +
            " \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"ODLN\" WHERE " + pSquery + " AND \"Indicator\" = '52' And \"DocSubType\" = '--' UNION " +
            " SELECT TOP " + pTop + " '08' AS \"TipoDocumento\", 'Guía Despacho Electrónica DM' AS \"DesDocumento\",  \"DocEntry\", \"DocNum\", \"FolioNum\", " +
            " \"CardCode\", \"CardName\", \"U_SCGCE_EstadoAut\", CAST (\"U_SCGCE_Des\" AS VARCHAR)AS \"U_SCGCE_Des\" " +
            " FROM \"ORPD\" WHERE " + pSquery + " AND \"Indicator\" = '52' And \"DocSubType\" = '--'";

            oRecordset.DoQuery(strSeries);


            if (!oRecordset.EoF)
            {
                oRecordset.MoveFirst();
                for (int i = 0; i < oRecordset.RecordCount; i++)
                {
                    oDataTable.Rows.Add();
                    int index = oDataTable.Rows.Count - 1;
                    oDataTable.SetValue("Tipo Doc", index, oRecordset.Fields.Item("TipoDocumento").Value.ToString());
                    oDataTable.SetValue("Des Doc", index, oRecordset.Fields.Item("DesDocumento").Value.ToString());
                    oDataTable.SetValue("ID interno", index, oRecordset.Fields.Item("DocEntry").Value.ToString());
                    oDataTable.SetValue("Num Doc", index, oRecordset.Fields.Item("DocNum").Value.ToString());
                    oDataTable.SetValue("Folio", index, oRecordset.Fields.Item("FolioNum").Value.ToString());
                    oDataTable.SetValue("ID Cliente", index, oRecordset.Fields.Item("CardCode").Value.ToString());
                    oDataTable.SetValue("Nombre", index, oRecordset.Fields.Item("CardName").Value.ToString());
                    oDataTable.SetValue("Cod Resp", index, oRecordset.Fields.Item("U_SCGCE_EstadoAut").Value.ToString());
                    Mensaje = oRecordset.Fields.Item("U_SCGCE_Des").Value.ToString();
                    if (Mensaje.Length > 254)
                    {
                        Mensaje = Mensaje.Substring(0, 254);
                    }
                    oDataTable.SetValue("Mensaje", index, Mensaje);
                    oRecordset.MoveNext();
                }
            }
        }

        //private string LengthServerType()
        //{
        //    return SCG.DAC.SqlConvert.CToSQLLength(oServerType);
        //}

        /// <summary>
        /// Método que maneja los errores del sistema
        /// </summary>
        /// <param name="ex">Excepción lanzada por el sistema</param>
        public void ManejoErrores(Exception ex)
        {
            var description = String.Empty;
            try
            {
                Exception exception = new Exception();
                exception = ex;
                // Company.ApplicationSBO.StatusBar.SetText(exception.Message);
                oSboAplication.StatusBar.SetText("Error encontrado: " + exception.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                description = exception.InnerException != null ? String.Format("{0} <br /><br />InnerException: {1} <br /><br /> Stack trace: {2}", exception.Message, exception.InnerException.Message, exception.StackTrace) : String.Format("{0} <br /><br /> Stack trace: {1}", exception.Message, exception.StackTrace);
                String mensaje = "<html><body><fieldset><legend><b>Detalle: </b></legend>" + description + " <br /></fieldset>" + "</body></html>";

                LogText(exception);
            }
            catch (Exception)
            {
                oSboAplication.StatusBar.SetText("Error encontrado en el ManejoErrores general", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }

        }

        /// <summary>
        /// Maneja la conversión para poder comparar fechas a nivel de SQL en formato yyyyMMdd mm:hh:ss
        /// </summary>
        /// <param name="fecha">Valor de tipo fecha</param>
        /// <returns>La fecha convertida en formato yyyyMMdd mm:hh:ss</returns>
        public string CToSQLAnsiDate(DateTime fecha)
        {
            return SCG.DAC.SqlConvert.CToSQLDate(fecha, oServerType);
        }

        /// <summary>
        /// Método que almacena en log XML los errores lanzados por el sistema
        /// </summary>
        /// <param name="ex">Excepción lanzada por el sistema</param>
        private static void LogText(Exception ex)
        {
            //obtenemos sólo la carpeta (quitamos el ejecutable) 
            string carpeta = Path.GetTempPath();
            //string defaultFileName = String.Format("{0}{1}.txt", "Logs - ", DateTime.Now.ToString("dd-MM-yyyy"));
            string defaultXmlName = String.Format("{0}{1}.xml", "XML Logs - ", DateTime.Now.ToString("dd-MM-yyyy"));

            //String rutaFichero = Path.Combine(carpeta, defaultFileName);
            String rutaXML = Path.Combine(carpeta, defaultXmlName);
            List<string> error = new List<string>();

            error.Add(String.Format("[{0}] -- Error: {1}", DateTime.Now, ex.Message));
            if (ex.InnerException != null)
                error.Add("InnerException: " + ex.InnerException);
            error.Add("StackTrace: " + ex.StackTrace);
            var lines = new List<string>();
            try
            {

                var writer = new XmlSerializer(typeof(LogErrores));
                var logErrores = new LogErrores();
                if (File.Exists(rutaXML))
                {
                    using (var reader = File.OpenText(rutaXML))
                    {
                        logErrores = (LogErrores)writer.Deserialize(reader);
                        var err = new Error();
                        err.TipoExcepcion = ex.GetType().Name;
                        err.Aplicacion = "SCG CE Addon";
                        err.Codigo = Marshal.GetExceptionCode().ToString();
                        err.CompañiaSBO = "";
                        err.Mensaje = ex.Message;
                        err.StackTrace = ex.StackTrace;
                        err.Fecha = DateTime.Now.ToString();
                        if (logErrores.Errores.Count > 0)
                        {
                            logErrores.Errores.Add(err);
                        }
                        else
                        {
                            var errs = new List<Error>();
                            errs.Add(err);
                            logErrores.Errores = errs;
                        }
                    }
                }
                else
                {
                    logErrores.Idioma = CultureInfo.CurrentCulture.Name;
                    var errs = new List<Error>();
                    var err = new Error();
                    err.TipoExcepcion = ex.GetType().Name;
                    err.Aplicacion = "SCG CE Addon";
                    err.Codigo = Marshal.GetExceptionCode().ToString();
                    err.CompañiaSBO = "";
                    err.Mensaje = ex.Message;
                    err.StackTrace = ex.StackTrace;
                    err.Fecha = DateTime.Now.ToString("dd-MM-yyyy");
                    if (ex.InnerException != null)
                    {
                        err.InnerException = ex.InnerException.Message;
                    }
                    else
                    {
                        err.InnerException = String.Empty;
                    }
                    errs.Add(err);
                    logErrores.Errores = errs;
                }


                var file = new StreamWriter(rutaXML);
                writer.Serialize(file, logErrores);
                file.Close();
            }
            catch (Exception exception)
            {
            }
        }
        #region ...Errores...
        [Serializable]
        public class LogErrores
        {
            public string Idioma;
            public List<Error> Errores;
        }
        public class Error
        {
            public string TipoExcepcion;
            public string Codigo;
            public string Mensaje;
            public string Aplicacion;
            public string CompañiaSBO;
            public string Fecha;
            public string StackTrace;
            public string InnerException;
        }
        #endregion
    }
}