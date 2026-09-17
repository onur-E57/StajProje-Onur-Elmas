using Microsoft.AspNetCore.Mvc;

namespace StajProje.WebUI.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult SendChatWithAI()
        {
            return View();
        }

        public PartialViewResult ChatbotPartial()
        {
            return PartialView();
        }
    }
}