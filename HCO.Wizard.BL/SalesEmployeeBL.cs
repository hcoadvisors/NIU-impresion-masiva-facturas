using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCO.Wizard.DTO;
using SAPbobsCOM;

namespace HCO.Wizard.BL
{
    public class SalesEmployeeBL
    {
        public List<SaleEmployeeDTO> GetSalesEmployees()
        {
            if (DIAPIHelper.Sb1Company == null)
                return null;

            List<SaleEmployeeDTO> salesEmployees = new List<SaleEmployeeDTO>();

            string query = "SELECT \"SlpCode\", \"SlpName\" FROM \"OSLP\" ";

            Recordset recordset = (Recordset)DIAPIHelper.Sb1Company.GetBusinessObject(BoObjectTypes.BoRecordset);

            recordset.DoQuery(query);


            while (!recordset.EoF)
            {
                SaleEmployeeDTO salesEmployee = new SaleEmployeeDTO();

                salesEmployee.SlpCode = (int)recordset.Fields.Item("SlpCode").Value;
                salesEmployee.SlpName = recordset.Fields.Item("SlpName").Value.ToString();


                salesEmployees.Add(salesEmployee);

                recordset.MoveNext();
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(recordset);
            recordset = null;

            return salesEmployees;

        }
    }
}
