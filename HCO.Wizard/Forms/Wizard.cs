

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using HCO.Wizard;
using HCO.Wizard.Forms.Panels;
using HCO.Wizard.Common;
using HCO.Wizard.BL;
using HCO.Wizard.DTO;
using System.Drawing.Printing;

namespace HCO.Wizard.Forms
{

    /// <summary>
    /// Enumeración para manejar los tipos de edición
    /// </summary>
    public enum FormStatus
    {
        Add = 1,
        Edit = 2
    }

    /// <summary>
    /// Formulario que permite gestionar mediante un asistente la creación y edición de un escenario de replicación
    /// </summary>
    public partial class Wizard : Form
    {
        #region Declaraciones

        private WizardSettings _wizardSettings;

        private PnlWelcome _pnlWelcome;
        private PnlFilters _pnlFilters;
        private PnlChooseDocuments _pnlChooseDocuments;
        private PnlProcess _pnlProcess;

        private int _start;
        private int _totalPanels;
        private int _currentPanel;
        private bool _execute;


        private List<HCO.WizardStepProgressBar.Step> _steps;


        private List<OrderDTO> _orders;
        private List<OrderDTO> _resultOfProcess;
        private List<InvoiceDTO> _invoicesCreated;

        private InvoiceBL _invoiceBL = new InvoiceBL();

        private string _form;

        private string _indicator;

        private string _condVenta;

        private int _count;

        private string print;

        #endregion

        #region Constructor

        /// <summary>
        /// Método constructor
        /// </summary>
        /// <param name="formStatus">Estado de edición del formulario</param>
        /// <param name="configXmlFilePath">Ruta del archivo Xml de configuración</param>
        /// <param name="scenarioId">En modo editar se debe utilizar el id del escenario a cargar en el Wizard.
        /// En modo agregar se debe utilizar el id del nuevo escenario a crear</param>
        public Wizard()
        {
            InitializeComponent();

            _execute = false;

            _currentPanel = 1;

            _invoiceBL = new InvoiceBL();

            _invoiceBL.OrderLineReached += _invoiceDA_OrderLineReached;
        }


        #endregion

        #region Métodos privados

        private void SetupPanels()
        {

            _steps = new List<WizardStepProgressBar.Step>();

            foreach (var wizardStep in _wizardSettings.WizardSteps)
            {
                _steps.Add(new WizardStepProgressBar.Step { PanelId = wizardStep.PanelId, Name = wizardStep.Name, Number = wizardStep.Number });
            }

            int first = _steps.Min(t => t.Number);
            int last = _steps.Max(t => t.Number);

            _start = first;
            _totalPanels = last;

            foreach (HCO.WizardStepProgressBar.Step step in _steps)
            {
                if (first == step.Number)
                    this.stepProgressBar.AddStep(step.Number, step.Name, false, true);
                else if (last == step.Number)
                    this.stepProgressBar.AddStep(step.Number, step.Name, true, false);
                else
                    this.stepProgressBar.AddStep(step.Number, step.Name, true, true);
            }

        }


        private void SetAnchor (Control control)
        {

            control.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));

        }

        /// <summary>
        /// Carga los paneles en el contenedor del formulario principal
        /// </summary>
        private void LoadPanels()
        {
            _pnlWelcome = new PnlWelcome();
            _pnlWelcome.Name = "PnlWelcome";
            SetAnchor(_pnlWelcome);

            _pnlFilters = new PnlFilters();
            _pnlFilters.Name = "PnlFilters";
            SetAnchor(_pnlFilters);

            _pnlChooseDocuments = new PnlChooseDocuments();
            _pnlChooseDocuments.Name = "PnlChooseDocuments";
            SetAnchor(_pnlChooseDocuments);

            _pnlProcess = new PnlProcess();
            _pnlProcess.Name = "PnlProcess";
            SetAnchor(_pnlProcess);

            _pnlWelcome.Visible = false;
            _pnlFilters.Visible = false;
            _pnlChooseDocuments.Visible = false;
            _pnlProcess.Visible = false;

            this.pnlContenedor.Controls.Add(_pnlWelcome);
            this.pnlContenedor.Controls.Add(_pnlFilters);
            this.pnlContenedor.Controls.Add(_pnlChooseDocuments);
            this.pnlContenedor.Controls.Add(_pnlProcess);

            _pnlProcess.llblReturnToStart.Click += LlblReturnToStart_Click;
            _pnlProcess.btnPrint.Click += btnPrint_Click;
        }

        private void LlblReturnToStart_Click(object sender, EventArgs e)
        {
            _currentPanel = 1;
            this.btnPrevious.Enabled = true;
            this.btnNext.Enabled = true;
            _pnlProcess.btnPrint.Enabled = false;
            _pnlProcess.btnPrint.Visible = false;
            _pnlProcess.llblReturnToStart.Visible = false;
            _pnlProcess.lblPrints.Visible = false;
            _pnlProcess.cbPrints.Visible = false;
            ShowPanel();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_pnlProcess.cbPrints.Text))
                print = _pnlProcess.cbPrints.Text;
                
            _pnlProcess.btnPrint.Enabled = false;
            _pnlProcess.llblReturnToStart.Enabled = false;
            startInvoicePrint();
        }

        /// <summary>
        /// Ejecuta las validaciones en el panel actual
        /// </summary>
        /// <returns>Devuelve verdadero si el panel cumple con las validaciones en caso contrario devuelve falso</returns>
        private bool ValidatePanel()
        {

            bool result = true;
            string panelName = _steps.Where(t => t.Number == _currentPanel).FirstOrDefault().PanelId;

            foreach (PnlBase panel in this.pnlContenedor.Controls)
            {
                if (panel.Name == panelName)
                {
                    result = panel.CheckInputData();
                }

            }

            return result;
        }

        private void InitializeCreation()
        {
            _pnlProcess.progressBar.Visible = false;
            _pnlProcess.lblInfo.Visible = false;
            _pnlProcess.lblPercent.Visible = false;

            _pnlProcess.lblInfo.Text = "";
            _pnlProcess.lblPercent.Text = "";

            if (_orders != null)
                _orders.Clear();

            if (_resultOfProcess != null)
                _resultOfProcess.Clear();
        }

        private void StartInvoiceCreation()
        {
            _pnlProcess.progressBar.Visible = true;
            _pnlProcess.lblInfo.Visible = true;
            _pnlProcess.lblPercent.Visible = true;

            _pnlProcess.progressBar.Value = 0;
            _pnlProcess.progressBar.Maximum = _orders.Where(o => o.Selected == true).Count();
            _pnlProcess.lblInfo.Text = "";
            _pnlProcess.lblPercent.Text = "";

            _resultOfProcess = new List<OrderDTO>();

            _invoicesCreated = new List<InvoiceDTO>();

            _form = _pnlProcess.cboDocumentType.SelectedValue.ToString();

            _condVenta = _pnlProcess.cboCondVenta.Text.Split('-')[0];

            IndicadorDocumentoBL indicadorDocumentoBL = new IndicadorDocumentoBL();
            _indicator = indicadorDocumentoBL.GetIndicador(_form);

            _count = 0;

            SetLblTextInfo("Iniciando creación de documentos");

            _pnlProcess.EnabledControls(false);

            this.backgroundWorker.RunWorkerAsync();
        }

        private void startInvoicePrint()
        {
            _pnlProcess.progressBar.Visible = true;
            _pnlProcess.lblInfo.Visible = true;
            _pnlProcess.lblPercent.Visible = true;

            _pnlProcess.progressBar.Value = 0;
            _pnlProcess.progressBar.Maximum = _invoicesCreated.Where(t => t.EstadoAut == 1).Count();
            _pnlProcess.lblInfo.Text = "";
            _pnlProcess.lblPercent.Text = "";

            _count = 0;

            SetLblTextInfo("Iniciando impresión de documentos");

            this.bgWPrint.RunWorkerAsync();
        }

        private void LoadOrders()
        {            
            DateTime? taxDateFrom = null; 
            DateTime? taxDateTo = null;
            DateTime? dueDateFrom = null;
            DateTime? dueDateTo = null;
            string groupNum = null;
            string slpCode = null;
            string groupCode = null;
            string deliveryType = null;


            if (_pnlFilters.chkTaxDate.Checked)
            {
                taxDateFrom = _pnlFilters.dtpFromTaxDate.Value;
                taxDateTo = _pnlFilters.dtpToTaxDate.Value;
            }

            if (_pnlFilters.chkDueDate.Checked)
            {
                dueDateFrom = _pnlFilters.dtpFromDocDueDate.Value;
                dueDateTo = _pnlFilters.dtpToDocDueDate.Value;
            }

            if (_pnlFilters.chkPaymentTerms.Checked)
                groupNum = _pnlFilters.cboPaymentTerms.SelectedValue.ToString();

            if (_pnlFilters.chkSalesEmployees.Checked)
                slpCode = _pnlFilters.cboSalesEmployee.SelectedValue.ToString();

            if (_pnlFilters.chkBusinessPartnerGroup.Checked)
                groupCode = _pnlFilters.cboBusinessPartnerGroup.SelectedValue.ToString();

            if (_pnlFilters.chkDelivery.Checked)
                deliveryType = _pnlFilters.cboDeliveryType.SelectedValue.ToString();

            if (!_pnlWelcome.rbtPrintDocuments.Checked)
            {
                _orders = new OrderBL().GetOrders(taxDateFrom,
                                            taxDateTo,
                                            dueDateFrom,
                                            dueDateTo,
                                            groupNum,
                                            slpCode,
                                            groupCode, deliveryType);
                _pnlChooseDocuments.dgvOrders.DataSource = _orders;
            }
            else
            {
                _invoicesCreated = new InvoiceBL().GetInvoices(taxDateFrom,
                                            taxDateTo,
                                            dueDateFrom,
                                            dueDateTo,
                                            groupNum,
                                            slpCode,
                                            groupCode, deliveryType);
                _pnlProcess.bsInvoicesElectronic.DataSource = _invoicesCreated;
            }

        }


        private void AddInvoices()
        {
            _invoiceBL.CreateInvoices(_orders,
                                    _form, 
                                    _indicator,
                                    _condVenta,
                                     DateTime.Now, 
                                     ref _resultOfProcess, 
                                     ref _invoicesCreated);   
        }

        private void printIncoives()
        {
            _invoiceBL.printInvoices(_invoicesCreated.Where(t => t.EstadoAut == 1).ToList(), _form, print);
        }


        /// <summary>
        /// Ejecuta la acción del panel actual
        /// </summary>
        private void ExecutePanelAction()
        {

            if (_currentPanel == _wizardSettings.WizardSteps.Where(t => t.PanelId == _pnlChooseDocuments.Name).FirstOrDefault().Number)
            {
                _pnlChooseDocuments.chkSel.Checked = false;
                InitializeCreation();         
                LoadOrders();
            }

            if (_currentPanel == _wizardSettings.WizardSteps.Where(t => t.PanelId == _pnlProcess.Name).FirstOrDefault().Number)
            {
                //if (_invoicesCreated != null)
                //    _invoicesCreated.Clear();

                //if (_resultOfProcess != null)
                //    _resultOfProcess.Clear();

                _pnlProcess.bsInvoicesElectronic.Clear();
                _pnlProcess.bsInvoices.Clear();
                _pnlProcess.bsOrders.Clear();

                if (_pnlWelcome.rbtPrintDocuments.Checked)
                    LoadOrders();
            }

            if (_currentPanel == _totalPanels)            
                Execute();            
            else
                _execute = false;

        }




        /// <summary>
        /// Aumenta el contador del panel actual
        /// </summary>
        private void Next()
        {
            bool result = ValidatePanel();

            //En caso que el panel actual no pase las validaciones se impide continuar
            if (!result)
                return;

            if (_currentPanel == _wizardSettings.WizardSteps.Where(t => t.PanelId == _pnlFilters.Name).FirstOrDefault().Number && _pnlWelcome.rbtPrintDocuments.Checked)
                _currentPanel = _totalPanels;

            else if (_currentPanel < _totalPanels)
                _currentPanel++;

            
        }

        /// <summary>
        /// Disminuye el contador del panel actual
        /// </summary>
        private void Previous()
        {
            if (_currentPanel > _start)
                _currentPanel--;
        }

        /// <summary>
        /// Muestra el panel actual
        /// </summary>
        private void ShowPanel()
        {
            string panelName = _steps.Where(t => t.Number == _currentPanel).FirstOrDefault().PanelId;

            foreach (PnlBase panel in this.pnlContenedor.Controls)
            {
                if (panel.Name == panelName)
                {
                    this.stepProgressBar.SetCurrentStep(_currentPanel);
                    panel.Visible = true;
                    panel.SetFocus();
                }
                else
                    panel.Visible = false;
            }

            btnPrevious.Visible = (_currentPanel > _start && _currentPanel <= _totalPanels);

            if (_currentPanel == _totalPanels)
            {
                btnNext.Text = "Procesar";
                if(_pnlWelcome.rbtPrintDocuments.Checked)
                {
                    btnNext.Enabled = false;
                    btnPrevious.Enabled = false;
                    _pnlProcess.btnPrint.Visible = true;
                    _pnlProcess.btnPrint.Enabled = true;
                    _pnlProcess.cbPrints.Visible = true;
                    _pnlProcess.cbPrints.Items.Clear();
                    _pnlProcess.lblPrints.Visible = true;
                    _pnlProcess.llblReturnToStart.Visible = true;
                    _pnlProcess.llblReturnToStart.Enabled = true;
                    _pnlProcess.progressBar.Value = 0;
                    _pnlProcess.lblInfo.Text = "";
                    _pnlProcess.lblPercent.Text = "";
                    for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
                    {
                        _pnlProcess.cbPrints.Items.Add(PrinterSettings.InstalledPrinters[i]);
                    }
                }
            }
            else
                btnNext.Text = "Siguiente>";
        }


        private void Execute()
        {
            if (_execute)
            {
                if (MessageUI.Question("¿Esta seguro de crear las facturas?. Este proceso es irreversible") == System.Windows.Forms.DialogResult.Yes)
                {
                    this.btnPrevious.Enabled = false;
                    this.btnNext.Enabled = false;
                    

                    StartInvoiceCreation();

                    _execute = false;
                }
            }

            _execute = true;
        }

        public void SetLblTextInfo(string text)
        {
            if (_pnlProcess.lblInfo.InvokeRequired)
            {
                _pnlProcess.lblInfo.BeginInvoke(
                    new MethodInvoker(
                    delegate () { SetLblTextInfo(text); }));
            }
            else
            {
                _pnlProcess.lblInfo.Text = text;
            }
        }

        public void SetProgressBarValue(int value)
        {
            if (_pnlProcess.progressBar.InvokeRequired)
            {
                _pnlProcess.progressBar.BeginInvoke(
                    new MethodInvoker(
                    delegate () { SetProgressBarValue(value); }));
            }
            else
            {
                _pnlProcess.progressBar.Value = value;
            }
        }

        #endregion

        #region Métodos públicos

        #endregion

        #region Propiedades

        #endregion

        #region Eventos

        private void Wizard_Load(object sender, EventArgs e)
        {
            try
            {

                Login login = new Login();

                login.ShowDialog();

                _wizardSettings = JSerializer.DeserializeFromFile<WizardSettings>(@"settings.json");

                LoadPanels();

                SetupPanels();

                ShowPanel();
            }
            catch (Exception ex)
            {
                MessageUI.Error(ex.Message, ex);
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                Previous();
                ShowPanel();
                ExecutePanelAction();
            }
            catch (Exception ex)
            {
                MessageUI.Error(ex.Message, ex);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                Next();                
                ShowPanel();
                ExecutePanelAction();
            }
            catch (Exception ex)
            {
                MessageUI.Error(ex.Message, ex);
            }
        }


        private void _invoiceDA_OrderLineReached(object sender, OrderLineReachedEventArgs e)
        {
            _count++;
            if (!e.Print)
                SetLblTextInfo("Procesando orden " + e.DocNum.ToString() + ", línea " + e.LineNum.ToString());
            else
                SetLblTextInfo("Imprimiendo documento " + e.DocNum.ToString() + ", folio " + e.Folio);

            SetProgressBarValue(_count);
        }

        #endregion

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            AddInvoices();
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _pnlProcess.lblInfo.Text = "Proceso finalizado";
            _pnlProcess.progressBar.Value = _pnlProcess.progressBar.Maximum;


            _pnlProcess.bsInvoicesElectronic.DataSource = _invoicesCreated.Where(t => t.EstadoAut == 1).ToList(); //Facturas creadas en SAP y timbradas electronicamente
            _pnlProcess.bsInvoices.DataSource = _invoicesCreated.Where(t => t.EstadoAut == 0).ToList(); //Facturas creadas pero con problemas al timbrar electronicamente
            _pnlProcess.bsOrders.DataSource = _resultOfProcess.Where(t => t.InvoiceCreated == false).ToList(); //Pedidos que no se pudieron crear
            _pnlProcess.llblReturnToStart.Visible = true;
            _pnlProcess.btnPrint.Visible = true;
            _pnlProcess.btnPrint.Enabled = true;
            _pnlProcess.cbPrints.Visible = true;
            _pnlProcess.cbPrints.Items.Clear();
            _pnlProcess.lblPrints.Visible = true;
            _pnlProcess.EnabledControls(true);

            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                _pnlProcess.cbPrints.Items.Add(PrinterSettings.InstalledPrinters[i]);
            }

            MessageUI.Show("Proceso finalizado");
        }

        private void Wizard_FormClosing(object sender, FormClosingEventArgs e)
        {
            DIAPIHelper.Disconnect();
        }

        private void bgWPrint_DoWork(object sender, DoWorkEventArgs e)
        {
            printIncoives();
        }

        private void bgWPrint_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _pnlProcess.lblInfo.Text = "Proceso finalizado";
            _pnlProcess.progressBar.Value = _pnlProcess.progressBar.Maximum;
            _pnlProcess.llblReturnToStart.Visible = true;
            _pnlProcess.llblReturnToStart.Enabled = true;
            _pnlProcess.btnPrint.Enabled = true;

            MessageUI.Show("Proceso finalizado");
        }
    }
}
