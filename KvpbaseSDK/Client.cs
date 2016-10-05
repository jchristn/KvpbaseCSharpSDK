using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;

namespace KvpbaseSDK
{
	/// <summary>
	/// The kvpbase client object.  Kvpbase is a RESTful object storage platform.
	/// </summary>
	public class Client
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the kvpbase client.
		/// </summary>
		/// <param name="userGuid">The GUID of the user.</param>
		/// <param name="apiKey">The API key of the user.</param>
		/// <param name="endpointUrl">The kvpbase server endpoint (e.g. http://api1.kvpbase.com/).</param>
		public Client(string userGuid, string apiKey, string endpointUrl)
		{
			if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));
			if (String.IsNullOrEmpty(apiKey)) throw new ArgumentNullException(nameof(apiKey));
			if (String.IsNullOrEmpty(endpointUrl)) throw new ArgumentNullException(nameof(endpointUrl));
			UserGuid = userGuid;
			ApiKey = apiKey;
			EndpointUrl = AppendSlash(endpointUrl);
		}

		#endregion

		#region Public-Members

		#endregion

		#region Public-Methods

		#region Objects

		/// <summary>
		/// Create an object with a system-supplied name.
		/// </summary>
		/// <returns>True if object was created, false otherwise.</returns>
		/// <param name="containerPath">The container path where the object should be written, i.e. /foo/bar/.</param>
		/// <param name="contentType">The content type of the object, i.e. text/plain or application/octet-stream.</param>
		/// <param name="data">The data to be contained in the object.</param>
		/// <param name="url">The URL to access the object after creation.</param>
		public bool CreateObjectWithoutName(
			string containerPath, 
			string contentType, 
			byte[] data, 
			out string url)
		{
			byte[] responseData;
			url = null;

			string tempUrl = EndpointUrl + AppendSlash(UserGuid);
			if (!String.IsNullOrEmpty(containerPath)) tempUrl += AppendSlash(containerPath);

			if (SubmitRestRequest(
				"POST",
				tempUrl, 
				contentType,
				AuthHeaders(), 
				data, 
				out responseData))
			{
				if (responseData != null && responseData.Length > 0)
				{
					url = Encoding.UTF8.GetString(responseData);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Create an object with a specific name.
		/// </summary>
		/// <returns>True if object was created, false otherwise.</returns>
		/// <param name="containerPath">The container path where the object should be written, i.e. /foo/bar/.</param>
		/// <param name="objectName">The name of the object to create, i.e. helloworld.txt.</param>
		/// <param name="contentType">The content type of the object, i.e. text/plain or application/octet-stream.</param>
		/// <param name="data">The data to be contained in the object.</param>
		/// <param name="url">The URL to access the object after creation.</param>
		public bool CreateObjectWithName(
			string containerPath, 
			string objectName, 
			string contentType, 
			byte[] data, 
			out string url)
		{
			byte[] responseData;
			url = null;

			if (String.IsNullOrEmpty(objectName)) throw new ArgumentNullException(nameof(objectName));

			string tempUrl = EndpointUrl + AppendSlash(UserGuid);
			if (!String.IsNullOrEmpty(containerPath)) tempUrl += AppendSlash(containerPath);
			tempUrl += objectName;

			if (SubmitRestRequest(
				"PUT",
				tempUrl,
				contentType,
				AuthHeaders(),
				data,
				out responseData))
			{
				if (responseData != null && responseData.Length > 0)
				{
					url = Encoding.UTF8.GetString(responseData);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Retrieve an object.
		/// </summary>
		/// <returns>True if object was retrieved successfully, false otherwise.</returns>
		/// <param name="objectPath">The container path and object name.</param>
		/// <param name="data">The data contained within the object.</param>
		public bool GetObject(
			string objectPath,
			out byte[] data)
		{
			data = null;

			if (String.IsNullOrEmpty(objectPath)) throw new ArgumentNullException(nameof(objectPath));

			if (SubmitRestRequest(
				"GET",
				EndpointUrl + AppendSlash(UserGuid) + objectPath,
				null,
				AuthHeaders(),
				data,
				out data))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Delete an object.
		/// </summary>
		/// <returns>True if object was deleted successfully, false otherwise.</returns>
		/// <param name="objectPath">The container path and object name.</param>
		public bool DeleteObject(
			string objectPath)
		{
			byte[] responseData;

			if (String.IsNullOrEmpty(objectPath)) throw new ArgumentNullException(nameof(objectPath));

			if (SubmitRestRequest(
				"DELETE",
				EndpointUrl + AppendSlash(UserGuid) + objectPath,
				null,
				AuthHeaders(),
				null,
				out responseData))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Move an object from one container path and name to another container path and name.
		/// </summary>
		/// <returns>True if object was moved successfully, false otherwise.</returns>
		/// <param name="fromContainerPath">The container path where the object currently resides, i.e. /path/to/container/.</param>
		/// <param name="fromObjectName">The current name of the object, i.e. helloworld.txt.</param>
		/// <param name="toContainerPath">The container path where the object should be moved, i.e. /path/to/newcontainer/.</param>
		/// <param name="toObjectName">The new name of the object, i.e. helloworld_new.txt.</param>
		public bool MoveObject(
			string fromContainerPath,
			string fromObjectName,
			string toContainerPath,
			string toObjectName)
		{
			if (String.IsNullOrEmpty(fromObjectName)) throw new ArgumentNullException(nameof(fromObjectName));
			if (String.IsNullOrEmpty(toObjectName)) throw new ArgumentNullException(nameof(toObjectName));

			string[] fromContainers = null;
			string[] toContainers = null;
			if (!String.IsNullOrEmpty(fromContainerPath)) fromContainers = fromContainerPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
			if (!String.IsNullOrEmpty(toContainerPath)) toContainers = toContainerPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

			var body = new
			{
				FromContainer = fromContainers,
				ToContainer = toContainers,
				MoveFrom = fromObjectName,
				MoveTo = toObjectName
			};

			byte[] data;

			if (SubmitRestRequest(
				"POST",
				EndpointUrl + AppendSlash(UserGuid) + "move",
				null,
				AuthHeaders(),
				Encoding.UTF8.GetBytes(SerializeJson(body)),
				out data))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Rename an object from one container path and name to another name within that same container.
		/// </summary>
		/// <returns>True if object was moved successfully, false otherwise.</returns>
		/// <param name="containerPath">The container path where the object currently resides, i.e. /path/to/container/.</param>
		/// <param name="fromObjectName">The current name of the object, i.e. helloworld.txt.</param>
		/// <param name="toObjectName">The new name of the object, i.e. helloworld_new.txt.</param>
		public bool RenameObject(
			string containerPath,
			string fromObjectName,
			string toObjectName)
		{
			if (String.IsNullOrEmpty(fromObjectName)) throw new ArgumentNullException(nameof(fromObjectName));
			if (String.IsNullOrEmpty(toObjectName)) throw new ArgumentNullException(nameof(toObjectName));

			string[] containers = null;
			if (!String.IsNullOrEmpty(containerPath)) containers = containerPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

			var body = new
			{
				ContainerPath = containers,
				RenameFrom = fromObjectName,
				RenameTo = toObjectName
			};

			byte[] data;

			if (SubmitRestRequest(
				"POST",
				EndpointUrl + AppendSlash(UserGuid) + "rename",
				null,
				AuthHeaders(),
				Encoding.UTF8.GetBytes(SerializeJson(body)),
				out data))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Verify whether or not an object exists.
		/// </summary>
		/// <returns>True if object exists, false otherwise.</returns>
		/// <param name="objectPath">The container path and object name.</param>
		public bool ObjectExists(
			string objectPath)
		{
			byte[] data;

			if (SubmitRestRequest(
				"HEAD",
				EndpointUrl + AppendSlash(UserGuid) + objectPath,
				null,
				AuthHeaders(),
				null,
				out data))
			{
				return true;
			}

			return false;
		}

		#endregion

		#region Containers

		/// <summary>
		/// Create a container.
		/// </summary>
		/// <returns>True if object was created, false otherwise.</returns>
		/// <param name="containerPath">The container path where the container should be written, i.e. /foo/bar/newcontainer.</param>
		/// <param name="url">The URL to access the container after creation.</param>
		public bool CreateContainer(
			string containerPath,
			out string url)
		{
			byte[] responseData;
			url = null;

			if (SubmitRestRequest(
				"PUT",
				EndpointUrl + AppendSlash(UserGuid) + AppendSlash(containerPath) + "?container=true",
				null,
				AuthHeaders(),
				null,
				out responseData))
			{
				if (responseData != null && responseData.Length > 0)
				{
					url = Encoding.UTF8.GetString(responseData);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Retrieve a container's metadata.
		/// </summary>
		/// <returns>True if container metadata was retrieved successfully, false otherwise.</returns>
		/// <param name="containerPath">The container path.</param>
		/// <param name="data">The container metadata.</param>
		public bool GetContainer(
			string containerPath,
			out byte[] data)
		{
			data = null;

			if (SubmitRestRequest(
				"GET",
				EndpointUrl + AppendSlash(UserGuid) + AppendSlash(containerPath) + "?container=true",
				null,
				AuthHeaders(),
				null,
				out data))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Delete a container.
		/// </summary>
		/// <returns>True if container was deleted successfully, false otherwise.</returns>
		/// <param name="containerPath">The container path.</param>
		/// <param name="recursive">If true, delete all objects, child containers, and nested objects and child containers from this container.</param>
		public bool DeleteContainer(
			string containerPath, 
			bool recursive)
		{
			byte[] responseData;
			string url = EndpointUrl + AppendSlash(UserGuid) + AppendSlash(containerPath) + "?container=true";
			if (recursive) url += "&recursive=true";

			if (SubmitRestRequest(
				"DELETE",
				url,
				null,
				AuthHeaders(),
				null,
				out responseData))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Move a container from one container path and name to another container path and name.
		/// </summary>
		/// <returns>True if container was moved successfully, false otherwise.</returns>
		/// <param name="fromContainerPath">The container path where the container currently resides, i.e. /path/to/.</param>
		/// <param name="fromContainerName">The current name of the container, i.e. container1.</param>
		/// <param name="toContainerPath">The container path where the container should be moved, i.e. /new/path/to/.</param>
		/// <param name="toContainerName">The new name of the container, i.e. container2.</param>
		public bool MoveContainer(
			string fromContainerPath,
			string fromContainerName,
			string toContainerPath,
			string toContainerName)
		{
			if (String.IsNullOrEmpty(fromContainerName)) throw new ArgumentNullException(nameof(fromContainerName));
			if (String.IsNullOrEmpty(toContainerName)) throw new ArgumentNullException(nameof(toContainerName));

			string[] fromContainers = null;
			string[] toContainers = null;
			if (!String.IsNullOrEmpty(fromContainerPath)) fromContainers = fromContainerPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
			if (!String.IsNullOrEmpty(toContainerPath)) toContainers = toContainerPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

			var body = new
			{
				FromContainer = fromContainers,
				ToContainer = toContainers,
				MoveFrom = fromContainerName,
				MoveTo = toContainerName
			};

			byte[] data;

			if (SubmitRestRequest(
				"POST",
				EndpointUrl + AppendSlash(UserGuid) + "move?container=true",
				null,
				AuthHeaders(),
				Encoding.UTF8.GetBytes(SerializeJson(body)),
				out data))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Rename a container from one container path and name to another name within that same container.
		/// </summary>
		/// <returns>True if container was moved successfully, false otherwise.</returns>
		/// <param name="containerPath">The container path where the container currently resides, i.e. /path/to/.</param>
		/// <param name="fromContainerName">The current name of the container, i.e. container1.</param>
		/// <param name="toContainerName">The new name of the container, i.e. container2.</param>
		public bool RenameContainer(
			string containerPath,
			string fromContainerName,
			string toContainerName)
		{
			if (String.IsNullOrEmpty(fromContainerName)) throw new ArgumentNullException(nameof(fromContainerName));
			if (String.IsNullOrEmpty(toContainerName)) throw new ArgumentNullException(nameof(toContainerName));

			string[] containers = null;
			if (!String.IsNullOrEmpty(containerPath)) containers = containerPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

			var body = new
			{
				ContainerPath = containers,
				RenameFrom = fromContainerName,
				RenameTo = toContainerName
			};

			byte[] data;

			if (SubmitRestRequest(
				"POST",
				EndpointUrl + AppendSlash(UserGuid) + "rename?container=true",
				null,
				AuthHeaders(),
				Encoding.UTF8.GetBytes(SerializeJson(body)),
				out data))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Verify whether or not an container exists.
		/// </summary>
		/// <returns>True if container exists, false otherwise.</returns>
		/// <param name="containerPath">The container path.</param>
		public bool ContainerExists(
			string containerPath)
		{
			byte[] data;

			if (SubmitRestRequest(
				"HEAD",
				EndpointUrl + AppendSlash(UserGuid) + AppendSlash(containerPath) + "?container=true",
				null,
				AuthHeaders(),
				null,
				out data))
			{
				return true;
			}

			return false;
		}

        #endregion

        #endregion

        #region Private-Members

        private string UserGuid;
        private string ApiKey;
        private string EndpointUrl;

        #endregion

        #region Private-Methods

        private Dictionary<string, string> AuthHeaders()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            ret.Add("x-api-key", ApiKey);
            return ret;
        }

        private string AppendSlash(
            string s)
        {
            if (String.IsNullOrEmpty(s)) return "/";
            if (s.EndsWith("/", StringComparison.InvariantCulture)) return s;
            return s + "/";
        }

        private bool SubmitRestRequest(
            string verb,
            string url,
            string contentType,
            Dictionary<string, string> headers,
            byte[] data,
            out byte[] responseData)
        {
            responseData = null;
            if (String.IsNullOrEmpty(verb)) throw new ArgumentNullException(nameof(verb));
            if (String.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));

            RestResponse resp = RestRequest.SendRequestSafe(
                url,
                contentType,
                verb,
                null, null, false, headers,
                data);

            if (resp == null) return false;
            if (resp.StatusCode != 200 && resp.StatusCode != 201) return false;
            responseData = resp.Data;
            return true;
        }

        private string SerializeJson(
            object obj)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = Int32.MaxValue;
            string json = s.Serialize(obj);
            return json;
        }

        #endregion

    }
}

