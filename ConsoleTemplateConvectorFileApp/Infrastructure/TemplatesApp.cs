using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TemplateWorkApp;

namespace TemplatesWorkApp
{
    ///<summary>
    ///класc реализающий запуск/останов приложения и контролирующий его жизненый цикл
    /// </summary>
    internal class TemplatesApp
    {
        private User _user;
        private bool _authenticatedUserFlag = false;
        private DirectoryInfo _templatesDirectory;
        private Dictionary<int, string> _validFileExtensionDict = new Dictionary<int, string>()
        {
            [1] = ".doc",
            [2] = ".docx",
            [3] = ".dotx",
            [4] = ".txt",
            [5] = ".xlsx",
            [6] = ".xltx",
            [7] = ".xls",
            [8] = ".xlt",
        };
        private FileInfo _templatesFile;
        private string _functionalityChoice;


        public void Start()
        {
            var appLog = new Loger(DateTime.Now, "Выбор пользователем режима работы программы");
            if (_user != null)
            {
                return;
            }
            bool _menuValidateCommandFlag = false;
            do
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1 - Зарегистрироваться");
                Console.WriteLine("2 - Авторизоваться");
                Console.WriteLine("3 - Продолжить без регистрации/авторизации");
                Console.WriteLine("0 - Выход");
                Console.Write("Ваш выбор: ");
                string choice = Console.ReadLine();
                Console.Clear();
                switch (choice)
                {
                    case "1":
                        Registration();
                        _menuValidateCommandFlag = true;
                        break;
                    case "2":
                        Authentication();
                        _menuValidateCommandFlag = true;
                        break;
                    case "3":
                        Alert.CustomAlert("Неавторизованные пользователи могут работать с  шаблонными файлами, но не могут отправлять шаблонные файлы по электронной почте.", ConsoleColor.Yellow);
                        _menuValidateCommandFlag = true;
                        break;
                    case "0":
                        Alert.CloseAppAlert();
                        break;
                    default:
                        Alert.CustomAlert("Некоректный выбор режима работы приложения. Попробуйте снова", ConsoleColor.Red);
                        break;
                }
            } while (_menuValidateCommandFlag == false);
            appLog.FinishLog(DateTime.Now);

        }

        public void Registration()
        {
            string email;
            string password;
            bool emailValidFlag = false;
            int userId;

            var appLog = new Loger(DateTime.Now, "Регистрация пользователя");
            do
            {
                Console.WriteLine("Введите адрес электронной почты: ");
                email = Console.ReadLine();
                if (Validation.IsValidEmail(email) == false)
                {
                    Alert.CustomAlert("Некорректный ввод адресса электронной почты.Попробуйте снова", ConsoleColor.Red);
                }
                else
                {
                    emailValidFlag = true;
                }
                Console.Clear();
            } while (emailValidFlag == false);

            Console.WriteLine("Введите пароль: ");
            password = Console.ReadLine();
            Console.Clear();

            userId = SqlLiteClient.CreateUser(email, password);
            if (userId != -1)
            {
                _user = new User(userId);
                _authenticatedUserFlag = true;
                appLog.FinishLog(DateTime.Now);
                Alert.CustomAlert("Регистрация прошла успешно!", ConsoleColor.Green);
            }
            else
            {
                appLog.FinishLog(DateTime.Now);
                Alert.CustomAlert("Регистрация прошла неуспешно.Работа программы будет продолжена без возможности отправки файлов шаблонов", ConsoleColor.Red);
            }
            appLog.FinishLog(DateTime.Now);
        }
        public void Authentication()
        {
            string email;
            string password;
            int userId;

            var appLog = new Loger(DateTime.Now, "Аутенфикация пользователя");
            do
            {
                Console.Write("Введите адрес электронной почты: ");
                email = Console.ReadLine();
                Console.Write("Введите пароль: ");
                password = Console.ReadLine();
                Console.Clear();
                userId = SqlLiteClient.GetUserID(email, password);
                if (userId != -1)
                {
                    _authenticatedUserFlag = true;
                    _user = new User(userId);
                    appLog.FinishLog(DateTime.Now);
                    Alert.CustomAlert("Аутенфикация прошла успешно!", ConsoleColor.Green);
                }
                else
                {
                    appLog.FinishLog(DateTime.Now);
                    Alert.CustomAlert("Аутенфикация прошла неуспешно.Работа программы будет продолжена без возможности отправки файлов шаблонов", ConsoleColor.Red);
                }
            } while (_authenticatedUserFlag == false);
            appLog.FinishLog(DateTime.Now);
        }

        public void GetTemplateFolderPath()
        {
            var appLog = new Loger(DateTime.Now, "Получение от пользователя каталога где будут размещаться шаблоны");
            string templateFolderPath;

            while (true)
            {
                Console.Write("Введите путь к папке с шаблонами: ");
                templateFolderPath = Console.ReadLine();

                if (Directory.Exists(templateFolderPath))
                {
                    _templatesDirectory = new DirectoryInfo(templateFolderPath);
                    break;
                }
                Alert.CustomAlert("Такой папки не существует. Попробуйте снова.", ConsoleColor.Yellow);
            }
            appLog.FinishLog(DateTime.Now);
            SelectATemplateFileToWork(_templatesDirectory, _validFileExtensionDict);
        }
        public void SelectATemplateFileToWork(DirectoryInfo templatesDirectory, Dictionary<int, string> validFileExtensionDict)
        {
            var appLog = new Loger(DateTime.Now, "Выбор шаблона для работы");
            int optionNumber = 1;
            Console.WriteLine("Список файлов:");
            foreach (var file in templatesDirectory.GetFiles())
            {
                string fileNamePattern = @"^\$.+\$$";
                if (Regex.IsMatch(Path.GetFileNameWithoutExtension(file.Name), fileNamePattern))
                {
                    string fileExtension = file.Extension.ToLower();
                    if (validFileExtensionDict.ContainsValue(fileExtension))
                    {
                        Console.WriteLine(optionNumber + ". " + file.Name);
                        optionNumber++;
                    }
                }
            }
            Console.Write("Выберите номер файла: ");
            int selectedOption = Convert.ToInt32(Console.ReadLine());
            Console.Clear();

            optionNumber = 1;
            foreach (var file in templatesDirectory.GetFiles())
            {
                string fileNamePattern = @"^\$.+\$$";
                string fileExtension = file.Extension.ToLower();

                if (Regex.IsMatch(Path.GetFileNameWithoutExtension(file.Name), fileNamePattern) && validFileExtensionDict.ContainsValue(fileExtension))
                {
                    if (optionNumber == selectedOption)
                    {
                        _templatesFile = file;
                    }

                    optionNumber++;
                }
            }
            appLog.FinishLog(DateTime.Now);
        }
        public void GetFunctionalityChoice()
        {
            var appLog = new Loger(DateTime.Now, "Выбор пользователем функциональности приложения");
            bool functionalityChoiceFlag = false;

            if (_user == null)
            {
                do
                {
                    Console.WriteLine("Выберите функционал:");
                    Console.WriteLine("1 - Работа с шаблонами");
                    Console.WriteLine("0 - Выход");
                    Console.Write("Ваш выбор: ");
                    string choice = Console.ReadLine();
                    Console.Clear();
                    switch (choice)
                    {
                        case "1":
                            functionalityChoiceFlag = true;
                            _functionalityChoice = choice;
                            break;
                        case "0":
                            functionalityChoiceFlag = true;
                            Alert.CloseAppAlert();
                            break;
                        default:
                            Alert.CustomAlert("Некоректный выбор режима работы приложения. Попробуйте снова", ConsoleColor.Red);
                            break;
                    }
                } while (functionalityChoiceFlag == false);
            }
            else
            {
                do
                {
                    Console.WriteLine("Выберите функционал:");
                    Console.WriteLine("1 - Работа с шаблонами");
                    Console.WriteLine("2 - Отправка шаблона по электронной почте");
                    Console.WriteLine("0 - Выход");
                    Console.Write("Ваш выбор: ");
                    string choice = Console.ReadLine();
                    Console.Clear();
                    switch (choice)
                    {
                        case "1":
                            functionalityChoiceFlag = true;
                            _functionalityChoice = choice;
                            break;
                        case "2":
                            functionalityChoiceFlag = true;
                            _functionalityChoice = choice;
                            break;
                        case "0":
                            functionalityChoiceFlag = true;
                            Alert.CloseAppAlert();
                            break;
                        default:
                            Alert.CustomAlert("Некоректный выбор режима работы приложения. Попробуйте снова", ConsoleColor.Red);
                            break;
                    }
                } while (functionalityChoiceFlag == false);
            }
            appLog.FinishLog(DateTime.Now);
        }
        public void Work()
        {
            if (_functionalityChoice == "1")
            {
                var appLogManipulateTemplate = new Loger(DateTime.Now, "Работ с файлом шаблона");
                var templateFileManipulation = new TemplateFileManipulation();
                templateFileManipulation.StartManipulatingTemplateFileData(_templatesFile);
                appLogManipulateTemplate.FinishLog(DateTime.Now);
                return;
            }
            var appLogSendTemplate = new Loger(DateTime.Now, "Отправка файла шаблона по электронной почте");
            var emailSender = new EmailSender(_user, _templatesFile);
            emailSender.FormationOfLetterParameters();
            emailSender.SendMessage();
            appLogSendTemplate.FinishLog(DateTime.Now);
        }
        public TemplatesApp()
        {

        }

    }

}