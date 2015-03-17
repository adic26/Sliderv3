using System;
using System.ComponentModel;
using TsdLib.Configuration;
using TsdLib.Configuration.Common;

namespace TSD_Slider.Configuration
{
    [Serializable]
    public class StationConfig : StationConfigCommon
    {
        //TODO: Create a station configuration structure using public properties with get and set accessors.
        //The values for these properties will be configured by the application at run-time (in Development mode only) or in the database
        //The property values will be accessed by the TestSequence.Execute() method

        //[Category("Power Supply")]
        //[Description("The VISA resource name used to identify the power supply")]
        //public string PowerSupplyAddress { get; set; }

        [Category("Fanuc Settings")]
        public string Fanuc_Ipaddress { get; set; }

        [Category("Fanuc Settings")]
        public string fanucUsername { get; set; }

        [Category("Fanuc Settings")]
        public string fanucPassword { get; set; }

        [Category("Fanuc Settings")]
        public string TPCharacterizationName { get; set; }

        [Category("Fanuc Settings")]
        public string TPSliderCycleName { get; set; }

        [Category("Fanuc Settings")]
        public string RegLiftOffName { get; set; }

        [Category("Fanuc Settings")]
        public string RegCyclesName { get; set; }

        [Category("Fanuc Settings")]
        public string RegCyclesCompletedName { get; set; }

        [Category("Station Name")]
        public string StationName { get; set; }

        [Category("Fanuc Settings")]
        public string ForceFilePath { get; set; }

        [Category("PC")]
        public string PCDataFolderPath { get; set; }

        [Category("R Settings")]
        public string i386Path { get; set; }

        [Category("R Settings")]
        public string RInstallerPath { get; set; }

        [Category("R Script")]
        public string scriptLocation { get; set; }

        [Category("R Script")]
        public string scriptFunctionName { get; set; }

        [Category("R Script")]
        public string scriptParameterName { get; set; }

        [Category("R Script")]
        public string scriptDisplacementName { get; set; }

        [Category("R Script")]
        public string scriptForceName { get; set; }




        /// <summary>
        /// Initialize the configuration properties to default values. Do not use a default constructor, as it can interfere with deserialization.
        /// </summary>
        public override void InitializeDefaultValues()
        {
            //PowerSupplyAddress = "GPIB0::1::INSTR";
                Fanuc_Ipaddress = "192.168.1.129";
                StationName = "Slider Station 2";
                fanucUsername = "anonymous";
                fanucPassword = "anonymous@domain.com";
                ForceFilePath = @"mc:/fstd1/";
                PCDataFolderPath = @"\\fsg16ykf\personal\achugh\Fanuc & Slider\slider_fanuc_computer\MCDrive\";
                i386Path = @"C:\Program Files\R\R-3.1.1\bin\i386";
                RInstallerPath = @"C:\Program Files\R\R-3.1.1";
                scriptFunctionName = "characterization_lines";
                scriptParameterName = "directory";
                scriptDisplacementName = "disp";
                scriptForceName = "force";
                scriptLocation = @"C:\Users\" + Environment.UserName + @"\Documents\CharacterizationData\";
        }
    }
}