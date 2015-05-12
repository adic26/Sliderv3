using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
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
using sliderv2.Exceptions;

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
        public DataFrame characterizationData;
        public dataFrame VeniceLiftOff;
        public Timer robServer;
        public Queue<string> robotServerHits;
        public int progResultBar;
        public double tempLiftOff;

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

            ////Connect Start Here
            RFilesCopy(stationconfig);

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

            //DataCollector
            dataCollection = new DataBuilder(stnConfig.i386Path, stnConfig.RInstallerPath);


            //MAIN ROBOT CONNECTION
            ROBOT.connect(stnConfig.Fanuc_Ipaddress);

            //connect Robot API Events
            ROBOT.mobjTasks.Change += new ITasksEvents_ChangeEventHandler(mobjTasks_Change);
            ROBOT.mobjNumericRegisters.Change += new IVarsEvents_ChangeEventHandler(register_Change);

            //Set up Progress Bar - the first time you connect
            updateProgressBar(0);
            tempLiftOff = 0;



            
            
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
            computeLiftOff(e);
        }

        /// <summary>
        /// Update Measurements
        /// </summary>
        /// <param name="filename">Name of the file that you will be logging with after archive</param>
        /// <param name="optionalLogger">Default liftoff as -1</param>
        private void updateMeasurement(string filename,double LiftOff = -1)
        {
            //Log Measurement
            //Get the current data file to log
            string[] files = new string[] { filename };
            foreach (string robotFiles in files)
                Trace.WriteLine(robotFiles);

            //update the measurement
            MeasurementParameter[] measurementParameteres = { new MeasurementParameter("#Cycles", ROBOT.PcCompletedCycles) };
            AddMeasurement(new Measurement<double>("Lift Off Point", LiftOff,
                "mm", 18.0, 28.0, files: files, parameters: measurementParameteres));
        }

        private void computeLiftOff(bool e)
        {
            try
            {

                Trace.WriteLine("Computing Lift Off......");

                ROBOT.pcLiftOffPoint = dataCollection.EvaluateLiftOffPoint(
                    characterizationData,
                    stnConfig.scriptDisplacementName,
                    stnConfig.scriptForceName);

                //check flag for invalid lift off
                if ((ROBOT.pcLiftOffPoint < 10) || (ROBOT.pcLiftOffPoint > 30.0))
                    throw new LiftoffEvalException("Lift Off Force, less than 10.0mm");

                Trace.WriteLine("New Lift Off Value --> " + ROBOT.pcLiftOffPoint);
            }
            catch (LiftoffEvalException liftoff)
            {
                Trace.WriteLine(liftoff.Message);
                Trace.WriteLine(liftoff.StackTrace);

               //archive file here
               string errorDtFile = archiveDTFiles(stnConfig);

               //log usually
               updateMeasurement(errorDtFile,LiftOff: ROBOT.pcLiftOffPoint);

               //going back to default lift off setting if lift force is less than 30 < L < 10
               Trace.WriteLine("Setting back old LiftOff point");
               ROBOT.pcLiftOffPoint = tempLiftOff;
               Trace.WriteLine("Lift Off Point Set to " + tempLiftOff.ToString());
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

            if (!e)
                Trace.WriteLine("Robot Disconnected");



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
                    case(FRETaskStatusConstants.frStatusRun):
                        robotServerHits.Enqueue("HIT");
                        break;
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

                        //Stop Timer and dispose the queue
                        robServer.Dispose(); //This both stops the timer and cleans up
                        robotServerHits = null; //GC queue
                        
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
                        FtpTestButton(stnConfig);

                        //execute lift off method
                        if (ROBOT.OkToEvaluateLiftOff)
                        {
                            //Analyze Data Set
                            dataAnalyze(true);
                            Trace.WriteLine("Data Extraction/LiftOff Process Completed");
                        }

                        //Timer Start
                        Trace.WriteLine("Starting timer thread to control Robot Server");
                        //Start timer and queue
                        ServerCheck();

                        Trace.WriteLine("Calibrate or End Cycles Method Invoke");
                        ROBOT.calibrateOrEndCycles();
                        break;
                    case (FRETaskStatusConstants.frStatusRun):
                        int currentLine = progTask.CurLine;
                        int result = ((int)(((float)currentLine / (float)ROBOT.tpChar_lines) * 100));
                        //progressBar.Report(result);
                        updateProgressBar(result);
                        Trace.WriteLine(result.ToString());
                        break;
                    case (FRETaskStatusConstants.frStatusPaused):
                        Trace.WriteLine(progTask.CurProgram.Name + " is paused");
                        break;
                    case (FRETaskStatusConstants.frStatusIdle):
                        Trace.WriteLine(progTask.CurProgram.Name + " Indicates that the task is not running, is not paused, has not been running in the past, and has been aborted");
                        break;
                    default:
                        break;
                }
            }
        }

        private void ServerCheck()
        {
            robotServerHits = new Queue<string>();
            robServer = new Timer(HitEvery1Min, "HIT", 60000, 60000);
        }

        private void HitEvery1Min(object state)
        {
            //check at every hit
            if (robotServerHits.Count == 0)
            {
                bool flag = ROBOT.mobjRobot.IsConnected;
                Trace.WriteLine("Queue count is 0. Status of Connection - " + flag.ToString());

                if (!flag)
                {
                    //server has crashed
                    Trace.WriteLine("Server Has Crashed!");
                    //Send the disconnect line
                    myView_robotConnectStatus(this, false);

                    //Reinitialize Robot Server, connect Task_Change Event and Re-Start the monitor Register Start Services
                    ROBOT.connect(stnConfig.Fanuc_Ipaddress);
                    Trace.WriteLine("ReConnected!");

                    //Connect Task Change
                    Trace.WriteLine("Attaching Events with Task Change and Numeric Register Change");
                    ROBOT.mobjTasks.Change += new ITasksEvents_ChangeEventHandler(mobjTasks_Change);
                    ROBOT.mobjNumericRegisters.Change += new IVarsEvents_ChangeEventHandler(register_Change);

                    //ReStart Monitor Register Start Services
                    Trace.WriteLine("Monitoring Slider Completed Register and Task Change Events");
                    ROBOT.monitorRegister(ROBOT.robot_SLIDER_COMPLETED_registerName); //register change monitor
                    ROBOT.mobjTasks.StartMonitor(2400);

                }
                else if (flag)
                {
                    //it is connected, however the program as halted - time to restart this program from wherever it is paused
                    //Slider Program - as this is only hit during the timing HIT
                    //Clear any alarms if there are any
                    ROBOT.FaultReset(); //mobjAlarms.Reset()
                    ROBOT.ExecuteProgram_ContinousMove();
                }

                

            }
            else if (robotServerHits.Count > 0)
            {
                Trace.WriteLine("Found events in Queue -- " + robotServerHits.Count.ToString());
                Trace.WriteLine("Clearing Events...");
                robotServerHits.Clear();
            }
        }


        protected override void ExecuteTest(CancellationToken token, StationConfig stationConfig, ProductConfig productConfig, TestConfig testConfig)
        {            
            //StartButton
            ROBOT.startSliderTest();
            ROBOT.WaitForFinish.WaitOne(); //Waiting for robot to finish all cycles

            //dataAnalyze(true);
            //Thread.Sleep(10000);
            //computeLiftOff(true);

            //Execute test by passing via reference
            //getCalibrationData(true);
            //Thread.Sleep(10000);


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

        public string archiveDTFiles(StationConfig stationConfig)
        {
            try
            {
                string[] files = System.IO.Directory.GetFiles(stationConfig.PCDataFolderPath);
                string lastfile = "";

                string _tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "TsdLib");
                if (!System.IO.Directory.Exists(_tempPath))
                    System.IO.Directory.CreateDirectory(_tempPath);

                foreach (string s in files)
                {
                    string filename = System.IO.Path.GetFileName(s);
                    Trace.WriteLine(String.Format("File {0}", filename));
                    string dir = String.Format(@"{0}\Slider\archive\{1}\", _tempPath, DateTime.Now.ToString("MMM_dd_yyyy_HH_mm_ss"));

                    Trace.WriteLine(String.Format("Directory {0}", dir));

                    if (!System.IO.Directory.Exists(dir))
                    {
                        Trace.WriteLine(String.Format("Creating {0}", dir));
                        System.IO.Directory.CreateDirectory(dir);
                    }

                    string destFile = String.Format(@"{0}\{1}", dir, filename);
                    Trace.WriteLine(String.Format("Destination: {0}", destFile));

                    System.IO.File.Copy(s, destFile, true);
                    Trace.WriteLine(String.Format("File {0} Archived in {1}", filename, dir));
                    System.IO.File.Delete(s);
                    Trace.WriteLine(String.Format("Deleted: {0}", s));

                    lastfile = destFile;

                }
                return lastfile;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        private void getCalibrationRawData(object sender, bool e)
        {
            dataAnalyze(e);
        }

        private void dataAnalyze(bool e)
        {
            try
            {
                if (e)
                {
                    //setting up parameters
                    var parameters = new Dictionary<string, SymbolicExpression>();
                    parameters.Add(stnConfig.scriptParameterName,
                        dataCollection.directory(stnConfig.PCDataFolderPath));

                    //Raw Data
                    characterizationData = dataCollection.GetLiftOffRawData(stnConfig.scriptLocation,
                        stnConfig.scriptFunctionName,
                        parameters);
                    Trace.WriteLine("Evaluation Lift Off Raw Data completed");

                    if (characterizationData != null)
                    {


                        computeLiftOff(true); // no archiving
                        Trace.WriteLine("Lift Off Module Completed");

                        //get my stuct file
                        VeniceLiftOff = new dataFrame(
                            dataCollection.cycles.ToArray(),
                            dataCollection.displacement.ToArray(),
                            dataCollection.force.ToArray());

                        //Send struct back to UI
                        Trace.WriteLine("Sending Information to UI");
                        SendData(VeniceLiftOff);

                        //clean Characterization Data
                        characterizationData = null;

                    }
                    else
                    {
                        throw new DataFrameNotFoundException("unable to find dataframe");
                    }

                }
                else
                    Trace.WriteLine("Unable to evaluate Lift off");
            }
            catch (DataFrameNotFoundException dfNotFound)
            {
                Trace.WriteLine(dfNotFound.Message);
                Trace.WriteLine(dfNotFound.StackTrace);
                
                //data frame not found due to error in rdotnet
                //we must force this as a fail but preserve robot.pcliftoffpoint for next cycle
                //we must archive the file here so that we can use it
                string errorFile = archiveDTFiles(stnConfig);

                //Log with -1 error
                updateMeasurement(errorFile); //default of -1



            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);

            }
            finally
            {
                //Archive Data Here : Omit for testing only
                string fileToLog = archiveDTFiles(stnConfig);

                //Log file here except when the string is empty
                if (fileToLog!="")
                    updateMeasurement(fileToLog,LiftOff: ROBOT.pcLiftOffPoint);

                if (ROBOT.pcLiftOffPoint == 0)
                {
                    Trace.WriteLine("setting to default lift off point");
                    ROBOT.pcLiftOffPoint = prdConfig.liftOffPoint; //default lift off point
                }

                //putting old liftoffpoint in memory
                tempLiftOff = ROBOT.pcLiftOffPoint;
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
                    //var sortedOrder = temp.OrderByDescending&lt;string, int&gt;(s =&gt; int.Parse(s.Replace("fsdt", "")));
                    //string fileName = sortedOrder.ElementAt(1);
                    //List of all files
                    foreach (string dtFiles in temp)
                        Trace.WriteLine(dtFiles);
                    
                    int MaxIndex;
                    Trace.WriteLine("Finding Max Index of File List");
                   try
                   {
                       var onlyfsdt = from fsdt in temp
                                      where fsdt.Contains("fsdt")
                                      orderby fsdt
                                      select fsdt;
                       temp = onlyfsdt.ToList<string>();

                       MaxIndex = temp.IndexOf(temp.Max<string>());
                       Trace.WriteLine("Finding MaxIndex " + MaxIndex.ToString());
                       string fileName = temp.ElementAt(MaxIndex - 1);
                       Trace.WriteLine("Most Updated DT File is : " + fileName);
                       if (fileName.Contains("9999"))
                           Trace.WriteLine("Reached MAXIMUM FILE. " +
                               "Lift off values will not be determined anymore" +
                               ".  Please delete files from the folder " + stationconfig.ForceFilePath);
                        //Download this file
                        myFTP.Download(stationconfig.ForceFilePath, fileName, stationconfig.PCDataFolderPath);
                        Trace.WriteLine(fileName + " downloaded in local drive : " + stationconfig.PCDataFolderPath);
                        if (ROBOT != null)
                            ROBOT.OkToEvaluateLiftOff = true;
                       }
                       catch (Exception ex)
                       {
                           Trace.WriteLine(ex.Message);
                           Trace.WriteLine(ex.StackTrace);
                       }

                
                }
            
            }

        
        }
    
    }

}
