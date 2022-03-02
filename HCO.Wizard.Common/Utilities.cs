using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCO.Wizard.DTO;

namespace HCO.Wizard.Common
{
    public class Utilities
    {

        public static string GetSystemPath ()
        {
            string systemDir = "\\\\saphaniusushi\\SBO_SHARE" + @"\HCO Invoices Wizard";

            if (!Directory.Exists(systemDir))
                Directory.CreateDirectory(systemDir);

            return systemDir;
        }

        public static void SaveConfigFile(Sb1ConnectionDTO sb1ConnectionDTO)
        {
            string filePath = GetSystemPath() + @"\settings.json";

            JSerializer.SerializeToFile(sb1ConnectionDTO, filePath);
        }

        public static Sb1ConnectionDTO GetConfigFile()
        {
            Sb1ConnectionDTO sb1ConnectionDTO = null;
            string filePath = GetSystemPath() + @"\settings.json";

            if (File.Exists(filePath))
                sb1ConnectionDTO = JSerializer.DeserializeFromFile<Sb1ConnectionDTO>(filePath);

            return sb1ConnectionDTO;
        }

    }
}
