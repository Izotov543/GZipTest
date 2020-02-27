using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;

namespace GZipTest.Utils
{
    /// <summary>
    /// Содержит методы обработки исключительных ситуаций
    /// </summary>
    internal static class ExceptionHandlers
    {
        /// <summary>
        /// Обрабатывает исключения <see cref="System.IO.IOException"></see>
        /// </summary>
        internal static class IOExceptionHandler
        {
            internal static void Run(string exceptionMessage, string[] args, string senderType)
            {
                if (exceptionMessage == "Недостаточно места на диске.\r\n")
                {
                    GetNewFilePathOrRestartApp(exceptionMessage, args, senderType, false);
                }

                else
                {
                    CloseApplication(exceptionMessage);
                }
            }
        }

        /// <summary>
        /// Обрабатывает исключения <see cref="System.UnauthorizedAccessException"></see>, 
        /// выброшенные при создании экземпляра объекта типов <see cref="GZipTest.IO.Reader"></see>
        /// или <see cref="GZipTest.IO.Writer"></see> по причине отсутствия у учетной записи пользователя,
        /// от имени которого запущено приложение, прав на чтение/запись по указанному пути.
        /// Либо при попытке записи в скрытый файл.
        /// </summary>
        internal static class UnauthorizedAccessExceptionHandler
        {
            internal static void Run(string exceptionMessage, string[] args, string senderType)
            {
                GetNewFilePathOrRestartApp(exceptionMessage, args, senderType, true);
            }
        }

        internal static class ThreadExceptionHandler
        {
            internal static void Run(Exception e)
            {
                CloseApplication(e.Message);
            }
        }

        /// <summary>
        /// Вызывает завершение работы, либо перезапуск приложения с новыми аргументами командной строки
        /// (<paramref name="args"></paramref>)
        /// </summary>
        private static void GetNewFilePathOrRestartApp(
            string exceptionMessage,
            string[] args,
            string senderType,
            bool adminOption)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(exceptionMessage +
                "\nВведите полное имя файла, если его необходимо изменить." +
                "\nДля отмены операции и завершения работы наберите {cancel} или нажмите {ctrl + C}.");
            if (adminOption)
            {
                Console.WriteLine("Для запуска приложения с правами администратора наберите {Administrator}");
            }

            Console.ResetColor();
            string usersAnswer = Console.ReadLine();
            if (usersAnswer.ToLower() == "cancel")
            {
                Environment.Exit(1);
            }

            else if (adminOption && (usersAnswer.ToLower() == "administrator" || usersAnswer.ToLower() == "admin"))
            {
                RestartAppWithArgs(args, true);
            }

            if (senderType == "Reader")
            {
                args[1] = usersAnswer;
                RestartAppWithArgs(args, false);
            }

            else if (senderType == "Writer")
            {
                args[2] = usersAnswer;
                RestartAppWithArgs(args, false);
            }
        }

        /// <summary>
        /// Завершает работу приложения
        /// </summary>
        private static void CloseApplication(string exceptionMessage)
        {
            for (int i = 5; i > 0; i--)
            {
                Console.WriteLine(exceptionMessage);
                Console.WriteLine($"\nРабота приложения будет завершена {i}");
                Thread.Sleep(1000);
                Console.Clear();
            }

            Environment.Exit(1);
        }

        /// <summary>
        /// Перезапускает приложения с новыми аргументами командной строки (<paramref name="Arguments"></paramref>), 
        /// в т.ч. от имени администратора
        /// </summary>
        private static void RestartAppWithArgs(string[] args, bool adminOption)
        {
            StringBuilder argsBuilder = new StringBuilder();
            foreach (string arg in args)
            {
                argsBuilder.Append("\"" + arg + "\" ");
            };

            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = Assembly.GetExecutingAssembly().Location,
                Arguments = argsBuilder.ToString()
            };

            if (adminOption)
            {
                processInfo.Verb = "runas";
            }

            try
            {
                Process.Start(processInfo);
                Environment.Exit(1);
            }

            catch (Win32Exception)
            {
                Environment.Exit(1);
            }
        }        
    }
}