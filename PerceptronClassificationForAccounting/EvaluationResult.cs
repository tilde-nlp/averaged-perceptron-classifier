using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerceptronLibrary
{
    [Serializable]
    public class EvaluationResult
    {
        public Dictionary<string, double> measurements = new Dictionary<string, double>();

        /// <summary>
        /// Number of entries evaluated
        /// </summary>
        public double entries = 0;

        public EvaluationResult()
        {

        }
    }

}
