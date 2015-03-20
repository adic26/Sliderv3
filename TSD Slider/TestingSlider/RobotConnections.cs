using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TSD_Slider;
using TSD_Slider.Communication;
using TSD_Slider.Instruments;
using TSD_Slider.UI.Components;
using TSD_Slider.Sequences;
using TSD_Slider.Configuration;

namespace TestingSlider
{
    [TestClass]
    public class RobotConnections
    {
        tsdFanuc robot;

        /// <summary>
        /// Testing Connection to Robots by throwing in random numbers and strings
        /// </summary>
        [TestMethod]
        public void ConnectRobot()
        {
            //Arrange
            robot = new tsdFanuc();

            //Act
            //RobotSequence sequence = new RobotSequence();
            try
            {
                robot.connect("192.168.1.129");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
       }

        [TestMethod]
        public void looseConnection()
        {
            robot = new tsdFanuc();
            
        }

        [TestMethod]
        public void checkConfigurations()
        {

        }
        
    }
}
