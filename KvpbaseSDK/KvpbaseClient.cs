using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using RestWrapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KvpbaseSDK
{
	/// <summary>
	/// The Kvpbase client.
	/// </summary>
	public class KvpbaseClient
    {
        #region Public-Members

        public bool IgnoreCertificateErrors { get; set; }
        public long MaxTransferSize { get; set; }

        #endregion

        #region Private-Members

        private string _UserGuid = null;
        private string _Email = null;
        private string _Password = null;
        private string _ApiKey = null;
        private string _Token = null;
        private string _Endpoint = null;

        private Dictionary<string, string> _AuthHeaders = null;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Initializes a new instance of the Kvpbase client.
        /// </summary>
        /// <param name="userGuid">The GUID of the user.</param>
        /// <param name="apiKey">The API key of the user.</param>
        /// <param name="endpointUrl">The Kvpbase server endpoint (e.g. http://api1.kvpbase.com:8001/, or, https://hostname.com:443/).</param>
        public KvpbaseClient(string apiKey, string endpointUrl)
		{ 
			if (String.IsNullOrEmpty(apiKey)) throw new ArgumentNullException(nameof(apiKey));
			if (String.IsNullOrEmpty(endpointUrl)) throw new ArgumentNullException(nameof(endpointUrl));
             
			_ApiKey = apiKey;
			_Endpoint = AppendSlash(endpointUrl);

            IgnoreCertificateErrors = true;
            MaxTransferSize = 536870912;

            SetAuthHeaders();
		}

        /// <summary>
        /// Initializes a new instance of the Kvpbase client.
        /// </summary>
        /// <param name="userGuid">The GUID of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="endpointUrl">The Kvpbase server endpoint (e.g. http://api1.kvpbase.com:8001/, or, https://hostname.com:443/).</param>
        public KvpbaseClient(string userGuid, string email, string password, string endpointUrl)
        {
            if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));
            if (String.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));
            if (String.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));
            if (String.IsNullOrEmpty(endpointUrl)) throw new ArgumentNullException(nameof(endpointUrl));

            _UserGuid = userGuid;
            _Email = email;
            _Password = password;
            _Endpoint = AppendSlash(endpointUrl);

            IgnoreCertificateErrors = true;
            MaxTransferSize = 536870912;

            SetAuthHeaders();
        }
         
        #endregion

        #region Public-Methods

        #region General

        public bool VerifyConnectivity()
        {
            RestResponse resp = RestRequest.SendRequestSafe(
                _Endpoint,
                null,
                "GET",
                null, null, false, IgnoreCertificateErrors, null, null);

            if (resp != null && resp.StatusCode >= 200 && resp.StatusCode <= 299)
            {
                return true;
            }

            return false;
        }

        public bool Authenticate()
        {
            RestResponse resp = RestRequest.SendRequestSafe(
                _Endpoint + "token",
                null,
                "GET",
                null, null, false, IgnoreCertificateErrors, _AuthHeaders, null);

            if (resp != null && resp.StatusCode >= 200 && resp.StatusCode <= 299)
            {
                if (resp.Data != null && resp.Data.Length > 0)
                {
                    _Token = Encoding.UTF8.GetString(resp.Data);
                }

                return true;
            }
            return false;
        }

        #endregion

        #region Objects

        public bool WriteObject(string userGuid, string container, string objectKey, string contentType, byte[] data)
        {
            if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));
            
            string url = _Endpoint + userGuid + "/" + container + "/" + objectKey;

            RestResponse resp = RestRequest.SendRequestSafe(
                url,
                contentType,
                "POST",
                null, null, false, IgnoreCertificateErrors, _AuthHeaders, data);

            if (resp == null || resp.StatusCode != 201)
            {
                return false;
            }

            return true;
        }

        public bool WriteObjectRange(string userGuid, string container, string objectKey, long startIndex, byte[] data)
        {
            if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));
            if (startIndex < 0) throw new ArgumentException("Invalid value for startIndex.");

            string url = _Endpoint + userGuid + "/" + container + "/" + objectKey + "?_index=" + startIndex;

            RestResponse resp = RestRequest.SendRequestSafe(
                url,
                null,
                "PUT",
                null, null, false, IgnoreCertificateErrors, _AuthHeaders, data);

            if (resp == null || resp.StatusCode != 200)
            {
                return false;
            }

            return true;
        }

        public bool ReadObject(string userGuid, string container, string objectKey, out byte[] data)
        {
            data = null;

            if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));

            string url = _Endpoint + userGuid + "/" + container + "/" + objectKey;

            RestResponse resp = RestRequest.SendRequestSafe(
                url,
                null,
                "GET",
                null, null, false, IgnoreCertificateErrors, _AuthHeaders, null);

            if (resp == null || resp.StatusCode != 200)
            {
                return false;
            }
            
            if (resp.Data != null && resp.Data.Length > 0)
            {
                data = new byte[resp.Data.Length];
                Buffer.BlockCopy(resp.Data, 0, data, 0, resp.Data.Length);
            }

            return true;
        }

        public bool ReadObjectRange(string userGuid, string container, string objectKey, long startIndex, long count, out byte[] data)
        {
            data = null;

            if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));
            if (startIndex < 0) throw new ArgumentException("Invalid value for startIndex.");
            if (count <= 0) throw new ArgumentException("Invalid value for count.");

            string url = _Endpoint + userGuid + "/" + container + "/" + objectKey + "?_index=" + startIndex + "&_count=" + count;

            RestResponse resp = RestRequest.SendRequestSafe(
                url,
                null,
                "GET",
                null, null, false, IgnoreCertificateErrors, _AuthHeaders, null);

            if (resp == null || resp.StatusCode != 200)
            {
                return false;
            }

            if (resp.Data != null && resp.Data.Length > 0)
            {
                data = new byte[resp.Data.Length];
                Buffer.BlockCopy(resp.Data, 0, data, 0, resp.Data.Length);
            }

            return true;
        }

        public bool RenameObject(string userGuid, string container, string originalObjectKey, string newObjectKey)
        {
            if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(originalObjectKey)) throw new ArgumentNullException(nameof(originalObjectKey));
            if (String.IsNullOrEmpty(newObjectKey)) throw new ArgumentNullException(nameof(newObjectKey));

            string url = _Endpoint + userGuid + "/" + container + "/" + originalObjectKey + "?_rename=" + newObjectKey;

            RestResponse resp = RestRequest.SendRequestSafe(
                url,
                null,
                "PUT",
                null, null, false, IgnoreCertificateErrors, _AuthHeaders, null);

            if (resp == null || resp.StatusCode != 200)
            {
                return false;
            }
             
            return true;
        }

        public bool DeleteObject(string userGuid, string container, string objectKey)
        { 
            if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));

            string url = _Endpoint + userGuid + "/" + container + "/" + objectKey;

            RestResponse resp = RestRequest.SendRequestSafe(
                url,
                null,
                "DELETE",
                null, null, false, IgnoreCertificateErrors, _AuthHeaders, null);

            if (resp == null || resp.StatusCode != 204)
            {
                return false;
            }
             
            return true;
        }

        public bool ObjectExists(string userGuid, string container, string objectKey)
        {
            if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));

            string url = _Endpoint + userGuid + "/" + container + "/" + objectKey;

            RestResponse resp = RestRequest.SendRequestSafe(
                url,
                null,
                "HEAD",
                null, null, false, IgnoreCertificateErrors, _AuthHeaders, null);

            if (resp == null || resp.StatusCode != 200)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Containers

        public bool ListContainers(string userGuid, out List<ContainerSettings> settings)
        {
            settings = new List<ContainerSettings>();

            if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));

            string url = _Endpoint + userGuid + "?_container=true&_stats=true";

            RestResponse resp = RestRequest.SendRequestSafe(
                url,
                null,
                "GET",
                null, null, false, IgnoreCertificateErrors, _AuthHeaders, null);

            if (resp == null || resp.StatusCode != 200 || resp.Data == null || resp.Data.Length < 1)
            {
                return false;
            }

            settings = KvpbaseCommon.DeserializeJson<List<ContainerSettings>>(resp.Data);
            return true;
        }

        public bool CreateContainer(string userGuid, string container, bool publicRead, bool publicWrite, bool auditLogging, ReplicationMode replication)
        {
            if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));

            ContainerSettings settings = new ContainerSettings();
            settings.User = userGuid;
            settings.Name = container;
            settings.IsPublicRead = publicRead;
            settings.IsPublicWrite = publicWrite;
            settings.EnableAuditLogging = auditLogging;
            settings.Replication = replication;

            string url = _Endpoint + userGuid + "/" + container + "?_container=true";

            RestResponse resp = RestRequest.SendRequestSafe(
                url,
                null,
                "POST",
                null, null, false, IgnoreCertificateErrors, _AuthHeaders,
                Encoding.UTF8.GetBytes(KvpbaseCommon.SerializeJson(settings, false)));

            if (resp == null || resp.StatusCode != 201)
            {
                return false;
            }

            return true;
        }

        public bool GetContainerSettings(string userGuid, string container, out ContainerSettings settings)
        {
            settings = null;

            if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
             
            string url = _Endpoint + userGuid + "/" + container + "?_container=true&_config=true";

            RestResponse resp = RestRequest.SendRequestSafe(
                url,
                null,
                "GET",
                null, null, false, IgnoreCertificateErrors, _AuthHeaders, null);

            if (resp == null || resp.StatusCode != 200 || resp.Data == null || resp.Data.Length < 1)
            {
                return false;
            }

            settings = KvpbaseCommon.DeserializeJson<ContainerSettings>(resp.Data);
            return true;
        }

        public bool UpdateContainer(ContainerSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            string url = _Endpoint + settings.User + "/" + settings.Name + "?_container=true";

            RestResponse resp = RestRequest.SendRequestSafe(
                url,
                null,
                "PUT",
                null, null, false, IgnoreCertificateErrors, _AuthHeaders, 
                Encoding.UTF8.GetBytes(KvpbaseCommon.SerializeJson(settings, false)));

            if (resp == null || resp.StatusCode != 200)
            {
                return false;
            }

            return true;
        }

        public bool EnumerateContainer(string userGuid, string container, long? startIndex, long? maxResults, out ContainerMetadata metadata)
        {
            metadata = null;

            if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));

            string url = _Endpoint + userGuid + "/" + container + "?_container=true";
            if (startIndex != null) url += "&_index=" + startIndex;
            if (maxResults != null) url += "&_count=" + maxResults;

            RestResponse resp = RestRequest.SendRequestSafe(
                url,
                null,
                "GET",
                null, null, false, IgnoreCertificateErrors, _AuthHeaders, null);

            if (resp == null || resp.StatusCode != 200 || resp.Data == null || resp.Data.Length < 1)
            {
                return false;
            }

            metadata = KvpbaseCommon.DeserializeJson<ContainerMetadata>(resp.Data);
            return true;
        }

        public bool DeleteContainer(string userGuid, string container)
        {
            if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));

            string url = _Endpoint + userGuid + "/" + container + "?_container=true";

            RestResponse resp = RestRequest.SendRequestSafe(
                url,
                null,
                "DELETE",
                null, null, false, IgnoreCertificateErrors, _AuthHeaders, null);

            if (resp == null || resp.StatusCode != 204)
            {
                return false;
            }

            return true;
        }

        public bool ContainerExists(string userGuid, string container)
        {
            if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            
            string url = _Endpoint + userGuid + "/" + container + "?_container=true";

            RestResponse resp = RestRequest.SendRequestSafe(
                url,
                null,
                "HEAD",
                null, null, false, IgnoreCertificateErrors, _AuthHeaders, null);

            if (resp == null || resp.StatusCode != 200)
            {
                return false;
            }

            return true;
        }

        #endregion

        #endregion

        #region Private-Methods

        private void SetAuthHeaders()
        {
            _AuthHeaders = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(_Email) && !String.IsNullOrEmpty(_Password))
            {
                #region Password-Auth

                _AuthHeaders.Add("x-email", _Email);
                _AuthHeaders.Add("x-password", _Password);
                return;

                #endregion
            }
            else if (!String.IsNullOrEmpty(_ApiKey))
            {
                #region Api-Key-Auth

                _AuthHeaders.Add("x-api-key", _ApiKey);
                return;

                #endregion
            }
            else
            {
                throw new Exception("No authentication material configured.");
            } 
        }

        private string AppendSlash(string s)
        {
            if (String.IsNullOrEmpty(s)) return "/";
            if (s.EndsWith("/", StringComparison.InvariantCulture)) return s;
            return s + "/";
        }
         
        #endregion

    }
}

