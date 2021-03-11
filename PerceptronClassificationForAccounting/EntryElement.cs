using System;
using System.Globalization;

namespace PerceptronLibrary
{
    public class EntryElement
    {
        public string year;
        public string buyer;
        public string supplier;
        public string supplierNace;
        public string buyerNace;
        public double proportion;
        public string someWeirdFeature;
        public double docAmount;
        public double rowAmount;
        public string debitAccount;
        public string creditAccount;
        public string currency;
        public string comment;
        public string docSeries;
        public string docComment;

        private static char[] sep = { '\t'};
        private static NumberFormatInfo nfi = null;
        public static EntryElement GetEntryFromString(string entryString)
        {
            if (nfi==null)
            {
                nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                nfi.CurrencyDecimalSeparator = ".";
                nfi.PercentDecimalSeparator = ".";
            }
            if (string.IsNullOrWhiteSpace(entryString)) return null;
            EntryElement res = new EntryElement();
            string[] arr = entryString.Split(sep, System.StringSplitOptions.None);
            if (arr.Length != 19) return null;
            res.year = arr[0].Trim();
            res.buyer = arr[1].Trim();
            res.buyerNace = arr[2].Trim();
            res.supplier = arr[3].Trim();
            res.supplierNace = arr[4].Trim();
            res.proportion = Convert.ToDouble(arr[5],nfi);
            res.docAmount = Convert.ToDouble(arr[6], nfi);
            res.someWeirdFeature = arr[7].Trim();
            res.debitAccount = arr[8].Trim();
            res.creditAccount = arr[9].Trim();
            res.rowAmount = Convert.ToDouble(arr[10], nfi);
            //11, 12, 13 - rounded to 1, 10, 100
            res.currency = arr[11].Trim();//14
            //15 - doc Nr
            res.docSeries = arr[12].Trim();//16
            res.comment = arr[13].Trim();//17
            if (!string.IsNullOrWhiteSpace(res.comment.ToLower()))
            {
                res.comment = CreateIDFTable.GetValidString(res.comment.ToLower());
            }
            res.docComment = arr[14].Trim();//18
            if (!string.IsNullOrWhiteSpace(res.docComment))
            {
                res.docComment = CreateIDFTable.GetValidString(res.docComment.ToLower());
            }
            return res;
        }
    }
}