using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HCO.WizardStepProgressBar
{
    public partial class StepControl : UserControl
    {

        public bool ShowLeftLine
        {
            get { return pnlLeftLine.Visible; }
            set { pnlLeftLine.Visible = value;  }
        }

        public bool ShowRightLine
        {
            get { return pnlRightLine.Visible; }
            set { pnlRightLine.Visible = value; }
        }

        public string StepNumber { 
        
            get { return lblStepNumber.Text; }
            set { lblStepNumber.Text = value;  }
        
        }

        public string StepName
        {

            get { return lblStepName.Text; }
            set { lblStepName.Text = value; }

        }

        public bool SetHighlight 
        { 
            get { return (lblStepNumber.Font.Bold && lblStepName.Font.Bold); }
            set { 

                if (value)
                {
                    lblStepNumber.Font = new Font(lblStepNumber.Font, FontStyle.Bold);
                    lblStepName.Font = new Font(lblStepName.Font, FontStyle.Bold);

                    this.lblStepNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(167)))), ((int)(((byte)(213)))));
                }
                else
                {
                    lblStepNumber.Font = new Font(lblStepNumber.Font, FontStyle.Regular);
                    lblStepName.Font = new Font(lblStepName.Font, FontStyle.Regular);

                    this.lblStepNumber.BackColor = Color.DarkGray;
                }
            }
        }

        public StepControl()
        {
            InitializeComponent();
        }
    }
}
