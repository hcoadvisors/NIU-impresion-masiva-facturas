using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HCO.Wizard
{
    class MessageUI
    {
        private const string appName = "Asistente de generación de documentos";

        /// <summary>
        /// Muestra un mensaje de operación exitosa
        /// </summary>
        public static void SuccessfulOperation()
        {
            MessageBox.Show("Operación finalizada con éxito.", appName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Muestra un mensaje de información
        /// </summary>
        /// <param name="message">Mensaje a mostrar</param>
        public static void Show(string message)
        {
            MessageBox.Show(message, appName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Muestra un mensaje de error
        /// </summary>
        /// <param name="message">Mensaje a mostrar</param>
        public static void Error(string message, Exception ex)
        {
            MessageBox.Show(message, appName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Muestra un mensaje de pregunta
        /// </summary>
        /// <param name="question">Pregunta a mostrar</param>
        /// <returns>Si o no según la elección del usuario</returns>
        public static DialogResult Question(string question)
        {
            return MessageBox.Show(question, appName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
    }
}
