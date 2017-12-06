using System;

using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace RESTApi.Repositories
{
    public class ErrorHandlingRepository
    {
        public void LogAndPrintError(Exception e)
        {
            var fileName = AppDomain.CurrentDomain.BaseDirectory + "restapilog.txt";
            var messageToLogAndPrint = this.GetErrorMessageToLogAndPrint(e);
            messageToLogAndPrint = messageToLogAndPrint.Remove(messageToLogAndPrint.Length - 3, 3);

            using (StreamWriter w = File.AppendText(fileName))
            {
                w.WriteLine("Error occured: " + DateTime.Now);
                w.WriteLine(messageToLogAndPrint);
                w.WriteLine();
            }

            Console.WriteLine(messageToLogAndPrint);
        }

        private string GetErrorMessageToLogAndPrint(Exception exception)
        {
            var result = string.Empty;

            while (true)
            {
                result += exception.Message + " - ";
                if (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
                else
                {
                    break;
                }
            }

            return result;
        }
    }
}
