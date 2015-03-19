using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSD_Slider.Communication
{
    [Serializable]
    public struct IntStruct
    {
        public int data;
        public string context;
        public IntStruct(int d, string c) { this.data = d; this.context = c; }
    }
}
