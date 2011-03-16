
using System.Net;

namespace Xpand.EmailTemplateEngine {
    using System;
    using System.IO;
    using System.Net.Mail;

    internal class Program {
        public static void Main() {
            IEmailTemplateContentReader templateReader = new FileSystemEmailTemplateContentReader();
            IEmailTemplateEngine templateEngine = new EmailTemplateEngine(templateReader);

            IEmailSender sender = new EmailSender {
                CreateClientFactory = () => new SmtpClientWrapper(CreateSmtpClientWhichDropsInLocalFileSystem())
            };

            EmailSubsystem subsystem = new EmailSubsystem("apostolis.bekiaris@gmail.com", templateEngine, sender);

            subsystem.SendWelcomeMail("Jon Smith", "~!Agc2d#7", "apostolis.bekiaris@gmail.com");

            Console.WriteLine("Mail delivered, check the outbox folder.");
            Console.Read();
        }

        private static SmtpClient CreateSmtpClientWhichDropsInLocalFileSystem() {
            var outbox = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "outbox");

            if (!Directory.Exists(outbox)) {
                Directory.CreateDirectory(outbox);
            }

            var smtpClientWhichDropsInLocalFileSystem = new SmtpClient {
                                                                           DeliveryMethod = SmtpDeliveryMethod.Network,
                                                                           PickupDirectoryLocation = outbox,
                                                                           Host = "smtp.gmail.com",
                                                                           EnableSsl = true,
                                                                           Port = 587,
                                                                           UseDefaultCredentials = true
                                                                       };
            smtpClientWhichDropsInLocalFileSystem.Credentials = new NetworkCredential("apostolis.bekiaris@gmail.com",
                                                                                      "4451@p0m");
            return smtpClientWhichDropsInLocalFileSystem;
        }
    }
}