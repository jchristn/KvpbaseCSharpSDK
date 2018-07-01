using System;
using System.Collections.Generic; 
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
        #region Public-Members

        /// <summary>
        /// The ID of the object.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// The object's key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The content type of the object.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The content length of the object.
        /// </summary>
        public long? ContentLength { get; set; }

        /// <summary>
        /// The MD5 hash of the object's data.
        /// </summary>
        public string Md5 { get; set; }

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
         
        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public ObjectMetadata()
        {
            Initialize();
        }
         
        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="key">The object's key.</param>
        /// <param name="contentType">The content type of the object.</param>
        /// <param name="data">The object's data.</param>
        public ObjectMetadata(string key, string contentType, byte[] data)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (String.IsNullOrEmpty(contentType)) contentType = "application/octet-stream";
            if (data == null) data = new byte[0];

            Initialize();
            Key = key;
            ContentType = contentType;
            ContentLength = data.Length;

            if (data != null && data.Length > 0) Md5 = KvpbaseCommon.Md5(data);
            else Md5 = null;

            DateTime ts = DateTime.Now.ToUniversalTime();
            CreatedUtc = ts;
            LastUpdateUtc = ts;
            LastAccessUtc = ts;
        }

        #endregion

        #region Public-Methods
         
        #endregion

        #region Private-Methods

        private void Initialize()
        {
            Id = 0;
            Key = null;
            ContentType = "application/octet-stream";
            ContentLength = 0;
            Md5 = null;
            CreatedUtc = null;
            LastUpdateUtc = null;
            LastAccessUtc = null;
        }

        #endregion
    }
}
