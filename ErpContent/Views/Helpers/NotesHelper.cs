using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using System.Configuration;
using System.Text;

namespace ErpContent.Views.Helpers
{


    public class NotesHelper
    {

        #region -- Constants and Members --
        static List<String> _phraseToItalize;
        static List<String> _wordsToHypenate;
        static List<String> _wordsToCapitalize; 
        static List<String> _wordsToRepalceWithSomething; 

        static string[] pairs = {
                "cuvee"         ,"cuvée" ,
                "Rhone"         ,"Rhône" ,
                "Rhones"        ,"Rhônes",
                "Cote"          ,"Côte" ,
                "Cotes"         ,"Côtes" ,
                "Chateau"       ,"Château" ,
                "Chateauneuf"   ,"Châteauneuf",
                "Provencal"     ,"Provençal",
                "Rotie"         ,"Rôtie",
                "Romanee",      "Romanée",
                "Echezeaux",    "Échezeaux",
                "Rose",         "Rosé",
                "Batard",       "Bâtard",
                "Bearn",        "Béarn",
                "superieur",    "supérieur",
                "Chenas",       "Chénas",
                "Costieres",    "Costières",
                "Cremant",      "Crémant",
                "Tache",        "Tâche",
                "Macon",        "Mâcon",
                "Medoc",        "Médoc",
                "Pessac-Leognan","Pessac-Léognan",
                "Fuisse"        ,"Fuissé",
                "Fume"          ,"Fumé",
                "Savennieres"   ,"Savennières",
                "Vire-Clesse"   ,"Viré-Clessé",
                "Tam"           ,"Tâm",
                "Pean"          ,"Peàn",

            };

        static string[] itilizedWords = {
            "élevage",
            "demi-muid",
            "négociant",
            "lieu-dit",
            "mélange",
            "cepage",
            "batonnage",
            "garrigue",
            "vigneron",
            "sur-maturité",
            "patisserie",
            "terroir",
            "vigneron",
          //  "cru",
          //  "crus",
            "vignoble",
            "bodega",
            "trockenheit",
            "feinherb",
            "mirabelle",
            "oechsle",
            "trocken",
            "halbtrocken",
            "spaetlese",
            "auslese",
            "griotte",
          //  "clos",
            "pigeage",
            "monopole",
            "tonneliers",
            "tirage",
            "veraison",
            "inter-alia",
            "oidium",
            "millerandage",
            "rancio",
            "solero",
            "saignée",
            "négoçe",
            "jus",
            "couloure",
            "vendage",
//            "domaine"
          };

        static string[] itilizedPhrases = {
            "bouquet garni",
            "vin de pays",
            "sur lie",
            "tour de force",
            "creme de cassis",
            "pain grillé",
            "cantus firmus",
            "herbs de provence",
            "lutte raisonée",
            "en route",
            "méthode Champenoise", 
            "premier cru", 
            "village cru", 
            "grand cru"};


        static Dictionary<string, string> accentMap = new Dictionary<string, string>();
        static Dictionary<string, string> itilizedWordsMap = new Dictionary<string, string>();
        #endregion

        #region -- Initializers --
        /// <summary>
        /// 
        /// </summary>
        private static void InitAccentMap()
        {
            accentMap.Clear();

            int pairCount = pairs.Length / 2;

            for (int i = 0; i < pairCount; i++)
            {

                var key = pairs[i * 2].ToLower();
                if (!accentMap.ContainsKey(key))
                {
                    accentMap.Add(key, pairs[i * 2 + 1]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void InitItilizedWordsMap()
        {
            itilizedWordsMap.Clear();

            int count = itilizedWords.Length;

            for (int i = 0; i < count; i++)
            {
                string key = itilizedWords[i].ToLower();

                if (!itilizedWordsMap.ContainsKey(key))
                {
                    itilizedWordsMap.Add(key, null);
                }
            }
        }
        #endregion






        /// <summary>
        /// split text into lines, sentences and finally into words. replace only words which are in accentMap, the rest of words
        /// leave without changes and combine them back to sentences and lines.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        /// 

        private delegate string[] SplitterDelegate(string src);
        private delegate string MergerDelegate(string[] src);
        private delegate string WordActionDelegate(string src);

        private static string SplitterMerger(string src, SplitterDelegate splitter, MergerDelegate merger, WordActionDelegate action )
        {
            string[] parts = splitter(src);
            if (action != null)
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] = action(parts[i]);
                }
            }
            return merger(parts);
        }


        public static bool endsWithDigit(string str)
        {
            return String.IsNullOrEmpty(str) ? false : Char.IsDigit(str[str.Length - 1]);
        }

        public static bool startsWithDigit(string str)
        {
            return String.IsNullOrEmpty(str) ? false : Char.IsDigit(str[0]);
        }


        static WordActionDelegate WordProcessorAccent = (part) =>
        {
            return SplitterMerger(part,
                (arg) => { return arg.Split(new string[] { " " }, StringSplitOptions.None); },
                (arg) => { return String.Join(" ", arg); },
                (arg) =>
                {
                    var key = arg.ToLower();
                    if (accentMap.ContainsKey(key))
                    {
                        var dest = accentMap[key];
                        //
                        // special case. source word could start with Upper case later
                        // but replacement could be in lower case, so preserve the case of the source
                        // Do it only if source is in Upper case, ddo not do that if it is in Lower case
                        //
                        if (Char.IsLower(dest[0]) && dest[0] != arg[0])
                        {
                            return arg.Substring(0, 1) + dest.Substring(1);
                        }
                        return dest;
                    }
                    return arg;
                });
        };



        public static string ReplaceToAccentPrivate(string src)
        {
            WordActionDelegate processWords = (part) =>
            {
                return SplitterMerger(part,
                    (arg) => { return arg.Split(new string[] { " " }, StringSplitOptions.None); },
                    (arg) => { return String.Join(" ", arg); },
                    (arg) =>
                    {
                        var key = arg.ToLower();
                        if (accentMap.ContainsKey(key))
                        {
                            var dest = accentMap[key];
                            //
                            // special case. source word could start with Upper case later
                            // but replacement could be in lower case, so preserve the case of the source
                            // Do it only if source is in Upper case, ddo not do that if it is in Lower case
                            //
                            if (Char.IsLower(dest[0]) && dest[0] != arg[0])
                            {
                                return arg.Substring(0, 1) + dest.Substring(1);
                            }
                            return dest;
                        }
                            return arg;
                    });
            };

            // special cases 
            //  number 1,000,000.00
            //
            WordActionDelegate processParts = (sentence) =>
            {
                return SplitterMerger(sentence,
                    (arg) => { return arg.Split(new string[] { "," }, StringSplitOptions.None); },
                    (arg) => {
                        StringBuilder sb = new StringBuilder(arg[0]);
                        for (int i = 1; i < arg.Length; i++)
                        {
                            if (startsWithDigit(arg[i-1]) && endsWithDigit(arg[i]))
                            {
                                sb.Append(",");
                                sb.Append(arg[i]);
                            }
                            else
                            {
                                sb.Append(", ");
                                sb.Append(arg[i]);
                            }
                        }
                        return sb.ToString();
                    },
                    (arg) => { return processWords(arg.Trim());
                    });
            };


            // special cases 
            //  number 1,000,000.00
            //  multiple dots ...
            //


            WordActionDelegate processSentences = (paragraph) =>
            {
                return SplitterMerger(paragraph,
                    (arg) => { return arg.Split(new string[] { "." }, StringSplitOptions.None); },
                    (arg) => {
                        StringBuilder sb = new StringBuilder(arg[0]);
                        for (int i = 1; i < arg.Length; i++)
                        {
                            if (startsWithDigit(arg[i-1]) && endsWithDigit(arg[i]))
                            {
                                sb.Append(".");
                                sb.Append(arg[i]);
                            }
                            else if (String.IsNullOrEmpty(arg[i - 1]) && String.IsNullOrEmpty(arg[i]))
                            {
                                sb.Append(".");
                                sb.Append(arg[i]);
                            }
                            else
                            {
                                sb.Append(". ");
                                sb.Append(arg[i]);
                            }
                        }
                        return sb.ToString();
                    },
                    (arg) => { return processParts(arg.Trim()); });
            };



            return  SplitterMerger(src,
                (arg) => { return arg.Split(new string[] { Environment.NewLine }, StringSplitOptions.None); },
                (arg) => { return String.Join(Environment.NewLine, arg); },
                (arg) => { return processSentences(arg); });
        }


        public static string ReplaceToAccent(string src)
        {
            return ReplaceToAccentPrivate(src);
        }

        #region -- Constructor --
        static NotesHelper()
        {
            InitAccentMap();
            InitItilizedWordsMap();

            _phraseToItalize = new List<String>();

            int count = itilizedPhrases.Length;
            for (int i = 0; i < count; i++)
            {
                _phraseToItalize.Add(itilizedPhrases[i]);
            }


            // Capitalization
            _wordsToCapitalize = new List<string>();
            _wordsToCapitalize.Add("provence");
            _wordsToCapitalize.Add("provencal");
            _wordsToCapitalize.Add("granny smith");
            _wordsToCapitalize.Add("golden delicious");
            _wordsToCapitalize.Add("chinese five spice");
            _wordsToCapitalize.Add("morello");
            _wordsToCapitalize.Add("grand cru");
            _wordsToCapitalize.Add("Pradikatswein");
            _wordsToCapitalize.Add("Kabinett");
            _wordsToCapitalize.Add("Bing cherries"); 

        }
        #endregion

        #region -- Methods --
        /**
         * This method will execute the process of applying our formatting 
         * rules to the notes or basically anything that supplied on the paramter. 
         * To execute the process, this publicly accessible method calls on to private 
         * methods that does the evaluations.
         * 
         * @param       String      notes
         * 
         * @return      String      formattedNotes      This represent the string that has passed through a serires modifications, based on our rules
         * 
         * @author      Joshua Fuentes      <joshua.fuentes@robertparker.com>
         */
        public static string applyFormatting(string notes)
        {

            string result = ReplaceToAccent(notes);

            // previous method will take care of  this formatting
            //result = evaluateForDotAndSpaces(result); // Call to check for space after each period

            result = evaluateForItalizedWords(result);

            result = evaluateForItalizedPhrases(result);

            result = evaluateForCapitalization(result);

            result = HttpUtility.HtmlDecode(evaluateForParagraphIndention(result));

            return result;
        }

        /**
         * A method to capitalize the first letter of a given word or set of words
         * 
         * @paramr          String      input
         * 
         * @return          String      
         * 
         * @author          Joshua Fuentes <joshua.fuentes@robertparker.com> 
         */ 
        private static string capitalizeFirstLetter(string input)
        {
            if (String.IsNullOrEmpty(input))
                return input;
            if (input.Length == 1)
                return input.ToUpper();
            return input.Remove(1).ToUpper() + input.Substring(1);
        }

        /**
         * This method is responsible for capitalizing the keywords provided. 
         * 
         * @param       String      input
         * 
         * @return      String      evaluatedString
         */
        private static string evaluateForCapitalization(string input)
        {
            foreach (String wordToLookFor in _wordsToCapitalize)
            {
                string replacement = capitalizeFirstLetter(wordToLookFor); 

                input = Regex.Replace(input, wordToLookFor, replacement, RegexOptions.IgnoreCase);
            }

            return input; 
        }

        /**
         * This private method exectues the process of evaluating for spaces after each dot. 
         * The evaluation is based on our documented rules. This is designed to be called from 
         * the publicly accessible method "applyFormatting"
         * 
         * @param       String              input
         * 
         * @return      String              evaluatedString      This represent the string that has been modified based on set of writing rules
         * 
         * @author      Joshua Fuentes      <joshua.fuentes@robertparker.com>
         */
        private static string evaluateForDotAndSpaces(string input)
        {
            string replaceDotWithSpace = ". ";
            string evaluatedString = Regex.Replace(input, @"\.(?! |$)", replaceDotWithSpace);

            return evaluatedString; 
        }

        /**
         * This method will replace matching words with their hypenated values
         * 
         * @param       String      input
         * 
         * @return      String      evaluatedString
         * 
         * @author      Joshua Fuente  <joshua.fuentes@robertparker.com>
         */
        private static string evaluateForHypens(String input)
        {
            return ""; 
        }

        public static string evaluateForItalizedWords(string src)
        {

            var lines = src.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var newLines = new List<string>();
            var newWords = new List<string>();
            var newParts = new List<string>();

            foreach (var line in lines)
            {
                newParts.Clear();
                var parts = line.Split(new string[] { "," }, StringSplitOptions.None);

                foreach (var part in parts)
                {
                    var words = part.Split(new string[] { " " }, StringSplitOptions.None);
                    newWords.Clear();
                    foreach (var word in words)
                    {
                        var key = word.ToLower();
                        if (itilizedWordsMap.ContainsKey(key))
                        {
                            newWords.Add("<i>" + word + "</i>");
                        }
                        else
                        {
                            newWords.Add(word);
                        }
                    }
                    newParts.Add(String.Join(" ", newWords));
                }
                newLines.Add(String.Join(",", newParts));
            }
            return String.Join(Environment.NewLine, newLines);
        }

        /**
         * This methods looks up keywords from a collection of defined workds, and implements 
         * HTML italisation on them. 
         * 
         * @param       String          input
         * 
         * @return      String          evaluatedString 
         * 
         * @author      Joshua Fuentes  <joshua.fuentes@robertpaker.com>
         */
        private static string evaluateForItalizedPhrases(string input)
        {
            
            foreach (String wordToLookFor in _phraseToItalize)
            {
                string replacement = "<i>" + wordToLookFor + "</i>";

                input = Regex.Replace(input, wordToLookFor, replacement, RegexOptions.IgnoreCase); 
            }

            return input; 
        }

        /**
        * This method is aimed to automatically apply indention at the start of the paragraph. 
        * The indention is set to 3 spaces. 
        * 
        * @param       String      input
        * 
        * @return      String      evaluatedString      This represent the string that has been modified based on set of writing rules
        * 
        * @author      Joshua Fuentes      <joshua.fuentes@robertparker.com>
        */
        private static string evaluateForParagraphIndention(string input)
        {
            string withSpacesRemoved = Regex.Replace(input, "^( )*", "");

            return withSpacesRemoved;
        }

        #endregion 
    }
}
