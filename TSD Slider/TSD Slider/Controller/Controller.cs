using System;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSD_Slider.Configuration;
using TSD_Slider.UI.Forms;
using TsdLib.Configuration;
using TsdLib.Configuration.Connections;
using TsdLib.Measurements;
using TsdLib.TestSystem.Controller;
using TsdLib.TestSystem.TestSequence;
using TSD_Slider.Instruments;
using TSD_Slider.UI.Components;
using TSD_Slider.Communication;
using RDotNet;



namespace TSD_Slider
{
    public class Controller : ControllerBase<TSD_SliderView, StationConfig, ProductConfig, TestConfig>
    {

        public static tsdFanuc ROBOT;
        public TSD_SliderView myView;
        public FTP myFTP;
        public CharData charDataView;
        public DataBuilder dataCollection;
        private IProgress<int> progressBar;
        private IProgress<DataFrame> progressData;
        private StationConfig stationconfig;
        private ProductConfig productconfig;

        public Controller(ITestDetails testDetails, IConfigConnection databaseConnection, bool localDomain)
            : base(testDetails,databaseConnection,localDomain)
        {
            //View Events
            myView = this.UI;
            
            //myView.startButton += new System.EventHandler(myView_startButton);
            //myView.stopButton += new System.EventHandler(myView_stopButton);
            //myView.connectButton += new EventHandler(myView_connectButton);
            myView.dataTestButton += getCalibrationRawData;
            myView.TESTLift += ROBOT_getLiftOffValue;

            //Robot Events
            //ROBOT = new tsdFanuc();
            //ROBOT.updateCyclesInView += ROBOT_updateCyclesInView;
            //ROBOT.robotConnectionStatus += myView_robotConnectStatus;
            //ROBOT.ftpDownloadDataFile += myView_ftpTestButton;
            //ROBOT.getCalibrationData += getCalibrationRawData;
            //ROBOT.getLiftOffValue += ROBOT_getLiftOffValue;
        }

        void myView_connectButton(object sender, System.EventArgs e)
        {
            try
            {
                if (!ROBOT.IsConnected)
                {
                    if (stationconfig == null)
                    {
                        //Local configurations
                        stationconfig = myView.stationConfig;
                        productconfig = myView.productConfig;
                        instantiations();
                    }
                }
            }
            catch (Exception ex)
            {

                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
            }
        }

        private void instantiations()
        {
            //Lift Off Instantiation and events
            dataCollection = new DataBuilder(stationconfig.i386Path, stationconfig.RInstallerPath);

            //Datagridview connection
            charDataView = myView.DataGridLiftOffData; //connecting datagridview with controllers instance

            if (progressData == null)
            {
                progressData = new Progress<DataFrame>(progMatrix =>
                {
                    charDataView.updateData(progMatrix);
                    myView.graphCharacterization(charDataView.GetValues(stationconfig.scriptDisplacementName), charDataView.GetValues(stationconfig.scriptForceName));
                });
                //connecting progress reporting with Lift Off Object
                dataCollection.setupProgress(progressData);

            }
        }

        void myView_stopButton(object sender, System.EventArgs e)
        {
            try
            {
                this.emergencyStop();
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
                    parameters.Add(stationconfig.scriptParameterName, dataCollection.directory(stationconfig.PCDataFolderPath));
                    DataFrame charDataMatrix = dataCollection.GetLiftOffRawData(stationconfig.scriptLocation, stationconfig.scriptFunctionName, parameters);
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

        void myView_startButton(object sender, System.EventArgs e)
        {
            try
            {
                this.executeSliderTest();
                myView.toggleTextBoxes();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
            }
        }

        void ROBOT_getLiftOffValue(object sender, bool e)
        {
            try
            {
                ROBOT.pcLiftOffPoint = dataCollection.EvaluateLiftOffPoint(charDataView.GetValues(stationconfig.scriptDisplacementName), charDataView.GetValues(stationconfig.scriptForceName));
                Trace.WriteLine("New Lift Off Value --> " + ROBOT.pcLiftOffPoint);

                //Archive the data file
                myView.archiveDTFiles();


                //update the measurement
                myView.LogMeasurement(ROBOT.pcLiftOffPoint);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
            }

        }

        private void myView_updateProgress(object sender, int e)
        {
            this.progressBar.Report(e);
        }

        private void myView_ftpTestButton(object sender, bool e)
        {
            tsdFanuc fanuc = sender as tsdFanuc;
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

                    if (fanuc != null)
                        fanuc.OkToEvaluateLiftOff = false;
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
                    if (fanuc != null)
                        fanuc.OkToEvaluateLiftOff = true;
                }
            }
        }

        void myView_robotConnectStatus(object sender, bool e)
        {
            myView.toggleConnStatus(e);
            myView.startButtonEnDis(e);
            myView.toggleStopButton();

            //setting up progress bar
            if (progressBar == null)
            {
                Trace.WriteLine("Initializing Progress Bar...");
                progressBar = new Progress<int>(progValue => { myView.updateProgressbar(progValue); });
                ROBOT.setupProgress(progressBar);
            }
        }

        void ROBOT_updateCyclesInView(object sender, int e)
        {
            myView.updateTextBox(e);
        }

        async private void executeSliderTest()
        {
            if (ROBOT.IsConnected)
            {
                myView.toggleStartButton();
                myView.toggleStopButton();
                await Task.Run(() => ROBOT.startSliderTest());

            }
            else
                Trace.WriteLine("Robot is not connected");
        }

        private void emergencyStop()
        {
            if (ROBOT.IsConnected)
            {
                ROBOT.AbortMotion();
                ROBOT.disconnect();
            }
        }



#if INSTRUMENT_LIBRARY
        protected override System.Collections.Generic.IEnumerable<System.CodeDom.CodeCompileUnit> GenerateAdditionalCodeCompileUnits(string nameSpace)
        {
            System.Collections.Generic.List<System.CodeDom.CodeCompileUnit> codeCompileUnits = new System.Collections.Generic.List<System.CodeDom.CodeCompileUnit>();

            if (!System.IO.Directory.Exists("Instruments"))
                return new System.CodeDom.CodeCompileUnit[0];

            string[] instrumentXmlFiles = System.IO.Directory.GetFiles("Instruments", "*.xml");
            TsdLib.InstrumentLibraryTools.InstrumentParser instrumentXmlParser = new TsdLib.InstrumentLibraryTools.InstrumentParser(nameSpace, Language.CSharp.ToString());
            codeCompileUnits.AddRange(instrumentXmlFiles.Select(xmlFile => instrumentXmlParser.Parse(new System.IO.StreamReader(xmlFile))));

            if (System.IO.Directory.Exists(@"Instruments\Helpers"))
            {
                string[] instrumentHelperCsFiles = System.IO.Directory.GetFiles(@"Instruments\Helpers", "*.cs");
                string[] instrumentHelperVbFiles = System.IO.Directory.GetFiles(@"Instruments\Helpers", "*.vb");
                TsdLib.BasicCodeParser instrumentHelperParser = new TsdLib.BasicCodeParser();
                codeCompileUnits.AddRange(instrumentHelperCsFiles.Concat(instrumentHelperVbFiles).Select(xmlFile => instrumentHelperParser.Parse(new System.IO.StreamReader(xmlFile))));
            }

            return codeCompileUnits;
        }
#endif

#if REMICONTROL
        private readonly System.Threading.Tasks.Task _webServiceInstantiateTask = System.Threading.Tasks.Task.Run(() => DBControl.Helpers.Helper.InstantiateWebServices());

        protected override void EditTestDetails(object sender, bool getFromDatabase)
        {
            if (getFromDatabase)
            {
                try
                {
                    _webServiceInstantiateTask.Wait(2000);
                    using (DBControl.Forms.Request remiForm = new DBControl.Forms.Request())
                        if (remiForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            DBControl.remiAPI.ScanReturnData batchInformation = remiForm.RemiData[0];
                            Details.TestSystemName = batchInformation.SelectedTestName;
                            string[] qraNumber = batchInformation.QRANumber.Split('-');
                            Details.RequestNumber = string.Join("-", qraNumber, 0, qraNumber.Length - 1);
                            Details.TestStage = batchInformation.TestStageName;
                            Details.TestType = batchInformation.JobName;
                            Details.UnitNumber = (uint)batchInformation.UnitNumber;
                        }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("DBControl Exception: " + ex);
                }
            }
            else
                base.EditTestDetails(sender, false);
        }

        protected override void PublishResults(TsdLib.Measurements.ITestResults results)
        {
            string xmlFile = TsdLib.SpecialFolders.GetResultsFileName(results.Details, results.Summary, "xml");
            string path = System.IO.Path.GetDirectoryName(xmlFile);
            if (path == null)
                throw new System.IO.DirectoryNotFoundException("The results folder does not exist on this machine.");
            System.Diagnostics.Trace.WriteLine("Uploading results to database...");
            DBControl.DAL.Results.UploadXML(xmlFile, path, System.IO.Path.Combine(path, "PublishFailed"), System.IO.Path.Combine(path, "Published"), false, true);
            System.Diagnostics.Trace.WriteLine("Upload complete. Results can be viewed at " + results.Details.RequestNumber);
        }
#endif

#if simREMICONTROL
        private readonly System.Threading.Tasks.Task _webServiceInstantiateTask = System.Threading.Tasks.Task.Run(() => DBControl.Helpers.Helper.InstantiateWebServices());

        protected override void EditTestDetails(object sender, bool getFromDatabase)
        {
            if (getFromDatabase)
            {
                try
                {
                    _webServiceInstantiateTask.Wait(2000);
                    using (DBControl.Forms.Request remiForm = new DBControl.Forms.Request())
                        if (remiForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            DBControl.remiAPI.ScanReturnData batchInformation = remiForm.RemiData[0];
                            Details.TestSystemName = batchInformation.SelectedTestName;
                            string[] qraNumber = batchInformation.QRANumber.Split('-');
                            Details.JobNumber = string.Join("-", qraNumber, 0, qraNumber.Length - 1);
                            Details.TestStage = batchInformation.TestStageName;
                            Details.TestType = batchInformation.JobName;
                            Details.UnitNumber = (uint)batchInformation.UnitNumber;
                        }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("DBControl Exception: " + ex);
                }
            }
            else
                base.EditTestDetails(sender, false);
        }

        protected override void PublishResults(TsdLib.Measurements.ITestResults results)
        {
            System.Diagnostics.Trace.WriteLine("Simulating database upload");
            System.Threading.Thread.Sleep(10000);
            System.Diagnostics.Trace.WriteLine("Done uploading");
        }
#endif

    }
}