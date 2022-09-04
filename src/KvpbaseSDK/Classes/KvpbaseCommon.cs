﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KvpbaseSDK
{ 
    /// <summary>
    /// Static methods used by the Kvpbase client.
    /// </summary>
    internal static class KvpbaseCommon
    { 
        #region Input

        internal static bool InputBoolean(string question, bool yesDefault)
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

        internal static string InputString(string question, string defaultAnswer, bool allowNull)
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

        internal static int InputInteger(string question, int defaultAnswer, bool positiveOnly, bool allowZero)
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

        internal static Dictionary<string, string> InputDictionary()
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

        internal static List<string> InputStringList()
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

        #endregion

        #region IsTrue

        internal static bool IsTrue(bool val)
        {
            return val;
        }

        internal static bool IsTrue(bool? val)
        {
            if (val == null) return false;
            return Convert.ToBoolean(val);
        }

        internal static bool IsTrue(string val)
        {
            if (String.IsNullOrEmpty(val)) return false;
            val = val.ToLower().Trim();
            int valInt = 0;
            if (Int32.TryParse(val, out valInt)) if (valInt == 1) return true;
            if (String.Compare(val, "true") == 0) return true;
            return false;
        }

        #endregion
         
        #region Compress

        internal static byte[] GzipCompress(byte[] input)
        {
            if (input == null) return null;
            if (input.Length < 1) return null;

            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gzip.Write(input, 0, input.Length);
                }
                return memory.ToArray();
            }
        }

        internal static byte[] GzipDecompress(byte[] input)
        {
            if (input == null) return null;
            if (input.Length < 1) return null;

            using (GZipStream stream = new GZipStream(new MemoryStream(input), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        #endregion

        #region Misc

        internal static byte[] StreamToBytes(Stream input)
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

        internal static string StringRemove(string original, string remove)
        {
            if (String.IsNullOrEmpty(original)) return null;
            if (String.IsNullOrEmpty(remove)) return original;

            int index = original.IndexOf(remove);
            string ret = (index < 0)
                ? original
                : original.Remove(index, remove.Length);

            return ret;
        }

        internal static string Line(int count, string fill)
        {
            if (count < 1) return "";

            string ret = "";
            for (int i = 0; i < count; i++)
            {
                ret += fill;
            }

            return ret;
        }

        internal static string RandomString(int numChar)
        {
            string ret = "";
            if (numChar < 1) return null;
            int valid = 0;
            Random random = new Random((int)DateTime.Now.Ticks);
            int num = 0;

            for (int i = 0; i < numChar; i++)
            {
                num = 0;
                valid = 0;
                while (valid == 0)
                {
                    num = random.Next(126);
                    if (((num > 47) && (num < 58)) ||
                        ((num > 64) && (num < 91)) ||
                        ((num > 96) && (num < 123)))
                    {
                        valid = 1;
                    }
                }
                ret += (char)num;
            }

            return ret;
        }

        internal static double TotalMsFrom(DateTime startTime)
        {
            try
            {
                DateTime endTime = DateTime.Now;
                TimeSpan totalTime = (endTime - startTime);
                return totalTime.TotalMilliseconds;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        internal static bool IsLaterThanNow(DateTime? dt)
        {
            try
            {
                DateTime curr = Convert.ToDateTime(dt);
                return IsLaterThanNow(curr);
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static bool IsLaterThanNow(DateTime dt)
        {
            if (DateTime.Compare(dt, DateTime.Now) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static bool ContainsUnsafeCharacters(string data)
        {
            /*
             * 
             * Returns true if unsafe characters exist
             * 
             * 
             */

            // see https://kb.acronis.com/content/39790

            if (String.IsNullOrEmpty(data)) return false;
            if (data.Equals(".")) return true;
            if (data.Equals("..")) return true;

            if (
                (String.Compare(data.ToLower(), "com1") == 0) ||
                (String.Compare(data.ToLower(), "com2") == 0) ||
                (String.Compare(data.ToLower(), "com3") == 0) ||
                (String.Compare(data.ToLower(), "com4") == 0) ||
                (String.Compare(data.ToLower(), "com5") == 0) ||
                (String.Compare(data.ToLower(), "com6") == 0) ||
                (String.Compare(data.ToLower(), "com7") == 0) ||
                (String.Compare(data.ToLower(), "com8") == 0) ||
                (String.Compare(data.ToLower(), "com9") == 0) ||
                (String.Compare(data.ToLower(), "lpt1") == 0) ||
                (String.Compare(data.ToLower(), "lpt2") == 0) ||
                (String.Compare(data.ToLower(), "lpt3") == 0) ||
                (String.Compare(data.ToLower(), "lpt4") == 0) ||
                (String.Compare(data.ToLower(), "lpt5") == 0) ||
                (String.Compare(data.ToLower(), "lpt6") == 0) ||
                (String.Compare(data.ToLower(), "lpt7") == 0) ||
                (String.Compare(data.ToLower(), "lpt8") == 0) ||
                (String.Compare(data.ToLower(), "lpt9") == 0) ||
                (String.Compare(data.ToLower(), "con") == 0) ||
                (String.Compare(data.ToLower(), "nul") == 0) ||
                (String.Compare(data.ToLower(), "prn") == 0) ||
                (String.Compare(data.ToLower(), "con") == 0)
                )
            {
                return true;
            }

            for (int i = 0; i < data.Length; i++)
            {
                if (
                    ((int)(data[i]) < 32) ||    // below range
                    ((int)(data[i]) > 126) ||   // above range
                    ((int)(data[i]) == 47) ||   // slash /
                    ((int)(data[i]) == 92) ||   // backslash \
                    ((int)(data[i]) == 63) ||   // question mark ?
                    ((int)(data[i]) == 60) ||   // less than < 
                    ((int)(data[i]) == 62) ||   // greater than >
                    ((int)(data[i]) == 58) ||   // colon :
                    ((int)(data[i]) == 42) ||   // asterisk *
                    ((int)(data[i]) == 124) ||  // pipe |
                    ((int)(data[i]) == 34) ||   // double quote "
                    ((int)(data[i]) == 39) ||   // single quote '
                    ((int)(data[i]) == 94)      // caret ^
                    )
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool ContainsUnsafeCharacters(List<string> data)
        {
            if (data == null || data.Count < 1) return true;
            foreach (string curr in data)
            {
                if (ContainsUnsafeCharacters(curr)) return true;
            }
            return false;
        }

        internal static int GuidToInt(string guid)
        {
            if (String.IsNullOrEmpty(guid)) return 0;
            byte[] bytes = Encoding.UTF8.GetBytes(guid);
            int ret = 0;

            foreach (byte currByte in bytes)
            {
                ret += (int)currByte;
            }

            return ret;
        }

        #endregion

        #region Dictionary

        internal static Dictionary<string, string> AddToDictionary(string key, string val, Dictionary<string, string> existing)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            if (existing == null)
            {
                ret.Add(key, val);
                return ret;
            }
            else
            {
                existing.Add(key, val);
                return existing;
            }
        }

        #endregion

        #region Crypto

        internal static string Md5(byte[] data)
        {
            if (data == null) return null;

            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(data);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++) sb.Append(hash[i].ToString("X2"));
            string ret = sb.ToString();
            return ret;
        }

        internal static string Md5(string data)
        {
            if (String.IsNullOrEmpty(data)) return null;

            MD5 md5 = MD5.Create();
            byte[] dataBytes = System.Text.Encoding.ASCII.GetBytes(data);
            byte[] hash = md5.ComputeHash(dataBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++) sb.Append(hash[i].ToString("X2"));
            string ret = sb.ToString();
            return ret;
        }

        #endregion

        #region Encoding

        internal static byte[] Base64ToBytes(string data)
        {
            return Convert.FromBase64String(data);
        }

        internal static string Base64ToString(string data)
        {
            if (String.IsNullOrEmpty(data)) return null;
            byte[] bytes = System.Convert.FromBase64String(data);
            return System.Text.UTF8Encoding.UTF8.GetString(bytes);
        }

        internal static string BytesToBase64(byte[] data)
        {
            if (data == null) return null;
            if (data.Length < 1) return null;
            return System.Convert.ToBase64String(data);
        }

        internal static string StringToBase64(string data)
        {
            if (String.IsNullOrEmpty(data)) return null;
            byte[] bytes = System.Text.UTF8Encoding.UTF8.GetBytes(data);
            return System.Convert.ToBase64String(bytes);
        }

        internal static string StringListToCsv(List<string> str)
        {
            if (str == null || str.Count < 1) return null;
            return string.Join(",", str);
        }

        internal static List<string> CsvToStringList(string csv)
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
    }
}