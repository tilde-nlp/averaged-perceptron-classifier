using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PerceptronLibrary
{
    [Serializable]
    public class EvaluationResults
    {
        public EvaluationResults()
        {
            confidenceIntervals = new Dictionary<string, ConfidenceInterval>();
            individualResults = new List<EvaluationResult>();
        }

        public Dictionary<string, ConfidenceInterval> confidenceIntervals;
        public List<EvaluationResult> individualResults;
        
        public void SetIndividualResult(EvaluationResult individualResult)
        {
            individualResults.Add(individualResult);
            Dictionary<string, List<double>> metricScores = new Dictionary<string, List<double>>();
            foreach (EvaluationResult er in individualResults)
            {
                foreach(string metric in er.measurements.Keys)
                {
                    if (!metricScores.ContainsKey(metric)) metricScores.Add(metric, new List<double>());
                    metricScores[metric].Add(er.measurements[metric]);
                }
            }
            foreach (string metric in metricScores.Keys)
            {
                ConfidenceInterval ci = new ConfidenceInterval(0.95, metricScores[metric]);
                if (!confidenceIntervals.ContainsKey(metric)) confidenceIntervals.Add(metric, ci);
                else confidenceIntervals[metric] = ci;
            }
        }
    }
}
