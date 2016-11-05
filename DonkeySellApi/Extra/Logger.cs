using System;
using System.Threading.Tasks;

namespace DonkeySellApi.Extra
{
    public interface ILogger
    {
        void Log(Exception ex);
    }


    public class Logger : ILogger
    {
        private static readonly log4net.ILog MyLogger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public void Log(Exception ex)
        {
            Task.Run(() =>
            {
                MyLogger.Error(DateTime.Now, ex);
            });
        }
    }
}