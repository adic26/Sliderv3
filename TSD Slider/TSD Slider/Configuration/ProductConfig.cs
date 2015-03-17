using System;
using System.ComponentModel;
using TsdLib.Configuration;
using TsdLib.Configuration.Common;

namespace TSD_Slider.Configuration
{
    [Serializable]
    public class ProductConfig : ProductConfigCommon
    {
        //TODO: Create a product configuration structure using public properties with get and set accessors.
        //The values for these properties will be configured by the application at run-time (in Development mode only) or in the database
        //The property values will be accessed by the TestSequence.Execute() method

        //[Category("Timing")]
        //[Description("Number of milliseconds to wait after adjusting voltage level")]
        //public int SettlingTime { get; set; }

        [Category("DUT Slider")]
        public double pushForcelbf { get; set; }

        [Category("DUT Slider")]
        public double liftOffPoint { get; set; }

        [Category("DUT Slider")]
        public int CalibrateEvery { get; set; }

        [Category("DUT Slider")]
        public int NumOfCycles { get; set; }

        [Category("DUT Slider")]
        public int CompletedCycles { get; set; }

        /// <summary>
        /// Initialize the configuration properties to default values. Do not use a default constructor, as it can interfere with deserialization.
        /// </summary>
        public override void InitializeDefaultValues()
        {
                CalibrateEvery = 1000;
                NumOfCycles = 150000;
                CompletedCycles = 0;
        }
    }
}
