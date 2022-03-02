namespace HCO.Wizard.Forms.Panels
{
    partial class PnlWelcome
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
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.rbtCreateDocuments = new System.Windows.Forms.RadioButton();
            this.rbtPrintDocuments = new System.Windows.Forms.RadioButton();
            this.pnlFondo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlFondo
            // 
            this.pnlFondo.Controls.Add(this.rbtPrintDocuments);
            this.pnlFondo.Controls.Add(this.rbtCreateDocuments);
            this.pnlFondo.Controls.Add(this.pictureBox1);
            this.pnlFondo.Controls.Add(this.label2);
            this.pnlFondo.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(20, 54);
            this.label2.Name = "label2";
            this.controlsValidator.SetOrden(this.label2, 0);
            this.controlsValidator.SetPuedeSerVacio(this.label2, false);
            this.controlsValidator.SetRequerido(this.label2, false);
            this.label2.Size = new System.Drawing.Size(387, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "El asistente de facturación le permite crear facturas en base a pedidos abiertos";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::HCO.Wizard.Properties.Resources.Documents;
            this.pictureBox1.Location = new System.Drawing.Point(429, 54);
            this.pictureBox1.Name = "pictureBox1";
            this.controlsValidator.SetOrden(this.pictureBox1, 0);
            this.controlsValidator.SetPuedeSerVacio(this.pictureBox1, false);
            this.controlsValidator.SetRequerido(this.pictureBox1, false);
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // rbtCreateDocuments
            // 
            this.rbtCreateDocuments.AutoSize = true;
            this.rbtCreateDocuments.Checked = true;
            this.rbtCreateDocuments.Location = new System.Drawing.Point(121, 169);
            this.rbtCreateDocuments.Name = "rbtCreateDocuments";
            this.controlsValidator.SetOrden(this.rbtCreateDocuments, 0);
            this.controlsValidator.SetPuedeSerVacio(this.rbtCreateDocuments, false);
            this.controlsValidator.SetRequerido(this.rbtCreateDocuments, false);
            this.rbtCreateDocuments.Size = new System.Drawing.Size(177, 17);
            this.rbtCreateDocuments.TabIndex = 10;
            this.rbtCreateDocuments.TabStop = true;
            this.rbtCreateDocuments.Tag = "9999";
            this.rbtCreateDocuments.Text = "Generar documentos en SAP B1";
            this.rbtCreateDocuments.UseVisualStyleBackColor = true;
            // 
            // rbtPrintDocuments
            // 
            this.rbtPrintDocuments.AutoSize = true;
            this.rbtPrintDocuments.Location = new System.Drawing.Point(121, 202);
            this.rbtPrintDocuments.Name = "rbtPrintDocuments";
            this.controlsValidator.SetOrden(this.rbtPrintDocuments, 0);
            this.controlsValidator.SetPuedeSerVacio(this.rbtPrintDocuments, false);
            this.controlsValidator.SetRequerido(this.rbtPrintDocuments, false);
            this.rbtPrintDocuments.Size = new System.Drawing.Size(177, 17);
            this.rbtPrintDocuments.TabIndex = 11;
            this.rbtPrintDocuments.Tag = "9999";
            this.rbtPrintDocuments.Text = "Imprimir documentos existentes";
            this.rbtPrintDocuments.UseVisualStyleBackColor = true;
            // 
            // PnlWelcome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "PnlWelcome";
            this.controlsValidator.SetOrden(this, 0);
            this.controlsValidator.SetPuedeSerVacio(this, false);
            this.controlsValidator.SetRequerido(this, false);
            this.pnlFondo.ResumeLayout(false);
            this.pnlFondo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.RadioButton rbtPrintDocuments;
        public System.Windows.Forms.RadioButton rbtCreateDocuments;
    }
}
