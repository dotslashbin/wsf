using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ErpContent;
using ErpContent.Controllers;
using EditorsCommon;
using Microsoft.Practices.Unity;
using System.Web.Routing;
using EditorsCommon.Publication;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.Web.Helpers;
using Newtonsoft.Json;
using System.Collections;

namespace ErpContent.Tests.Controllers
{
    [TestClass]
    public class VinControllerTest
    {


        VinController controller = new VinController(UnitTestInit.container.Resolve<IVinStorage>());

        [TestMethod]
        public void Index()
        {
            // Arrange
            var storage = UnitTestInit.container.Resolve<IVinStorage>();

            VinController controller = new VinController(storage);



        }

        //----------------------------------------------------------------------------//
        //Added by: Romel
        //Date Added: June 2, 2014
        //Details: to test each VinController methods if users are to be authenticated and checked to be in certain roles.    


        //Test SearchWineLabel Authorization Method Exists
        [TestMethod]
        [TestCategory("VinController")]
        public void Test_If_SearchWineLabel_Authorization()
        {
            var methodName = "SearchWineLabel";
            var paramsType = new Type[] { typeof(string), typeof(string) };
            It_Should_Require_Authorization(controller, methodName, paramsType);
        }

        //Test SearchWineLabel Authorization Method using specific roles
        [TestMethod]
        [TestCategory("VinController")]
        public void Test_Role_SearchWineLabel_Authorization()
        {
            var methodName = "SearchWineLabel";
            var roles = new[] { EditorsCommon.Constants.roleNameReviewer, EditorsCommon.Constants.roleNameAdmin };
            var paramsType = new Type[] { typeof(string), typeof(string) };
            It_Should_Require_Authorization(controller, methodName, paramsType, roles);
        }



        //Test SearchProducer Authorization Method Exists
        [TestMethod]
        [TestCategory("VinController")]
        public void Test_If_SearchProducer_Authorization()
        {
            var methodName = "SearchProducer";
            var paramsType = new Type[] { typeof(string) };
            It_Should_Require_Authorization(controller, methodName, paramsType);
        }

        //Test SearchProducer Authorization Method using specific roles
        [TestMethod]
        [TestCategory("VinController")]
        public void Test_Role_SearchProducer_Authorization()
        {
            var methodName = "SearchProducer";
            var roles = new[] { EditorsCommon.Constants.roleNameReviewer, EditorsCommon.Constants.roleNameAdmin };
            var paramsType = new Type[] { typeof(string) };
            It_Should_Require_Authorization(controller, methodName, paramsType, roles);
        }




        //----------------------------------------------------------------------------//
        //Added by: Romel
        //Date Added: June 10, 2014

        //Tasks
        //v. Test a method and feed it with correct parameters and returns a result.
        //vii. Test a method return values to have the correct format (example: JSON)
        //Not sure how to test
        //vi. Test a method and feed it with incorrect parameters and it should return false


        // Test SearchWineLabel
        [TestMethod]
        public void Test_SearchWineLabel_Feed_Correct_Parameters()
        {
            // Arrange
            string term = "Lindauer";
            string p = "Lindauer";
            var storage = UnitTestInit.container.Resolve<IVinStorage>();
            VinController controller = new VinController(storage);

            // Act
            var result = controller.SearchWineLabel(term, p);

            // Assert
            Assert.IsNotNull(result, "Result is not returning any values");
            Assert.IsInstanceOfType(result, typeof(JsonResult), "Result is not returning correct type");
            
        }
       

        // Test SearchProducer
        [TestMethod]
        public void Test_SearchProducer_Feed_Correct_Parameters()
        {
            // Arrange
            string term = "Lindauer";
            var storage = UnitTestInit.container.Resolve<IVinStorage>();
            VinController controller = new VinController(storage);

            // Act
            var result = controller.SearchProducer(term);
            dynamic producerInformation = (JsonResult)result;

            

            // Assert
            /*----  Test if return is correct type per collection*/

            foreach (dynamic info in producerInformation.Data){
                Assert.IsInstanceOfType(info, typeof(VinN), "Result is not returning as VinN type");
                if (info.wines != null){
                    foreach (dynamic wineInfo in info.wines)
                    {
                        Assert.IsInstanceOfType(wineInfo, typeof(WineN), "Result is not returning as WineN type");
                    }
                }
            }

            Assert.IsNotNull(result, "Result is not returning any values");
            Assert.IsInstanceOfType(result, typeof(JsonResult), "Result is not returning correct type");
        }


        // Test SearchProducerExtended
        [TestMethod]
        public void Test_SearchProducerExtended_Feed_Correct_Parameters()
        {
            // Arrange
            string term = "Lindauer";
            var storage = UnitTestInit.container.Resolve<IVinStorage>();
            VinController controller = new VinController(storage);

            // Act
            var result = controller.SearchProducerExtended(term);

            // Assert
            Assert.IsNotNull(result, "Result is not returning any values");
            Assert.IsInstanceOfType(result, typeof(JsonResult), "Result is not returning correct type");
        }



        // Test SearchLocation
        [TestMethod]
        public void Test_SearchLocation_Feed_Correct_Parameters()
        {
            // Arrange
            string country = "Germany";
            string region = "Nahe";
            string location = "Niederhausen";
            string locale = "Hermannshohle";
            string site = "";

            var storage = UnitTestInit.container.Resolve<IVinStorage>();
            VinController controller = new VinController(storage);

            // Act
            var result = controller.SearchLocation(country, region, location, locale, site);

            // Assert
            Assert.IsNotNull(result, "Result is not returning any values");
            Assert.IsInstanceOfType(result, typeof(JsonResult), "Result is not returning correct type");
        }




        // Test SearchWineN
        [TestMethod]
        public void Test_SearchWineN_Feed_Correct_Parameters()
        {
            // Arrange
            string term = "Lindauer";
            int state = EditorsCommon.WorkFlowState.STATE_GROUP_ALL;
            var storage = UnitTestInit.container.Resolve<IVinStorage>();
            VinController controller = new VinController(storage);

            // Act
            var result = controller.SearchWineN(term, state);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult, "Result is suppose to be JsonResult");


            IEnumerable<VinN> data = jsonResult.Data as IEnumerable<VinN>;
            Assert.IsNotNull(data, "Expected that data is IEnumerable<WineN>");

            Assert.IsNotNull(data.Count() > 0, "Expected that data has some results");



            // Assert
            Assert.IsNotNull(result, "Result is not returning any values");
            Assert.IsInstanceOfType(result, typeof(JsonResult), "Result is not returning correct type");
        }




        // Test ApproveVinN
        [TestMethod]
        public void Test_ApproveVinN_Feed_Correct_Parameters()
        {
            // Arrange
            int id = 2;
            var storage = UnitTestInit.container.Resolve<IVinStorage>();
            VinController controller = new VinController(storage);

            // Act
            
            var result = controller.ApproveVinN(id);

            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult, "Result is suppose to be JsonResult");


            dynamic count = jsonResult.Data;
            Assert.IsTrue(count > 0, "Number of WineN update should be more than 0");
            

            // Assert
            Assert.IsNotNull(result, "Result is not returning any values");
            Assert.IsInstanceOfType(result, typeof(JsonResult), "Result is not returning correct type");
        }
        

        




        //----------------------------------------------------------------------------//

        //Reusable methods

        /// <summary> It should require authorization for Controller or ApiController.</summary>
        /// <param name="controller"> The controller.</param>
        /// <returns>The Authorize Attribute from the controller .</returns>
        protected AuthorizeAttribute It_Should_Require_Authorization(object controller, string methodName, Type[] paramsType)
        {
            //var paramsType = new Type[] { typeof(string), typeof(string) };

            var type = controller.GetType();
            var methodInfo = type.GetMethod(methodName, paramsType);
            var attributes = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), true);
            Assert.IsTrue(attributes.Any(), "No AuthorizeAttribute found");
            return attributes.Any() ? attributes[0] as AuthorizeAttribute : null;
        }

        /// <summary> It should require authorization for Controller or ApiController.</summary>
        /// <param name="controller"> The controller.</param>
        /// <param name="roles">      The roles.</param>
        protected void It_Should_Require_Authorization(object controller, string methodName, Type[] paramsType, string[] roles)
        {
            var authorizeAttribute = this.It_Should_Require_Authorization(controller, methodName, paramsType);
            if (!roles.Any())
            {
                return;
            }

            if (authorizeAttribute == null)
            {
                return;
            }

            bool all = authorizeAttribute.Roles.Split(',').All(r => roles.Contains(r.Trim()));
            Assert.IsTrue(all);
        }

    }
}
