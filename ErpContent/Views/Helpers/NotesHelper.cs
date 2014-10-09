using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;

namespace ErpContent.Views.Helpers
{
    public class NotesHelper
    {
        static string[] itilizedWords = {
        "élevage",
        "demi-muid", 
        "bouquet garni",
        "négociant",
        "lieu-dit",
        "lieux-dits",
        "vin de pays",
        "mélange",
        "cépage",
        "bâtonnage",
        "sur lie",
        "garrigue",
        "tour de force",
        "crème de cassis",
        "sur-maturité",
        "pâtisserie",
        "pain grillé",
        "terroir",
        "vigneron",
        "cru",
        "crus",
        "vignoble",
        "bodega",
        "trockenheit",
        "feinherb",
        "mirabelle",
        "cantus firmus",
        "Oechsle",
        "trocken", 
        "halbtrocken",
        "spatlese",
        "auslese",
        "griotte",
        "clos",
        "pigeage", 
        "monopole",
        "tonneliers",
        "tirage",
        "veraison",
        "inter-alia",
        "oidium",
        "millerandage",
        "rancio",
        "solera",
        "herbes de Provence",
        "saignée",
        "négoçe", 
        "lutte raisonée",
        "en route",
        "jus",
        "coulure",
        "vendage",
        "méthode Champenoise",
        "premier cru",
        "village cru",
        "grand cru",
        "domaine"
        };



        static List<String> _wordsToItalize;

        static NotesHelper()
        {
            _wordsToItalize = new List<String>();

            foreach (var v in itilizedWords)
            {
                _wordsToItalize.Add(v);
            }
        }

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

            string evaluatedForItalics = evaluateItalics(evaluatedFroDotsAndSpaces);

            string evaluatedForIndention = HttpUtility.HtmlDecode(evaluateParagraphIndention(evaluatedForItalics));

            string formattedNotes = evaluatedForIndention;

            return formattedNotes;
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
         * This method is aimed to automatically apply indention at the start of the paragraph. 
         * The indention is set to 3 spaces. 
         * 
         * @param       String      input
         * 
         * @return      String      evaluatedString      This represent the string that has been modified based on set of writing rules
         * 
         * @author      Joshua Fuentes      <joshua.fuentes@robertparker.com>
         */
        private static string evaluateParagraphIndention(string input)
        {
            string withSpacesRemoved = Regex.Replace(input, "^( )*", "");

            return withSpacesRemoved;
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
        private static string evaluateItalics(string input)
        {

            foreach (String wordToLookFor in _wordsToItalize)
            {
                string replacement = "<i>" + wordToLookFor + "</i>";

                input = Regex.Replace(input, wordToLookFor, replacement);
            }

            return input;
        }
    }
}
