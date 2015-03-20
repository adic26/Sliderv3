using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using TSD_Slider.Configuration;
using TsdLib.Measurements;
using TsdLib.TestSystem.TestSequence;
using System.Diagnostics;
using TSD_Slider.Instruments;
using TSD_Slider.UI.Components;
using FRRobot;
using RDotNet;
using TSD_Slider.Communication;

namespace TSD_Slider.Sequences
{
    public class RobotSequence : SequentialTestSequence<StationConfig, ProductConfig, TestConfig>
    {
        public tsdFanuc ROBOT;
        public IntStruct updateCycles;
        public FTP myFTP;
        public StationConfig stnConfig;
        public ProductConfig prdConfig;
        public CharData charDataView;
        public DataBuilder dataCollection;

        protected override void ExecutePreTest(CancellationToken token, StationConfig stationconfig, ProductConfig productconfig)
        {
            //connection
            base.ExecutePreTest(token, stationconfig, productconfig);

            stnConfig = stationconfig;
            prdConfig = productconfig;
            ROBOT = new tsdFanuc();

            //Setting up Robot to Model Events
            ROBOT.updateCyclesInView += ROBOT_updateCyclesInView;
            ROBOT.robotConnectionStatus += myView_robotConnectStatus;
            ROBOT.ftpDownloadDataFile += myView_ftpTestButton;
            ROBOT.getCalibrationData += getCalibrationRawData;
            ROBOT.getLiftOffValue += ROBOT_getLiftOffValue;

            //Connect Start Here
            RFilesCopy(stationconfig);
            
            //Graph Raw Data -> Controller -> Graph and DataGridView
            dataCollection = new DataBuilder(stnConfig.i386Path, stnConfig.RInstallerPath);

            //FTP Instantiation
            myFTP = new FTP(stnConfig.Fanuc_Ipaddress, stnConfig.fanucUsername, stnConfig.fanucPassword);

            //include robot properties
            ROBOT.pcCyclesToDo = productconfig.NumOfCycles;
            ROBOT.pcCalibrate = productconfig.CalibrateEvery;
            ROBOT.PcCompletedCycles = productconfig.CompletedCycles;
            ROBOT.robot_SLIDER_CYCLES_registerName = stationconfig.RegCyclesName;
            ROBOT.robot_SLIDER_COMPLETED_registerName = stationconfig.RegCyclesCompletedName;
            ROBOT.robot_LIFT_registerName = stationconfig.RegLiftOffName;
            ROBOT.pcLiftOffPoint = productconfig.liftOffPoint;
            ROBOT.tpChar = stationconfig.TPCharacterizationName;
            ROBOT.tpSlider = stationconfig.TPSliderCycleName;
            

            //MAIN ROBOT CONNECTION
            ROBOT.connect(stnConfig.Fanuc_Ipaddress);

            //connect Robot API Events
            ROBOT.mobjTasks.Change += new ITasksEvents_ChangeEventHandler(mobjTasks_Change);
            ROBOT.mobjNumericRegisters.Change += new IVarsEvents_ChangeEventHandler(register_Change);

            //Set up Progress Bar - the first time you connect
            updateProgressBar(0);

            
            
        }

        private void register_Change(ref object Var)
        {
            try
            {
                FRCVar register = (FRCVar)Var;
                FRCRegNumeric reg = (FRCRegNumeric)register.Value;

                //Slider completed Register handler
                if (reg.Comment == ROBOT.robot_SLIDER_COMPLETED_registerName)
                    if (reg.Type == FRETypeCodeConstants.frIntegerType)
                        ROBOT.PcCompletedCycles = reg.RegLong;

            }
            catch (Exception registerException)
            {
                Trace.WriteLine(registerException.Message);
                Trace.WriteLine(registerException.StackTrace);
            }
        }

        private void ROBOT_getLiftOffValue(object sender, bool e)
        {
            try
            {
                ROBOT.pcLiftOffPoint = dataCollection.EvaluateLiftOffPoint(charDataView.GetValues(stnConfig.scriptDisplacementName), charDataView.GetValues(stnConfig.scriptForceName));
                Trace.WriteLine("New Lift Off Value --> " + ROBOT.pcLiftOffPoint);

                //Archive the data file
                archiveDTFiles(stnConfig);


                //update the measurement
                //myView.LogMeasurement(ROBOT.pcLiftOffPoint);
                MeasurementParameter[] measurementParameteres = 
                { 
                    new MeasurementParameter("#Cycles", ROBOT.PcCompletedCycles)
                };

                AddMeasurement(new Measurement<double>("Lift Off Point", ROBOT.pcLiftOffPoint,
                    "mm",
                    18.0,
                    28.0,
                    parameters: measurementParameteres));


            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
            }
        }

        private void myView_ftpTestButton(object sender, bool e)
        {
            //have access to FTP
            //make a method that would execute ftptestButton
            //need access to StationConfig
            FtpTestButton(stnConfig);
            
        }

        private void myView_robotConnectStatus(object sender, bool e)
        {
            //Show if you are connected or not
            //simple senddata struct back to view to check whether you are connected or not
            //execute StartButtonEnDis(e);
            //SendData<bool>(e);
            SendDataByValue<ConnectionStatus>(new ConnectionStatus(e));
            

        }

        private void ROBOT_updateCyclesInView(object sender, int e)
        {

            updateCycles = new IntStruct(e, "UpdateCycles");
            SendDataByValue<IntStruct>(updateCycles);
        }

        private void updateProgressBar(int value)
        {
            SendDataByValue<IntStruct>(new IntStruct(value, "progressBar"));
        }

        private void WriteToScreen(string p)
        {
            Trace.WriteLine(p);
        }

        private void mobjTasks_Change(FREStatusTypeConstants ChangeType, FRCTask progTask)
        {
            //Slider Change or Characterization Change
            if (progTask.CurProgram.Name == ROBOT.tpSlider)
            {
                switch (progTask.Status)
                {
                    case (FRETaskStatusConstants.frStatusIdle):
                        WriteToScreen("Idle Status: " + progTask.CurProgram.Name);
                        break;
                    case (FRETaskStatusConstants.frStatusAborted):
                        //cycleTimer.Dispose();
                        WriteToScreen("Aborted " + progTask.CurProgram.Name);

                        //Only place to stop monitor for all tasks change
                        ROBOT.mobjTasks.StopMonitor();
                        WriteToScreen("Stopping Watch Event for Slider Completed - Changes!");
                        ROBOT.StopMonitorDataRegister(ROBOT.robot_SLIDER_COMPLETED_registerName); //Stopping Monitor : Telling ActiveX robot

                        //Calibration routine
                        ROBOT.startCalibrationTPProgram();
                        break;
                    case (FRETaskStatusConstants.frStatusPaused):
                        WriteToScreen("Paused " + progTask.CurProgram.Name);
                        break;
                    default:
                        break;

                }
            }
            else if (progTask.CurProgram.Name == ROBOT.tpChar)
            {
                switch (progTask.Status)
                {
                    case (FRETaskStatusConstants.frStatusAborted):
                        //close calibration timer
                        //calibrationTimer.Dispose();

                        WriteToScreen("Aborted " + progTask.CurProgram.Name);
                        //check whether the program interrupted in the middle or not
                        //determine what kind of alarm do you usually get
                        ROBOT.mobjTasks.StopMonitor();

                        //FTP module - Robot to controller. Empty string for future use
                        //ROBOT.ftpDownloadDataFile(this, true);
                        myView_ftpTestButton(this, true);

                        //TODO 2.  Evaluate Lift off
                        //execute lift off method
                        if (ROBOT.OkToEvaluateLiftOff)
                        {
                            //Adds the dataset onto datagridview and on graph
                            //ROBOT.getCalibrationData(this, OkToEvaluateLiftOff);
                            getCalibrationRawData(this, ROBOT.OkToEvaluateLiftOff);
                            Trace.WriteLine("Data Extraction Process Completed");

                            //Register LIft off update
                            //ROBOT.getLiftOffValue(this, true);
                            ROBOT_getLiftOffValue(this, true);
                            Trace.WriteLine("Lift Off Module Completed");
                        }
                        ROBOT.calibrateOrEndCycles();
                        break;
                    case (FRETaskStatusConstants.frStatusRun):
                        int currentLine = progTask.CurLine;
                        int result = ((int)(((float)currentLine / (float)ROBOT.tpChar_lines) * 100));
                        //progressBar.Report(result);
                        updateProgressBar(result);

                        break;
                    default:
                        break;
                }
            }
        }


        protected override void ExecuteTest(CancellationToken token, StationConfig stationConfig, ProductConfig productConfig, TestConfig testConfig)
        {
           
            Trace.WriteLine("Please press Start to start testing");

            //StartButton
            ROBOT.startSliderTest();

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

        private void getCalibrationRawData(object sender, bool e)
        {
            try
            {
                if (e)
                {
                    var parameters = new Dictionary<string, SymbolicExpression>();
                    parameters.Add(stnConfig.scriptParameterName, dataCollection.directory(stnConfig.PCDataFolderPath));
                    DataFrame charDataMatrix = dataCollection.GetLiftOffRawData(stnConfig.scriptLocation, stnConfig.scriptFunctionName, parameters);
                    //SendData<dataFrameContainer>(new dataFrameContainer(charDataMatrix));
                    SendDataByReference<DataFrame>(new TsdLib.DataContainer<DataFrame>(charDataMatrix));
                }
                else
                    Trace.WriteLine("Unable to evaluate Lift off");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
            }
        }

        private void FtpTestButton(StationConfig stationconfig)
        {
            if (myFTP != null)
            {
                //produces Robot listed directory
                Trace.WriteLine("Getting my list under : " + stationconfig.ForceFilePath);
                List<string> temp = myFTP.GetFileList(stationconfig.ForceFilePath);

                if (temp == null)
                {
                    Trace.WriteLine(
                        "Could Not Connect to Host Using " +
                        stationconfig.fanucUsername +
                        " Username" + stationconfig.fanucUsername +
                        " And " + stationconfig.fanucPassword +
                        " :" + stationconfig.ForceFilePath);

                    Trace.WriteLine(
                        "IF its NULL, it means: It couldn't connect anonymously " +
                        "Or Could not connect using Force File Path Setting : " +
                        stationconfig.ForceFilePath);

                    if (ROBOT != null)
                        ROBOT.OkToEvaluateLiftOff = false;
                }
                else
                {
                    //finding out the maximum file by sorting and downloading this specific file
                    //var sortedOrder = temp.OrderByDescending<string, int>(s => int.Parse(s.Replace("fsdt", "")));
                    //string fileName = sortedOrder.ElementAt(1);
                    int MaxIndex = temp.IndexOf(temp.Max<string>());
                    string fileName = temp.ElementAt(MaxIndex - 1);
                    Trace.WriteLine("Most Updated DT File is : " + fileName);
                    if (fileName.Contains("9999"))
                        Trace.WriteLine("Reached MAXIMUM FILE. " +
                            "Please not the lift off values will not be determined anymore" +
                            ".  Please delete from the folder " + stationconfig.ForceFilePath);

                    //Download this file
                    myFTP.Download(stationconfig.ForceFilePath, fileName, stationconfig.PCDataFolderPath);
                    Trace.WriteLine(fileName + " downloaded in local drive : " + stationconfig.PCDataFolderPath);
                    if (ROBOT != null)
                        ROBOT.OkToEvaluateLiftOff = true;
                }
            }

        }
    }
}
