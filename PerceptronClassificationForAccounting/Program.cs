using PerceptronLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerceptronClassificationForAccounting
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.Unicode;
            string trainingDataPath = null;
            string devDataPath = null;
            string outFile = null;
            string modelOutPath = null;
            string modelInPath = null;
            string method = "train";
            int classificationTarget = 0; // debit=0, credit=1, combined=2
            bool addSynthData = false; // buyers, suppliers and buyers&suppliers masked
            bool addSecondTypeSynthData = false; //4 types masked separately (buyers, suppliers, and both NACE codes)
            bool printAllPredictions = false;
            bool maskBuyer = false;
            bool maskSupplier = false;
            bool skipNumericFeatures = false;
            bool useNullInsteadOfNone = false;
            int numberOfEpochs = 10;

            bool useAmount = true;
            bool useAmountR0 = true;
            bool useAmountR10 = true;
            bool useAmountR100 = true;
            bool useDocAmount = true;
            bool useDocAmountR0 = true;
            bool useDocAmountR10 = true;
            bool useDocAmountR100 = true;
            bool useProportion = true;
            bool useBuyer = true;
            bool useBuyerNace = true;
            bool useSupplier = true;
            bool useSupplierNace = true;
            bool useYear = true;
            bool useReverseVat = true;
            bool useCurrency = true;
            bool useDocSeries = true;
            bool useComment = true;
            bool useDocComment = true;
            bool stemWords = false;
            string language = "lv";
            string stopWordFile = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-i" && args.Length > i + 1)
                {
                    trainingDataPath = args[i + 1];
                }
                if (args[i] == "-d" && args.Length > i + 1)
                {
                    devDataPath = args[i + 1];
                }
                else if (args[i] == "-o" && args.Length > i + 1)
                {
                    outFile = args[i + 1];
                }
                else if (args[i] == "-mo" && args.Length > i + 1)
                {
                    modelOutPath = args[i + 1];
                }
                else if (args[i] == "-m" && args.Length > i + 1)
                {
                    method = args[i + 1].ToLower();
                }
                else if (args[i] == "--skip-amount")
                {
                    useAmount = false;
                }
                else if (args[i] == "--skip-amount-r0")
                {
                    useAmountR0 = false;
                }
                else if (args[i] == "--skip-amountr-10")
                {
                    useAmountR10 = false;
                }
                else if (args[i] == "--skip-amount-r100")
                {
                    useAmountR100 = false;
                }
                else if (args[i] == "--skip-doc-amount")
                {
                    useDocAmount = false;
                }
                else if (args[i] == "--skip-doc-amount-r0")
                {
                    useDocAmountR0 = false;
                }
                else if (args[i] == "--skip-doc-amountr-10")
                {
                    useDocAmountR10 = false;
                }
                else if (args[i] == "--skip-doc-amount-r100")
                {
                    useDocAmountR100 = false;
                }
                else if (args[i] == "--skip-proportion")
                {
                    useProportion = false;
                }
                else if (args[i] == "--skip-buyer")
                {
                    useBuyer = false;
                }
                else if (args[i] == "--skip-buyer-nace")
                {
                    useBuyerNace = false;
                }
                else if (args[i] == "--skip-supplier")
                {
                    useSupplier = false;
                }
                else if (args[i] == "--skip-supplier-nace")
                {
                    useSupplierNace = false;
                }
                else if (args[i] == "--skip-year")
                {
                    useYear = false;
                }
                else if (args[i] == "--skip-reverse-vat")
                {
                    useReverseVat = false;
                }
                else if (args[i] == "--skip-currency")
                {
                    useCurrency = false;
                }
                else if (args[i] == "--skip-comment")
                {
                    useComment = false;
                }
                else if (args[i] == "--skip-doc-comment")
                {
                    useDocComment = false;
                }
                else if (args[i] == "--skip-doc-series")
                {
                    useDocSeries = false;
                }
                else if (args[i] == "-s")
                {
                    addSynthData = true;
                }
                else if (args[i] == "-s2")
                {
                    addSecondTypeSynthData = true;
                }
                else if (args[i] == "-sn")
                {
                    skipNumericFeatures = true;
                }
                else if (args[i] == "-a")
                {
                    printAllPredictions = true;
                }
                else if (args[i] == "-mb")
                {
                    maskBuyer = true;
                }
                else if (args[i] == "-ms")
                {
                    maskSupplier = true;
                }
                else if (args[i] == "--stem-words")
                {
                    stemWords = true;
                }
                else if (args[i] == "-l" && args.Length > i + 1)
                {
                    language = args[i + 1].Trim();
                }
                else if (args[i] == "--stop-words" && args.Length > i + 1)
                {
                    stopWordFile = args[i + 1].Trim();
                }
                else if (args[i] == "--use-null" || args[i] == "-un")
                {
                    useNullInsteadOfNone = true;
                }
                else if (args[i] == "-mi" && args.Length > i + 1)
                {
                    modelInPath = args[i + 1].ToLower();
                }
                else if (args[i] == "-e" && args.Length > i + 1)
                {
                    numberOfEpochs = Convert.ToInt32(args[i + 1]);
                }
                else if (args[i] == "-c" && args.Length > i + 1)
                {
                    switch (args[i + 1].ToLower())
                    {
                        case "debit":
                            classificationTarget = 0;
                            break;
                        case "credit":
                            classificationTarget = 1;
                            break;
                        case "combined":
                            classificationTarget = 2;
                            break;
                        default:
                            classificationTarget = 0;
                            break;
                    }
                }
            }
            if (method == "train")
            {
                if (string.IsNullOrWhiteSpace(trainingDataPath) || string.IsNullOrWhiteSpace(modelOutPath) || string.IsNullOrWhiteSpace(devDataPath))
                {
                    PrintUsage();
                    return;
                }

                char[] sep = new char[] { '\t', ' ' };
                List<EntryElement> data = new List<EntryElement>();
                List<string> trueValues = new List<string>();
                StreamReader sr = new StreamReader(trainingDataPath, Encoding.UTF8);
                HashSet<string> buyers = new HashSet<string>();
                HashSet<string> suppliers = new HashSet<string>();
                int counter = 0;
                int entryCounter = 0;
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    counter++;
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        EntryElement entry = EntryElement.GetEntryFromString(line);
                        if (entry != null)
                        {
                            if (string.IsNullOrWhiteSpace(entry.buyer))
                            {
                                entry.buyer = useNullInsteadOfNone ? null : "NONE";
                            }
                            if (string.IsNullOrWhiteSpace(entry.supplier))
                            {
                                entry.supplier = useNullInsteadOfNone ? null : "NONE";
                            }
                            if (entry.buyer != null && !buyers.Contains(entry.buyer))
                            {
                                buyers.Add(entry.buyer);
                            }
                            if (entry.supplier != null && !suppliers.Contains(entry.supplier))
                            {
                                suppliers.Add(entry.supplier);
                            }
                            data.Add(entry);
                            entryCounter++;
                            switch (classificationTarget)
                            {
                                case 0:
                                    trueValues.Add(entry.debitAccount);
                                    break;
                                case 1:
                                    trueValues.Add(entry.creditAccount);
                                    break;
                                case 2:
                                    trueValues.Add(entry.debitAccount + entry.creditAccount);
                                    break;
                                default:
                                    trueValues.Add(entry.debitAccount);
                                    break;
                            }
                        }
                        if (addSynthData)
                        {
                            entry = EntryElement.GetEntryFromString(line);
                            if (entry != null && entry.buyer != "NONE" && entry.buyer != null)
                            {
                                entry.buyer = useNullInsteadOfNone ? null : "NONE";
                                entryCounter++;
                                data.Add(entry);
                                switch (classificationTarget)
                                {
                                    case 0:
                                        trueValues.Add(entry.debitAccount);
                                        break;
                                    case 1:
                                        trueValues.Add(entry.creditAccount);
                                        break;
                                    case 2:
                                        trueValues.Add(entry.debitAccount + entry.creditAccount);
                                        break;
                                    default:
                                        trueValues.Add(entry.debitAccount);
                                        break;
                                }
                            }
                            entry = EntryElement.GetEntryFromString(line);
                            if (entry != null && entry.supplier != "NONE" && entry.supplier != null)
                            {
                                entry.supplier = useNullInsteadOfNone ? null : "NONE";
                                entryCounter++;
                                data.Add(entry);
                                switch (classificationTarget)
                                {
                                    case 0:
                                        trueValues.Add(entry.debitAccount);
                                        break;
                                    case 1:
                                        trueValues.Add(entry.creditAccount);
                                        break;
                                    case 2:
                                        trueValues.Add(entry.debitAccount + entry.creditAccount);
                                        break;
                                    default:
                                        trueValues.Add(entry.debitAccount);
                                        break;
                                }
                            }
                            entry = EntryElement.GetEntryFromString(line);
                            if (!addSecondTypeSynthData && entry != null && entry.buyer != "NONE" && entry.supplier != "NONE" && entry.buyer != null && entry.supplier != null)
                            {
                                entry.buyer = useNullInsteadOfNone ? null : "NONE";
                                entry.supplier = useNullInsteadOfNone ? null : "NONE";
                                entryCounter++;
                                data.Add(entry);
                                switch (classificationTarget)
                                {
                                    case 0:
                                        trueValues.Add(entry.debitAccount);
                                        break;
                                    case 1:
                                        trueValues.Add(entry.creditAccount);
                                        break;
                                    case 2:
                                        trueValues.Add(entry.debitAccount + entry.creditAccount);
                                        break;
                                    default:
                                        trueValues.Add(entry.debitAccount);
                                        break;
                                }
                            }
                            else if (addSecondTypeSynthData && entry != null && entry.buyerNace != "NONE" && entry.buyerNace != null)
                            {
                                entry.buyerNace = useNullInsteadOfNone ? null : "NONE";
                                entryCounter++;
                                data.Add(entry);
                                switch (classificationTarget)
                                {
                                    case 0:
                                        trueValues.Add(entry.debitAccount);
                                        break;
                                    case 1:
                                        trueValues.Add(entry.creditAccount);
                                        break;
                                    case 2:
                                        trueValues.Add(entry.debitAccount + entry.creditAccount);
                                        break;
                                    default:
                                        trueValues.Add(entry.debitAccount);
                                        break;
                                }
                                entry = EntryElement.GetEntryFromString(line);
                                if (entry != null && entry.supplierNace != "NONE" && entry.supplierNace != null)
                                {
                                    entry.supplierNace = useNullInsteadOfNone ? null : "NONE";
                                    entryCounter++;
                                    data.Add(entry);
                                    switch (classificationTarget)
                                    {
                                        case 0:
                                            trueValues.Add(entry.debitAccount);
                                            break;
                                        case 1:
                                            trueValues.Add(entry.creditAccount);
                                            break;
                                        case 2:
                                            trueValues.Add(entry.debitAccount + entry.creditAccount);
                                            break;
                                        default:
                                            trueValues.Add(entry.debitAccount);
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
                sr.Close();
                //Read development data
                List<EntryElement> devData = new List<EntryElement>();
                List<string> devTrueValues = new List<string>();
                sr = new StreamReader(devDataPath, Encoding.UTF8);
                int devCounter = 0;
                int devEntryCounter = 0;
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    devCounter++;
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        EntryElement entry = EntryElement.GetEntryFromString(line);
                        if (entry != null)
                        {
                            if (string.IsNullOrWhiteSpace(entry.buyer))
                            {
                                entry.buyer = useNullInsteadOfNone ? null : "NONE";
                            }
                            if (string.IsNullOrWhiteSpace(entry.supplier))
                            {
                                entry.supplier = useNullInsteadOfNone ? null : "NONE";
                            }
                            devData.Add(entry);
                            devEntryCounter++;
                            switch (classificationTarget)
                            {
                                case 0:
                                    devTrueValues.Add(entry.debitAccount);
                                    break;
                                case 1:
                                    devTrueValues.Add(entry.creditAccount);
                                    break;
                                case 2:
                                    devTrueValues.Add(entry.debitAccount + entry.creditAccount);
                                    break;
                                default:
                                    devTrueValues.Add(entry.debitAccount);
                                    break;
                            }
                        }
                        if (addSynthData)
                        {
                            entry = EntryElement.GetEntryFromString(line);
                            if (entry != null && entry.buyer != "NONE" && entry.buyer != null)
                            {
                                entry.buyer = useNullInsteadOfNone ? null : "NONE";
                                devEntryCounter++;
                                devData.Add(entry);
                                switch (classificationTarget)
                                {
                                    case 0:
                                        devTrueValues.Add(entry.debitAccount);
                                        break;
                                    case 1:
                                        devTrueValues.Add(entry.creditAccount);
                                        break;
                                    case 2:
                                        devTrueValues.Add(entry.debitAccount + entry.creditAccount);
                                        break;
                                    default:
                                        devTrueValues.Add(entry.debitAccount);
                                        break;
                                }
                            }
                            entry = EntryElement.GetEntryFromString(line);
                            if (entry != null && entry.supplier != "NONE" && entry.supplier != null)
                            {
                                entry.supplier = useNullInsteadOfNone ? null : "NONE";
                                devEntryCounter++;
                                devData.Add(entry);
                                switch (classificationTarget)
                                {
                                    case 0:
                                        devTrueValues.Add(entry.debitAccount);
                                        break;
                                    case 1:
                                        devTrueValues.Add(entry.creditAccount);
                                        break;
                                    case 2:
                                        devTrueValues.Add(entry.debitAccount + entry.creditAccount);
                                        break;
                                    default:
                                        devTrueValues.Add(entry.debitAccount);
                                        break;
                                }
                            }
                            entry = EntryElement.GetEntryFromString(line);
                            if (!addSecondTypeSynthData && entry != null && entry.buyer != "NONE" && entry.supplier != "NONE" && entry.buyer != null && entry.supplier != null)
                            {
                                entry.buyer = useNullInsteadOfNone ? null : "NONE";
                                entry.supplier = useNullInsteadOfNone ? null : "NONE";
                                devEntryCounter++;
                                devData.Add(entry);
                                switch (classificationTarget)
                                {
                                    case 0:
                                        devTrueValues.Add(entry.debitAccount);
                                        break;
                                    case 1:
                                        devTrueValues.Add(entry.creditAccount);
                                        break;
                                    case 2:
                                        devTrueValues.Add(entry.debitAccount + entry.creditAccount);
                                        break;
                                    default:
                                        devTrueValues.Add(entry.debitAccount);
                                        break;
                                }
                            }
                            else if (addSecondTypeSynthData && entry != null && entry.buyerNace != "NONE" && entry.buyerNace != null)
                            {
                                entry.buyerNace = useNullInsteadOfNone ? null : "NONE";
                                devEntryCounter++;
                                devData.Add(entry);
                                switch (classificationTarget)
                                {
                                    case 0:
                                        devTrueValues.Add(entry.debitAccount);
                                        break;
                                    case 1:
                                        devTrueValues.Add(entry.creditAccount);
                                        break;
                                    case 2:
                                        devTrueValues.Add(entry.debitAccount + entry.creditAccount);
                                        break;
                                    default:
                                        devTrueValues.Add(entry.debitAccount);
                                        break;
                                }
                                entry = EntryElement.GetEntryFromString(line);
                                if (entry != null && entry.supplierNace != "NONE" && entry.supplierNace != null)
                                {
                                    entry.supplierNace = useNullInsteadOfNone ? null : "NONE";
                                    devEntryCounter++;
                                    devData.Add(entry);
                                    switch (classificationTarget)
                                    {
                                        case 0:
                                            devTrueValues.Add(entry.debitAccount);
                                            break;
                                        case 1:
                                            devTrueValues.Add(entry.creditAccount);
                                            break;
                                        case 2:
                                            devTrueValues.Add(entry.debitAccount + entry.creditAccount);
                                            break;
                                        default:
                                            devTrueValues.Add(entry.debitAccount);
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
                sr.Close();

                Console.Error.WriteLine("Lines: " + counter.ToString());
                Console.Error.WriteLine("Entries read: " + entryCounter.ToString());

                List<FeatureExtractor> featureExtractors = new List<FeatureExtractor>();
                WordFeatureExtractor wfe = new WordFeatureExtractor("w", useBuyer, useBuyerNace, useSupplier, useSupplierNace, useYear, useReverseVat, useCurrency, useDocSeries, useComment, useDocComment, stemWords, language, stopWordFile);
                NumberFeatureExtractor nfe = new NumberFeatureExtractor("n", useAmount, useAmountR0, useAmountR10, useAmountR100, useProportion, useDocAmount, useDocAmountR0, useDocAmountR10, useDocAmountR100);
                featureExtractors.Add(wfe);
                if (!skipNumericFeatures) featureExtractors.Add(nfe);
                Perceptron p = new Perceptron();
                PerceptronModel m = p.TrainPerceptronModelWithoutCrossValidation(data, trueValues, devData, devTrueValues, numberOfEpochs, featureExtractors);
                m.buyers = buyers;
                m.suppliers = suppliers;
                PerceptronModel.Save(m, modelOutPath);
            }
            else if (method == "test")
            {
                if (string.IsNullOrWhiteSpace(modelInPath)|| string.IsNullOrWhiteSpace(trainingDataPath)|| string.IsNullOrWhiteSpace(outFile))
                {
                    PrintUsage();
                    return;
                }
                if (!string.IsNullOrWhiteSpace(modelInPath))
                {
                    Console.Error.WriteLine("Loading model!");
                    PerceptronModel pm = PerceptronModel.Load(modelInPath);

                    //The current implementation does not save stopword and stemming information. Therefore, it is lost... We re-create it from arguments as it is important when preparing the features.
                    if (stemWords || !string.IsNullOrWhiteSpace(stopWordFile))
                    {
                        foreach (FeatureExtractor fe in pm.featureExtractors)
                        {
                            if (fe.Name == "w")
                            {
                                WordFeatureExtractor wfe = (WordFeatureExtractor)fe;
                                wfe.LoadStemmerAndStopwords(stemWords, language, stopWordFile);
                            }
                        }
                    }


                    Console.Error.WriteLine("Model loaded!");
                    StreamReader sr = new StreamReader(trainingDataPath, Encoding.UTF8);
                    StreamWriter sw = new StreamWriter(outFile, false, new UTF8Encoding(false));
                    double correct = 0;
                    double total = 0;

                    double correctNoNone = 0;
                    double totalNoNone = 0;

                    double correctBuyerNone = 0;
                    double totalBuyerNone = 0;

                    double correctSupplierNone = 0;
                    double totalSupplierNone = 0;

                    double correctBuyerAndSupplierNone = 0;
                    double totalBuyerAndSupplierNone = 0;

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        EntryElement entry = EntryElement.GetEntryFromString(line);
                        if (maskBuyer) entry.buyer = useNullInsteadOfNone ? null : "NONE";
                        if (maskSupplier) entry.supplier = useNullInsteadOfNone ? null : "NONE";
                        if (entry.buyer != null && !pm.buyers.Contains(entry.buyer)) entry.buyer = useNullInsteadOfNone ? null : "NONE";
                        if (entry.supplier != null && !pm.suppliers.Contains(entry.supplier)) entry.supplier = useNullInsteadOfNone ? null : "NONE";
                        string trueValue = null;
                        switch (classificationTarget)
                        {
                            case 0:
                                trueValue = entry.debitAccount;
                                break;
                            case 1:
                                trueValue = entry.creditAccount;
                                break;
                            case 2:
                                trueValue = entry.debitAccount + entry.creditAccount;
                                break;
                            default:
                                trueValue = entry.debitAccount;
                                break;
                        }
                        Dictionary<string, double> res = pm.Predict(entry);
                        res = PerceptronModel.GetProportions(res);
                        List<KeyValuePair<string, double>> predictions = res.ToList();
                        predictions.Sort((x, y) => y.Value.CompareTo(x.Value));
                        bool first = true;
                        foreach (KeyValuePair<string, double> kvp in predictions)
                        {
                            if (!first) sw.Write(" ");
                            first = false;
                            sw.Write(kvp.Key);
                            sw.Write(" ");
                            sw.Write(kvp.Value.ToString());
                            if (!printAllPredictions)
                            {
                                break;
                            }
                        }
                        sw.WriteLine();
                        if (predictions[0].Key == trueValue)
                        {
                            correct++;
                            if ((entry.buyer == "NONE" || entry.buyer == null) && (entry.supplier == "NONE" || entry.supplier == null))
                            {
                                correctBuyerAndSupplierNone++;
                            }
                            else if (entry.buyer == "NONE" || entry.buyer == null)
                            {
                                correctBuyerNone++;
                            }
                            else if (entry.supplier == "NONE" || entry.supplier == null)
                            {
                                correctSupplierNone++;
                            }
                            else
                            {
                                correctNoNone++;
                            }
                        }
                        total++;
                        if ((entry.buyer == "NONE" || entry.buyer == null) && (entry.supplier == "NONE" || entry.supplier == null))
                        {
                            totalBuyerAndSupplierNone++;
                        }
                        else if (entry.buyer == "NONE" || entry.buyer == null)
                        {
                            totalBuyerNone++;
                        }
                        else if (entry.supplier == "NONE" || entry.supplier == null)
                        {
                            totalSupplierNone++;
                        }
                        else
                        {
                            totalNoNone++;
                        }
                    }
                    sr.Close();
                    sw.Close();

                    Console.WriteLine("Model: " + modelInPath);
                    Console.WriteLine("Correct: " + correct.ToString("0.00"));
                    Console.WriteLine("Total: " + total.ToString("0.00"));
                    Console.WriteLine("Precision: " + (correct / total).ToString("0.0000"));
                    Console.WriteLine("Correct no NONE: " + correctNoNone.ToString("0.00"));
                    Console.WriteLine("Total no NONE: " + totalNoNone.ToString("0.00"));
                    Console.WriteLine("Precision no NONE: " + (correctNoNone / totalNoNone).ToString("0.0000"));
                    Console.WriteLine("Correct buyer NONE: " + correctBuyerNone.ToString("0.00"));
                    Console.WriteLine("Total buyer NONE: " + totalBuyerNone.ToString("0.00"));
                    Console.WriteLine("Precision buyer NONE: " + (correctBuyerNone / totalBuyerNone).ToString("0.0000"));
                    Console.WriteLine("Correct supplier NONE: " + correctSupplierNone.ToString("0.00"));
                    Console.WriteLine("Total supplier NONE: " + totalSupplierNone.ToString("0.00"));
                    Console.WriteLine("Precision supplier NONE: " + (correctSupplierNone / totalSupplierNone).ToString("0.0000"));
                    Console.WriteLine("Correct buyer and supplier NONE: " + correctBuyerAndSupplierNone.ToString("0.00"));
                    Console.WriteLine("Total buyer and supplier NONE: " + totalBuyerAndSupplierNone.ToString("0.00"));
                    Console.WriteLine("Precision buyer and supplier NONE: " + (correctBuyerAndSupplierNone / totalBuyerAndSupplierNone).ToString("0.0000"));
                }
            }
            else if (method == "get-feature-statistics")
            {
                if (string.IsNullOrWhiteSpace(modelInPath))
                {
                    PrintUsage();
                    return;
                }
                if (!string.IsNullOrWhiteSpace(modelInPath))
                {
                    Console.Error.WriteLine("Loading model!");
                    PerceptronModel pm = PerceptronModel.Load(modelInPath);
                    Console.Error.WriteLine("Model loaded!");
                    HashSet<string> features = new HashSet<string>();
                    foreach (string c in pm.weights.Keys)
                    {
                        foreach (string f in pm.weights[c].Keys)
                        {
                            if (!features.Contains(f)) features.Add(f);
                        }
                    }
                    Console.WriteLine("Total feature count: " + features.Count.ToString());
                }
                Console.WriteLine("Done!");
            }
            else if (method == "get-one-r-and-zero-r-scores")
            {
                if (string.IsNullOrWhiteSpace(trainingDataPath) || string.IsNullOrWhiteSpace(devDataPath))
                {
                    PrintUsage();
                    return;
                }
                //We will use train and dev data to calculate this...
                StreamReader sr = new StreamReader(trainingDataPath, Encoding.UTF8);
                //buyer-> supplier -> code -> count
                Dictionary<string, Dictionary<string, Dictionary<string, int>>> trainData = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
                Dictionary<string, Dictionary<string, string>> maxFreq = new Dictionary<string, Dictionary<string, string>>();
                //"NaN" - this will be used to count masked data freqencies
                trainData.Add("NaN", new Dictionary<string, Dictionary<string, int>>());
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    EntryElement entry = EntryElement.GetEntryFromString(line);
                    string trueValue;
                    switch (classificationTarget)
                    {
                        case 0:
                            trueValue = entry.debitAccount;
                            break;
                        case 1:
                            trueValue = entry.creditAccount;
                            break;
                        case 2:
                            trueValue = entry.debitAccount + entry.creditAccount;
                            break;
                        default:
                            trueValue = entry.debitAccount;
                            break;
                    }
                    if (string.IsNullOrWhiteSpace(entry.buyer) || entry.buyer == "NONE")
                    {
                        entry.buyer = "NaN";
                    }
                    if (string.IsNullOrWhiteSpace(entry.supplier) || entry.supplier == "NONE")
                    {
                        entry.supplier = "NaN";
                    }
                    if (!trainData.ContainsKey(entry.buyer)) trainData.Add(entry.buyer, new Dictionary<string, Dictionary<string, int>>());
                    if (!trainData[entry.buyer].ContainsKey(entry.supplier)) trainData[entry.buyer].Add(entry.supplier, new Dictionary<string, int>());
                    if (!trainData[entry.buyer][entry.supplier].ContainsKey(trueValue)) trainData[entry.buyer][entry.supplier].Add(trueValue, 1);
                    else trainData[entry.buyer][entry.supplier][trueValue]++;

                    if (entry.buyer != "NaN")
                    {
                        if (!trainData.ContainsKey("NaN")) trainData.Add("NaN", new Dictionary<string, Dictionary<string, int>>());
                        if (!trainData["NaN"].ContainsKey(entry.supplier)) trainData["NaN"].Add(entry.supplier, new Dictionary<string, int>());
                        if (!trainData["NaN"][entry.supplier].ContainsKey(trueValue)) trainData["NaN"][entry.supplier].Add(trueValue, 1);
                        else trainData["NaN"][entry.supplier][trueValue]++;

                        if (!trainData.ContainsKey(entry.buyer)) trainData.Add(entry.buyer, new Dictionary<string, Dictionary<string, int>>());
                        if (!trainData[entry.buyer].ContainsKey("NaN")) trainData[entry.buyer].Add("NaN", new Dictionary<string, int>());
                        if (!trainData[entry.buyer]["NaN"].ContainsKey(trueValue)) trainData[entry.buyer]["NaN"].Add(trueValue, 1);
                        else trainData[entry.buyer]["NaN"][trueValue]++;
                    }
                    else if (entry.supplier != "NaN")
                    {
                        if (!trainData.ContainsKey(entry.buyer)) trainData.Add(entry.buyer, new Dictionary<string, Dictionary<string, int>>());
                        if (!trainData[entry.buyer].ContainsKey("NaN")) trainData[entry.buyer].Add("NaN", new Dictionary<string, int>());
                        if (!trainData[entry.buyer]["NaN"].ContainsKey(trueValue)) trainData[entry.buyer]["NaN"].Add(trueValue, 1);
                        else trainData[entry.buyer]["NaN"][trueValue]++;
                    }
                    //if (!trainData.ContainsKey("NaN")) trainData.Add("NaN", new Dictionary<string, Dictionary<string, int>>());
                    //if (!trainData["NaN"].ContainsKey("NaN")) trainData["NaN"].Add("NaN", new Dictionary<string, int>());
                    //if (!trainData["NaN"]["NaN"].ContainsKey(trueValue)) trainData["NaN"]["NaN"].Add(trueValue, 1);
                    //else trainData["NaN"]["NaN"][trueValue]++;                    
                }
                sr.Close();
                foreach (string buyer in trainData.Keys)
                {
                    if (!maxFreq.ContainsKey(buyer)) maxFreq.Add(buyer, new Dictionary<string, string>());
                    foreach (string supplier in trainData[buyer].Keys)
                    {
                        List<KeyValuePair<string, int>> accounts = trainData[buyer][supplier].ToList();
                        accounts.Sort((x, y) => (y.Value.CompareTo(x.Value)));
                        maxFreq[buyer].Add(supplier, accounts[0].Key);
                    }
                }

                double correctTwoR = 0;
                double total = 0;
                double correctZeroR = 0;

                sr = new StreamReader(devDataPath, Encoding.UTF8);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    EntryElement entry = EntryElement.GetEntryFromString(line);

                    if (maskBuyer || string.IsNullOrWhiteSpace(entry.buyer) || entry.buyer == "NULL" || !maxFreq.ContainsKey(entry.buyer)) entry.buyer = "NaN";
                    if (maskSupplier || string.IsNullOrWhiteSpace(entry.supplier) || entry.supplier == "NULL" || !maxFreq[entry.buyer].ContainsKey(entry.supplier)) entry.supplier = "NaN";
                    string trueValue = null;
                    switch (classificationTarget)
                    {
                        case 0:
                            trueValue = entry.debitAccount;
                            break;
                        case 1:
                            trueValue = entry.creditAccount;
                            break;
                        case 2:
                            trueValue = entry.debitAccount + entry.creditAccount;
                            break;
                        default:
                            trueValue = entry.debitAccount;
                            break;
                    }
                    string twoRPrediction = maxFreq[entry.buyer][entry.supplier];
                    string zeroRPrediction = maxFreq["NaN"]["NaN"];
                    if (twoRPrediction == trueValue) correctTwoR++;
                    if (zeroRPrediction == trueValue) correctZeroR++;
                    total++;
                }
                Console.WriteLine("ZeroR: " + (correctZeroR / total).ToString("0.0000"));
                Console.WriteLine("TwoR: " + (correctTwoR / total).ToString("0.0000"));
                sr.Close();
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: .\\PerceptronClassificationForAccounting.exe [Arguments]");
            Console.WriteLine("Arguments (T - for the \"train\" method, I - for \"test\" (inference) method,");
            Console.WriteLine("O - for the ZeroR/OneR/TwoR method, S - for the feature statistics method;");
            Console.WriteLine("_M - mandatory):");
            Console.WriteLine("  -i [path]              - training data path (T_M, O_M) or test data path");
            Console.WriteLine("                           (I_M)");
            Console.WriteLine("  -d [path]              - validation data path (T_M) or test data (O_M)");
            Console.WriteLine("  -o [path]              - output data path (I_M)");
            Console.WriteLine("  -mo [path]             - model output path (T_M)");
            Console.WriteLine("  -m [method]            - method - one of \"train\" (default), \"test\" (I_M),");
            Console.WriteLine("                           \"get-feature-statistics\" (S_M),");
            Console.WriteLine("                           \"get-one-r-and-zero-r-scores\" (O_M)");
            Console.WriteLine("  --skip-amount          - skips amount feature if used (T)");
            Console.WriteLine("  --skip-amount-r0       - skips amount rounded to ones if used (T)");
            Console.WriteLine("  --skip-amountr-10      - skips amount rounded to tens if used (T)");
            Console.WriteLine("  --skip-amount-r100     - skips amount rounded to hundreds if used (T)");
            Console.WriteLine("  --skip-doc-amount      - skips document amount feature if used (T)");
            Console.WriteLine("  --skip-doc-amount-r0   - skips document amount rounded to ones if used (T)");
            Console.WriteLine("  --skip-doc-amountr-10  - skips document amount rounded to tens if used (T)");
            Console.WriteLine("  --skip-doc-amount-r100 - skips document amount rounded to hundreds if used (T)");
            Console.WriteLine("  --skip-proportion      - skips proportion feature if used (T)");
            Console.WriteLine("  --skip-buyer           - skips buyer feature if used (T)");
            Console.WriteLine("  --skip-buyer-nace      - skips buyer NACE code feature if used (T)");
            Console.WriteLine("  --skip-supplier        - skips supplier feature if used (T)");
            Console.WriteLine("  --skip-supplier-nace   - skips supplier NACE code feature if used (T)");
            Console.WriteLine("  --skip-year            - skips year feature if used (T)");
            Console.WriteLine("  --skip-reverse-vat     - skips reverse VAT indicator feature if used (T)");
            Console.WriteLine("  --skip-currency        - skips currency feature if used (T)");
            Console.WriteLine("  --skip-comment         - skips row comment features if used (T)");
            Console.WriteLine("  --skip-doc-comment     - skips document comment features if used (T)");
            Console.WriteLine("  --skip-doc-series      - skips document series feature if used (T)");
            Console.WriteLine("  -s                     - adds three synthetic entries with masked buyers,");
            Console.WriteLine("                           suppliers, and buyers and suppliers for each training");
            Console.WriteLine("                           data entry if used (T)");
            Console.WriteLine("  -s2                    - adds four synthetic entries with masked buyers,");
            Console.WriteLine("                           suppliers, buyer NACE codes, and supplier NACE codes");
            Console.WriteLine("                           (requires also -s to be specified, however, it");
            Console.WriteLine("                           overrides -s behaviour) (T)");
            Console.WriteLine("  -sn                    - skips all numeric features if used (T)");
            Console.WriteLine("  -a                     - instead of the most probable class, prints also all");
            Console.WriteLine("                           prediction scores of all other classes (I)");
            Console.WriteLine("  -mb                    - masks buyer if used (T, I, O); this can be used to");
            Console.WriteLine("                           calculate OneR scores - the TwoR for cases when");
            Console.WriteLine("                           buyers are masked is equal to OneR.");
            Console.WriteLine("  -ms                    - masks supplier if used (T, I, O)");
            Console.WriteLine("  --stem-words           - whether to apply stemming to comment features (T, I)");
            Console.WriteLine("  -l [language]          - language to use for stemming (T, I)");
            Console.WriteLine("  --stop-words [path]    - path of the stop-words file (T, I)");
            Console.WriteLine("  --use-null             - whether to use the \"null\" value for unknown/undefined");
            Console.WriteLine("                           buyers and suppliers instead of the feature \"NONE\"");
            Console.WriteLine("                           (i.e., either no feature triggers, or \"NONE\"");
            Console.WriteLine("                           triggers) (T, I)");
            Console.WriteLine("  -mi [path]             - modelInPath (I_M, S_M)");
            Console.WriteLine("  -e [number]            - the maximum number of epochs to run during training");
            Console.WriteLine("                           (default: 10) (T)");
            Console.WriteLine("  -c [target]            - the classification target to train to predict; one");
            Console.WriteLine("                           of \"debit\", \"credit\", or \"combined\" (default debit)");
            Console.WriteLine("                           (T, O)");
        }
    }
}
