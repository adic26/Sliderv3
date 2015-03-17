using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using TsdLib;
using TsdLib.Measurements;
using TsdLib.UI;

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

        public virtual IProgressControl ProgressControl { get { return progressControl; } }

        /// <summary>
        /// Initializes a new instance of the UI form.
        /// </summary>
        public TSD_SliderView()
        {
            InitializeComponent();

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
