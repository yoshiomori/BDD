using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BDD.Controllers
{
    [Produces("application/json")]
    [Route("api/Person")]
    public class PersonController : Controller
    {
        // GET: api/Person
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(new List<Person>() { null, new Person { NameUIHSASFDUIH = "Ok", Age = new DateTime(1990, 1, 1) } });
        }

        // GET: api/Person/5
        [HttpGet("{id}", Name = "Get")]
        public ActionResult Get(int id)
        {
            return Ok(new Person { NameUIHSASFDUIH = "Ok", Age = new DateTime(1990, 1, 1) }) ;
        }
        
        // POST: api/Person
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/Person/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
