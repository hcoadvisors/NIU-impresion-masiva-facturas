using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCO.Wizard.DTO;
using SAPbobsCOM;

namespace HCO.Wizard.BL
{
    public class IndicadorDocumentoBL
    {

        public List<IndicadorDocumentoDTO> GetIndicadoresDocumentos()
        {
            if (DIAPIHelper.Sb1Company == null)
                return null;

            List<IndicadorDocumentoDTO> indicadores = new List<IndicadorDocumentoDTO>();

            string query = "SELECT \"U_IdSerS\", \"U_Desc\", \"U_Term\" FROM \"@SCGCE_CONFS\" WHERE \"U_Term\" IN ('133','65302','65304','65305')";

            Recordset recordset = (Recordset)DIAPIHelper.Sb1Company.GetBusinessObject(BoObjectTypes.BoRecordset);

            recordset.DoQuery(query);


            while (!recordset.EoF)
            {
                IndicadorDocumentoDTO indicadorDocumentoDTO = new IndicadorDocumentoDTO();

                indicadorDocumentoDTO.Indicador = recordset.Fields.Item("U_IdSerS").Value.ToString();
                indicadorDocumentoDTO.Descripcion = recordset.Fields.Item("U_Desc").Value.ToString();
                indicadorDocumentoDTO.Formulario = recordset.Fields.Item("U_Term").Value.ToString();

                indicadores.Add(indicadorDocumentoDTO);

                recordset.MoveNext();
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(recordset);
            recordset = null;

            return indicadores;

        }

        public string GetIndicador (string formulario)
        {
            string query = "SELECT \"U_IdSerS\"  FROM \"@SCGCE_CONFS\" WHERE \"U_Term\" ='" + formulario + "'";

            Recordset recordset = (Recordset)DIAPIHelper.Sb1Company.GetBusinessObject(BoObjectTypes.BoRecordset);

            recordset.DoQuery(query);

            string indicador = recordset.Fields.Item("U_IdSerS").Value.ToString();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(recordset);
            recordset = null;

            return indicador;
        }

    }
}
