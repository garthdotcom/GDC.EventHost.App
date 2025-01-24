using GDC.EventHost.App;
using GDC.EventHost.App.Areas.Admin.ViewModels;
using GDC.EventHost.App.Models;
using GDC.EventHost.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GDC.EventHost.App.Areas.Admin.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IEmailSender _sender;
        private readonly IWebHostEnvironment _environment;

        public HomeController(IConfiguration config, IEmailSender emailSender,
            IWebHostEnvironment environment)
        {
            _config = config;
            _sender = emailSender;
            _environment = environment;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> SendTestEmail()
        {
            
            var toEmailAddress = "ghc.dev.uk@outlook.com";  // string.Empty;

            var subject = "Email system verification for Event-Host";

            var builder = new StringBuilder();

            var filePath = Path.Combine(_environment.WebRootPath, "content", "emailTemplate.html");

            var templateFile = new FileInfo(filePath);

            using (var reader = templateFile.OpenText())
            {
                builder.Append(reader.ReadToEnd());
            }

            builder.Replace("{{messageText}}", 
                "Email system verification for Event-Host. Thanks for confirming.");

            var message = builder.ToString();

            await _sender.SendEmailAsync(toEmailAddress, subject, message);

            return View(
                    new MessageVM
                    {
                        TheMessage = "The email was sent. Please check your inbox."
                    });


            //var emailSettings = _config.GetSection("Email").Get<AuthMessageSenderOptions>();

            //var sendGridKey = emailSettings.SendGridKey;
            //var defaultSender = emailSettings.DefaultSenderEmail;
            //var defaultReceiver = emailSettings.DefaultReceiverEmail;

            //var client = new SendGridClient(sendGridKey);
            //var from = new EmailAddress(defaultSender, "GDC Dev");
            //var subject = "Email system verification for Event-Host";
            //var to = new EmailAddress(defaultReceiver, "GDC Customer");
            //var plainTextContent = "This is easy to do anywhere, even with C#";
            //var htmlContent = "<strong>This is easy to do anywhere, even with C#</strong>";
            //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            //var response = await client.SendEmailAsync(msg);

            //var responseMessage = response.Body.ReadAsStringAsync().Result;

            //var messageVM = new MessageVM();

            //if (string.IsNullOrEmpty(responseMessage))
            //{
            //    return View(
            //        new MessageVM
            //        {
            //            TheMessage = "The email was sent successfully. Please check your inbox."
            //        });
            //}

            //return View(
            //    new MessageVM
            //    {
            //        TheMessage = responseMessage
            //    });
        }
    }
}