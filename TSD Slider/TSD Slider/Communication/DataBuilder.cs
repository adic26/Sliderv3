using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using sliderv2.Exceptions;
using RDotNet;
using TSD_Slider.Instruments;

namespace TSD_Slider.Communication
{
    /// <summary>
    /// This class should Evaluate the lift off once a file is given
    /// This file would be parsed, it would evaluate the lift off and end
    /// </summary>
    public class DataBuilder
    {
        private IProgress<DataFrame> progressData;

        public REngine rengine;
        private double liftOff;
        private string r386Path;
        private string rPath;
        public dataFrame characterizationData;
        public List<double> displacement;
        public List<double> force;
        public List<int> cycles;

        public void setupProgress(IProgress<DataFrame> _prog)
        {
            progressData = _prog;
            Trace.WriteLine("Progress Data and Graph now Linked");
        }
        

        public double Lift_Off
        {
            get { return liftOff; }
            set { liftOff = value; }
        }

        public DataBuilder(string i386path, string rpath)
        {
            r386Path = i386path;
            rPath = rpath;
            
            REngine.SetEnvironmentVariables(r386Path,rPath);
            rengine = REngine.GetInstance();
        }

        public DataFrame GetLiftOffRawData(string rScript, string functionName, Dictionary<string,SymbolicExpression> parameterNames)
        {
            try
            {
                if (rengine == null)
                {
                    REngine.SetEnvironmentVariables(r386Path, rPath);
                    rengine = REngine.GetInstance();
                }
                //setup library
                rengine.Evaluate("require('ggplot2')");
                rengine.Evaluate("library('ggplot2')");

                Trace.WriteLine("Requirement and Library addition completed for GGPLOT2");

                Trace.WriteLine("Using " + functionName +
                            " (function Name) " +
                            parameterNames.First().Key +
                            " (parameter Name)");

 
                string[] files = System.IO.Directory.GetFiles(rScript);
                var scripts = from allFiles in files select allFiles.Replace(@"\", @"/");
                
                foreach (string s in scripts)
                {
                    Trace.WriteLine("Sourcing " + s);
                    if (s.Last<char>() == 'r')
                    {
                        Trace.WriteLine("source('" + s + "')");
                        rengine.Evaluate("source('" + s + "')");
                    }
                    else
                        Trace.WriteLine("not an R file -- " + s);
                }
                //slider function setup
                Function sliderfunc = rengine.Evaluate(functionName).AsFunction();

                Trace.WriteLine("Function Name Evaluated! : " + functionName);

                //Assuming function parameters are pre-setup
                DataFrame dataset = sliderfunc.Invoke(parameterNames).AsDataFrame();
                
                //dataFrame did not work, we must try to do this as characterMatrix
                if (dataset == null)
                {
                    Trace.WriteLine("dataset is null");
                    throw new DataFrameNotFoundException();
                }

                Trace.WriteLine("Parameter Name Invoked using " 
                    + functionName 
                    + ".Invoke(" 
                    + parameterNames.First().Key 
                    + ")");

                //send data frame to different method to find out the maximum value
                return dataset;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
                return null;
            }
        }

        public CharacterVector directory(string stDirectory)
        {
            return rengine.CreateCharacterVector(new[] { stDirectory });
        }

        public double EvaluateLiftOffPoint(DataFrame myTable, string config_Displacement, string config_Force)
        {
            try
            {
                if (displacement == null && force == null)
                {
                    displacement = new List<double>();
                    force = new List<double>();
                    cycles = new List<int>();
                }
                else
                {
                    displacement.Clear();
                    force.Clear();
                    cycles.Clear();
                }

                for (int i = 0; i < myTable.RowCount; i++)
                {
                    displacement.Add(Convert.ToDouble(myTable[i, config_Displacement]));
                    force.Add(Convert.ToDouble(myTable[i, config_Force]));
                    cycles.Add(Convert.ToInt32(myTable[i, 0]));
                }

                
                 return EvaluateLiftOffPoint(displacement, force); 
            }
            catch (LiftoffEvalException ex)
            {
                Trace.WriteLine(ex.Message);
                throw new LiftoffEvalException("Lift Off Error");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }

        }

        public double EvaluateLiftOffPoint(List<double> displacement, List<double> force)
        {

            try
            {

                List<double> absDisp = displacement.ConvertAll<double>(x => Math.Abs(x));

                var window = from disp in absDisp
                             where (disp > 10.0)
                             where (disp < 32.0)
                             select absDisp.IndexOf(disp);

                var windowForce = from f in force.Skip(window.First()).Take(window.Last() - window.First()) select f;
                int indexValue = force.IndexOf(windowForce.Max());

                double resultant = Math.Abs(displacement[indexValue]) - 5.0;

                return resultant;
            }
            catch (Exception liftOffEvalEx)
            {
                throw new LiftoffEvalException("Lift Off force Error", liftOffEvalEx);
            }
        }

    }
}
