
namespace HCO.WizardStepProgressBar
{
    partial class StepControl
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
            this.lblStepNumber = new System.Windows.Forms.Label();
            this.lblStepName = new System.Windows.Forms.Label();
            this.pnlLeftLine = new System.Windows.Forms.Panel();
            this.pnlRightLine = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // lblStepNumber
            // 
            this.lblStepNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(167)))), ((int)(((byte)(213)))));
            this.lblStepNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStepNumber.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStepNumber.ForeColor = System.Drawing.Color.White;
            this.lblStepNumber.Location = new System.Drawing.Point(64, 4);
            this.lblStepNumber.Name = "lblStepNumber";
            this.lblStepNumber.Size = new System.Drawing.Size(28, 25);
            this.lblStepNumber.TabIndex = 22;
            this.lblStepNumber.Text = "#";
            this.lblStepNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStepName
            // 
            this.lblStepName.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStepName.Location = new System.Drawing.Point(0, 28);
            this.lblStepName.Name = "lblStepName";
            this.lblStepName.Size = new System.Drawing.Size(157, 50);
            this.lblStepName.TabIndex = 21;
            this.lblStepName.Text = "Step Name";
            this.lblStepName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlLeftLine
            // 
            this.pnlLeftLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.pnlLeftLine.Location = new System.Drawing.Point(0, 14);
            this.pnlLeftLine.Name = "pnlLeftLine";
            this.pnlLeftLine.Size = new System.Drawing.Size(70, 6);
            this.pnlLeftLine.TabIndex = 23;
            // 
            // pnlRightLine
            // 
            this.pnlRightLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.pnlRightLine.Location = new System.Drawing.Point(87, 14);
            this.pnlRightLine.Name = "pnlRightLine";
            this.pnlRightLine.Size = new System.Drawing.Size(70, 6);
            this.pnlRightLine.TabIndex = 24;
            // 
            // StepControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblStepNumber);
            this.Controls.Add(this.pnlRightLine);
            this.Controls.Add(this.pnlLeftLine);
            this.Controls.Add(this.lblStepName);
            this.Name = "StepControl";
            this.Size = new System.Drawing.Size(157, 78);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnlLeftLine;
        private System.Windows.Forms.Label lblStepNumber;
        private System.Windows.Forms.Label lblStepName;
        private System.Windows.Forms.Panel pnlRightLine;
    }
}
