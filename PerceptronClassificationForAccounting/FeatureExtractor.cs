using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerceptronLibrary
{
    public abstract class FeatureExtractor
    {
        public abstract string Name { get; set; }
        //public abstract string GetValueFromParallel(List<TokenElement> sourceTokens, List<TokenElement> targetTokens, int? sourceIdx = null, int? targetIdx = null);
        public abstract List<string> GetValuesFromEntry(EntryElement entry);

        public override string ToString()
        {
            return Name;
        }
    }
}
