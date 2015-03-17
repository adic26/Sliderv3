using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using TSD_Slider;
using RDotNet;

namespace TSD_Slider.UI.Components
{
    public class CharData : DataGridView
    {
        public DataGridView data;
        private List<double> displacement;
        public IProgress<DataFrame> progressData;

        public List<double> Displacement
        {
            get { return displacement; }
            set { displacement = value; }
        }
        private List<double> force;

        public List<double> Force
        {
            get { return force; }
            set { force = value; }
        }

        public void updateData(DataFrame dataset)
        {
            AddMeasurement(dataset);
            //this.Update();
        }

        /// <summary>
        /// Adds all the measurements that are currently available in Displacement and Force
        /// </summary>
        public void AddMeasurement(string colNameForce, string colNameDisplacement)
        {
            List<object> newRowObject = new List<object> { displacement.ToArray(), force.ToArray() };
            Columns[0].Name = colNameForce;
            Columns[1].Name = colNameDisplacement;

            Rows.Add(newRowObject.ToArray());
        }

        /// <summary>
        /// Addds all the measurement given from R's NumericMatrix calculated from script
        /// </summary>
        /// <param name="dataset">RdotNet: NumericMatrix</param>
        public void AddMeasurement(DataFrame dataset)
        {
            try
            {
                Columns.Clear();
                Rows.Clear();
                for (int i = 0; i < dataset.ColumnCount; i++)
                {
                    ColumnCount++;
                    Columns[i].Name = dataset.ColumnNames[i];
                }

                //now manage the data into a proper set
                for (int i = 0; i < dataset.RowCount; i++)
                {
                    RowCount++;
                    for (int k = 0; k < dataset.ColumnCount; k++)
                        Rows[i].Cells[k].Value = dataset[i, k];
                }

            }
            catch (Exception measurementException)
            {
                Trace.WriteLine(measurementException.Message);
                Trace.WriteLine(measurementException.StackTrace);
            }
        }

        /// <summary>
        /// Gives back the list of double values that are currently in the data grid view
        /// </summary>
        /// <param name="colName">Name of the column that is in the list. Usually "displacement" or "force" </param>
        /// <returns>List of double from the list</returns>
        public List<double> GetValues(string colName)
        {
            //1 is displacement and 2 is force - it needs to be a variable
            var query = from d in Rows.Cast<DataGridViewRow>()
                        select Convert.ToDouble(d.Cells[colName].Value);

            //query = from d in query select Math.Round(d, 2);
            List<double> myList = query.ToList<double>();
            return myList;
        }



    }
}
