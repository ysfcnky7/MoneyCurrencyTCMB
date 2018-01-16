using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace SanConsole
{
    public class XmlReader
    {
        //ilk 12 parabirimi için sayaç
        private int moneyCounter = 12;
        private static string conString = "Server=yusufpc;Database=SAN;Uid=sa;Password=394722ys;";
        private SqlConnection connection = new SqlConnection(conString);
        public void Reader()
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                XmlTextReader today = new XmlTextReader("http://www.tcmb.gov.tr/kurlar/today.xml");
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(today);

                DateTime exchangeDate = Convert.ToDateTime(xmlDoc.SelectSingleNode("//Tarih_Date").Attributes["Tarih"].Value);
                String bulletin = Convert.ToString(xmlDoc.SelectSingleNode("//Tarih_Date").Attributes["Bulten_No"].Value);

                XmlNode Date = xmlDoc.SelectSingleNode("/Tarih_Date/@Tarih");
                XmlNodeList mylist = xmlDoc.SelectNodes("/Tarih_Date/Currency");
                XmlNodeList Name = xmlDoc.SelectNodes("/Tarih_Date/Currency/Isim");
                XmlNodeList Code = xmlDoc.SelectNodes("/Tarih_Date/Currency/@Kod");
                XmlNodeList Exchange_Buying = xmlDoc.SelectNodes("/Tarih_Date/Currency/ForexBuying");
                XmlNodeList Exchange_Selling = xmlDoc.SelectNodes("/Tarih_Date/Currency/ForexSelling");
                XmlNodeList Effective_Reception = xmlDoc.SelectNodes("/Tarih_Date/Currency/BanknoteBuying");
                XmlNodeList Effective_Sales = xmlDoc.SelectNodes("/Tarih_Date/Currency/BanknoteSelling");

                //checkDatabase metodu ile kayıt var mı bakıyorum yoksa insert varsa update sogusu çalıştırıyorum
                string sqlQuery = "";
                for (int i = 0; i < moneyCounter; i++)
                {
                    if (checkDatabase(Code.Item(i).InnerText.ToString()))
                    {
                        sqlQuery = "Update MoneyCurrencies Set " +
                           "Name=@Name," +
                           "Code=@Code," +
                           "Exchange_Buying=@Exchange_Buying," +
                           "Exchange_Selling=@Exchange_Selling," +
                           "Effective_Reception=@Effective_Reception," +
                           "Date=@Date," +
                           "Effective_Sales=@Effective_Sales where Code=@Code";
                    }
                    else
                    {
                        sqlQuery = "insert into MoneyCurrencies(" +
                            "Name," +
                            "Code," +
                            "Exchange_Buying," +
                            "Exchange_Selling," +
                            "Effective_Reception," +
                            "Effective_Sales," +
                            "Date" +
                            ") values (" +
                            "@Name," +
                            "@Code," +
                            "@Exchange_Buying," +
                            "@Exchange_Selling," +
                            "@Effective_Reception," +
                            "@Effective_Sales," +
                            "@Date" +
                        ")";
                    }

                    var name = Name.Item(i).InnerText.ToString();
                    Console.WriteLine(Name.Item(i).InnerText.ToString());
                    var sym = Code.Item(i).InnerText.ToString();
                    Console.WriteLine(Code.Item(i).InnerText.ToString());
                    var buy = Exchange_Buying.Item(i).InnerText.ToString();
                    Console.WriteLine(Exchange_Buying.Item(i).InnerText.ToString());
                    var sell = Exchange_Selling.Item(i).InnerText.ToString();
                    Console.WriteLine(Exchange_Selling.Item(i).InnerText.ToString());
                    var ebuy = Effective_Reception.Item(i).InnerText.ToString();
                    Console.WriteLine(Effective_Reception.Item(i).InnerText.ToString());
                    var esell = Effective_Sales.Item(i).InnerText.ToString();
                    Console.WriteLine(Effective_Sales.Item(i).InnerText.ToString());

                    SqlCommand cmd = new SqlCommand(sqlQuery, connection);

                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@Code", SqlDbType.NVarChar).Value = sym;
                    cmd.Parameters.Add("@Exchange_Buying", SqlDbType.NVarChar).Value = buy;
                    cmd.Parameters.Add("@Exchange_Selling", SqlDbType.NVarChar).Value = sell;
                    cmd.Parameters.Add("@Effective_Reception", SqlDbType.NVarChar).Value = ebuy;
                    cmd.Parameters.Add("@Effective_Sales", SqlDbType.NVarChar).Value = esell;
                    cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.ExecuteNonQuery();
                }//for done
                Console.WriteLine("Kayıt İşlemi Başarıyla tamamlandı...");
            }//try done
            catch (Exception error)
            {
                Console.WriteLine("Bir Hata İle Karşılaşıldı" + error.Message);
            }//catch done
        }
        //Kayıtların kontrolünü para birimin kısaltmasına göre bakıyorum Code'a göre 
        private bool checkDatabase(string code)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM MoneyCurrencies WHERE Code ='" + code + "'", connection);
            object Adi = cmd.ExecuteScalar();

            if (Adi == null)
            {
                return false;
            }
            return true;
        }
    }
}

