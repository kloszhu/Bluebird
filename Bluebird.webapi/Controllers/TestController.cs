using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.IO;
using Bluebird.FileManager;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Bluebird.webapi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class TestController : Controller
    {
        private readonly ApplicationPartManager _partManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        public TestController(
            ApplicationPartManager partManager,
            IHostingEnvironment env)
        {
            _partManager = partManager;
            _hostingEnvironment = env;
        }
        [HttpGet]
        public bool RegisterControllerAtRuntime()
        {
            string Temp = System.IO.File.ReadAllText("Template/File.txt");
            GeneratEntity generat = new GeneratEntity();
            var assembly= generat.CreateDll("mycontroller", Temp);
            
            if (assembly != null) {
                _partManager.ApplicationParts.Add(new AssemblyPart(assembly));
                // Notify change
                MyActionDescriptorChangeProvider.Instance.HasChanged = true;
                MyActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
                return true;
            }
            return false;
        }
    }
}
