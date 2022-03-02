using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbobsCOM;

namespace HCO.Wizard.BL
{

    public class DIAPIHelper
    {
        public static Company Sb1Company { get; set; }

        public static string Sb1CompanyName { get; set; }

        public static void Connect(string server, string dbServerType, string companyDB,
            string licenseServer, string dbUserName, string dbPassword, string userName, string password)
        {

            if (Sb1Company == null || (Sb1Company != null && !Sb1Company.Connected))
            {
                Sb1Company = new Company();
                Sb1Company.Server = server;
                Sb1Company.DbServerType = GetSboDataServerType(dbServerType);
                Sb1Company.CompanyDB = companyDB;
                Sb1Company.SLDServer = licenseServer;
                Sb1Company.language = BoSuppLangs.ln_Spanish_La;

                if (!string.IsNullOrEmpty(dbUserName) && !string.IsNullOrEmpty(dbPassword))
                {
                    Sb1Company.DbUserName = dbUserName;
                    Sb1Company.DbPassword = dbPassword;
                    Sb1Company.UseTrusted = false;
                }

                Sb1Company.UserName = userName;
                Sb1Company.Password = password;

                int result = Sb1Company.Connect();

                if (result != 0)
                    throw new Exception("Error : "  + Sb1Company.GetLastErrorCode() + " (" + Sb1Company.GetLastErrorDescription() + ")");

                Sb1CompanyName = Sb1Company.CompanyName;

            }

        }

        /// <summary>
        /// Obtiene el tipo de servidor de base de datos de SAP en base a un string
        /// </summary>
        /// <param name="strServerType">String con el tipo de servidor</param>
        /// <returns></returns>
        public static BoDataServerTypes GetSboDataServerType(string strServerType)
        {

            BoDataServerTypes serverType = (BoDataServerTypes)Enum.Parse(typeof(BoDataServerTypes), strServerType);

            return serverType;
        }

        public static void Disconnect()
        {

            if (Sb1Company != null && Sb1Company.Connected)
            {
                Sb1Company.Disconnect();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(Sb1Company);

                Sb1Company = null;
            }
        }

    }
}
