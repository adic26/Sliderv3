using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using TsdLib;
using TsdLib.Measurements;
using TsdLib.UI;
using RDotNet;
using TSD_Slider.Instruments;
using TSD_Slider.UI.Components;
using TSD_Slider.Configuration;
using TSD_Slider.Communication;
using sliderv2.Exceptions;
using System.Threading.Tasks;
using TsdLib.Observer;

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

        public StationConfig stationConfig;
        public ProductConfig productConfig;
        public CharData DataGridLiftOffData;
        public bool setExecuteRunner;

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
            setExecuteRunner = false;

            stationConfig = multiConfigControl.SelectedStationConfig[0] as StationConfig;
            productConfig = multiConfigControl.SelectedProductConfig[0] as ProductConfig;

            lblTitle.Text = stationConfig.StationName;

            //Slider Control
            txtNumCycles.Text = productConfig.NumOfCycles.ToString();
            txtZForce.Text = productConfig.pushForcelbf.ToString();
            txtCalEvery.Text = productConfig.CalibrateEvery.ToString();
            txtCycleOffset.Text = "0";
            txtLiftOff.Text = productConfig.liftOffPoint.ToString();

            //Important to move to home position in case of emergency stop
            txtFanucIP.Text = stationConfig.Fanuc_Ipaddress;
            txtCharName.Text = stationConfig.TPCharacterizationName;
            txtSlideCycleName.Text = stationConfig.TPSliderCycleName;
            txtRegLiftOffName.Text = stationConfig.RegLiftOffName;
            txtRegCyclesName.Text = stationConfig.RegCyclesName;
            txtRegCycCompleteName.Text = stationConfig.RegCyclesCompletedName;

            //Make the data grid view instance
            DataGridLiftOffData = charData1;


        }

        /// <summary>
        /// Add a general data object to the UI. Uses dynamic method overload resolution to automatically bind to the correct method based on the type of data.
        /// </summary>
        /// <param name="dataContainer">Data to add.</param>
        public void AddData(DataContainer dataContainer)
        {
            addData((dynamic)dataContainer.Data);
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
            Trace.WriteLine("graphing method reached");
            string seriesName = "Characterization " + ResultsChart.Series.Count;

            var newseries = new Series();
            newseries.ChartType = SeriesChartType.Line;
            newseries.Name = seriesName;

            newseries.Points.DataBindXY(disp.ToArray(), force.ToArray());

            ResultsChart.Series.Add(newseries);
            ResultsChart.Update();
            Trace.WriteLine("Results Charts Updated");

        }

        public void graphCharacterization(double[] displacement, double[] force)
        {
            Trace.WriteLine("graphing method reached");
            string seriesName = "Characterization " + ResultsChart.Series.Count;

            var newseries = new Series();
            newseries.ChartType = SeriesChartType.Line;
            newseries.Name = seriesName;

            newseries.Points.DataBindXY(displacement, force);

            ResultsChart.Series.Add(newseries);
            ResultsChart.Update();
            Trace.WriteLine("Results Charts Updated");
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
            if (setExecuteRunner == true)
            {
                testSequenceControl.Enabled = false;
                
            }
            setExecuteRunner = true;
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


        private void addData(dataFrame veniceData)
        {
            //Update Char data
            charData1.updateData(veniceData);
            Trace.WriteLine("Finish Updating the DataGridView");

            //Update Graph
            graphCharacterization(veniceData.displacement, veniceData.force);
            Trace.WriteLine("Finish Updating the Graph");
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
