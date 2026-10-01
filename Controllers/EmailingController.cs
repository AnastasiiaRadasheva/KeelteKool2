using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using KeelteKool2.Models.Emailing;
using Microsoft.AspNetCore.Mvc;

namespace KeelteKool2.Controllers
{
    public class EmailingController : Controller
    {
        private readonly IEmailingServices _emailingServices;
        public EmailingController(IEmailingServices emailingServices)
        {
            _emailingServices = emailingServices;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SendEmail(EmailViewModel vm)
        {
            var isFiles = Request.Form.Files.Any();
            List<IFormFile> files = null;
            if (isFiles != null)
            {
                files = Request.Form.Files.ToList();
            }
            else
            {
                files = new List<IFormFile>();
            }

            var emailDTO = new EmailDTO
            {
                To = vm.To,
                Subject = vm.Subject,
                Body = vm.Body,
                Attachment = (List<IFormFile>)vm.Attachment,
            };
            _emailingServices.SendEmail(emailDTO);
            return RedirectToAction(nameof(Index));
        }
    }
}
