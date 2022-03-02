using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCO.Wizard.DTO;
using SAPbobsCOM;

namespace HCO.Wizard.BL
{
    public class OrderBL
    {

        public List<OrderDTO> GetOrders (DateTime? taxDateFrom, DateTime? taxDateTo, DateTime? docDueDateFrom, DateTime? docDueDateTo, string groupNum, string slpCode, string groupCode, string deliveryType) 
        {
			string filterTaxDate =  taxDateFrom.HasValue ? " AND T0.\"TaxDate\" BETWEEN '" + taxDateFrom.Value.ToString("yyyyMMdd") + "' AND '" + taxDateTo.Value.ToString("yyyyMMdd") + "'" : string.Empty;
			string filterDocDueDate = docDueDateFrom.HasValue ? " AND T0.\"DocDueDate\" BETWEEN '" + docDueDateFrom.Value.ToString("yyyyMMdd") + "' AND '" + docDueDateTo.Value.ToString("yyyyMMdd") + "'" : string.Empty;
			string filterGroupNum = !string.IsNullOrEmpty(groupNum) ? " AND T0.\"U_SCGCE_CondVent\" = '" + groupNum + "'" : string.Empty;
			string filterSlpCode = !string.IsNullOrEmpty(slpCode) ? " AND T0.\"SlpCode\" = " + slpCode : string.Empty;
			string filterGroupCode = !string.IsNullOrEmpty(groupCode) ? " AND T2.\"GroupCode\" = " + groupCode : string.Empty;
			string filterDeliveryType = !string.IsNullOrEmpty(deliveryType) ? " AND T0.\"U_TDespacho\" = '" + deliveryType + "'" : string.Empty;

			string filter = filterTaxDate + filterDocDueDate + filterGroupNum + filterSlpCode + filterGroupCode + filterDeliveryType;

			filter = " WHERE T0.\"DocStatus\" = 'O' AND T1.\"LineStatus\" = 'O' " + filter;

			List<OrderDTO> orders = new List<OrderDTO>();
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
					"	   T1.\"LineNum\", " +
					"	   T1.\"ItemCode\", " +
					"	   T1.\"Dscription\", " +
					"	   T1.\"LineStatus\", " +
					"	   T1.\"Quantity\", " +
					"	   T1.\"OpenQty\"  " +
					"FROM ORDR T0 INNER JOIN RDR1 T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" " +
					"INNER JOIN OCRD T2 ON T0.\"CardCode\" = T2.\"CardCode\" " +
					"INNER JOIN OCTG T3 ON T0.\"GroupNum\" = T3.\"GroupNum\" " +
					"INNER JOIN OSLP T4 ON T0.\"SlpCode\" = T4.\"SlpCode\" " +
					"INNER JOIN OCRG T5 ON T2.\"GroupCode\" = T5.\"GroupCode\" " + filter;

			Recordset recordset = (Recordset)DIAPIHelper.Sb1Company.GetBusinessObject(BoObjectTypes.BoRecordset);

			recordset.DoQuery(query);


			while (!recordset.EoF)
            {
				OrderDTO orderDTO = new OrderDTO();

				orderDTO.DocEntry = (int)recordset.Fields.Item("DocEntry").Value;
				orderDTO.DocNum = (int)recordset.Fields.Item("DocNum").Value;
				orderDTO.DocStatus = recordset.Fields.Item("DocStatus").Value.ToString();
				orderDTO.TaxDate = (DateTime)recordset.Fields.Item("TaxDate").Value;
				orderDTO.DocDueDate = (DateTime)recordset.Fields.Item("DocDueDate").Value;
				orderDTO.LicTradNum = recordset.Fields.Item("LicTradNum").Value.ToString();
				orderDTO.CardCode = recordset.Fields.Item("CardCode").Value.ToString();
				orderDTO.CardName = recordset.Fields.Item("CardName").Value.ToString();
				orderDTO.GroupCode = (int)recordset.Fields.Item("GroupCode").Value;
				orderDTO.GroupName = recordset.Fields.Item("GroupName").Value.ToString();
				orderDTO.GroupNum = (int)recordset.Fields.Item("GroupNum").Value;
				orderDTO.PymntGroup = recordset.Fields.Item("PymntGroup").Value.ToString();
				orderDTO.SlpCode = (int)recordset.Fields.Item("SlpCode").Value;
				orderDTO.SlpName = recordset.Fields.Item("SlpName").Value.ToString();
				orderDTO.LineNum = (int)recordset.Fields.Item("LineNum").Value;
				orderDTO.ItemCode = recordset.Fields.Item("ItemCode").Value.ToString();
				orderDTO.Dscription = recordset.Fields.Item("Dscription").Value.ToString();
				orderDTO.LineStatus = recordset.Fields.Item("LineStatus").Value.ToString();
				orderDTO.Quantity = (double)recordset.Fields.Item("Quantity").Value;
				orderDTO.OpenQty = (double)recordset.Fields.Item("OpenQty").Value;
				orderDTO.Selected = false;
				 

				orders.Add(orderDTO);

				recordset.MoveNext();
			}

			System.Runtime.InteropServices.Marshal.ReleaseComObject(recordset);
			recordset = null;

			return orders;

		}

    }
}
