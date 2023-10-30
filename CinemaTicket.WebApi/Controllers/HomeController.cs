using CinemaTicket.TestObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private static List<TestObject> testObjects = new List<TestObject>();
        [HttpGet]
        [Route("info")]
        public string GetInfo()
        {
            return $"Backend is started {DateTime.Now}";
        }
        [HttpPost]
        [Route("Hello")]
        public string AddName(string name)
        {
            return $"Hello, {name}!";
        }
        [HttpGet]
        [Route("GetTestObject")]
        public TestObject GetTestObject()
        {
            var testObject = new TestObject
            {
                Name = "Misato",
                Description = "Commanger Of Evangelion Squad",
                CreateDate = new DateTime(2023, 10, 30, 20, 46, 00)
            };
            return testObject;
        }
        [HttpGet]
        [Route("GetTestObjects")]
        public List<TestObject> GetTestObjects()
        {
            return testObjects;
        }
        [HttpPost]
        [Route("AddTestObject")]
        public bool PostTestObject(TestObject testObject)
        {
            if (testObject == null)
            {
                return false;
            }
            testObjects.Add(testObject);
            return true;
        }
    }
}
