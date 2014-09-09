using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EditorsDbLayer;
using EditorsCommon;
using System.Collections.Generic;

namespace ErpContent.Tests.Models
{
    [TestClass]
    public class TastingNoteTest
    {

        [TestMethod]
        public void TastingNote_SearchTastingNoteById()
        {

            // Arrange
            SqlConnectionFactory connection = new SqlConnectionFactory(); 
            TastingNoteStorage model = new TastingNoteStorage(connection);
                        
            var id0 = 0;
            var id1 = 12;


            // Act 

            /**
             * TEST CASE
             * 
             * Test for fetching note from model, giving an ID
             */ 
            TastingNote noteWithID = model.SearchTastingNoteById(id1);

            // Assert
            Assert.IsNotNull(noteWithID);
            Assert.IsInstanceOfType(noteWithID, typeof(TastingNote)); 
            
            /**
             * TEST CASE
             * 
             * Test for fetching note from model, when the supplied paramter
             * is 0 
             */
            TastingNote noteWithoutID = model.SearchTastingNoteById(id0);
            Assert.IsNull(noteWithoutID); 

        }

        [TestMethod]
        public void TastingNote_SearchTastingNoteByVinN()
        {

            // Arrange
            SqlConnectionFactory connection = new SqlConnectionFactory();
            TastingNoteStorage model = new TastingNoteStorage(connection);

            var id0 = 0;
            var id1 = 12;

            /**
             * TEST CASE
             * 
             * Test for fetching notes from model, giving an ID. 
             * 
             */
            // Act
            IEnumerable<TastingNote> notes = model.SearchTastingNoteByVinN(id1);
            
            // Assert
            Assert.IsNotNull(notes);

            /**
             * Test if each element in the collection is of type
             * "TastingNote"
             */
            foreach (dynamic note in notes)
            {
                Assert.IsInstanceOfType(note, typeof(TastingNote));
                Assert.IsNotNull(note);
            }
        }

        [TestMethod]
        public void TastingNote_Create()
        {
                        
            SqlConnectionFactory connection = new SqlConnectionFactory();
            TastingNoteStorage model = new TastingNoteStorage(connection);

            /**
             * TEST CASE
             * 
             * Adding a tasting note that has values on its members. 
             * 
             * TODO: implement all the members
             */ 

            // Arrange
            TastingNote testNote = new TastingNote();
            testNote.id = 100;
            testNote.userId = 1063571;
            testNote.noteId = 1234;
            testNote.wineN = 234324;
            testNote.vinN = 23424;
            testNote.tastingN = 32432414; 

            testNote.note = "This is the first test Note";

            testNote.wineName = "Sample wine for testing";
            testNote.producer = "Sample producer";
            testNote.vintage = "2008";
            testNote.reviewer = "Joshua Fuentes";
            testNote.country = "Philippines";
            testNote.region = "Misamis Oriental";
            testNote.location = "Location Value";
            testNote.locale = "Locale Value";
            testNote.site = "Site Value";
            testNote.variety = "variety value";
            testNote.color = "red";
            testNote.dryness = "dry";
            testNote.rating = "99";
            testNote.estimatedCost = "100";
            testNote.estimatedCostHi = "101";

            testNote.wineType = "red wine";
            testNote.maturityId = 10;
            testNote.ratingLo = 90;
            testNote.ratingHi = 98;

            testNote.tastingDate = new DateTime(2014, 01, 01);
            testNote.drinkDateHi = new DateTime(2010, 01, 01);
            testNote.drinkDateLo = new DateTime(2015, 01, 01);

            testNote.wfState = 1;
            testNote.wfStateWineN = 2;
            testNote.wfStateVinN = 3; 

            // Act
            var result = model.Create(testNote); 
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result, testNote); 
        }

        [TestMethod]
        public void TastingNote_Search()
        {
            SqlConnectionFactory connection = new SqlConnectionFactory();
            TastingNoteStorage model = new TastingNoteStorage(connection);

            string searchString = "bordeaux";

            //var result = model.Search(searchString); 
            
        }

        [TestMethod]
        public void TastingNote_Update()
        {

            // Arrange
            SqlConnectionFactory connection = new SqlConnectionFactory();
            TastingNoteStorage model = new TastingNoteStorage(connection);
            
            int tastingNoteID = 262514;
            TastingNote testNote = model.SearchTastingNoteById(tastingNoteID);
            testNote.note = "this is an updated note";

            // Act 
            var result = model.Update(testNote); 

            // Asserts
            Assert.IsNotNull(result); 

        }

        [TestMethod]
        public void TastingNote_Delete()
        {

            // Arrange
            SqlConnectionFactory connection = new SqlConnectionFactory();
            TastingNoteStorage model = new TastingNoteStorage(connection);

            int tastingNoteID = 262514;

            TastingNote testNote = model.SearchTastingNoteById(tastingNoteID);

            // Act
            var deletedNote = model.Delete(testNote);

            // Assert
            Assert.IsNotNull(deletedNote); 

        }
    }
}
