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

        static List<String> _wordsToItalize;
        static List<String> _wordsToHypenate;
        static List<String> _wordsToCapitalize; 
        static List<String> _wordsToRepalceWithSomething; 

        static NotesHelper()
        {

            //_wordsToItalize = new List<String>();

            //_wordsToItalize.Add("élevage");
            //_wordsToItalize.Add("demi-muid");   
            //_wordsToItalize.Add("bouquet garni");
            //_wordsToItalize.Add("négociant");
            //_wordsToItalize.Add("lieu-dit");
            //_wordsToItalize.Add("vin de pays");
            //_wordsToItalize.Add("mélange");
            //_wordsToItalize.Add("cepage");
            //_wordsToItalize.Add("batonnage");
            //_wordsToItalize.Add("sur lie");
            //_wordsToItalize.Add("garrigue");
            //_wordsToItalize.Add("tour de force");
            //_wordsToItalize.Add("vigneron");
            //_wordsToItalize.Add("creme de cassis");
            //_wordsToItalize.Add("sur-maturité");
            //_wordsToItalize.Add("patisserie");
            //_wordsToItalize.Add("pain grillé");
            //_wordsToItalize.Add("terroir");
            //_wordsToItalize.Add("vigneron");
            //_wordsToItalize.Add("cru");
            //_wordsToItalize.Add("crus");
            //_wordsToItalize.Add("vignoble");
            //_wordsToItalize.Add("bodega");
            //_wordsToItalize.Add("trockenheit");
            //_wordsToItalize.Add("feinherb");
            //_wordsToItalize.Add("mirabelle");
            //_wordsToItalize.Add("cantus firmus");
            //_wordsToItalize.Add("oechsle");
            //_wordsToItalize.Add("trocken");
            //_wordsToItalize.Add("halbtrocken");
            //_wordsToItalize.Add("spaetlese");
            //_wordsToItalize.Add("auslese");
            //_wordsToItalize.Add("griotte");
            //_wordsToItalize.Add("clos");
            //_wordsToItalize.Add("pigeage");
            //_wordsToItalize.Add("monopole");
            //_wordsToItalize.Add("tonneliers");
            //_wordsToItalize.Add("tirage");
            //_wordsToItalize.Add("veraison");
            //_wordsToItalize.Add("inter-alia");
            //_wordsToItalize.Add("oidium");
            //_wordsToItalize.Add("millerandage");
            //_wordsToItalize.Add("rancio");
            //_wordsToItalize.Add("solero");
            //_wordsToItalize.Add("herbs de provence");
            //_wordsToItalize.Add("saignée");
            //_wordsToItalize.Add("négoçe");
            //_wordsToItalize.Add("lutte raisonée");
            //_wordsToItalize.Add("en route");
            //_wordsToItalize.Add("jus");
            //_wordsToItalize.Add("couloure");
            //_wordsToItalize.Add("vendage");

            String[] container;
            string csvFromConfig; 


            // Italization
            csvFromConfig = ConfigurationManager.AppSettings["ItalizedWords"];
            container = csvFromConfig.Split(',');
            _wordsToItalize = new List<string>(container); 

            // Capitalization
            csvFromConfig = ConfigurationManager.AppSettings["CapitalizedWords"];
            container = csvFromConfig.Split(',');
            _wordsToCapitalize = new List<string>(container);
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

            string evaluatedForItalics = evaluateForItalics(evaluatedFroDotsAndSpaces);

            string evaluatedFoCapitalizatoin = evaluateForCapitalization(evaluatedForItalics);

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
        private static string evaluateForItalics(string input)
        {
            
            foreach (String wordToLookFor in _wordsToItalize)
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
    }
}
