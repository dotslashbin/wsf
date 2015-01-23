using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;
using ErpContent;
using ErpContent.Controllers;
using System.Web.Mvc;
using EditorsCommon;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

using EditorsCommon.Publication;
using Newtonsoft.Json.Linq;

namespace ErpContent.Tests.Controllers
{
    [TestClass]
    public class ChiefEditorControllerTest
    {

        ChiefEditorController controller = new ChiefEditorController(UnitTestInit.container.Resolve<IIssueStorage>());

        [TestMethod]
        public void CheifEditorController_Index()
        {
            // Arrange 

            // Act
            ActionResult result = this.controller.Index();

            // Assert 

            /**
             * TEST CASE
             * 
             * This is to test if the return type is a valid view 
             */ 
            Assert.IsInstanceOfType(result, typeof(ViewResult)); 
        }

        [TestMethod]
        public void CheifEditorController_TastingEvents()
        {
            // Arrange 
            int assignmentID        = 10;

            // Act
            var result_withID                   = this.controller.TastingEvents(assignmentID);
            var result_withoutID                = this.controller.TastingEvents(0);
            dynamic resultsWithIDSupplied       = (JsonResult)result_withID;
            dynamic resultsWihtoutIDSupplied    = (JsonResult)result_withoutID; 
            
            // Assert

            /**
             * TEST CASE 
             * 
             * This is to check if there is a result returned
             */
            Assert.IsNotNull(result_withID, "There is a returned result when there is ID supplied");
            Assert.IsNotNull(result_withoutID, "There is a returned result when there is no ID supplied");

            /**
             * TEST CASE 
             * 
             * This is to test if the collection returns null, if there are 
             * no ID supplied 
             */ 
            Assert.IsNull(resultsWihtoutIDSupplied.Data, "There are no data returned when there is no ID supplied"); 
            
            /**
             * TEST CASE 
             * 
             * This is to test the data returned, if they are of type "TastingEvent". This will
             * only be done if the there are data to be tested
             */
            if( (resultsWithIDSupplied.Data != null) && resultsWithIDSupplied.Data.count() > 0) 
            {
                foreach(dynamic objectToTest in resultsWithIDSupplied) 
                {
                    Assert.IsInstanceOfType(objectToTest, typeof(TastingEvent), "The object returend is of type TastingEvent");
                }
            }

        }

        [TestMethod]
        public void CheifEditorController_publications()
        {
            
            // Arrange
            var result = this.controller.Publications();

            // Assert

            /**
             * TEST CASE
             * 
             * This is to test if there is a result returned
             */ 
            Assert.IsNotNull(result, "There is a valid returend result"); // Test to see if the result is not null 

            /**
             * TEST CASE 
             * This is to test if the objects returned are of type "PublicationItem"
             */
            dynamic jsonCollection = (JsonResult)result; 
            foreach(dynamic objectToTest in jsonCollection.Data) {
                Assert.IsInstanceOfType(objectToTest, typeof(PublicationItem), "The object returned is of type PublicationItem"); 
            }
        }
    }

}
