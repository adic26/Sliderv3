using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FRRobot;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using sliderv2.Exceptions;

namespace TSD_Slider.Instruments
{
    public class tsdFanuc
    {
        #region Robot Variables Initializations
        //Public and Private ActiveX Robot Functions
        public FRCRobot mobjRobot;
        public FRCAlarms mobjAlarms;
        private FRCTask currTaskProgram;
        public FRCRegNumeric reg;
        public FRCVars mobjNumericRegisters;
        public FRCTPProgram objProgram;
        public FRCTasks mobjTasks;
        private FRCPrograms allMyProg;
        private FRCSysPositions allSysRegisters;
        public enum IncrementDirection { DOWN, UP };
        public Dictionary<string, int> copyRegisters;
        public Dictionary<string, FRCRegNumeric> dataRegisters;
        public Dictionary<string, string> dataRegisterNameToValue;
        public string robot_SLIDER_CYCLES_registerName;
        public string robot_SLIDER_COMPLETED_registerName;
        public string robot_LIFT_registerName;
        public int pcCyclesToDo, pcCalibrate, pcCompletedCycles;
        public event EventHandler<int> updateCyclesInView;
        public event EventHandler<bool> robotConnectionStatus;
        public event EventHandler<bool> ftpDownloadDataFile;
        public event EventHandler<bool> getCalibrationData;
        public event EventHandler<bool> getLiftOffValue;
        private IProgress<int> progressBar;
        public bool OkToEvaluateLiftOff = false;
        private Timer cycleTimer;
        private Timer calibrationTimer;
        private string IPAddress;

        public int PcCompletedCycles
        {
            get { return pcCompletedCycles; }
            set
            {
                pcCompletedCycles = value;
                //setup event
                if (updateCyclesInView != null)
                {
                    updateCyclesInView(this, pcCompletedCycles);
                }



            }
        }
        public double pcLiftOffPoint;
        public string tpChar, tpSlider;
        public int tpChar_lines;
        private bool isConnected = false;

        public bool IsConnected
        {
            get { return isConnected; }
            set
            {

                isConnected = value;

                if (robotConnectionStatus != null)
                    robotConnectionStatus(this, isConnected);

            }
        }

        #endregion



        /// <summary>
        /// constructor for controller
        /// </summary>
        public tsdFanuc()
        {
            copyRegisters = new Dictionary<string, int>();
            dataRegisters = new Dictionary<string, FRCRegNumeric>();
            dataRegisterNameToValue = new Dictionary<string, string>();

        }

        public void setupProgress(IProgress<int> _prog)
        {
            Trace.WriteLine("Progress Bar Linked within Robot");
            progressBar = _prog;
        }

        #region Fanuc Robot Code

        /// <summary>
        /// Users that wants to disconnect the robot
        /// </summary>
        public void disconnect()
        {
            //deleting all copies of registers
            if (copyRegisters.Keys.Count != 0)
            {
                foreach (string copies in copyRegisters.Keys)
                    DeletePosition(copies);
            }

            dataRegisters.Clear();
            dataRegisterNameToValue.Clear();

            /* Unsubscribing to the alarm event */
            mobjAlarms.AlarmNotify -= new IAlarmNotify_AlarmNotifyEventHandler(mobjAlarms_AlarmNotify);
            mobjTasks.Change -= new ITasksEvents_ChangeEventHandler(mobjTasks_Change);
            mobjRobot.Error -= new IRobotEvents_ErrorEventHandler(robotError);
            allMyProg.Select -= new IProgramsEvents_SelectEventHandler(ProgramSelected);
            allSysRegisters.Change -= new ISysPositionsEvents_ChangeEventHandler(mobjSysPosition_Change);
            mobjTasks.Change -= new ITasksEvents_ChangeEventHandler(mobjTasks_Change);

            //updating connection status
            IsConnected = false;

            Trace.WriteLine("Disconnected");

            /* Releasing the object and calling GC on them */
            releaseObjects();


        }

        /// <summary>
        /// Initial Connection to Fanuc 200ic Robot
        /// </summary>
        /// <param name="ipaddress">Static IP-ADDRESS example: 192.168.1.129</param>
        public void connect(string ipaddress)
        {
            try
            {

                this.IPAddress = ipaddress;
                mobjRobot = new FRCRobot();
                mobjRobot.Error += new IRobotEvents_ErrorEventHandler(robotError);
                mobjRobot.Connect(ipaddress);

                /* Subscribing to the alarm event */
                mobjAlarms = mobjRobot.Alarms;
                mobjAlarms.AlarmNotify += new IAlarmNotify_AlarmNotifyEventHandler(mobjAlarms_AlarmNotify);

                //Subscribing to Tasks Change Event
                //Place Holders for Task Change Start and Stop Monitor are described in : 
                //1.  startSliderTPProgram
                //2.  Under Abort Status under mobjTasks_Change method
                mobjTasks = mobjRobot.Tasks;
                mobjTasks.Change += new ITasksEvents_ChangeEventHandler(mobjTasks_Change);

                //All Programs Currently Listed
                allMyProg = mobjRobot.Programs;
                allMyProg.Select += new IProgramsEvents_SelectEventHandler(ProgramSelected);
                //txtCurrProgram.Text = allMyProg.Selected;

                //All system registers currently listed
                allSysRegisters = mobjRobot.RegPositions;
                allSysRegisters.Change += new ISysPositionsEvents_ChangeEventHandler(mobjSysPosition_Change);

                //Selecting all data registesr
                mobjNumericRegisters = mobjRobot.RegNumerics;

                //monitor register SLIDER_COMPLETED
                mobjNumericRegisters.Change += new IVarsEvents_ChangeEventHandler(register_Change);

                //Find all register names
                findAllDataRegisters();

                //Updating connection status
                IsConnected = true;

                //Writing on screen the last error and alarm from the robot
                Trace.WriteLine(string.Format("{0} {1}", mobjAlarms[0].ErrorMnemonic, mobjAlarms[0].ErrorMessage));



            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                Trace.WriteLine(ex.ErrorCode + " - " + ex.Message);
                Trace.WriteLine(ex.StackTrace);
                releaseObjects();

            }
            catch (Exception connectException)
            {
                Trace.WriteLine(connectException.Message + " - HRESULT:" + connectException.HResult.ToString());
                releaseObjects();
            }
        }

        public void startSliderTest()
        {
            #region Slider Logic
            /*
             * Start the Slider Test:
             *  TODO: Disable Button()
             *  update regsiter values to the robot with 
             *                      SLIDER_CYCLES = pcCompletedCycles + pcCalibrate (0+100)
             *                      SLIDER_COMPLETED = pcCompletedCycles (0)
             *                      
             *  if (SLIDER_COMPLETED >= SLIDER_CYCLES) QUIT!  (0 >= 100) false
             *  
             *  Start Monitor for SLIDER_COMPLETED (start monitor )
             *                      local_SLIDER_COMPLETED --> SLIDER_COMPLETED; (1...99..100..101...999,1000)
             *                      every interrupt logic : local_SLIDER_COMPLETED == SLIDER_CYCLES? (1..99..100==100, 101==200,....999==1000,1000==1000)
             *                          YES: 
             *                              if (SLIDER_COMPLETED < pcCyclesToDo) (1000 = 1000)
                 *                              Start Characterization Test 
                 *                              Update Lift off register
                 *                              Update register: robot_SLIDER_CYCLES = SLIDER_CYCLES + pcCalibrate (1000 + 100) 1100
                 *                              Update variable: SLIDER_CYCLES = SLIDER_CYCLES + pcCalibrate (1000) 1100
             *                                  Start Slider Test (slider_cycle.tp)
             *                              else
             *                                  Stop Monitor
             *                                  De-prioritize event
             *                                  WrittoScreen(done)
             *                                  Enable Button()
             *                          NO: 
             *                              update slider completed every cycle to textbox
             *                              update pcCompletedCycles (0,1...99,999)
             *                              
             * Start the monitor for Register Boolean Characterization 
             * start the characterization (char test)
             * Once the value changes:
             *      get the liftoff value (lift off value)
             *          Get the latest DT file from the associated folder (Folder location in Memory Card --> add it to StationConfig)
             *          Copy the DT file into the local folder
             *          R programming to plot all the DT files in one shot
             *          
             *      update the liftoff register (register update)
             *      start the slider cycles (start slider_cycle.tp)
             *  
             * 
             * 
             *  QUIT THE PROGRAM! END (done)
             * 
             */
            #endregion

            //TODO Disable Buttons
            try
            {
                //Update Register values to the robot and copy local values
                //updateDataRegister(robot_SLIDER_CYCLES_registerName, (PcCompletedCycles + pcCalibrate));
                //updating slider cycles to 0 at the baseline
                updateDataRegister(robot_SLIDER_CYCLES_registerName, 0);
                updateDataRegister(robot_SLIDER_COMPLETED_registerName, PcCompletedCycles);
                updateDataRegister(robot_LIFT_registerName, pcLiftOffPoint);
            }
            catch (InvalidCastException ex)
            {
                WriteToScreen(ex.Message);
                WriteToScreen(ex.StackTrace);
            }

            //Before selecting the program - make sure to abort any programs if running any
            mobjTasks.AbortAll(true);



            //Start off with calibration program
            startCalibrationTPProgram();


        }

        private void timer_StartCycleTimer(int timeInMS)
        {
            TimerCallback tcb = new TimerCallback(checkCycleRobotStatus);
            if (cycleTimer == null)
                cycleTimer = new Timer(tcb, "Checking slider Connection Status...", timeInMS, timeInMS);
            else
            {
                Trace.WriteLine("Disposing Slider Timer");
                cycleTimer.Dispose();
                cycleTimer = new Timer(tcb, "Checking slider connection status...", timeInMS, timeInMS);
            }
        }

        public void checkCycleRobotStatus(object obj)
        {
            if (mobjRobot.IsConnected)
            {
                Trace.WriteLine("Cycle Timer Check Status : " + true.ToString());
            }
            else
            {
                Trace.WriteLine("Connection got disconnected ....Restarting connection");
                this.connect(IPAddress);

                Trace.WriteLine("Resetting monitoring events :  Register Event + taske_change event");
                //Restarting calibration routine
                if (pcCompletedCycles % pcCalibrate != 0)
                {
                    Trace.WriteLine("Restarting Slider Cycle Monitoring...");
                    mobjTasks.StartMonitor(2400); //task change event
                    monitorRegister(robot_SLIDER_COMPLETED_registerName); //register change event

                }
                else
                    Trace.WriteLine("Error: Slider Timed Out");

            }

        }

        private void timer_StartCalibrationTimer(int timeInMS)
        {
            TimerCallback tcb = new TimerCallback(checkCalibrationStatus);
            if (calibrationTimer == null)
                calibrationTimer = new Timer(tcb, "Checking Calibration Connection Status...", timeInMS, timeInMS);
            else
            {
                Trace.WriteLine("Disposing Calibration Timer");
                calibrationTimer.Dispose();
                calibrationTimer = new Timer(tcb, "Checking Calibration Connection Status...", timeInMS, timeInMS);
            }
        }

        private void checkCalibrationStatus(object state)
        {
            Trace.WriteLine(state.ToString());
            if (mobjRobot.IsConnected)
            {
                //do nothing
                Trace.WriteLine("Calibration Timer Check Status : " + true.ToString());
            }
            else
            {
                Trace.WriteLine("Connection got disconnected ....Restarting connection");
                this.connect(IPAddress);

                //check if the calibration routine has stopped
                if (mobjTasks[tpChar].Status == FRETaskStatusConstants.frStatusRun)
                {
                    //the program is running and hence you should just start the monitor and pick it up
                    //Restarting calibration routine
                    Trace.WriteLine("Setting up Calibration Monitor progress again");
                    mobjTasks.StartMonitor(1500);
                }
                else if (mobjTasks[tpChar].Status == FRETaskStatusConstants.frStatusAborted)
                {
                    //mobjTasks_Change(mobjTasks[tpChar].Status, mobjTasks[tpChar]);
                    Trace.WriteLine("Trace has aborted.... re starting calibration routine");
                    mobjTasks.AbortAll();
                    Task.Run(() => startCalibrationTPProgram());
                }
            }
        }

        private void startSliderTPProgram()
        {
            try
            {
                selectProgram(tpSlider);

                if (mobjTasks == null)
                    mobjTasks = mobjRobot.Tasks;

                //Only place to start monitoring Tasks Change
                if (!mobjTasks.IsMonitoring)
                    mobjTasks.StartMonitor(2400);

                ExecuteProgram_ContinousMove();


            }
            catch (TPProgramNotFoundException notFound)
            {
                Trace.WriteLine(notFound.Message);
                Trace.WriteLine(notFound.StackTrace);
                Trace.WriteLine("Cannot find file to load");
            }
            catch (Exception ex)
            {
                WriteToScreen(ex.Message);
                WriteToScreen(ex.StackTrace);
                disconnect();
            }
        }

        private void startCalibrationTPProgram()
        {
            try
            {

                //start calibration timer
                timer_StartCalibrationTimer(30000);

                //choosing the program
                selectProgram(tpChar);

                //getting the number of lines for progress bar
                if (objProgram != null)
                {
                    //making the progressbar absolutely 0 value
                    progressBar.Report(0);
                    tpChar_lines = objProgram.Lines.Count;
                }

                if (!mobjTasks.IsMonitoring)
                    mobjTasks.StartMonitor(1500);

                WriteToScreen("Executing Calibration Routine");
                ExecuteProgram_ContinousMove();

                //Initiate a new timer here

            }
            catch (TPProgramNotFoundException notFound)
            {
                Trace.WriteLine(notFound.Message);
                Trace.WriteLine("Cannot find file to load");
            }
            catch (Exception ex)
            {
                WriteToScreen(ex.Message);
                WriteToScreen(ex.StackTrace);
                disconnect();
            }
        }

        /// <summary>
        /// Finds all the data registers in the controller
        /// Copies into data registers "dataRegisters" dictionary(string,FRCRegNumeric)
        /// </summary>
        private void findAllDataRegisters()
        {
            if (dataRegisters.Count == 0)
            {
                foreach (FRCVar reg in mobjNumericRegisters)
                {
                    if (reg.IsInitialized)
                    {
                        FRCRegNumeric register = (FRCRegNumeric)reg.Value;

                        //Key , value pair. Where key is variable name and value is the actual register
                        dataRegisters.Add(reg.VarName, register);

                        //cannot add comment because it is absolutely blank
                        if (register.Comment != "")
                        {
                            dataRegisterNameToValue.Add(register.Comment, reg.VarName);
                            Trace.WriteLine("Found " + register.Comment);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Updates register values inside the robot
        /// </summary>
        /// <param name="registerName">K-Sensitive register name as displayed in robot</param>
        /// <param name="registerValue">Integer or Double value to update the register</param>
        /// <exception cref="Invalid Cast Exception">Invalid casting exception will be thrown if register expects a float but gets an int32 or visaversa</exception>
        /// <exception cref="General Exception">Cannot find data register from the robot. Note: Register is K-Sensitive</exception>
        public void updateDataRegister(string registerName, object registerValue)
        {
            try
            {
                //find appropriate data register value
                string _registerName = dataRegisterNameToValue[registerName];
                if (dataRegisters.ContainsKey(_registerName))
                {
                    FRCRegNumeric currReg = dataRegisters[_registerName];
                    Trace.WriteLine("Type for " + registerName + " is " + currReg.Type.ToString());
                    //Copying integer values
                    if (currReg.Type == FRETypeCodeConstants.frIntegerType && registerValue.GetType() == typeof(int))
                    {
                        currReg.RegLong = (int)registerValue;
                        WriteToScreen(currReg.Comment + " : Set to " + registerValue.ToString());
                    }
                    //copying single values
                    else if (currReg.Type == FRETypeCodeConstants.frRealType && registerValue.GetType() == typeof(double))
                    {
                        currReg.RegFloat = Convert.ToSingle(registerValue);
                        WriteToScreen(currReg.Comment + " : Set to " + registerValue.ToString());
                    }
                    else
                        throw new InvalidCastException("cannot update register. Invalid type.");

                    if (currReg.Parent.NoUpdate)
                        currReg.Parent.Update();

                    Trace.WriteLine("Updated " + currReg.Comment + " to : " +
                        (currReg.Type == FRETypeCodeConstants.frIntegerType ? currReg.RegLong.ToString() : currReg.RegFloat.ToString()));

                }
                else
                    Trace.WriteLine("Cannot find " + registerName + " in data register list in this robot");
            }
            catch (Exception ex3)
            {
                Trace.WriteLine(ex3.Message);
                Trace.WriteLine(ex3.StackTrace);
            }
        }

        /// <summary>
        /// Starts throwing events from registers once it has been changed
        /// </summary>
        /// <param name="registerName">Register Name to Monitor</param>
        /// <returns>FRCRegNumeric: Data Numeric Register</returns>
        public FRCRegNumeric monitorRegister(string registerName)
        {
            registerName = dataRegisterNameToValue[registerName];

            FRCVar selectedRegister = dataRegisters[registerName].Parent;

            //Scanning every 1000ms on the FRCVar object
            selectedRegister.StartMonitor(1000);

            return (FRCRegNumeric)selectedRegister.Value;

        }

        /// <summary>
        /// Stop throwing events from registers - started from MonitorRegister
        /// </summary>
        /// <param name="registerName">Register Name to UN-Monitor</param>
        public void StopMonitorDataRegister(string registerName)
        {
            registerName = dataRegisterNameToValue[registerName];
            FRCVar selectedRegister = dataRegisters[registerName].Parent;
            if (selectedRegister.IsMonitoring)
            {
                selectedRegister.StopMonitor();
            }
        }

        /// <summary>
        /// Stop throwing events from registers - started from MonitorRegister
        /// </summary>
        /// <param name="register">FRCVar: Taken from RegNumerics from mobjNumericRegsiter to un-monitor</param>
        public void StopMonitorDataRegister(FRCVar register)
        {
            if (register.IsMonitoring)
                register.StopMonitor();

        }

        /// <summary>
        /// Stop throwing events from registers - started from MonitorRegister
        /// </summary>
        /// <param name="register">FRCRegNumeric: Numeric Register to un-monitor</param>
        public void StopMonitorDataRegister(FRCRegNumeric register)
        {
            if (register.Parent.IsMonitoring)
                register.Parent.StopMonitor();
        }



        /// <summary>
        /// Event of Data Register Change.
        /// Event should have logic on what to do when the slider program has ended
        /// </summary>
        /// <param name="Var">FRCVar</param>
        private void register_Change(ref object Var)
        {
            try
            {
                FRCVar register = (FRCVar)Var;
                FRCRegNumeric reg = (FRCRegNumeric)register.Value;

                //Slider completed Register handler
                if (reg.Comment == robot_SLIDER_COMPLETED_registerName)
                    if (reg.Type == FRETypeCodeConstants.frIntegerType)
                        PcCompletedCycles = reg.RegLong;

            }
            catch (Exception registerException)
            {
                Trace.WriteLine(registerException.Message);
                Trace.WriteLine(registerException.StackTrace);
            }
        }

        private void calibrateOrEndCycles()
        {
            // if current completed cycles < full series cycles 
            if ((PcCompletedCycles < pcCyclesToDo) && (PcCompletedCycles % pcCalibrate == 0))
            {

                updateDataRegister(robot_LIFT_registerName, evaluateLiftOff());

                //update register robot_SLIDER_CYCLES = SLIDER_CYCLES + pcCalibrate
                updateDataRegister(robot_SLIDER_CYCLES_registerName, (dataRegisters[dataRegisterNameToValue[robot_SLIDER_CYCLES_registerName]].RegLong + pcCalibrate));

                //re-Start monitor slider cycles completed register
                monitorRegister(robot_SLIDER_COMPLETED_registerName); //Only place to start monitoring slider_completed

                //Start new timer thread
                timer_StartCycleTimer(30000);

                //Run the  new program again with the updated register values
                Task.Run(() => this.startSliderTPProgram());

            }
            else if (pcCompletedCycles >= pcCyclesToDo)
            {
                WriteToScreen("Finished!");
                disconnect();
            }
            else
            {
                WriteToScreen("Calibration must have failed: CompletedCycles Mod CalibrateEvery does not equate to 0");
                disconnect();

            }
        }

        private double evaluateLiftOff()
        {

            return pcLiftOffPoint;
        }

        /// <summary>
        /// This finds all copied registers and throws it in dictionary
        /// </summary>
        private void findAllCopiedRegisters()
        {
            //Find all copied registers
            foreach (FRCSysPosition pos in allSysRegisters)
            {
                if (pos.IsInitialized)
                {
                    if (pos.Group[1].Comment.Contains("COPY_"))
                        copyRegisters.Add(pos.Group[1].Comment, pos.Id);
                }
            }
        }

        /// <summary>
        /// Notification to user whether the register value has been changed or not
        /// </summary>
        /// <param name="Id">ID for Register System</param>
        /// <param name="GroupNum">Group number for system register</param>
        private void mobjSysPosition_Change(int Id, short GroupNum)
        {
            Trace.WriteLine("System Position Changed " + GroupNum.ToString() + " " + Id.ToString());
        }

        /* Fully releasing COM objects requires waiting for garbage collection */
        private void releaseObjects()
        {

            WriteToScreen("Stopping Watch Event for Slider Completed - Changes!");
            StopMonitorDataRegister(robot_SLIDER_COMPLETED_registerName); //Stopping Monitor : Telling ActiveX robot

            WriteToScreen("Stopping Event - Devoking event handlers");
            mobjNumericRegisters.Change -= register_Change; //Register Event Devoking

            WriteToScreen("Stopping Monitor for Task Change Events");
            if (mobjTasks.IsMonitoring) mobjTasks.StopMonitor();


            mobjRobot = (FRCRobot)releaseObject("FRRobot.FRCRobot", mobjRobot);
            mobjAlarms = (FRCAlarms)releaseObject("FRRobot.FRCAlarms", mobjAlarms);
            System.GC.Collect();




        }

        /* Wrap object release in Try-Catch for enhanced diagnostics */
        private object releaseObject(string objectName, object item)
        {
            try
            {
                item = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error releasing {0}.{1}Error: {2}", objectName, Environment.NewLine, ex.Message);
            }

            return null;
        }

        private void ProgramSelected()
        {
            WriteToScreen("Program has been selected --> " + allMyProg.Selected);
        }

        private void WriteToScreen(string p)
        {
            Trace.WriteLine(p);
        }

        private void mobjTasks_Change(FREStatusTypeConstants ChangeType, FRCTask progTask)
        {
            //Slider Change or Characterization Change
            if (progTask.CurProgram.Name == tpSlider)
            {
                switch (progTask.Status)
                {
                    case (FRETaskStatusConstants.frStatusIdle):
                        WriteToScreen("Idle Status: " + progTask.CurProgram.Name);
                        break;
                    case (FRETaskStatusConstants.frStatusAborted):
                        cycleTimer.Dispose();
                        WriteToScreen("Aborted " + progTask.CurProgram.Name);

                        //Only place to stop monitor for all tasks change
                        mobjTasks.StopMonitor();
                        WriteToScreen("Stopping Watch Event for Slider Completed - Changes!");
                        StopMonitorDataRegister(robot_SLIDER_COMPLETED_registerName); //Stopping Monitor : Telling ActiveX robot

                        //Calibration routine
                        startCalibrationTPProgram();
                        break;
                    case (FRETaskStatusConstants.frStatusPaused):
                        WriteToScreen("Paused " + progTask.CurProgram.Name);
                        break;
                    default:
                        break;

                }
            }
            else if (progTask.CurProgram.Name == tpChar)
            {
                switch (progTask.Status)
                {
                    case (FRETaskStatusConstants.frStatusAborted):
                        //close calibration timer
                        calibrationTimer.Dispose();

                        WriteToScreen("Aborted " + progTask.CurProgram.Name);
                        //check whether the program interrupted in the middle or not
                        //determine what kind of alarm do you usually get
                        mobjTasks.StopMonitor();

                        //FTP module - Robot to controller. Empty string for future use
                        ftpDownloadDataFile(this, true);

                        //TODO 2.  Evaluate Lift off
                        //execute lift off method
                        if (OkToEvaluateLiftOff)
                        {
                            //Adds the dataset onto datagridview and on graph
                            getCalibrationData(this, OkToEvaluateLiftOff);
                            Trace.WriteLine("Data Extraction Process Completed");
                            //Register LIft off update
                            getLiftOffValue(this, true);
                            Trace.WriteLine("Lift Off Module Completed");
                        }
                        calibrateOrEndCycles();
                        break;
                    case (FRETaskStatusConstants.frStatusRun):
                        int currentLine = progTask.CurLine;
                        int result = ((int)(((float)currentLine / (float)tpChar_lines) * 100));
                        progressBar.Report(result);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Alarm Notification: Resets the alarm.
        /// </summary>
        /// <param name="alarm">Alarm object produced by alarm event</param>
        private void mobjAlarms_AlarmNotify(object alarm)
        {
            WriteToScreen(String.Format("{0} {1}", ((FRCAlarm)alarm).ErrorMnemonic, ((FRCAlarm)alarm).ErrorMessage));
            if (((FRCAlarm)alarm).ErrorMnemonic.Contains("INTP-105"))
            {
                WriteToScreen("Fault indicated!..RESETTING!");
                mobjAlarms.Reset();
            }
            //Interrupt when program is interrupted in the middle
            else if (((FRCAlarm)alarm).ErrorMnemonic.Contains("INTP-107"))
            {
                WriteToScreen("Program interrupted due to : " + ((FRCAlarm)alarm).ErrorMessage);
                WriteToScreen("Please ensure that the robot is set to Homing Postion for possible collisions before you restart the program");
                WriteToScreen("Disconnecting Robot!");
                disconnect();

            }
        }

        private void robotError(FRCRobotErrorInfo Error)
        {
            WriteToScreen(Error.Number.ToString() + "-" + Error.Description);
        }

        /// <summary>
        /// Manually Fault Reset
        /// </summary>
        public void FaultReset()
        {
            if (mobjRobot == null)
                return;
            else
                mobjAlarms.Reset();
        }

        /// <summary>
        /// Part of Homing procedure for Sanity Check
        /// </summary>
        public void Home()
        {
            if (mobjRobot != null)
            {
                //Kill the homing task if it exists
                KillTask("HOMING");

                //Move to position Register HOMING
                MoveToUnregisteredPosition("AIRGESTURE_HOMING");
            }
        }

        /// <summary>
        /// Goes to homing position register value
        /// </summary>
        /// <param name="homingRegister">Homing Register Name: Taken from Station Configuration</param>
        public void Home(string homingRegister)
        {
            if (mobjRobot != null)
            {
                //Move to position Register HOMING
                MoveToUnregisteredPosition(homingRegister);
            }
        }

        /// <summary>
        /// Selects the program. Assumes the TP program is already loaded in the Fanuc File System
        /// </summary>
        /// <param name="progName">Exact Name of the program (case sensitive)</param>
        public void selectProgram(string progName)
        {
            //
            if (mobjRobot == null)
            {
                WriteToScreen("Robot Not Connected!");
                return;
            }

            //Show what is Currently Selected
            WriteToScreen("Program Currently Selected is : " + allMyProg.Selected);

            //If program already selected then show it to the user
            if (allMyProg.Selected.Contains(progName))
            {
                //Do not load
                WriteToScreen("Program Already Selected. Skipping Select...");

                //Skip the select() portion
                objProgram = (FRCTPProgram)mobjRobot.Programs[progName];

                //End selection
                WriteToScreen("Program Selected " + progName);
            }
            else
            {
                /*
                 * if Program is not selected
                 *
                 * 
                 */

                int selectedTestIndex = -1;

                //Before loading a program, check tasks list
                WriteToScreen("Current Running Tasks: " + mobjTasks.Count.ToString());

                for (int i = 0; i < mobjTasks.Count; i++)
                {
                    //Finding the current task is the current program or not
                    if (mobjTasks[i].Name.Contains(progName))
                    {
                        WriteToScreen("Found the running task for current program: " + mobjTasks[i].Name + " INDEX: " + i.ToString());
                        //Getting the index out
                        selectedTestIndex = i;
                    }
                }

                //Program does exist in the task list
                //if it does exist in the task list then we should select that program
                if (selectedTestIndex > 0)
                {
                    currTaskProgram = mobjTasks[selectedTestIndex];
                    if (mobjRobot != null)
                    {
                        objProgram = (FRCTPProgram)mobjRobot.Programs[progName];
                        WriteToScreen("Current task " + currTaskProgram.Name + "Selected!");
                    }
                    else
                    {
                        //if the task is not in the task list then just select the program to run it
                        if (mobjRobot != null)
                            objProgram = (FRCTPProgram)mobjRobot.Programs[progName];

                        WriteToScreen("Program Selected! A New Task Will Be Created Once You Run It!");
                    }
                }
                //if it does not exist in the task list, then that program isn't loaded nor I can find it
                else if (selectedTestIndex == -1)
                {
                    //Could not find the program
                    WriteToScreen("Cannot find the program " + progName);

                    foreach (FRCProgram x in allMyProg)
                    {
                        if (x.Name.Contains(progName))
                        {
                            //found the program
                            //Load the program here
                            Trace.WriteLine("Selecting program " + progName);
                            objProgram = (FRCTPProgram)mobjRobot.Programs[progName];
                            return;
                        }
                    }

                    throw new TPProgramNotFoundException("Cannot find file " + progName);
                }
            }

            WriteToScreen("Program Selection Module Completed");
        }

        /// <summary>
        /// Condition : Make sure that program is selected before executing this method
        /// </summary>
        public void StepIntoProgram()
        {
            if (mobjRobot != null && objProgram != null)
            {
                objProgram.Run(FREStepTypeConstants.frStepTPMotion);

            }
            else if (mobjRobot == null)
            {
                throw new Exception("Robot is not connected!");
            }
            else if (objProgram == null)
            {
                WriteToScreen("ABORT STEP INTO PROGRAM: \nReason: Objprogram is null. In other words TP Program is not selected!");
                WriteToScreen("Attempting to finding the task --> " + allMyProg.Selected + " and killing it!");
                KillTask(allMyProg.Selected);


            }
        }

        /// <summary>
        /// Condition : Make sure that program is selected before executing this method
        /// </summary>
        public void ExecuteProgram_ContinousMove()
        {
            WriteToScreen("Run Program Module Begin");
            try
            {
                if (mobjRobot != null && objProgram != null)
                {
                    objProgram.Run(FREStepTypeConstants.frStepNone);

                }
                else if (mobjRobot == null)
                {
                    throw new Exception("Robot is not connected!");
                }
                else if (objProgram == null)
                {
                    WriteToScreen("ABORT STEP INTO PROGRAM: \nReason: Objprogram is null. In other words TP Program is not selected!");
                }
            }
            catch (Exception ex)
            {
                WriteToScreen(ex.Message);
                WriteToScreen(ex.StackTrace);
                WriteToScreen("Cannot Run Program...Disconnecting");
                disconnect();
            }
        }

        /// <summary>
        /// Moving to This Position
        /// </summary>
        /// <param name="registerName">Exact Name of the System Register (case sensitive)</param>
        public void MoveToUnregisteredPosition(string registerName)
        {
            if (mobjRobot == null)
            {
                WriteToScreen("Robot is not connected!");
                return;
            }

            //Finding Position Register
            FRCSysPosition positionRegister = findPositionRegister(registerName);

            //Starting to monitor value
            positionRegister.Group[1].StartMonitor(500);

            //move command
            if (positionRegister != null)
                allSysRegisters[positionRegister.Id].Moveto();
            else
            {
                WriteToScreen("Cannot find position Register --> " + registerName);
                throw new Exception("Position Register Not Found");
            }
        }


        /// <summary>
        /// Move to Registered Position within Air Gesture Application
        /// </summary>
        /// <param name="registerName">Name of the original Register that contains inside 
        /// Fanuc Robot System Registers without the copy name associated</param>
        public void MoveToRegisteredPosition(string registerName)
        {
            FRCSysPosition positionRegister;

            if (mobjRobot == null)
            {
                WriteToScreen("Robot is not connected!");
                return;
            }

            //Finding Position Register.  Finds it in dictionary and throws it in findPosition
            if (copyRegisters.ContainsKey("COPY_" + registerName))
            {
                positionRegister = findPositionRegister(copyRegisters["COPY_" + registerName]);
                MoveToPositionRegister(positionRegister);
            }
            else
                //Position Register is not in the dictionary
                WriteToScreen("This position is not registered.  Please register this system register before moving it");
        }

        /// <summary>
        /// Position Register Move (simple move)
        /// </summary>
        /// <param name="position">System Position Register</param>
        private void MoveToPositionRegister(FRCSysPosition position)
        {
            //move command
            if (position != null)
                position.Moveto();
            else
            {
                WriteToScreen("Cannot find position Register --> " + position.Group[1].Comment);
                throw new Exception("Position Register Not Found");
            }
        }

        /// <summary>
        /// Move to Position register with the adjusted Z Height. From Homing (top to bottom)
        /// </summary>
        /// <param name="currentPosition">Position Register</param>
        /// <param name="zHeightChange_In_mm">Change in Height required (in mm) example: destination = (Position.Z - Z Height in mm)</param>
        //private bool IncrementPosition(FRCSysPosition currentPosition, IncrementDirection direction, FRCSysPosition limitSystemRegister ,double zHeightChange_In_mm)
        //{
        //    if (checkReachable(currentPosition,limitSystemRegister))
        //    {
        //        FRCXyzWpr destination;
        //        //destination = (FRCXyzWpr)currentPosition.Group[1].Formats[FRETypeCodeConstants.frXyzWpr];
        //        destination = (FRCXyzWpr)currentPosition.Group[1].Formats[FRETypeCodeConstants.frXyzWpr];
        //        if (direction == IncrementDirection.DOWN)
        //        {
        //            WriteToScreen("WARNING!!! ---> MOVING ROBOT TO DOWN POSITION. (NOT RECOMMENDED DUE TO COLLISION)");
        //            destination.Z = destination.Z - zHeightChange_In_mm;
        //        }
        //        else if (direction == IncrementDirection.UP)
        //            destination.Z = destination.Z + zHeightChange_In_mm;
        //        currentPosition.Update();
        //        currentPosition.Refresh();

        //        //Moving to new position
        //        currentPosition.Moveto();
        //        return true;
        //    }
        //    else
        //    {
        //        WriteToScreen("Position not reachable to: " + limitSystemRegister.Group[1].Comment);
        //        return false;
        //    }
        //}

        /// <summary>
        /// Move to Position register with the adjusted Z Height.  From Homing (top to bottom)
        /// </summary>
        /// <param name="positionName">Position Register</param>
        /// <param name="zHeightChange_In_mm">Change in Height requried (in mm)</param>
        /// <param name="direction">UP/DOWN</param>
        /// <returns>TRUE if successful. False if position not reachable</returns>
        //public bool IncrementRegisteredPosition(string positionName, double zHeightChange_In_mm, IncrementDirection direction)
        //{
        //    return IncrementPosition(findPositionRegister(copyRegisters["COPY_" + positionName]), direction, findPositionRegister(copyRegisters["COPY_AG_LIMIT_REG"]), zHeightChange_In_mm);
        //}

        /// <summary>
        /// Move to Position register with the adjusted Z Height.  From Homing (top to bottom)
        /// </summary>
        /// <param name="positionName">Position Register</param>
        /// <param name="limitRegister">Limit Register name without the COPY_ before it</param>
        /// <param name="zHeightChange_In_mm">Change in Height requried (in mm)</param>
        /// <param name="direction">UP/DOWN</param>
        /// <returns>TRUE if successful. False if position not reachable</returns>
        //public bool IncrementRegisteredPosition(string positionName,string limitRegister, double zHeightChange_In_mm, IncrementDirection direction)
        //{
        //    return IncrementPosition(findPositionRegister(copyRegisters["COPY_" + positionName]), direction, findPositionRegister(copyRegisters["COPY_" + limitRegister]), zHeightChange_In_mm);
        //}

        /// <summary>
        /// Checks whether the current height is reachable or not i.e. it is beyond the limit
        /// </summary>
        /// <param name="currentPosition">Current Position Register where the position is currently existing</param>
        /// <param name="limitRegister">Limit register</param>
        /// <returns>True if is reachable, else false</returns>
        //private bool checkReachable(FRCSysPosition currentPosition, FRCSysPosition limitRegister)
        //{
        //    if (limitRegister == null || currentPosition == null)
        //    {
        //        throw new Exception("Limit Register or Current Position register is NULL");
        //    }
        //    else
        //    {
        //        FRCXyzWpr currHeight = (FRCXyzWpr)currentPosition.Group[1].Formats[FRETypeCodeConstants.frXyzWpr];
        //        FRCXyzWpr limHeight = (FRCXyzWpr)limitRegister.Group[1].Formats[FRETypeCodeConstants.frXyzWpr];

        //        return ((currHeight.Z < limHeight.Z) ? true : false);
        //    }
        //}

        /// <summary>
        /// Aborts all the current motion
        /// </summary>
        public void AbortMotion()
        {
            if (mobjRobot != null && objProgram != null)
            {
                //aborts all the current motion
                mobjTasks.AbortAll(true);
                //Kill Current Task if exists

            }
            else if (mobjRobot == null)
            {
                throw new Exception("Robot is not connected!");
            }
            else if (objProgram == null)
            {
                WriteToScreen("ABORT STEP INTO PROGRAM: \nReason: Objprogram is null. In other words TP Program is not selected!");
            }
        }

        /// <summary>
        /// Kills a desired task (Name of TP Program)
        /// </summary>
        /// <param name="TPprogName">Exact Name of the TPprogram (case sensitive)</param>
        public void KillTask(string TPprogName)
        {
            try
            {
                if (mobjRobot != null)
                {
                    //Updating mobjTasks one more time before killing a task
                    mobjTasks = mobjRobot.Tasks;
                }

                int _index = -1;
                //Program already exists!
                foreach (FRCTask x in mobjTasks)
                {
                    _index++;

                    if (x.Name == TPprogName)
                    {
                        WriteToScreen("Aborting current Task of " + mobjTasks[_index].Name);
                        //Abort the current execution of the task
                        mobjTasks[_index].Abort(true);

                        //Task Killed Message to User
                        WriteToScreen("Task Killed!");
                    }

                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                WriteToScreen(ex.Message);
            }
            catch (Exception killTask)
            {
                WriteToScreen(killTask.Message);
            }
        }

        /// <summary>
        /// Finds position register and returns to the client
        /// </summary>
        /// <param name="registerName">Exact (k-sensitive) name of the position register</param>
        /// <returns>
        /// FRCSysPosition:  if the position register name matches exactly
        /// NULL : if it does not match
        /// </returns>
        private FRCSysPosition findPositionRegister(string registerName)
        {
            foreach (FRCSysPosition pos in allSysRegisters)
            {
                if (pos.IsInitialized)
                {
                    if (pos.Group[1].Comment == registerName)
                    {
                        //return position register
                        return pos;
                    }
                }
            }
            return null;

        }

        /// <summary>
        /// Find Position Register
        /// </summary>
        /// <param name="ID">Unique ID where the position register exists</param>
        /// <returns>Returns the Position Register</returns>
        private FRCSysPosition findPositionRegister(int ID)
        {
            return allSysRegisters[ID];
        }


        /// <summary>
        /// Copies the register position
        /// </summary>
        /// <param name="registerPosition">Name of the register that you want to copy</param>
        /// <returns>returns the ID where it copied to.  
        /// Returns -1 if does not find any space.
        /// Returns -2 if it has not been initialized</returns>
        private int copyPosition(FRCSysPosition registerPosition)
        {
            foreach (FRCSysPosition pos in allSysRegisters)
            {
                if (!pos.IsInitialized)
                {
                    WriteToScreen("Found an uninitialized position to copy to --> " + pos.Id);
                    WriteToScreen("Copying " + registerPosition.Group[1].Comment + "...");
                    WriteToScreen("Copying from" + registerPosition.Id + " to " + pos.Id);

                    //copy
                    allSysRegisters[pos.Id].Copy(allSysRegisters[registerPosition.Id]);
                    //rename
                    allSysRegisters[pos.Id].Group[1].Comment = "COPY_" + registerPosition.Group[1].Comment;

                    allSysRegisters.Update();
                    allSysRegisters.Refresh();

                    //Validating whether the new position has been initialized
                    if (allSysRegisters[pos.Id].IsInitialized)
                        WriteToScreen("Register has been successfully copied");
                    else
                        return -2;

                    //Key register
                    WriteToScreen("Copy Register added to Dictionary");
                    copyRegisters.Add(allSysRegisters[pos.Id].Group[1].Comment, pos.Id);

                    return pos.Id;
                }
            }
            return -1;
        }

        /// <summary>
        /// Deletes a copied register
        /// </summary>
        /// <param name="copiedRegister">FRCSystemRegister COPY_origRegister</param>
        /// <returns>TRUE if it has been deleted, FALSE it is not!</returns>
        private bool DeletePosition(string copiedRegister)
        {
            int id = copyRegisters[copiedRegister];
            allSysRegisters[id].Uninitialize();
            allSysRegisters[id].Update();
            allSysRegisters.Refresh();

            return !allSysRegisters[id].IsInitialized;
        }

        /// <summary>
        /// Registers homing position in the dictionary.  Throws an exception if the dictionary contains a key
        /// </summary>
        /// <param name="HomingPosition">Position Register from Station Configuration</param>
        /// <returns>ID of the copy register that you now need to work with.  If ERROR then -1</returns>
        public int register(string HomingPosition)
        {
            try
            {
                //Find out whether the key and value already exists
                if (copyRegisters.ContainsKey("COPY_" + HomingPosition)) return copyRegisters["COPY_" + HomingPosition];

                //Finding the copied position and its ID from the system registers. 
                //Returns NULL if it cannot find the position
                FRCSysPosition position = this.findPositionRegister("COPY_" + HomingPosition);

                //cannot find the copy
                if (position == null)
                {
                    //Copies Reigster and add it in dictionary
                    int ret = copyPosition(findPositionRegister(HomingPosition));
                    if (ret != -1)
                    {
                        WriteToScreen("ERROR: CANNOT REGISTER DUE TO NO SPACE LEFT IN SYSTEM REGISTERS");
                        throw new Exception("Cannot Register Positition in System Register");
                    }
                    else if (ret == -2)
                    {
                        WriteToScreen("Warning: Register has not been initialized");
                        WriteToScreen("Warning: Not copied to the dictionary. Try reconnecting to the system or manually initialize the " + "COPY_" + HomingPosition);
                        return ret;
                    }
                    else
                    {
                        return ret;
                    }
                }
                else
                {
                    //Find out whether the key and value already exists
                    if (copyRegisters.ContainsKey(position.Group[1].Comment)) return position.Id;
                    else copyRegisters.Add(position.Group[1].Comment, position.Id);
                }

                return position.Id;

            }
            //Throws a argumentexception if the dictionary already contains a key
            catch (Exception ex)
            {
                WriteToScreen(ex.Message);
                return -1;
            }

        }

        /// <summary>
        /// Deletes the copied version of the register.
        /// </summary>
        /// <param name="position">Name of the original register</param>
        public void unregister(string position)
        {
            DeletePosition("COPY_" + position);
        }

        #endregion
    }
}
