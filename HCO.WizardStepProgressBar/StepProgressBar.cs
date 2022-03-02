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

    public partial class StepProgressBar : UserControl
    {

        private List<Step> _steps = new List<Step>();
        private int _x = 0;

        public StepProgressBar()
        {
            InitializeComponent();
        }

        public void AddStep(int number, string name, bool showLeftLine, bool showRightLine)
        {
            StepControl stepControl = new StepControl();
            stepControl.StepNumber = number.ToString();
            stepControl.StepName = name.ToString();
            stepControl.Location = new Point(_x, 0);
            stepControl.ShowLeftLine = showLeftLine;
            stepControl.ShowRightLine = showRightLine;

            _steps.Add(new Step { Number = number, Name = name });

            _x += stepControl.Width;

            this.Controls.Add(stepControl);
        }

        public void SetCurrentStep (int number)
        {
            foreach (StepControl stepControl in this.Controls)
            {
                if  (stepControl.StepNumber == number.ToString())
                    stepControl.SetHighlight = true;
                else
                    stepControl.SetHighlight = false;
            }
        }
    }
}
