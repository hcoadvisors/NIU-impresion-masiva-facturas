using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCGCE.BL.Objetos
{
    public class UDO
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string TableName { get; set; }

        public string LogTable { get; set; }

        public int TYPE { get; set; }

        public string MngSeries { get; set; }

        public string CanDelete { get; set; }

        public string CanCancel { get; set; }

        public string ExtName { get; set; }

        public string CanFind { get; set; }

        public string CanYrTrnsf { get; set; }

        public string CanDefForm { get; set; }

        public string CanLog { get; set; }

        public string OvrWrtDll { get; set; }

        public List<ChildTable> ChildTables { get; set; }
    }
}
