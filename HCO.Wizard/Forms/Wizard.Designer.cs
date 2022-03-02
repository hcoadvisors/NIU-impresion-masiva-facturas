namespace HCO.Wizard.Forms
{
    partial class Wizard
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Wizard));
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.pnlContenedor = new System.Windows.Forms.Panel();
            this.stepProgressBar = new HCO.WizardStepProgressBar.StepProgressBar();
            this.skinEngine = new Sunisoft.IrisSkin.SkinEngine();
            this.picBarra = new System.Windows.Forms.PictureBox();
            this.picPie = new System.Windows.Forms.PictureBox();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.bgWPrint = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.picBarra)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPie)).BeginInit();
            this.SuspendLayout();
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(791, 606);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 21);
            this.btnNext.TabIndex = 10;
            this.btnNext.Text = "Siguiente>";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrevious.Location = new System.Drawing.Point(710, 606);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(75, 21);
            this.btnPrevious.TabIndex = 11;
            this.btnPrevious.Text = "<Atrás";
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Visible = false;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // pnlContenedor
            // 
            this.pnlContenedor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlContenedor.Location = new System.Drawing.Point(0, 78);
            this.pnlContenedor.Name = "pnlContenedor";
            this.pnlContenedor.Size = new System.Drawing.Size(874, 517);
            this.pnlContenedor.TabIndex = 12;
            // 
            // stepProgressBar
            // 
            this.stepProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stepProgressBar.BackColor = System.Drawing.Color.White;
            this.stepProgressBar.Location = new System.Drawing.Point(0, 7);
            this.stepProgressBar.Name = "stepProgressBar";
            this.stepProgressBar.Size = new System.Drawing.Size(874, 71);
            this.stepProgressBar.TabIndex = 13;
            this.stepProgressBar.Tag = "9999";
            // 
            // skinEngine
            // 
            this.skinEngine.@__DrawButtonFocusRectangle = true;
            this.skinEngine.DisabledButtonTextColor = System.Drawing.Color.Gray;
            this.skinEngine.DisabledMenuFontColor = System.Drawing.SystemColors.GrayText;
            this.skinEngine.InactiveCaptionColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.skinEngine.SerialNumber = "U8XxhWQ7f0vz2ZCQ0R/Zoar2JJsDYOIzWNjdiqqfm9x4rZSajGGoJQ==";
            this.skinEngine.SkinColorDialogs = false;
            this.skinEngine.SkinDialogs = false;
            this.skinEngine.SkinFile = "C:\\Users\\wflores\\Desktop\\HCO.Wizard\\skins\\sboFiori.ssk";
            this.skinEngine.SkinStreamMain = ((System.IO.Stream)(resources.GetObject("skinEngine.SkinStreamMain")));
            // 
            // picBarra
            // 
            this.picBarra.Dock = System.Windows.Forms.DockStyle.Top;
            this.picBarra.Location = new System.Drawing.Point(0, 0);
            this.picBarra.Name = "picBarra";
            this.picBarra.Size = new System.Drawing.Size(874, 6);
            this.picBarra.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBarra.TabIndex = 8;
            this.picBarra.TabStop = false;
            // 
            // picPie
            // 
            this.picPie.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.picPie.Location = new System.Drawing.Point(0, 597);
            this.picPie.Name = "picPie";
            this.picPie.Size = new System.Drawing.Size(874, 38);
            this.picPie.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPie.TabIndex = 9;
            this.picPie.TabStop = false;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // bgWPrint
            // 
            this.bgWPrint.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWPrint_DoWork);
            this.bgWPrint.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWPrint_RunWorkerCompleted);
            // 
            // Wizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 635);
            this.Controls.Add(this.picBarra);
            this.Controls.Add(this.stepProgressBar);
            this.Controls.Add(this.pnlContenedor);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPrevious);
            this.Controls.Add(this.picPie);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Wizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Asistente de creación de documentos";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Wizard_FormClosing);
            this.Load += new System.EventHandler(this.Wizard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picBarra)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPie)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picPie;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Panel pnlContenedor;
        private WizardStepProgressBar.StepProgressBar stepProgressBar;
        private System.Windows.Forms.PictureBox picBarra;
        private Sunisoft.IrisSkin.SkinEngine skinEngine;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.ComponentModel.BackgroundWorker bgWPrint;
    }
}