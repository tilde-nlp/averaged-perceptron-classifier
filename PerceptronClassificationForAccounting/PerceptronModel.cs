using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PerceptronLibrary
{
    [Serializable]
    public class PerceptronModel
    {
        //Standard variables for the perceptron model
        //weights = class -> feature -> weight
        public Dictionary<string, Dictionary<string, double>> weights = new Dictionary<string, Dictionary<string, double>>();
        public Dictionary<string, double> bias = new Dictionary<string, double>();
        public List<FeatureExtractor> featureExtractors = new List<FeatureExtractor>();
        public Dictionary<string, Dictionary<string, double>> averagedWeights = new Dictionary<string, Dictionary<string, double>>();

        //These variables are for the averaged perceptron:
        public Dictionary<string, Dictionary<string, double>> totals = new Dictionary<string, Dictionary<string, double>>();
        public Dictionary<string, Dictionary<string, double>> timestamps = new Dictionary<string, Dictionary<string, double>>();
        public int counter = 0;
        public HashSet<string> buyers = new HashSet<string>();
        public HashSet<string> suppliers = new HashSet<string>();

        //In cases when, e.g., empty sentence will be used for prediction, the default class will be returned.
        public string defaultClass = "0";

        public EvaluationResults crossValidationResults = new EvaluationResults();
        public PerceptronModel()
        {
            weights = new Dictionary<string, Dictionary<string, double>>();
            featureExtractors = new List<FeatureExtractor>();
        }

        public Dictionary<string,double> Predict(EntryElement entry)
        {
            if (entry == null)
            {
                Dictionary<string, double> res = new Dictionary<string, double>();
                foreach (string c in averagedWeights.Keys)
                {
                    res.Add(c, 0);
                }
                res[defaultClass] = 1;
                return res;
            }
            else
            {
                Dictionary<string, double> res = new Dictionary<string, double>();
                foreach (string c in averagedWeights.Keys)
                {
                    res.Add(c, 0);
                }
                List<string> triggeredFeatures = new List<string>(200);
                foreach (FeatureExtractor fe in featureExtractors)
                {
                    triggeredFeatures.AddRange(fe.GetValuesFromEntry(entry));
                }
                int foundFeatures = 0;
                if (triggeredFeatures.Count > 0)
                {
                    foreach (string feature in triggeredFeatures)
                    {
                        foreach (string c in averagedWeights.Keys)
                        {
                            if (averagedWeights[c].ContainsKey(feature))
                            {
                                res[c] += averagedWeights[c][feature];
                                foundFeatures++;
                            }
                        }
                    }
                    if (foundFeatures<1)
                    {
                        res[defaultClass] = 1;
                    }
                }
                else
                {
                    res[defaultClass] = 1;
                }
                return res;
            }
        }

        public void PredictAndUpdateWithoutAveraging(EntryElement entry, string correctClass)
        {
            counter++;
            //First, perform prediction using the current model:
            Dictionary<string, double> prediction = new Dictionary<string, double>();
            foreach (string c in weights.Keys)
            {
                prediction.Add(c, 0);
            }
            List<string> triggeredFeatures = new List<string>(200);
            foreach (FeatureExtractor fe in featureExtractors)
            {
                triggeredFeatures.AddRange(fe.GetValuesFromEntry(entry));
            }
            foreach (string feature in triggeredFeatures)
            {
                foreach (string c in weights.Keys)
                {
                    if (weights[c].ContainsKey(feature))
                    {
                        prediction[c] += weights[c][feature];
                    }
                }
            }
            //Get the predicted class
            string maxClass = null;
            double maxScore = -1;
            foreach (string c in prediction.Keys)
            {
                if (prediction[c] > maxScore)
                {
                    maxClass = c;
                    maxScore = prediction[c];
                }
            }
            if (correctClass == maxClass)
            {
                //Do nothing if prediction is correct.
            }
            else
            {
                if (!totals.ContainsKey(maxClass)) totals.Add(maxClass, new Dictionary<string, double>());
                if (!timestamps.ContainsKey(maxClass)) timestamps.Add(maxClass, new Dictionary<string, double>());
                if (!totals.ContainsKey(correctClass)) totals.Add(correctClass, new Dictionary<string, double>());
                if (!timestamps.ContainsKey(correctClass)) timestamps.Add(correctClass, new Dictionary<string, double>());
                
                //Update the weights if the prediction was wrong.
                foreach (string f in triggeredFeatures)
                {
                    //Keep track of totals and timestamps in the averaged perceptron algorithm.
                    if (!totals[maxClass].ContainsKey(f)) totals[maxClass].Add(f, 0);
                    if (!totals[correctClass].ContainsKey(f)) totals[correctClass].Add(f, 0);
                    if (!timestamps[maxClass].ContainsKey(f)) timestamps[maxClass].Add(f, 0);
                    if (!timestamps[correctClass].ContainsKey(f)) timestamps[correctClass].Add(f, 0);

                    if (weights[maxClass].ContainsKey(f)) totals[maxClass][f] += (counter - timestamps[maxClass][f]) * weights[maxClass][f];
                    if (weights[correctClass].ContainsKey(f)) totals[correctClass][f] += (counter - timestamps[correctClass][f]) * weights[correctClass][f];

                    timestamps[maxClass][f] = counter;
                    timestamps[correctClass][f] = counter;

                    //Decrease the weights for the wrong class.
                    if (!weights[maxClass].ContainsKey(f)) weights[maxClass].Add(f, -1);
                    else weights[maxClass][f]--;
                    //Increase the weights for the correct class.
                    if (!weights[correctClass].ContainsKey(f)) weights[correctClass].Add(f, 1);
                    else weights[correctClass][f]++;
                }
            }
        }

        public void PredictAndUpdateWithAveraging(EntryElement entry, string correctClass)
        {
            PredictAndUpdateWithoutAveraging(entry, correctClass);
            //TODO - figure out a better averaging function as this is too slow!
            //Average the weights in the averaged perceptron algorithm.
            PerformAveraging();
        }

        public void PerformAveraging(bool updateTotals = true)
        {
            if (!updateTotals)
            {
                averagedWeights.Clear();
                foreach (string c in weights.Keys)
                {
                    if (!averagedWeights.ContainsKey(c)) averagedWeights.Add(c, new Dictionary<string, double>());
                    foreach (string f in weights[c].Keys)
                    {
                        if (!averagedWeights[c].ContainsKey(f)) averagedWeights[c].Add(f, 0);
                        double tempTotals = totals[c][f] + ((counter - timestamps[c][f]) * weights[c][f]);
                        averagedWeights[c][f] = tempTotals / counter;
                    }
                }
            }
            else
            {
                foreach (string c in weights.Keys)
                {
                    if (!averagedWeights.ContainsKey(c)) averagedWeights.Add(c, new Dictionary<string, double>());
                    foreach (string f in weights[c].Keys)
                    {
                        if (!averagedWeights[c].ContainsKey(f)) averagedWeights[c].Add(f, 0);
                        totals[c][f] += (counter - timestamps[c][f]) * weights[c][f];
                        averagedWeights[c][f] = totals[c][f] / counter;
                    }
                }
            }
        }

        public static Dictionary<string,double> GetProportions(Dictionary<string,double> scores)
        {
            Dictionary<string, double> res = new Dictionary<string, double>(scores.Count);
            double min = scores.Values.Min();
            int negScores = 0;
            foreach(string s in scores.Keys)
            {
                if (scores[s] < 0) negScores++;
            }
            if (negScores + 1 == scores.Count)
            {
                foreach (string s in scores.Keys)
                {
                    if (scores[s] <= 0)
                    {
                        res.Add(s, 0);
                    }
                    else if (scores[s] > 0)
                    {
                        res.Add(s, 1);
                    }
                }
            }
            else
            {
                foreach (string s in scores.Keys)
                {
                    if (min < 0)
                    {
                        res.Add(s, scores[s] - min);
                    }
                    else
                    {
                        res.Add(s, scores[s]);
                    }
                }
                double sum = res.Values.Sum();
                if (sum == 0) sum = 1;
                foreach(string c in scores.Keys)
                {
                    res[c]/= sum;
                }
            }
            return res;
        }

        public static PerceptronModel Load(string file)
        {
            var binder = new TypeNameSerializationBinder("PerceptronLibrary.{0}, PerceptronClassificationForAccounting");

            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file)) return null;
            return JsonConvert.DeserializeObject<PerceptronModel>(File.ReadAllText(file, Encoding.UTF8), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Binder = binder
            });
        }
        public static void Save (PerceptronModel pm, string file)
        {
            var binder = new TypeNameSerializationBinder("PerceptronLibrary.{0}, PerceptronClassificationForAccounting");
            
            string outData = JsonConvert.SerializeObject(pm, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Binder = binder
            });
            File.WriteAllText(file, outData, new UTF8Encoding(false));
        }
    }

    public class TypeNameSerializationBinder : SerializationBinder
    {
        public string TypeFormat { get; private set; }

        public TypeNameSerializationBinder(string typeFormat)
        {
            TypeFormat = typeFormat;
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            string resolvedTypeName = string.Format(TypeFormat, typeName);

            return Type.GetType(resolvedTypeName, true);
        }
    }
}
