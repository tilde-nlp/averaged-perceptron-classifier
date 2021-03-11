using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PerceptronLibrary
{
    public class Perceptron
    {
        public PerceptronModel TrainPerceptronModelWithoutCrossValidation(List<EntryElement> data, List<string> trueValues, List<EntryElement> devData, List<string> devTrueValues, int maxIterations, List<FeatureExtractor> featureExtractors)
        {
            Console.WriteLine("Training entry count: " + data.Count);
            //Figure out the number of classes (the number of unique true values).
            List<string> features = new List<string>();
            HashSet<string> classes = new HashSet<string>();
            foreach (string c in trueValues)
            {
                if (!classes.Contains(c))
                {
                    classes.Add(c);
                    //res.weights.Add(c, new Dictionary<string, double>());
                }
            }
            Console.WriteLine("Unique classes to be trained: " + classes.Count.ToString());

            PerceptronModel res = TrainPerceptronModel(data, trueValues, classes, devData, devTrueValues, maxIterations, featureExtractors);
            return res;
        }

        public PerceptronModel TrainPerceptronModel(List<EntryElement> data, List<string> trueValues, int crossValidationSets, List<EntryElement> devData, List<string> devTrueValues, int maxIterations, List<FeatureExtractor> featureExtractors)
        {

            Console.WriteLine("Initial training entry count: " + data.Count);
            //Figure out the number of classes (the number of unique true values).
            List<string> features = new List<string>();
            HashSet<string> classes = new HashSet<string>();
            foreach(string c in trueValues)
            {
                if (!classes.Contains(c))
                {
                    classes.Add(c);
                    //res.weights.Add(c, new Dictionary<string, double>());
                }
            }
            Console.WriteLine("Unique classes to be trained: "+classes.Count.ToString());

            //Now, split data in cross-validation sets
            List<List<EntryElement>> trainingSets = new List<List<EntryElement>>();
            List<List<string>> trainingTruthValues = new List<List<string>>();
            List<List<EntryElement>> evalSets = new List<List<EntryElement>>();
            List<List<string>> evalTruthValues = new List<List<string>>();
            List<List<EntryElement>> devSets = new List<List<EntryElement>>();
            List<List<string>> devTruthValues = new List<List<string>>();
            GetRandomFolds(data, trueValues, crossValidationSets, ref trainingSets, ref trainingTruthValues, ref evalSets, ref evalTruthValues, ref devSets, ref devTruthValues);

            //Train each cross-validation set.
            EvaluationResults results = new EvaluationResults();
            for (int i=0;i<trainingSets.Count;i++)
            {
                PerceptronModel pm = TrainPerceptronModel(trainingSets[i], trainingTruthValues[i], classes, devSets[i], devTruthValues[i], maxIterations, featureExtractors);
                EvaluationResult er = EvaluatePerceptronModel(pm, evalSets[i], evalTruthValues[i]);
                results.SetIndividualResult(er);
            }

            PerceptronModel res = TrainPerceptronModel(data, trueValues, classes, devData, devTrueValues, maxIterations, featureExtractors);
            res.crossValidationResults = results;
            return res;
        }

        private EvaluationResult EvaluatePerceptronModel(PerceptronModel pm, List<EntryElement> evalData, List<string> truthValues)
        {
            //TODO - improve the evaluation to count also individual class performance
            int correct = 0;
            int wrong = 0;
            for( int i=0;i< evalData.Count;i++)// List<TokenElement> sentence in evalData)
            {
                Dictionary<string, double> classification = pm.Predict(evalData[i]);
                Dictionary<string, double> softMaxClassification = PerceptronModel.GetProportions(classification);
                string maxClass = pm.defaultClass;
                double maxScore = 0;
                foreach(string c in softMaxClassification.Keys)
                {
                    if (softMaxClassification[c]>maxScore)
                    {
                        maxClass = c;
                        maxScore = softMaxClassification[c];
                    }
                }
                if (truthValues[i] == maxClass) correct++;
                else wrong++;
            }
            EvaluationResult er = new EvaluationResult();
            er.entries = evalData.Count;
            double precision = (double)correct / ((double)(correct + wrong));
            er.measurements.Add("precision", precision);
            return er;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">The training sentences</param>
        /// <param name="truthValues">The truth values for the training sentences</param>
        /// <param name="classes">The HashSet of class strings</param>
        /// <param name="n">The number of training iterations</param>
        /// <returns></returns>
        private PerceptronModel TrainPerceptronModel(List<EntryElement> data, List<string> truthValues, HashSet<string> classes, List<EntryElement> devData, List<string> devTruthValues, int n, List<FeatureExtractor> featureExtractors)
        {
            PerceptronModel pm = new PerceptronModel();
            foreach(string c in classes)
            {
                pm.weights.Add(c, new Dictionary<string, double>());
            }
            pm.featureExtractors = featureExtractors;

            Dictionary<string, int> truthValueCount = new Dictionary<string, int>();
            foreach(string c in truthValues)
            {
                if (!truthValueCount.ContainsKey(c)) truthValueCount.Add(c, 1);
                else truthValueCount[c]++;
            }
            KeyValuePair<string, int> maxCountKvp = truthValueCount.ToList().OrderByDescending(key => key.Value).First();
            pm.defaultClass = maxCountKvp.Key;
            //Train the model in n iterations:
            double previousPrecision = 0;
            for (int i=0;i<n;i++)
            {
                Console.Error.WriteLine("Iteration: " + i.ToString());
                //Randomise training data:
                List<EntryElement> shuffledData = null;
                List<string> shuffledTruthValues = null;
                Console.Error.WriteLine("Shuffling data.");
                ShuffleTrainingData(data, truthValues, out shuffledData, out shuffledTruthValues);
                Console.Error.WriteLine("Iterating through data.");
                //Go through the training data instances one-by-one, predict and update weights if necessary.
                for (int j = 0; j < shuffledData.Count; j++)
                {
                    if (j%10000==0)
                    {
                        Console.Error.Write(".");
                        if (j%500000==0)
                        {
                            Console.Error.WriteLine(" - " + j.ToString());
                        }
                    }
                    pm.PredictAndUpdateWithoutAveraging(shuffledData[j], shuffledTruthValues[j]);
                }
                //Perform averaging before evaluation otherwise the model won't be usable for validation
                pm.PerformAveraging(false);
                //Validate
                EvaluationResult er = EvaluatePerceptronModel(pm, devData, devTruthValues);
                Console.Error.WriteLine("Precision after iteration "+i.ToString()+": "+er.measurements["precision"].ToString("0.000"));
                Console.Error.WriteLine(" - " + shuffledData.Count.ToString());
                if (er.measurements["precision"]<=previousPrecision)
                {
                    Console.Error.WriteLine("Precision not improving! Early stopping engaged!");
                    break;
                }
                else
                {
                    previousPrecision = er.measurements["precision"];
                    Console.Error.WriteLine("Precision improving!");
                    if (i==n-1)
                        Console.Error.WriteLine("Stopping training as maximum epoch count reached!");
                }
            }

            //Average the weights in the averaged perceptron algorithm.
            pm.PerformAveraging();
            return pm;
        }

        public static void ShuffleTrainingData(List<EntryElement> xList, List<string> yList, out List<EntryElement> shuffledXList, out List<string> shuffledYList)
        {
            shuffledXList = new List<EntryElement>(xList.Count);
            shuffledYList = new List<string>(yList.Count);
            Random r = new Random(7);//TODO: Refactor the code so that the seed can be specified externally!
            List<int> idxList = new List<int>();
            for (int i = 0; i < xList.Count; i++)
            {
                idxList.Add(i);
            }

            List<int> randomisedIdxList = new List<int>();

            while (idxList.Count > 0)
            {
                int random = r.Next(0, idxList.Count);
                randomisedIdxList.Add(idxList[random]);
                idxList.RemoveAt(random);
            }
            foreach(int i in randomisedIdxList)
            {
                shuffledXList.Add(xList[i]);
                shuffledYList.Add(yList[i]);
            }
        }

        public static void GetRandomFolds(List<EntryElement> xList, List<string> yList, int folds, ref List<List<EntryElement>> randomTrainDataFolds, ref List<List<string>> randomTrainDataObjectiveFolds, ref List<List<EntryElement>> randomTestDataFolds, ref List<List<string>> randomTestDataObjectiveFolds, ref List<List<EntryElement>> randomDevDataFolds, ref List<List<string>> randomDevDataObjectiveFolds)
        {
            if (folds < 3) throw new NotSupportedException("Cross-validation folds have to be 3 or more");
            int entriesPerFold = xList.Count / folds;
            Random r = new Random(7);//TODO: Refactor the code so that the seed can be specified externally!

            List<int> idxList = new List<int>();
            for (int i = 0; i < xList.Count; i++)
            {
                idxList.Add(i);
            }

            List<int> randomisedIdxList = new List<int>();

            while (idxList.Count > 0)
            {
                int random = r.Next(0, idxList.Count);
                randomisedIdxList.Add(idxList[random]);
                idxList.RemoveAt(random);
            }
            for (int i = 0; i < folds; i++)
            {
                int evalIndex = i;
                int evalLowerBound = evalIndex * entriesPerFold;
                int evalUpperBound = evalIndex * entriesPerFold + entriesPerFold - 1;

                int devIndex = (i + 1) % folds;
                int devLowerBound = devIndex * entriesPerFold;
                int devUpperBound = devIndex * entriesPerFold + entriesPerFold - 1;

                List<EntryElement> trainFold = new List<EntryElement>();
                List<EntryElement> testFold = new List<EntryElement>();
                List<EntryElement> devFold = new List<EntryElement>();
                List<string> trainObjectiveFold = new List<string>();
                List<string> testObjectiveFold = new List<string>();
                List<string> devObjectiveFold = new List<string>();

                for (int j = 0; j < randomisedIdxList.Count; j++)
                {
                    if (j >= evalLowerBound && j <= evalUpperBound)
                    {
                        testFold.Add(xList[randomisedIdxList[j]]);
                        testObjectiveFold.Add(yList[randomisedIdxList[j]]);
                    }
                    else if (j >= devLowerBound && j <= devUpperBound)
                    {
                        devFold.Add(xList[randomisedIdxList[j]]);
                        devObjectiveFold.Add(yList[randomisedIdxList[j]]);
                    }
                    else
                    {
                        trainFold.Add(xList[randomisedIdxList[j]]);
                        trainObjectiveFold.Add(yList[randomisedIdxList[j]]);
                    }
                }

                randomTrainDataFolds.Add(trainFold);
                randomTrainDataObjectiveFolds.Add(trainObjectiveFold);
                randomTestDataFolds.Add(testFold);
                randomTestDataObjectiveFolds.Add(testObjectiveFold);
                randomDevDataFolds.Add(devFold);
                randomDevDataObjectiveFolds.Add(devObjectiveFold);
            }
        }

    }
}
