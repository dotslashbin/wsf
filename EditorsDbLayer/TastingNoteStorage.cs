using EditorsCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorsDbLayer
{
    public class TastingNoteStorage : ITastingNoteStorage
    {

        private ISqlConnectionFactory _connFactory;



        public TastingNoteStorage(ISqlConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public TastingNote SearchTastingNoteById(int id)
        {
            List<TastingNote> result = new List<TastingNote>();


            using (var con = _connFactory.GetConnection())
            {
                var query = new StringBuilder();

                var nullDate = new DateTime(0);

                using (var cmd = new SqlCommand("", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "TastingNote_GetById";
                    cmd.Parameters.AddWithValue("@ID", id);

                    using (var rdr = cmd.ExecuteReader())
                    {

                        if (rdr.Read())
                        {

                            return ReadTastingFromDb(rdr);

                        }

                    }
                }
            }

            return null;
        }



        public IEnumerable<TastingNote> SearchTastingNoteByVinN(int vinN)
        {
            List<TastingNote> result = new List<TastingNote>();



            using (var con = _connFactory.GetConnection())
            {
                var query = new StringBuilder();

                var nullDate = new DateTime(0);

                using (var cmd = new SqlCommand("", con))
                {
                    cmd.CommandText = @"
	select 
		ID = tn.ID,
		OriginID = tn.OriginID,
		UserId = tn.UserId,
		UserrName = u.FullName,
		
		Wine_N_ID = tn.Wine_N_ID,
		Wine_ProducerID = w.ProducerID,
		Wine_Producer = w.ProducerToShow,
		Wine_Country  = w.Country,
		Wine_Region   = w.Region,
		Wine_Location = w.Location,
		Wine_Locale   = w.Locale,
		Wine_Site     = w.Site,
		Wine_Label    = w.Label,
		Wine_Vintage  = w.Vintage,
		Wine_Name     = w.Name,
		
		Wine_Type     = w.Type,
		Wine_Variety  = w.Variety,
		Wine_Drynes   = w.Dryness,
		Wine_Color    = w.Color,
		

		TasteDate     = tn.TasteDate, 
		MaturityID    = tn.MaturityID, 
		MaturityName  = wm.Name,
		MaturitySuggestion = wm.Suggestion,
		Rating_Lo = tn.Rating_Lo, 
		Rating_Hi = tn.Rating_Hi, 
		DrinkDate_Lo = tn.DrinkDate_Lo, 
		DrinkDate_Hi = tn.DrinkDate_Hi, 
		IsBarrelTasting = tn.IsBarrelTasting, 
		Notes = tn.Notes, 

		WF_StatusID = tn.WF_StatusID,
		WF_StatusName = '',
		created = tn.created, 
		updated = tn.updated, 
        Wine_N_WF_StatusID = w.Wine_N_WF_StatusID,
		Vin_N_WF_StatusID = w.Vin_N_WF_StatusID,
		EstimatedCost,
		EstimatedCost_Hi 

        ,RatingQ
        ,Importers =  STUFF(  (select '+'+'---new-line---'+ Name 
                     +  case
                          when LEN( isnull(Address,'')) > 0 then (',' + Address )
                          else ''
                        end   
                     +  case
                          when LEN( isnull(Phone1,'')) > 0 then (',' + Phone1 )
                          else ''
                        end   
                     +  case
                          when LEN( isnull(URL,'')) > 0 then (',' + URL)
                          else ''
                        end   
                    from WineImporter wi
                    join WineProducer_WineImporter wpi  (nolock) on wpi.ImporterId  = wi.ID
                    where 
                    wpi.ProducerId = w.ProducerID
                    FOR XML PATH('')), 1, 1, '' )		
				
	from TasteNote tn (nolock)
		join Users u (nolock) on tn.UserId = u.UserId
		join vWineDetails w on tn.Wine_N_ID = w.Wine_N_ID
		join WineMaturity wm (nolock) on tn.MaturityID = wm.ID
		join TastingEvent_TasteNote ttn  (nolock) on ttn.TasteNoteID = tn.ID
	where w.Wine_VinN_ID = @VinN
    and tn.WF_StatusID = 100
	order by TasteDate desc, UserName, tn.ID

";
                    cmd.Parameters.AddWithValue("@VinN ", vinN);

                    using (var rdr = cmd.ExecuteReader())
                    {

                        while (rdr.Read())
                        {
                            TastingNote note = ReadTastingFromDb(rdr);

                            result.Add(note);
                        }

                    }
                    return result;
                }
            }
        }




        public IEnumerable<TastingNote> SearchTastingNoteByWineN(int wineN)
        {
            List<TastingNote> result = new List<TastingNote>();



            using (var con = _connFactory.GetConnection())
            {
                var query = new StringBuilder();

                var nullDate = new DateTime(0);

                using (var cmd = new SqlCommand("", con))
                {
                    cmd.CommandText = @"
	select 
		ID = tn.ID,
		OriginID = tn.OriginID,
		UserId = tn.UserId,
		UserrName = u.FullName,
		
		Wine_N_ID = tn.Wine_N_ID,
		Wine_ProducerID = w.ProducerID,
		Wine_Producer = w.ProducerToShow,
		Wine_Country  = w.Country,
		Wine_Region   = w.Region,
		Wine_Location = w.Location,
		Wine_Locale   = w.Locale,
		Wine_Site     = w.Site,
		Wine_Label    = w.Label,
		Wine_Vintage  = w.Vintage,
		Wine_Name     = w.Name,
		
		Wine_Type     = w.Type,
		Wine_Variety  = w.Variety,
		Wine_Drynes   = w.Dryness,
		Wine_Color    = w.Color,
		

		TasteDate     = tn.TasteDate, 
		MaturityID    = tn.MaturityID, 
		MaturityName  = wm.Name,
		MaturitySuggestion = wm.Suggestion,
		Rating_Lo = tn.Rating_Lo, 
		Rating_Hi = tn.Rating_Hi, 
		DrinkDate_Lo = tn.DrinkDate_Lo, 
		DrinkDate_Hi = tn.DrinkDate_Hi, 
		IsBarrelTasting = tn.IsBarrelTasting, 
		Notes = tn.Notes, 

		WF_StatusID = tn.WF_StatusID,
		WF_StatusName = '',
		created = tn.created, 
		updated = tn.updated, 
        Wine_N_WF_StatusID = w.Wine_N_WF_StatusID,
		Vin_N_WF_StatusID = w.Vin_N_WF_StatusID,
		EstimatedCost,
		EstimatedCost_Hi 

        ,RatingQ
        ,Importers =  STUFF(  (select '+'+'---new-line---'+ Name 
                     +  case
                          when LEN( isnull(Address,'')) > 0 then (',' + Address )
                          else ''
                        end   
                     +  case
                          when LEN( isnull(Phone1,'')) > 0 then (',' + Phone1 )
                          else ''
                        end   
                     +  case
                          when LEN( isnull(URL,'')) > 0 then (',' + URL)
                          else ''
                        end   
                    from WineImporter wi
                    join WineProducer_WineImporter wpi  (nolock) on wpi.ImporterId  = wi.ID
                    where 
                    wpi.ProducerId = w.ProducerID
                    FOR XML PATH('')), 1, 1, '' )		
				
	from TasteNote tn (nolock)
		join Users u (nolock) on tn.UserId = u.UserId
		join vWineDetails w on tn.Wine_N_ID = w.Wine_N_ID
		join WineMaturity wm (nolock) on tn.MaturityID = wm.ID
		join TastingEvent_TasteNote ttn  (nolock) on ttn.TasteNoteID = tn.ID
	where w.Wine_N_ID = @WineN
    and tn.WF_StatusID = 100
	order by TasteDate desc, UserName, tn.ID

";
                    cmd.Parameters.AddWithValue("@WineN ", wineN);

                    using (var rdr = cmd.ExecuteReader())
                    {

                        while (rdr.Read())
                        {
                            TastingNote note = ReadTastingFromDb(rdr);

                            result.Add(note);
                        }

                    }
                    return result;
                }
            }
        }



        public IEnumerable<TastingNote> SearchTastingNoteByProducerN(int producerN)
        {
            List<TastingNote> result = new List<TastingNote>();



            using (var con = _connFactory.GetConnection())
            {
                var query = new StringBuilder();

                var nullDate = new DateTime(0);

                using (var cmd = new SqlCommand("", con))
                {
                    cmd.CommandText = @"
	select 
		ID = tn.ID,
		OriginID = tn.OriginID,
		UserId = tn.UserId,
		UserrName = u.FullName,
		
		Wine_N_ID = tn.Wine_N_ID,
		Wine_ProducerID = w.ProducerID,
		Wine_Producer = w.ProducerToShow,
		Wine_Country  = w.Country,
		Wine_Region   = w.Region,
		Wine_Location = w.Location,
		Wine_Locale   = w.Locale,
		Wine_Site     = w.Site,
		Wine_Label    = w.Label,
		Wine_Vintage  = w.Vintage,
		Wine_Name     = w.Name,
		
		Wine_Type     = w.Type,
		Wine_Variety  = w.Variety,
		Wine_Drynes   = w.Dryness,
		Wine_Color    = w.Color,
		

		TasteDate     = tn.TasteDate, 
		MaturityID    = tn.MaturityID, 
		MaturityName  = wm.Name,
		MaturitySuggestion = wm.Suggestion,
		Rating_Lo = tn.Rating_Lo, 
		Rating_Hi = tn.Rating_Hi, 
		DrinkDate_Lo = tn.DrinkDate_Lo, 
		DrinkDate_Hi = tn.DrinkDate_Hi, 
		IsBarrelTasting = tn.IsBarrelTasting, 
		Notes = tn.Notes, 

		WF_StatusID = tn.WF_StatusID,
		WF_StatusName = '',
		created = tn.created, 
		updated = tn.updated, 
        Wine_N_WF_StatusID = w.Wine_N_WF_StatusID,
		Vin_N_WF_StatusID = w.Vin_N_WF_StatusID,
		EstimatedCost,
		EstimatedCost_Hi 

        ,RatingQ
        ,Importers =  STUFF(  (select '+'+'---new-line---'+ Name 
                     +  case
                          when LEN( isnull(Address,'')) > 0 then (',' + Address )
                          else ''
                        end   
                     +  case
                          when LEN( isnull(Phone1,'')) > 0 then (',' + Phone1 )
                          else ''
                        end   
                     +  case
                          when LEN( isnull(URL,'')) > 0 then (',' + URL)
                          else ''
                        end   
                    from WineImporter wi
                    join WineProducer_WineImporter wpi  (nolock) on wpi.ImporterId  = wi.ID
                    where 
                    wpi.ProducerId = w.ProducerID
                    FOR XML PATH('')), 1, 1, '' )		
				
	from TasteNote tn (nolock)
		join Users u (nolock) on tn.UserId = u.UserId
		join vWineDetails w on tn.Wine_N_ID = w.Wine_N_ID
		join WineMaturity wm (nolock) on tn.MaturityID = wm.ID
		join TastingEvent_TasteNote ttn  (nolock) on ttn.TasteNoteID = tn.ID

	where w.ProducerID = @ProducerN
	order by Wine_Vintage desc,  Wine_Label asc,  tn.ID

";
                    cmd.Parameters.AddWithValue("@ProducerN", producerN);

                    using (var rdr = cmd.ExecuteReader())
                    {

                        while (rdr.Read())
                        {
                            TastingNote note = ReadTastingFromDb(rdr);

                            result.Add(note);
                        }

                    }
                    return result;
                }
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public IEnumerable<TastingNote> SearchTastingNoteByTastingEvent(int eventId)
        {
            List<TastingNote> result = new List<TastingNote>();

            //Dictionary<int, TastingNote> dict = new Dictionary<int, TastingNote>();

            using (var con = _connFactory.GetConnection())
            {
                var query = new StringBuilder();

                var nullDate = new DateTime(0);

                using (var cmd = new SqlCommand("", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "TastingNote_GetListByTastingEvent";
                    cmd.Parameters.AddWithValue("@TastingEventID ", eventId);

                    using (var rdr = cmd.ExecuteReader())
                    {

                        while (rdr.Read())
                        {
                            TastingNote note = ReadTastingFromDb(rdr);


                            
                            note.tastingEventId = eventId;

                            result.Add(note);
                        }

                    }

                    //return dict.Values.ToList<TastingNote>();
                    return result;
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public TastingNote Create(TastingNote e)
        {

            using (var con = _connFactory.GetConnection())
            {
                using (var transation = con.BeginTransaction())
                {
                    var query = new StringBuilder();

                    using (var cmd = new SqlCommand("", con))
                    {
                        cmd.Transaction = transation;
                        cmd.CommandText =

                        @" 
                        
                            declare @Wine_N_ID int;
                            declare @NoteId int;
                            declare @IssueId int;
       
                            set nocount on;
                 

                            select @IssueId = a.IssueID from Assignment a
                              inner join Assignment_TastingEvent e on a.ID = e.AssignmentID
                              where e.TastingEventID =  @TastingEventID;
                 
                            
                            select result = @IssueId; 


                            exec @Wine_N_ID = Wine_GetID @Producer=@Producer, @WineType=@WineType, @Label=@Label, @Variety=@Variety,
                                         @Dryness=@Dryness, @Color=@Color,@Vintage=@Vintage,
                                         @locCountry= @locCountry, @locRegion=@locRegion,@locLocation=@locLocation, @locLocale=@locLocale, @locSite=@locSite;


                            exec @NoteId = TastingNote_Add @UserId = @UserId,@Wine_N_ID=@Wine_N_ID,@TasteDate=@TasteDate,@MaturityID=@MaturityID,
                                 @Rating_Lo=@Rating_Lo, @Rating_Hi= @Rating_Hi,@DrinkDate_Lo=@DrinkDate_Lo,@DrinkDate_Hi=@DrinkDate_Hi,
                                 @EstimatedCost=@EstimatedCost, @EstimatedCost_Hi= @EstimatedCost_Hi,
                                 @IsBarrelTasting=@IsBarrelTasting,@RatingQ= @RatingQ, @Notes=@Notes, @IssueID = @IssueId;


                            exec TastingEvent_TasteNote_Add @TastingEventID=@TastingEventID, @TasteNote=@NoteId;


                            exec TastingNote_GetByID @ID =@NoteId;

                            ";



                        cmd.Parameters.AddWithValue("@Producer", String.IsNullOrEmpty(e.producer) ? null : e.producer);
                        cmd.Parameters.AddWithValue("@WineType", String.IsNullOrEmpty(e.wineType) ? "Table" : e.wineType);
                        cmd.Parameters.AddWithValue("@Label", String.IsNullOrEmpty(e.wineName) ? "" : e.wineName);
                        cmd.Parameters.AddWithValue("@Variety", String.IsNullOrEmpty(e.variety) ? "" : e.variety);
                        cmd.Parameters.AddWithValue("@Dryness", String.IsNullOrEmpty(e.dryness) ? "" : e.dryness);
                        cmd.Parameters.AddWithValue("@Color", String.IsNullOrEmpty(e.color) ? null : e.color);
                        cmd.Parameters.AddWithValue("@Vintage", String.IsNullOrEmpty(e.vintage) ? null : e.vintage);
                        cmd.Parameters.AddWithValue("@locCountry", String.IsNullOrEmpty(e.country) ? "" : e.country);
                        cmd.Parameters.AddWithValue("@locRegion", String.IsNullOrEmpty(e.region) ? "" : e.region);
                        cmd.Parameters.AddWithValue("@locLocation", String.IsNullOrEmpty(e.location) ? "" : e.location);
                        cmd.Parameters.AddWithValue("@locLocale", String.IsNullOrEmpty(e.locale) ? "" : e.locale);
                        cmd.Parameters.AddWithValue("@locSite", String.IsNullOrEmpty(e.site) ? "" : e.site);

                        cmd.Parameters.AddWithValue("@UserId", e.userId <= 0 ? 0 : e.userId);
                        cmd.Parameters.AddWithValue("@TasteDate", e.tastingDate.Ticks == 0 ? DateTime.Today : e.tastingDate);
                        cmd.Parameters.AddWithValue("@MaturityID", e.maturityId );

                        e.decodeRating();
                        cmd.Parameters.AddWithValue("@Rating_Lo", e.ratingLo);
                        cmd.Parameters.AddWithValue("@Rating_Hi", e.ratingHi);



                        if (String.IsNullOrEmpty(e.ratingQ) || e.isBarrelTasting == false)
                        {
                            cmd.Parameters.AddWithValue("@RatingQ", DBNull.Value);

                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@RatingQ", e.ratingQ);
                        }



                        if (e.drinkDateLo.Ticks > 0)
                        {
                            cmd.Parameters.AddWithValue("@DrinkDate_Lo", e.drinkDateLo);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@DrinkDate_Lo", DBNull.Value);
                        }


                        if (e.drinkDateHi.Ticks > 0)
                        {
                            cmd.Parameters.AddWithValue("@DrinkDate_Hi", e.drinkDateHi);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@DrinkDate_Hi", DBNull.Value);
                        }

                        cmd.Parameters.AddWithValue("@EstimatedCost", String.IsNullOrEmpty(e.estimatedCost) ? 0 : Decimal.Parse( e.estimatedCost));
                        cmd.Parameters.AddWithValue("@EstimatedCost_Hi", String.IsNullOrEmpty(e.estimatedCostHi) ? 0 : Decimal.Parse( e.estimatedCostHi));


                        cmd.Parameters.AddWithValue("@IsBarrelTasting", e.isBarrelTasting);

                        cmd.Parameters.AddWithValue("@TastingEventID", e.tastingEventId);
                        cmd.Parameters.AddWithValue("@Notes", String.IsNullOrEmpty(e.note) ? "" : e.note);


                        try
                        {

                            TastingNote note = null;

                            using (var rdr = cmd.ExecuteReader())
                            {
                                int id = 0, wineN = 0, issueId = 0;

                                //
                                // order is important.
                                //
                                //
                                if (rdr.Read())
                                {
                                    issueId = rdr.GetInt32(0);
                                }
                                if (rdr.NextResult() && rdr.Read())
                                {
                                    wineN = rdr.GetInt32(0);
                                }
                                if (rdr.NextResult() && rdr.Read())
                                {
                                    id = rdr.GetInt32(0);
                                }

                                e.id = id;
                                e.wineN = wineN;
                                e.issueId = issueId;

                                if (rdr.NextResult() && rdr.NextResult() && rdr.Read())
                                {
                                    note = ReadTastingFromDb(rdr);
                                    note.tastingEventId = e.tastingEventId;

                                    if (note.id != e.id)
                                    {
                                        throw new Exception("error in logic. read note with wrong id");
                                    }
                                }
                                else
                                {
                                    throw new Exception("error in logic. could not create tasting note");
                                }
                            }

                            transation.Commit();
                            return note;

                        }
                        catch (Exception ex)
                        {
                            transation.Rollback();
                            throw new Exception("exception while executing transaction", ex);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public IEnumerable<TastingNote> Search(TastingNote e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public TastingNote Update(TastingNote e)
        {

            using (var con = _connFactory.GetConnection())
            {
                using (var transation = con.BeginTransaction())
                {
                    var query = new StringBuilder();

                    using (var cmd = new SqlCommand("", con))
                    {
                        cmd.Transaction = transation;
                        cmd.CommandText =
                        @" 
                        
    declare @Wine_N_ID int;
    declare @NoteId int;
    declare @IssueId int;
    declare @UpdatedId int;
       
    set nocount on;
                 

    select @IssueId = a.IssueID from Assignment a
        inner join Assignment_TastingEvent e on a.ID = e.AssignmentID
        where e.TastingEventID =  @TastingEventID;
                 
    select result = @IssueId; 

    exec @Wine_N_ID = Wine_GetID @Producer=@Producer, @WineType=@WineType, @Label=@Label, @Variety=@Variety,
                    @Dryness=@Dryness, @Color=@Color,@Vintage=@Vintage,
                    @locCountry= @locCountry, @locRegion=@locRegion,@locLocation=@locLocation, @locLocale=@locLocale, @locSite=@locSite;


    exec @UpdatedId = TastingNote_Update @ID=@ID, @UserId = @UserId,@Wine_N_ID=@Wine_N_ID,@TasteDate=@TasteDate,@MaturityID=@MaturityID,
            @Rating_Lo=@Rating_Lo, @Rating_Hi= @Rating_Hi,@DrinkDate_Lo=@DrinkDate_Lo,@DrinkDate_Hi=@DrinkDate_Hi,
            @EstimatedCost=@EstimatedCost, @EstimatedCost_Hi= @EstimatedCost_Hi,
            @IsBarrelTasting=@IsBarrelTasting,@RatingQ=@RatingQ, @Notes=@Notes, @IssueID = @IssueId;



	if @UpdatedId <> @ID begin
        exec TastingEvent_TasteNote_Add @TastingEventID=@TastingEventID, @TasteNote=@UpdatedId;
    end

    exec TastingNote_GetByID @ID =@UpdatedId;

                            ";



                        cmd.Parameters.AddWithValue("@Producer", String.IsNullOrEmpty(e.producer) ? null : e.producer);
                        cmd.Parameters.AddWithValue("@WineType", String.IsNullOrEmpty(e.wineType) ? "Table" : e.wineType);
                        cmd.Parameters.AddWithValue("@Label", String.IsNullOrEmpty(e.wineName) ? "" : e.wineName);
                        cmd.Parameters.AddWithValue("@Variety", String.IsNullOrEmpty(e.variety) ? "" : e.variety);
                        cmd.Parameters.AddWithValue("@Dryness", String.IsNullOrEmpty(e.dryness) ? "" : e.dryness);
                        cmd.Parameters.AddWithValue("@Color", String.IsNullOrEmpty(e.color) ? null : e.color);
                        cmd.Parameters.AddWithValue("@Vintage", String.IsNullOrEmpty(e.vintage) ? null : e.vintage);
                        cmd.Parameters.AddWithValue("@locCountry", String.IsNullOrEmpty(e.country) ? "" : e.country);
                        cmd.Parameters.AddWithValue("@locRegion", String.IsNullOrEmpty(e.region) ? "" : e.region);
                        cmd.Parameters.AddWithValue("@locLocation", String.IsNullOrEmpty(e.location) ? "" : e.location);
                        cmd.Parameters.AddWithValue("@locLocale", String.IsNullOrEmpty(e.locale) ? "" : e.locale);
                        cmd.Parameters.AddWithValue("@locSite", String.IsNullOrEmpty(e.site) ? "" : e.site);

                        cmd.Parameters.AddWithValue("@ID", e.id);
                        cmd.Parameters.AddWithValue("@UserId", e.userId <= 0 ? 0 : e.userId);
                        cmd.Parameters.AddWithValue("@TasteDate", e.tastingDate.Ticks == 0 ? DateTime.Today : e.tastingDate);
                        cmd.Parameters.AddWithValue("@MaturityID",  e.maturityId);

                        e.decodeRating();
                        cmd.Parameters.AddWithValue("@Rating_Lo", e.ratingLo);
                        cmd.Parameters.AddWithValue("@Rating_Hi", e.ratingHi);

                        if (String.IsNullOrEmpty(e.ratingQ) || e.isBarrelTasting == false)
                        {
                            cmd.Parameters.AddWithValue("@RatingQ",  DBNull.Value );

                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@RatingQ",  e.ratingQ);
                        }


                        if (e.drinkDateLo.Ticks > 0)
                        {
                            cmd.Parameters.AddWithValue("@DrinkDate_Lo", e.drinkDateLo);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@DrinkDate_Lo", DBNull.Value);
                        }


                        if (e.drinkDateHi.Ticks > 0)
                        {
                            cmd.Parameters.AddWithValue("@DrinkDate_Hi", e.drinkDateHi);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@DrinkDate_Hi", DBNull.Value);
                        }

                        cmd.Parameters.AddWithValue("@EstimatedCost", String.IsNullOrEmpty(e.estimatedCost) ? 0 : Decimal.Parse(e.estimatedCost));
                        cmd.Parameters.AddWithValue("@EstimatedCost_Hi", String.IsNullOrEmpty(e.estimatedCostHi) ? 0 : Decimal.Parse(e.estimatedCostHi));

                        cmd.Parameters.AddWithValue("@IsBarrelTasting", e.isBarrelTasting);

                        cmd.Parameters.AddWithValue("@TastingEventID", e.tastingEventId);
                        cmd.Parameters.AddWithValue("@Notes", e.note);


                        try
                        {

                            TastingNote note = null;

                            using (var rdr = cmd.ExecuteReader())
                            {
                                int wineN = 0, issueId = 0;

                                if (rdr.Read())
                                {
                                    issueId = rdr.GetInt32(0);
                                }


                                if (rdr.NextResult() && rdr.Read())
                                {
                                    wineN = rdr.GetInt32(0);
                                }


                                if (rdr.NextResult() && rdr.Read())
                                {
                                    int resultId = rdr.GetInt32(0);
                                    if (resultId != e.id)
                                    {
                                        e.id = resultId;
                                        rdr.NextResult();
                                    }
                                }


                                e.wineN = wineN; // this value could be changed
                                //e.issueId = issueId;

                                if (rdr.NextResult() && rdr.Read())
                                {
                                    note = ReadTastingFromDb(rdr);

                                    note.tastingEventId = e.tastingEventId;
                                    if (note.id != e.id)
                                    {
                                        throw new Exception("error in logic. read note with wrong id");
                                    }
                                }
                                else
                                {
                                    throw new Exception("error in logic. could not update tasting note");
                                }
                            }

                            transation.Commit();
                            return note;
                        }
                        catch (Exception)
                        {
                            transation.Rollback();
                            throw;
                        }

                    }
                }
            }
        }

        private static TastingNote ReadTastingFromDb(SqlDataReader rdr)
        {
            var nullDate = new DateTime(0);
            TastingNote note = new TastingNote();

            note.id = rdr.GetInt32(0);
            note.noteId = rdr.IsDBNull(1) ? note.id : rdr.GetInt32(1);
            note.reviewer = rdr.IsDBNull(3) ? "" : rdr.GetString(3);
            note.wineN = rdr.GetInt32(4);

            note.producer = rdr.IsDBNull(6) ? "" : rdr.GetString(6);
            note.country = rdr.IsDBNull(7) ? "" : rdr.GetString(7);
            note.region = rdr.IsDBNull(8) ? "" : rdr.GetString(8);
            note.location = rdr.IsDBNull(9) ? "" : rdr.GetString(9);
            note.locale = rdr.IsDBNull(10) ? "" : rdr.GetString(10);
            note.site = rdr.IsDBNull(11) ? "" : rdr.GetString(11);

            note.wineName = rdr.IsDBNull(12) ? "" : rdr.GetString(12);
            note.vintage = rdr.IsDBNull(13) ? "" : rdr.GetString(13);

            note.wineType = rdr.IsDBNull(15) ? "" : rdr.GetString(15);
            note.variety = rdr.IsDBNull(16) ? "" : rdr.GetString(16);
            note.dryness = rdr.IsDBNull(17) ? "" : rdr.GetString(17);
            note.color = rdr.IsDBNull(18) ? "" : rdr.GetString(18);
            note.tastingDate = rdr.IsDBNull(19) ? nullDate : rdr.GetDateTime(19);
            note.maturityId = rdr.IsDBNull(20) ? 5 : rdr.GetFieldValue<Int16>(20);


            note.ratingLo = rdr.IsDBNull(23) ? (short)0 : rdr.GetFieldValue<Int16>(23);
            note.ratingHi = rdr.IsDBNull(24) ? (short)0 : rdr.GetFieldValue<Int16>(24);
            note.encodeRating();

            if (!rdr.IsDBNull(25))
                note.drinkDateLo = rdr.GetDateTime(25);
            if (!rdr.IsDBNull(26))
                note.drinkDateHi = rdr.GetDateTime(26);

            note.isBarrelTasting = rdr.IsDBNull(27) ? false : rdr.GetBoolean(27);

            note.note = rdr.IsDBNull(28) ? "" : rdr.GetString(28);

            note.wfState = rdr.GetFieldValue<Int16>(29);
            note.wfStateWineN = rdr.GetFieldValue<Int16>(33);
            note.wfStateVinN = rdr.GetFieldValue<Int16>(34);

            note.estimatedCost = rdr.IsDBNull(35) ? "" : rdr.GetDecimal(35).ToString("0.##");
            note.estimatedCostHi = rdr.IsDBNull(36) ? "" : rdr.GetDecimal(36).ToString("0.##");
            note.estimatedCost = note.estimatedCost.CompareTo("0") == 0 ? "" : note.estimatedCost;
            note.estimatedCostHi = note.estimatedCostHi.CompareTo("0") == 0 ? "" : note.estimatedCostHi;

            note.ratingQ = rdr.IsDBNull(37) ? "" : rdr.GetString(37);
            note.importers = rdr.IsDBNull(38) ? "" : rdr.GetString(38);

            note.importers = note.importers.Replace("---new-line---", "\r\n");


            return note;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public TastingNote Delete(TastingNote e)
        {
            using (var con = _connFactory.GetConnection())
            {
                return DeleteInternal(e, con);
            }
        }

        private static TastingNote DeleteInternal(TastingNote e, SqlConnection con)
        {
            var query = new StringBuilder();

            using (var cmd = new SqlCommand("", con))
            {
                cmd.CommandText =
                @" 
                declare @Result int;
       
                set nocount on;
                 
                exec @Result = TastingNote_Del @ID = @ID

                select @Result;

                ";


                cmd.Parameters.AddWithValue("@ID", e.id);

                using (var rdr = cmd.ExecuteReader())
                {

                    if (rdr.Read())
                    {
                        if (rdr.GetInt32(0) > 0)
                            return e;
                    }
                }

                return null;

            }
        }


        public int SetNoteState(int noteId, int stateId)
        {

            switch (stateId)
            {
                case WorkFlowState.ARCHIVED:
                    break;
                case WorkFlowState.DRAFT:
                    break;
                case WorkFlowState.PUBLISHED:
                    break;
                case WorkFlowState.READY_APPROVED:
                case WorkFlowState.READY_FOR_REVIEW:
                case WorkFlowState.READY_FOR_PROOF_READ:
                    break;
                default:
                    throw new Exception("Attempt set unknown  state -> " + stateId);
            }

            List<TastingNote> result = new List<TastingNote>();

            using (var con = _connFactory.GetConnection())
            {

                using (var cmd = new SqlCommand("", con))
                {
                    cmd.CommandText = "update TasteNote set WF_StatusID = @wfStateId where id = @noteId";
                    cmd.Parameters.AddWithValue("@noteId", noteId);
                    cmd.Parameters.AddWithValue("@wfStateId", stateId);
                    cmd.ExecuteNonQuery();
                    return stateId;
                }
            }

        }


        public bool MoveTastingNote(int eventId, int noteId)
        {
            using (var con = _connFactory.GetConnection())
            {
                var query = new StringBuilder();

                using (var cmd = new SqlCommand("", con))
                {
                    cmd.CommandText =
                    @" 
        
                         delete TastingEvent_TasteNote where TasteNoteID = @tastingNoteId;

                         exec TastingEvent_TasteNote_Add @TastingEventID=@tastingEventId, @TasteNote=@tastingNoteId;

                     ";
                    cmd.Parameters.AddWithValue("@tastingNoteId", noteId);
                    cmd.Parameters.AddWithValue("@tastingEventId", eventId);

                    cmd.ExecuteNonQuery();

                    return true;

                }
            }
        }



        public int GetInQueueCount()
        {
            using (var con = _connFactory.GetConnection())
            {
                var query = new StringBuilder();

                using (var cmd = new SqlCommand("", con))
                {
                    cmd.CommandText =
                    @" select COUNT(*) from Wine inner join TasteNote on wine.TasteNote_ID = TasteNote.ID
                       where  wine.RV_TasteNote <> TasteNote.RV";

                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            return rdr.GetInt32(0);
                        }

                    }

                    return 0;

                }
            }
        }

        public IEnumerable<TastingNote> GetInQueue()
        {

            List<TastingNote> result = new List<TastingNote>();


            using (var con = _connFactory.GetConnection())
            {
                var query = new StringBuilder();

                using (var cmd = new SqlCommand("", con))
                {
                    cmd.CommandText =
                    @" 

select  top 200
		ID = tn.ID,
		OriginID = tn.OriginID,
		UserId = tn.UserId,
		UserrName = u.FullName,
		
		Wine_N_ID = tn.Wine_N_ID,
		Wine_ProducerID = w.ProducerID,
		Wine_Producer = w.ProducerToShow,
		Wine_Country  = w.Country,
		Wine_Region   = w.Region,
		Wine_Location = w.Location,
		Wine_Locale   = w.Locale,
		Wine_Site     = w.Site,
		Wine_Label    = w.Label,
		Wine_Vintage  = w.Vintage,
		Wine_Name     = w.Name,
		
		Wine_Type     = w.Type,
		Wine_Variety  = w.Variety,
		Wine_Drynes   = w.Dryness,
		Wine_Color    = w.Color,
		

		TasteDate     = tn.TasteDate, 
		MaturityID    = tn.MaturityID, 
		MaturityName  = wm.Name,
		MaturitySuggestion = wm.Suggestion,
		Rating_Lo = tn.Rating_Lo, 
		Rating_Hi = tn.Rating_Hi, 
		DrinkDate_Lo = tn.DrinkDate_Lo, 
		DrinkDate_Hi = tn.DrinkDate_Hi, 
		IsBarrelTasting = tn.IsBarrelTasting, 
		Notes = tn.Notes, 

		WF_StatusID = tn.WF_StatusID,
		WF_StatusName = '',
		created = tn.created, 
		updated = tn.updated, 
        Wine_N_WF_StatusID = w.Wine_N_WF_StatusID,
		Vin_N_WF_StatusID = w.Vin_N_WF_StatusID,
		EstimatedCost = tn.EstimatedCost,
		EstimatedCost_Hi 

        ,RatingQ
        ,Importers =  STUFF(  (select '+'+'---new-line---'+ Name 
                     +  case
                          when LEN( isnull(Address,'')) > 0 then (',' + Address )
                          else ''
                        end   
                     +  case
                          when LEN( isnull(Phone1,'')) > 0 then (',' + Phone1 )
                          else ''
                        end   
                     +  case
                          when LEN( isnull(URL,'')) > 0 then (',' + URL)
                          else ''
                        end   
                    from WineImporter wi
                    join WineProducer_WineImporter wpi  (nolock) on wpi.ImporterId  = wi.ID
                    where 
                    wpi.ProducerId = w.ProducerID
                    FOR XML PATH('')), 1, 1, '' )		
				
		
				
	from TasteNote tn (nolock)
		join Users u (nolock) on tn.UserId = u.UserId
		join vWineDetails w on tn.Wine_N_ID = w.Wine_N_ID
		join WineMaturity wm (nolock) on tn.MaturityID = wm.ID
		join TastingEvent_TasteNote ttn  (nolock) on ttn.TasteNoteID = tn.ID
	where  tn.WF_StatusID = 100
    and tn.ID in (select TasteNote.ID from Wine inner join TasteNote on wine.TasteNote_ID = TasteNote.ID
                  where  wine.RV_TasteNote <> TasteNote.RV)
	order by TasteDate desc, UserName, tn.ID
";

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            TastingNote note = ReadTastingFromDb(rdr);
                            result.Add(note);
                        }

                    }

                    return result;
                }
            }
        }


        public void PublishFromQueue()
        {
            using (var con = _connFactory.GetConnection())
            {
                var query = new StringBuilder();

                using (var cmd = new SqlCommand("", con))
                {
                    cmd.CommandText = @"exec [srv].[Wine_Reload]  @IsFullReload = 0";
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
