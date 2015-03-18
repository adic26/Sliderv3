using System.Threading;
using TSD_Slider.Configuration;
using TsdLib.Measurements;
using TsdLib.TestSystem.TestSequence;
using System.Diagnostics;

namespace TSD_Slider.Sequences
{
    public class RobotSequence : SequentialTestSequence<StationConfig, ProductConfig, TestConfig>
    {
        protected override void ExecuteTest(CancellationToken token, StationConfig stationConfig, ProductConfig productConfig, TestConfig testConfig)
        {
            //Use the System.Diagnostics.Debugger.Break() method to insert breakpoints.
            //System.Diagnostics.Debugger.Break();

            //ExamplePowerSupply ps = ExamplePowerSupply.Connect("addr");

            //for (int i = 0; i < testConfig.LoopIterations; i++)
            //{
            //    foreach (double voltageSetting in testConfig.VoltageSettings)
            //    {
            //        token.ThrowIfCancellationRequested();
            //        ps.SetVoltage(voltageSetting);
            //        Thread.Sleep(productConfig.SettlingTime);

            //        MeasurementParameter[] measurementParameters =
            //        {
            //            new MeasurementParameter("Loop Iteration", i),
            //            new MeasurementParameter("Voltage", voltageSetting),
            //            new MeasurementParameter("Temperature", 22.5)
            //        };
            //        Measurement<double> measurement = new Measurement<double>("Current", ps.GetCurrent(), "Amps", 0.1, 0.8, parameters: measurementParameters);
            //        AddMeasurement(measurement);
            //    }
            //}

            //Initialize all the event handlers
            //controller - do not have access , but do not need it
            
            //view - do not have access
            
            //robot - have access
            Trace.WriteLine("Please press Start to start testing");

            //Sequence
            //1.  Connect

            //2. Start Testing



        }
    }
}
