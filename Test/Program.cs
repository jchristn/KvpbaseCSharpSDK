using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using KvpbaseSDK;

namespace Test
{
    public class Program
    {
        private static string _UserGuid = "";
        private static string _Endpoint = "";
        private static string _ApiKey = "";
        private static KvpbaseClient _Kvpbase;

        public static void Main(string[] args)
        {
            #region Initialize

            _UserGuid = InputString("User GUID:", "default", false);
            _ApiKey = InputString("API Key:", "default", false);
            _Endpoint = InputString("Endpoint:", "http://localhost:8000", false);
            _Kvpbase = new KvpbaseClient(_UserGuid, _ApiKey, _Endpoint);

            #endregion

            #region Variables

            bool runForever = true;
            string userInput = "";

            #endregion

            #region Menu

            while (runForever)
            {
                Console.Write("Command [? for help]: ");
                userInput = Console.ReadLine();
                if (String.IsNullOrEmpty(userInput)) continue;

                switch (userInput)
                {
                    case "?":
                        Menu();
                        break;

                    case "c":
                    case "cls":
                        Console.Clear();
                        break;

                    case "q":
                    case "quit":
                        runForever = false;
                        break;

                    case "test":
                        TestConnectivity();
                        break;

                    #region Object-Commands

                    case "object write":
                        ObjectWrite();
                        break;

                    case "object write keys":
                        ObjectWriteKeys();
                        break;

                    case "object write range":
                        ObjectWriteRange();
                        break;

                    case "object write tags":
                        ObjectWriteTags();
                        break;

                    case "object upload":
                        ObjectUpload();
                        break;

                    case "object read":
                        ObjectRead();
                        break;

                    case "object read keys":
                        ObjectReadKeys();
                        break;

                    case "object read range":
                        ObjectReadRange();
                        break;

                    case "object download":
                        ObjectDownload();
                        break;

                    case "object rename":
                        ObjectRename();
                        break;

                    case "object delete":
                        ObjectDelete();
                        break;

                    case "object exists":
                        ObjectExists();
                        break;

                    case "object metadata":
                        ObjectMetadata();
                        break;

                    #endregion

                    #region Container-Commands

                    case "container list":
                        ContainerList();
                        break;

                    case "container create":
                        ContainerCreate();
                        break;

                    case "container write keys":
                        ContainerWriteKeys();
                        break;

                    case "container read keys":
                        ContainerReadKeys();
                        break;

                    case "container read settings":
                        ContainerReadSettings();
                        break;

                    case "container update":
                        ContainerUpdate();
                        break;

                    case "container enumerate":
                        ContainerEnumerate();
                        break;

                    case "container delete":
                        ContainerDelete();
                        break;

                    case "container exists":
                        ContainerExists();
                        break;

                    #endregion 

                    default:
                        break;
                }
            }

            #endregion

            return;
        }

        public static void Menu()
        {
            // Console.WriteLine("12345678901234567890123456789012345678901234567890123456789012345678901234567890");
            Console.WriteLine("---");
            Console.WriteLine("General commands:");
            Console.WriteLine("  q                   Quit");
            Console.WriteLine("  cls                 Clear the screen");
            Console.WriteLine("  ?                   Help (this menu)");
            Console.WriteLine("  test                Test connectivity to the endpoint");
            Console.WriteLine("");
            Console.WriteLine("Object commands:");
            Console.WriteLine("  object <cmd> where <cmd> is one of the following:");
            Console.WriteLine("    write             Write an object");
            Console.WriteLine("    write keys        Write key-value pair metadata to the object");
            Console.WriteLine("    write range       Write a range of bytes to an existing object");
            Console.WriteLine("    write tags        Write tags to an object");
            Console.WriteLine("    upload            Write an object from a file");
            Console.WriteLine("    read              Read an object");
            Console.WriteLine("    read keys         Read key-value pair metadata from an object");
            Console.WriteLine("    read range        Read a range of bytes from an existing object");
            Console.WriteLine("    download          Read an object to a file");
            Console.WriteLine("    rename            Rename an object");
            Console.WriteLine("    delete            Delete an object");
            Console.WriteLine("    exists            Check if an object exists");
            Console.WriteLine("    metadata          Retrieve object metadata");
            Console.WriteLine("");
            Console.WriteLine("Container commands:");
            Console.WriteLine("  container <cmd> where <cmd> is one of the following:");
            Console.WriteLine("    list              List available containers");
            Console.WriteLine("    create            Create a container");
            Console.WriteLine("    write keys        Write key-value pair metadata to the container");
            Console.WriteLine("    read keys         Read key-value pair metadata from the container");
            Console.WriteLine("    read settings     Retrieve container settings");
            Console.WriteLine("    update            Update container settings");
            Console.WriteLine("    enumerate         Enumerate container contents");
            Console.WriteLine("    delete            Delete a container");
            Console.WriteLine("    exists            Check if a container exists");
            Console.WriteLine("");
        }

        #region Private-General-Methods

        private static void TestConnectivity()
        {
            if (!_Kvpbase.VerifyConnectivity().Result)
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
            }
        }

        private static EnumerationFilter BuildEnumerationFilter()
        {
            if (!InputBoolean("Create filter?", true)) return null;

            EnumerationFilter ret = new EnumerationFilter();

            string userInput = null;

            userInput = InputString("CreatedBefore:", DateTime.Now.ToUniversalTime().ToString(), true);
            if (!String.IsNullOrEmpty(userInput))
                ret.CreatedBefore = Convert.ToDateTime(userInput);

            userInput = InputString("CreatedAfter:", DateTime.Now.AddDays(-14).ToUniversalTime().ToString(), true);
            if (!String.IsNullOrEmpty(userInput))
                ret.CreatedAfter = Convert.ToDateTime(userInput);

            userInput = InputString("UpdatedBefore:", DateTime.Now.ToUniversalTime().ToString(), true);
            if (!String.IsNullOrEmpty(userInput))
                ret.UpdatedBefore = Convert.ToDateTime(userInput);

            userInput = InputString("UpdatedAfter:", DateTime.Now.AddDays(-14).ToUniversalTime().ToString(), true);
            if (!String.IsNullOrEmpty(userInput))
                ret.UpdatedAfter = Convert.ToDateTime(userInput);

            userInput = InputString("LastAccessBefore:", DateTime.Now.ToUniversalTime().ToString(), true);
            if (!String.IsNullOrEmpty(userInput))
                ret.LastAccessBefore = Convert.ToDateTime(userInput);

            userInput = InputString("LastAccessAfter:", DateTime.Now.AddDays(-14).ToUniversalTime().ToString(), true);
            if (!String.IsNullOrEmpty(userInput))
                ret.LastAccessAfter = Convert.ToDateTime(userInput);

            ret.Prefix = InputString("Prefix:", null, true);

            ret.Md5 = InputString("Md5:", null, true);

            ret.ContentType = InputString("ContentType:", null, true);

            userInput = InputString("SizeMin:", null, true);
            if (!String.IsNullOrEmpty(userInput))
                ret.SizeMin = Convert.ToInt64(userInput);

            userInput = InputString("SizeMax:", null, true);
            if (!String.IsNullOrEmpty(userInput))
                ret.SizeMax = Convert.ToInt64(userInput);

            userInput = InputString("Tags:", null, true);
            if (!String.IsNullOrEmpty(userInput))
                ret.Tags = CsvToStringList(userInput);

            Dictionary<string, string> kvps = InputDictionary();
            if (kvps != null && kvps.Count > 0)
                ret.KeyValuePairs = kvps;

            return ret;
        }

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

        private static Dictionary<string, string> InputDictionary()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            Console.WriteLine("Press ENTER with no input on key to exit");

            while (true)
            {
                string key = InputString("Key [ENTER to end]:", null, true);
                if (String.IsNullOrEmpty(key)) break;

                string val = InputString("Value:", null, true);

                if (ret.ContainsKey(key))
                {
                    Console.WriteLine("Key already exists");
                    continue;
                }

                ret.Add(key, val);
            }

            return ret;
        }

        private static List<string> InputStringList()
        {
            List<string> ret = new List<string>();
            Console.WriteLine("Press ENTER with no input to exit");

            while (true)
            {
                string value = InputString("Value [ENTER to end]:", null, true);
                if (String.IsNullOrEmpty(value)) break;

                ret.Add(value);
            }

            return ret;
        }

        private static byte[] StreamToBytes(Stream input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (!input.CanRead) throw new InvalidOperationException("Input stream is not readable");

            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;

                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
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

        private static List<string> CsvToStringList(string csv)
        {
            if (String.IsNullOrEmpty(csv)) return null;

            List<string> ret = new List<string>();

            string[] array = csv.Split(',');

            if (array != null)
            {
                if (array.Length > 0)
                {
                    foreach (string curr in array)
                    {
                        if (String.IsNullOrEmpty(curr)) continue;
                        ret.Add(curr.Trim());
                    }

                    return ret;
                }
            }

            return null;
        }

        #endregion

        #region Private-Object-Methods

        private static void ObjectWrite()
        {
            _Kvpbase.WriteObject(
                InputString("Container:", "default", false),
                InputString("Object Key:", "hello.txt", false),
                Encoding.UTF8.GetBytes(InputString("Data:", "Hello world!", false)),
                InputString("Content Type:", "text/plain", false)).Wait();
        }

        private static void ObjectWriteKeys()
        {
            _Kvpbase.WriteObjectKeyValuePairs(
                InputString("Container:", "default", false),
                InputString("Object Key:", "hello.txt", false),
                InputDictionary()).Wait();
        }

        private static void ObjectWriteRange()
        {
            _Kvpbase.WriteObjectRange(
                InputString("Container:", "default", false),
                InputString("Object Key:", "hello.txt", false),
                InputInteger("Start Index:", 0, true, true),
                Encoding.UTF8.GetBytes(InputString("Data:", "Hello world!", false))).Wait();
        }

        private static void ObjectWriteTags()
        {
            _Kvpbase.WriteObjectTags(
                InputString("Container:", "default", false),
                InputString("Object Key:", "hello.txt", false),
                InputStringList()).Wait();
        }

        private static void ObjectUpload()
        {
            ObjectMetadata md = _Kvpbase.UploadFile(
                InputString("Filename:", null, false),
                InputString("Container:", "default", false),
                InputString("Object Key:", "hello.txt", false),
                InputString("Content Type:", "text/plain", false)).Result;

            if (md == null)
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
                Console.WriteLine(SerializeJson(md, true));
            }
        }

        private static void ObjectRead()
        {
            KvpbaseObject ret = _Kvpbase.ReadObject(
                InputString("Container:", "default", false),
                InputString("Object Key:", "hello.txt", false)).Result;

            if (ret.ContentLength > 0 && ret.Data != null)
            {
                Console.WriteLine(Encoding.UTF8.GetString(StreamToBytes(ret.Data)));
            }
        }

        private static void ObjectReadKeys()
        {
            Dictionary<string, string> keyValuePairs = _Kvpbase.ReadObjectKeyValuePairs(
                InputString("Container:", "default", false),
                InputString("Object Key:", "hello.txt", false)).Result;

            if (keyValuePairs != null && keyValuePairs.Count > 0)
            {
                foreach (KeyValuePair<string, string> curr in keyValuePairs)
                {
                    Console.WriteLine("  " + curr.Key + "=" + curr.Value);
                }
            }
        }

        private static void ObjectReadRange()
        {
            KvpbaseObject ret = _Kvpbase.ReadObjectRange(
                InputString("Container:", "default", false),
                InputString("Object Key:", "hello.txt", false),
                InputInteger("Start Index:", 0, true, true),
                InputInteger("Count:", 10, true, false)).Result;

            if (ret.ContentLength > 0 && ret.Data != null)
            {
                Console.WriteLine(Encoding.UTF8.GetString(StreamToBytes(ret.Data)));
            }
        }

        private static void ObjectDownload()
        {
            _Kvpbase.DownloadFile(
                InputString("Filename:", null, false),
                InputString("Container:", "default", false),
                InputString("Object Key:", "hello.txt", false)).Wait();
        }

        private static void ObjectRename()
        {
            _Kvpbase.RenameObject(
                InputString("Container:", "default", false),
                InputString("Original Object Key:", "hello.txt", false),
                InputString("New Object Key:", "renamed.txt", false)).Wait();
        }

        private static void ObjectDelete()
        {
            _Kvpbase.DeleteObject(
                InputString("Container:", "default", false),
                InputString("Object Key:", "hello.txt", false)).Wait();
        }

        private static void ObjectExists()
        {
            if (!_Kvpbase.ObjectExists(
                InputString("Container:", "default", false),
                InputString("Object Key:", "hello.txt", false)).Result)
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
            }
        }

        private static void ObjectMetadata()
        {
            ObjectMetadata metadata = _Kvpbase.ReadObjectMetadata(
                InputString("Container:", "default", false),
                InputString("Object Key:", "hello.txt", false)).Result;

            if (metadata == null)
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
                Console.WriteLine(SerializeJson(metadata, true));
            }
        }

        #endregion

        #region Private-Container-Methods

        private static void ContainerList()
        {
            List<string> containers = _Kvpbase.ListContainers().Result;
            if (containers != null && containers.Count > 0)
            {
                foreach (string curr in containers)
                {
                    Console.WriteLine("  " + curr);
                }
            }
        }

        private static void ContainerCreate()
        {
            _Kvpbase.CreateContainer(
                InputString("Container:", "default", false),
                InputBoolean("Public Read:", true),
                InputBoolean("Public Write:", false),
                InputBoolean("Audit Logging:", true)).Wait();
        }

        private static void ContainerWriteKeys()
        {
            _Kvpbase.WriteContainerKeyValuePairs(
                InputString("Container:", "default", false),
                InputDictionary()).Wait();
        }

        private static void ContainerReadSettings()
        {
            Container container = _Kvpbase.GetContainerSettings(
                InputString("Container:", "default", false)).Result;


            if (container != null)
            {
                Console.WriteLine("Success");
                Console.WriteLine(SerializeJson(container, true));
            }
        }

        private static void ContainerReadKeys()
        {
            Dictionary<string, string> keyValuePairs = _Kvpbase.GetContainerKeyValuePairs(
                InputString("Container:", "default", false)).Result;

            if (keyValuePairs != null)
            {
                foreach (KeyValuePair<string, string> curr in keyValuePairs)
                {
                    Console.WriteLine("  " + curr.Key + "=" + curr.Value);
                }
            }
        }

        private static void ContainerUpdate()
        {
            Container container = _Kvpbase.GetContainerSettings(
                InputString("Container:", "default", false)).Result;

            if (container != null)
            {
                container.IsPublicRead = InputBoolean("Public Read:", true);
                container.IsPublicWrite = InputBoolean("Public Write:", false);
                container.EnableAuditLogging = InputBoolean("Audit Logging:", true);

                _Kvpbase.UpdateContainer(container).Wait();
            }
        }

        private static void ContainerEnumerate()
        {
            ContainerMetadata metadata = _Kvpbase.EnumerateContainer(
                BuildEnumerationFilter(),
                InputString("Container:", "default", false),
                InputInteger("Start Index:", 0, true, true),
                InputInteger("Count:", 10, true, false)).Result;

            if (metadata != null)
            {
                Console.WriteLine("Success");
                Console.WriteLine(SerializeJson(metadata, true));
            }
        }

        private static void ContainerDelete()
        {
            _Kvpbase.DeleteContainer(InputString("Container:", "default", false)).Wait();
        }

        private static void ContainerExists()
        {
            if (!_Kvpbase.ContainerExists(
                   InputString("Container:", "default", false)).Result)
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
            }
        }

        #endregion
    }
}
