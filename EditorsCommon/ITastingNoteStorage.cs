
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EditorsCommon
{
    /// <summary>
    /// 
    /// </summary>
    public class TastingNote 
    {
    public int id;
    public int userId;
        /// <summary>
        /// noteId is equal id if note does not have any versions. if it does, there will be multiple notes in the DB where
        /// noteId is the same, but ids are different. for one of them (origin) notdeId and id would be equal
        /// </summary>
    public int noteId;
    public int wineN;
    public int vinN;
    public int tastingN;

    public String note;
        //
        // set default value to label. some wines do not have one
        //
    public String wineName = "";
    public String producer;
    public String vintage;
    public String reviewer;

    public String country;
    public String region;
    public String location;
    public String locale;
    public String site;
    public String variety;
    public String color;
    public String dryness;
    public String rating;
    public String estimatedCost;
    public String estimatedCostHi;



    public String wineType;

    public int maturityId = 5;

    public short ratingLo;
    public short ratingHi;

    public DateTime tastingDate;
    public DateTime drinkDateLo;
    public DateTime drinkDateHi;


    public int tastingEventId;
    public int issueId;
    public bool isBarrelTasting;

    public int wfState;
    public int wfStateWineN;
    public int wfStateVinN;

    public string ratingQ; 
    public string importers;


    public string DrindDateToString()
    {
        if (drinkDateLo.Year > 1 && drinkDateHi.Year > 1)
            return drinkDateLo.Year.ToString() + "-" + drinkDateHi.Year.ToString();

        return "";
    }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="maturityName"></param>
        /// <returns></returns>
    public static int fromNameToMaturity(string maturityName)
    {
        if (!String.IsNullOrEmpty(maturityName))
        {
            switch (maturityName.ToLower().Trim())
            {
                case "young":
                    return 0;
                case "early":
                    return 1;
                case "mature":
                    return 2;
                case "late":
                    return 3;
                case "old":
                    return 4;
            }
        }

        return 5;
    }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="maturityValue"></param>
        /// <returns></returns>
    public static string fromMaturityToName(int maturityValue)
    {

        switch(maturityValue){
            case 0: 
                return "Young";
            case 1: 
                return "Early";
            case 2: 
                return "Mature";
            case 3: 
                return "Late";
            case 4: 
                return "Old";
        }
        return "";
    }


   
        /// <summary>
        /// Expected that "rating" string is assignment (usually on clint side.
        /// It's value could be "", "99", "99-99"
        /// </summary>
    public void decodeRating()
    {

        if (!String.IsNullOrEmpty(rating))
        {
            Match m = Regex.Match(rating, @"(\d{2})-(100)");
            if (m.Success)
            {
                ratingLo = short.Parse(m.Groups[1].Value);
                ratingHi = short.Parse(m.Groups[2].Value);
                return;
            }

            m = Regex.Match(rating, @"(\d{2})-(\d{2})");

            if (m.Success)
            {
                ratingLo = short.Parse(m.Groups[1].Value);
                ratingHi = short.Parse(m.Groups[2].Value);
                return;
            }

            m = Regex.Match(rating, @"100");

            if (m.Success)
            {
                ratingLo =  short.Parse(rating);
                ratingHi = 0;
                return;
            }


            m = Regex.Match(rating, @"\d{2}");

            if (m.Success)
            {
                ratingLo = short.Parse(rating);
                ratingHi = 0;
                return;
            }

        }



        ratingLo = ratingHi = 0;
    }

        /// <summary>
        /// combine rating filed based on ratingLo and ratingHi
        /// </summary>
    public void encodeRating()
    {

        if (ratingLo == ratingHi)
        {
            if (ratingLo == 0){
                rating = "";
                return;
            }

            rating = ratingLo.ToString();
            return; 
        }

        if( ratingHi < ratingLo )
            rating = ratingLo.ToString();
        else
            rating = ratingLo.ToString() + "-" + ratingHi.ToString();
    }


    public string encodeRatingForPrinting()
    {
        string result = "";
        string q = String.IsNullOrEmpty(ratingQ) ? "" : ratingQ;

        if (ratingLo == ratingHi)
        {
            if (ratingLo == 0)
            {
                result = "";
            }
            else
            {

                result = ratingLo.ToString() + q;
            }
        }
        else if (ratingHi < ratingLo)
        {
            result = ratingLo.ToString() + q;
        }
        else
        {
            result = "(" + ratingLo.ToString() + "-" + ratingHi.ToString() + q + ")";
        }

        return result;
    }


    public String noteFormated
    {
        get
        {
            return String.IsNullOrEmpty(note) ? "" : 
                TastingNoteHelpers.ReplaceToItilized(
                TastingNoteHelpers.ReplaceToAccent( 
                TastingNoteHelpers.ReplaceFromEnglish(note)));
        }
    }

    }




    public interface ITastingNoteStorage : IStorage<TastingNote>
    {
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tastingEventId"></param>
        /// <returns></returns>
        IEnumerable<TastingNote> SearchTastingNoteByTastingEvent(int tastingEventId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TastingNote SearchTastingNoteById(int id);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="stateId"></param>
        int SetNoteState(int noteId, int stateId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wineN"></param>
        /// <returns></returns>
        IEnumerable<TastingNote> SearchTastingNoteByWineN(int wineN);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vinN"></param>
        /// <returns></returns>
        IEnumerable<TastingNote> SearchTastingNoteByVinN(int vinN);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="producerN"></param>
        /// <returns></returns>
        IEnumerable<TastingNote> SearchTastingNoteByProducerN(int producerN);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="noteId"></param>
        /// <returns></returns>
        bool MoveTastingNote(int eventId, int noteId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetInQueueCount();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<TastingNote> GetInQueue();

        /// <summary>
        /// 
        /// </summary>
        void PublishFromQueue();
    }
}
