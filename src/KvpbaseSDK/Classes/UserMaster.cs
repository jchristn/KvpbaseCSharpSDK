using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace KvpbaseSDK
{
    /// <summary>
    /// A Kvpbase user.
    /// </summary>
    public class UserMaster
    {
        /// <summary>
        /// Row ID in the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// GUID.
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// The first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The company name.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The user's home directory.  If null, a directory will be created under the default storage directory.
        /// </summary>
        public string HomeDirectory { get; set; }

        /// <summary>
        /// Indicates if the object is active or disabled.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// The timestamp from when the object was created.
        /// </summary>
        public DateTime CreatedUtc { get; set; }

        /// <summary>
        /// Instantiates the object.
        /// </summary>
        public UserMaster()
        {
            GUID = Guid.NewGuid().ToString();
            CreatedUtc = DateTime.Now.ToUniversalTime();
        }
    }
}
