using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using RawPrint;

namespace HCO.Wizard.Printer
{
    public class PrinterHelper
    {
        public static void PrintPDFWithAcrobat(string filePath)
        {

            using (PrintDialog Dialog = new PrintDialog())
            {
                Dialog.ShowDialog();

                ProcessStartInfo printProcessInfo = new ProcessStartInfo()
                {
                    Verb = "print",
                    CreateNoWindow = true,
                    FileName = filePath,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                Process printProcess = new Process();
                printProcess.StartInfo = printProcessInfo;
                printProcess.Start();

                printProcess.WaitForInputIdle();

                Thread.Sleep(3000);

                if (false == printProcess.CloseMainWindow())
                {
                    printProcess.Kill();
                }
            }
        }

        public static void PrintRawFile (string printerName, string filepath, string filename)
        {
            IPrinter printer = new RawPrint.Printer();

            // Print the file
            printer.PrintRawFile(printerName, filepath, filename);
        }
    }
}
