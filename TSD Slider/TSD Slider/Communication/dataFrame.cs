using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSD_Slider.Communication
{
    [Serializable]
    public struct dataFrame : IDisposable
    {
        public int[] cycles;
        public double[] displacement;
        public double[] force;
        public string[] ColumnNames;

        public dataFrame(int[] cycles, double[] displacement, double[] force)
        {
            this.cycles = cycles;
            this.displacement = displacement;
            this.force = force;
            this.ColumnNames = new string[] { "cycles", "displacement", "force" };
        }

        public void Dispose()
        {
            cycles = null;
            displacement = null;
            force = null;
        }
    }
}
