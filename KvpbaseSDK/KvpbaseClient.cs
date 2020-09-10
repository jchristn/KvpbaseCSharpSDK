using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestWrapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KvpbaseSDK
{
	/// <summary>
	/// Kvpbase object storage client.
	/// </summary>
	public class KvpbaseClient
    {
        #region Public-Members

        /// <summary>
        /// Specify whether or not to ignore SSL certificate errors (default is true).
        /// </summary>
        public bool IgnoreCertificateErrors { get; set; }

        /// <summary>
        /// Specify the maximum transfer size in bytes (default is 536870912).
        /// </summary>
        public long MaxTransferSize
        {
            get
            {
                return _MaxTransferSize;
            }
            set
            {
                if (value < 1) throw new ArgumentException("MaxTransferSize must be greater than zero.");
                _MaxTransferSize = value;
            }
        }

        /// <summary>
        /// Buffer size to use when uploading files for file APIs or using stream, default 1MB.
        /// </summary>
        public int UploadStreamBufferSize
        {
            get
            {
                return _UploadStreamBufferSize;
            }
            set
            {
                if (value < 1) throw new ArgumentException("UploadStreamBufferSize must be greater than zero.");
                _UploadStreamBufferSize = value;
            }
        }

        /// <summary>
        /// Buffer size to use when downloading files for file APIs or using stream, default 1MB.
        /// </summary>
        public int DownloadStreamBufferSize
        {
            get
            {
                return _DownloadStreamBufferSize;
            }
            set
            {
                if (value < 1) throw new ArgumentException("DownloadStreamBufferSize must be greater than zero.");
                _DownloadStreamBufferSize = value;
            }
        }

        /// <summary>
        /// Retrieve the user GUID for the client.
        /// </summary>
        public string UserGuid
        {
            get { return _UserGuid; }
            private set { _UserGuid = value; }
        }

        /// <summary>
        /// Retrieve the endpoint for the client.
        /// </summary>
        public string Endpoint
        {
            get { return _Endpoint; }
            private set { _Endpoint = value; }
        }

        #endregion

        #region Private-Members

        private string _UserGuid = null;
        private string _Email = null;
        private string _Password = null;
        private string _ApiKey = null; 
        private string _Endpoint = null;

        private Dictionary<string, string> _AuthHeaders = null;

        private long _MaxTransferSize = 536870912;
        private int _UploadStreamBufferSize = (1024 * 1024);
        private int _DownloadStreamBufferSize = (1024 * 1024);

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Initializes a new instance of the Kvpbase client.
        /// </summary>
        /// <param name="userGuid">The GUID of the user.</param>
        /// <param name="email">Email address of the user.</param>
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
            UploadStreamBufferSize = 1048576;
            DownloadStreamBufferSize = 1048576;

            SetAuthHeaders();
        }

        /// <summary>
        /// Initializes a new instance of the Kvpbase client.
        /// </summary>
        /// <param name="userGuid">The GUID of the user.</param>
        /// <param name="apiKey">The API key of the user.</param>
        /// <param name="endpointUrl">The Kvpbase server endpoint (e.g. http://api1.kvpbase.com:8001/, or, https://hostname.com:443/).</param>
        public KvpbaseClient(string userGuid, string apiKey, string endpointUrl)
        {
            if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));
            if (String.IsNullOrEmpty(apiKey)) throw new ArgumentNullException(nameof(apiKey));
            if (String.IsNullOrEmpty(endpointUrl)) throw new ArgumentNullException(nameof(endpointUrl));

            _UserGuid = userGuid;
            _ApiKey = apiKey;
            _Endpoint = AppendSlash(endpointUrl);

            IgnoreCertificateErrors = true;
            MaxTransferSize = 536870912;
            UploadStreamBufferSize = 1048576;
            DownloadStreamBufferSize = 1048576;

            SetAuthHeaders();
        }

        #endregion

        #region Public-Methods

        #region General

        /// <summary>
        /// Verify connectivity to Kvpbase.
        /// </summary>
        /// <returns>True if connectivity exists.</returns>
        public async Task<bool> VerifyConnectivity()
        {
            RestRequest req = new RestRequest(_Endpoint, HttpMethod.GET, null, null);
            req.IgnoreCertificateErrors = IgnoreCertificateErrors; 
            RestResponse resp = await req.SendAsync();

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return true;
        }
         
        #endregion
          
        #region Containers

        /// <summary>
        /// List the names of the existing containers.
        /// </summary>  
        /// <returns>List of container names.</returns>
        public async Task<List<string>> ListContainers()
        { 
            string url = _Endpoint + _UserGuid;

            RestRequest req = new RestRequest(
                url,
                HttpMethod.GET,
                _AuthHeaders,
                null);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync();

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            if (resp.Data != null && resp.ContentLength > 0)
            {
                byte[] data = KvpbaseCommon.StreamToBytes(resp.Data);
                return KvpbaseCommon.DeserializeJson<List<string>>(data);
            }

            return new List<string>();
        }

        /// <summary>
        /// Create a container.
        /// </summary> 
        /// <param name="container">Container name.</param>
        /// <param name="publicRead">True if available for read by unauthenticated users.</param>
        /// <param name="publicWrite">True if available for write by unauthenticated users.</param>
        /// <param name="auditLogging">True if audit logging should be enabled.</param>  
        public async Task CreateContainer(string container, bool publicRead, bool publicWrite, bool auditLogging)
        {
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));

            Container settings = new Container();
            settings.UserGUID = _UserGuid;
            settings.Name = container;
            settings.IsPublicRead = publicRead;
            settings.IsPublicWrite = publicWrite;
            settings.EnableAuditLogging = auditLogging;
            
            string url = _Endpoint + _UserGuid + "/" + container;

            RestRequest req = new RestRequest(
                url,
                HttpMethod.POST,
                _AuthHeaders,
                "application/json");

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;
             
            RestResponse resp = await req.SendAsync(KvpbaseCommon.SerializeJson(settings, false));

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return;
        }

        /// <summary>
        /// Retrieve a container's settings.
        /// </summary> 
        /// <param name="container">Container name.</param> 
        /// <returns>Container settings.</returns>
        public async Task<Container> GetContainerSettings(string container)
        { 
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));

            string url = _Endpoint + _UserGuid + "/" + container + "?config";

            RestRequest req = new RestRequest(
                url,
                HttpMethod.GET,
                _AuthHeaders,
                null);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync();

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            byte[] data = null;

            if (resp.Data != null && resp.ContentLength > 0)
            {
                data = KvpbaseCommon.StreamToBytes(resp.Data);
                return KvpbaseCommon.DeserializeJson<Container>(data);
            }

            return null;
        }
         
        /// <summary>
        /// Retrieve key-value pairs applied to a container.
        /// </summary>
        /// <param name="container">Container name.</param> 
        /// <returns>Container key-value pairs.</returns>
        public async Task<Dictionary<string, string>> GetContainerKeyValuePairs(string container)
        {
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));

            string url = _Endpoint + _UserGuid + "/" + container + "?keys";

            RestRequest req = new RestRequest(
                url,
                HttpMethod.GET,
                _AuthHeaders,
                null);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync();

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            byte[] data = null;

            if (resp.Data != null && resp.ContentLength > 0)
            {
                data = KvpbaseCommon.StreamToBytes(resp.Data); 
                return KvpbaseCommon.DeserializeJson<Dictionary<string, string>>(data);
            }

            return null;
        }

        /// <summary>
        /// Update a container's settings.
        /// </summary>
        /// <param name="settings">Settings for the container.</param>
        /// <returns>True if successful.</returns>
        public async Task UpdateContainer(Container settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            string url = _Endpoint + _UserGuid + "/" + settings.Name;

            RestRequest req = new RestRequest(
                url,
                HttpMethod.PUT,
                _AuthHeaders,
                "application/json");

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync(KvpbaseCommon.SerializeJson(settings, false));

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return;
        }
         
        /// <summary>
        /// Write key-value pairs to a container.
        /// </summary>
        /// <param name="container">Container name.</param>
        /// <param name="keyValuePairs">Key-value pairs.</param>
        /// <returns>True if successful.</returns>
        public async Task WriteContainerKeyValuePairs(string container, Dictionary<string, string> keyValuePairs)
        {
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));

            string url = _Endpoint + _UserGuid + "/" + container + "?keys";

            RestRequest req = new RestRequest(
                url,
                HttpMethod.PUT,
                _AuthHeaders,
                "application/json");

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = null;
            
            if (keyValuePairs != null)
            {
                resp = await req.SendAsync(KvpbaseCommon.SerializeJson(keyValuePairs, true));
            }
            else
            {
                resp = await req.SendAsync();
            }

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return;
        }

        /// <summary>
        /// Enumerate container statistics and objects within the container.
        /// </summary> 
        /// <param name="container">Container name.</param>
        /// <param name="startIndex">Begin object enumeration from this position.</param>
        /// <param name="maxResults">Maximum number of objects to return.</param> 
        /// <returns>Container metadata.</returns>
        public async Task<ContainerMetadata> EnumerateContainer(string container, long? startIndex, long? maxResults)
        { 
            return await EnumerateContainerInternal(null, container, startIndex, maxResults); 
        }
        
        /// <summary>
        /// Enumerate container statistics and objects within the container using a filter.
        /// </summary>
        /// <param name="filter">Enumeration filter.</param>
        /// <param name="container">Container name.</param>
        /// <param name="startIndex">Begin object enumeration from this position.</param>
        /// <param name="maxResults">Maximum number of objects to return.</param> 
        /// <returns>Container metadata.</returns>
        public async Task<ContainerMetadata> EnumerateContainer(EnumerationFilter filter, string container, long? startIndex, long? maxResults)
        {
            return await EnumerateContainerInternal(filter, container, startIndex, maxResults);
        }

        /// <summary>
        /// Delete a container.
        /// </summary> 
        /// <param name="container">Container name.</param>
        /// <returns>True if successful.</returns>
        public async Task DeleteContainer(string container)
        {
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));

            string url = _Endpoint + _UserGuid + "/" + container;

            RestRequest req = new RestRequest(
                url,
                HttpMethod.DELETE,
                _AuthHeaders,
                null);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync();

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return;
        }

        /// <summary>
        /// Check if a container exists.
        /// </summary> 
        /// <param name="container">Container name.</param>
        /// <returns>True if the container exists.</returns>
        public async Task<bool> ContainerExists(string container)
        {
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));

            string url = _Endpoint + _UserGuid + "/" + container;

            RestRequest req = new RestRequest(
                url,
                HttpMethod.HEAD,
                _AuthHeaders,
                null);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync();
            if (resp != null && resp.StatusCode == 404) return false;

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return true;
        }

        #endregion

        #region Objects

        /// <summary>
        /// Write an object.
        /// </summary>
        /// <param name="container">Container name.</param>
        /// <param name="objectKey">Object key.</param>
        /// <param name="contentType">The content type for the object.</param>
        /// <param name="data">The data to write.</param>
        /// <returns>True if successful.</returns>
        public async Task WriteObject(string container, string objectKey, string contentType, byte[] data)
        { 
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));
            
            string url = _Endpoint + _UserGuid + "/" + container + "/" + objectKey;

            RestRequest req = new RestRequest(
                url,
                HttpMethod.POST,
                _AuthHeaders,
                contentType);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync(data);

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return;
        }

        /// <summary>
        /// Write an object.
        /// </summary>
        /// <param name="container">Container name.</param>
        /// <param name="objectKey">Object key.</param>
        /// <param name="contentType">The content type for the object.</param>
        /// <param name="contentLength">The length of the data in the stream.</param>
        /// <param name="stream">The stream containing the data.</param>
        /// <returns>True if successful.</returns>
        public async Task WriteObject(string container, string objectKey, string contentType, long contentLength, Stream stream)
        {
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));

            string url = _Endpoint + _UserGuid + "/" + container + "/" + objectKey;

            RestRequest req = new RestRequest(
                url,
                HttpMethod.POST,
                _AuthHeaders,
                contentType);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync(contentLength, stream);

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return;
        }

        /// <summary>
        /// Write a range of bytes to an existing object.
        /// </summary>
        /// <param name="container">Container name.</param>
        /// <param name="objectKey">Object key.</param>
        /// <param name="startIndex">The byte position at which to write the data.</param>
        /// <param name="data">The data to write.</param>
        /// <returns>True if successful.</returns>
        public async Task WriteObjectRange(string container, string objectKey, long startIndex, byte[] data)
        { 
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));
            if (startIndex < 0) throw new ArgumentException("Invalid value for startIndex.");

            string url = _Endpoint + _UserGuid + "/" + container + "/" + objectKey + "?index=" + startIndex;

            RestRequest req = new RestRequest(
                url,
                HttpMethod.PUT,
                _AuthHeaders,
                null);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync(data);

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return;
        }

        /// <summary>
        /// Write a range of bytes to an existing object.
        /// </summary>
        /// <param name="container">Container name.</param>
        /// <param name="objectKey">Object key.</param>
        /// <param name="startIndex">The byte position at which to write the data.</param>
        /// <param name="contentLength">The length of the data in the stream.</param>
        /// <param name="stream">The stream containing the data.</param>
        /// <returns>True if successful.</returns>
        public async Task WriteObjectRange(string container, string objectKey, long startIndex, long contentLength, Stream stream)
        {
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));
            if (startIndex < 0) throw new ArgumentException("Invalid value for startIndex.");

            string url = _Endpoint + _UserGuid + "/" + container + "/" + objectKey + "?index=" + startIndex;

            RestRequest req = new RestRequest(
                url,
                HttpMethod.PUT,
                _AuthHeaders,
                null);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync(contentLength, stream);

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return;
        }

        /// <summary>
        /// Write tags to an object.
        /// </summary>
        /// <param name="container">Container name.</param>
        /// <param name="objectKey">Object key.</param>
        /// <param name="tags">Tags.</param>
        /// <returns>True if successful.</returns>
        public async Task WriteObjectTags(string container, string objectKey, List<string> tags)
        {
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));

            string url = _Endpoint + _UserGuid + "/" + container + "/" + objectKey + "?tags=" + KvpbaseCommon.StringListToCsv(tags);

            RestRequest req = new RestRequest(
                url,
                HttpMethod.PUT,
                _AuthHeaders,
                null);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync();

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return;
        }

        /// <summary>
        /// Write key-value pairs to an object.
        /// </summary>
        /// <param name="container">Container name.</param>
        /// <param name="objectKey">Object key.</param>
        /// <param name="keyValuePairs">Key-value pairs.</param>
        /// <returns>True if successful.</returns>
        public async Task WriteObjectKeyValuePairs(string container, string objectKey, Dictionary<string, string> keyValuePairs)
        {
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));

            string url = _Endpoint + _UserGuid + "/" + container + "/" + objectKey + "?keys";

            RestRequest req = new RestRequest(
                url,
                HttpMethod.PUT,
                _AuthHeaders,
                null);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = null;

            if (keyValuePairs == null || keyValuePairs.Count < 1)
            {
                resp = await req.SendAsync();
            }
            else
            {
                resp = await req.SendAsync(KvpbaseCommon.SerializeJson(keyValuePairs, true));
            }

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return;
        }

        /// <summary>
        /// Retrieve an object's metadata.
        /// </summary>
        /// <param name="container">Container name.</param>
        /// <param name="objectKey">Object key.</param> 
        /// <returns>Object metadata.</returns>
        public async Task<ObjectMetadata> ReadObjectMetadata(string container, string objectKey)
        { 
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));

            string url = _Endpoint + _UserGuid + "/" + container + "/" + objectKey + "?metadata";

            RestRequest req = new RestRequest(
                url,
                HttpMethod.GET,
                _AuthHeaders,
                null);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync();

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            if (resp.Data != null && resp.ContentLength > 0)
            {
                return KvpbaseCommon.DeserializeJson<ObjectMetadata>(KvpbaseCommon.StreamToBytes(resp.Data));
            }

            return null;
        }
         
        /// <summary>
        /// Read an object.
        /// </summary>
        /// <param name="container">Container name.</param>
        /// <param name="objectKey">Object key.</param> 
        /// <returns>KvpbaseObject.</returns>
        public async Task<KvpbaseObject> ReadObject(string container, string objectKey)
        { 
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));

            string url = _Endpoint + _UserGuid + "/" + container + "/" + objectKey;

            RestRequest req = new RestRequest(
                url,
                HttpMethod.GET,
                _AuthHeaders,
                null);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync();

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return KvpbaseObject.FromRestResponse(resp);
        }
         
        /// <summary>
        /// Read a range of bytes from an object.
        /// </summary>
        /// <param name="container">Container name.</param>
        /// <param name="objectKey">Object key.</param>
        /// <param name="startIndex">The byte position from which to read the data.</param>
        /// <param name="count">The number of bytes to read.</param> 
        /// <returns>KvpbaseObject.</returns>
        public async Task<KvpbaseObject> ReadObjectRange(string container, string objectKey, long startIndex, long count)
        { 
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));
            if (startIndex < 0) throw new ArgumentException("Invalid value for startIndex.");
            if (count <= 0) throw new ArgumentException("Invalid value for count.");

            string url = _Endpoint + _UserGuid + "/" + container + "/" + objectKey + "?index=" + startIndex + "&count=" + count;

            RestRequest req = new RestRequest(
                url,
                HttpMethod.GET,
                _AuthHeaders,
                null);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync();

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return KvpbaseObject.FromRestResponse(resp);
        }
        
        /// <summary>
        /// Retrieve key-value pairs from an object.
        /// </summary>
        /// <param name="container">Container name.</param>
        /// <param name="objectKey">Object key.</param> 
        /// <returns>Object key-value pairs.</returns>
        public async Task<Dictionary<string, string>> ReadObjectKeyValuePairs(string container, string objectKey)
        { 
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));

            string url = _Endpoint + _UserGuid + "/" + container + "/" + objectKey + "?keys";

            RestRequest req = new RestRequest(
                url,
                HttpMethod.GET,
                _AuthHeaders,
                null);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync();

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;
             
            if (resp.Data != null && resp.ContentLength > 0)
            {
                return KvpbaseCommon.DeserializeJson<Dictionary<string, string>>(KvpbaseCommon.StreamToBytes(resp.Data));
            }

            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Rename an object.
        /// </summary>
        /// <param name="container">Container name.</param>
        /// <param name="originalObjectKey">The original object key.</param>
        /// <param name="newObjectKey">The desired object key.</param>
        /// <returns>True if successful.</returns>
        public async Task RenameObject(string container, string originalObjectKey, string newObjectKey)
        { 
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(originalObjectKey)) throw new ArgumentNullException(nameof(originalObjectKey));
            if (String.IsNullOrEmpty(newObjectKey)) throw new ArgumentNullException(nameof(newObjectKey));

            string url = _Endpoint + _UserGuid + "/" + container + "/" + originalObjectKey + "?rename=" + newObjectKey;

            RestRequest req = new RestRequest(
                url,
                HttpMethod.PUT,
                _AuthHeaders,
                null);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync();

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return;
        }

        /// <summary>
        /// Delete an object.
        /// </summary>
        /// <param name="container">Container name.</param>
        /// <param name="objectKey">Object key.</param>
        /// <returns>True if successful.</returns>
        public async Task DeleteObject(string container, string objectKey)
        {  
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));

            string url = _Endpoint + _UserGuid + "/" + container + "/" + objectKey;

            RestRequest req = new RestRequest(
                url,
                HttpMethod.DELETE,
                _AuthHeaders,
                null);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync();

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return;
        }

        /// <summary>
        /// Check if an object exists.
        /// </summary>
        /// <param name="container">Container name.</param>
        /// <param name="objectKey">Object key.</param>
        /// <returns>True if the object exists.</returns>
        public async Task<bool> ObjectExists(string container, string objectKey)
        { 
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));

            string url = _Endpoint + _UserGuid + "/" + container + "/" + objectKey;

            RestRequest req = new RestRequest(
                url,
                HttpMethod.HEAD,
                _AuthHeaders,
                null);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync();
            if (resp != null && resp.StatusCode == 404) return false;

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return true;
        }

        /// <summary>
        /// Upload from a file to an object.
        /// </summary>
        /// <param name="filename">The filename of the file to upload.</param>
        /// <param name="container">Container name.</param>
        /// <param name="objectKey">Object key.</param>
        /// <param name="contentType">The content type.</param> 
        /// <returns>Object metadata.</returns>
        public async Task<ObjectMetadata> UploadFile(string filename, string container, string objectKey, string contentType)
        { 
            if (String.IsNullOrEmpty(filename)) throw new ArgumentNullException(nameof(filename));
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));

            bool containerExists = await ContainerExists(container);
            if (!containerExists) throw new IOException("Container does not exist.");

            if (!File.Exists(filename)) throw new IOException("File specified does not exist.");

            bool objectExists = await ObjectExists(container, objectKey);
            if (objectExists) throw new IOException("Object specified already exists.");

            string url = _Endpoint + _UserGuid + "/" + container + "/" + objectKey;

            long fileLength = new FileInfo(filename).Length;
            RestResponse resp = null;

            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                RestRequest req = new RestRequest(
                    url,
                    HttpMethod.POST,
                    _AuthHeaders,
                    contentType);

                req.IgnoreCertificateErrors = IgnoreCertificateErrors;

                resp = await req.SendAsync(fileLength, fs);
            }

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return await ReadObjectMetadata(container, objectKey);
        }

        /// <summary>
        /// Upload from a stream to an object.
        /// </summary>
        /// <param name="stream">The input stream from which to read.</param>
        /// <param name="contentLength">Number of bytes to read from the input stream.</param>
        /// <param name="container">Container name.</param>
        /// <param name="objectKey">Object key.</param>
        /// <param name="contentType">The content type.</param> 
        /// <returns>Object metadata.</returns>
        public async Task<ObjectMetadata> UploadFromStream(Stream stream, long contentLength, string container, string objectKey, string contentType)
        { 
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead) throw new ArgumentException("Stream cannot be read.");
            if (contentLength < 0) throw new ArgumentException("Content length must be zero or greater.");
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));

            bool containerExists = await ContainerExists(container);
            if (!containerExists) throw new IOException("Container does not exist.");

            bool objectExists = await ObjectExists(container, objectKey);
            if (objectExists) throw new IOException("Object specified already exists.");

            string url = _Endpoint + _UserGuid + "/" + container + "/" + objectKey;

            RestResponse resp = null;
             
            RestRequest req = new RestRequest(
                url,
                HttpMethod.POST,
                _AuthHeaders,
                contentType);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            resp = await req.SendAsync(contentLength, stream);

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return await ReadObjectMetadata(container, objectKey);
        }

        /// <summary>
        /// Download an object to a file.
        /// </summary>
        /// <param name="filename">The filename of the file to upload.</param>
        /// <param name="container">Container name.</param>
        /// <param name="objectKey">Object key.</param>
        /// <returns>True if successful.</returns>
        public async Task DownloadFile(string filename, string container, string objectKey)
        {
            if (String.IsNullOrEmpty(filename)) throw new ArgumentNullException(nameof(filename));
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));

            bool containerExists = await ContainerExists(container);
            if (!containerExists) throw new IOException("Container does not exist.");

            if (File.Exists(filename)) throw new IOException("File specified already exists.");

            bool objectExists = await ObjectExists(container, objectKey);
            if (!objectExists) throw new IOException("Object specified does not exist.");

            ObjectMetadata metadata = await ReadObjectMetadata(container, objectKey);
            if (metadata == null) throw new IOException("Unable to retrieve object metadata.");

            string url = _Endpoint + _UserGuid + "/" + container + "/" + objectKey;
             
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                RestRequest req = new RestRequest(
                    url,
                    HttpMethod.GET,
                    _AuthHeaders,
                    null);

                req.IgnoreCertificateErrors = IgnoreCertificateErrors;

                RestResponse resp = req.Send();

                KvpbaseException e = KvpbaseException.FromRestResponse(resp);
                if (e != null) throw e;
                 
                long bytesRemaining = resp.ContentLength;
                byte[] buffer = new byte[DownloadStreamBufferSize];

                if (bytesRemaining > 0)
                {
                    while (bytesRemaining > 0)
                    {
                        int bytesRead = resp.Data.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            bytesRemaining -= bytesRead;
                            fs.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Download an object to a stream.
        /// </summary> 
        /// <param name="container">Container name.</param>
        /// <param name="objectKey">Object key.</param> 
        /// <returns>True if successful.</returns>
        public async Task<KvpbaseObject> DownloadToStream(string container, string objectKey)
        {               
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));
            if (String.IsNullOrEmpty(objectKey)) throw new ArgumentNullException(nameof(objectKey));

            bool containerExists = await ContainerExists(container);
            if (!containerExists) throw new IOException("Container does not exist.");

            bool objectExists = await ObjectExists(container, objectKey);
            if (!objectExists) throw new IOException("Object specified does not exist.");

            ObjectMetadata metadata = await ReadObjectMetadata(container, objectKey);
            if (metadata == null) throw new IOException("Unable to retrieve object metadata.");

            string url = _Endpoint + _UserGuid + "/" + container + "/" + objectKey;
             
            RestRequest req = new RestRequest(
                url,
                HttpMethod.GET,
                _AuthHeaders,
                null);

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            RestResponse resp = await req.SendAsync();

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return KvpbaseObject.FromRestResponse(resp);
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
          
        private async Task<ContainerMetadata> EnumerateContainerInternal(EnumerationFilter filter, string container, long? startIndex, long? maxResults)
        { 
            if (String.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));

            string url = _Endpoint + _UserGuid + "/" + container;
            url += "?search";
            if (startIndex != null) url += "&index=" + startIndex;
            if (maxResults != null) url += "&count=" + maxResults;
            
            RestRequest req = new RestRequest(
                url,
                HttpMethod.PUT,
                _AuthHeaders,
                "application/json");

            req.IgnoreCertificateErrors = IgnoreCertificateErrors;

            string data = null;
            RestResponse resp = null;
             
            if (filter != null)
            {
                data = KvpbaseCommon.SerializeJson(filter, true);
                resp = await req.SendAsync(data);
            }
            else
            {
                resp = await req.SendAsync();
            } 

            KvpbaseException e = KvpbaseException.FromRestResponse(resp);
            if (e != null) throw e;

            return KvpbaseCommon.DeserializeJson<ContainerMetadata>(KvpbaseCommon.StreamToBytes(resp.Data)); 
        }

        #endregion
    }
}

