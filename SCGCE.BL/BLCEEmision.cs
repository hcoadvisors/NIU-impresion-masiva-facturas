using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCGCE.BL.facele;
using SCGCE.BL.Objetos;
using SAPbobsCOM;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using SCG.DAC;
using System.Diagnostics;

namespace SCGCE.BL
{
    public class BLCEEmision
    {
        private SAPbobsCOM.Company oCompany;
        private DbHelper oDbHelper = new DbHelper();
        private string tipoDoc;
        private NumberFormatInfo numberFormat;
        private string UrlWS = string.Empty;
        private string UsuarioWS = string.Empty;
        private string PassWS = string.Empty;
        public int WSresponse;
        public string WSdescripcion = string.Empty;
        private long WsfolioDte;
        private string pathDocumentos = string.Empty;
        private string RutaDTE = string.Empty;

        #region Inicializa Objetos CE Chile
        private DTE objDte;
        private SCGCE.BL.Objetos.Documento objDocumento;
        private Encabezado objEncabezado;
        private IdDoc objIdDoc;
        private Emisor objEmisor;
        private Receptor objReceptor;
        private Totales objTotales;
        private Detalle objDetalle;
        private DscRcgGlobal objDscRcgGlobal;

        //Exportaciones
        private SCGCE.BL.Objetos.Exportaciones objExportaciones;
        private Transporte objTransporte;
        private Aduana objAduana;
        private OtraMoneda objOtraMoneda;
        #endregion

        public BLCEEmision() { }
        public BLCEEmision(string pathXMLPDF)
        {
            pathDocumentos = pathXMLPDF;
        }

        #region Metodos de Documentos Electrónicos Chile

        private DTE ConsultaFactura(string strKey, string tipoDoc, out string p_comentarios, SAPbobsCOM.Company oCompany)
        {
            try
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRecordsetAct = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string FormatFecha = string.Empty;
                string strConsulta = string.Empty;
                string strControl = string.Empty;
                string bdComp = ObtenerDatos(oCompany);
                string strDetAdic = string.Empty;
                objDte = new DTE();
                objDocumento = new SCGCE.BL.Objetos.Documento();
                objEncabezado = new Encabezado();
                objIdDoc = new IdDoc();
                objEmisor = new Emisor();
                objReceptor = new Receptor();
                objTotales = new Totales();
                objTransporte = new Transporte();
                objAduana = new Aduana();
                p_comentarios = string.Empty;

                //Consulta para vista de encabezado del documento
                strConsulta = "SELECT \"Indicador\",sum(\"Folio\") AS \"Folio\",max(\"FechaDocumento\") AS \"FechaDocumento\",\"CondicionVenta\", " +
                    " \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", \"EmSennas\", \"EmComuna\", " +
                    " \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\",\"ReSennas\", \"DirDest\",\"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", " +
                    " \"ReTelefono\", \"ReCodPaisFax\", \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\", \"NombreTransp\",\"Vendedor\",\"Contacto\",\"Comentarios\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_ENCABEZADO_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pTipoDoc$$','" + tipoDoc + "'),'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"Indicador\", \"CondicionVenta\", \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", " +
                    " \"EmSennas\", \"EmComuna\", \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\",\"DirDest\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", \"ReTelefono\", \"ReCodPaisFax\", " +
                    " \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\", \"NombreTransp\",\"Vendedor\",\"Contacto\",\"Comentarios\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan los objetos requeridos para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar la parte de IdDoc
                    objIdDoc.TipoDTE = oRecordset.Fields.Item("Indicador").Value.ToString();
                    objIdDoc.Folio = oRecordset.Fields.Item("Folio").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaDocumento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchEmis = FormatFecha;
                    objIdDoc.FmaPago = oRecordset.Fields.Item("CondicionVenta").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaVencimiento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchVenc = FormatFecha;
                    //Llenar la parte de Emisor
                    objEmisor.RUTEmisor = oRecordset.Fields.Item("EmNumIdentificacion").Value.ToString();
                    objEmisor.RznSoc = oRecordset.Fields.Item("EmRazonSocial").Value.ToString();
                    objEmisor.GiroEmis = oRecordset.Fields.Item("EmGiroNegocio").Value.ToString();
                    //objEmisor.Acteco = oRecordset.Fields.Item("EmActeco").Value.ToString();

                    //Se agrega recorrido de Actecos 
                    strConsulta = @"Select ""Acteco"" FROM ""_SYS_BIC"".""sap." + bdComp + @"/CA_SCGCE_ACTECOS_Query""";
                    oRecordsetAct.DoQuery(strConsulta);
                    if (oRecordsetAct.RecordCount != 0)
                    {
                        objEmisor.Acteco = new List<string>();
                        for (int i = 0; i < oRecordsetAct.RecordCount; i++)
                        {

                            objEmisor.Acteco.Add(oRecordsetAct.Fields.Item("Acteco").Value.ToString());
                            oRecordsetAct.MoveNext();

                        }
                    }

                    objEmisor.DirOrigen = oRecordset.Fields.Item("EmSennas").Value.ToString();
                    objEmisor.CmnaOrigen = oRecordset.Fields.Item("EmComuna").Value.ToString();
                    objEmisor.CiudadOrigen = oRecordset.Fields.Item("EmCiudad").Value.ToString();
                    objEmisor.CdgVendedor = oRecordset.Fields.Item("Vendedor").Value.ToString();
                    //Llenar la parte de Receptor
                    objReceptor.RUTRecep = oRecordset.Fields.Item("ReNumIdentificacion").Value.ToString();
                    objReceptor.RznSocRecep = oRecordset.Fields.Item("ReRazonSocial").Value.ToString();
                    objReceptor.GiroRecep = oRecordset.Fields.Item("ReGiroNegocio").Value.ToString();
                    objReceptor.DirRecep = oRecordset.Fields.Item("ReSennas").Value.ToString();
                    objReceptor.CmnaRecep = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objReceptor.CiudadRecep = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    objReceptor.Contacto = oRecordset.Fields.Item("Contacto").Value.ToString();
                    objTransporte.DirDest = oRecordset.Fields.Item("DirDest").Value.ToString();
                    objTransporte.CmnaDest = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objTransporte.CiudadDest = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    objAduana.NombreTransp = oRecordset.Fields.Item("NombreTransp").Value.ToString();
                    objTransporte.Aduana = objAduana;
                    //Llenar el encabezado
                    objEncabezado.IdDoc = objIdDoc;
                    objEncabezado.Emisor = objEmisor;
                    objEncabezado.Receptor = objReceptor;
                    objEncabezado.Transporte = objTransporte;

                    //Llenar Informaci{on del WS
                    UrlWS = oRecordset.Fields.Item("URLDll").Value.ToString();
                    UsuarioWS = oRecordset.Fields.Item("UserDll").Value.ToString();
                    PassWS = oRecordset.Fields.Item("PassDll").Value.ToString();
                    p_comentarios = oRecordset.Fields.Item("Comentarios").Value.ToString();
                }

                //Consulta para vista de Totales del documento
                strConsulta = "SELECT sum(\"TotalVentaNeta\") AS \"TotalVentaNeta\", sum(\"TotalGravado\") AS \"TotalGravado\", sum(\"TotalExento\") AS \"TotalExento\", sum(\"PorcentajeImpueto\") AS \"PorcentajeImpueto\", " +
                    " sum(\"TotalImpuesto\") AS \"TotalImpuesto\", sum(\"TotalComprobante\") AS \"TotalComprobante\", sum(\"TipoCambio\") AS \"TipoCambio\", sum(\"PorcentajeDescuento\") AS \"PorcentajeDescuento\", sum(\"DescuentoEncabezado\") AS \"DescuentoEncabezado\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_TOTALCOMPROBANTE_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                objDocumento.DscRcgGlobal = new List<DscRcgGlobal>();
                int Descuento = 0;

                //Se cargan el objeto de totales para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar los Totales
                    objTotales.MntNeto = oRecordset.Fields.Item("TotalVentaNeta").Value.ToString();
                    objTotales.MntExe = oRecordset.Fields.Item("TotalExento").Value.ToString();
                    objTotales.TasaIVA = oRecordset.Fields.Item("PorcentajeImpueto").Value.ToString();
                    objTotales.IVA = oRecordset.Fields.Item("TotalImpuesto").Value.ToString();
                    objTotales.MntTotal = oRecordset.Fields.Item("TotalComprobante").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.Totales = objTotales;
                    //Llenar descuento
                    Descuento = Convert.ToInt32(oRecordset.Fields.Item("PorcentajeDescuento").Value.ToString());
                    if (Descuento != 0)
                    {
                        objDscRcgGlobal = new DscRcgGlobal();
                        objDscRcgGlobal.NroLinDR = "1";
                        objDscRcgGlobal.TpoMov = "D";
                        objDscRcgGlobal.GlosaDR = "Descuento";
                        objDscRcgGlobal.TpoValor = "$";
                        objDscRcgGlobal.ValorDR = oRecordset.Fields.Item("DescuentoEncabezado").Value.ToString();
                        objDocumento.DscRcgGlobal.Add(objDscRcgGlobal);
                    }
                }
                //Llenar el Documento
                objDocumento.Encabezado = objEncabezado;

                //Consulta para vista de Detalle del documento
                strConsulta = "SELECT sum(\"LineNum\"+1) AS \"LineNum\", \"CodigoTipoLinea\", \"Detalle\", \"DetalleAdic\",	\"TipoCodigo\", sum(\"Cantidad\") AS \"Cantidad\", " +
                    " sum(\"PrecioUnitario\") AS \"PrecioUnitario\", sum(\"MontoTotal\") AS \"MontoTotal\", sum(\"MontoTotalLinea\") AS \"MontoTotalLinea\", sum(\"PorcentDescuento\") AS \"PorcentDescuento\", sum(\"MontoDescuento\") AS \"MontoDescuento\", \"UnidadMedida\" AS \"UnidadMedida\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_DETALLE_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"LineNum\", \"CodigoTipoLinea\", \"Detalle\", \"DetalleAdic\", \"TipoCodigo\",\"UnidadMedida\" ORDER BY \"LineNum\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan el objeto para la información de Detalle que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    oRecordset.MoveFirst();
                    objDocumento.Detalle = new List<Detalle>();
                    for (int i = 0; i < oRecordset.RecordCount; i++)
                    {
                        objDetalle = new Detalle();
                        objDetalle.NroLinDet = oRecordset.Fields.Item("LineNum").Value.ToString();
                        objDetalle.CdgItem = new CdgItem();
                        objDetalle.CdgItem.TpoCodigo = oRecordset.Fields.Item("TipoCodigo").Value.ToString();
                        objDetalle.CdgItem.VlrCodigo = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        objDetalle.NmbItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        strDetAdic = oRecordset.Fields.Item("DetalleAdic").Value.ToString();
                        if (!string.IsNullOrEmpty(strDetAdic))
                        {
                            objDetalle.DscItem = strDetAdic;
                        }
                        objDetalle.UnmdItem = oRecordset.Fields.Item("UnidadMedida").Value.ToString();
                        //objDetalle.NmbItem = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        //objDetalle.DscItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        strControl = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        objDetalle.QtyItem = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        objDetalle.PrcItem = strControl.Replace(",", ".");

                        strControl = oRecordset.Fields.Item("MontoDescuento").Value.ToString();
                        objDetalle.DescuentoMonto = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PorcentDescuento").Value.ToString();
                        objDetalle.DescuentoPct = strControl.Replace(",", ".");

                        objDetalle.MontoItem = oRecordset.Fields.Item("MontoTotal").Value.ToString();
                        objDocumento.Detalle.Add(objDetalle);
                        oRecordset.MoveNext();
                    }
                }

                //Consulta para vista de referencias del documento
                strConsulta = "SELECT \"TipoDocNC\", \"NumeroNC\", \"FechaEmisionNC\", \"CodigoNC\", \"RazonNC\", \"DocEntry\", \"VisOrder\" " +
                    "FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEND_REFERENCIA_Query\"('PLACEHOLDER' = ('$$pObjType$$', '13'), 'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                if (oRecordset.RecordCount != 0)
                {
                    objDocumento.Referencia = new List<Referencia>();
                    //Se cargan el objeto de referencia para la información que se va a enviar al XML
                    while (!oRecordset.EoF)
                    {
                        Referencia objReferencia = new Referencia();
                        //Llenar los Totales
                        objReferencia.NroLinRef = (int.Parse(oRecordset.Fields.Item("VisOrder").Value.ToString()) + 1).ToString();
                        objReferencia.TpoDocRef = oRecordset.Fields.Item("TipoDocNC").Value.ToString();
                        objReferencia.FolioRef = oRecordset.Fields.Item("NumeroNC").Value.ToString();
                        FormatFecha = oRecordset.Fields.Item("FechaEmisionNC").Value.ToString();
                        //Se le da formato correcto a las fechas
                        FormatFecha = FormatearFecha(FormatFecha);
                        objReferencia.FchRef = FormatFecha;
                        objReferencia.CodRef = oRecordset.Fields.Item("CodigoNC").Value.ToString();
                        objReferencia.RazonRef = oRecordset.Fields.Item("RazonNC").Value.ToString();
                        //Llenar el encabezado
                        objDocumento.Referencia.Add(objReferencia);
                        oRecordset.MoveNext();
                    }
                }
                objDte.Documento = objDocumento;
                return objDte;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DTE ConsultaFacturaExenta(string strKey, string tipoDoc, out string p_comentarios, SAPbobsCOM.Company oCompany)
        {
            try//Se deben validar los campos para la factura exenta
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRecordsetAct = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string IndExent = string.Empty;
                string FormatFecha = string.Empty;
                string strConsulta = string.Empty;
                string strControl = string.Empty;
                string strDetAdic = string.Empty;
                string bdComp = ObtenerDatos(oCompany);
                objDte = new DTE();
                objDocumento = new SCGCE.BL.Objetos.Documento();
                objEncabezado = new Encabezado();
                objIdDoc = new IdDoc();
                objEmisor = new Emisor();
                objReceptor = new Receptor();
                objTotales = new Totales();
                objTransporte = new Transporte();
                objAduana = new Aduana();
                p_comentarios = string.Empty;

                //Consulta para vista de encabezado del documento
                strConsulta = "SELECT \"Indicador\",sum(\"Folio\") AS \"Folio\",max(\"FechaDocumento\") AS \"FechaDocumento\",\"CondicionVenta\", " +
                    " \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", \"EmSennas\", \"EmComuna\", " +
                    " \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"DirDest\",\"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", " +
                    " \"ReTelefono\", \"ReCodPaisFax\", \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\" , \"NombreTransp\",\"Vendedor\",\"Contacto\",\"Comentarios\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_ENCABEZADO_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pTipoDoc$$','" + tipoDoc + "'),'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"Indicador\", \"CondicionVenta\", \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", " +
                    " \"EmSennas\", \"EmComuna\", \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"DirDest\",\"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", \"ReTelefono\", \"ReCodPaisFax\", " +
                    " \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\" , \"NombreTransp\",\"Vendedor\",\"Contacto\",\"Comentarios\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan los objetos requeridos para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar la parte de IdDoc
                    objIdDoc.TipoDTE = oRecordset.Fields.Item("Indicador").Value.ToString();
                    objIdDoc.Folio = oRecordset.Fields.Item("Folio").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaDocumento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchEmis = FormatFecha;
                    objIdDoc.FmaPago = oRecordset.Fields.Item("CondicionVenta").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaVencimiento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchVenc = FormatFecha;
                    //Llenar la parte de Emisor
                    objEmisor.RUTEmisor = oRecordset.Fields.Item("EmNumIdentificacion").Value.ToString();
                    objEmisor.RznSoc = oRecordset.Fields.Item("EmRazonSocial").Value.ToString();
                    objEmisor.GiroEmis = oRecordset.Fields.Item("EmGiroNegocio").Value.ToString();
                    objEmisor.CdgVendedor = oRecordset.Fields.Item("Vendedor").Value.ToString();
                    //objEmisor.Acteco = oRecordset.Fields.Item("EmActeco").Value.ToString();

                    //Se agrega recorrido de Actecos 
                    strConsulta = @"Select ""Acteco"" FROM ""_SYS_BIC"".""sap." + bdComp + @"/CA_SCGCE_ACTECOS_Query""";
                    oRecordsetAct.DoQuery(strConsulta);
                    if (oRecordsetAct.RecordCount != 0)
                    {
                        objEmisor.Acteco = new List<string>();
                        for (int i = 0; i < oRecordsetAct.RecordCount; i++)
                        {

                            objEmisor.Acteco.Add(oRecordsetAct.Fields.Item("Acteco").Value.ToString());
                            oRecordsetAct.MoveNext();

                        }
                    }

                    objEmisor.DirOrigen = oRecordset.Fields.Item("EmSennas").Value.ToString();
                    objEmisor.CmnaOrigen = oRecordset.Fields.Item("EmComuna").Value.ToString();
                    objEmisor.CiudadOrigen = oRecordset.Fields.Item("EmCiudad").Value.ToString();
                    //Llenar la parte de Receptor
                    objReceptor.RUTRecep = oRecordset.Fields.Item("ReNumIdentificacion").Value.ToString();
                    objReceptor.RznSocRecep = oRecordset.Fields.Item("ReRazonSocial").Value.ToString();
                    objReceptor.GiroRecep = oRecordset.Fields.Item("ReGiroNegocio").Value.ToString();
                    objReceptor.DirRecep = oRecordset.Fields.Item("ReSennas").Value.ToString();
                    objReceptor.CmnaRecep = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objReceptor.CiudadRecep = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    objReceptor.Contacto = oRecordset.Fields.Item("Contacto").Value.ToString();
                    objTransporte.DirDest = oRecordset.Fields.Item("DirDest").Value.ToString();
                    objTransporte.CmnaDest = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objTransporte.CiudadDest = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    objAduana.NombreTransp = oRecordset.Fields.Item("NombreTransp").Value.ToString();
                    objTransporte.Aduana = objAduana;
                    //Llenar el encabezado
                    objEncabezado.IdDoc = objIdDoc;
                    objEncabezado.Emisor = objEmisor;
                    objEncabezado.Receptor = objReceptor;
                    objEncabezado.Transporte = objTransporte;

                    //Llenar Informaci{on del WS
                    UrlWS = oRecordset.Fields.Item("URLDll").Value.ToString();
                    UsuarioWS = oRecordset.Fields.Item("UserDll").Value.ToString();
                    PassWS = oRecordset.Fields.Item("PassDll").Value.ToString();
                    p_comentarios = oRecordset.Fields.Item("Comentarios").Value.ToString();
                }

                //Consulta para vista de Totales del documento
                strConsulta = "SELECT sum(\"TotalVentaNeta\") AS \"TotalVentaNeta\", sum(\"TotalGravado\") AS \"TotalGravado\", sum(\"TotalExento\") AS \"TotalExento\", sum(\"PorcentajeImpueto\") AS \"PorcentajeImpueto\", " +
                    " sum(\"TotalImpuesto\") AS \"TotalImpuesto\", sum(\"TotalComprobante\") AS \"TotalComprobante\", sum(\"TipoCambio\") AS \"TipoCambio\", sum(\"PorcentajeDescuento\") AS \"PorcentajeDescuento\", sum(\"DescuentoEncabezado\") AS \"DescuentoEncabezado\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_TOTALCOMPROBANTE_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                objDocumento.DscRcgGlobal = new List<DscRcgGlobal>();
                int Descuento = 0;

                //Se cargan el objeto de totales para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar los Totales
                    objTotales.MntNeto = oRecordset.Fields.Item("TotalVentaNeta").Value.ToString();
                    objTotales.MntExe = oRecordset.Fields.Item("TotalExento").Value.ToString();
                    objTotales.MntTotal = oRecordset.Fields.Item("TotalComprobante").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.Totales = objTotales;
                    //Llenar descuento
                    Descuento = Convert.ToInt32(oRecordset.Fields.Item("PorcentajeDescuento").Value.ToString());
                    if (Descuento != 0)
                    {
                        objDscRcgGlobal = new DscRcgGlobal();
                        objDscRcgGlobal.NroLinDR = "1";
                        objDscRcgGlobal.TpoMov = "D";
                        objDscRcgGlobal.GlosaDR = "Descuento";
                        objDscRcgGlobal.TpoValor = "$";
                        objDscRcgGlobal.ValorDR = oRecordset.Fields.Item("DescuentoEncabezado").Value.ToString();
                        objDocumento.DscRcgGlobal.Add(objDscRcgGlobal);
                    }
                }
                //Llenar el Documento
                objDocumento.Encabezado = objEncabezado;

                //Consulta para vista de Detalle del documento
                strConsulta = "SELECT sum(\"LineNum\"+1) AS \"LineNum\", \"IndicadorExento\", \"CodigoTipoLinea\", 	" +
                    " \"Detalle\", \"DetalleAdic\",  \"TipoCodigo\", 	sum(\"Cantidad\") AS \"Cantidad\", " +
                    " sum(\"PrecioUnitario\") AS \"PrecioUnitario\", sum(\"MontoTotal\") AS \"MontoTotal\", " +
                    " sum(\"MontoTotalLinea\") AS \"MontoTotalLinea\", sum(\"PorcentDescuento\") AS \"PorcentDescuento\", sum(\"MontoDescuento\") AS \"MontoDescuento\", \"UnidadMedida\" AS \"UnidadMedida\"  " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_DETALLE_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"LineNum\", \"CodigoTipoLinea\", \"IndicadorExento\", \"Detalle\", \"DetalleAdic\", \"TipoCodigo\",\"UnidadMedida\" ORDER BY \"LineNum\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan el objeto para la información de Detalle que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    oRecordset.MoveFirst();
                    objDocumento.Detalle = new List<Detalle>();
                    for (int i = 0; i < oRecordset.RecordCount; i++)
                    {
                        objDetalle = new Detalle();
                        objDetalle.NroLinDet = oRecordset.Fields.Item("LineNum").Value.ToString();
                        IndExent = oRecordset.Fields.Item("IndicadorExento").Value.ToString();
                        if (!string.IsNullOrEmpty(IndExent))
                        {
                            objDetalle.IndExe = IndExent;
                        }
                        objDetalle.CdgItem = new CdgItem();
                        objDetalle.CdgItem.TpoCodigo = oRecordset.Fields.Item("TipoCodigo").Value.ToString();
                        objDetalle.CdgItem.VlrCodigo = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        objDetalle.NmbItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        strDetAdic = oRecordset.Fields.Item("DetalleAdic").Value.ToString();
                        if (!string.IsNullOrEmpty(strDetAdic))
                        {
                            objDetalle.DscItem = strDetAdic;
                        }
                        objDetalle.UnmdItem = oRecordset.Fields.Item("UnidadMedida").Value.ToString();
                        //objDetalle.NmbItem = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        //objDetalle.DscItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        //objDetalle.QtyItem = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        strControl = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        objDetalle.QtyItem = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        objDetalle.PrcItem = strControl.Replace(",", ".");

                        strControl = oRecordset.Fields.Item("MontoDescuento").Value.ToString();
                        objDetalle.DescuentoMonto = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PorcentDescuento").Value.ToString();
                        objDetalle.DescuentoPct = strControl.Replace(",", ".");

                        objDetalle.MontoItem = oRecordset.Fields.Item("MontoTotal").Value.ToString();
                        objDocumento.Detalle.Add(objDetalle);
                        oRecordset.MoveNext();
                    }
                }

                strConsulta = "SELECT \"TipoDocNC\", \"NumeroNC\", \"FechaEmisionNC\", \"CodigoNC\", \"RazonNC\", \"DocEntry\", \"VisOrder\" " +
                    "FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEND_REFERENCIA_Query\"('PLACEHOLDER' = ('$$pObjType$$', '13'), 'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                if (oRecordset.RecordCount != 0)
                {
                    objDocumento.Referencia = new List<Referencia>();
                    //Se cargan el objeto de referencia para la información que se va a enviar al XML
                    while (!oRecordset.EoF)
                    {
                        Referencia objReferencia = new Referencia();
                        //Llenar los Totales
                        objReferencia.NroLinRef = (int.Parse(oRecordset.Fields.Item("VisOrder").Value.ToString()) + 1).ToString();
                        objReferencia.TpoDocRef = oRecordset.Fields.Item("TipoDocNC").Value.ToString();
                        objReferencia.FolioRef = oRecordset.Fields.Item("NumeroNC").Value.ToString();
                        FormatFecha = oRecordset.Fields.Item("FechaEmisionNC").Value.ToString();
                        //Se le da formato correcto a las fechas
                        FormatFecha = FormatearFecha(FormatFecha);
                        objReferencia.FchRef = FormatFecha;
                        objReferencia.CodRef = oRecordset.Fields.Item("CodigoNC").Value.ToString();
                        objReferencia.RazonRef = oRecordset.Fields.Item("RazonNC").Value.ToString();
                        //Llenar el encabezado
                        objDocumento.Referencia.Add(objReferencia);
                        oRecordset.MoveNext();
                    }
                }
                objDte.Documento = objDocumento;
                return objDte;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DTE ConsultaFacturaAnticipo(string strKey, string tipoDoc, out string p_comentarios, SAPbobsCOM.Company oCompany)
        {
            try
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRecordsetAct = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string FormatFecha = string.Empty;
                string strConsulta = string.Empty;
                string strControl = string.Empty;
                string bdComp = ObtenerDatos(oCompany);
                string strDetAdic = string.Empty;
                objDte = new DTE();
                objDocumento = new SCGCE.BL.Objetos.Documento();
                objEncabezado = new Encabezado();
                objIdDoc = new IdDoc();
                objEmisor = new Emisor();
                objReceptor = new Receptor();
                objTotales = new Totales();
                p_comentarios = string.Empty;

                //Consulta para vista de encabezado del documento
                strConsulta = "SELECT \"Indicador\",sum(\"Folio\") AS \"Folio\",max(\"FechaDocumento\") AS \"FechaDocumento\",\"CondicionVenta\", " +
                    " \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", \"EmSennas\", \"EmComuna\", " +
                    " \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\",\"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", " +
                    " \"ReTelefono\", \"ReCodPaisFax\", \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\", \"Comentarios\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEAN_ENCABEZADO_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pTipoDoc$$','" + tipoDoc + "'),'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"Indicador\", \"CondicionVenta\", \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", " +
                    " \"EmSennas\", \"EmComuna\", \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", \"ReTelefono\", \"ReCodPaisFax\", " +
                    " \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\", \"Comentarios\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan los objetos requeridos para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar la parte de IdDoc
                    objIdDoc.TipoDTE = oRecordset.Fields.Item("Indicador").Value.ToString();
                    objIdDoc.Folio = oRecordset.Fields.Item("Folio").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaDocumento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchEmis = FormatFecha;
                    objIdDoc.FmaPago = oRecordset.Fields.Item("CondicionVenta").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaVencimiento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchVenc = FormatFecha;
                    //Llenar la parte de Emisor
                    objEmisor.RUTEmisor = oRecordset.Fields.Item("EmNumIdentificacion").Value.ToString();
                    objEmisor.RznSoc = oRecordset.Fields.Item("EmRazonSocial").Value.ToString();
                    objEmisor.GiroEmis = oRecordset.Fields.Item("EmGiroNegocio").Value.ToString();
                    //objEmisor.Acteco = oRecordset.Fields.Item("EmActeco").Value.ToString();

                    //Se agrega recorrido de Actecos 
                    strConsulta = @"Select ""Acteco"" FROM ""_SYS_BIC"".""sap." + bdComp + @"/CA_SCGCE_ACTECOS_Query""";
                    oRecordsetAct.DoQuery(strConsulta);
                    if (oRecordsetAct.RecordCount != 0)
                    {
                        objEmisor.Acteco = new List<string>();
                        for (int i = 0; i < oRecordsetAct.RecordCount; i++)
                        {

                            objEmisor.Acteco.Add(oRecordsetAct.Fields.Item("Acteco").Value.ToString());
                            oRecordsetAct.MoveNext();

                        }
                    }

                    objEmisor.DirOrigen = oRecordset.Fields.Item("EmSennas").Value.ToString();
                    objEmisor.CmnaOrigen = oRecordset.Fields.Item("EmComuna").Value.ToString();
                    objEmisor.CiudadOrigen = oRecordset.Fields.Item("EmCiudad").Value.ToString();
                    //Llenar la parte de Receptor
                    objReceptor.RUTRecep = oRecordset.Fields.Item("ReNumIdentificacion").Value.ToString();
                    objReceptor.RznSocRecep = oRecordset.Fields.Item("ReRazonSocial").Value.ToString();
                    objReceptor.GiroRecep = oRecordset.Fields.Item("ReGiroNegocio").Value.ToString();
                    objReceptor.DirRecep = oRecordset.Fields.Item("ReSennas").Value.ToString();
                    objReceptor.CmnaRecep = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objReceptor.CiudadRecep = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.IdDoc = objIdDoc;
                    objEncabezado.Emisor = objEmisor;
                    objEncabezado.Receptor = objReceptor;

                    //Llenar Informaci{on del WS
                    UrlWS = oRecordset.Fields.Item("URLDll").Value.ToString();
                    UsuarioWS = oRecordset.Fields.Item("UserDll").Value.ToString();
                    PassWS = oRecordset.Fields.Item("PassDll").Value.ToString();
                    p_comentarios = oRecordset.Fields.Item("Comentarios").Value.ToString();
                }

                //Consulta para vista de Totales del documento
                strConsulta = "SELECT sum(\"TotalVentaNeta\") AS \"TotalVentaNeta\", sum(\"TotalGravado\") AS \"TotalGravado\", sum(\"TotalExento\") AS \"TotalExento\", sum(\"PorcentajeImpueto\") AS \"PorcentajeImpueto\", " +
                    " sum(\"TotalImpuesto\") AS \"TotalImpuesto\", sum(\"TotalComprobante\") AS \"TotalComprobante\", sum(\"TipoCambio\") AS \"TipoCambio\", sum(\"PorcentajeDescuento\") AS \"PorcentajeDescuento\", sum(\"DescuentoEncabezado\") AS \"DescuentoEncabezado\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEAN_TOTALCOMPROBANTE_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                objDocumento.DscRcgGlobal = new List<DscRcgGlobal>();
                int Descuento = 0;

                //Se cargan el objeto de totales para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar los Totales
                    objTotales.MntNeto = oRecordset.Fields.Item("TotalVentaNeta").Value.ToString();
                    objTotales.MntExe = oRecordset.Fields.Item("TotalExento").Value.ToString();
                    objTotales.TasaIVA = oRecordset.Fields.Item("PorcentajeImpueto").Value.ToString();
                    objTotales.IVA = oRecordset.Fields.Item("TotalImpuesto").Value.ToString();
                    objTotales.MntTotal = oRecordset.Fields.Item("TotalComprobante").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.Totales = objTotales;
                    //Llenar descuento
                    Descuento = Convert.ToInt32(oRecordset.Fields.Item("PorcentajeDescuento").Value.ToString());
                    if (Descuento != 0)
                    {
                        objDscRcgGlobal = new DscRcgGlobal();
                        objDscRcgGlobal.NroLinDR = "1";
                        objDscRcgGlobal.TpoMov = "D";
                        objDscRcgGlobal.GlosaDR = "Descuento";
                        objDscRcgGlobal.TpoValor = "$";
                        objDscRcgGlobal.ValorDR = oRecordset.Fields.Item("DescuentoEncabezado").Value.ToString();
                        objDocumento.DscRcgGlobal.Add(objDscRcgGlobal);
                    }
                }
                //Llenar el Documento
                objDocumento.Encabezado = objEncabezado;

                //Consulta para vista de Detalle del documento
                strConsulta = "SELECT sum(\"LineNum\"+1) AS \"LineNum\", \"CodigoTipoLinea\", \"Detalle\", \"DetalleAdic\",	\"TipoCodigo\", sum(\"Cantidad\") AS \"Cantidad\", " +
                    " sum(\"PrecioUnitario\") AS \"PrecioUnitario\", sum(\"MontoTotal\") AS \"MontoTotal\", sum(\"MontoTotalLinea\") AS \"MontoTotalLinea\", sum(\"PorcentDescuento\") AS \"PorcentDescuento\", sum(\"MontoDescuento\") AS \"MontoDescuento\", \"UnidadMedida\" AS \"UnidadMedida\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEAN_DETALLE_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"LineNum\", \"CodigoTipoLinea\", \"Detalle\", \"DetalleAdic\", \"TipoCodigo\",\"UnidadMedida\" ORDER BY \"LineNum\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan el objeto para la información de Detalle que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    oRecordset.MoveFirst();
                    objDocumento.Detalle = new List<Detalle>();
                    for (int i = 0; i < oRecordset.RecordCount; i++)
                    {
                        objDetalle = new Detalle();
                        objDetalle.NroLinDet = oRecordset.Fields.Item("LineNum").Value.ToString();
                        objDetalle.CdgItem = new CdgItem();
                        objDetalle.CdgItem.TpoCodigo = oRecordset.Fields.Item("TipoCodigo").Value.ToString();
                        objDetalle.CdgItem.VlrCodigo = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        objDetalle.NmbItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        strDetAdic = oRecordset.Fields.Item("DetalleAdic").Value.ToString();
                        if (!string.IsNullOrEmpty(strDetAdic))
                        {
                            objDetalle.DscItem = strDetAdic;
                        }
                        objDetalle.UnmdItem = oRecordset.Fields.Item("UnidadMedida").Value.ToString();
                        //objDetalle.NmbItem = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        //objDetalle.DscItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        strControl = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        objDetalle.QtyItem = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        objDetalle.PrcItem = strControl.Replace(",", ".");

                        strControl = oRecordset.Fields.Item("MontoDescuento").Value.ToString();
                        objDetalle.DescuentoMonto = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PorcentDescuento").Value.ToString();
                        objDetalle.DescuentoPct = strControl.Replace(",", ".");

                        objDetalle.MontoItem = oRecordset.Fields.Item("MontoTotal").Value.ToString();
                        objDocumento.Detalle.Add(objDetalle);
                        oRecordset.MoveNext();
                    }
                }

                //Consulta para vista de referencias del documento
                strConsulta = "SELECT \"TipoDocNC\", \"NumeroNC\", \"FechaEmisionNC\", \"CodigoNC\", \"RazonNC\", \"DocEntry\", \"VisOrder\" " +
                    "FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEND_REFERENCIA_Query\"('PLACEHOLDER' = ('$$pObjType$$', '203'), 'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                if (oRecordset.RecordCount != 0)
                {
                    objDocumento.Referencia = new List<Referencia>();
                    //Se cargan el objeto de referencia para la información que se va a enviar al XML
                    while (!oRecordset.EoF)
                    {
                        Referencia objReferencia = new Referencia();
                        //Llenar los Totales
                        objReferencia.NroLinRef = (int.Parse(oRecordset.Fields.Item("VisOrder").Value.ToString()) + 1).ToString();
                        objReferencia.TpoDocRef = oRecordset.Fields.Item("TipoDocNC").Value.ToString();
                        objReferencia.FolioRef = oRecordset.Fields.Item("NumeroNC").Value.ToString();
                        FormatFecha = oRecordset.Fields.Item("FechaEmisionNC").Value.ToString();
                        //Se le da formato correcto a las fechas
                        FormatFecha = FormatearFecha(FormatFecha);
                        objReferencia.FchRef = FormatFecha;
                        objReferencia.CodRef = oRecordset.Fields.Item("CodigoNC").Value.ToString();
                        objReferencia.RazonRef = oRecordset.Fields.Item("RazonNC").Value.ToString();
                        //Llenar el encabezado
                        objDocumento.Referencia.Add(objReferencia);
                        oRecordset.MoveNext();
                    }
                }
                objDte.Documento = objDocumento;
                return objDte;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DTE ConsultaFacturaExport(string strKey, string tipoDoc, out string p_comentarios, SAPbobsCOM.Company oCompany)
        {
            try
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRecordsetAct = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string FormatFecha = string.Empty;
                string strConsulta = string.Empty;
                string strControl = string.Empty;
                string bdComp = ObtenerDatos(oCompany);
                string strDetAdic = string.Empty;
                objDte = new DTE();
                objExportaciones = new SCGCE.BL.Objetos.Exportaciones();
                objEncabezado = new Encabezado();
                objIdDoc = new IdDoc();
                objEmisor = new Emisor();
                objReceptor = new Receptor();
                objTotales = new Totales();
                objTransporte = new Transporte();
                objAduana = new Aduana();
                objOtraMoneda = new OtraMoneda();
                p_comentarios = string.Empty;

                //Consulta para vista de encabezado del documento
                strConsulta = "SELECT \"Indicador\",sum(\"Folio\") AS \"Folio\",\"IndicadorServicio\",max(\"FechaDocumento\") AS \"FechaDocumento\",\"CondicionVenta\", " +
                    " \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", \"EmSennas\", \"EmComuna\", " +
                    " \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\",\"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", " +
                    " \"ReTelefono\", \"ReCodPaisFax\", \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\", \"Comentarios\", \"ReCodigoInterno\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_ENCABEZADO_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pTipoDoc$$','" + tipoDoc + "'),'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"Indicador\",\"IndicadorServicio\", \"CondicionVenta\", \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", " +
                    " \"EmSennas\", \"EmComuna\", \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", \"ReTelefono\", \"ReCodPaisFax\", " +
                    " \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\", \"Comentarios\", \"ReCodigoInterno\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan los objetos requeridos para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar la parte de IdDoc
                    objIdDoc.TipoDTE = "1" + oRecordset.Fields.Item("Indicador").Value.ToString();
                    objIdDoc.Folio = oRecordset.Fields.Item("Folio").Value.ToString();
                    objIdDoc.IndServicio = oRecordset.Fields.Item("IndicadorServicio").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaDocumento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchEmis = FormatFecha;
                    objIdDoc.FmaPago = oRecordset.Fields.Item("CondicionVenta").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaVencimiento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchVenc = FormatFecha;
                    //Llenar la parte de Emisor
                    objEmisor.RUTEmisor = oRecordset.Fields.Item("EmNumIdentificacion").Value.ToString();
                    objEmisor.RznSoc = oRecordset.Fields.Item("EmRazonSocial").Value.ToString();
                    objEmisor.GiroEmis = oRecordset.Fields.Item("EmGiroNegocio").Value.ToString();
                    //objEmisor.Acteco = oRecordset.Fields.Item("EmActeco").Value.ToString();

                    //Se agrega recorrido de Actecos 
                    strConsulta = @"Select ""Acteco"" FROM ""_SYS_BIC"".""sap." + bdComp + @"/CA_SCGCE_ACTECOS_Query""";
                    oRecordsetAct.DoQuery(strConsulta);
                    if (oRecordsetAct.RecordCount != 0)
                    {
                        objEmisor.Acteco = new List<string>();
                        for (int i = 0; i < oRecordsetAct.RecordCount; i++)
                        {

                            objEmisor.Acteco.Add(oRecordsetAct.Fields.Item("Acteco").Value.ToString());
                            oRecordsetAct.MoveNext();

                        }
                    }

                    objEmisor.DirOrigen = oRecordset.Fields.Item("EmSennas").Value.ToString();
                    objEmisor.CmnaOrigen = oRecordset.Fields.Item("EmComuna").Value.ToString();
                    objEmisor.CiudadOrigen = oRecordset.Fields.Item("EmCiudad").Value.ToString();
                    //Llenar la parte de Receptor
                    objReceptor.RUTRecep = "55555555-5";
                    objReceptor.CdgIntRecep = oRecordset.Fields.Item("ReCodigoInterno").Value.ToString();
                    objReceptor.RznSocRecep = oRecordset.Fields.Item("ReRazonSocial").Value.ToString();
                    objReceptor.GiroRecep = oRecordset.Fields.Item("ReGiroNegocio").Value.ToString();
                    objReceptor.DirRecep = oRecordset.Fields.Item("ReSennas").Value.ToString();
                    objReceptor.CmnaRecep = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objReceptor.CiudadRecep = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.IdDoc = objIdDoc;
                    objEncabezado.Emisor = objEmisor;
                    objEncabezado.Receptor = objReceptor;

                    //Llenar Informaci{on del WS
                    UrlWS = oRecordset.Fields.Item("URLDll").Value.ToString();
                    UsuarioWS = oRecordset.Fields.Item("UserDll").Value.ToString();
                    PassWS = oRecordset.Fields.Item("PassDll").Value.ToString();
                    p_comentarios = oRecordset.Fields.Item("Comentarios").Value.ToString();
                }

                //Consulta para vista de Totales del documento
                strConsulta = "SELECT sum(\"TotalVentaNeta\") AS \"TotalVentaNeta\", sum(\"TotalGravado\") AS \"TotalGravado\", sum(\"TotalExento\") AS \"TotalExento\", sum(\"PorcentajeImpueto\") AS \"PorcentajeImpueto\", " +
                    " sum(\"TotalImpuesto\") AS \"TotalImpuesto\", sum(\"TotalComprobante\") AS \"TotalComprobante\", sum(\"TipoCambio\") AS \"TipoCambio\", sum(\"PorcentajeDescuento\") AS \"PorcentajeDescuento\", sum(\"DescuentoEncabezado\") AS \"DescuentoEncabezado\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_TOTALCOMPROBANTE_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                objExportaciones.DscRcgGlobal = new List<DscRcgGlobal>();
                int Descuento = 0;

                //Se cargan el objeto de totales para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar los Totales
                    //objTotales.MntNeto = oRecordset.Fields.Item("TotalVentaNeta").Value.ToString();
                    objTotales.MntExe = oRecordset.Fields.Item("TotalExento").Value.ToString();
                    //objTotales.TasaIVA = oRecordset.Fields.Item("PorcentajeImpueto").Value.ToString();
                    //objTotales.IVA = oRecordset.Fields.Item("TotalImpuesto").Value.ToString();
                    objTotales.MntTotal = oRecordset.Fields.Item("TotalComprobante").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.Totales = objTotales;
                    //Llenar descuento
                    Descuento = Convert.ToInt32(oRecordset.Fields.Item("PorcentajeDescuento").Value.ToString());
                    if (Descuento != 0)
                    {
                        objDscRcgGlobal = new DscRcgGlobal();
                        objDscRcgGlobal.NroLinDR = "1";
                        objDscRcgGlobal.TpoMov = "D";
                        objDscRcgGlobal.GlosaDR = "Descuento";
                        objDscRcgGlobal.TpoValor = "$";
                        objDscRcgGlobal.ValorDR = oRecordset.Fields.Item("DescuentoEncabezado").Value.ToString();
                        objExportaciones.DscRcgGlobal.Add(objDscRcgGlobal);
                    }
                }
                //Llenar el Documento
                objExportaciones.Encabezado = objEncabezado;

                //Consulta para vista de Detalle del documento
                strConsulta = "SELECT sum(\"LineNum\"+1) AS \"LineNum\", \"CodigoTipoLinea\", \"Detalle\", \"DetalleAdic\",	\"TipoCodigo\", sum(\"Cantidad\") AS \"Cantidad\", " +
                    " sum(\"PrecioUnitario\") AS \"PrecioUnitario\", sum(\"MontoTotal\") AS \"MontoTotal\", sum(\"MontoTotalLinea\") AS \"MontoTotalLinea\", sum(\"PorcentDescuento\") AS \"PorcentDescuento\", sum(\"MontoDescuento\") AS \"MontoDescuento\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_DETALLE_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"LineNum\", \"CodigoTipoLinea\", \"Detalle\", \"DetalleAdic\", \"TipoCodigo\" ORDER BY \"LineNum\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan el objeto para la información de Detalle que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    oRecordset.MoveFirst();
                    objExportaciones.Detalle = new List<Detalle>();
                    for (int i = 0; i < oRecordset.RecordCount; i++)
                    {
                        objDetalle = new Detalle();
                        objDetalle.NroLinDet = oRecordset.Fields.Item("LineNum").Value.ToString();
                        objDetalle.CdgItem = new CdgItem();
                        objDetalle.CdgItem.TpoCodigo = oRecordset.Fields.Item("TipoCodigo").Value.ToString();
                        objDetalle.CdgItem.VlrCodigo = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        objDetalle.NmbItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        strDetAdic = oRecordset.Fields.Item("DetalleAdic").Value.ToString();
                        if (!string.IsNullOrEmpty(strDetAdic))
                        {
                            objDetalle.DscItem = strDetAdic;
                        }
                        //objDetalle.NmbItem = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        //objDetalle.DscItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        strControl = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        objDetalle.QtyItem = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        objDetalle.PrcItem = strControl.Replace(",", ".");
                        if (oRecordset.Fields.Item("PorcentDescuento").Value.ToString() != "0")
                        {
                            strControl = oRecordset.Fields.Item("MontoDescuento").Value.ToString();
                            objDetalle.DescuentoMonto = strControl.Replace(",", ".");
                            strControl = oRecordset.Fields.Item("PorcentDescuento").Value.ToString();
                            objDetalle.DescuentoPct = strControl.Replace(",", ".");
                        }

                        objDetalle.MontoItem = oRecordset.Fields.Item("MontoTotal").Value.ToString();
                        objExportaciones.Detalle.Add(objDetalle);
                        oRecordset.MoveNext();
                    }
                }

                //Consulta para vista de Exportación
                strConsulta = "SELECT \"U_SCGCE_TipoDesp\",\"U_SCGCE_CondVentExp\",\"U_SCGCE_ModVenta\",\"U_SCGCE_PaisDestino\",\"U_SCGCE_TpoMoneda\", " +
                               " \"TipoMoneda\",sum(\"U_SCGCE_TotBultos\") AS \"U_SCGCE_TotBultos\",sum(\"DocRate\") AS \"DocRate\",sum(\"MntTotOtraMond\") AS \"MntTotOtraMond\",\"U_SCGCE_LugEmbarq\",\"U_SCGCE_ViaTrans\" " +
                               "FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_ENCABEZADOEXP_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                                "GROUP BY \"U_SCGCE_TipoDesp\",\"U_SCGCE_CondVentExp\",\"U_SCGCE_ModVenta\",\"U_SCGCE_PaisDestino\",\"U_SCGCE_TpoMoneda\",\"TipoMoneda\" ,\"U_SCGCE_LugEmbarq\",\"U_SCGCE_ViaTrans\"";

                oRecordset.DoQuery(strConsulta);

                //Se cargan el objeto de Exportación ara la factura
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar los Totales
                    string Valor = string.Empty;
                    Valor = oRecordset.Fields.Item("U_SCGCE_TipoDesp").Value.ToString();
                    if (!string.IsNullOrEmpty(Valor) || !string.IsNullOrWhiteSpace(Valor))
                    {
                        if (Valor != "0")
                        {
                            objEncabezado.IdDoc.TipoDespacho = oRecordset.Fields.Item("U_SCGCE_TipoDesp").Value.ToString();
                        }
                    }
                    if (!string.IsNullOrEmpty(oRecordset.Fields.Item("U_SCGCE_CondVentExp").Value.ToString()) || !string.IsNullOrWhiteSpace(oRecordset.Fields.Item("U_SCGCE_CondVentExp").Value.ToString()))
                    {
                        if (oRecordset.Fields.Item("U_SCGCE_CondVentExp").Value.ToString() != "0")
                        {
                            objEncabezado.IdDoc.FmaPagExp = oRecordset.Fields.Item("U_SCGCE_CondVentExp").Value.ToString();
                        }
                    }
                    if (!string.IsNullOrEmpty(oRecordset.Fields.Item("U_SCGCE_ModVenta").Value.ToString()) || !string.IsNullOrWhiteSpace(oRecordset.Fields.Item("U_SCGCE_ModVenta").Value.ToString()))
                    {
                        if (oRecordset.Fields.Item("U_SCGCE_ModVenta").Value.ToString() != "0")
                        {
                            objAduana.CodModVenta = oRecordset.Fields.Item("U_SCGCE_ModVenta").Value.ToString();
                        }
                    }
                    if (oRecordset.Fields.Item("U_SCGCE_TotBultos").Value.ToString() != "0")
                    {
                        objAduana.TotBultos = oRecordset.Fields.Item("U_SCGCE_TotBultos").Value.ToString();
                    }


                    objAduana.CodPaisDestin = oRecordset.Fields.Item("U_SCGCE_PaisDestino").Value.ToString();
                    objAduana.CodPtoEmbarque = oRecordset.Fields.Item("U_SCGCE_LugEmbarq").Value.ToString();
                    objAduana.CodViaTransp = oRecordset.Fields.Item("U_SCGCE_ViaTrans").Value.ToString();
                    objTotales.TpoMoneda = oRecordset.Fields.Item("U_SCGCE_TpoMoneda").Value.ToString();
                    objOtraMoneda.TpoMoneda = oRecordset.Fields.Item("TipoMoneda").Value.ToString();
                    objOtraMoneda.TpoCambio = oRecordset.Fields.Item("DocRate").Value.ToString();
                    objOtraMoneda.MntExeOtrMnda = oRecordset.Fields.Item("MntTotOtraMond").Value.ToString();
                    objOtraMoneda.MntTotOtrMnda = oRecordset.Fields.Item("MntTotOtraMond").Value.ToString();
                    objTransporte.Aduana = objAduana;
                    objEncabezado.Transporte = objTransporte;
                    objEncabezado.OtraMoneda = objOtraMoneda;
                }

                //Consulta para vista de referencias del documento
                strConsulta = "SELECT \"TipoDocNC\", \"NumeroNC\", \"FechaEmisionNC\", \"CodigoNC\", \"RazonNC\", \"DocEntry\", \"VisOrder\" " +
                    "FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEND_REFERENCIA_Query\"('PLACEHOLDER' = ('$$pObjType$$', '203'), 'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                if (oRecordset.RecordCount != 0)
                {
                    objDocumento.Referencia = new List<Referencia>();
                    //Se cargan el objeto de referencia para la información que se va a enviar al XML
                    while (!oRecordset.EoF)
                    {
                        Referencia objReferencia = new Referencia();
                        //Llenar los Totales
                        objReferencia.NroLinRef = (int.Parse(oRecordset.Fields.Item("VisOrder").Value.ToString()) + 1).ToString();
                        objReferencia.TpoDocRef = oRecordset.Fields.Item("TipoDocNC").Value.ToString();
                        objReferencia.FolioRef = oRecordset.Fields.Item("NumeroNC").Value.ToString();
                        FormatFecha = oRecordset.Fields.Item("FechaEmisionNC").Value.ToString();
                        //Se le da formato correcto a las fechas
                        FormatFecha = FormatearFecha(FormatFecha);
                        objReferencia.FchRef = FormatFecha;
                        objReferencia.CodRef = oRecordset.Fields.Item("CodigoNC").Value.ToString();
                        objReferencia.RazonRef = oRecordset.Fields.Item("RazonNC").Value.ToString();
                        //Llenar el encabezado
                        objDocumento.Referencia.Add(objReferencia);
                        oRecordset.MoveNext();
                    }
                }

                objDte.Exportaciones = objExportaciones;
                return objDte;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DTE ConsultaBoleta(string strKey, string tipoDoc, out string p_comentarios, SAPbobsCOM.Company oCompany)
        {
            try//Se deben validar los campos para la Boleta
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string FormatFecha = string.Empty;
                string strConsulta = string.Empty;
                string strControl = string.Empty;
                string strDetAdic = string.Empty;
                string bdComp = ObtenerDatos(oCompany);
                objDte = new DTE();
                objDocumento = new SCGCE.BL.Objetos.Documento();
                objEncabezado = new Encabezado();
                objIdDoc = new IdDoc();
                objEmisor = new Emisor();
                objReceptor = new Receptor();
                objTotales = new Totales();
                p_comentarios = string.Empty;

                //Consulta para vista de encabezado del documento
                strConsulta = "SELECT \"Indicador\", sum(\"Folio\") AS \"Folio\", \"IndicadorServicio\",max(\"FechaDocumento\") AS \"FechaDocumento\",\"CondicionVenta\", " +
                    " \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", \"EmSennas\", \"EmComuna\", " +
                    " \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReCodigoInterno\",\"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", " +
                    " \"ReTelefono\", \"ReCodPaisFax\", \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\" , \"Comentarios\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_ENCABEZADO_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pTipoDoc$$','" + tipoDoc + "'),'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"Indicador\", \"IndicadorServicio\", \"CondicionVenta\", \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", " +
                    " \"EmSennas\", \"EmComuna\", \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReCodigoInterno\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", \"ReTelefono\", \"ReCodPaisFax\", " +
                    " \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\" , \"Comentarios\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan los objetos requeridos para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar la parte de IdDoc
                    objIdDoc.TipoDTE = oRecordset.Fields.Item("Indicador").Value.ToString();
                    objIdDoc.Folio = oRecordset.Fields.Item("Folio").Value.ToString();
                    objIdDoc.IndServicio = oRecordset.Fields.Item("IndicadorServicio").Value.ToString();
                    objIdDoc.IndMntNeto = "2";
                    FormatFecha = oRecordset.Fields.Item("FechaDocumento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchEmis = FormatFecha;
                    //Llenar la parte de Emisor
                    objEmisor.RUTEmisor = oRecordset.Fields.Item("EmNumIdentificacion").Value.ToString();
                    objEmisor.RznSoc = oRecordset.Fields.Item("EmRazonSocial").Value.ToString();
                    objEmisor.GiroEmis = oRecordset.Fields.Item("EmGiroNegocio").Value.ToString();
                    objEmisor.DirOrigen = oRecordset.Fields.Item("EmSennas").Value.ToString();
                    objEmisor.CmnaOrigen = oRecordset.Fields.Item("EmComuna").Value.ToString();
                    objEmisor.CiudadOrigen = oRecordset.Fields.Item("EmCiudad").Value.ToString();
                    //Llenar la parte de Receptor
                    objReceptor.RUTRecep = oRecordset.Fields.Item("ReNumIdentificacion").Value.ToString();
                    objReceptor.CdgIntRecep = oRecordset.Fields.Item("ReCodigoInterno").Value.ToString();
                    objReceptor.RznSocRecep = oRecordset.Fields.Item("ReRazonSocial").Value.ToString();
                    objReceptor.Contacto = oRecordset.Fields.Item("ReEmail").Value.ToString();
                    objReceptor.DirRecep = oRecordset.Fields.Item("ReSennas").Value.ToString();
                    objReceptor.CmnaRecep = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objReceptor.CiudadRecep = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.IdDoc = objIdDoc;
                    objEncabezado.Emisor = objEmisor;
                    objEncabezado.Receptor = objReceptor;

                    //Llenar Informaci{on del WS
                    UrlWS = oRecordset.Fields.Item("URLDll").Value.ToString();
                    UsuarioWS = oRecordset.Fields.Item("UserDll").Value.ToString();
                    PassWS = oRecordset.Fields.Item("PassDll").Value.ToString();
                    p_comentarios = oRecordset.Fields.Item("Comentarios").Value.ToString();
                }

                //Consulta para vista de Totales del documento
                strConsulta = "SELECT sum(\"TotalVentaNeta\") AS \"TotalVentaNeta\", sum(\"TotalGravado\") AS \"TotalGravado\", sum(\"TotalExento\") AS \"TotalExento\", sum(\"PorcentajeImpueto\") AS \"PorcentajeImpueto\", " +
                    " sum(\"TotalImpuesto\") AS \"TotalImpuesto\", sum(\"TotalComprobante\") AS \"TotalComprobante\", sum(\"TipoCambio\") AS \"TipoCambio\", sum(\"PorcentajeDescuento\") AS \"PorcentajeDescuento\", sum(\"DescuentoEncabezado\") AS \"DescuentoEncabezado\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_TOTALCOMPROBANTE_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                objDocumento.DscRcgGlobal = new List<DscRcgGlobal>();
                int Descuento = 0;

                //Se cargan el objeto de totales para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar los Totales 
                    objTotales.MntNeto = oRecordset.Fields.Item("TotalVentaNeta").Value.ToString();
                    objTotales.IVA = oRecordset.Fields.Item("TotalImpuesto").Value.ToString();
                    objTotales.MntTotal = oRecordset.Fields.Item("TotalComprobante").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.Totales = objTotales;
                    //Llenar descuento
                    Descuento = Convert.ToInt32(oRecordset.Fields.Item("PorcentajeDescuento").Value.ToString());
                    if (Descuento != 0)
                    {
                        objDscRcgGlobal = new DscRcgGlobal();
                        objDscRcgGlobal.NroLinDR = "1";
                        objDscRcgGlobal.TpoMov = "D";
                        objDscRcgGlobal.GlosaDR = "Descuento";
                        objDscRcgGlobal.TpoValor = "$";
                        objDscRcgGlobal.ValorDR = oRecordset.Fields.Item("DescuentoEncabezado").Value.ToString();
                        objDocumento.DscRcgGlobal.Add(objDscRcgGlobal);
                    }
                }
                //Llenar el Documento
                objDocumento.Encabezado = objEncabezado;

                //Consulta para vista de Detalle del documento
                strConsulta = "SELECT sum(\"LineNum\"+1) AS \"LineNum\", 	\"CodigoTipoLinea\", 	\"Detalle\", \"DetalleAdic\",	\"TipoCodigo\",	sum(\"Cantidad\") AS \"Cantidad\", " +
                    " sum(\"PrecioUnitario\") AS \"PrecioUnitario\", sum(\"MontoTotal\") AS \"MontoTotal\", sum(\"MontoTotalLinea\") AS \"MontoTotalLinea\", sum(\"PorcentDescuento\") AS \"PorcentDescuento\", sum(\"MontoDescuento\") AS \"MontoDescuento\"  " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_DETALLE_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"LineNum\", \"CodigoTipoLinea\", \"Detalle\", \"DetalleAdic\", \"TipoCodigo\" ORDER BY \"LineNum\"";

                oRecordset.DoQuery(strConsulta);

                //Se cargan el objeto para la información de Detalle que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    oRecordset.MoveFirst();
                    objDocumento.Detalle = new List<Detalle>();
                    for (int i = 0; i < oRecordset.RecordCount; i++)
                    {
                        objDetalle = new Detalle();
                        objDetalle.NroLinDet = oRecordset.Fields.Item("LineNum").Value.ToString();
                        objDetalle.CdgItem = new CdgItem();
                        objDetalle.CdgItem.TpoCodigo = oRecordset.Fields.Item("TipoCodigo").Value.ToString();
                        objDetalle.CdgItem.VlrCodigo = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        objDetalle.NmbItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        strDetAdic = oRecordset.Fields.Item("DetalleAdic").Value.ToString();
                        if (!string.IsNullOrEmpty(strDetAdic))
                        {
                            objDetalle.DscItem = strDetAdic;
                        }
                        //objDetalle.NmbItem = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        //objDetalle.DscItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        strControl = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        objDetalle.QtyItem = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        objDetalle.PrcItem = strControl.Replace(",", ".");

                        if (oRecordset.Fields.Item("PorcentDescuento").Value.ToString() != "0")
                        {
                            strControl = oRecordset.Fields.Item("MontoDescuento").Value.ToString();
                            objDetalle.DescuentoMonto = strControl.Replace(",", ".");
                            strControl = oRecordset.Fields.Item("PorcentDescuento").Value.ToString();
                            objDetalle.DescuentoPct = strControl.Replace(",", ".");
                        }

                        objDetalle.MontoItem = oRecordset.Fields.Item("MontoTotal").Value.ToString();
                        objDocumento.Detalle.Add(objDetalle);
                        oRecordset.MoveNext();
                    }
                }

                //Consulta para vista de referencias del documento
                strConsulta = "SELECT \"TipoDocNC\", \"NumeroNC\", \"FechaEmisionNC\", \"CodigoNC\", \"RazonNC\", \"DocEntry\", \"VisOrder\" " +
                    "FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEND_REFERENCIA_Query\"('PLACEHOLDER' = ('$$pObjType$$', '13'), 'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                if (oRecordset.RecordCount != 0)
                {
                    objDocumento.Referencia = new List<Referencia>();
                    //Se cargan el objeto de referencia para la información que se va a enviar al XML
                    while (!oRecordset.EoF)
                    {
                        Referencia objReferencia = new Referencia();
                        //Llenar los Totales
                        objReferencia.NroLinRef = (int.Parse(oRecordset.Fields.Item("VisOrder").Value.ToString()) + 1).ToString();
                        objReferencia.TpoDocRef = oRecordset.Fields.Item("TipoDocNC").Value.ToString();
                        objReferencia.FolioRef = oRecordset.Fields.Item("NumeroNC").Value.ToString();
                        FormatFecha = oRecordset.Fields.Item("FechaEmisionNC").Value.ToString();
                        //Se le da formato correcto a las fechas
                        FormatFecha = FormatearFecha(FormatFecha);
                        objReferencia.FchRef = FormatFecha;
                        objReferencia.CodRef = oRecordset.Fields.Item("CodigoNC").Value.ToString();
                        objReferencia.RazonRef = oRecordset.Fields.Item("RazonNC").Value.ToString();
                        //Llenar el encabezado
                        objDocumento.Referencia.Add(objReferencia);
                        oRecordset.MoveNext();
                    }
                }

                objDte.Documento = objDocumento;
                return objDte;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DTE ConsultaBoletaExenta(string strKey, string tipoDoc, out string p_comentarios, SAPbobsCOM.Company oCompany)
        {
            try//Se deben validar los campos para la Boleta
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string IndExent = string.Empty;
                string FormatFecha = string.Empty;
                string strConsulta = string.Empty;
                string strControl = string.Empty;
                string strDetAdic = string.Empty;
                string bdComp = ObtenerDatos(oCompany);
                objDte = new DTE();
                objDocumento = new SCGCE.BL.Objetos.Documento();
                objEncabezado = new Encabezado();
                objIdDoc = new IdDoc();
                objEmisor = new Emisor();
                objReceptor = new Receptor();
                objTotales = new Totales();
                p_comentarios = string.Empty;

                //Consulta para vista de encabezado del documento
                strConsulta = "SELECT \"Indicador\", sum(\"Folio\") AS \"Folio\", \"IndicadorServicio\",max(\"FechaDocumento\") AS \"FechaDocumento\",\"CondicionVenta\", " +
                    " \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", \"EmSennas\", \"EmComuna\", " +
                    " \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReCodigoInterno\",\"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", " +
                    " \"ReTelefono\", \"ReCodPaisFax\", \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\" , \"Comentarios\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_ENCABEZADO_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pTipoDoc$$','" + tipoDoc + "'),'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"Indicador\", \"IndicadorServicio\", \"CondicionVenta\", \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", " +
                    " \"EmSennas\", \"EmComuna\", \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReCodigoInterno\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", \"ReTelefono\", \"ReCodPaisFax\", " +
                    " \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\", \"Comentarios\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan los objetos requeridos para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar la parte de IdDoc
                    objIdDoc.TipoDTE = oRecordset.Fields.Item("Indicador").Value.ToString();
                    objIdDoc.Folio = oRecordset.Fields.Item("Folio").Value.ToString();
                    objIdDoc.IndServicio = oRecordset.Fields.Item("IndicadorServicio").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaDocumento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchEmis = FormatFecha;
                    //Llenar la parte de Emisor
                    objEmisor.RUTEmisor = oRecordset.Fields.Item("EmNumIdentificacion").Value.ToString();
                    objEmisor.RznSoc = oRecordset.Fields.Item("EmRazonSocial").Value.ToString();
                    objEmisor.GiroEmis = oRecordset.Fields.Item("EmGiroNegocio").Value.ToString();
                    objEmisor.DirOrigen = oRecordset.Fields.Item("EmSennas").Value.ToString();
                    objEmisor.CmnaOrigen = oRecordset.Fields.Item("EmComuna").Value.ToString();
                    objEmisor.CiudadOrigen = oRecordset.Fields.Item("EmCiudad").Value.ToString();
                    //Llenar la parte de Receptor
                    objReceptor.RUTRecep = oRecordset.Fields.Item("ReNumIdentificacion").Value.ToString();
                    objReceptor.CdgIntRecep = oRecordset.Fields.Item("ReCodigoInterno").Value.ToString();
                    objReceptor.RznSocRecep = oRecordset.Fields.Item("ReRazonSocial").Value.ToString();
                    objReceptor.Contacto = oRecordset.Fields.Item("ReEmail").Value.ToString();
                    objReceptor.DirRecep = oRecordset.Fields.Item("ReSennas").Value.ToString();
                    objReceptor.CmnaRecep = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objReceptor.CiudadRecep = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.IdDoc = objIdDoc;
                    objEncabezado.Emisor = objEmisor;
                    objEncabezado.Receptor = objReceptor;

                    //Llenar Informaci{on del WS
                    UrlWS = oRecordset.Fields.Item("URLDll").Value.ToString();
                    UsuarioWS = oRecordset.Fields.Item("UserDll").Value.ToString();
                    PassWS = oRecordset.Fields.Item("PassDll").Value.ToString();
                    p_comentarios = oRecordset.Fields.Item("Comentarios").Value.ToString();

                }

                //Consulta para vista de Totales del documento
                strConsulta = "SELECT sum(\"TotalVentaNeta\") AS \"TotalVentaNeta\", sum(\"TotalGravado\") AS \"TotalGravado\", sum(\"TotalExento\") AS \"TotalExento\", sum(\"PorcentajeImpueto\") AS \"PorcentajeImpueto\", " +
                    " sum(\"TotalImpuesto\") AS \"TotalImpuesto\", sum(\"TotalComprobante\") AS \"TotalComprobante\", sum(\"TipoCambio\") AS \"TipoCambio\", sum(\"PorcentajeDescuento\") AS \"PorcentajeDescuento\", sum(\"DescuentoEncabezado\") AS \"DescuentoEncabezado\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_TOTALCOMPROBANTE_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                objDocumento.DscRcgGlobal = new List<DscRcgGlobal>();
                int Descuento = 0;

                //Se cargan el objeto de totales para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar los Totales
                    objTotales.MntExe = oRecordset.Fields.Item("TotalExento").Value.ToString();
                    objTotales.MntTotal = oRecordset.Fields.Item("TotalVentaNeta").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.Totales = objTotales;
                    //Llenar descuento
                    Descuento = Convert.ToInt32(oRecordset.Fields.Item("PorcentajeDescuento").Value.ToString());
                    if (Descuento != 0)
                    {
                        objDscRcgGlobal = new DscRcgGlobal();
                        objDscRcgGlobal.NroLinDR = "1";
                        objDscRcgGlobal.TpoMov = "D";
                        objDscRcgGlobal.GlosaDR = "Descuento";
                        objDscRcgGlobal.TpoValor = "$";
                        objDscRcgGlobal.ValorDR = oRecordset.Fields.Item("DescuentoEncabezado").Value.ToString();
                        objDocumento.DscRcgGlobal.Add(objDscRcgGlobal);
                    }
                }
                //Llenar el Documento
                objDocumento.Encabezado = objEncabezado;

                //Consulta para vista de Detalle del documento
                strConsulta = "SELECT sum(\"LineNum\"+1) AS \"LineNum\", \"IndicadorExento\",\"CodigoTipoLinea\", 	\"Detalle\",\"DetalleAdic\",\"TipoCodigo\",	sum(\"Cantidad\") AS \"Cantidad\", " +
                    " sum(\"PrecioUnitario\") AS \"PrecioUnitario\", sum(\"MontoTotal\") AS \"MontoTotal\", sum(\"MontoTotalLinea\") AS \"MontoTotalLinea\", sum(\"PorcentDescuento\") AS \"PorcentDescuento\", sum(\"MontoDescuento\") AS \"MontoDescuento\"  " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_DETALLE_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"LineNum\", \"CodigoTipoLinea\", \"IndicadorExento\",\"Detalle\", \"DetalleAdic\", \"TipoCodigo\"  ORDER BY \"LineNum\"";

                oRecordset.DoQuery(strConsulta);

                //Se cargan el objeto para la información de Detalle que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    oRecordset.MoveFirst();
                    objDocumento.Detalle = new List<Detalle>();
                    for (int i = 0; i < oRecordset.RecordCount; i++)
                    {
                        objDetalle = new Detalle();
                        objDetalle.NroLinDet = oRecordset.Fields.Item("LineNum").Value.ToString();
                        IndExent = oRecordset.Fields.Item("IndicadorExento").Value.ToString();
                        if (!string.IsNullOrEmpty(IndExent))
                        {
                            objDetalle.IndExe = IndExent;
                        }
                        objDetalle.CdgItem = new CdgItem();
                        objDetalle.CdgItem.TpoCodigo = oRecordset.Fields.Item("TipoCodigo").Value.ToString();
                        objDetalle.CdgItem.VlrCodigo = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        objDetalle.NmbItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        strDetAdic = oRecordset.Fields.Item("DetalleAdic").Value.ToString();
                        if (!string.IsNullOrEmpty(strDetAdic))
                        {
                            objDetalle.DscItem = strDetAdic;
                        }
                        //objDetalle.NmbItem = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        //objDetalle.DscItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        strControl = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        objDetalle.QtyItem = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        objDetalle.PrcItem = strControl.Replace(",", ".");

                        if (oRecordset.Fields.Item("PorcentDescuento").Value.ToString() != "0")
                        {
                            strControl = oRecordset.Fields.Item("MontoDescuento").Value.ToString();
                            objDetalle.DescuentoMonto = strControl.Replace(",", ".");
                            strControl = oRecordset.Fields.Item("PorcentDescuento").Value.ToString();
                            objDetalle.DescuentoPct = strControl.Replace(",", ".");
                        }

                        objDetalle.MontoItem = oRecordset.Fields.Item("MontoTotal").Value.ToString();
                        objDocumento.Detalle.Add(objDetalle);
                        oRecordset.MoveNext();
                    }
                }

                //Consulta para vista de referencias del documento
                strConsulta = "SELECT \"TipoDocNC\", \"NumeroNC\", \"FechaEmisionNC\", \"CodigoNC\", \"RazonNC\", \"DocEntry\", \"VisOrder\" " +
                    "FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEND_REFERENCIA_Query\"('PLACEHOLDER' = ('$$pObjType$$', '13'), 'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                if (oRecordset.RecordCount != 0)
                {
                    objDocumento.Referencia = new List<Referencia>();
                    //Se cargan el objeto de referencia para la información que se va a enviar al XML
                    while (!oRecordset.EoF)
                    {
                        Referencia objReferencia = new Referencia();
                        //Llenar los Totales
                        objReferencia.NroLinRef = (int.Parse(oRecordset.Fields.Item("VisOrder").Value.ToString()) + 1).ToString();
                        objReferencia.TpoDocRef = oRecordset.Fields.Item("TipoDocNC").Value.ToString();
                        objReferencia.FolioRef = oRecordset.Fields.Item("NumeroNC").Value.ToString();
                        FormatFecha = oRecordset.Fields.Item("FechaEmisionNC").Value.ToString();
                        //Se le da formato correcto a las fechas
                        FormatFecha = FormatearFecha(FormatFecha);
                        objReferencia.FchRef = FormatFecha;
                        objReferencia.CodRef = oRecordset.Fields.Item("CodigoNC").Value.ToString();
                        objReferencia.RazonRef = oRecordset.Fields.Item("RazonNC").Value.ToString();
                        //Llenar el encabezado
                        objDocumento.Referencia.Add(objReferencia);
                        oRecordset.MoveNext();
                    }
                }

                objDte.Documento = objDocumento;
                return objDte;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DTE ConsultaNotaDebito(string strKey, string tipoDoc, out string p_comentarios, SAPbobsCOM.Company oCompany)
        {
            try//Se deben validar los campos para la Nota Debito
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRecordsetAct = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string FormatFecha = string.Empty;
                string strConsulta = string.Empty;
                string strControl = string.Empty;
                string strDetAdic = string.Empty;
                string bdComp = ObtenerDatos(oCompany);
                string IndExent = string.Empty;
                objDte = new DTE();
                objDocumento = new SCGCE.BL.Objetos.Documento();
                objEncabezado = new Encabezado();
                objIdDoc = new IdDoc();
                objEmisor = new Emisor();
                objReceptor = new Receptor();
                objTotales = new Totales();
                p_comentarios = string.Empty;

                //Consulta para vista de encabezado del documento
                strConsulta = "SELECT \"Indicador\",sum(\"Folio\") AS \"Folio\",max(\"FechaDocumento\") AS \"FechaDocumento\",\"CondicionVenta\", " +
                    " \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", \"EmSennas\", \"EmComuna\", " +
                    " \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", " +
                    " \"ReTelefono\", \"ReCodPaisFax\", \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\" , \"Comentarios\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_ENCABEZADO_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pTipoDoc$$','" + tipoDoc + "'),'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"Indicador\", \"CondicionVenta\", \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", " +
                    " \"EmSennas\", \"EmComuna\", \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", \"ReTelefono\", \"ReCodPaisFax\", " +
                    " \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\" , \"Comentarios\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan los objetos requeridos para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar la parte de IdDoc
                    objIdDoc.TipoDTE = oRecordset.Fields.Item("Indicador").Value.ToString();
                    objIdDoc.Folio = oRecordset.Fields.Item("Folio").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaDocumento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchEmis = FormatFecha;
                    FormatFecha = oRecordset.Fields.Item("FechaVencimiento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchVenc = FormatFecha;
                    //Llenar la parte de Emisor
                    objEmisor.RUTEmisor = oRecordset.Fields.Item("EmNumIdentificacion").Value.ToString();
                    objEmisor.RznSoc = oRecordset.Fields.Item("EmRazonSocial").Value.ToString();
                    objEmisor.GiroEmis = oRecordset.Fields.Item("EmGiroNegocio").Value.ToString();
                    // objEmisor.Acteco = oRecordset.Fields.Item("EmActeco").Value.ToString();


                    //Se agrega recorrido de Actecos 
                    strConsulta = @"Select ""Acteco"" FROM ""_SYS_BIC"".""sap." + bdComp + @"/CA_SCGCE_ACTECOS_Query""";
                    oRecordsetAct.DoQuery(strConsulta);
                    if (oRecordsetAct.RecordCount != 0)
                    {
                        objEmisor.Acteco = new List<string>();
                        for (int i = 0; i < oRecordsetAct.RecordCount; i++)
                        {

                            objEmisor.Acteco.Add(oRecordsetAct.Fields.Item("Acteco").Value.ToString());
                            oRecordsetAct.MoveNext();

                        }
                    }


                    objEmisor.DirOrigen = oRecordset.Fields.Item("EmSennas").Value.ToString();
                    objEmisor.CmnaOrigen = oRecordset.Fields.Item("EmComuna").Value.ToString();
                    objEmisor.CiudadOrigen = oRecordset.Fields.Item("EmCiudad").Value.ToString();
                    //Llenar la parte de Receptor
                    objReceptor.RUTRecep = oRecordset.Fields.Item("ReNumIdentificacion").Value.ToString();
                    objReceptor.RznSocRecep = oRecordset.Fields.Item("ReRazonSocial").Value.ToString();
                    objReceptor.GiroRecep = oRecordset.Fields.Item("ReGiroNegocio").Value.ToString();
                    objReceptor.DirRecep = oRecordset.Fields.Item("ReSennas").Value.ToString();
                    objReceptor.CmnaRecep = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objReceptor.CiudadRecep = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.IdDoc = objIdDoc;
                    objEncabezado.Emisor = objEmisor;
                    objEncabezado.Receptor = objReceptor;

                    //Llenar Informaci{on del WS
                    UrlWS = oRecordset.Fields.Item("URLDll").Value.ToString();
                    UsuarioWS = oRecordset.Fields.Item("UserDll").Value.ToString();
                    PassWS = oRecordset.Fields.Item("PassDll").Value.ToString();
                    p_comentarios = oRecordset.Fields.Item("Comentarios").Value.ToString();
                }

                //Consulta para vista de Totales del documento
                strConsulta = "SELECT sum(\"TotalVentaNeta\") AS \"TotalVentaNeta\", sum(\"TotalGravado\") AS \"TotalGravado\", sum(\"TotalExento\") AS \"TotalExento\", sum(\"PorcentajeImpueto\") AS \"PorcentajeImpueto\", " +
                    " sum(\"TotalImpuesto\") AS \"TotalImpuesto\", sum(\"TotalComprobante\") AS \"TotalComprobante\", sum(\"TipoCambio\") AS \"TipoCambio\", sum(\"PorcentajeDescuento\") AS \"PorcentajeDescuento\", sum(\"DescuentoEncabezado\") AS \"DescuentoEncabezado\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_TOTALCOMPROBANTE_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                objDocumento.DscRcgGlobal = new List<DscRcgGlobal>();
                int Descuento = 0;

                //Se cargan el objeto de totales para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar los Totales
                    objTotales.MntNeto = oRecordset.Fields.Item("TotalVentaNeta").Value.ToString();
                    objTotales.MntExe = oRecordset.Fields.Item("TotalExento").Value.ToString();
                    objTotales.TasaIVA = oRecordset.Fields.Item("PorcentajeImpueto").Value.ToString();
                    objTotales.IVA = oRecordset.Fields.Item("TotalImpuesto").Value.ToString();
                    objTotales.MntTotal = oRecordset.Fields.Item("TotalComprobante").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.Totales = objTotales;
                    //Llenar descuento
                    Descuento = Convert.ToInt32(oRecordset.Fields.Item("PorcentajeDescuento").Value.ToString());
                    if (Descuento != 0)
                    {
                        objDscRcgGlobal = new DscRcgGlobal();
                        objDscRcgGlobal.NroLinDR = "1";
                        objDscRcgGlobal.TpoMov = "D";
                        objDscRcgGlobal.GlosaDR = "Descuento";
                        objDscRcgGlobal.TpoValor = "$";
                        objDscRcgGlobal.ValorDR = oRecordset.Fields.Item("DescuentoEncabezado").Value.ToString();
                        objDocumento.DscRcgGlobal.Add(objDscRcgGlobal);
                    }
                }
                //Llenar el Documento
                objDocumento.Encabezado = objEncabezado;

                //Consulta para vista de Detalle del documento
                strConsulta = "SELECT sum(\"LineNum\"+1) AS \"LineNum\",\"IndicadorExento\", \"CodigoTipoLinea\", 	\"Detalle\", \"DetalleAdic\",\"TipoCodigo\",	sum(\"Cantidad\") AS \"Cantidad\", " +
                    " sum(\"PrecioUnitario\") AS \"PrecioUnitario\", sum(\"MontoTotal\") AS \"MontoTotal\", sum(\"MontoTotalLinea\") AS \"MontoTotalLinea\", sum(\"PorcentDescuento\") AS \"PorcentDescuento\", sum(\"MontoDescuento\") AS \"MontoDescuento\", \"UnidadMedida\" AS \"UnidadMedida\"  " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_DETALLE_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"LineNum\", \"CodigoTipoLinea\",\"IndicadorExento\", \"Detalle\", \"DetalleAdic\", \"TipoCodigo\",\"UnidadMedida\"  ORDER BY \"LineNum\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan el objeto para la información de Detalle que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    oRecordset.MoveFirst();
                    objDocumento.Detalle = new List<Detalle>();
                    for (int i = 0; i < oRecordset.RecordCount; i++)
                    {
                        objDetalle = new Detalle();
                        objDetalle.NroLinDet = oRecordset.Fields.Item("LineNum").Value.ToString();
                        IndExent = oRecordset.Fields.Item("IndicadorExento").Value.ToString();
                        if (!string.IsNullOrEmpty(IndExent))
                        {
                            objDetalle.IndExe = IndExent;
                        }
                        objDetalle.UnmdItem = oRecordset.Fields.Item("UnidadMedida").Value.ToString();
                        //objDetalle.IndExe = oRecordset.Fields.Item("IndicadorExento").Value.ToString();
                        objDetalle.CdgItem = new CdgItem();
                        objDetalle.CdgItem.TpoCodigo = oRecordset.Fields.Item("TipoCodigo").Value.ToString();
                        objDetalle.CdgItem.VlrCodigo = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        objDetalle.NmbItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        if (!string.IsNullOrEmpty(strDetAdic))
                        {
                            objDetalle.DscItem = strDetAdic;
                        }
                        //objDetalle.NmbItem = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        //objDetalle.DscItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        //objDetalle.QtyItem = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        //objDetalle.PrcItem = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        strControl = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        objDetalle.QtyItem = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        objDetalle.PrcItem = strControl.Replace(",", ".");

                        strControl = oRecordset.Fields.Item("MontoDescuento").Value.ToString();
                        objDetalle.DescuentoMonto = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PorcentDescuento").Value.ToString();
                        objDetalle.DescuentoPct = strControl.Replace(",", ".");

                        objDetalle.MontoItem = oRecordset.Fields.Item("MontoTotal").Value.ToString();
                        objDocumento.Detalle.Add(objDetalle);
                        oRecordset.MoveNext();
                    }
                }

                //Consulta para vista de referencias del documento
                strConsulta = "SELECT \"TipoDocNC\", \"NumeroNC\", \"FechaEmisionNC\", \"CodigoNC\", \"RazonNC\", \"DocEntry\", \"VisOrder\" " +
                    "FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEND_REFERENCIA_Query\"('PLACEHOLDER' = ('$$pObjType$$', '13'), 'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                if (oRecordset.RecordCount != 0)
                {
                    objDocumento.Referencia = new List<Referencia>();
                    //Se cargan el objeto de referencia para la información que se va a enviar al XML
                    while (!oRecordset.EoF)
                    {
                        Referencia objReferencia = new Referencia();
                        //Llenar los Totales
                        objReferencia.NroLinRef = (int.Parse(oRecordset.Fields.Item("VisOrder").Value.ToString()) + 1).ToString();
                        objReferencia.TpoDocRef = oRecordset.Fields.Item("TipoDocNC").Value.ToString();
                        objReferencia.FolioRef = oRecordset.Fields.Item("NumeroNC").Value.ToString();
                        FormatFecha = oRecordset.Fields.Item("FechaEmisionNC").Value.ToString();
                        //Se le da formato correcto a las fechas
                        FormatFecha = FormatearFecha(FormatFecha);
                        objReferencia.FchRef = FormatFecha;
                        objReferencia.CodRef = oRecordset.Fields.Item("CodigoNC").Value.ToString();
                        objReferencia.RazonRef = oRecordset.Fields.Item("RazonNC").Value.ToString();
                        //Llenar el encabezado
                        objDocumento.Referencia.Add(objReferencia);
                        oRecordset.MoveNext();
                    }
                }

                objDte.Documento = objDocumento;
                return objDte;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DTE ConsultaNotaDebitoExp(string strKey, string tipoDoc, out string p_comentarios, SAPbobsCOM.Company oCompany)
        {
            try//Se deben validar los campos para la Nota Debito
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRecordsetAct = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string FormatFecha = string.Empty;
                string strConsulta = string.Empty;
                string strControl = string.Empty;
                string strDetAdic = string.Empty;
                string bdComp = ObtenerDatos(oCompany);
                string IndExent = string.Empty;

                objDte = new DTE();
                objExportaciones = new SCGCE.BL.Objetos.Exportaciones();
                objEncabezado = new Encabezado();
                objIdDoc = new IdDoc();
                objEmisor = new Emisor();
                objReceptor = new Receptor();
                objTotales = new Totales();
                objTransporte = new Transporte();
                objAduana = new Aduana();
                objOtraMoneda = new OtraMoneda();
                p_comentarios = string.Empty;

                //Consulta para vista de encabezado del documento
                strConsulta = "SELECT \"Indicador\",sum(\"Folio\") AS \"Folio\",max(\"FechaDocumento\") AS \"FechaDocumento\",\"CondicionVenta\", " +
                    " \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", \"EmSennas\", \"EmComuna\", " +
                    " \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", " +
                    " \"ReTelefono\", \"ReCodPaisFax\", \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\" , \"Comentarios\", \"ReCodigoInterno\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_ENCABEZADO_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pTipoDoc$$','" + tipoDoc + "'),'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"Indicador\", \"CondicionVenta\", \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", " +
                    " \"EmSennas\", \"EmComuna\", \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", \"ReTelefono\", \"ReCodPaisFax\", " +
                    " \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\" , \"Comentarios\", \"ReCodigoInterno\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan los objetos requeridos para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar la parte de IdDoc
                    objIdDoc.TipoDTE = "1" + oRecordset.Fields.Item("Indicador").Value.ToString();
                    objIdDoc.Folio = oRecordset.Fields.Item("Folio").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaDocumento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchEmis = FormatFecha;
                    FormatFecha = oRecordset.Fields.Item("FechaVencimiento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchVenc = FormatFecha;
                    //Llenar la parte de Emisor
                    objEmisor.RUTEmisor = oRecordset.Fields.Item("EmNumIdentificacion").Value.ToString();
                    objEmisor.RznSoc = oRecordset.Fields.Item("EmRazonSocial").Value.ToString();
                    objEmisor.GiroEmis = oRecordset.Fields.Item("EmGiroNegocio").Value.ToString();
                    // objEmisor.Acteco = oRecordset.Fields.Item("EmActeco").Value.ToString();


                    //Se agrega recorrido de Actecos 
                    strConsulta = @"Select ""Acteco"" FROM ""_SYS_BIC"".""sap." + bdComp + @"/CA_SCGCE_ACTECOS_Query""";
                    oRecordsetAct.DoQuery(strConsulta);
                    if (oRecordsetAct.RecordCount != 0)
                    {
                        objEmisor.Acteco = new List<string>();
                        for (int i = 0; i < oRecordsetAct.RecordCount; i++)
                        {

                            objEmisor.Acteco.Add(oRecordsetAct.Fields.Item("Acteco").Value.ToString());
                            oRecordsetAct.MoveNext();

                        }
                    }


                    objEmisor.DirOrigen = oRecordset.Fields.Item("EmSennas").Value.ToString();
                    objEmisor.CmnaOrigen = oRecordset.Fields.Item("EmComuna").Value.ToString();
                    objEmisor.CiudadOrigen = oRecordset.Fields.Item("EmCiudad").Value.ToString();
                    //Llenar la parte de Receptor
                    objReceptor.RUTRecep = "55555555-5";
                    objReceptor.CdgIntRecep = oRecordset.Fields.Item("ReCodigoInterno").Value.ToString();
                    objReceptor.RznSocRecep = oRecordset.Fields.Item("ReRazonSocial").Value.ToString();
                    objReceptor.GiroRecep = oRecordset.Fields.Item("ReGiroNegocio").Value.ToString();
                    objReceptor.DirRecep = oRecordset.Fields.Item("ReSennas").Value.ToString();
                    objReceptor.CmnaRecep = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objReceptor.CiudadRecep = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.IdDoc = objIdDoc;
                    objEncabezado.Emisor = objEmisor;
                    objEncabezado.Receptor = objReceptor;

                    //Llenar Informaci{on del WS
                    UrlWS = oRecordset.Fields.Item("URLDll").Value.ToString();
                    UsuarioWS = oRecordset.Fields.Item("UserDll").Value.ToString();
                    PassWS = oRecordset.Fields.Item("PassDll").Value.ToString();
                    p_comentarios = oRecordset.Fields.Item("Comentarios").Value.ToString();
                }

                //Consulta para vista de Totales del documento
                strConsulta = "SELECT sum(\"TotalVentaNeta\") AS \"TotalVentaNeta\", sum(\"TotalGravado\") AS \"TotalGravado\", sum(\"TotalExento\") AS \"TotalExento\", sum(\"PorcentajeImpueto\") AS \"PorcentajeImpueto\", " +
                    " sum(\"TotalImpuesto\") AS \"TotalImpuesto\", sum(\"TotalComprobante\") AS \"TotalComprobante\", sum(\"TipoCambio\") AS \"TipoCambio\", sum(\"PorcentajeDescuento\") AS \"PorcentajeDescuento\", sum(\"DescuentoEncabezado\") AS \"DescuentoEncabezado\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_TOTALCOMPROBANTE_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                objExportaciones.DscRcgGlobal = new List<DscRcgGlobal>();
                int Descuento = 0;

                //Se cargan el objeto de totales para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar los Totales
                    //objTotales.MntNeto = oRecordset.Fields.Item("TotalVentaNeta").Value.ToString();
                    objTotales.MntExe = oRecordset.Fields.Item("TotalExento").Value.ToString();
                    //objTotales.TasaIVA = oRecordset.Fields.Item("PorcentajeImpueto").Value.ToString();
                    //objTotales.IVA = oRecordset.Fields.Item("TotalImpuesto").Value.ToString();
                    objTotales.MntTotal = oRecordset.Fields.Item("TotalComprobante").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.Totales = objTotales;
                    //Llenar descuento
                    Descuento = Convert.ToInt32(oRecordset.Fields.Item("PorcentajeDescuento").Value.ToString());
                    if (Descuento != 0)
                    {
                        objDscRcgGlobal = new DscRcgGlobal();
                        objDscRcgGlobal.NroLinDR = "1";
                        objDscRcgGlobal.TpoMov = "D";
                        objDscRcgGlobal.GlosaDR = "Descuento";
                        objDscRcgGlobal.TpoValor = "$";
                        objDscRcgGlobal.ValorDR = oRecordset.Fields.Item("DescuentoEncabezado").Value.ToString();
                        objExportaciones.DscRcgGlobal.Add(objDscRcgGlobal);
                    }
                }
                //Llenar el Documento
                objExportaciones.Encabezado = objEncabezado;

                //Consulta para vista de Detalle del documento
                strConsulta = "SELECT sum(\"LineNum\"+1) AS \"LineNum\",\"IndicadorExento\", \"CodigoTipoLinea\", 	\"Detalle\", \"DetalleAdic\",\"TipoCodigo\",	sum(\"Cantidad\") AS \"Cantidad\", " +
                    " sum(\"PrecioUnitario\") AS \"PrecioUnitario\", sum(\"MontoTotal\") AS \"MontoTotal\", sum(\"MontoTotalLinea\") AS \"MontoTotalLinea\", sum(\"PorcentDescuento\") AS \"PorcentDescuento\", sum(\"MontoDescuento\") AS \"MontoDescuento\"  " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_DETALLE_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"LineNum\", \"CodigoTipoLinea\",\"IndicadorExento\", \"Detalle\", \"DetalleAdic\", \"TipoCodigo\"  ORDER BY \"LineNum\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan el objeto para la información de Detalle que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    oRecordset.MoveFirst();
                    objExportaciones.Detalle = new List<Detalle>();
                    for (int i = 0; i < oRecordset.RecordCount; i++)
                    {
                        objDetalle = new Detalle();
                        objDetalle.NroLinDet = oRecordset.Fields.Item("LineNum").Value.ToString();
                        IndExent = oRecordset.Fields.Item("IndicadorExento").Value.ToString();
                        if (!string.IsNullOrEmpty(IndExent))
                        {
                            objDetalle.IndExe = IndExent;
                        }
                        //objDetalle.IndExe = oRecordset.Fields.Item("IndicadorExento").Value.ToString();
                        objDetalle.CdgItem = new CdgItem();
                        objDetalle.CdgItem.TpoCodigo = oRecordset.Fields.Item("TipoCodigo").Value.ToString();
                        objDetalle.CdgItem.VlrCodigo = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        objDetalle.NmbItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        if (!string.IsNullOrEmpty(strDetAdic))
                        {
                            objDetalle.DscItem = strDetAdic;
                        }
                        //objDetalle.NmbItem = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        //objDetalle.DscItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        //objDetalle.QtyItem = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        //objDetalle.PrcItem = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        strControl = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        objDetalle.QtyItem = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        objDetalle.PrcItem = strControl.Replace(",", ".");

                        if (oRecordset.Fields.Item("PorcentDescuento").Value.ToString() != "0")
                        {
                            strControl = oRecordset.Fields.Item("MontoDescuento").Value.ToString();
                            objDetalle.DescuentoMonto = strControl.Replace(",", ".");
                            strControl = oRecordset.Fields.Item("PorcentDescuento").Value.ToString();
                            objDetalle.DescuentoPct = strControl.Replace(",", ".");

                        }


                        objDetalle.MontoItem = oRecordset.Fields.Item("MontoTotal").Value.ToString();
                        objExportaciones.Detalle.Add(objDetalle);
                        oRecordset.MoveNext();
                    }
                }

                //Consulta para vista de referencias del documento
                strConsulta = "SELECT \"TipoDocNC\", \"NumeroNC\", \"FechaEmisionNC\", \"CodigoNC\", \"RazonNC\", \"DocEntry\", \"VisOrder\" " +
                    "FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEND_REFERENCIA_Query\"('PLACEHOLDER' = ('$$pObjType$$', '13'), 'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                if (oRecordset.RecordCount != 0)
                {
                    objDocumento.Referencia = new List<Referencia>();
                    //Se cargan el objeto de referencia para la información que se va a enviar al XML
                    while (!oRecordset.EoF)
                    {
                        Referencia objReferencia = new Referencia();
                        //Llenar los Totales
                        objReferencia.NroLinRef = (int.Parse(oRecordset.Fields.Item("VisOrder").Value.ToString()) + 1).ToString();
                        objReferencia.TpoDocRef = oRecordset.Fields.Item("TipoDocNC").Value.ToString();
                        objReferencia.FolioRef = oRecordset.Fields.Item("NumeroNC").Value.ToString();
                        FormatFecha = oRecordset.Fields.Item("FechaEmisionNC").Value.ToString();
                        //Se le da formato correcto a las fechas
                        FormatFecha = FormatearFecha(FormatFecha);
                        objReferencia.FchRef = FormatFecha;
                        objReferencia.CodRef = oRecordset.Fields.Item("CodigoNC").Value.ToString();
                        objReferencia.RazonRef = oRecordset.Fields.Item("RazonNC").Value.ToString();
                        //Llenar el encabezado
                        objDocumento.Referencia.Add(objReferencia);
                        oRecordset.MoveNext();
                    }
                }

                //Consulta para vista de Exportación
                strConsulta = "SELECT \"U_SCGCE_TipoDesp\",\"U_SCGCE_CondVentExp\",\"U_SCGCE_ModVenta\",\"U_SCGCE_PaisDestino\",\"U_SCGCE_TpoMoneda\", " +
                               " \"TipoMoneda\",sum(\"U_SCGCE_TotBultos\") AS \"U_SCGCE_TotBultos\",sum(\"DocRate\") AS \"DocRate\",sum(\"MntTotOtraMond\") AS \"MntTotOtraMond\" " +
                               "FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFA_ENCABEZADOEXP_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                                "GROUP BY \"U_SCGCE_TipoDesp\",\"U_SCGCE_CondVentExp\",\"U_SCGCE_ModVenta\",\"U_SCGCE_PaisDestino\",\"U_SCGCE_TpoMoneda\",\"TipoMoneda\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan el objeto de Exportación ara la factura
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar los Totales
                    //objEncabezado.IdDoc.TipoDespacho = oRecordset.Fields.Item("U_SCGCE_TipoDesp").Value.ToString();
                    //objEncabezado.IdDoc.FmaPagExp = oRecordset.Fields.Item("U_SCGCE_CondVentExp").Value.ToString();
                    //objAduana.CodModVenta = oRecordset.Fields.Item("U_SCGCE_ModVenta").Value.ToString();
                    //objAduana.TotBultos = oRecordset.Fields.Item("U_SCGCE_TotBultos").Value.ToString();
                    //objAduana.CodPaisDestin = oRecordset.Fields.Item("U_SCGCE_PaisDestino").Value.ToString();
                    objTotales.TpoMoneda = oRecordset.Fields.Item("U_SCGCE_TpoMoneda").Value.ToString();
                    objOtraMoneda.TpoMoneda = oRecordset.Fields.Item("TipoMoneda").Value.ToString();
                    objOtraMoneda.TpoCambio = oRecordset.Fields.Item("DocRate").Value.ToString();
                    objOtraMoneda.MntExeOtrMnda = oRecordset.Fields.Item("MntTotOtraMond").Value.ToString();
                    objOtraMoneda.MntTotOtrMnda = oRecordset.Fields.Item("MntTotOtraMond").Value.ToString();
                    //objTransporte.Aduana = objAduana;
                    objEncabezado.Transporte = objTransporte;
                    objEncabezado.OtraMoneda = objOtraMoneda;
                }

                objDte.Exportaciones = objExportaciones;
                return objDte;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DTE ConsultaNotaCredito(string strKey, string tipoDoc, out string p_comentarios, SAPbobsCOM.Company oCompany)
        {
            try//Se deben validar los campos para la Nota Debito
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRecordsetAct = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string FormatFecha = string.Empty;
                string strConsulta = string.Empty;
                string strControl = string.Empty;
                string strDetAdic = string.Empty;
                string IndExent = string.Empty;
                string bdComp = ObtenerDatos(oCompany);
                objDte = new DTE();
                objDocumento = new SCGCE.BL.Objetos.Documento();
                objEncabezado = new Encabezado();
                objIdDoc = new IdDoc();
                objEmisor = new Emisor();
                objReceptor = new Receptor();
                objTotales = new Totales();
                p_comentarios = string.Empty;

                //Consulta para vista de encabezado del documento
                strConsulta = "SELECT \"Indicador\",sum(\"Folio\") AS \"Folio\",max(\"FechaDocumento\") AS \"FechaDocumento\",\"CondicionVenta\", " +
                    " \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", \"EmSennas\", \"EmComuna\", " +
                    " \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", " +
                    " \"ReTelefono\", \"ReCodPaisFax\", \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\" , \"Comentarios\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCENC_ENCABEZADO_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pTipoDoc$$','" + tipoDoc + "'),'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"Indicador\", \"CondicionVenta\", \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", " +
                    " \"EmSennas\", \"EmComuna\", \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", \"ReTelefono\", \"ReCodPaisFax\", " +
                    " \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\" , \"Comentarios\"";

                oRecordset.DoQuery(strConsulta);

                //Se cargan los objetos requeridos para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar la parte de IdDoc
                    objIdDoc.TipoDTE = oRecordset.Fields.Item("Indicador").Value.ToString();
                    objIdDoc.Folio = oRecordset.Fields.Item("Folio").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaDocumento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchEmis = FormatFecha;
                    FormatFecha = oRecordset.Fields.Item("FechaVencimiento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchVenc = FormatFecha;
                    //Llenar la parte de Emisor
                    objEmisor.RUTEmisor = oRecordset.Fields.Item("EmNumIdentificacion").Value.ToString();
                    objEmisor.RznSoc = oRecordset.Fields.Item("EmRazonSocial").Value.ToString();
                    objEmisor.GiroEmis = oRecordset.Fields.Item("EmGiroNegocio").Value.ToString();
                    //  objEmisor.Acteco = oRecordset.Fields.Item("EmActeco").Value.ToString();

                    //Se agrega recorrido de Actecos 
                    strConsulta = @"Select ""Acteco"" FROM ""_SYS_BIC"".""sap." + bdComp + @"/CA_SCGCE_ACTECOS_Query""";
                    oRecordsetAct.DoQuery(strConsulta);
                    if (oRecordsetAct.RecordCount != 0)
                    {
                        objEmisor.Acteco = new List<string>();
                        for (int i = 0; i < oRecordsetAct.RecordCount; i++)
                        {

                            objEmisor.Acteco.Add(oRecordsetAct.Fields.Item("Acteco").Value.ToString());
                            oRecordsetAct.MoveNext();

                        }
                    }

                    objEmisor.DirOrigen = oRecordset.Fields.Item("EmSennas").Value.ToString();
                    objEmisor.CmnaOrigen = oRecordset.Fields.Item("EmComuna").Value.ToString();
                    objEmisor.CiudadOrigen = oRecordset.Fields.Item("EmCiudad").Value.ToString();
                    //Llenar la parte de Receptor
                    objReceptor.RUTRecep = oRecordset.Fields.Item("ReNumIdentificacion").Value.ToString();
                    objReceptor.RznSocRecep = oRecordset.Fields.Item("ReRazonSocial").Value.ToString();
                    objReceptor.GiroRecep = oRecordset.Fields.Item("ReGiroNegocio").Value.ToString();
                    objReceptor.DirRecep = oRecordset.Fields.Item("ReSennas").Value.ToString();
                    objReceptor.CmnaRecep = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objReceptor.CiudadRecep = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.IdDoc = objIdDoc;
                    objEncabezado.Emisor = objEmisor;
                    objEncabezado.Receptor = objReceptor;

                    //Llenar Informaci{on del WS
                    UrlWS = oRecordset.Fields.Item("URLDll").Value.ToString();
                    UsuarioWS = oRecordset.Fields.Item("UserDll").Value.ToString();
                    PassWS = oRecordset.Fields.Item("PassDll").Value.ToString();
                    p_comentarios = oRecordset.Fields.Item("Comentarios").Value.ToString();
                }

                //Consulta para vista de Totales del documento
                strConsulta = "SELECT sum(\"TotalVentaNeta\") AS \"TotalVentaNeta\", sum(\"TotalGravado\") AS \"TotalGravado\", " +
                    " sum(\"TotalExento\") AS \"TotalExento\", sum(\"PorcentajeImpueto\") AS \"PorcentajeImpueto\", " +
                    " sum(\"TotalImpuesto\") AS \"TotalImpuesto\", sum(\"TotalComprobante\") AS \"TotalComprobante\", " +
                    " sum(\"TipoCambio\") AS \"TipoCambio\", sum(\"PorcentajeDescuento\") AS \"PorcentajeDescuento\", sum(\"DescuentoEncabezado\") AS \"DescuentoEncabezado\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCENC_TOTALCOMPROBANTE_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                objDocumento.DscRcgGlobal = new List<DscRcgGlobal>();
                int Descuento = 0;

                //Se cargan el objeto de totales para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar los Totales
                    objTotales.MntNeto = oRecordset.Fields.Item("TotalVentaNeta").Value.ToString();
                    objTotales.MntExe = oRecordset.Fields.Item("TotalExento").Value.ToString();
                    objTotales.TasaIVA = oRecordset.Fields.Item("PorcentajeImpueto").Value.ToString();
                    objTotales.IVA = oRecordset.Fields.Item("TotalImpuesto").Value.ToString();
                    objTotales.MntTotal = oRecordset.Fields.Item("TotalComprobante").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.Totales = objTotales;
                    //Llenar descuento
                    Descuento = Convert.ToInt32(oRecordset.Fields.Item("PorcentajeDescuento").Value.ToString());
                    if (Descuento != 0)
                    {
                        objDscRcgGlobal = new DscRcgGlobal();
                        objDscRcgGlobal.NroLinDR = "1";
                        objDscRcgGlobal.TpoMov = "D";
                        objDscRcgGlobal.GlosaDR = "Descuento";
                        objDscRcgGlobal.TpoValor = "$";
                        objDscRcgGlobal.ValorDR = oRecordset.Fields.Item("DescuentoEncabezado").Value.ToString();
                        objDocumento.DscRcgGlobal.Add(objDscRcgGlobal);
                    }
                }
                //Llenar el Documento
                objDocumento.Encabezado = objEncabezado;

                //Consulta para vista de Detalle del documento
                strConsulta = "SELECT sum(\"LineNum\"+1) AS \"LineNum\", \"IndicadorExento\",	\"CodigoTipoLinea\", 	\"Detalle\", \"DetalleAdic\",\"TipoCodigo\",	sum(\"Cantidad\") AS \"Cantidad\", " +
                    " sum(\"PrecioUnitario\") AS \"PrecioUnitario\", sum(\"MontoTotal\") AS \"MontoTotal\", sum(\"MontoTotalLinea\") AS \"MontoTotalLinea\", sum(\"PorcentDescuento\") AS \"PorcentDescuento\", sum(\"MontoDescuento\") AS \"MontoDescuento\", \"UnidadMedida\" AS \"UnidadMedida\"  " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCENC_DETALLE_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"LineNum\", \"CodigoTipoLinea\",\"IndicadorExento\", \"Detalle\", \"DetalleAdic\", \"TipoCodigo\",\"UnidadMedida\"  ORDER BY \"LineNum\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan el objeto para la información de Detalle que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    oRecordset.MoveFirst();
                    objDocumento.Detalle = new List<Detalle>();
                    for (int i = 0; i < oRecordset.RecordCount; i++)
                    {
                        objDetalle = new Detalle();
                        objDetalle.NroLinDet = oRecordset.Fields.Item("LineNum").Value.ToString();
                        IndExent = oRecordset.Fields.Item("IndicadorExento").Value.ToString();
                        if (!string.IsNullOrEmpty(IndExent))
                        {
                            objDetalle.IndExe = IndExent;
                        }
                        //objDetalle.IndExe = oRecordset.Fields.Item("IndicadorExento").Value.ToString();
                        objDetalle.CdgItem = new CdgItem();
                        objDetalle.CdgItem.TpoCodigo = oRecordset.Fields.Item("TipoCodigo").Value.ToString();
                        objDetalle.CdgItem.VlrCodigo = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        objDetalle.NmbItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        strDetAdic = oRecordset.Fields.Item("DetalleAdic").Value.ToString();
                        if (!string.IsNullOrEmpty(strDetAdic))
                        {
                            objDetalle.DscItem = strDetAdic;
                        }
                        objDetalle.UnmdItem = oRecordset.Fields.Item("UnidadMedida").Value.ToString();
                        //objDetalle.NmbItem = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        //objDetalle.DscItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        //objDetalle.QtyItem = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        //objDetalle.PrcItem = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        strControl = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        objDetalle.QtyItem = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        objDetalle.PrcItem = strControl.Replace(",", ".");

                        strControl = oRecordset.Fields.Item("MontoDescuento").Value.ToString();
                        objDetalle.DescuentoMonto = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PorcentDescuento").Value.ToString();
                        objDetalle.DescuentoPct = strControl.Replace(",", ".");

                        objDetalle.MontoItem = oRecordset.Fields.Item("MontoTotal").Value.ToString();
                        objDocumento.Detalle.Add(objDetalle);
                        oRecordset.MoveNext();
                    }
                }

                //Consulta para vista de referencias del documento
                strConsulta = "SELECT \"TipoDocNC\", \"NumeroNC\", \"FechaEmisionNC\", \"CodigoNC\", \"RazonNC\", \"DocEntry\", \"VisOrder\" " +
                    "FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEND_REFERENCIA_Query\"('PLACEHOLDER' = ('$$pObjType$$', '14'), 'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                if (oRecordset.RecordCount != 0)
                {
                    objDocumento.Referencia = new List<Referencia>();
                    //Se cargan el objeto de referencia para la información que se va a enviar al XML
                    while (!oRecordset.EoF)
                    {
                        Referencia objReferencia = new Referencia();
                        //Llenar los Totales
                        objReferencia.NroLinRef = (int.Parse(oRecordset.Fields.Item("VisOrder").Value.ToString()) + 1).ToString();
                        objReferencia.TpoDocRef = oRecordset.Fields.Item("TipoDocNC").Value.ToString();
                        objReferencia.FolioRef = oRecordset.Fields.Item("NumeroNC").Value.ToString();
                        FormatFecha = oRecordset.Fields.Item("FechaEmisionNC").Value.ToString();
                        //Se le da formato correcto a las fechas
                        FormatFecha = FormatearFecha(FormatFecha);
                        objReferencia.FchRef = FormatFecha;
                        objReferencia.CodRef = oRecordset.Fields.Item("CodigoNC").Value.ToString();
                        objReferencia.RazonRef = oRecordset.Fields.Item("RazonNC").Value.ToString();
                        //Llenar el encabezado
                        objDocumento.Referencia.Add(objReferencia);
                        oRecordset.MoveNext();
                    }
                }

                objDte.Documento = objDocumento;
                return objDte;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DTE ConsultaNotaCreditoExp(string strKey, string tipoDoc, out string p_comentarios, SAPbobsCOM.Company oCompany)
        {
            try//Se deben validar los campos para la Nota Debito
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRecordsetAct = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string FormatFecha = string.Empty;
                string strConsulta = string.Empty;
                string strControl = string.Empty;
                string strDetAdic = string.Empty;
                string IndExent = string.Empty;
                string bdComp = ObtenerDatos(oCompany);
                objDte = new DTE();
                objExportaciones = new SCGCE.BL.Objetos.Exportaciones();
                //objDocumento = new SCGCE.BL.Objetos.Documento();
                objEncabezado = new Encabezado();
                objIdDoc = new IdDoc();
                objEmisor = new Emisor();
                objReceptor = new Receptor();
                objTotales = new Totales();
                objTransporte = new Transporte();
                objAduana = new Aduana();
                objOtraMoneda = new OtraMoneda();
                p_comentarios = string.Empty;

                //Consulta para vista de encabezado del documento
                strConsulta = "SELECT \"Indicador\",sum(\"Folio\") AS \"Folio\",max(\"FechaDocumento\") AS \"FechaDocumento\",\"CondicionVenta\", " +
                    " \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", \"EmSennas\", \"EmComuna\", " +
                    " \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", " +
                    " \"ReTelefono\", \"ReCodPaisFax\", \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\" , \"Comentarios\", \"ReCodigoInterno\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCENC_ENCABEZADO_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pTipoDoc$$','" + tipoDoc + "'),'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"Indicador\", \"CondicionVenta\", \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", " +
                    " \"EmSennas\", \"EmComuna\", \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", \"ReTelefono\", \"ReCodPaisFax\", " +
                    " \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\" , \"Comentarios\", \"ReCodigoInterno\"";

                oRecordset.DoQuery(strConsulta);

                //Se cargan los objetos requeridos para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar la parte de IdDoc
                    objIdDoc.TipoDTE = "1" + oRecordset.Fields.Item("Indicador").Value.ToString();
                    objIdDoc.Folio = oRecordset.Fields.Item("Folio").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaDocumento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchEmis = FormatFecha;
                    FormatFecha = oRecordset.Fields.Item("FechaVencimiento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchVenc = FormatFecha;
                    //Llenar la parte de Emisor
                    objEmisor.RUTEmisor = oRecordset.Fields.Item("EmNumIdentificacion").Value.ToString();
                    objEmisor.RznSoc = oRecordset.Fields.Item("EmRazonSocial").Value.ToString();
                    objEmisor.GiroEmis = oRecordset.Fields.Item("EmGiroNegocio").Value.ToString();
                    //  objEmisor.Acteco = oRecordset.Fields.Item("EmActeco").Value.ToString();

                    //Se agrega recorrido de Actecos 
                    strConsulta = @"Select ""Acteco"" FROM ""_SYS_BIC"".""sap." + bdComp + @"/CA_SCGCE_ACTECOS_Query""";
                    oRecordsetAct.DoQuery(strConsulta);
                    if (oRecordsetAct.RecordCount != 0)
                    {
                        objEmisor.Acteco = new List<string>();
                        for (int i = 0; i < oRecordsetAct.RecordCount; i++)
                        {

                            objEmisor.Acteco.Add(oRecordsetAct.Fields.Item("Acteco").Value.ToString());
                            oRecordsetAct.MoveNext();

                        }
                    }

                    objEmisor.DirOrigen = oRecordset.Fields.Item("EmSennas").Value.ToString();
                    objEmisor.CmnaOrigen = oRecordset.Fields.Item("EmComuna").Value.ToString();
                    objEmisor.CiudadOrigen = oRecordset.Fields.Item("EmCiudad").Value.ToString();
                    //Llenar la parte de Receptor
                    objReceptor.RUTRecep = "55555555-5";
                    objReceptor.CdgIntRecep = oRecordset.Fields.Item("ReCodigoInterno").Value.ToString();
                    objReceptor.RznSocRecep = oRecordset.Fields.Item("ReRazonSocial").Value.ToString();
                    objReceptor.GiroRecep = oRecordset.Fields.Item("ReGiroNegocio").Value.ToString();
                    objReceptor.DirRecep = oRecordset.Fields.Item("ReSennas").Value.ToString();
                    objReceptor.CmnaRecep = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objReceptor.CiudadRecep = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.IdDoc = objIdDoc;
                    objEncabezado.Emisor = objEmisor;
                    objEncabezado.Receptor = objReceptor;

                    //Llenar Informaci{on del WS
                    UrlWS = oRecordset.Fields.Item("URLDll").Value.ToString();
                    UsuarioWS = oRecordset.Fields.Item("UserDll").Value.ToString();
                    PassWS = oRecordset.Fields.Item("PassDll").Value.ToString();
                    p_comentarios = oRecordset.Fields.Item("Comentarios").Value.ToString();
                }

                //Consulta para vista de Totales del documento
                strConsulta = "SELECT sum(\"TotalVentaNeta\") AS \"TotalVentaNeta\", sum(\"TotalGravado\") AS \"TotalGravado\", " +
                    " sum(\"TotalExento\") AS \"TotalExento\", sum(\"PorcentajeImpueto\") AS \"PorcentajeImpueto\", " +
                    " sum(\"TotalImpuesto\") AS \"TotalImpuesto\", sum(\"TotalComprobante\") AS \"TotalComprobante\", " +
                    " sum(\"TipoCambio\") AS \"TipoCambio\", sum(\"PorcentajeDescuento\") AS \"PorcentajeDescuento\", sum(\"DescuentoEncabezado\") AS \"DescuentoEncabezado\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCENC_TOTALCOMPROBANTE_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                objExportaciones.DscRcgGlobal = new List<DscRcgGlobal>();
                int Descuento = 0;

                //Se cargan el objeto de totales para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar los Totales
                    //objTotales.MntNeto = oRecordset.Fields.Item("TotalVentaNeta").Value.ToString();
                    objTotales.MntExe = oRecordset.Fields.Item("TotalExento").Value.ToString();
                    //objTotales.TasaIVA = oRecordset.Fields.Item("PorcentajeImpueto").Value.ToString();
                    //objTotales.IVA = oRecordset.Fields.Item("TotalImpuesto").Value.ToString();
                    objTotales.MntTotal = oRecordset.Fields.Item("TotalComprobante").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.Totales = objTotales;
                    //Llenar descuento
                    Descuento = Convert.ToInt32(oRecordset.Fields.Item("PorcentajeDescuento").Value.ToString());
                    if (Descuento != 0)
                    {
                        objDscRcgGlobal = new DscRcgGlobal();
                        objDscRcgGlobal.NroLinDR = "1";
                        objDscRcgGlobal.TpoMov = "D";
                        objDscRcgGlobal.GlosaDR = "Descuento";
                        objDscRcgGlobal.TpoValor = "$";
                        objDscRcgGlobal.ValorDR = oRecordset.Fields.Item("DescuentoEncabezado").Value.ToString();
                        objExportaciones.DscRcgGlobal.Add(objDscRcgGlobal);
                    }
                }
                //Llenar el Documento
                objExportaciones.Encabezado = objEncabezado;

                //Consulta para vista de Detalle del documento
                strConsulta = "SELECT sum(\"LineNum\"+1) AS \"LineNum\", \"IndicadorExento\",	\"CodigoTipoLinea\", 	\"Detalle\", \"DetalleAdic\",\"TipoCodigo\",	sum(\"Cantidad\") AS \"Cantidad\", " +
                    " sum(\"PrecioUnitario\") AS \"PrecioUnitario\", sum(\"MontoTotal\") AS \"MontoTotal\", sum(\"MontoTotalLinea\") AS \"MontoTotalLinea\", sum(\"PorcentDescuento\") AS \"PorcentDescuento\", sum(\"MontoDescuento\") AS \"MontoDescuento\"  " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCENC_DETALLE_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"LineNum\", \"CodigoTipoLinea\",\"IndicadorExento\", \"Detalle\", \"DetalleAdic\", \"TipoCodigo\"  ORDER BY \"LineNum\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan el objeto para la información de Detalle que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    oRecordset.MoveFirst();
                    objExportaciones.Detalle = new List<Detalle>();
                    for (int i = 0; i < oRecordset.RecordCount; i++)
                    {
                        objDetalle = new Detalle();
                        objDetalle.NroLinDet = oRecordset.Fields.Item("LineNum").Value.ToString();
                        IndExent = oRecordset.Fields.Item("IndicadorExento").Value.ToString();
                        if (!string.IsNullOrEmpty(IndExent))
                        {
                            objDetalle.IndExe = IndExent;
                        }
                        //objDetalle.IndExe = oRecordset.Fields.Item("IndicadorExento").Value.ToString();
                        objDetalle.CdgItem = new CdgItem();
                        objDetalle.CdgItem.TpoCodigo = oRecordset.Fields.Item("TipoCodigo").Value.ToString();
                        objDetalle.CdgItem.VlrCodigo = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        objDetalle.NmbItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        strDetAdic = oRecordset.Fields.Item("DetalleAdic").Value.ToString();
                        if (!string.IsNullOrEmpty(strDetAdic))
                        {
                            objDetalle.DscItem = strDetAdic;
                        }
                        //objDetalle.NmbItem = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        //objDetalle.DscItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        //objDetalle.QtyItem = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        //objDetalle.PrcItem = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        strControl = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        objDetalle.QtyItem = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        objDetalle.PrcItem = strControl.Replace(",", ".");

                        if (oRecordset.Fields.Item("PorcentDescuento").Value.ToString() != "0")
                        {
                            strControl = oRecordset.Fields.Item("MontoDescuento").Value.ToString();
                            objDetalle.DescuentoMonto = strControl.Replace(",", ".");
                            strControl = oRecordset.Fields.Item("PorcentDescuento").Value.ToString();
                            objDetalle.DescuentoPct = strControl.Replace(",", ".");
                        }

                        objDetalle.MontoItem = oRecordset.Fields.Item("MontoTotal").Value.ToString();
                        objExportaciones.Detalle.Add(objDetalle);
                        oRecordset.MoveNext();
                    }
                }

                //Consulta para vista de referencias del documento
                strConsulta = "SELECT \"TipoDocNC\", \"NumeroNC\", \"FechaEmisionNC\", \"CodigoNC\", \"RazonNC\", \"DocEntry\", \"VisOrder\" " +
                    "FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEND_REFERENCIA_Query\"('PLACEHOLDER' = ('$$pObjType$$', '14'), 'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                if (oRecordset.RecordCount != 0)
                {
                    objExportaciones.Referencia = new List<Referencia>();
                    //Se cargan el objeto de referencia para la información que se va a enviar al XML
                    while (!oRecordset.EoF)
                    {
                        Referencia objReferencia = new Referencia();
                        //Llenar los Totales
                        objReferencia.NroLinRef = (int.Parse(oRecordset.Fields.Item("VisOrder").Value.ToString()) + 1).ToString();
                        objReferencia.TpoDocRef = oRecordset.Fields.Item("TipoDocNC").Value.ToString();
                        objReferencia.FolioRef = oRecordset.Fields.Item("NumeroNC").Value.ToString();
                        FormatFecha = oRecordset.Fields.Item("FechaEmisionNC").Value.ToString();
                        //Se le da formato correcto a las fechas
                        FormatFecha = FormatearFecha(FormatFecha);
                        objReferencia.FchRef = FormatFecha;
                        objReferencia.CodRef = oRecordset.Fields.Item("CodigoNC").Value.ToString();
                        objReferencia.RazonRef = oRecordset.Fields.Item("RazonNC").Value.ToString();
                        //Llenar el encabezado
                        objExportaciones.Referencia.Add(objReferencia);
                        oRecordset.MoveNext();
                    }
                }

                //Consulta para vista de Exportación
                strConsulta = "SELECT \"U_SCGCE_TipoDesp\",\"U_SCGCE_CondVentExp\",\"U_SCGCE_ModVenta\",\"U_SCGCE_PaisDestino\",\"U_SCGCE_TpoMoneda\", " +
                               " \"TipoMoneda\",sum(\"U_SCGCE_TotBultos\") AS \"U_SCGCE_TotBultos\",sum(\"DocRate\") AS \"DocRate\",sum(\"MntTotOtraMond\") AS \"MntTotOtraMond\" " +
                               "FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCENC_ENCABEZADOEXP_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                                "GROUP BY \"U_SCGCE_TipoDesp\",\"U_SCGCE_CondVentExp\",\"U_SCGCE_ModVenta\",\"U_SCGCE_PaisDestino\",\"U_SCGCE_TpoMoneda\",\"TipoMoneda\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan el objeto de Exportación ara la factura
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar los Totales
                    //objEncabezado.IdDoc.TipoDespacho = oRecordset.Fields.Item("U_SCGCE_TipoDesp").Value.ToString();
                    //objEncabezado.IdDoc.FmaPagExp = oRecordset.Fields.Item("U_SCGCE_CondVentExp").Value.ToString();
                    //objAduana.CodModVenta = oRecordset.Fields.Item("U_SCGCE_ModVenta").Value.ToString();
                    if (oRecordset.Fields.Item("U_SCGCE_TotBultos").Value.ToString() != "0")
                    {
                        objAduana.TotBultos = oRecordset.Fields.Item("U_SCGCE_TotBultos").Value.ToString();
                    }

                    //objAduana.CodPaisDestin = oRecordset.Fields.Item("U_SCGCE_PaisDestino").Value.ToString();
                    objTotales.TpoMoneda = oRecordset.Fields.Item("U_SCGCE_TpoMoneda").Value.ToString();
                    objOtraMoneda.TpoMoneda = oRecordset.Fields.Item("TipoMoneda").Value.ToString();
                    objOtraMoneda.TpoCambio = oRecordset.Fields.Item("DocRate").Value.ToString();
                    objOtraMoneda.MntExeOtrMnda = oRecordset.Fields.Item("MntTotOtraMond").Value.ToString();
                    objOtraMoneda.MntTotOtrMnda = oRecordset.Fields.Item("MntTotOtraMond").Value.ToString();
                    objTransporte.Aduana = objAduana;
                    objEncabezado.Transporte = objTransporte;
                    objEncabezado.OtraMoneda = objOtraMoneda;
                }

                objDte.Exportaciones = objExportaciones;
                return objDte;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DTE ConsultaFacturaCompra(string strKey, string tipoDoc, out string p_comentarios, SAPbobsCOM.Company oCompany)
        {
            try//Se deben validar los campos para la Factura Compra
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRecordsetAct = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string FormatFecha = string.Empty;
                string strConsulta = string.Empty;
                string strControl = string.Empty;
                string strDetAdic = string.Empty;
                string bdComp = ObtenerDatos(oCompany);
                objDte = new DTE();
                objDocumento = new SCGCE.BL.Objetos.Documento();
                objEncabezado = new Encabezado();
                objIdDoc = new IdDoc();
                objEmisor = new Emisor();
                objReceptor = new Receptor();
                objTotales = new Totales();
                p_comentarios = string.Empty;

                //Consulta para vista de encabezado del documento
                strConsulta = "SELECT \"Indicador\",sum(\"Folio\") AS \"Folio\",max(\"FechaDocumento\") AS \"FechaDocumento\",\"CondicionVenta\", " +
                    " \"PlazoCredito\", \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", " +
                    " \"EmSennas\", \"EmComuna\", \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", " +
                    " \"EmFaxNumero\", \"EmCorreoElectronico\", \"ReNumIdentificacion\", \"ReCodigoInterno\", \"ReRazonSocial\", \"ReGiroNegocio\", " +
                    " \"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", \"ReTelefono\", \"ReCodPaisFax\", \"ReFax\", \"ReEmail\", " +
                    " \"UserDll\", \"PassDll\", \"URLDll\" , \"Comentarios\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFC_ENCABEZADO_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pTipoDoc$$','" + tipoDoc + "'),'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"Indicador\", \"CondicionVenta\", \"PlazoCredito\", \"FechaVencimiento\", \"EmNumIdentificacion\", " +
                    " \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", \"EmSennas\", \"EmComuna\", \"EmCiudad\", \"EmTelefonoCodPais\", " +
                    " \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", \"ReNumIdentificacion\", \"ReCodigoInterno\", " +
                    " \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", \"ReTelefono\",  " +
                    " \"ReCodPaisFax\", \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\" , \"Comentarios\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan los objetos requeridos para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar la parte de IdDoc
                    objIdDoc.TipoDTE = oRecordset.Fields.Item("Indicador").Value.ToString();
                    objIdDoc.Folio = oRecordset.Fields.Item("Folio").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaDocumento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchEmis = FormatFecha;
                    objIdDoc.FmaPago = oRecordset.Fields.Item("CondicionVenta").Value.ToString();
                    objIdDoc.TermPagoDias = oRecordset.Fields.Item("PlazoCredito").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaVencimiento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchVenc = FormatFecha;
                    //Llenar la parte de Emisor
                    objEmisor.RUTEmisor = oRecordset.Fields.Item("EmNumIdentificacion").Value.ToString();
                    objEmisor.RznSoc = oRecordset.Fields.Item("EmRazonSocial").Value.ToString();
                    objEmisor.GiroEmis = oRecordset.Fields.Item("EmGiroNegocio").Value.ToString();
                    //objEmisor.Acteco = oRecordset.Fields.Item("EmActeco").Value.ToString();

                    //Se agrega recorrido de Actecos 
                    //strConsulta = @"Select ""U_Acteco"" FROM ""SYS_BIC.sap."" " + bdComp + @"CA_SCGCEFA_ACTECOS where ""U_Acteco"" <> '' ";
                    strConsulta = @"Select ""Acteco"" FROM ""_SYS_BIC"".""sap." + bdComp + @"/CA_SCGCE_ACTECOS_Query""";
                    oRecordsetAct.DoQuery(strConsulta);
                    if (oRecordsetAct.RecordCount != 0)
                    {
                        objEmisor.Acteco = new List<string>();
                        for (int i = 0; i < oRecordsetAct.RecordCount; i++)
                        {

                            objEmisor.Acteco.Add(oRecordsetAct.Fields.Item("Acteco").Value.ToString());
                            oRecordsetAct.MoveNext();

                        }
                    }


                    objEmisor.DirOrigen = oRecordset.Fields.Item("EmSennas").Value.ToString();
                    objEmisor.CmnaOrigen = oRecordset.Fields.Item("EmComuna").Value.ToString();
                    objEmisor.CiudadOrigen = oRecordset.Fields.Item("EmCiudad").Value.ToString();
                    //Llenar la parte de Receptor
                    objReceptor.RUTRecep = oRecordset.Fields.Item("ReNumIdentificacion").Value.ToString();
                    objReceptor.CdgIntRecep = oRecordset.Fields.Item("ReCodigoInterno").Value.ToString();
                    objReceptor.RznSocRecep = oRecordset.Fields.Item("ReRazonSocial").Value.ToString();
                    objReceptor.GiroRecep = oRecordset.Fields.Item("ReGiroNegocio").Value.ToString();
                    objReceptor.DirRecep = oRecordset.Fields.Item("ReSennas").Value.ToString();
                    objReceptor.CmnaRecep = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objReceptor.CiudadRecep = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.IdDoc = objIdDoc;
                    objEncabezado.Emisor = objEmisor;
                    objEncabezado.Receptor = objReceptor;

                    //Llenar Informaci{on del WS
                    UrlWS = oRecordset.Fields.Item("URLDll").Value.ToString();
                    UsuarioWS = oRecordset.Fields.Item("UserDll").Value.ToString();
                    PassWS = oRecordset.Fields.Item("PassDll").Value.ToString();
                    p_comentarios = oRecordset.Fields.Item("Comentarios").Value.ToString();
                }

                //Consulta para vista de Totales del documento
                strConsulta = "SELECT sum(\"TotalVentaNeta\") AS \"TotalVentaNeta\", sum(\"TotalGravado\") AS \"TotalGravado\", sum(\"TotalExento\") AS \"TotalExento\", sum(\"PorcentajeImpueto\") AS \"PorcentajeImpueto\", " +
                    " sum(\"TotalImpuesto\") AS \"TotalImpuesto\", sum(\"TotalComprobante\") AS \"TotalComprobante\", sum(\"TipoCambio\") AS \"TipoCambio\", sum(\"PorcentajeDescuento\") AS \"PorcentajeDescuento\", sum(\"DescuentoEncabezado\") AS \"DescuentoEncabezado\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFC_TOTALCOMPROBANTE_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                objDocumento.DscRcgGlobal = new List<DscRcgGlobal>();
                int Descuento = 0;

                //Se cargan el objeto de totales para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar los Totales
                    objTotales.MntNeto = oRecordset.Fields.Item("TotalVentaNeta").Value.ToString();
                    objTotales.MntExe = "0";
                    objTotales.TasaIVA = oRecordset.Fields.Item("PorcentajeImpueto").Value.ToString();
                    objTotales.IVA = oRecordset.Fields.Item("TotalImpuesto").Value.ToString();
                    objTotales.MntTotal = oRecordset.Fields.Item("TotalComprobante").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.Totales = objTotales;
                    //Llenar descuento
                    Descuento = Convert.ToInt32(oRecordset.Fields.Item("PorcentajeDescuento").Value.ToString());
                    if (Descuento != 0)
                    {
                        objDscRcgGlobal = new DscRcgGlobal();
                        objDscRcgGlobal.NroLinDR = "1";
                        objDscRcgGlobal.TpoMov = "D";
                        objDscRcgGlobal.GlosaDR = "Descuento";
                        objDscRcgGlobal.TpoValor = "$";
                        objDscRcgGlobal.ValorDR = oRecordset.Fields.Item("DescuentoEncabezado").Value.ToString();
                        objDocumento.DscRcgGlobal.Add(objDscRcgGlobal);
                    }
                }
                //Llenar el Documento
                objDocumento.Encabezado = objEncabezado;

                //Consulta para vista de Detalle del documento
                strConsulta = "SELECT sum(\"LineNum\"+1) AS \"LineNum\", 	\"CodigoTipoLinea\", 	\"Detalle\", \"DetalleAdic\",\"TipoCodigo\", " +
                    " sum(\"Cantidad\") AS \"Cantidad\", \"UnidadMedida\", sum(\"PrecioUnitario\") AS \"PrecioUnitario\", " +
                    " sum(\"MontoTotal\") AS \"MontoTotal\", sum(\"MontoTotalLinea\") AS \"MontoTotalLinea\", sum(\"PorcentDescuento\") AS \"PorcentDescuento\", sum(\"MontoDescuento\") AS \"MontoDescuento\"  " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEFC_DETALLE_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"LineNum\", \"CodigoTipoLinea\", \"Detalle\", \"DetalleAdic\", \"TipoCodigo\", \"UnidadMedida\" ORDER BY \"LineNum\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan el objeto para la información de Detalle que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    oRecordset.MoveFirst();
                    objDocumento.Detalle = new List<Detalle>();
                    for (int i = 0; i < oRecordset.RecordCount; i++)
                    {
                        objDetalle = new Detalle();
                        objDetalle.NroLinDet = oRecordset.Fields.Item("LineNum").Value.ToString();
                        objDetalle.CdgItem = new CdgItem();
                        objDetalle.CdgItem.TpoCodigo = oRecordset.Fields.Item("TipoCodigo").Value.ToString();
                        objDetalle.CdgItem.VlrCodigo = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        objDetalle.NmbItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        strDetAdic = oRecordset.Fields.Item("DetalleAdic").Value.ToString();
                        if (!string.IsNullOrEmpty(strDetAdic))
                        {
                            objDetalle.DscItem = strDetAdic;
                        }
                        //objDetalle.NmbItem = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        //objDetalle.DscItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        //objDetalle.QtyItem = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        //objDetalle.UnmdItem = oRecordset.Fields.Item("UnidadMedida").Value.ToString();
                        strControl = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        objDetalle.QtyItem = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        objDetalle.PrcItem = strControl.Replace(",", ".");
                        objDetalle.PrcItem = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();

                        strControl = oRecordset.Fields.Item("MontoDescuento").Value.ToString();
                        objDetalle.DescuentoMonto = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PorcentDescuento").Value.ToString();
                        objDetalle.DescuentoPct = strControl.Replace(",", ".");

                        objDetalle.MontoItem = oRecordset.Fields.Item("MontoTotal").Value.ToString();
                        objDocumento.Detalle.Add(objDetalle);
                        oRecordset.MoveNext();
                    }
                }

                //Consulta para vista de referencias del documento
                strConsulta = "SELECT \"TipoDocNC\", \"NumeroNC\", \"FechaEmisionNC\", \"CodigoNC\", \"RazonNC\", \"DocEntry\", \"VisOrder\" " +
                    "FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEND_REFERENCIA_Query\"('PLACEHOLDER' = ('$$pObjType$$', '18'), 'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                if (oRecordset.RecordCount != 0)
                {
                    objDocumento.Referencia = new List<Referencia>();
                    //Se cargan el objeto de referencia para la información que se va a enviar al XML
                    while (!oRecordset.EoF)
                    {
                        Referencia objReferencia = new Referencia();
                        //Llenar los Totales
                        objReferencia.NroLinRef = (int.Parse(oRecordset.Fields.Item("VisOrder").Value.ToString()) + 1).ToString();
                        objReferencia.TpoDocRef = oRecordset.Fields.Item("TipoDocNC").Value.ToString();
                        objReferencia.FolioRef = oRecordset.Fields.Item("NumeroNC").Value.ToString();
                        FormatFecha = oRecordset.Fields.Item("FechaEmisionNC").Value.ToString();
                        //Se le da formato correcto a las fechas
                        FormatFecha = FormatearFecha(FormatFecha);
                        objReferencia.FchRef = FormatFecha;
                        objReferencia.CodRef = oRecordset.Fields.Item("CodigoNC").Value.ToString();
                        objReferencia.RazonRef = oRecordset.Fields.Item("RazonNC").Value.ToString();
                        //Llenar el encabezado
                        objDocumento.Referencia.Add(objReferencia);
                        oRecordset.MoveNext();
                    }
                }

                objDte.Documento = objDocumento;
                return objDte;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DTE ConsultaGuiaDespacho(string strKey, string tipoDoc, string pAlmacen, out string p_comentarios, SAPbobsCOM.Company oCompany)
        {
            try//Se deben validar los campos para la Guía de Despacho
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRecordsetAct = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string FormatFecha = string.Empty;
                string strConsulta = string.Empty;
                string strControl = string.Empty;
                string strDetAdic = string.Empty;
                string bdComp = ObtenerDatos(oCompany);
                objDte = new DTE();
                objDocumento = new SCGCE.BL.Objetos.Documento();
                objEncabezado = new Encabezado();
                objIdDoc = new IdDoc();
                objEmisor = new Emisor();
                objReceptor = new Receptor();
                objTotales = new Totales();
                objTransporte = new Transporte();
                objAduana = new Aduana();
                p_comentarios = string.Empty;

                //Consulta para vista de encabezado del documento
                strConsulta = "SELECT \"Indicador\",sum(\"Folio\") AS \"Folio\",max(\"FechaDocumento\") AS \"FechaDocumento\",\"CondicionVenta\", " +
                    " \"FechaVencimiento\", \"TipoDespacho\", \"IndicadorTraslado\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", " +
                    " \"EmActeco\", \"EmSennas\", \"EmComuna\", \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", " +
                    " \"EmCorreoElectronico\", \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", " +
                    " \"ReTelefono\", \"ReCodPaisFax\", \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\", \"NombreTransp\", \"Comentarios\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEGD_ENCABEZADO_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pWhsCode$$', '" + pAlmacen + "'),'PLACEHOLDER' = ('$$pTipoDoc$$','" + tipoDoc + "'),'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"Indicador\", \"CondicionVenta\", \"FechaVencimiento\", \"TipoDespacho\", \"IndicadorTraslado\", \"EmNumIdentificacion\", " +
                    " \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", \"EmSennas\", \"EmComuna\", \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", " +
                    " \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\", " +
                    " \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", \"ReTelefono\", \"ReCodPaisFax\", \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\" ,\"NombreTransp\", \"Comentarios\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan los objetos requeridos para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar la parte de IdDoc
                    strControl = oRecordset.Fields.Item("Indicador").Value.ToString();
                    if (strControl == "Y")
                    {
                        objIdDoc.TipoDTE = "52";
                    }

                    objIdDoc.Folio = oRecordset.Fields.Item("Folio").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaDocumento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchEmis = FormatFecha;
                    objIdDoc.TipoDespacho = oRecordset.Fields.Item("TipoDespacho").Value.ToString();
                    objIdDoc.IndTraslado = oRecordset.Fields.Item("IndicadorTraslado").Value.ToString();
                    //Llenar la parte de Emisor
                    objEmisor.RUTEmisor = oRecordset.Fields.Item("EmNumIdentificacion").Value.ToString();
                    objEmisor.RznSoc = oRecordset.Fields.Item("EmRazonSocial").Value.ToString();
                    objEmisor.GiroEmis = oRecordset.Fields.Item("EmGiroNegocio").Value.ToString();
                    //  objEmisor.Acteco = oRecordset.Fields.Item("EmActeco").Value.ToString();

                    //Se agrega recorrido de Actecos 
                    strConsulta = @"Select ""Acteco"" FROM ""_SYS_BIC"".""sap." + bdComp + @"/CA_SCGCE_ACTECOS_Query""";
                    oRecordsetAct.DoQuery(strConsulta);
                    if (oRecordsetAct.RecordCount != 0)
                    {
                        objEmisor.Acteco = new List<string>();
                        for (int i = 0; i < oRecordsetAct.RecordCount; i++)
                        {

                            objEmisor.Acteco.Add(oRecordsetAct.Fields.Item("Acteco").Value.ToString());
                            oRecordsetAct.MoveNext();

                        }
                    }

                    objEmisor.DirOrigen = oRecordset.Fields.Item("EmSennas").Value.ToString();
                    objEmisor.CmnaOrigen = oRecordset.Fields.Item("EmComuna").Value.ToString();
                    objEmisor.CiudadOrigen = oRecordset.Fields.Item("EmCiudad").Value.ToString();
                    //Llenar la parte de Receptor
                    objReceptor.RUTRecep = objEmisor.RUTEmisor;
                    objReceptor.RznSocRecep = objEmisor.RznSoc;
                    objReceptor.GiroRecep = objEmisor.GiroEmis;
                    objReceptor.DirRecep = oRecordset.Fields.Item("ReSennas").Value.ToString();
                    objReceptor.CmnaRecep = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objReceptor.CiudadRecep = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    objTransporte.DirDest = oRecordset.Fields.Item("ReSennas").Value.ToString();
                    objTransporte.CmnaDest = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objTransporte.CiudadDest = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    objAduana.NombreTransp = oRecordset.Fields.Item("NombreTransp").Value.ToString();
                    objTransporte.Aduana = objAduana;
                    //Llenar el encabezado
                    objEncabezado.IdDoc = objIdDoc;
                    objEncabezado.Emisor = objEmisor;
                    objEncabezado.Receptor = objReceptor;
                    objEncabezado.Transporte = objTransporte;

                    //Llenar Informaci{on del WS
                    UrlWS = oRecordset.Fields.Item("URLDll").Value.ToString();
                    UsuarioWS = oRecordset.Fields.Item("UserDll").Value.ToString();
                    PassWS = oRecordset.Fields.Item("PassDll").Value.ToString();
                    p_comentarios = oRecordset.Fields.Item("Comentarios").Value.ToString();
                }

                //Consulta para vista de Totales del documento
                strConsulta = "SELECT sum(\"TotalVentaNeta\") AS \"TotalVentaNeta\", sum(\"TotalGravado\") AS \"TotalGravado\", sum(\"TotalExento\") AS \"TotalExento\", sum(\"PorcentajeImpueto\") AS \"PorcentajeImpueto\", " +
                    " sum(\"TotalImpuesto\") AS \"TotalImpuesto\", sum(\"TotalComprobante\") AS \"TotalComprobante\", sum(\"TipoCambio\") AS \"TipoCambio\", sum(\"PorcentajeDescuento\") AS \"PorcentajeDescuento\", sum(\"DescuentoEncabezado\") AS \"DescuentoEncabezado\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEGD_TOTALCOMPROBANTE_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                objDocumento.DscRcgGlobal = new List<DscRcgGlobal>();
                int Descuento = 0;

                //Se cargan el objeto de totales para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar los Totales
                    objTotales.MntNeto = oRecordset.Fields.Item("TotalVentaNeta").Value.ToString();
                    objTotales.MntExe = "0";
                    objTotales.TasaIVA = oRecordset.Fields.Item("PorcentajeImpueto").Value.ToString();
                    objTotales.IVA = oRecordset.Fields.Item("TotalImpuesto").Value.ToString();
                    objTotales.MntTotal = oRecordset.Fields.Item("TotalComprobante").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.Totales = objTotales;
                    //Llenar descuento
                    Descuento = Convert.ToInt32(oRecordset.Fields.Item("PorcentajeDescuento").Value.ToString());
                    if (Descuento != 0)
                    {
                        objDscRcgGlobal = new DscRcgGlobal();
                        objDscRcgGlobal.NroLinDR = "1";
                        objDscRcgGlobal.TpoMov = "D";
                        objDscRcgGlobal.GlosaDR = "Descuento";
                        objDscRcgGlobal.TpoValor = "$";
                        objDscRcgGlobal.ValorDR = oRecordset.Fields.Item("DescuentoEncabezado").Value.ToString();
                        objDocumento.DscRcgGlobal.Add(objDscRcgGlobal);
                    }
                }
                //Llenar el Documento
                objDocumento.Encabezado = objEncabezado;

                //Consulta para vista de Detalle del documento
                strConsulta = "SELECT sum(\"LineNum\"+1) AS \"LineNum\", \"CodigoTipoLinea\", 	\"Detalle\",\"DetalleAdic\",\"TipoCodigo\", sum(\"Cantidad\") AS \"Cantidad\", " +
                    " \"UnidadMedida\", sum(\"PrecioUnitario\") AS \"PrecioUnitario\", sum(\"MontoTotal\") AS \"MontoTotal\", " +
                    " sum(\"MontoTotalLinea\") AS \"MontoTotalLinea\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEGD_DETALLE_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"LineNum\", \"CodigoTipoLinea\", \"Detalle\", \"DetalleAdic\", \"TipoCodigo\", \"UnidadMedida\" ORDER BY \"LineNum\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan el objeto para la información de Detalle que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    oRecordset.MoveFirst();
                    objDocumento.Detalle = new List<Detalle>();
                    for (int i = 0; i < oRecordset.RecordCount; i++)
                    {
                        objDetalle = new Detalle();
                        objDetalle.NroLinDet = oRecordset.Fields.Item("LineNum").Value.ToString();
                        objDetalle.CdgItem = new CdgItem();
                        objDetalle.CdgItem.TpoCodigo = oRecordset.Fields.Item("TipoCodigo").Value.ToString();
                        objDetalle.CdgItem.VlrCodigo = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        objDetalle.NmbItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        strDetAdic = oRecordset.Fields.Item("DetalleAdic").Value.ToString();
                        if (!string.IsNullOrEmpty(strDetAdic))
                        {
                            objDetalle.DscItem = strDetAdic;
                        }
                        //objDetalle.NmbItem = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        //objDetalle.DscItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        //objDetalle.QtyItem = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        objDetalle.UnmdItem = oRecordset.Fields.Item("UnidadMedida").Value.ToString();
                        strControl = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        objDetalle.QtyItem = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        objDetalle.PrcItem = strControl.Replace(",", ".");
                        //objDetalle.PrcRef = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        objDetalle.MontoItem = oRecordset.Fields.Item("MontoTotal").Value.ToString(); ;
                        objDocumento.Detalle.Add(objDetalle);
                        oRecordset.MoveNext();
                    }
                }

                //Consulta para vista de referencias del documento
                strConsulta = "SELECT \"TipoDocNC\", \"NumeroNC\", \"FechaEmisionNC\", \"CodigoNC\", \"RazonNC\", \"DocEntry\", \"VisOrder\" " +
                    "FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEND_REFERENCIA_Query\"('PLACEHOLDER' = ('$$pObjType$$', '67'), 'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                if (oRecordset.RecordCount != 0)
                {
                    objDocumento.Referencia = new List<Referencia>();
                    //Se cargan el objeto de referencia para la información que se va a enviar al XML
                    while (!oRecordset.EoF)
                    {
                        Referencia objReferencia = new Referencia();
                        //Llenar los Totales
                        objReferencia.NroLinRef = (int.Parse(oRecordset.Fields.Item("VisOrder").Value.ToString()) + 1).ToString();
                        objReferencia.TpoDocRef = oRecordset.Fields.Item("TipoDocNC").Value.ToString();
                        objReferencia.FolioRef = oRecordset.Fields.Item("NumeroNC").Value.ToString();
                        FormatFecha = oRecordset.Fields.Item("FechaEmisionNC").Value.ToString();
                        //Se le da formato correcto a las fechas
                        FormatFecha = FormatearFecha(FormatFecha);
                        objReferencia.FchRef = FormatFecha;
                        objReferencia.CodRef = oRecordset.Fields.Item("CodigoNC").Value.ToString();
                        objReferencia.RazonRef = oRecordset.Fields.Item("RazonNC").Value.ToString();
                        //Llenar el encabezado
                        objDocumento.Referencia.Add(objReferencia);
                        oRecordset.MoveNext();
                    }
                }

                objDte.Documento = objDocumento;
                return objDte;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DTE ConsultaGuiaDespachoEntrega(string strKey, string tipoDoc, out string p_comentarios, SAPbobsCOM.Company oCompany)
        {
            try//Se deben validar los campos para la Guía de Despacho
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRecordsetAct = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string FormatFecha = string.Empty;
                string strConsulta = string.Empty;
                string strControl = string.Empty;
                string strDetAdic = string.Empty;
                string bdComp = ObtenerDatos(oCompany);
                objDte = new DTE();
                objDocumento = new SCGCE.BL.Objetos.Documento();
                objEncabezado = new Encabezado();
                objIdDoc = new IdDoc();
                objEmisor = new Emisor();
                objReceptor = new Receptor();
                objTotales = new Totales();
                objTransporte = new Transporte();
                objAduana = new Aduana();
                p_comentarios = string.Empty;

                //Consulta para vista de encabezado del documento
                strConsulta = "SELECT \"Indicador\",sum(\"Folio\") AS \"Folio\",max(\"FechaDocumento\") AS \"FechaDocumento\",\"CondicionVenta\", " +
                    " \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", \"EmSennas\", \"EmComuna\", " +
                    " \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\",\"ReSennas\",\"DirDest\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", " +
                    " \"ReTelefono\", \"ReCodPaisFax\", \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\", \"NombreTransp\",\"Comentarios\",\"TipoDespacho\",\"IndicadorTraslado\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEEN_ENCABEZADO_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pTipoDoc$$','" + tipoDoc + "'),'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"Indicador\", \"CondicionVenta\", \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", " +
                    " \"EmSennas\", \"EmComuna\", \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\",\"DirDest\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", \"ReTelefono\", \"ReCodPaisFax\", " +
                    " \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\", \"NombreTransp\",\"Comentarios\",\"TipoDespacho\",\"IndicadorTraslado\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan los objetos requeridos para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar la parte de IdDoc
                    objIdDoc.TipoDTE = oRecordset.Fields.Item("Indicador").Value.ToString();
                    objIdDoc.Folio = oRecordset.Fields.Item("Folio").Value.ToString();
                    objIdDoc.IndTraslado = oRecordset.Fields.Item("IndicadorTraslado").Value.ToString();
                    objIdDoc.TipoDespacho = oRecordset.Fields.Item("TipoDespacho").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaDocumento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchEmis = FormatFecha;
                    objIdDoc.FmaPago = oRecordset.Fields.Item("CondicionVenta").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaVencimiento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchVenc = FormatFecha;
                    //Llenar la parte de Emisor
                    objEmisor.RUTEmisor = oRecordset.Fields.Item("EmNumIdentificacion").Value.ToString();
                    objEmisor.RznSoc = oRecordset.Fields.Item("EmRazonSocial").Value.ToString();
                    objEmisor.GiroEmis = oRecordset.Fields.Item("EmGiroNegocio").Value.ToString();
                    //objEmisor.Acteco = oRecordset.Fields.Item("EmActeco").Value.ToString();

                    //Se agrega recorrido de Actecos 
                    strConsulta = @"Select ""Acteco"" FROM ""_SYS_BIC"".""sap." + bdComp + @"/CA_SCGCE_ACTECOS_Query""";
                    oRecordsetAct.DoQuery(strConsulta);
                    if (oRecordsetAct.RecordCount != 0)
                    {
                        objEmisor.Acteco = new List<string>();
                        for (int i = 0; i < oRecordsetAct.RecordCount; i++)
                        {

                            objEmisor.Acteco.Add(oRecordsetAct.Fields.Item("Acteco").Value.ToString());
                            oRecordsetAct.MoveNext();

                        }
                    }

                    objEmisor.DirOrigen = oRecordset.Fields.Item("EmSennas").Value.ToString();
                    objEmisor.CmnaOrigen = oRecordset.Fields.Item("EmComuna").Value.ToString();
                    objEmisor.CiudadOrigen = oRecordset.Fields.Item("EmCiudad").Value.ToString();
                    //Llenar la parte de Receptor
                    objReceptor.RUTRecep = oRecordset.Fields.Item("ReNumIdentificacion").Value.ToString();
                    objReceptor.RznSocRecep = oRecordset.Fields.Item("ReRazonSocial").Value.ToString();
                    objReceptor.GiroRecep = oRecordset.Fields.Item("ReGiroNegocio").Value.ToString();
                    objReceptor.DirRecep = oRecordset.Fields.Item("ReSennas").Value.ToString();
                    objReceptor.CmnaRecep = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objReceptor.CiudadRecep = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    objTransporte.DirDest = oRecordset.Fields.Item("DirDest").Value.ToString();
                    objTransporte.CmnaDest = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objTransporte.CiudadDest = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    objAduana.NombreTransp = oRecordset.Fields.Item("NombreTransp").Value.ToString();
                    objTransporte.Aduana = objAduana;
                    //Llenar el encabezado
                    objEncabezado.IdDoc = objIdDoc;
                    objEncabezado.Emisor = objEmisor;
                    objEncabezado.Receptor = objReceptor;
                    objEncabezado.Transporte = objTransporte;

                    //Llenar Informaci{on del WS
                    UrlWS = oRecordset.Fields.Item("URLDll").Value.ToString();
                    UsuarioWS = oRecordset.Fields.Item("UserDll").Value.ToString();
                    PassWS = oRecordset.Fields.Item("PassDll").Value.ToString();
                    p_comentarios = oRecordset.Fields.Item("Comentarios").Value.ToString();
                }

                //Consulta para vista de Totales del documento
                strConsulta = "SELECT sum(\"TotalVentaNeta\") AS \"TotalVentaNeta\", sum(\"TotalGravado\") AS \"TotalGravado\", sum(\"TotalExento\") AS \"TotalExento\", sum(\"PorcentajeImpueto\") AS \"PorcentajeImpueto\", " +
                    " sum(\"TotalImpuesto\") AS \"TotalImpuesto\", sum(\"TotalComprobante\") AS \"TotalComprobante\", sum(\"TipoCambio\") AS \"TipoCambio\", sum(\"PorcentajeDescuento\") AS \"PorcentajeDescuento\", sum(\"DescuentoEncabezado\") AS \"DescuentoEncabezado\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEEN_TOTALCOMPROBANTE_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                objDocumento.DscRcgGlobal = new List<DscRcgGlobal>();
                int Descuento = 0;

                //Se cargan el objeto de totales para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar los Totales
                    objTotales.MntNeto = oRecordset.Fields.Item("TotalVentaNeta").Value.ToString();
                    objTotales.MntExe = "0";
                    objTotales.TasaIVA = oRecordset.Fields.Item("PorcentajeImpueto").Value.ToString();
                    objTotales.IVA = oRecordset.Fields.Item("TotalImpuesto").Value.ToString();
                    objTotales.MntTotal = oRecordset.Fields.Item("TotalComprobante").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.Totales = objTotales;
                    //Llenar descuento
                    Descuento = Convert.ToInt32(oRecordset.Fields.Item("PorcentajeDescuento").Value.ToString());
                    if (Descuento != 0)
                    {
                        objDscRcgGlobal = new DscRcgGlobal();
                        objDscRcgGlobal.NroLinDR = "1";
                        objDscRcgGlobal.TpoMov = "D";
                        objDscRcgGlobal.GlosaDR = "Descuento";
                        objDscRcgGlobal.TpoValor = "$";
                        objDscRcgGlobal.ValorDR = oRecordset.Fields.Item("DescuentoEncabezado").Value.ToString();
                        objDocumento.DscRcgGlobal.Add(objDscRcgGlobal);
                    }
                }
                //Llenar el Documento
                objDocumento.Encabezado = objEncabezado;

                //Consulta para vista de Detalle del documento
                strConsulta = "SELECT sum(\"LineNum\"+1) AS \"LineNum\", \"CodigoTipoLinea\", \"Detalle\",\"DetalleAdic\",	\"TipoCodigo\", sum(\"Cantidad\") AS \"Cantidad\", " +
                    " \"UnidadMedida\", sum(\"PrecioUnitario\") AS \"PrecioUnitario\", sum(\"MontoTotal\") AS \"MontoTotal\", " +
                    " sum(\"MontoTotalLinea\") AS \"MontoTotalLinea\", sum(\"PorcentDescuento\") AS \"PorcentDescuento\", sum(\"MontoDescuento\") AS \"MontoDescuento\"  " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEEN_DETALLE_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"LineNum\", \"CodigoTipoLinea\", \"Detalle\",\"DetalleAdic\",	\"TipoCodigo\", \"UnidadMedida\" ORDER BY \"LineNum\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan el objeto para la información de Detalle que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    oRecordset.MoveFirst();
                    objDocumento.Detalle = new List<Detalle>();
                    for (int i = 0; i < oRecordset.RecordCount; i++)
                    {
                        objDetalle = new Detalle();
                        objDetalle.NroLinDet = oRecordset.Fields.Item("LineNum").Value.ToString();
                        objDetalle.CdgItem = new CdgItem();
                        objDetalle.CdgItem.TpoCodigo = oRecordset.Fields.Item("TipoCodigo").Value.ToString();
                        objDetalle.CdgItem.VlrCodigo = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        objDetalle.NmbItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        strDetAdic = oRecordset.Fields.Item("DetalleAdic").Value.ToString();
                        //if (!string.IsNullOrEmpty(strDetAdic))
                        //{
                        //    objDetalle.DscItem = strDetAdic;
                        //}
                        //objDetalle.NmbItem = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        //objDetalle.DscItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        //objDetalle.QtyItem = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        objDetalle.UnmdItem = oRecordset.Fields.Item("UnidadMedida").Value.ToString();
                        strControl = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        objDetalle.QtyItem = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        objDetalle.PrcItem = strControl.Replace(",", ".");

                        strControl = oRecordset.Fields.Item("MontoDescuento").Value.ToString();
                        objDetalle.DescuentoMonto = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PorcentDescuento").Value.ToString();
                        objDetalle.DescuentoPct = strControl.Replace(",", ".");

                        //objDetalle.PrcRef = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        objDetalle.MontoItem = oRecordset.Fields.Item("MontoTotal").Value.ToString(); ;
                        objDocumento.Detalle.Add(objDetalle);
                        oRecordset.MoveNext();
                    }
                }

                //Consulta para vista de referencias del documento
                strConsulta = "SELECT \"TipoDocNC\", \"NumeroNC\", \"FechaEmisionNC\", \"CodigoNC\", \"RazonNC\", \"DocEntry\", \"VisOrder\" " +
                    "FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEND_REFERENCIA_Query\"('PLACEHOLDER' = ('$$pObjType$$', '15'), 'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                if (oRecordset.RecordCount != 0)
                {
                    objDocumento.Referencia = new List<Referencia>();
                    //Se cargan el objeto de referencia para la información que se va a enviar al XML
                    while (!oRecordset.EoF)
                    {
                        Referencia objReferencia = new Referencia();
                        //Llenar los Totales
                        objReferencia.NroLinRef = (int.Parse(oRecordset.Fields.Item("VisOrder").Value.ToString()) + 1).ToString();
                        objReferencia.TpoDocRef = oRecordset.Fields.Item("TipoDocNC").Value.ToString();
                        objReferencia.FolioRef = oRecordset.Fields.Item("NumeroNC").Value.ToString();
                        FormatFecha = oRecordset.Fields.Item("FechaEmisionNC").Value.ToString();
                        //Se le da formato correcto a las fechas
                        FormatFecha = FormatearFecha(FormatFecha);
                        objReferencia.FchRef = FormatFecha;
                        objReferencia.CodRef = oRecordset.Fields.Item("CodigoNC").Value.ToString();
                        objReferencia.RazonRef = oRecordset.Fields.Item("RazonNC").Value.ToString();
                        //Llenar el encabezado
                        objDocumento.Referencia.Add(objReferencia);
                        oRecordset.MoveNext();
                    }
                }

                objDte.Documento = objDocumento;
                return objDte;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DTE ConsultaGuiaDespachoDevolucion(string strKey, string tipoDoc, out string p_comentarios, SAPbobsCOM.Company oCompany)
        {
            try//Se deben validar los campos para la Guía de Despacho
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRecordsetAct = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string FormatFecha = string.Empty;
                string strConsulta = string.Empty;
                string strControl = string.Empty;
                string strDetAdic = string.Empty;
                string bdComp = ObtenerDatos(oCompany);
                objDte = new DTE();
                objDocumento = new SCGCE.BL.Objetos.Documento();
                objEncabezado = new Encabezado();
                objIdDoc = new IdDoc();
                objEmisor = new Emisor();
                objReceptor = new Receptor();
                objTotales = new Totales();
                objTransporte = new Transporte();
                objAduana = new Aduana();
                p_comentarios = string.Empty;

                //Consulta para vista de encabezado del documento
                strConsulta = "SELECT \"Indicador\",sum(\"Folio\") AS \"Folio\",max(\"FechaDocumento\") AS \"FechaDocumento\",\"CondicionVenta\", " +
                    " \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", \"EmSennas\", \"EmComuna\", " +
                    " \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\",\"ReSennas\",\"DirDest\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", " +
                    " \"ReTelefono\", \"ReCodPaisFax\", \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\",  \"NombreTransp\",\"Comentarios\",\"TipoDespacho\",\"IndicadorTraslado\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEDP_ENCABEZADO_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pTipoDoc$$','" + tipoDoc + "'),'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"Indicador\", \"CondicionVenta\", \"FechaVencimiento\", \"EmNumIdentificacion\", \"EmRazonSocial\", \"EmGiroNegocio\", \"EmActeco\", " +
                    " \"EmSennas\", \"EmComuna\", \"EmCiudad\", \"EmTelefonoCodPais\", \"EmTelefonoNumero\", \"EmFaxCodPais\", \"EmFaxNumero\", \"EmCorreoElectronico\", " +
                    " \"ReNumIdentificacion\", \"ReRazonSocial\", \"ReGiroNegocio\", \"ReSennas\",\"DirDest\", \"ReBarrio\", \"ReDistrito\", \"ReCodPaisTel\", \"ReTelefono\", \"ReCodPaisFax\", " +
                    " \"ReFax\", \"ReEmail\", \"UserDll\", \"PassDll\", \"URLDll\", \"NombreTransp\", \"Comentarios\",\"TipoDespacho\",\"IndicadorTraslado\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan los objetos requeridos para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar la parte de IdDoc
                    objIdDoc.TipoDTE = oRecordset.Fields.Item("Indicador").Value.ToString();
                    objIdDoc.Folio = oRecordset.Fields.Item("Folio").Value.ToString();
                    objIdDoc.IndTraslado = oRecordset.Fields.Item("IndicadorTraslado").Value.ToString();
                    objIdDoc.TipoDespacho = oRecordset.Fields.Item("TipoDespacho").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaDocumento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchEmis = FormatFecha;
                    objIdDoc.FmaPago = oRecordset.Fields.Item("CondicionVenta").Value.ToString();
                    FormatFecha = oRecordset.Fields.Item("FechaVencimiento").Value.ToString();
                    //Se le da formato correcto a las fechas
                    FormatFecha = FormatearFecha(FormatFecha);
                    objIdDoc.FchVenc = FormatFecha;
                    //Llenar la parte de Emisor
                    objEmisor.RUTEmisor = oRecordset.Fields.Item("EmNumIdentificacion").Value.ToString();
                    objEmisor.RznSoc = oRecordset.Fields.Item("EmRazonSocial").Value.ToString();
                    objEmisor.GiroEmis = oRecordset.Fields.Item("EmGiroNegocio").Value.ToString();
                    //objEmisor.Acteco = oRecordset.Fields.Item("EmActeco").Value.ToString();

                    //Se agrega recorrido de Actecos 
                    strConsulta = @"Select ""Acteco"" FROM ""_SYS_BIC"".""sap." + bdComp + @"/CA_SCGCE_ACTECOS_Query""";
                    oRecordsetAct.DoQuery(strConsulta);
                    if (oRecordsetAct.RecordCount != 0)
                    {
                        objEmisor.Acteco = new List<string>();
                        for (int i = 0; i < oRecordsetAct.RecordCount; i++)
                        {

                            objEmisor.Acteco.Add(oRecordsetAct.Fields.Item("Acteco").Value.ToString());
                            oRecordsetAct.MoveNext();

                        }
                    }

                    objEmisor.DirOrigen = oRecordset.Fields.Item("EmSennas").Value.ToString();
                    objEmisor.CmnaOrigen = oRecordset.Fields.Item("EmComuna").Value.ToString();
                    objEmisor.CiudadOrigen = oRecordset.Fields.Item("EmCiudad").Value.ToString();
                    //Llenar la parte de Receptor
                    objReceptor.RUTRecep = oRecordset.Fields.Item("ReNumIdentificacion").Value.ToString();
                    objReceptor.RznSocRecep = oRecordset.Fields.Item("ReRazonSocial").Value.ToString();
                    objReceptor.GiroRecep = oRecordset.Fields.Item("ReGiroNegocio").Value.ToString();
                    objReceptor.DirRecep = oRecordset.Fields.Item("ReSennas").Value.ToString();
                    objReceptor.CmnaRecep = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objReceptor.CiudadRecep = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    objTransporte.DirDest = oRecordset.Fields.Item("DirDest").Value.ToString();
                    objTransporte.CmnaDest = oRecordset.Fields.Item("ReBarrio").Value.ToString();
                    objTransporte.CiudadDest = oRecordset.Fields.Item("ReDistrito").Value.ToString();
                    objAduana.NombreTransp = oRecordset.Fields.Item("NombreTransp").Value.ToString();
                    objTransporte.Aduana = objAduana;
                    //Llenar el encabezado
                    objEncabezado.IdDoc = objIdDoc;
                    objEncabezado.Emisor = objEmisor;
                    objEncabezado.Receptor = objReceptor;
                    objEncabezado.Transporte = objTransporte;

                    //Llenar Informaci{on del WS
                    UrlWS = oRecordset.Fields.Item("URLDll").Value.ToString();
                    UsuarioWS = oRecordset.Fields.Item("UserDll").Value.ToString();
                    PassWS = oRecordset.Fields.Item("PassDll").Value.ToString();
                    p_comentarios = oRecordset.Fields.Item("Comentarios").Value.ToString();
                }

                //Consulta para vista de Totales del documento
                strConsulta = "SELECT sum(\"TotalVentaNeta\") AS \"TotalVentaNeta\", sum(\"TotalGravado\") AS \"TotalGravado\", sum(\"TotalExento\") AS \"TotalExento\", sum(\"PorcentajeImpueto\") AS \"PorcentajeImpueto\", " +
                    " sum(\"TotalImpuesto\") AS \"TotalImpuesto\", sum(\"TotalComprobante\") AS \"TotalComprobante\", sum(\"TipoCambio\") AS \"TipoCambio\", sum(\"PorcentajeDescuento\") AS \"PorcentajeDescuento\", sum(\"DescuentoEncabezado\") AS \"DescuentoEncabezado\" " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEDP_TOTALCOMPROBANTE_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                objDocumento.DscRcgGlobal = new List<DscRcgGlobal>();
                int Descuento = 0;

                //Se cargan el objeto de totales para la información de encabezado que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    //Llenar los Totales
                    objTotales.MntNeto = oRecordset.Fields.Item("TotalVentaNeta").Value.ToString();
                    objTotales.MntExe = "0";
                    objTotales.TasaIVA = oRecordset.Fields.Item("PorcentajeImpueto").Value.ToString();
                    objTotales.IVA = oRecordset.Fields.Item("TotalImpuesto").Value.ToString();
                    objTotales.MntTotal = oRecordset.Fields.Item("TotalComprobante").Value.ToString();
                    //Llenar el encabezado
                    objEncabezado.Totales = objTotales;
                    //Llenar descuento
                    Descuento = Convert.ToInt32(oRecordset.Fields.Item("PorcentajeDescuento").Value.ToString());
                    if (Descuento != 0)
                    {
                        objDscRcgGlobal = new DscRcgGlobal();
                        objDscRcgGlobal.NroLinDR = "1";
                        objDscRcgGlobal.TpoMov = "D";
                        objDscRcgGlobal.GlosaDR = "Descuento";
                        objDscRcgGlobal.TpoValor = "$";
                        objDscRcgGlobal.ValorDR = oRecordset.Fields.Item("DescuentoEncabezado").Value.ToString();
                        objDocumento.DscRcgGlobal.Add(objDscRcgGlobal);
                    }
                }
                //Llenar el Documento
                objDocumento.Encabezado = objEncabezado;

                //Consulta para vista de Detalle del documento
                strConsulta = "SELECT sum(\"LineNum\"+1) AS \"LineNum\", \"CodigoTipoLinea\", \"Detalle\",\"DetalleAdic\",	\"TipoCodigo\", sum(\"Cantidad\") AS \"Cantidad\", " +
                    " \"UnidadMedida\", sum(\"PrecioUnitario\") AS \"PrecioUnitario\", sum(\"MontoTotal\") AS \"MontoTotal\", " +
                    " sum(\"MontoTotalLinea\") AS \"MontoTotalLinea\", sum(\"PorcentDescuento\") AS \"PorcentDescuento\", sum(\"MontoDescuento\") AS \"MontoDescuento\"  " +
                    " FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEDP_DETALLE_DOCUMENTO_Query\"('PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "')) " +
                    " GROUP BY \"LineNum\", \"CodigoTipoLinea\", \"Detalle\",\"DetalleAdic\",	\"TipoCodigo\", \"UnidadMedida\" ORDER BY \"LineNum\" ";

                oRecordset.DoQuery(strConsulta);

                //Se cargan el objeto para la información de Detalle que se va a enviar al XML
                if (oRecordset.RecordCount != 0)
                {
                    oRecordset.MoveFirst();
                    objDocumento.Detalle = new List<Detalle>();
                    for (int i = 0; i < oRecordset.RecordCount; i++)
                    {
                        objDetalle = new Detalle();
                        objDetalle.NroLinDet = oRecordset.Fields.Item("LineNum").Value.ToString();
                        objDetalle.CdgItem = new CdgItem();
                        objDetalle.CdgItem.TpoCodigo = oRecordset.Fields.Item("TipoCodigo").Value.ToString();
                        objDetalle.CdgItem.VlrCodigo = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        objDetalle.NmbItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        strDetAdic = oRecordset.Fields.Item("DetalleAdic").Value.ToString();
                        //if (!string.IsNullOrEmpty(strDetAdic))
                        //{
                        //    objDetalle.DscItem = strDetAdic;
                        //}
                        //objDetalle.NmbItem = oRecordset.Fields.Item("CodigoTipoLinea").Value.ToString();
                        //objDetalle.DscItem = oRecordset.Fields.Item("Detalle").Value.ToString();
                        //objDetalle.QtyItem = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        objDetalle.UnmdItem = oRecordset.Fields.Item("UnidadMedida").Value.ToString();
                        strControl = oRecordset.Fields.Item("Cantidad").Value.ToString();
                        objDetalle.QtyItem = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        objDetalle.PrcItem = strControl.Replace(",", ".");

                        strControl = oRecordset.Fields.Item("MontoDescuento").Value.ToString();
                        objDetalle.DescuentoMonto = strControl.Replace(",", ".");
                        strControl = oRecordset.Fields.Item("PorcentDescuento").Value.ToString();
                        objDetalle.DescuentoPct = strControl.Replace(",", ".");

                        //objDetalle.PrcRef = oRecordset.Fields.Item("PrecioUnitario").Value.ToString();
                        objDetalle.MontoItem = oRecordset.Fields.Item("MontoTotal").Value.ToString(); ;
                        objDocumento.Detalle.Add(objDetalle);
                        oRecordset.MoveNext();
                    }
                }

                //Consulta para vista de referencias del documento
                strConsulta = "SELECT \"TipoDocNC\", \"NumeroNC\", \"FechaEmisionNC\", \"CodigoNC\", \"RazonNC\", \"DocEntry\", \"VisOrder\" " +
                    "FROM \"_SYS_BIC\".\"sap." + bdComp + "/CA_SCGCEND_REFERENCIA_Query\"('PLACEHOLDER' = ('$$pObjType$$', '13'), 'PLACEHOLDER' = ('$$pDocEntry$$', '" + strKey + "'))";

                oRecordset.DoQuery(strConsulta);
                if (oRecordset.RecordCount != 0)
                {
                    objDocumento.Referencia = new List<Referencia>();
                    //Se cargan el objeto de referencia para la información que se va a enviar al XML
                    while (!oRecordset.EoF)
                    {
                        Referencia objReferencia = new Referencia();
                        //Llenar los Totales
                        objReferencia.NroLinRef = (int.Parse(oRecordset.Fields.Item("VisOrder").Value.ToString()) + 1).ToString();
                        objReferencia.TpoDocRef = oRecordset.Fields.Item("TipoDocNC").Value.ToString();
                        objReferencia.FolioRef = oRecordset.Fields.Item("NumeroNC").Value.ToString();
                        FormatFecha = oRecordset.Fields.Item("FechaEmisionNC").Value.ToString();
                        //Se le da formato correcto a las fechas
                        FormatFecha = FormatearFecha(FormatFecha);
                        objReferencia.FchRef = FormatFecha;
                        objReferencia.CodRef = oRecordset.Fields.Item("CodigoNC").Value.ToString();
                        objReferencia.RazonRef = oRecordset.Fields.Item("RazonNC").Value.ToString();
                        //Llenar el encabezado
                        objDocumento.Referencia.Add(objReferencia);
                        oRecordset.MoveNext();
                    }
                }


                objDte.Documento = objDocumento;
                return objDte;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static string GetXMLFromObject(DTE pDocumento)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(pDocumento.GetType());
                tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, pDocumento, ns);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }
            }
            string XML = sw.ToString();
            XML = XML.Replace("encoding=\"utf-16\"", "encoding=\"ISO-8859-1\"");
            //XML = XML.Replace("<DTE xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", "<DTE>");
            return XML;
        }

        private string FormatearFecha(string pFormatFecha)
        {
            DateTime Fecha;
            string strFecha = string.Empty;
            Fecha = Convert.ToDateTime(pFormatFecha);
            strFecha = Fecha.ToString("yyyy-MM-dd");
            return strFecha;
        }

        private void EnviarDte(string XML, string pTipoDte, string pRutEmisor, string p_comentario, SAPbobsCOM.Company oCompany)
        {
            try
            {
                SAPbobsCOM.Recordset oRecordsettxt = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                DoceleOLClient oProxy = new DoceleOLClient();
                XmlDocument xmlDoc = new XmlDocument();
                string rutEmisor = pRutEmisor;
                string bdComp = ObtenerDatos(oCompany);
                int tipoDTE = int.Parse(pTipoDte);
                string descripcionOperacion = string.Empty;
                long folioDTE;
                int response;

                oProxy.ClientCredentials.UserName.UserName = "0cc713a13";
                oProxy.ClientCredentials.UserName.Password = "mWUX8WQOfuexV9CLWaNFSA==";
                using (new OperationContextScope(oProxy.InnerChannel))
                {
                    // Add a HTTP Header to an outgoing request
                    string User = oProxy.ClientCredentials.UserName.UserName;
                    string Pass = oProxy.ClientCredentials.UserName.Password;
                    HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
                    requestMessage.Headers["facele.user"] = User;
                    requestMessage.Headers["facele.pass"] = Pass;
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;

                    response = oProxy.generaDTE(ref rutEmisor, ref tipoDTE, generaDTEFormato.XML, p_comentario, XML, null, out descripcionOperacion, out folioDTE);
                }

                WSresponse = response;
                WSdescripcion = descripcionOperacion;
                WsfolioDte = folioDTE;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void AgregarDatosDocumento(SAPbobsCOM.BoObjectTypes pDocumento, string pDocEntry, string pIndicador, SAPbobsCOM.Company oCompany)
        {
            try
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string strConsulta = string.Empty;

                //Consulta para vista de encabezado del documento
                strConsulta = "SELECT \"U_FolioRef\" FROM \"@SCGCE_CONF\" ";

                oRecordset.DoQuery(strConsulta);

                string CopiarFolio = oRecordset.Fields.Item("U_FolioRef").Value.ToString();

                if (pDocumento == BoObjectTypes.oStockTransfer)
                {
                    SAPbobsCOM.StockTransfer oDocumento;
                    oDocumento = (StockTransfer)oCompany.GetBusinessObject(pDocumento);

                    oDocumento.GetByKey(Convert.ToInt32(pDocEntry));

                    if (oDocumento != null)
                    {
                        oDocumento.UserFields.Fields.Item("U_SCGCE_EstadoAut").Value = WSresponse;
                        oDocumento.UserFields.Fields.Item("U_SCGCE_Des").Value = WSdescripcion;
                        oDocumento.FolioNumber = Convert.ToInt32(WsfolioDte);
                        oDocumento.FolioPrefixString = pIndicador;

                        oCompany.StartTransaction();
                        oDocumento.Update();

                        if (oCompany.InTransaction)
                        {
                            oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        }
                        else
                        {
                            oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        }
                    }
                }
                else
                {
                    SAPbobsCOM.Documents oDocumento;
                    oDocumento = (Documents)oCompany.GetBusinessObject(pDocumento);

                    oDocumento.GetByKey(Convert.ToInt32(pDocEntry));

                    if (oDocumento != null)
                    {
                        oDocumento.UserFields.Fields.Item("U_SCGCE_EstadoAut").Value = WSresponse;

                        if (string.IsNullOrEmpty(WSdescripcion))
                        {
                            oDocumento.UserFields.Fields.Item("U_SCGCE_Des").Value = "Error -1: Se produjo un error al enviar el documento";

                        }
                        else
                        {
                            oDocumento.UserFields.Fields.Item("U_SCGCE_Des").Value = WSdescripcion;
                        }
                        oDocumento.FolioNumber = Convert.ToInt32(WsfolioDte);
                        oDocumento.FolioPrefixString = pIndicador;
                        if (CopiarFolio == "Y")
                        {
                            oDocumento.NumAtCard = WsfolioDte.ToString();
                        }

                        oCompany.StartTransaction();
                        oDocumento.Update();

                        if (oCompany.InTransaction)
                        {
                            oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        }
                        else
                        {
                            oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                //oForm.Freeze(false);
                throw ex;
            }

        }

        public bool EsDocumentoElectronico(string pIndicador, out string pTipoDoc, string pFormTypeEx, SAPbobsCOM.Company oCompany)
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
                        break;
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

        public string EnviarDocumentoElectronico(string pFormTypeEx, string Indicador, string Almacen, string pDocEntry, bool bit, int DocNum, SAPbobsCOM.Company oCompany)
        {
            try
            {
                string p_comentario = string.Empty;
                string XML = string.Empty;
                string tipoDocumento = string.Empty;
                string seccion = string.Empty;
                string TipoDte = string.Empty;
                SAPbobsCOM.BoObjectTypes ObjectType = new BoObjectTypes();

                if (EsDocumentoElectronico(Indicador, out tipoDoc, pFormTypeEx, oCompany)) //Se busca si ese documento esta configurado para CE
                {
                    switch (pFormTypeEx)
                    {
                        case "133"://Facturas Electrónica
                            {
                                DTE objFacturaElec = ConsultaFactura(pDocEntry, tipoDoc, out p_comentario, oCompany);
                                TipoDte = objFacturaElec.Documento.Encabezado.IdDoc.TipoDTE;
                                XML = GetXMLFromObject(objFacturaElec);
                                EnviarDte(XML, objFacturaElec.Documento.Encabezado.IdDoc.TipoDTE, objFacturaElec.Documento.Encabezado.Emisor.RUTEmisor, p_comentario, oCompany);
                                ObjectType = BoObjectTypes.oInvoices;
                                tipoDocumento = "Facturas Electrónica";
                                break;
                            }
                        case "65302"://Facturas Exenta
                            {
                                DTE objFacturaExen = ConsultaFacturaExenta(pDocEntry, tipoDoc, out p_comentario, oCompany);
                                TipoDte = objFacturaExen.Documento.Encabezado.IdDoc.TipoDTE;
                                XML = GetXMLFromObject(objFacturaExen);
                                EnviarDte(XML, objFacturaExen.Documento.Encabezado.IdDoc.TipoDTE, objFacturaExen.Documento.Encabezado.Emisor.RUTEmisor, p_comentario, oCompany);
                                ObjectType = BoObjectTypes.oInvoices;
                                tipoDocumento = "Facturas Exenta Electrónica";
                                break;
                            }
                        case "65304"://Boleta Electrónica
                            {
                                DTE objBoletaElec = ConsultaBoleta(pDocEntry, tipoDoc, out p_comentario, oCompany);
                                TipoDte = objBoletaElec.Documento.Encabezado.IdDoc.TipoDTE;
                                XML = GetXMLFromObject(objBoletaElec);
                                XML = XML.Replace("<RznSoc>", "<RznSocEmisor>");
                                XML = XML.Replace("</RznSoc>", "</RznSocEmisor>");
                                XML = XML.Replace("GiroEmis", "GiroEmisor");
                                EnviarDte(XML, objBoletaElec.Documento.Encabezado.IdDoc.TipoDTE, objBoletaElec.Documento.Encabezado.Emisor.RUTEmisor, p_comentario, oCompany);
                                ObjectType = BoObjectTypes.oInvoices;
                                tipoDocumento = "Boleta Electrónica";
                                break;
                            }
                        case "65305"://Boleta Exenta
                            {
                                DTE objBoletaExen = ConsultaBoletaExenta(pDocEntry, tipoDoc, out p_comentario, oCompany);
                                TipoDte = objBoletaExen.Documento.Encabezado.IdDoc.TipoDTE;
                                XML = GetXMLFromObject(objBoletaExen);
                                XML = XML.Replace("<RznSoc>", "<RznSocEmisor>");
                                XML = XML.Replace("</RznSoc>", "</RznSocEmisor>");
                                XML = XML.Replace("GiroEmis", "GiroEmisor");
                                EnviarDte(XML, objBoletaExen.Documento.Encabezado.IdDoc.TipoDTE, objBoletaExen.Documento.Encabezado.Emisor.RUTEmisor, p_comentario, oCompany); ;
                                ObjectType = BoObjectTypes.oInvoices;
                                tipoDocumento = "Boleta Exenta Electrónica";
                                break;
                            }
                        case "65303"://Nota Debito
                            {
                                DTE objNotaDebito;
                                if (Indicador == "11")
                                {
                                    objNotaDebito = ConsultaNotaDebitoExp(pDocEntry, tipoDoc, out p_comentario, oCompany);
                                    TipoDte = objNotaDebito.Exportaciones.Encabezado.IdDoc.TipoDTE;
                                    XML = GetXMLFromObject(objNotaDebito);
                                    EnviarDte(XML, objNotaDebito.Exportaciones.Encabezado.IdDoc.TipoDTE, objNotaDebito.Exportaciones.Encabezado.Emisor.RUTEmisor, p_comentario, oCompany);
                                }
                                else
                                {
                                    objNotaDebito = ConsultaNotaDebito(pDocEntry, tipoDoc, out p_comentario, oCompany);
                                    TipoDte = objNotaDebito.Documento.Encabezado.IdDoc.TipoDTE;
                                    XML = GetXMLFromObject(objNotaDebito);
                                    EnviarDte(XML, objNotaDebito.Documento.Encabezado.IdDoc.TipoDTE, objNotaDebito.Documento.Encabezado.Emisor.RUTEmisor, p_comentario, oCompany);
                                }

                                ObjectType = BoObjectTypes.oInvoices;
                                tipoDocumento = "Nota Debito Electrónica";
                                break;
                            }
                        case "179"://Nota Credito
                            {
                                DTE objNotaCredito;
                                if (Indicador == "12")
                                {
                                    objNotaCredito = ConsultaNotaCreditoExp(pDocEntry, tipoDoc, out p_comentario, oCompany);
                                    TipoDte = objNotaCredito.Exportaciones.Encabezado.IdDoc.TipoDTE;
                                    XML = GetXMLFromObject(objNotaCredito);
                                    EnviarDte(XML, objNotaCredito.Exportaciones.Encabezado.IdDoc.TipoDTE, objNotaCredito.Exportaciones.Encabezado.Emisor.RUTEmisor, p_comentario, oCompany);
                                }
                                else
                                {
                                    objNotaCredito = ConsultaNotaCredito(pDocEntry, tipoDoc, out p_comentario, oCompany);
                                    TipoDte = objNotaCredito.Documento.Encabezado.IdDoc.TipoDTE;
                                    XML = GetXMLFromObject(objNotaCredito);
                                    EnviarDte(XML, objNotaCredito.Documento.Encabezado.IdDoc.TipoDTE, objNotaCredito.Documento.Encabezado.Emisor.RUTEmisor, p_comentario, oCompany);
                                }

                                ObjectType = BoObjectTypes.oCreditNotes;
                                tipoDocumento = "Nota Credito Electrónica";
                                break;
                            }

                        case "141"://Factura Compra
                            {
                                DTE objFacturaCompra = ConsultaFacturaCompra(pDocEntry, tipoDoc, out p_comentario, oCompany);
                                TipoDte = objFacturaCompra.Documento.Encabezado.IdDoc.TipoDTE;
                                XML = GetXMLFromObject(objFacturaCompra);
                                EnviarDte(XML, objFacturaCompra.Documento.Encabezado.IdDoc.TipoDTE, objFacturaCompra.Documento.Encabezado.Emisor.RUTEmisor, p_comentario, oCompany);
                                ObjectType = BoObjectTypes.oPurchaseInvoices;
                                tipoDocumento = "Factura Compra Electrónica";
                                break;
                            }
                        case "940"://Guia Despacho
                            {
                                DTE objGuiaDespacho = ConsultaGuiaDespacho(pDocEntry, tipoDoc, Almacen, out p_comentario, oCompany);
                                TipoDte = objGuiaDespacho.Documento.Encabezado.IdDoc.TipoDTE;
                                XML = GetXMLFromObject(objGuiaDespacho);
                                EnviarDte(XML, objGuiaDespacho.Documento.Encabezado.IdDoc.TipoDTE, objGuiaDespacho.Documento.Encabezado.Emisor.RUTEmisor, p_comentario, oCompany);
                                ObjectType = BoObjectTypes.oStockTransfer;
                                tipoDocumento = "Guia Despacho Electrónica";
                                break;
                            }
                        case "140"://Guia Despacho Entrega
                            {
                                DTE objGuiaDespachoEntrega = ConsultaGuiaDespachoEntrega(pDocEntry, tipoDoc, out p_comentario, oCompany);
                                TipoDte = objGuiaDespachoEntrega.Documento.Encabezado.IdDoc.TipoDTE;
                                XML = GetXMLFromObject(objGuiaDespachoEntrega);
                                EnviarDte(XML, objGuiaDespachoEntrega.Documento.Encabezado.IdDoc.TipoDTE, objGuiaDespachoEntrega.Documento.Encabezado.Emisor.RUTEmisor, p_comentario, oCompany);
                                ObjectType = BoObjectTypes.oDeliveryNotes;
                                tipoDocumento = "Guia Despacho Electrónica";
                                break;
                            }
                        case "182"://Guia Despacho Devolución
                            {
                                DTE objGuiaDespachoDevolucion = ConsultaGuiaDespachoDevolucion(pDocEntry, tipoDoc, out p_comentario, oCompany);
                                TipoDte = objGuiaDespachoDevolucion.Documento.Encabezado.IdDoc.TipoDTE;
                                XML = GetXMLFromObject(objGuiaDespachoDevolucion);
                                EnviarDte(XML, objGuiaDespachoDevolucion.Documento.Encabezado.IdDoc.TipoDTE, objGuiaDespachoDevolucion.Documento.Encabezado.Emisor.RUTEmisor, p_comentario, oCompany);
                                ObjectType = BoObjectTypes.oPurchaseReturns;
                                tipoDocumento = "Guia Despacho Electrónica";
                                break;
                            }
                        case "60091"://Facturas Reserva Electrónica
                            {
                                DTE objFacturaElec = ConsultaFactura(pDocEntry, tipoDoc, out p_comentario, oCompany);
                                TipoDte = objFacturaElec.Documento.Encabezado.IdDoc.TipoDTE;
                                XML = GetXMLFromObject(objFacturaElec);
                                EnviarDte(XML, objFacturaElec.Documento.Encabezado.IdDoc.TipoDTE, objFacturaElec.Documento.Encabezado.Emisor.RUTEmisor, p_comentario, oCompany);
                                ObjectType = BoObjectTypes.oInvoices;
                                tipoDocumento = "Facturas Electrónica";
                                break;
                            }
                        case "65300"://Facturas Anticipo Electrónica
                            {
                                DTE objFacturaElec = ConsultaFacturaAnticipo(pDocEntry, tipoDoc, out p_comentario, oCompany);
                                TipoDte = objFacturaElec.Documento.Encabezado.IdDoc.TipoDTE;
                                XML = GetXMLFromObject(objFacturaElec);
                                EnviarDte(XML, objFacturaElec.Documento.Encabezado.IdDoc.TipoDTE, objFacturaElec.Documento.Encabezado.Emisor.RUTEmisor, p_comentario, oCompany);
                                ObjectType = BoObjectTypes.oDownPayments;
                                tipoDocumento = "Facturas Anticipo Electrónica";
                                break;
                            }
                        case "65307"://Facturas de exportación Electrónica 
                            {
                                DTE objFacturaElec = ConsultaFacturaExport(pDocEntry, tipoDoc, out p_comentario, oCompany);
                                TipoDte = objFacturaElec.Exportaciones.Encabezado.IdDoc.TipoDTE;
                                XML = GetXMLFromObject(objFacturaElec);
                                EnviarDte(XML, objFacturaElec.Exportaciones.Encabezado.IdDoc.TipoDTE, objFacturaElec.Exportaciones.Encabezado.Emisor.RUTEmisor, p_comentario, oCompany);
                                ObjectType = BoObjectTypes.oInvoices;
                                tipoDocumento = "Facturas Exportación Electrónica";
                                break;
                            }
                    }

                    if (WSresponse == 1)
                    {
                        AgregarDatosDocumento(ObjectType, pDocEntry, Indicador, oCompany);
                        string NombrePDF = ObtenerNombrePDF(oCompany);
                        int intTipoDTE = int.Parse(TipoDte);
                        int intFolio = int.Parse(WsfolioDte.ToString());
                        GuardarXMLError(XML, tipoDocumento, DocNum.ToString(), true, oCompany);
                        OptieneDte(pFormTypeEx, intTipoDTE, intFolio, SCGCE.BL.facele.obtieneDTEFormato.PDF, oCompany, tipoDocumento, bit, NombrePDF, ObjectType, pDocEntry);
                        return "OK";
                    }
                    else
                    {
                        AgregarDatosDocumento(ObjectType, pDocEntry, Indicador, oCompany);
                        GuardarXMLError(XML, tipoDocumento, DocNum.ToString(), false, oCompany);
                        return "Error";
                    }
                }
                else
                {
                    return "No Doc";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string ObtenerNombrePDF(SAPbobsCOM.Company oCompany)
        {
            SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            string strConsulta = string.Empty;
            string NombrePDF = null;
            //Consulta para vista de encabezado del documento
            strConsulta = "SELECT \"U_RenomPDF\", \"U_NomUDFPDF\" FROM \"@SCGCE_CONF\" ";

            oRecordset.DoQuery(strConsulta);

            if (oRecordset.Fields.Item("U_RenomPDF").Value.ToString() == "Y")
            {
                NombrePDF = oRecordset.Fields.Item("U_NomUDFPDF").Value.ToString();

            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordset);
            return NombrePDF;
        }

        private void GuardarXMLError(string xML, string tipoDocumento, string DocNum, bool error, SAPbobsCOM.Company oCompany)
        {
            SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            string strConsulta = string.Empty;

            //Consulta para vista de encabezado del documento
            strConsulta = "SELECT \"U_RutaDTE\" FROM \"@SCGCE_CONF\" ";

            oRecordset.DoQuery(strConsulta);

            this.RutaDTE = oRecordset.Fields.Item("U_RutaDTE").Value.ToString();

            string rutaDocumentos = string.IsNullOrEmpty(this.pathDocumentos) ? (string.IsNullOrEmpty(RutaDTE) ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString() : RutaDTE) : this.pathDocumentos;
            //string rutaDocumentos = string.IsNullOrEmpty(this.pathDocumentos) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString() : this.pathDocumentos;
            //string RutaLocal = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordset);
            string path = string.Empty;
            string nombreDoc = string.Empty;
            if (error)
            {
                path = rutaDocumentos + "\\XML\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                nombreDoc = tipoDocumento + "Documento número" + DocNum + ".txt";
            }
            else
            {
                path = rutaDocumentos + "\\XML_Errores\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                nombreDoc = tipoDocumento + "Fallo el documento número" + DocNum + ".txt";
            }
            File.WriteAllText(path + nombreDoc, xML);
        }

        public void OptieneDte(string pFormTypeEx, int TipoDte, int Folio, SCGCE.BL.facele.obtieneDTEFormato TipoDoc, SAPbobsCOM.Company oCompany, string tipoDocumento, bool bit, string pNombrePDF, SAPbobsCOM.BoObjectTypes pDocumento, string pDocEntry)
        {
            try
            {
                SAPbobsCOM.Recordset oRecordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string strConsulta = string.Empty;
                string Select = string.Empty;
                string NombrePDF = string.Empty;
                bool cedibles = false;
                int copias = 0;


                //Consulta para vista de encabezado del documento
                strConsulta = "SELECT \"U_IdNum\",\"U_RutaDTE\",\"U_cedibles\",\"U_CopiasCed\" FROM \"@SCGCE_CONF\" ";
                //strConsulta = "SELECT \"U_IdNum\",\"U_RutaDTE\" FROM \"@SCGCE_CONF\" ";

                oRecordset.DoQuery(strConsulta);

                this.RutaDTE = oRecordset.Fields.Item("U_RutaDTE").Value.ToString();
                string RutEmisor = oRecordset.Fields.Item("U_IdNum").Value.ToString();


                //Consulta el UDF para renombrar el PDF
                if (pDocumento == BoObjectTypes.oStockTransfer)
                {
                    SAPbobsCOM.StockTransfer oDocumento;
                    oDocumento = (StockTransfer)oCompany.GetBusinessObject(pDocumento);

                    oDocumento.GetByKey(Convert.ToInt32(pDocEntry));
                    if (!string.IsNullOrEmpty(pNombrePDF))
                    {
                        NombrePDF = oDocumento.UserFields.Fields.Item(pNombrePDF).Value.ToString();
                    }

                }
                else
                {
                    SAPbobsCOM.Documents oDocumento;
                    oDocumento = (Documents)oCompany.GetBusinessObject(pDocumento);

                    oDocumento.GetByKey(Convert.ToInt32(pDocEntry));
                    if (!string.IsNullOrEmpty(pNombrePDF))
                    {
                        NombrePDF = oDocumento.UserFields.Fields.Item(pNombrePDF).Value.ToString();
                    }
                }

                SCGCE.BL.facele.DoceleOLClient oProxy = new DoceleOLClient();

                if (TipoDte == 10 || TipoDte == 11 || TipoDte == 12)
                {
                    TipoDte = TipoDte + 100;
                }

                if (TipoDte == 33 || TipoDte == 34 || TipoDte == 52 || TipoDte == 46)
                {
                    if (oRecordset.Fields.Item("U_cedibles").Value.ToString() == "Y")
                    {
                        cedibles = true;
                        copias = int.Parse(oRecordset.Fields.Item("U_CopiasCed").Value.ToString());
                    }
                }


                System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordset);
                //string RutaLocal = rutaDocumentos;
                //string tipoDocumento = string.Empty;
                //int TipoDte;
                //long Folio;
                int response;
                int responseXML;

                string Descripcion = string.Empty;
                string DescripcionXML = string.Empty;
                string XML = string.Empty;
                byte[] pdfResponseByte;
                byte[] XMLResponse;
                string pdfResponseStr = string.Empty;
                string outUrl = string.Empty;
                string outUrlXML = string.Empty;
                string rutaDocumentos = string.IsNullOrEmpty(this.pathDocumentos) ? (string.IsNullOrEmpty(RutaDTE) ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString() : RutaDTE) : this.pathDocumentos;


                oProxy.ClientCredentials.UserName.UserName = "0cc713a13";
                oProxy.ClientCredentials.UserName.Password = "mWUX8WQOfuexV9CLWaNFSA==";
                using (new OperationContextScope(oProxy.InnerChannel))
                {
                    // Add a HTTP Header to an outgoing request
                    string User = oProxy.ClientCredentials.UserName.UserName;
                    string Pass = oProxy.ClientCredentials.UserName.Password;
                    HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
                    requestMessage.Headers["facele.user"] = User;
                    requestMessage.Headers["facele.pass"] = Pass;
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
                    response = oProxy.obtieneDTE(RutEmisor, TipoDte, Folio, TipoDoc, cedibles, copias, out Descripcion, out XML, out pdfResponseByte, out outUrl);
                    responseXML = oProxy.obtieneDTE(RutEmisor, TipoDte, Folio, SCGCE.BL.facele.obtieneDTEFormato.XML, false, 0, out DescripcionXML, out XML, out XMLResponse, out outUrlXML);
                }

                if (response == 0)
                {
                    throw new Exception("Error al obtener el DTE: " + Descripcion);
                }
                else
                {
                    pdfResponseStr = Encoding.UTF8.GetString(pdfResponseByte);
                    byte[] ByteFinalDoc = Convert.FromBase64String(pdfResponseStr);
                    string path = string.Empty;

                    path = rutaDocumentos + "\\DocumentosElectronicos\\";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string nombreDoc = string.Empty;
                    string nombreXML = string.Empty;
                    if (string.IsNullOrEmpty(NombrePDF))
                    {
                        nombreDoc = tipoDocumento + " Folio " + Folio + ".pdf";
                        nombreXML = tipoDocumento + " Folio " + Folio + ".xml";
                    }
                    else
                    {
                        nombreDoc = NombrePDF + ".pdf";
                        nombreXML = NombrePDF + ".xml";
                    }

                    File.WriteAllBytes(path + nombreDoc, ByteFinalDoc);
                    File.WriteAllText(path + nombreXML, XML);
                    if (bit)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo(path + nombreDoc);
                        Process.Start(startInfo);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string ObtenerDatos(SAPbobsCOM.Company oCompany)
        {
            //String strRuta = "C:\\Users\\desarrollo_12\\Desktop\\";
            string comp = oCompany.CompanyDB.ToString();
            Regex reg = new Regex("[*'\",_&#^@]");
            comp = reg.Replace(comp, string.Empty);
            comp = comp.ToLower();

            return comp;
        }

        public string ObtenerFormTypeEx(string pTipoDoc, string pDes)
        {
            string FormTypeEx = string.Empty;
            switch (pTipoDoc)
            {
                case "01"://Facturas Electrónica
                    {
                        if (pDes == "Factura Electrónica")
                        {
                            FormTypeEx = "133";
                        }
                        else if (pDes == "Factura Reserva Electrónica")
                        {
                            FormTypeEx = "60091";
                        }
                        else if (pDes == "Factura Anticipo Electrónica")
                        {
                            FormTypeEx = "65300";
                        }
                        else if (pDes == "Factura Exportación")
                        {
                            FormTypeEx = "65307";
                        }
                        break;
                    }
                case "02"://Facturas Exenta
                    {
                        FormTypeEx = "65302";
                        break;
                    }
                case "03"://Boleta Electrónica
                    {
                        FormTypeEx = "65304";
                        break;
                    }
                case "04"://Boleta Exenta
                    {
                        FormTypeEx = "65305";
                        break;
                    }
                case "06"://Nota Debito
                    {
                        FormTypeEx = "65303";
                        break;
                    }
                case "07"://Nota Credito
                    {
                        FormTypeEx = "179";
                        break;
                    }

                case "05"://Factura Compra
                    {
                        FormTypeEx = "141";
                        break;
                    }
                case "08"://Guia Despacho
                    {
                        if (pDes == "Guía Despacho Electrónica TS")
                        {
                            FormTypeEx = "940";
                        }
                        else if (pDes == "Guía Despacho Electrónica EN")
                        {
                            FormTypeEx = "140";
                        }
                        else if (pDes == "Guía Despacho Electrónica DM")
                        {
                            FormTypeEx = "182";
                        }
                        break;
                    }
            }

            return FormTypeEx;
        }

        #endregion
    }
}