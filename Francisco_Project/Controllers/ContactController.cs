using Francisco_Project.Models.DAO;
using Francisco_Project.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Francisco_Project.Controllers
{
    public class ContactController : Controller
    {
        private readonly ContactDao _contactDao = new ContactDao();
        private readonly string clientId = "franZa";
        private readonly string tokenPass = "test01";

        public async Task<ActionResult> Index()
        {
            string token = await _contactDao.AuthenticateAsync(clientId, tokenPass);
            if (token == null)
            {
                return new HttpStatusCodeResult(500, "Authentication failed");
            }

            List<ContactDto> contacts = await _contactDao.GetAllContactsAsync(token);
            return View(contacts);
        }

        public async Task<ActionResult> DetailsByEmail(string email)
        {
            string token = await _contactDao.AuthenticateAsync(clientId, tokenPass);
            if (token == null)
            {
                return new HttpStatusCodeResult(500, "Authentication failed");
            }

            ContactDto contact = await _contactDao.GetContactByEmailAsync(token, email);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(ContactDto newContact)
        {
            if (ModelState.IsValid)
            {
                string token = await _contactDao.AuthenticateAsync(clientId, tokenPass);
                if (token == null)
                {
                    return new HttpStatusCodeResult(500, "Authentication failed");
                }

                ContactDto createdContact = await _contactDao.CreateContactAsync(token, newContact);
                if (createdContact != null)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(newContact);
        }
    }
}
