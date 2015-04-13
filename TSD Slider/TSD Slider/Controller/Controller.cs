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
        public DataBuilder dataCollection;

        public Controller(ITestDetails testDetails, IConfigConnection databaseConnection, bool localDomain)
            : base(testDetails,databaseConnection,localDomain)
        {
            //this.UI.stationConfig
            //StationConfig stnConfig = UI.stationConfig as StationConfig;
            //dataCollection = new DataBuilder(stnConfig.i386Path, stnConfig.RInstallerPath);
        }

        public void TestingRawData()
        {
            
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
            //string xmlFile = TsdLib.SpecialFolders.GetResultsFileName(results.Details, results.Summary, "xml");
            string xmlFile = System.IO.Path.Combine(TsdLib.SpecialFolders.GetResultsFolder(results.Details.SafeTestSystemName).FullName, TsdLib.SpecialFolders.GetResultsFileName(results.Details, results.Summary, "xml"));
            string path = System.IO.Path.GetDirectoryName(xmlFile);
            if (path == null)
                throw new System.IO.DirectoryNotFoundException("The results folder does not exist on this machine.");
            System.Diagnostics.Trace.WriteLine("Uploading results to database...");
            System.Diagnostics.Trace.WriteLine("XML Path: " + path);
            System.Diagnostics.Trace.WriteLine("xmlFile : " + xmlFile);
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