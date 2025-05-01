using Microsoft.AspNetCore.Mvc;
using ConfigurationLibrary;

namespace ConfigurationWebApp.Controllers
{
    public class ConfigViewerController : Controller
    {
        private readonly ConfigurationReader _reader;

        public ConfigViewerController(ConfigurationReader reader)
        {
            _reader = reader;
        }

        public IActionResult Index()
        {
            var configs = _reader.GetAll(); 
            return View(configs);
        }
    }
}
