using System;
using System.Diagnostics;
using System.Text;
using KvpbaseSDK;

namespace KvpbaseSDKTest
{
	public class MainClass
	{
		static string endpoint = "";
		static string userGuid = "";
		static string apiKey = "";

		public static void Main(string[] args)
		{
			#region Initialize

			endpoint = UserInputString("Endpoint", "http://localhost:8080", false);
			userGuid = UserInputString("GUID", "default", false);
			apiKey = UserInputString("API Key", "default", false);
			Client kvp = new Client(userGuid, apiKey, endpoint);

			#endregion

			#region Variables

			bool runForever = true;
			string userInput = "";
			string url = "";
			byte[] data;

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

					case "ocreate":
						if (kvp.CreateObjectWithName(
							UserInputString("Container path", null, true),
							UserInputString("Object name", null, false), 
							UserInputString("Content type", "text/plain", true), 
							Encoding.UTF8.GetBytes(UserInputString("Data", "Hello, world!", false)), 
							out url))
						{
							Console.WriteLine("Success: " + url);
						}
						else
						{
							Console.WriteLine("Failed");
						}
						break;

					case "ocreatenoname":
						if (kvp.CreateObjectWithoutName(
							UserInputString("Container path", null, true),
							UserInputString("Content type", "text/plain", true),
							Encoding.UTF8.GetBytes(UserInputString("Data", "Hello, world!", false)),
							out url))
						{
							Console.WriteLine("Success: " + url);
						}
						else
						{
							Console.WriteLine("Failed");
						}
						break;
						
					case "oget":
						if (kvp.GetObject(
							UserInputString("Object path", null, true),
							out data))
						{
							Console.WriteLine("Success: " + Encoding.UTF8.GetString(data));
						}
						else
						{
							Console.WriteLine("Failed");
						}
						break;

					case "odel":
						if (kvp.DeleteObject(
							UserInputString("Object path", null, true)))
						{
							Console.WriteLine("Success");
						}
						else
						{
							Console.WriteLine("Failed");
						}
						break;

					case "oexists":
						if (kvp.ObjectExists(
							UserInputString("Object path", null, true)))
						{
							Console.WriteLine("Success");
						}
						else
						{
							Console.WriteLine("Failed");
						}
						break;
						
					case "orn":
						if (kvp.RenameObject(
							   UserInputString("Container path", null, true), 
							   UserInputString("From object name", null, false),
								UserInputString("To object name", null, false)))
						{
							Console.WriteLine("Success");
						}
						else
						{
							Console.WriteLine("Failed");
						}
						break;

					case "omv":
						if (kvp.MoveObject(
							   UserInputString("From container path", null, true),
							   UserInputString("From object name", null, false), 
							   UserInputString("To container path", null, true),
							   UserInputString("To object name", null, false)))
						{
							Console.WriteLine("Success");
						}
						else
						{
							Console.WriteLine("Failed");
						}
						break;

					case "ccreate":
						if (kvp.CreateContainer(
							UserInputString("Container path", null, true),
							out url))
						{
							Console.WriteLine("Success: " + url);
						}
						else
						{
							Console.WriteLine("Failed");
						}
						break;

					case "cget":
						if (kvp.GetContainer(
							UserInputString("Container path", null, true),
							out data))
						{
							Console.WriteLine("Success: " + Encoding.UTF8.GetString(data));
						}
						else
						{
							Console.WriteLine("Failed");
						}
						break;

					case "cdel":
						if (kvp.DeleteContainer(
						    UserInputString("Container path", null, true),
						    UserInputBool("Recursive", true)))
						{
							Console.WriteLine("Success");
						}
						else
						{
							Console.WriteLine("Failed");
						}
						break;
						
					case "crn":
						if (kvp.RenameContainer(
							UserInputString("Container path", null, true),
							UserInputString("From container name", null, false),
							UserInputString("To container name", null, false)))
						{
							Console.WriteLine("Success");
						}
						else
						{
							Console.WriteLine("Failed");
						}
						break;

					case "cmv":
						if (kvp.MoveContainer(
							   UserInputString("From container path", null, true),
							   UserInputString("From container name", null, false),
							   UserInputString("To container path", null, true),
							   UserInputString("To container name", null, false)))
						{
							Console.WriteLine("Success");
						}
						else
						{
							Console.WriteLine("Failed");
						}
						break;
						
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
			Console.WriteLine(" q               Quit");
			Console.WriteLine(" cls             Clear the screen");
			Console.WriteLine(" ?               Help (this menu)");
			Console.WriteLine("");
			Console.WriteLine("Object commands:");
			Console.WriteLine(" ocreate         Create an object");
			Console.WriteLine(" ocreatenoname   Create an object (system-supplied name)");
			Console.WriteLine(" oget            Get an object");
			Console.WriteLine(" odel            Delete an object");
			Console.WriteLine(" oexists         Check if an object exists");
			Console.WriteLine(" orn             Rename an object");
			Console.WriteLine(" omv             Move an object");
			Console.WriteLine("");
			Console.WriteLine("Container commands:");
			Console.WriteLine(" ccreate         Create a container");
			Console.WriteLine(" cget            Get a container");
			Console.WriteLine(" cdel            Delete a container");
			Console.WriteLine(" cexists         Check if a container exists");
			Console.WriteLine(" crn             Rename a container");
			Console.WriteLine(" cmv             Move a container");
			Console.WriteLine("");
		}

		private static string StackToString()
		{
			string ret = "";

			StackTrace t = new StackTrace();
			for (int i = 0; i < t.FrameCount; i++)
			{
				if (i == 0)
				{
					ret += t.GetFrame(i).GetMethod().Name;
				}
				else
				{
					ret += " <= " + t.GetFrame(i).GetMethod().Name;
				}
			}

			return ret;
		}

		private static void ExceptionConsole(string method, string text, Exception e)
		{
			var st = new StackTrace(e, true);
			var frame = st.GetFrame(0);
			int line = frame.GetFileLineNumber();
			string filename = frame.GetFileName();

			Console.WriteLine("---");
			Console.WriteLine("An exception was encountered which triggered this message.");
			Console.WriteLine("  Method: " + method);
			Console.WriteLine("  Text: " + text);
			Console.WriteLine("  Type: " + e.GetType().ToString());
			Console.WriteLine("  Data: " + e.Data);
			Console.WriteLine("  Inner: " + e.InnerException);
			Console.WriteLine("  Message: " + e.Message);
			Console.WriteLine("  Source: " + e.Source);
			Console.WriteLine("  StackTrace: " + e.StackTrace);
			Console.WriteLine("  Stack: " + StackToString());
			Console.WriteLine("  Line: " + line);
			Console.WriteLine("  File: " + filename);
			Console.WriteLine("  ToString: " + e.ToString());
			Console.WriteLine("---");

			return;
		}

		private static bool UserInputBool(string question, bool def)
		{
			while (true)
			{
				Console.Write(question);
				if (def) Console.Write(" [true]");
				else Console.Write(" [false]");
				Console.Write(" ");

				bool ret = false;
				string userInput = Console.ReadLine();
				if (String.IsNullOrEmpty(userInput)) return def;

				if (Boolean.TryParse(userInput, out ret))
				{
					return ret;
				}
			}	
		}

	    private static string UserInputString(string question, string defaultAnswer, bool allowNull)
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

		private static int UserInputInt(string question, int defaultAnswer, bool positiveOnly, bool allowZero)
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
	}
}
