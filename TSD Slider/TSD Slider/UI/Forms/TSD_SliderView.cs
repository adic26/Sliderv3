using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using TsdLib;
using TsdLib.Measurements;
using TsdLib.UI;
using RDotNet;
using TSD_Slider.Instruments;
using TSD_Slider.UI.Components;
using TSD_Slider.Configuration;
using TSD_Slider.Communication;
using System.Threading.Tasks;

namespace TSD_Slider.UI.Forms
{
    /// <summary>
    /// Base UI form containing controls and code common to all TsdLib applications.
    /// Protected controls may be overridden in application implementations but private controls cannot be modified by derived UI classes.
    /// </summary>
    public partial class TSD_SliderView : Form, IView
    {
        public virtual ITestCaseControl TestCaseControl { get { return testCasesMenuItem; } }

        public virtual IConfigControl ConfigControl { get { return multiConfigControl; } }

        public virtual ITestInfoDisplayControl TestInfoDisplayControl { get { return testInfoDataGridViewControl; } }

        public virtual IMeasurementDisplayControl MeasurementDisplayControl { get { return measurementDataGridViewControl; } }

        public virtual ITestSequenceControl TestSequenceControl { get { return testSequenceControl; } }

        public virtual ITestDetailsControl TestDetailsControl { get { return testDetailsControl; } }

        public virtual ITraceListenerControl TraceListenerControl { get { return traceListenerTextBoxControl; } }

        public virtual IProgressControl ProgressControl { get { return null; } }

        #region prototypes

        public int test;
        public StationConfig stationConfig;
        public ProductConfig productConfig;
        public tsdFanuc fanuc;
        public event EventHandler startButton;
        public event EventHandler stopButton;
        public event EventHandler connectButton;
        public event EventHandler ftpTestButton;
        public event EventHandler<bool> TESTLift;
        public event EventHandler<bool> dataTestButton;
        public REngine engine;
        public CharData DataGridLiftOffData;



        #endregion

        /// <summary>
        /// Initializes a new instance of the UI form.
        /// </summary>
        public TSD_SliderView()
        {
            InitializeComponent();

        }

        /// <summary>
        /// On Load During View Bootup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoad(object sender, EventArgs e)
        {
            stationConfig = multiConfigControl.SelectedStationConfig[0] as StationConfig;
            productConfig = multiConfigControl.SelectedProductConfig[0] as ProductConfig;

            lblTitle.Text = stationConfig.StationName;

            //Slider Control
            txtNumCycles.Text = productConfig.NumOfCycles.ToString();
            txtZForce.Text = productConfig.pushForcelbf.ToString();
            txtCalEvery.Text = productConfig.CalibrateEvery.ToString();
            txtCycleOffset.Text = "0";
            txtLiftOff.Text = productConfig.liftOffPoint.ToString();

            //Fanuc Control
            fanuc = Controller.ROBOT;

            //Important to move to home position in case of emergency stop
            txtFanucIP.Text = stationConfig.Fanuc_Ipaddress;
            txtCharName.Text = stationConfig.TPCharacterizationName;
            txtSlideCycleName.Text = stationConfig.TPSliderCycleName;
            txtRegLiftOffName.Text = stationConfig.RegLiftOffName;
            txtRegCyclesName.Text = stationConfig.RegCyclesName;
            txtRegCycCompleteName.Text = stationConfig.RegCyclesCompletedName;

            //Make the data grid view instance
            DataGridLiftOffData = (TSD_Slider.UI.Components.CharData) charData1;


        }


        public void updateProgressbar(int value)
        {
            progCalibration.Value = value;
            progCalibration.Update();
        }

        public void updateTextBox(int result)
        {
            txtCycleOffset.Text = result.ToString();

        }

        public void toggleTextBoxes()
        {
            txtNumCycles.Enabled = !txtNumCycles.Enabled;
            txtZForce.Enabled = !txtZForce.Enabled;
            txtCalEvery.Enabled = !txtCalEvery.Enabled;
            txtCycleOffset.Enabled = !txtCycleOffset.Enabled;
            txtLiftOff.Enabled = !txtLiftOff.Enabled;

            txtFanucIP.Enabled = !txtFanucIP.Enabled;
            txtCharName.Enabled = !txtCharName.Enabled;
            txtSlideCycleName.Enabled = !txtSlideCycleName.Enabled;
            txtRegLiftOffName.Enabled = !txtRegLiftOffName.Enabled;
            txtRegCyclesName.Enabled = !txtRegCyclesName.Enabled;
            txtRegCycCompleteName.Enabled = !txtRegCycCompleteName.Enabled;

        }

        public void startButtonEnDis(bool enable)
        {
            txtCycleOffset.ForeColor = System.Drawing.Color.Green;
            txtCycleOffset.Font = new System.Drawing.Font(txtCycleOffset.Font, System.Drawing.FontStyle.Bold);
            //button_ExecuteTestSequence.Enabled = enable;
            toggleConnStatus(enable);
        }

        public void toggleConnStatus(bool flag)
        {
            if (flag)
            {
                lblStatus.Text = "CONNECTED";
                lblStatus.ForeColor = System.Drawing.Color.Green;
                //btnConnect.Enabled = false;
            }
            else
            {
                lblStatus.Text = "DISCONNECTED";
                lblStatus.ForeColor = System.Drawing.Color.Black;
                //btnConnect.Enabled = true;
            }
        }

        public void graphCharacterization(List<double> disp, List<double> force)
        {
            string seriesName = "Characterization " + ResultsChart.Series.Count;

            var newseries = new Series();
            newseries.ChartType = SeriesChartType.Line;
            newseries.Name = seriesName;

            newseries.Points.DataBindXY(disp.ToArray(), force.ToArray());

            ResultsChart.Series.Add(newseries);
            ResultsChart.Update();

        }

        /// <summary>
        /// Set the appearance and behaviour of IU controls, based on the current status of the system.
        /// </summary>
        /// <param name="state">State to set.</param>
        public virtual void SetState(State state)
        {
            foreach (object control in Controls)
            {
                ITsdLibControl tsdCtrl = control as ITsdLibControl;
                if (tsdCtrl != null)
                    tsdCtrl.SetState(state);
            }
        }

        /// <summary>
        /// Add an <see cref="IMeasurement"/> to the DataGridView.
        /// </summary>
        /// <param name="measurement">Measurement to add.</param>
        public virtual void AddMeasurement(IMeasurement measurement)
        {
            measurementDataGridViewControl.AddMeasurement(measurement);
        }

        /// <summary>
        /// Add a general data object to the UI. Override to perform specific operations based on the type of the internal data value.
        /// </summary>
        /// <param name="data">Data to add.</param>
        public void AddData(object data)
        {
            addData((dynamic)data);
        }

        private void addData(object data)
        {
            Trace.WriteLine(string.Format("Unsupported data type received: {0}", data.GetType().Name));
            Trace.WriteLine(string.Format("String representation: {0}", data));
        }

        private void addData(double data)
        {
            //some logic to add the double to your UI
        }

        private void addData(dataFrameContainer x)
        {
            //this is the dataframe object
            //execute my report
            DataGridLiftOffData.updateData(x.xFrame);
            graphCharacterization(DataGridLiftOffData.GetValues(stationConfig.scriptDisplacementName), DataGridLiftOffData.GetValues(stationConfig.scriptForceName));

        }

        /// <summary>
        /// Update Cycles In View Coming From Robot/Sequence
        /// </summary>
        /// <param name="updateCycles">Update Cycles Method</param>
        private void addData(IntStruct data)
        {
            if (data.context == "UpdateCycles") 
                updateTextBox(data.data);

            if (data.context == "progressBar")
                updateProgressbar(data.data);
        }

        private void addData(ConnectionStatus status)
        {
            startButtonEnDis(status.status);
            toggleTextBoxes();
        }

        private void ViewBase_FormClosing(object sender, FormClosingEventArgs e)
        {
            EventHandler<CancelEventArgs> invoker = UIClosing;
            if (invoker != null)
                invoker(this, e);
        }

        /// <summary>
        /// Sets the text displayed in the title section of the UI.
        /// </summary>
        /// <param name="title">Text to display.</param>
        public void SetTitle(string title)
        {
            Text = title;
        }

        /// <summary>
        /// Event fired when the UI is about to close.
        /// </summary>
        public event EventHandler<CancelEventArgs> UIClosing;


       
    }
}
