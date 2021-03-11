using POSTaggingModule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerceptronLibrary
{
    class CreateIDFTable
    {
        public static Dictionary<string,HashSet<int>> GetTermListFromStringList(string language, bool stemTerms, List<string> lines, out int iFiles, int maxLines = 3000000, bool mergeBPEParts = false)
        {
            iFiles = 0;
            Dictionary<string, HashSet<int>> termList = new Dictionary<string, HashSet<int>>();
            char[] sep = { '\t' };
            Console.WriteLine("Analyzing lines...");
            Stemmer stemmer = Stemmer.GetLightStemmer(language);
            foreach(string line in lines)
            {
                if (maxLines < iFiles) break;
                if (iFiles % 10000 == 0)
                {
                    Console.Write(".");
                    if (iFiles % 500000 == 0)
                    {
                        Console.WriteLine(" / " + iFiles.ToString());
                    }
                }
                string mergedLine = mergeBPEParts ? line.Replace("@@ ", "") : line;
                if (!string.IsNullOrWhiteSpace(mergedLine))
                {
                    List<string> stems = ReadInputString(mergedLine, stemTerms, stemmer);
                    if (stems != null && stems.Count > 0)
                    {
                        iFiles++;
                        foreach (string stem in stems)
                        {
                            if (!termList.ContainsKey(stem))
                            {
                                termList.Add(stem, new HashSet<int>());
                            }
                            if (!termList[stem].Contains(iFiles))
                            {
                                termList[stem].Add(iFiles);
                            }
                        }
                    }
                }
            }
            return termList;
        }
        public static Dictionary<string, HashSet<int>> ReadMonoCorpus(string language, bool stemTerms, string alignmentFile, out int iFiles, int maxLines = 3000000, bool mergeBPEParts = false)
        {
            iFiles = 0;
            Dictionary<string, HashSet<int>> termList = new Dictionary<string, HashSet<int>>();
            char[] sep = { '\t' };
            Console.WriteLine("Analyzing mono corpus file: " + alignmentFile);
            StreamReader sr = new StreamReader(alignmentFile, Encoding.UTF8, false, 1024 * 1024);
            Stemmer stemmer = Stemmer.GetLightStemmer(language);
            while (!sr.EndOfStream)
            {
                if (maxLines < iFiles) break;
                if (iFiles % 10000 == 0)
                {
                    Console.Write(".");
                    if (iFiles % 500000 == 0)
                    {
                        Console.WriteLine(" / " + iFiles.ToString());
                    }
                }
                string line = mergeBPEParts ? sr.ReadLine().Trim().Replace("@@ ", "") : sr.ReadLine().Trim();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    List<string> stems = ReadInputString(line, stemTerms, stemmer);
                    if (stems != null && stems.Count > 0)
                    {
                        iFiles++;
                        foreach (string stem in stems)
                        {
                            if (!termList.ContainsKey(stem))
                            {
                                termList.Add(stem, new HashSet<int>());
                            }
                            if (!termList[stem].Contains(iFiles))
                            {
                                termList[stem].Add(iFiles);
                            }
                        }
                    }
                }
            }
            sr.Close();
            return termList;
        }

        public static List<ValueComparablePair> CalculateIdf(Dictionary<string, HashSet<int>> termList, double files)
        {
            List<ValueComparablePair> idfList = new List<ValueComparablePair>();
            foreach (string stem in termList.Keys)
            {
                idfList.Add(new ValueComparablePair(stem, Math.Log(files / Convert.ToDouble(termList[stem].Count), Math.E)));
            }
            idfList.Sort();
            return idfList;
        }

        public static Dictionary<string, double> CalculateIdfHash(Dictionary<string, Dictionary<int, bool>> termList, double files, out double maxIdf)
        {
            maxIdf = 0;
            Dictionary<string, double> idfList = new Dictionary<string, double>();
            foreach (string stem in termList.Keys)
            {
                double idf = Math.Log(files / Convert.ToDouble(termList[stem].Count), Math.E);
                idfList.Add(stem, idf);
                if (maxIdf < idf) maxIdf = idf;
            }
            return idfList;
        }

        private static Dictionary<string, string> stemDictionary = new Dictionary<string, string>();

        private static List<string> ReadInputString(string text, bool stemTerms, Stemmer stemmer)
        {
            List<string> res = new List<string>(1000);
            string line = GetValidString(text);
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] tokens = line.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                foreach (string token in tokens)
                {
                    if (stemTerms)
                    {
                        if (stemDictionary.ContainsKey(token)) res.Add(stemDictionary[token]);
                        else
                        {
                            //Console.WriteLine("\t\t\texecuting stem");
                            string stem = stemmer.StemWord(token);
                            //Console.WriteLine("\t\t\t done!");
                            stemDictionary.Add(token, stem);
                            res.Add(stem);
                        }
                    }
                    else
                    {
                        res.Add(token);
                    }
                }
            }
            return res;
        }


        private static char[] sep = { ' ' };
        private static List<string> ReadInput(string file, string language, bool stemTerms)
        {
            Stemmer stemmer = Stemmer.GetLightStemmer(language);
            List<string> res = new List<string>(1000);
            StreamReader sr = new StreamReader(file, Encoding.UTF8, false, 1024 * 1024);
            while (!sr.EndOfStream)
            {
                res.AddRange(ReadInputString(sr.ReadToEnd().Trim(), stemTerms, stemmer));
            }
            sr.Close();
            return res;
        }

        private static char[] lineSep = { '\n', '\r', '\0' };
        public static string GetValidString(string s)
        {
            string tempString = s;
            StringBuilder res = new StringBuilder(1000);
            foreach (char c in tempString)
            {
                if (Char.IsControl(c) || (Char.IsPunctuation(c) && c != '-') || Char.IsSeparator(c) || Char.IsWhiteSpace(c))
                {
                    res.Append(" ");
                }
                else
                {
                    res.Append(c);
                }
            }
            return res.ToString().ToLower();
        }
    }

    public class ValueComparablePair : IComparable<ValueComparablePair>
    {
        public ValueComparablePair(string k, double v)
        {
            key = k;
            value = v;
            altKey = "";
        }
        public ValueComparablePair(string k, string a, double v)
        {
            key = k;
            value = v;
            altKey = a;
        }
        public string key;
        public string altKey;
        public double value;
        public int CompareTo(ValueComparablePair other)
        {
            return value.CompareTo(other.value);
        }
    }
}
