using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvpbaseSDK
{
    /// <summary>
    /// Metadata describing an object.
    /// </summary>
    public class ObjectMetadata
    {
        /// <summary>
        /// The ID of the object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The GUID of the object, also used as a unique name to store the object.
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// The GUID of the container that contains the object. 
        /// </summary>
        public string ContainerGUID { get; set; }

        /// <summary>
        /// The object's key.
        /// </summary>
        public string ObjectKey { get; set; }

        /// <summary>
        /// The content type of the object.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The content length of the object.
        /// </summary>
        public long ContentLength { get; set; }

        /// <summary>
        /// The MD5 hash of the object's data.
        /// </summary>
        public string Md5 { get; set; }

        /// <summary>
        /// The comma-separated list of tags associated with an object.
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// The creation timestamp, in UTC.
        /// </summary>
        public DateTime? CreatedUtc { get; set; }

        /// <summary>
        /// The time of last update, in UTC.
        /// </summary>
        public DateTime? LastUpdateUtc { get; set; }

        /// <summary>
        /// The time of last access, in UTC.
        /// </summary>
        public DateTime? LastAccessUtc { get; set; }

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public ObjectMetadata()
        {
            Id = 0;
            GUID = Guid.NewGuid().ToString();
            ObjectKey = null;
            ContentType = "application/octet-stream";
            ContentLength = 0;
            Md5 = null;
            CreatedUtc = null;
            LastUpdateUtc = null;
            LastAccessUtc = null;
        }

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="key">The object's key.</param>
        /// <param name="contentType">The content type of the object.</param>
        /// <param name="data">The object's data.</param>
        /// <param name="tags">Tags associated with the object.</param>
        public ObjectMetadata(string key, string contentType, byte[] data, List<string> tags)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (String.IsNullOrEmpty(contentType)) contentType = "application/octet-stream";
            if (data == null) data = new byte[0];

            ObjectKey = key;
            GUID = Guid.NewGuid().ToString();
            ContentType = contentType;
            ContentLength = data.Length;
            Tags = tags;

            if (data != null && data.Length > 0) Md5 = KvpbaseCommon.Md5(data);
            else Md5 = null;

            DateTime ts = DateTime.Now.ToUniversalTime();
            CreatedUtc = ts;
            LastUpdateUtc = ts;
            LastAccessUtc = ts;
        }

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="key">The object's key.</param>
        /// <param name="contentType">The content type of the object.</param>
        /// <param name="contentLength">The object's length.</param>
        /// <param name="tags">Tags associated with the object.</param>
        public ObjectMetadata(string key, string contentType, long contentLength, List<string> tags)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (String.IsNullOrEmpty(contentType)) contentType = "application/octet-stream";
            if (contentLength < 0) throw new ArgumentException("Invalid content length.");

            ObjectKey = key;
            GUID = Guid.NewGuid().ToString();
            ContentType = contentType;
            ContentLength = contentLength;
            Tags = tags;
            Md5 = null;

            DateTime ts = DateTime.Now.ToUniversalTime();
            CreatedUtc = ts;
            LastUpdateUtc = ts;
            LastAccessUtc = ts;
        }
         
        private void Initialize()
        {
            Id = 0;
            ObjectKey = null;
            ContentType = "application/octet-stream";
            ContentLength = 0;
            Tags = new List<string>();
            Md5 = null;
            CreatedUtc = null;
            LastUpdateUtc = null;
            LastAccessUtc = null;
        } 
    }
}
