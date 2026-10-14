using KeeltekoolV2.ApplicationServices.Servises;
using KeeltekoolV2.Core.ServiceInterface;
using KeeltekoolV2.Data;
using Microsoft.AspNetCore.Mvc;
using KeeltekoolV2.Data;
using KeeltekoolV2.Core.ServiceInterface;

namespace KeelteKool2.Controllers
{
    public class LecturersController : Controller
    {
        private readonly KeeltekoolV2Context _context;

        private readonly ILecturersServices _lecturersServices;

             private readonly IFileServices _filesServices;
        public LecturersController(
    KeeltekoolV2Context context,
    ILecturersServices lecturersServices,
    IFileServices filesServices)
        {
            _context = context;
            _lecturersServices = lecturersServices;
            _filesServices = filesServices;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
