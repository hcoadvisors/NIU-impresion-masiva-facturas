using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCGCE.BL.Objetos
{
    public class UserField
    {
        public string TableID { get; set; }
        public int FieldID { get; set; }
        public string AliasID { get; set; }
        public string Descr { get; set; }
        public int Tipo { get; set; }
        public int SubTipo { get; set; }
        public int SizeID { get; set; }
        public int EditSize { get; set; }
        public string Dflt { get; set; }
        public string NotNull { get; set; }
        public string IndexID { get; set; }
        public string RTable { get; set; }
        public string RField { get; set; }
        public string Action { get; set; }
        public string Sys { get; set; }
        public List<ValidValue> ValidValues { get; set; }
    }
}
