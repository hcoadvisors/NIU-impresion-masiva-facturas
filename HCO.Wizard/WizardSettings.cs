using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCO.Wizard
{
    public class WizardSettings
    {

        public string Description { get; set; }

        private List<WizardStep> _wizardSteps = new List<WizardStep>();

        public List<WizardStep> WizardSteps { get { return _wizardSteps; } }


    }
}
