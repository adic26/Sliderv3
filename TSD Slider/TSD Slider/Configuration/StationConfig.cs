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

        [Category("Power Supply")]
        [Description("The VISA resource name used to identify the power supply")]
        public string PowerSupplyAddress { get; set; }

        /// <summary>
        /// Initialize the configuration properties to default values. Do not use a default constructor, as it can interfere with deserialization.
        /// </summary>
        public override void InitializeDefaultValues()
        {
            PowerSupplyAddress = "GPIB0::1::INSTR";
        }
    }
}