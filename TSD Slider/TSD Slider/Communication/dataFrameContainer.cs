using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;

namespace TSD_Slider.Communication
{
    [Serializable]
    public class dataFrameContainer : MarshalByRefObject
    {

        private DataFrame myRealFrame;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public dataFrameContainer(DataFrame myFrame)
        {
            xFrame = myFrame;
        }

        public DataFrame xFrame
        {
            get { return myRealFrame; }
            set { myRealFrame = value; }
        }
    }
}
