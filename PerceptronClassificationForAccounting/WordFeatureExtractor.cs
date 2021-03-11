using POSTaggingModule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerceptronLibrary
{
    public class WordFeatureExtractor : FeatureExtractor
    {
        private string _name = null;
        private bool _useBuyer = true;
        private bool _useBuyerNace = true;
        private bool _useSupplier = true;
        private bool _useSupplierNace = true;
        private bool _useYear = true;
        private bool _useReverseVat = true;
        private bool _useCurrency = true;
        private bool _useComment = true;
        private bool _useDocComment = true;
        private bool _useDocSeries = true;
        private bool _stemWords = false;
        private string _language = "lv";
        private HashSet<string> _stopWords = new HashSet<string>();
        private Stemmer _stemmer = null;
        public WordFeatureExtractor()
        {
            _name = "w";
        }
        public WordFeatureExtractor(string name)
        {
            _name = name;
        }

        public void LoadStemmerAndStopwords(bool stemWords, string language, string stopWordFile)
        {
            _stemWords = stemWords;
            _language = language;
            _stemmer = Stemmer.GetLightStemmer(_language);
            if (!string.IsNullOrWhiteSpace(stopWordFile))
            {
                StreamReader sr = new StreamReader(stopWordFile, Encoding.UTF8);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine().Trim().ToLower();
                    if (_stemWords)
                    {
                        line = _stemmer.StemWord(line);
                    }
                    if (!_stopWords.Contains(line)) _stopWords.Add(line);
                }
            }
        }
        public WordFeatureExtractor(string name, bool useBuyer, bool useBuyerNace, bool useSupplier, bool useSupplierNace, bool useYear, bool useReverseVat, bool useCurrency, bool useDocSeries, bool useComment, bool useDocComment, bool stemWords, string language, string stopWordFile)
        {
            _useBuyer = useBuyer;
            _useBuyerNace = useBuyerNace;
            _useSupplier = useSupplier;
            _useSupplierNace = useSupplierNace;
            _useYear = useYear;
            _useReverseVat = useReverseVat;
            _useCurrency = useCurrency;
            _name = name;
            _useComment = useComment;
            _useDocComment = useDocComment;
            _useDocSeries = useDocSeries;
            _stemWords = stemWords;
            _language = language;
            _stemmer = Stemmer.GetLightStemmer(_language);
            if (!string.IsNullOrWhiteSpace(stopWordFile))
            {
                StreamReader sr = new StreamReader(stopWordFile, Encoding.UTF8);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine().Trim().ToLower();
                    if (_stemWords)
                    {
                        line = _stemmer.StemWord(line);
                    }
                    if (!_stopWords.Contains(line)) _stopWords.Add(line);
                }
            }
        }

        public override string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private char[] _sep = { ' ' };
        private Dictionary<string, string> _stemDict = new Dictionary<string, string>();
        public override List<string> GetValuesFromEntry(EntryElement entry)
        {
            HashSet<string> res = new HashSet<string>();
            if (_useBuyer&&!string.IsNullOrWhiteSpace(entry.buyer))
            {
                StringBuilder current = new StringBuilder(50);
                current.Append(Name);
                current.Append("_b_");
                current.Append(entry.buyer);
                if (!res.Contains(current.ToString()))res.Add(current.ToString());
            }
            if (_useBuyerNace&&!string.IsNullOrWhiteSpace(entry.buyerNace))
            {
                StringBuilder current = new StringBuilder(50);
                current.Append(Name);
                current.Append("_bn_");
                current.Append(entry.buyerNace);
                if (!res.Contains(current.ToString())) res.Add(current.ToString());
            }
            if (_useSupplier&&!string.IsNullOrWhiteSpace(entry.supplier))
            {
                StringBuilder current = new StringBuilder(50);
                current.Append(Name);
                current.Append("_s_");
                current.Append(entry.supplier);
                if (!res.Contains(current.ToString())) res.Add(current.ToString());
            }
            if (_useSupplierNace&&!string.IsNullOrWhiteSpace(entry.supplierNace))
            {
                StringBuilder current = new StringBuilder(50);
                current.Append(Name);
                current.Append("_sn_");
                current.Append(entry.supplierNace);
                if (!res.Contains(current.ToString())) res.Add(current.ToString());
            }
            if (_useYear&&!string.IsNullOrWhiteSpace(entry.year))
            {
                StringBuilder current = new StringBuilder(50);
                current.Append(Name);
                current.Append("_y_");
                current.Append(entry.year);
                if (!res.Contains(current.ToString())) res.Add(current.ToString());
            }
            if (_useReverseVat&&!string.IsNullOrWhiteSpace(entry.someWeirdFeature))
            {
                StringBuilder current = new StringBuilder(50);
                current.Append(Name);
                current.Append("_swf_");
                current.Append(entry.someWeirdFeature);
                if (!res.Contains(current.ToString())) res.Add(current.ToString());
            }
            if (_useCurrency && !string.IsNullOrWhiteSpace(entry.currency))
            {
                StringBuilder current = new StringBuilder(50);
                current.Append(Name);
                current.Append("_c_");
                current.Append(entry.currency);
                if (!res.Contains(current.ToString())) res.Add(current.ToString());
            }
            if (_useDocSeries && !string.IsNullOrWhiteSpace(entry.docSeries))
            {
                StringBuilder current = new StringBuilder(50);
                current.Append(Name);
                current.Append("_ds_");
                current.Append(entry.docSeries);
                if (!res.Contains(current.ToString())) res.Add(current.ToString());
            }
            if (_useComment && !string.IsNullOrWhiteSpace(entry.comment))
            {
                StringBuilder current = new StringBuilder(50);
                current.Append(Name);
                current.Append("_ec_");
                string[] tokens = entry.comment.Split(_sep, StringSplitOptions.RemoveEmptyEntries);
                foreach (string token in tokens)
                {
                    string key = null;
                    if (_stemWords)
                    {
                        if (_stemDict.ContainsKey(token)) key=_stemDict[token];
                        else
                        {
                            //Console.WriteLine("\t\t\texecuting stem");
                            string stem = _stemmer.StemWord(token);
                            //Console.WriteLine("\t\t\t done!");
                            _stemDict.Add(token, stem);
                        }
                    }
                    else
                    {
                        key = token;
                    }
                    if (!_stopWords.Contains(key))
                    {
                        string tokFeature = current.ToString() + key;
                        if (!res.Contains(tokFeature)) res.Add(tokFeature);
                    }
                }
            }
            if (_useDocComment && !string.IsNullOrWhiteSpace(entry.docComment))
            {
                StringBuilder current = new StringBuilder(50);
                current.Append(Name);
                current.Append("_dc_");
                string[] tokens = entry.docComment.Split(_sep, StringSplitOptions.RemoveEmptyEntries);
                foreach (string token in tokens)
                {
                    string key = null;
                    if (_stemWords)
                    {
                        if (_stemDict.ContainsKey(token)) key = _stemDict[token];
                        else
                        {
                            //Console.WriteLine("\t\t\texecuting stem");
                            string stem = _stemmer.StemWord(token);
                            //Console.WriteLine("\t\t\t done!");
                            _stemDict.Add(token, stem);
                        }
                    }
                    else
                    {
                        key = token;
                    }
                    if (!_stopWords.Contains(key))
                    {
                        string tokFeature = current.ToString() + key;
                        if (!res.Contains(tokFeature)) res.Add(tokFeature);
                    }
                }
            }

            return res.ToList();
        }
    }
}
