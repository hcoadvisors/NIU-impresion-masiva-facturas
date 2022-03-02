using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCGCE.BL.Objetos
{
    public class UserTable
    {
        public string TableName { get; set; }

        public string Descr { get; set; }

        public int TblNum { get; set; }

        public int ObjectType { get; set; }

        public string UsedInObj { get; set; }

        public string LogTable { get; set; }
    }
}
