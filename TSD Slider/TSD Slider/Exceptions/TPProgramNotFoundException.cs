using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sliderv2.Exceptions
{
    public class TPProgramNotFoundException : Exception
    {
        public TPProgramNotFoundException() { }
        public TPProgramNotFoundException(string message) : base(message) { }
        public TPProgramNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
