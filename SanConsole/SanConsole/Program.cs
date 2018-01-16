using System;
using System.Data.SqlClient;
using System.Threading;

namespace SanConsole
{
    class Program
    {
        //Timer Belli zaman aralıklarında programı yeniden run et
        static void Main(string[] args)
        {
            Timer t = new Timer(TimerCallback, null, 0, 300000);
            Console.ReadLine();
        }

        private static void TimerCallback(Object o)
        {
            string conString = "Server=yusufpc;Database=SAN;Uid=sa;Password=394722ys;";
            SqlConnection connection = new SqlConnection(conString);
            Console.WriteLine("Güncel Kur Bilgileri");
            XmlReader rd = new XmlReader();
            //İşlemleri yapacak sınıf
            rd.Reader();
            connection.Close();
            GC.Collect();
        }
    }
}
