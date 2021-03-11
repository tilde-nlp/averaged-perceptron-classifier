//
//  Stemmers.cs
//
//  Author:
//       Mārcis Pinnis <marcis.pinnis@gmail.com>
//
//  Copyright (c) 2014 Mārcis Pinnis, SIA Tilde
// 
//  This program is the property of SIA Tilde.
//  In order to use this program for any purpose (commercial or non-commercial),
//	a license has to be acquired from SIA Tilde.
//
//  This program is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace POSTaggingModule
{
    public abstract class Stemmer
    {
        public virtual List<string> Stem(string text, bool onlyLowercase = false)
        {
            List<string> stemmed = new List<string>();
            int pos = -1;

            int i;
            for (i = 0; i < text.Length; i++)
            {
                if (char.IsWhiteSpace(text[i]))
                {
                    if (pos != -1 && i - pos > 0)
                    {
                        stemmed.Add(onlyLowercase ? text.Substring(pos, i - pos).ToLower() : StemWord(text.Substring(pos, i - pos)));
                    }
                    pos = -1;
                }
                else
                {
                    if (pos == -1)
                        pos = i;
                }
            }

            if (pos != -1 && i - pos > 0)
            {
                stemmed.Add(onlyLowercase ? text.Substring(pos, i - pos).ToLower() : StemWord(text.Substring(pos, i - pos)));
            }

            return stemmed;
        }

        public abstract string StemWord(string word);

        public static Stemmer GetLightStemmer(string lang)
        {
            switch (lang)
            {
                case "en": return new EnglishStemmer();
                case "lv": return new LatvianStemmer();
                default: return new GenericStemmer();
            }
        }
    }

    public class GenericStemmer : Stemmer
    {
        public override string StemWord(string word)
        {
            word = word.ToLower();

            if (word.Length < 4)
                return word;
            if (word.Length == 4)
                return word.Substring(0, word.Length - 1);
            if (word.Length < 7)
                return word.Substring(0, word.Length - 2);
            return word.Substring(0, word.Length - 3);
        }
    }

    public class EnglishStemmer : Stemmer
    {
        private readonly static Dictionary<string, bool> esEndings = "xes'|xes|sses'|sses|shes|ches|oes|shes'|ches'|oes'".Split('|').Distinct().ToDictionary(w => w, w => true);
        private readonly static Dictionary<string, bool> endings = "s's|ing|'s|s'|ed|s|'".Split('|').Distinct().ToDictionary(w => w, w => true);
        private readonly static int MaxEsEndingLength = esEndings.Keys.Max(e => e.Length);
        private readonly static int MinEsEndingLength = esEndings.Keys.Min(e => e.Length);
        private readonly static int MaxEndingLength = endings.Keys.Max(e => e.Length);
        private readonly static int MinEndingLength = endings.Keys.Min(e => e.Length);
        private const int MinStemLength = 2;

        public override string StemWord(string word)
        {
            word = word.ToLower();

            for (int i = MaxEsEndingLength; i >= MinEsEndingLength; i--)
            {
                if (word.Length >= MinStemLength + i && esEndings.ContainsKey(word.Substring(word.Length - i)))
                {
                    int idx = word.Substring(word.Length - i).IndexOf('e');
                    return word.Substring(0, word.Length - i + idx);
                }
            }

            for (int i = MaxEndingLength; i >= MinEndingLength; i--)
            {
                if (word.Length >= MinStemLength + i && endings.ContainsKey(word.Substring(word.Length - i)))
                {
                    return word.Substring(0, word.Length - i);
                }
            }

            return word;
        }
    }

    public class LatvianStemmer : Stemmer
    {
        private readonly static Dictionary<string, bool> words = "ārā|cik|kad|maz|pus|rīt|sen|šad|šur|tur|žēl|kur|jau|tad|vēl|tik|pie|pēc|gar|par|pār|bez|aiz|zem|dēļ|lai|vai|arī|gan|bet|jeb|būt|esi|būs|kas|kam|kur".Split('|').Distinct().ToDictionary(w => w, w => true);
        private readonly static Dictionary<string, bool> endings = "iem|ais|ies|iet|am|as|ai|ām|ās|os|ie|es|em|ēm|ēs|ij|īm|is|īs|us|um|im|āt|at|it|a|ā|e|ē|i|ī|m|o|s|š|t|u|ū".Split('|').Distinct().ToDictionary(w => w, w => true);
        private readonly static int MaxEndingLength = endings.Keys.Max(e => e.Length);
        private readonly static int MinEndingLength = endings.Keys.Min(e => e.Length);
        private const int MinStemLength = 2;

        public override string StemWord(string word)
        {
            word = word.ToLower();

            if (words.ContainsKey(word))
                return word;

            for (int i = MaxEndingLength; i >= MinEndingLength; i--)
            {
                if (word.Length >= MinStemLength + i && endings.ContainsKey(word.Substring(word.Length - i)))
                {
                    return word.Substring(0, word.Length - i);
                }
            }

            return word;
        }
    }
}
