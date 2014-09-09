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

namespace ErpContent.Tests.Controllers
{
    [TestClass]
    public class ReviewerControllerTest
    {

        //[TestMethod]
        //public void Test_ReviewerControllerIndex()
        //{
        //    // Arrange
        //    ReviewerController controller = new ReviewerController();

        //    // Act
        //    ViewResult result = controller.Index() as ViewResult;

        //    // Assert
        //    Assert.AreEqual(null, result); 
        //}

        [TestMethod]
        public void Test_Notes()
        {
            // Arrange 
            var storage = UnitTestInit.container.Resolve<IVinStorage>();
            ReviewerController controller = new ReviewerController(storage);

            // Act
            ActionResult result = controller.Notes(); 

            // Assert 
            Assert.IsInstanceOfType(result, typeof(ViewResult)); 
        }

        [TestMethod]
        public void Test_NewAddNote()
        {
            // Arrange
            var storage = UnitTestInit.container.Resolve<IVinStorage>();
            ReviewerController controller = new ReviewerController(storage); 

            // Act
            ActionResult result = controller.NewAddNote(); 

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult)); 
        }

        [TestMethod]
        [TestCategory("ReviewerController")]
        public void Test_SearchWineLabel()
        {
            // ARRANGE
            var storage = UnitTestInit.container.Resolve<IVinStorage>();
            ReviewerController controller = new ReviewerController(storage);

            string term = "Edward Sellers Vineyards";
            string producer = "Grenache Halter Ranch & Vista Creek Vineyard"; 

            // ACT
            var jsonResult = controller.SearchWineLabel(term, producer);

            // ASSERT
            Assert.IsInstanceOfType(jsonResult, typeof(JsonResult));
        }
    }
}
