using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using RestWrapper;

namespace KvpbaseSDK
{
    /// <summary>
    /// An object stored on Kvpbase.
    /// </summary>
    public class KvpbaseObject
    {
        /// <summary>
        /// The content-type of the object.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The number of bytes contained in the object, which also indicates the number of bytes to read from Data.
        /// </summary>
        public long ContentLength { get; set; }

        /// <summary>
        /// The stream containing the object data.
        /// </summary>
        public Stream Data { get; set; }

        /// <summary>
        /// Instantiates the object.
        /// </summary>
        public KvpbaseObject()
        {

        }

        /// <summary>
        /// Instantiates the object.
        /// </summary>
        /// <param name="contentType">The content-type of the object.</param>
        /// <param name="contentLength">The number of bytes contained in the object.</param>
        /// <param name="data">The stream containing the object data.</param>
        public KvpbaseObject(string contentType, long contentLength, Stream data)
        {
            ContentType = contentType;
            ContentLength = contentLength;
            Data = data;
        }

        internal static KvpbaseObject FromRestResponse(RestResponse resp)
        {
            if (resp == null) throw new ArgumentNullException(nameof(resp));

            KvpbaseObject ret = new KvpbaseObject();
            ret.ContentType = resp.ContentType;
            ret.ContentLength = resp.ContentLength;
            ret.Data = resp.Data;

            return ret;
        }
    }
}
