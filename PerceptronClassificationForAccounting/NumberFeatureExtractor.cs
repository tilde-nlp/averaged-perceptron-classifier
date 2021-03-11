using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerceptronLibrary
{
    public class NumberFeatureExtractor : FeatureExtractor
    {
        private string _name = null;
        private bool _useAmount = true;
        private bool _useAmountR0 = true;
        private bool _useAmountR10 = true;
        private bool _useAmountR100 = true;
        private bool _useDocAmount = true;
        private bool _useDocAmountR0 = true;
        private bool _useDocAmountR10 = true;
        private bool _useDocAmountR100 = true;
        private bool _useProportion = true;
        public override string Name {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public NumberFeatureExtractor()
        {
            _name = "n";
        }
        public NumberFeatureExtractor(string name)
        {
            _name = name;
        }
        public NumberFeatureExtractor(string name,bool useAmount, bool useAmountR0, bool useAmountR10, bool useAmountR100, bool useProportion, bool useDocAmount, bool useDocAmountR0, bool useDocAmountR10, bool useDocAmountR100)
        {
            _useAmount = useAmount;
            _useAmountR0 = useAmountR0;
            _useAmountR10 = useAmountR10;
            _useAmountR100 = useAmountR100;
            _useDocAmount = useDocAmount;
            _useDocAmountR0 = useDocAmountR0;
            _useDocAmountR10 = useDocAmountR10;
            _useDocAmountR100 = useDocAmountR100;
            _useProportion = useProportion;
            _name = name;
        }

        public override List<string> GetValuesFromEntry(EntryElement entry)
        {
            HashSet<string> res = new HashSet<string>();

            StringBuilder current = new StringBuilder(50);
            if (_useAmount)
            {
                current.Append(Name);
                current.Append("_a_");
                current.Append(entry.rowAmount.ToString());
                res.Add(current.ToString());
            }
            if (_useAmountR0)
            {
                current = new StringBuilder(50);
                current.Append(Name);
                current.Append("_a_r_");
                current.Append(Math.Round(entry.rowAmount, 0).ToString());
                res.Add(current.ToString());
            }
            if (_useAmountR10)
            {
                current = new StringBuilder(50);
                current.Append(Name);
                current.Append("_a_r10_");
                current.Append(Math.Round(entry.rowAmount / 10, 0).ToString());
                res.Add(current.ToString());
            }
            if (_useAmountR100)
            {
                current = new StringBuilder(50);
                current.Append(Name);
                current.Append("_a_r100_");
                current.Append(Math.Round(entry.rowAmount / 100, 0).ToString());
                res.Add(current.ToString());
            }
            if (_useProportion)
            {
                current = new StringBuilder(50);
                current.Append(Name);
                current.Append("_p_");
                current.Append(entry.proportion.ToString());
                res.Add(current.ToString());
            }
            if (_useDocAmount)
            {
                current.Append(Name);
                current.Append("_da_");
                current.Append(entry.docAmount.ToString());
                res.Add(current.ToString());
            }
            if (_useDocAmountR0)
            {
                current = new StringBuilder(50);
                current.Append(Name);
                current.Append("_da_r_");
                current.Append(Math.Round(entry.docAmount, 0).ToString());
                res.Add(current.ToString());
            }
            if (_useDocAmountR10)
            {
                current = new StringBuilder(50);
                current.Append(Name);
                current.Append("_da_r10_");
                current.Append(Math.Round(entry.docAmount / 10, 0).ToString());
                res.Add(current.ToString());
            }
            if (_useDocAmountR100)
            {
                current = new StringBuilder(50);
                current.Append(Name);
                current.Append("_da_r100_");
                current.Append(Math.Round(entry.docAmount / 100, 0).ToString());
                res.Add(current.ToString());
            }
            return res.ToList();
        }
    }
}
