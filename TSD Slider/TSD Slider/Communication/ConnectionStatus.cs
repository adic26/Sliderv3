using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSD_Slider.Communication
{
    [Serializable]
    public struct ConnectionStatus
    {
        public bool status;
        public ConnectionStatus(bool status) { this.status = status; }
    }
}
