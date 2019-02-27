using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using KvpbaseSDK;

namespace Test
{
	public class MainClass
	{
        private static string _UserGuid = "";
		private static string _Endpoint = ""; 
		private static string _ApiKey = "";
        private static string _Email = "";
        private static string _Password = "";
        private static KvpbaseClient _Kvpbase;

		public static void Main(string[] args)
		{
            #region Initialize

            _UserGuid = KvpbaseCommon.InputString("User GUID:", "default", false);
            _Endpoint = KvpbaseCommon.InputString("Endpoint:", "http://localhost:8080", false);
            _ApiKey = KvpbaseCommon.InputString("API Key:", "default", false);
            _Email = KvpbaseCommon.InputString("Email:", "default@default.com", false);
            _Password = KvpbaseCommon.InputString("Password:", "default", false);
            _Kvpbase = new KvpbaseClient(_UserGuid, _Email, _Password, _Endpoint);

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

                    case "login":
                        Login();
                        break;
                         
                    #region Object-Commands

                    case "object write":
                        ObjectWrite();
                        break;

                    case "object write range":
                        ObjectWriteRange();
                        break;

                    case "object read":
                        ObjectRead();
                        break;

                    case "object read range":
                        ObjectReadRange();
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

                    case "container get settings":
                        ContainerGetSettings();
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
            Console.WriteLine("  login               Test login to the endpoint"); 
			Console.WriteLine("");
			Console.WriteLine("Object commands:");
            Console.WriteLine("  object <cmd> where <cmd> is one of the following:");
            Console.WriteLine("    write             Write an object");
            Console.WriteLine("    write range       Write a range of bytes to an existing object");
            Console.WriteLine("    read              Read an object");
            Console.WriteLine("    read range        Read a range of bytes from an existing object");
            Console.WriteLine("    rename            Rename an object");
            Console.WriteLine("    delete            Delete an object");
            Console.WriteLine("    exists            Check if an object exists");
            Console.WriteLine("    metadata          Retrieve object metadata");
			Console.WriteLine("");
			Console.WriteLine("Container commands:");
            Console.WriteLine("  container <cmd> where <cmd> is one of the following:");
            Console.WriteLine("    list              List available containers");
            Console.WriteLine("    create            Create a container");
            Console.WriteLine("    get settings      Retrieve container settings");
            Console.WriteLine("    update            Update container settings");
            Console.WriteLine("    enumerate         Enumerate container contents");
            Console.WriteLine("    delete            Delete a container");
            Console.WriteLine("    exists            Check if a container exists");
			Console.WriteLine("");
		}

        #region Private-General-Methods
         
        private static ReplicationMode GetReplicationMode()
        {
            while (true)
            {
                Console.Write("Replication Mode: [none|sync|async] ");
                string input = Console.ReadLine();
                if (String.IsNullOrEmpty(input)) continue;

                if (input.ToLower().Equals("none")) return ReplicationMode.None;
                if (input.ToLower().Equals("sync")) return ReplicationMode.Sync;
                if (input.ToLower().Equals("async")) return ReplicationMode.Async;
            }
        }

        private static void TestConnectivity()
        {
            if (!_Kvpbase.VerifyConnectivity())
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
            }
        }

        private static void Login()
        {
            if (!_Kvpbase.Authenticate())
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
            }
        }

        #endregion

        #region Private-Object-Methods

        private static void ObjectWrite()
        {
            if (!_Kvpbase.WriteObject(
                KvpbaseCommon.InputString("Container:", "default", false),
                KvpbaseCommon.InputString("Object Key:", "hello.txt", false),
                KvpbaseCommon.InputString("Content Type:", "text/plain", false),
                Encoding.UTF8.GetBytes(KvpbaseCommon.InputString("Data:", "Hello world!", false))))
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
            }
        }

        private static void ObjectWriteRange()
        {
            if (!_Kvpbase.WriteObjectRange(
                KvpbaseCommon.InputString("Container:", "default", false),
                KvpbaseCommon.InputString("Object Key:", "hello.txt", false),
                KvpbaseCommon.InputInteger("Start Index:", 0, true, true),
                Encoding.UTF8.GetBytes(KvpbaseCommon.InputString("Data:", "Hello world!", false))))
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
            }
        }

        private static void ObjectRead()
        {
            byte[] data = null;

            if (!_Kvpbase.ReadObject(
                KvpbaseCommon.InputString("Container:", "default", false),
                KvpbaseCommon.InputString("Object Key:", "hello.txt", false),
                out data))
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
                if (data != null && data.Length > 0)
                {
                    Console.WriteLine(Encoding.UTF8.GetString(data));
                }
            }
        }

        private static void ObjectReadRange()
        {
            byte[] data = null;

            if (!_Kvpbase.ReadObjectRange(
                KvpbaseCommon.InputString("Container:", "default", false),
                KvpbaseCommon.InputString("Object Key:", "hello.txt", false),
                KvpbaseCommon.InputInteger("Start Index:", 0, true, true),
                KvpbaseCommon.InputInteger("Count:", 10, true, false),
                out data))
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
                if (data != null && data.Length > 0)
                {
                    Console.WriteLine(Encoding.UTF8.GetString(data));
                }
            }
        }

        private static void ObjectRename()
        { 
            if (!_Kvpbase.RenameObject(
                KvpbaseCommon.InputString("Container:", "default", false),
                KvpbaseCommon.InputString("Original Object Key:", "hello.txt", false),
                KvpbaseCommon.InputString("New Object Key:", "renamed.txt", false)))
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success"); 
            }
        }

        private static void ObjectDelete()
        {
            if (!_Kvpbase.DeleteObject(
                KvpbaseCommon.InputString("Container:", "default", false),
                KvpbaseCommon.InputString("Object Key:", "hello.txt", false)))
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
            }
        }

        private static void ObjectExists()
        {
            if (!_Kvpbase.ObjectExists(
                KvpbaseCommon.InputString("Container:", "default", false),
                KvpbaseCommon.InputString("Object Key:", "hello.txt", false)))
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
            ObjectMetadata metadata = null;
            if (!_Kvpbase.GetObjectMetadata(
                KvpbaseCommon.InputString("Container:", "default", false),
                KvpbaseCommon.InputString("Object Key:", "hello.txt", false),
                out metadata))
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
                Console.WriteLine(KvpbaseCommon.SerializeJson(metadata, true));
            }
        }

        #endregion

        #region Private-Container-Methods

        private static void ContainerList()
        {
            List<ContainerSettings> settings = null;

            if (!_Kvpbase.ListContainers(out settings))
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
                Console.WriteLine(KvpbaseCommon.SerializeJson(settings, true));
            }
        }

        private static void ContainerCreate()
        {
            if (!_Kvpbase.CreateContainer(
                KvpbaseCommon.InputString("Container:", "default", false),
                KvpbaseCommon.InputBoolean("Public Read:", true),
                KvpbaseCommon.InputBoolean("Public Write:", false), 
                KvpbaseCommon.InputBoolean("Audit Logging:", true),
                GetReplicationMode()))
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
            }
        }

        private static void ContainerGetSettings()
        {
            ContainerSettings settings = null;

            if (!_Kvpbase.GetContainerSettings( 
                KvpbaseCommon.InputString("Container:", "default", false),
                out settings))
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
                Console.WriteLine(KvpbaseCommon.SerializeJson(settings, true));
            }
        }

        private static void ContainerUpdate()
        { 
            ContainerSettings settings = null;

            if (!_Kvpbase.GetContainerSettings(
                KvpbaseCommon.InputString("Container:", "default", false),
                out settings))
            {
                Console.WriteLine("Failed");
            }
            else
            {
                settings.IsPublicRead = KvpbaseCommon.InputBoolean("Public Read:", true);
                settings.IsPublicWrite = KvpbaseCommon.InputBoolean("Public Write:", false);
                settings.EnableAuditLogging = KvpbaseCommon.InputBoolean("Audit Logging:", true);
                settings.Replication = GetReplicationMode();

                if (!_Kvpbase.UpdateContainer(settings))
                {
                    Console.WriteLine("Failed");
                }
                else
                {
                    Console.WriteLine("Success");
                }
            }
        }

        private static void ContainerEnumerate()
        {
            ContainerMetadata metadata = null;

            if (!_Kvpbase.EnumerateContainer(
                KvpbaseCommon.InputString("Container:", "default", false),
                KvpbaseCommon.InputInteger("Start Index:", 0, true, true),
                KvpbaseCommon.InputInteger("Count:", 10, true, false),
                out metadata))
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
                Console.WriteLine(KvpbaseCommon.SerializeJson(metadata, true));
            } 
        }

        private static void ContainerDelete()
        {
            if (!_Kvpbase.DeleteContainer(
                   KvpbaseCommon.InputString("Container:", "default", false)))
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Success");
            }
        }

        private static void ContainerExists()
        {
            if (!_Kvpbase.ContainerExists(
                   KvpbaseCommon.InputString("Container:", "default", false)))
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
