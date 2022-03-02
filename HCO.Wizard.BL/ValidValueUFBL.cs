using HCO.Wizard.DTO;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCO.Wizard.BL
{
    public class ValidValueUFBL
    {
        public List<ValidValueUFDTO> GetValidValues(string aliasID, string tableID)
        {
            if (DIAPIHelper.Sb1Company == null)
                return null;

            List<ValidValueUFDTO> validValues = new List<ValidValueUFDTO>();

            string query = "SELECT T1.\"FldValue\", T1.\"Descr\" " +
                "FROM CUFD T0 " +
                "INNER JOIN UFD1 T1 ON T0.\"TableID\" = T1.\"TableID\" AND T0.\"FieldID\" = T1.\"FieldID\" " +
                "WHERE T0.\"AliasID\" = '" + aliasID + "' AND T0.\"TableID\" = '" + tableID + "' ";

            Recordset recordset = (Recordset)DIAPIHelper.Sb1Company.GetBusinessObject(BoObjectTypes.BoRecordset);

            recordset.DoQuery(query);

            while (!recordset.EoF)
            {
                ValidValueUFDTO validValue = new ValidValueUFDTO();

                validValue.FldValue = recordset.Fields.Item("FldValue").Value.ToString();
                validValue.Descr = recordset.Fields.Item("Descr").Value.ToString();


                validValues.Add(validValue);

                recordset.MoveNext();
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(recordset);
            recordset = null;

            return validValues;

        }
    }
}
