using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KvpbaseSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace KvpbaseStreamTest
{
    class Program
    {
        static string _ApiKey;
        static string _UserGuid;
        static string _Endpoint;
        static string _Container;
        static string _ObjectKey;

        static KvpbaseClient _Kvpbase;
        static KvpbaseStream _Stream;

        static bool _RunForever = true;

        static void Main(string[] args)
        {
            _ApiKey = InputString("API key:", "default", false);
            _UserGuid = InputString("User GUID:", "default", false);
            _Endpoint = InputString("Endpoint:", "http://localhost:8001", false);
            _Container = InputString("Container:", "test", false);
            _ObjectKey = InputString("Object key:", "test1.txt", false);

            _Kvpbase = new KvpbaseClient(_UserGuid, _ApiKey, _Endpoint);
            _Stream = new KvpbaseStream(_Kvpbase, _Container, _ObjectKey);

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

                    case "len":
                        Console.WriteLine(_Stream.Length);
                        break;

                    case "pos":
                        Console.WriteLine(_Stream.Position);
                        break;

                    case "seek":
                        Seek();
                        break;

                    case "w":
                        Write();
                        break;

                    case "r":
                        Read();
                        break;

                    case "md":
                        Console.WriteLine(SerializeJson(_Stream.Metadata, true));
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
            Console.WriteLine("  len       Show the length");
            Console.WriteLine("  pos       Show the current position");
            Console.WriteLine("  seek      Seek to a specific place in the stream");
            Console.WriteLine("  w         Write to the stream");
            Console.WriteLine("  r         Read from the stream");
            Console.WriteLine("  md        Show the object metadata");
        }

        #region Basic-Stream-Methods

        static void Seek()
        {
            string origin = InputString("Origin [begin/current/end]:", "begin", false);
            long offset = Convert.ToInt64(InputString("Offset:", "0", false));

            switch (origin)
            {
                case "begin":
                    _Stream.Seek(offset, System.IO.SeekOrigin.Begin);
                    break;
                case "current":
                    _Stream.Seek(offset, System.IO.SeekOrigin.Current);
                    break;
                case "end":
                    _Stream.Seek(offset, System.IO.SeekOrigin.End);
                    break;
            }
        }

        static void Write()
        {
            byte[] data = Encoding.UTF8.GetBytes(InputString("Data:", "test", false));
            _Stream.Write(data, 0, data.Length);
        }

        static void Read()
        {
            int count = InputInteger("Count:", 1, true, false);
            byte[] data = new byte[count];
            _Stream.Read(data, 0, count);
            Console.WriteLine(Encoding.UTF8.GetString(data));
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
