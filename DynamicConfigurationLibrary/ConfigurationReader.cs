using Microsoft.Data.SqlClient;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Reflection.PortableExecutable;
using System.Threading;

namespace ConfigurationLibrary
{
    public class ConfigurationReader 
    {
        private readonly string _applicationName;
        private readonly string _connectionString;
        private readonly int _refreshTimerIntervalInMs;
        private readonly Timer _refreshTimer;
        private readonly ConcurrentDictionary<string, (string Type, string Value)> _configCache;

        public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs)
        {
            if (applicationName == null)
            {
                Console.WriteLine("Uygulama adı boş olamaz.");
                return;
            }
            else
            {
                _applicationName = applicationName;
            }

            if (connectionString == null)
            {
                Console.WriteLine("Connection String boş olamaz.");
                return;
            }
            else
            {
                _connectionString = connectionString;
            }

            if (refreshTimerIntervalInMs <= 0)
            {
                Console.WriteLine("Yenileme aralığı pozitif bir sayı olmalıdır. Varsayılan değer 10000 ms kullanılacaktır.");
                _refreshTimerIntervalInMs = 10000; // 10 saniye
            }
            else
            {
                _refreshTimerIntervalInMs = refreshTimerIntervalInMs;
            }

            // Yapılandırmaları bellekte tutacak 
            _configCache = new ConcurrentDictionary<string, (string Type, string Value)>();

            // Belirli aralıklarla yapılandırmaları güncellemek için zamanlayıcı
            _refreshTimer = new Timer(RefreshConfig, null, 0, _refreshTimerIntervalInMs);

            LoadConfigurations();
        }

        private void RefreshConfig(object state)
        {
            LoadConfigurations();
        }

        /// <summary>
        /// Veritabanından uygulamaya özel yapılandırma ayarlarını çeker ve bellek (cache) içinde saklar.
        /// Bu metot belirli aralıklarla çalışarak güncellenmiş verileri alır.
        /// </summary>
        private void LoadConfigurations()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "SELECT Name, Type, Value FROM ConfigurationSettings WHERE ApplicationName = @AppName AND IsActive = 1";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AppName", _applicationName);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read()) 
                            {
                                string name = reader.GetString(0);
                                string type = reader.GetString(1);
                                string value = reader.GetString(2);

                                _configCache[name] = (type, value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Yapılandırmalar alınırken hata oluştu: " + ex.Message);
            }
        }
        /// <summary>
            ///Bu metod, key'i alır ve cache'den veriyi bulur.Eğer tür destekleniyorsa, uygun dönüşüm yapılır ve değer döndürülür.
            ///Eğer key bulunamazsa veya tür desteklenmiyorsa, hata fırlatılır.
        /// </summary>
        public T GetValue<T>(string key)
        {
            if (!_configCache.ContainsKey(key))
            {
                throw new KeyNotFoundException($"Ayar '{key}' bulunamadı.");
            }

            var (type, value) = _configCache[key];

            if (type == "string")
            {
                return (T)(object)value;
            }
            else if (type == "int")
            {
                return (T)(object)int.Parse(value);
            }
            else if (type == "bool")
            {
                return (T)(object)(value == "1" || bool.Parse(value));
            }
            else if (type == "double")
            {
                return (T)(object)double.Parse(value);
            }
            else
            {
                throw new InvalidOperationException($"Desteklenmeyen tür: {type}");
            }
        }
        /// <summary>
        /// controllerda doğrudan çağırmak için kullanıldı. 
        /// Bu metod, cache'deki tüm yapılandırma ayarlarını (ad, tür, değer) dışarıya yeni bir Dictionary olarak döndürür.
        /// Bu sayede dışarıdaki kod, yapılandırma verilerine erişim sağlayabilir ve değişiklik yapmadan sadece okumak için kullanabilir.
        /// </summary>
        public Dictionary<string, (string Type, string Value)> GetAll()
        {
            return new Dictionary<string, (string Type, string Value)>(_configCache);
        }
       
    }
}