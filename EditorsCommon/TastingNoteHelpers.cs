using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorsCommon
{
    public class TastingNoteHelpers
    {

   

        static string[] accentsPairs = {
                "cuvee"         ,"cuvée" ,
                "Cuvee"         ,"Cuvée" ,
                "Rhone"         ,"Rhône" ,
                "Rhones"        ,"Rhônes",
                "Rhone-like"    ,"Rhône-like",   //   addition 12.03.2014
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
                "creme"         ,"crème",

                 "brulee"       ,"brulée",      //   addition 12.03.2014
                 "fumé"         ,"fumé",        //   addition 12.03.2014
                 "metayage"     ,"métayage",    //   addition 12.03.2014
                 "mineralite"   ,"mineralité",  //   addition 12.03.2014
                 "raisonee"     ,"raisonée",    //   addition 12.03.2014
                 "fruite"       ,"fruité",      //   addition 12.03.2014
                 "matiere"      ,"matière",     //   addition 12.03.2014

                 "Mineralite", "Mineralité"    //   addition 12.03.2014
            };



        static string[] englishPairs = {
            "Whilst","While",
            "Amongst","Among",
            "Colour","Color",
            "Vigour","Vigor",
            "Favourite","Favorite",
            "Savoury","Savory",
            "Centre","Center",
            "Utilise","Utilize",
            "Materialise","Materialize",
            "Commercialised","Commercialized",
            "Minimise","Minimize",
            "Maximise","Maximize",
            "Intellectualise","Intellectualize",
            "Realise","Realize",
            "Stabilise","Stabilize",
            "Persistency","Persistence",
            "n/a","not available",
            "Ha/hl","Hectoliter per hectare",
            "Sulphur","Sulfur",
            "Sulphide","Sulfide",
            "Litre","liter"
            };



        static string[] itilizedWords = {
            "auslese",
            "batonnage",
            "bodega",
            "cepage",
            "coulure",
            "demi-muid",
            "elevage",
            "élevage",
            "inter-alia",
            "lieu-dit",
            "lieux-dits", 
            "mélange",
            "melange",
            "millerandage",
            "mirabelle",
            "monopole",
            "négociant",
            "négoçe",
            "negociant",
            "négociant",
            "halbtrocken",
            "garrigue",
            "griotte",
            "oechsle",
            "pain grille", 
            "patisserie",
            "pigeage",
            "terroir", 
            "spaetlese",
            "sur-maturité",
            "sur-maturite",
            "tirage",
            "veraison",
            "oidium",
            "oechesle",
            "rancio",
            "solero",
            "saignée",
            "saignee",
            "spatlese",
            "jus",
            "trockenheit",
            "feinherb",
            "tonneliers",
            "trocken",
            "vendage",
            "vignoble",
            "vigneron",

            "Terroir-driven"       //   addition 12.03.2014
          };


        /// <summary>
        /// will require exact match. leters and cases
        /// </summary>
        static string[] itilizedPhrases = {
            "bouquet garni",
            "vin de pays",
            "sur lie",
            "tour de force",
            "creme de cassis",
            "crème de cassis",
            "pain grillé",
            "cantus firmus",
            "herbes de Provence",
            "lutte raisonée",
            "lutte raisonee",
            "Lutte raisonee", 
            "Lutte raisonée", 
            "en route",
            "méthode Champenoise", 
            "premier cru", 
            "village cru", 
            "grand cru",

            "petit pois",       //   addition 12.03.2014
            "lutte raisonée",   //   addition 12.03.2014
            "sous bois",        //   addition 12.03.2014
            "je ne sais quoi",  //   addition 12.03.2014
            "crème brulée",     //   addition 12.03.2014
            "vin de réserve",   //   addition 12.03.2014
            "Vin de table",     //   addition 12.03.2014
            "Bouquet garni"    //   addition 12.03.2014
             };



        static Dictionary<string, string> accentMap = new Dictionary<string, string>();
        static Dictionary<string, string> englishMap = new Dictionary<string, string>();
        static Dictionary<string, string> itilizedWordsMap = new Dictionary<string, string>();
        static Dictionary<string, string[]> itilizedPhrasesMap = new Dictionary<string, string[]>();


        static TastingNoteHelpers()
        {
            InitAccentMap();
            InitEnglishMap();
            InitItilizedWordsMap();
            InitItilizedPhrasesMap();
        }

        private static void InitEnglishMap()
        {
            englishMap.Clear();

            int pairCount = englishPairs.Length / 2;

            for (int i = 0; i < pairCount; i++)
            {
                var key = englishPairs[i * 2];
                if (!englishMap.ContainsKey(key))
                {
                    englishMap.Add(key, englishPairs[i * 2 + 1]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void InitAccentMap()
        {
            accentMap.Clear();

            int pairCount = accentsPairs.Length / 2;

            for (int i = 0; i < pairCount; i++)
            {

                var key = accentsPairs[i * 2];
                if (!accentMap.ContainsKey(key))
                {
                    accentMap.Add(key, accentsPairs[i * 2 + 1]);
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


        /// <summary>
        /// 
        /// </summary>
        private static void InitItilizedPhrasesMap()
        {
            itilizedPhrasesMap.Clear();

            int count = itilizedPhrases.Length;
            char[] splitter = {' '};

            for (int i = 0; i < count; i++)
            {
                string[] words = itilizedPhrases[i].Split(splitter);

                string key = words[0];
                if (!itilizedPhrasesMap.ContainsKey(key))
                {
                    for (int j = 0; j < words.Length; j++)
                    {
                        words[j] = words[j]; 
                    }
                    itilizedPhrasesMap.Add(key, words);
                }
            }
        }




        /// <summary>
        /// split text into lines, sentences and finally into words. replace only words which are in accentMap, the rest of words
        /// leave without changes and combine them back to sentences and lines.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        /// 

        private delegate string[] SplitterDelegate(string src);
        private delegate string MergerDelegate(string[] src);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src">contains string for processing</param>
        /// <param name="last">says if the giving string is last in the processing queue and sygnal the processor to flush all
        /// unfinished material to output</param>
        /// <returns></returns>
        private delegate string PartActionDelegate(string src, bool last);

  

        static bool endsWithDigit(string str)
        {
            return String.IsNullOrEmpty(str) ? false : Char.IsDigit(str[str.Length - 1]);
        }

        static bool startsWithDigit(string str)
        {
            return String.IsNullOrEmpty(str) ? false : Char.IsDigit(str[0]);
        }


        private static string SplitterMerger(string src, SplitterDelegate splitter, MergerDelegate merger, PartActionDelegate action)
        {
            string[] parts = splitter(src);
            if (action != null)
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] = action(parts[i], i == (parts.Length -1));
                }
            }
            return merger(parts);
        }


        /// <summary>
        /// check if input match to words or phrases to itilized
        /// </summary>
        static PartActionDelegate WordProcessorItilic = (part, last) =>
        {
            Queue<string> queue = new Queue<string>();
            string[] phraseToMatch = null;
            int      phraseToMatchIndex = 0;
            char[]   delimiters = {};

            return SplitterMerger(part,
                (arg) => { return arg.Split(delimiters, StringSplitOptions.None); },
//                (arg) => { return arg.Split(new string[] { " " }, StringSplitOptions.None); },
                (arg) => { return String.Join(" ", arg); },
                (arg, lastLocal) =>
                {
                    var key = arg.ToLower();
                    StringBuilder sb = new StringBuilder("");

                    //
                    // itilized phrases are case sensative
                    //
                    if (phraseToMatch != null)
                    {
                       if( arg.CompareTo( phraseToMatch[ phraseToMatchIndex] ) == 0){
                           queue.Enqueue(arg);

                           phraseToMatchIndex++;

                           //
                           // complete match
                           //
                           if (phraseToMatch.Length == phraseToMatchIndex)
                           {
                               phraseToMatch = null;
                               phraseToMatchIndex = 0;


                               sb.Append("<i>").Append(String.Join(" ", queue.ToArray())).Append("</i>");

                               queue.Clear();
                               return sb.ToString();
                           }


                           if (lastLocal)
                           {
                               phraseToMatch = null;
                               phraseToMatchIndex = 0;
                               queue.Clear();
                               sb.Append(String.Join(" ", queue.ToArray()));
                           }

                           return sb.ToString();
                       }
                    }

                    if (queue.Count > 0)
                    {
                        sb.Append(String.Join(" ", queue.ToArray()));
                        sb.Append(" ");

                        phraseToMatch = null;
                        phraseToMatchIndex = 0;
                        queue.Clear();
                    }


                    if (!lastLocal)
                    {
                        if (itilizedPhrasesMap.ContainsKey(arg))
                        {
                            phraseToMatch = itilizedPhrasesMap[arg];
                            phraseToMatchIndex = 1;
                            queue.Enqueue(arg);
                            return sb.ToString();
                        }
                    }



                    if (itilizedWordsMap.ContainsKey(key))
                    {
                        sb.Append("<i>").Append(arg).Append("</i>");
                    }
                    else
                    {
                        sb.Append(arg);
                    }

                    return sb.ToString(); ;
                });
        };



        static PartActionDelegate WordProcessorEnglish = (part, last) =>
        {
            return SplitterMerger(part,
                (arg) => { return arg.Split(new string[] { " " }, StringSplitOptions.None); },
                (arg) => { return String.Join(" ", arg); },
                (arg, lastLocal) =>
                {
                    var key = arg;
                    if ( englishMap.ContainsKey(key))
                    {
                        return englishMap[key];
                    }
                    return arg;
                });
        };


        static PartActionDelegate WordProcessorAccent  = ( part, last) => 
        {
            return SplitterMerger(part,
                (arg) => { return arg.Split(new string[] { " " }, StringSplitOptions.None); },
                (arg) => { return String.Join(" ", arg); },
                (arg, lastLocal) =>
                {
                    var key = arg;
                    if (accentMap.ContainsKey(key))
                    {
                        return accentMap[key];
                    }
                    return arg;
                });
        };

        static MergerDelegate SentencePartMerger = (arg) =>
        {
            StringBuilder sb = new StringBuilder(arg[0]);
            for (int i = 1; i < arg.Length; i++)
            {
                if (startsWithDigit(arg[i - 1]) && endsWithDigit(arg[i]))
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
        };

        /// <summary>
        /// special cases 
        ///  number 1,000,000.00
        ///  multiple dots ...
        /// 
        /// </summary>
        static MergerDelegate SentenceMerger = (arg) =>
        {
            StringBuilder sb = new StringBuilder(arg[0]);
            for (int i = 1; i < arg.Length; i++)
            {
                if (startsWithDigit(arg[i - 1]) && endsWithDigit(arg[i]))
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
        };




        static string TextProcessorPrivate(string src, PartActionDelegate wordProcessor)
        {
            PartActionDelegate processParts = (sentence,last) =>
            {
                return SplitterMerger(sentence,
                    (arg) => { return arg.Split(new string[] { "," }, StringSplitOptions.None); },
                    (arg) => { return SentencePartMerger(arg); },
                    (arg,lastLocal) => { return wordProcessor(arg.Trim(),lastLocal); });
            };


            PartActionDelegate processSentences = (paragraph,last) =>
            {
                return SplitterMerger(paragraph,
                    (arg) => { return arg.Split(new string[] { "." }, StringSplitOptions.None); },
                    (arg) => { return SentenceMerger(arg); },
                    (arg,lastLocal) => { return processParts(arg.Trim(),lastLocal); });
            };



            return SplitterMerger(src,
                (arg) => { return arg.Split(new string[] { Environment.NewLine }, StringSplitOptions.None); },
                (arg) => { return String.Join(Environment.NewLine, arg); },
                (arg,last) => { return processSentences(arg,last); });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private static string ReplaceToAccentPrivate(string src)
        {
            return TextProcessorPrivate(src, WordProcessorAccent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string ReplaceFromEnglishPrivate(string src)
        {
            return TextProcessorPrivate(src, WordProcessorEnglish);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string ReplaceToItilized(string src)
        {
            return TextProcessorPrivate(src, WordProcessorItilic);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string ReplaceToAccent(string src)
        {
            return ReplaceToAccentPrivate(src);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string ReplaceFromEnglish(string src)
        {
            return ReplaceFromEnglishPrivate(src);
        }


    }
}
