using System;
using System.Threading;
using TSD_Slider.Configuration;
using TsdLib.Measurements;
using TsdLib.TestSystem.TestSequence;
using System.Diagnostics;
using TSD_Slider.Instruments;
using FRRobot;
using RDotNet;

namespace TSD_Slider.Sequences
{
    public class RobotSequence : SequentialTestSequence<StationConfig, ProductConfig, TestConfig>
    {
        public tsdFanuc ROBOT;
        protected override void ExecutePreTest(CancellationToken token, StationConfig stationConfig, ProductConfig productConfig)
        {
            //connection
            base.ExecutePreTest(token, stationConfig, productConfig);
            ROBOT = new tsdFanuc();

            //Setting up Robot Events
            ROBOT.updateCyclesInView += ROBOT_updateCyclesInView;
            ROBOT.robotConnectionStatus += myView_robotConnectStatus;
            ROBOT.ftpDownloadDataFile += myView_ftpTestButton;
            ROBOT.getCalibrationData += getCalibrationRawData;
            ROBOT.getLiftOffValue += ROBOT_getLiftOffValue;

            //Connect Start Here
            RFilesCopy(stationConfig);
            
            //Graph Raw Data -> Controller -> Graph and DataGridView
            //Instantiations (remain inside Controller)
                //1.  DataCollection (DataBuilder stuff)
                //2.  ProgressData
                //3.  How to send information for charDataView and graphCharacterization

            //FTP Instantiation

            //connection necessary stuff
            ROBOT.connect(stationConfig.Fanuc_Ipaddress);
            //include robot properties
            
        }

        private void ROBOT_getLiftOffValue(object sender, bool e)
        {
            throw new NotImplementedException();
        }

        private void getCalibrationRawData(object sender, bool e)
        {
            throw new NotImplementedException();
        }

        private void myView_ftpTestButton(object sender, bool e)
        {
            throw new NotImplementedException();
        }

        private void myView_robotConnectStatus(object sender, bool e)
        {
            throw new NotImplementedException();
        }

        private void ROBOT_updateCyclesInView(object sender, int e)
        {
            SendData<int>(e);
        }

        //private void mobjTasks_Change(FREStatusTypeConstants ChangeType, FRCTask progTask)
        //{
        //    //Slider Change or Characterization Change
        //    if (progTask.CurProgram.Name == tpSlider)
        //    {
        //        switch (progTask.Status)
        //        {
        //            case (FRETaskStatusConstants.frStatusIdle):
        //                WriteToScreen("Idle Status: " + progTask.CurProgram.Name);
        //                break;
        //            case (FRETaskStatusConstants.frStatusAborted):
        //                cycleTimer.Dispose();
        //                WriteToScreen("Aborted " + progTask.CurProgram.Name);

        //                //Only place to stop monitor for all tasks change
        //                mobjTasks.StopMonitor();
        //                WriteToScreen("Stopping Watch Event for Slider Completed - Changes!");
        //                StopMonitorDataRegister(robot_SLIDER_COMPLETED_registerName); //Stopping Monitor : Telling ActiveX robot

        //                //Calibration routine
        //                startCalibrationTPProgram();
        //                break;
        //            case (FRETaskStatusConstants.frStatusPaused):
        //                WriteToScreen("Paused " + progTask.CurProgram.Name);
        //                break;
        //            default:
        //                break;

        //        }
        //    }
        //    else if (progTask.CurProgram.Name == tpChar)
        //    {
        //        switch (progTask.Status)
        //        {
        //            case (FRETaskStatusConstants.frStatusAborted):
        //                //close calibration timer
        //                calibrationTimer.Dispose();

        //                WriteToScreen("Aborted " + progTask.CurProgram.Name);
        //                //check whether the program interrupted in the middle or not
        //                //determine what kind of alarm do you usually get
        //                mobjTasks.StopMonitor();

        //                //FTP module - Robot to controller. Empty string for future use
        //                ftpDownloadDataFile(this, true);

        //                //TODO 2.  Evaluate Lift off
        //                //execute lift off method
        //                if (OkToEvaluateLiftOff)
        //                {
        //                    //Adds the dataset onto datagridview and on graph
        //                    getCalibrationData(this, OkToEvaluateLiftOff);
        //                    Trace.WriteLine("Data Extraction Process Completed");
        //                    //Register LIft off update
        //                    getLiftOffValue(this, true);
        //                    Trace.WriteLine("Lift Off Module Completed");
        //                }
        //                calibrateOrEndCycles();
        //                break;
        //            case (FRETaskStatusConstants.frStatusRun):
        //                int currentLine = progTask.CurLine;
        //                int result = ((int)(((float)currentLine / (float)tpChar_lines) * 100));
        //                progressBar.Report(result);
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}


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
            //1.  Connect - pretest

            //2. StartButton
            ROBOT.startSliderTest();

            //While Testing
            //Task Change
            //Register Change

            //Calibrate Seperately
            //Cycle Sepreately



        }

        private void RFilesCopy(StationConfig stationConfig)
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

        public void archiveDTFiles(StationConfig stationConfig)
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
    
    }
}
