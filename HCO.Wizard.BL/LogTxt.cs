using System;
using System.IO;

namespace HCO.Wizard.BL
{
    class LogTxt
    {
        private string mensaje;

        public LogTxt(string mensaje)
        {
            this.mensaje = mensaje;
            escribirLog();
        }

        private void escribirLog()
        {
            string systemDir = "\\\\saphaniusushi\\SBO_SHARE" + @"\HCO Invoices Wizard";

            if (!Directory.Exists(systemDir))
                Directory.CreateDirectory(systemDir);
            string path = systemDir + "\\log.txt";
            File.AppendAllText(path, "\r\n" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " -> " + mensaje);
        }
    }
}
