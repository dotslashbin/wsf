using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using System.Configuration;

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

                var src = pairs[i * 2];
                if (!accentMap.ContainsKey(src))
                {
                    accentMap.Add(src.ToLower(), pairs[i * 2 + 1]);
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
        public static string ReplaceToAccent(string src)
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
                        if (accentMap.ContainsKey(key))
                        {
                            newWords.Add(accentMap[key]);
                        }
                        else
                        {
                            newWords.Add(word);
                        }
                    }
                    newParts.Add(String.Join(" ", newWords));
                }
                newLines.Add(String.Join(",",newParts));
            }
            return String.Join(Environment.NewLine, newLines);
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
            string evaluatedFroDotsAndSpaces = evaluateForDotAndSpaces(notes); // Call to check for space after each period

            string evaluatedForItalizedWords = evaluateForItalizedWords(evaluatedFroDotsAndSpaces);

            string evaluatedForItalizedPhrases = evaluateForItalizedPhrases(evaluatedForItalizedWords);

            string evaluatedFoCapitalizatoin = evaluateForCapitalization(evaluatedForItalizedPhrases);

            string evaluatedForIndention = HttpUtility.HtmlDecode(evaluateForParagraphIndention(evaluatedFoCapitalizatoin));

            string formattedNotes = evaluatedForIndention; 

            return formattedNotes;
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
