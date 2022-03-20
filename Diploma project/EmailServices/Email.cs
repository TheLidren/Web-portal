using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Diploma_project.EmailServices
{
    public class Email
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [StringLength(50, ErrorMessage = "Введите корректно размерность", MinimumLength = 2)]
        [RegularExpression(@"^([А-ЯЁ]{1}[а-яё]{1,50})$", ErrorMessage = "Текст содержит только латинские буквы с соблюдением регистра")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [StringLength(50, ErrorMessage = "Введите корректно размерность", MinimumLength = 2)]
        [RegularExpression(@"^([А-ЯЁ]{1}[а-яё]{1,50})$", ErrorMessage = "Текст содержит только латинские буквы с соблюдением регистра")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+\@(mail.ru|gmail.com)$", ErrorMessage = "Почта введена некорректно")]
        public string EmailFrom { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [RegularExpression(@"^([а-яА-ЯёЁa-zA-Z0-9\s\,-_.""\:]{1,50})$", ErrorMessage = "Введите корректно тему сообщения")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [RegularExpression(@"^(\+375|80)(29|25|44|33)(\d{7})$", ErrorMessage = "Некорректно введён номер телефона")]
        public string Phone { get; set; }

        public string Message { get; set; }

        //Send message to directors email address
        public void SendEmailAsync(Email email, HttpPostedFileBase[] uploadDocx)
        {
            MailMessage mail = new();
            mail.From = new MailAddress(email.EmailFrom, $"{email.Surname} {email.Name}");
            mail.To.Add("the_lidrenvlamis@mail.ru");
            mail.Subject = email.Subject;
            mail.Body = $"<p>{email.Message}</p><i>Номер телефона отправителя: {email.Phone}</i>";
            mail.IsBodyHtml = true;
            if (uploadDocx != null)
                foreach (var file in uploadDocx)
                {
                    string path = Path.GetTempPath() + file.FileName;
                    file.SaveAs(path);
                    mail.Attachments.Add(new Attachment(path));
                }
            using SmtpClient client = new("smtp.gmail.com");
            client.Credentials = new NetworkCredential("vladmisevic92@gmail.com", "EfT41DjGK5xNT57Zn1AVM");
            client.Port = 587;
            client.EnableSsl = true;
            client.Send(mail);
        }

        //Send message to email address
        public static void ComfirmEmailMessage(string emailTo, string body, string subject)
        {
            MailMessage message = new();
            message.IsBodyHtml = true;
            message.From = new("vladmisevic92@gmail.com", subject);
            message.To.Add(emailTo);
            message.Subject = subject;
            message.Body = body;
            using SmtpClient client = new("smtp.gmail.com");
            client.Credentials = new NetworkCredential("vladmisevic92@gmail.com", "EfT41DjGK5xNT57Zn1AVM");
            client.Port = 587;
            client.EnableSsl = true;
            client.Send(message);
        }

    }
}
