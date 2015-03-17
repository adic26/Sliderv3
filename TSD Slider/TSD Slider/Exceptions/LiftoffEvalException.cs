using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sliderv2.Exceptions
{
    public class LiftoffEvalException : Exception
    {
        public LiftoffEvalException() { }
        public LiftoffEvalException(string message) : base(message) { }
        public LiftoffEvalException(string message, Exception innerException) : base(message, innerException) { }
    }
}
