using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ErpContent.Views.Helpers
{
    public class NotesHelper
    {

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

            string evaluatedForIndention = evaluateParagraphIndention(evaluatedFroDotsAndSpaces);

            string formattedNotes = evaluatedForIndention; 

            return formattedNotes;
        }

        /**
         * This private method exectues the process of evaluating for spaces after each dot. 
         * The evaluation is based on our documented rules. This is designed to be called from 
         * the publicly accessible method "applyFormatting"
         * 
         * @param       String      input
         * 
         * @return      String      evaluatedString      This represent the string that has been modified based on set of writing rules
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
            //string evaluatedString = "";

            // Removes all spaces at the beginning of each paragraph

            string withSpacesRemoved = Regex.Replace(input, "^( )*", "");

            return withSpacesRemoved; 
        }
    }
}