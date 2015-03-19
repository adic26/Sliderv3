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


        private void RFilesCopy()
        {
            string fileName;
            string destFile;
            string[] files;

            //create a directory if does not exist
            if (!System.IO.Directory.Exists(stationConfig.scriptLocation))
            {
                System.IO.Directory.CreateDirectory(stationConfig.scriptLocation);

            }
            if (!System.IO.Directory.Exists(System.IO.Path.Combine(Environment.CurrentDirectory, "archive")))
            {
                System.IO.Directory.CreateDirectory("archive");
            }


            //once the directory is made, now copy all the designated files
            string source = System.IO.Path.Combine(Environment.CurrentDirectory, "Scripts");
            string targetPath = stationConfig.scriptLocation;
            files = System.IO.Directory.GetFiles(source);

            if (System.IO.Directory.Exists(source))
            {
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    fileName = System.IO.Path.GetFileName(s);
                    destFile = System.IO.Path.Combine(targetPath, fileName);
                    System.IO.File.Copy(s, destFile, true); //overwrite

                }
            }
            else
                Trace.WriteLine("Souce path does not exist!");




        }

        public void archiveDTFiles()
        {
            try
            {
                string[] files = System.IO.Directory.GetFiles(stationConfig.PCDataFolderPath);

                foreach (string s in files)
                {
                    string filename = System.IO.Path.GetFileName(s);
                    string destFile = System.IO.Path.Combine(System.Environment.CurrentDirectory, "archive", System.IO.Path.GetFileName(s));
                    System.IO.File.Move(s, destFile);
                    Trace.WriteLine("File " +
                        System.IO.Path.GetFileName(s) +
                        " Archived in " +
                        System.IO.Path.Combine(System.Environment.CurrentDirectory, "archive"));
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
            }
        }

        private void button_AbortTestSequence_Click(object sender, System.EventArgs e)
        {
            //Important to move to home position in case of emergency stop
        }

        private void button_ExecuteTestSequence_Click(object sender, System.EventArgs e)
        {
            //in parallel for all logging if required
            if (startButton != null)
                startButton(this, e);
        }

        private void btnConnect_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Copying all the environmental R Files to designated folder
                RFilesCopy();

                if (connectButton != null)
                    connectButton(this, e);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
            }
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

        public void toggleStartButton()
        {
            txtCycleOffset.ForeColor = System.Drawing.Color.Green;
            txtCycleOffset.Font = new System.Drawing.Font(txtCycleOffset.Font, System.Drawing.FontStyle.Bold);
            button_ExecuteTestSequence.Enabled = !button_ExecuteTestSequence.Enabled;
        }

        public void toggleStopButton()
        {
            btnStop.Enabled = !btnStop.Enabled;
        }

        public void startButtonEnDis(bool enable)
        {
            txtCycleOffset.ForeColor = System.Drawing.Color.Green;
            txtCycleOffset.Font = new System.Drawing.Font(txtCycleOffset.Font, System.Drawing.FontStyle.Bold);
            button_ExecuteTestSequence.Enabled = enable;
        }

        public void toggleConnStatus(bool flag)
        {
            if (flag)
            {
                lblStatus.Text = "CONNECTED";
                lblStatus.ForeColor = System.Drawing.Color.Green;
                btnConnect.Enabled = false;
            }
            else
            {
                lblStatus.Text = "DISCONNECTED";
                lblStatus.ForeColor = System.Drawing.Color.Black;
                btnConnect.Enabled = true;
            }


        }

        private void btnFtpTest_Click(object sender, EventArgs e)
        {
            if (ftpTestButton != null)
                Task.Run(() => ftpTestButton(this, e));
            btnFtpTest.Enabled = false;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            stopButton(this, e);
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

        public void btnREngine_Click_1(object sender, EventArgs e)
        {
            RFilesCopy();
            connectButton(this, e);
            //make sure to creat the progress reporting on the UI thread
            Task.Run(() => dataTestButton(this, true));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() => TESTLift(this, true));
        }

        public void LogMeasurement(double datapoint)
        {
            //MeasurementParameter measurementParam = new MeasurementParameter("Baseline", 0);
            //Measurement meas = Measurement.CreateMeasurement<double>("Lift Off", datapoint, "mm", 18.0, 28.0, parameters: measurementParam);

            //TestResultsHeader header = new TestResultsHeader("", "", "", "", "", "", "", DateTime.Today, DateTime.Now, "", "");
            //tr.AddHeader(header);
            //tr.AddMeasurement(meas);
            //tr.Save(new System.IO.DirectoryInfo(@"C:\TestResults"));

            //measurementDataGridView.AddMeasurement(meas);
            //Trace.WriteLine("Measurements saved to " + @"C:\TestResults" + " folder");

            Measurement<double> meas = new Measurement<double>("Lift Off", datapoint, "mm", 18.0, 28.0);
            AddMeasurement(meas);


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
