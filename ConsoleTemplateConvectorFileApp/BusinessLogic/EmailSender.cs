using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace TemplateWorkApp
{
    internal class EmailSender
    {
        private int _userID;
        private string _smtpServer = "smtp.mail.ru";
        private int _smtpPort = 587;
        private string _smtpUsername;
        private string _smtpPassword;//пароль приложения( здесь должен быть введен пароль не от самого почтового ящика, а пароль для внешних приложений использующих почтовый ящик) 
        private string _to;
        private string _subject;
        private string _body;
        private FileInfo _attachmentDoc;
        public int UserID
        {
            get { return _userID; }
        }
        public void FormationOfLetterParameters()
        {
            string to;
            string subject;
            string body;
            bool emailValidFlag = false;

            Tuple<string, string> tupleMainInfo = SqlLiteClient.GetUserMailInfo(UserID);
            _smtpUsername=tupleMainInfo.Item1;
            _smtpPassword=tupleMainInfo.Item2;

            do
            {
                Console.Write("Введите адрес электронной почты получателя: ");
                to = Console.ReadLine();
                if (Validation.IsValidEmail(to) == false)
                {
                    Alert.CustomAlert("Некорректный ввод адресса электронной почты.Попробуйте снова", ConsoleColor.Red);
                }
                else
                {
                    emailValidFlag = true;
                    _to=to;
                }

                Console.Write("Введите тему письма: ");
                subject = Console.ReadLine();

                Console.Write("Введите текст письма: ");
                body = Console.ReadLine();
                if (Validation.IsValidString(body) == true) 
                {
                    Alert.CustomAlert("Письмо не должно содержать пустую графу Сообщение", ConsoleColor.Red);
                }
                else
                {
                    emailValidFlag = false;
                    _body = body;
                }
                Console.Clear();
            } while (emailValidFlag == false && Validation.IsValidString(body)==true);


        }
        public void SendMessage()
        {
            using (SmtpClient smtpClient = new SmtpClient(_smtpServer, _smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                smtpClient.EnableSsl = true;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(_smtpUsername);
                    mailMessage.To.Add(_to);
                    mailMessage.Subject = _subject;
                    mailMessage.Body = _body;
                    mailMessage.Attachments.Add(new Attachment(_attachmentDoc.FullName));

                    try
                    {
                        smtpClient.Send(mailMessage);
                        Console.WriteLine("Сообщение успешно отправлено.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка отправки сообщения: {ex.Message}");
                    }
                }
            }
        }
        public EmailSender(User user, FileInfo templatesFile)
        {
            _userID = user.Id;
            _attachmentDoc = templatesFile;
        }
    }
}
