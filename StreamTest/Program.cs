using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KvpbaseSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace StreamTest
{
    class Program
    {
        static string _ApiKey;
        static string _UserGuid;
        static string _Endpoint;
        static string _Container; 

        static KvpbaseClient _Kvpbase; 

        static bool _RunForever = true;

        static void Main(string[] args)
        {
            _ApiKey = InputString("API key:", "default", false);
            _UserGuid = InputString("User GUID:", "default", false);
            _Endpoint = InputString("Endpoint:", "http://localhost:8001", false);
            _Container = InputString("Container:", "test", false);
             
            _Kvpbase = new KvpbaseClient(_UserGuid, _ApiKey, _Endpoint);

            _Kvpbase.DownloadStreamBufferSize = 4096;
            _Kvpbase.UploadStreamBufferSize = 4096;

            while (_RunForever)
            {
                string userInput = InputString("Command [? for help]:", null, false);

                switch (userInput)
                {
                    case "?":
                        Menu();
                        break;

                    case "q":
                        _RunForever = false;
                        break;

                    case "c":
                    case "cls":
                        Console.Clear();
                        break;

                    case "up":
                        Upload();
                        break;

                    case "down":
                        Download();
                        break;

                    case "buf":
                        _Kvpbase.DownloadStreamBufferSize = InputInteger("Stream buffer size:", 4096, true, false);
                        _Kvpbase.UploadStreamBufferSize = _Kvpbase.DownloadStreamBufferSize;
                        break;
                         
                    default:
                        break;
                }
            }
        }

        static void Menu()
        {
            Console.WriteLine("--- Available Commands ---");
            Console.WriteLine("  ?         Help, this menu");
            Console.WriteLine("  q         Quit");
            Console.WriteLine("  cls       Clear the screen");
            Console.WriteLine("  up        Upload an object");
            Console.WriteLine("  down      Download an object");
            Console.WriteLine("  buf       Set buffer size");
        }

        #region Basic-Stream-Methods
         
        static void Upload()
        {
            string filename = InputString("Filename:", null, false);
            string key = InputString("Object key:", null, false);
            string contentType = InputString("Content type:", "application/octet-stream", false);
            ObjectMetadata metadata = null;

            if (_Kvpbase.UploadFile(filename, _Container, key, contentType, out metadata))
            {
                Console.WriteLine("Success");
                Console.WriteLine(SerializeJson(metadata, true));
            }
            else
            {
                Console.WriteLine("Failed");
            }
        }

        static void Download()
        {
            string key = InputString("Object key:", null, false);
            string filename = InputString("Filename:", null, false); 

            if (_Kvpbase.DownloadFile(filename, _Container, key))
            {
                Console.WriteLine("Success"); 
            }
            else
            {
                Console.WriteLine("Failed");
            }
        }

        #endregion

        #region Supporting-Methods

        private static bool InputBoolean(string question, bool yesDefault)
        {
            Console.Write(question);

            if (yesDefault) Console.Write(" [Y/n]? ");
            else Console.Write(" [y/N]? ");

            string userInput = Console.ReadLine();

            if (String.IsNullOrEmpty(userInput))
            {
                if (yesDefault) return true;
                return false;
            }

            userInput = userInput.ToLower();

            if (yesDefault)
            {
                if (
                    (String.Compare(userInput, "n") == 0)
                    || (String.Compare(userInput, "no") == 0)
                   )
                {
                    return false;
                }

                return true;
            }
            else
            {
                if (
                    (String.Compare(userInput, "y") == 0)
                    || (String.Compare(userInput, "yes") == 0)
                   )
                {
                    return true;
                }

                return false;
            }
        }

        private static string InputString(string question, string defaultAnswer, bool allowNull)
        {
            while (true)
            {
                Console.Write(question);

                if (!String.IsNullOrEmpty(defaultAnswer))
                {
                    Console.Write(" [" + defaultAnswer + "]");
                }

                Console.Write(" ");

                string userInput = Console.ReadLine();

                if (String.IsNullOrEmpty(userInput))
                {
                    if (!String.IsNullOrEmpty(defaultAnswer)) return defaultAnswer;
                    if (allowNull) return null;
                    else continue;
                }

                return userInput;
            }
        }

        private static List<string> InputStringList(string question, bool allowEmpty)
        {
            List<string> ret = new List<string>();

            while (true)
            {
                Console.Write(question);

                Console.Write(" ");

                string userInput = Console.ReadLine();

                if (String.IsNullOrEmpty(userInput))
                {
                    if (ret.Count < 1 && !allowEmpty) continue;
                    return ret;
                }

                ret.Add(userInput);
            }
        }

        private static int InputInteger(string question, int defaultAnswer, bool positiveOnly, bool allowZero)
        {
            while (true)
            {
                Console.Write(question);
                Console.Write(" [" + defaultAnswer + "] ");

                string userInput = Console.ReadLine();

                if (String.IsNullOrEmpty(userInput))
                {
                    return defaultAnswer;
                }

                int ret = 0;
                if (!Int32.TryParse(userInput, out ret))
                {
                    Console.WriteLine("Please enter a valid integer.");
                    continue;
                }

                if (ret == 0)
                {
                    if (allowZero)
                    {
                        return 0;
                    }
                }

                if (ret < 0)
                {
                    if (positiveOnly)
                    {
                        Console.WriteLine("Please enter a value greater than zero.");
                        continue;
                    }
                }

                return ret;
            }
        }

        private static string SerializeJson(object obj, bool pretty)
        {
            if (obj == null) return null;
            string json;

            if (pretty)
            {
                json = JsonConvert.SerializeObject(
                  obj,
                  Newtonsoft.Json.Formatting.Indented,
                  new JsonSerializerSettings
                  {
                      NullValueHandling = NullValueHandling.Ignore,
                      DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                  });
            }
            else
            {
                json = JsonConvert.SerializeObject(obj,
                  new JsonSerializerSettings
                  {
                      NullValueHandling = NullValueHandling.Ignore,
                      DateTimeZoneHandling = DateTimeZoneHandling.Utc
                  });
            }

            return json;
        }

        private static T DeserializeJson<T>(string json)
        {
            if (String.IsNullOrEmpty(json)) throw new ArgumentNullException(nameof(json));

            return JsonConvert.DeserializeObject<T>(json);
        }

        private static T DeserializeJson<T>(byte[] data)
        {
            if (data == null || data.Length < 1) throw new ArgumentNullException(nameof(data));
            return DeserializeJson<T>(Encoding.UTF8.GetString(data));
        }

        #endregion
    }
}
