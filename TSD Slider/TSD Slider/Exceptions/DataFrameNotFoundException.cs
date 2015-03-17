using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sliderv2.Exceptions
{
    public class DataFrameNotFoundException : Exception
    {
        public DataFrameNotFoundException() { }
        public DataFrameNotFoundException(string message) : base(message) { }
        public DataFrameNotFoundException(string message, Exception innerException) : base(message, innerException) { }

    }
}
