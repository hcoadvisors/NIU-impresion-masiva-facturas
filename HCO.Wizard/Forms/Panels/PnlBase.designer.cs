namespace HCO.Wizard.Forms.Panels
{
    partial class PnlBase
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.controlsValidator = new SCG.UX.Windows.SCGControlsValidator();
            this.pnlFondo = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // controlsValidator
            // 
            this.controlsValidator.ColorError = System.Drawing.Color.Beige;
            this.controlsValidator.Duracion = 5000;
            this.controlsValidator.ErrorSub = null;
            this.controlsValidator.FormularioPadre = null;
            // 
            // pnlFondo
            // 
            this.pnlFondo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFondo.Location = new System.Drawing.Point(0, 0);
            this.pnlFondo.Name = "pnlFondo";
            this.controlsValidator.SetOrden(this.pnlFondo, 0);
            this.controlsValidator.SetPuedeSerVacio(this.pnlFondo, false);
            this.controlsValidator.SetRequerido(this.pnlFondo, false);
            this.pnlFondo.Size = new System.Drawing.Size(874, 517);
            this.pnlFondo.TabIndex = 6;
            // 
            // PnlBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlFondo);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PnlBase";
            this.controlsValidator.SetOrden(this, 0);
            this.controlsValidator.SetPuedeSerVacio(this, false);
            this.controlsValidator.SetRequerido(this, false);
            this.Size = new System.Drawing.Size(874, 517);
            this.ResumeLayout(false);

        }

        #endregion
        public SCG.UX.Windows.SCGControlsValidator controlsValidator;
        public System.Windows.Forms.Panel pnlFondo;
    }
}
