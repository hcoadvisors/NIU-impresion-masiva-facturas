using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCO.Wizard.DTO;
using SAPbobsCOM;

namespace HCO.Wizard.BL
{
    public class BusinessPartnerGroupBL
    {

        public List<BusinessPartnerGroupDTO> GetBusinessPartnerGroups()
        {
            List<BusinessPartnerGroupDTO> groups = new List<BusinessPartnerGroupDTO>();

            string query = "SELECT \"GroupCode\", \"GroupName\" FROM \"OCRG\" WHERE \"GroupType\" = 'C' ";

            Recordset recordset = (Recordset)DIAPIHelper.Sb1Company.GetBusinessObject(BoObjectTypes.BoRecordset);

            recordset.DoQuery(query);


            while (!recordset.EoF)
            {
                BusinessPartnerGroupDTO group = new BusinessPartnerGroupDTO();

                group.GroupCode = (int)recordset.Fields.Item("GroupCode").Value;
                group.GroupName = recordset.Fields.Item("GroupName").Value.ToString();


                groups.Add(group);

                recordset.MoveNext();
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(recordset);
            recordset = null;

            return groups;

        }
    }
}
