using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TsdLib.Configuration;
using System.Configuration;
//using TSD_Slider.Configuration.Connections;
using TsdLib.Configuration.Common;
using TsdLib.Configuration.Connections;
using TsdLib.Configuration.Managers;

namespace TSD_Slider
{
    class TSD_SliderProgram
    {
#if DEBUG
        private const OperatingMode DefaultMode = OperatingMode.Engineering;
#else
        private const OperatingMode DefaultMode = OperatingMode.Production;
#endif

        private const string TestSystemNameArg = "-testSystemName";
        private const string TestSystemVersionArg = "-testSystemVersion";
        private const string TestSystemVersionMaskArg = "-testSystemVersionMask";
        private const string TestSystemModeArg = "-testSystemMode";
        private const string LocalDomainArg = "-localDomain";
        private const string SettingsLocationArg = "-settingsLocation";
        private const string SeqFolderArg = "-seq";

        private static List<string> _argsList;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Trace.Listeners.Add(new ConsoleTraceListener());

                _argsList = args.ToList();

                string testSystemName = getConfigValue(TestSystemNameArg) ?? Application.ProductName;
                Version testSystemVersion = Version.Parse(getConfigValue(TestSystemVersionArg) ?? Application.ProductVersion.Split('-')[0]);
                string testSystemVersionMask = getConfigValue(TestSystemVersionMaskArg) ?? @"\d+\.\d+";
                OperatingMode testSystemMode = (OperatingMode)Enum.Parse(typeof(OperatingMode), getConfigValue(TestSystemModeArg) ?? DefaultMode.ToString());
                bool localDomain = bool.Parse(getConfigValue(LocalDomainArg) ?? "false");
                string settingsLocation = getConfigValue(SettingsLocationArg) ?? @"C:\temp\TsdLibSettings";

                ITestDetails testDetails = new TestDetails(testSystemName, testSystemVersion, testSystemMode);

#if REMICONTROL
                IConfigConnection sharedConfigConnection = new TSD_Slider.Configuration.Connections.DatabaseConfigConnection(testSystemVersionMask);
#else
                IConfigConnection sharedConfigConnection = new FileSystemConnection(new DirectoryInfo(settingsLocation), testSystemVersionMask);
#endif
                if (args.Contains(SeqFolderArg))
                {
                    synchronizeSequences(testDetails, sharedConfigConnection, getConfigValue(SeqFolderArg), true);
                    return;
                }

                Controller c = new Controller(testDetails, sharedConfigConnection, localDomain);
                Application.Run(c.UI);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ex.GetType().Name);
            }
        }

        private static string getConfigValue(string key)
        {
            try
            {
                if (_argsList.Contains(key))
                    return _argsList[_argsList.IndexOf(key) + 1];
                string appConfigValue = ConfigurationManager.AppSettings[key];
                return string.IsNullOrWhiteSpace(appConfigValue) ? null : appConfigValue;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return null;
            }
        }

        //TODO: move this to ConfigManager?
        private static void synchronizeSequences(ITestDetails testDetails, IConfigConnection sharedConfigConnection, string sequenceFolder, bool storeInDatabase)
        {
            ConfigManager<SequenceConfigCommon> sequenceConfigManager = new ConfigManager<SequenceConfigCommon>(testDetails, sharedConfigConnection);

            HashSet<string> assemblyReferences = new HashSet<string>(AppDomain.CurrentDomain.GetAssemblies().Select(asy => Path.GetFileName(asy.GetName().CodeBase)), StringComparer.InvariantCultureIgnoreCase) { Path.GetFileName(Assembly.GetEntryAssembly().GetName().CodeBase) };
            foreach (string fileName in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll").Select(Path.GetFileName))
                assemblyReferences.Add(fileName);

            //foreach (SequenceConfigCommon sequence in sequenceConfigManager.GetConfigGroup().Where(seq => !seq.IsDefault))
            //{
            //    string vsFile = Path.Combine(sequenceFolder, sequence.Name + ".cs");
            //    if (!File.Exists(vsFile))
            //        File.WriteAllText(vsFile, sequence.SourceCode);
            //}
            foreach (string seqFile in Directory.EnumerateFiles(sequenceFolder))
            {
                Trace.WriteLine("Found " + seqFile);
                //TODO: only replace if newer?
                sequenceConfigManager.Add(new SequenceConfigCommon(seqFile, storeInDatabase, assemblyReferences));
            }
            sequenceConfigManager.Save();
        }
    }
}
