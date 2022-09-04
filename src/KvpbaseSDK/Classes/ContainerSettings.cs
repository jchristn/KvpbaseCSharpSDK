using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvpbaseSDK
{
    /// <summary>
    /// Settings for a container.
    /// </summary>
    public class Container
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
        /// The username of the owner.
        /// </summary>
        public string UserGUID { get; set; }

        /// <summary>
        /// The name of the container.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The full path to where container objects should be stored.
        /// </summary>
        public string ObjectsDirectory { get; set; }

        /// <summary>
        /// Enable or disable audit logging.
        /// </summary>
        public bool EnableAuditLogging { get; set; }

        /// <summary>
        /// Enable or disable public read access.
        /// </summary>
        public bool IsPublicRead { get; set; }

        /// <summary>
        /// Enable or disable public write access.
        /// </summary>
        public bool IsPublicWrite { get; set; }

        /// <summary>
        /// The timestamp from when the object was created.
        /// </summary>
        public DateTime? CreatedUtc { get; set; }

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public Container()
        {
            GUID = Guid.NewGuid().ToString();
            CreatedUtc = DateTime.Now.ToUniversalTime();
        }

        /// <summary>
        /// Instantiate the object with a specific name (propagates to child members).
        /// </summary>
        /// <param name="userGuid">The GUID of the user that owns the container.</param>
        /// <param name="name">The name of the container.</param>
        /// <param name="baseDir">The base directory under which the container subdirectory will be created.</param>
        public Container(string userGuid, string name, string baseDir)
        {
            if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (String.IsNullOrEmpty(baseDir)) throw new ArgumentNullException(nameof(baseDir));
            DefaultSettings(userGuid, name, baseDir);
        }
         
        private void DefaultSettings(string user, string name, string baseDir)
        {
            if (String.IsNullOrEmpty(user)) user = "default";
            if (String.IsNullOrEmpty(name)) name = "default";
            if (String.IsNullOrEmpty(baseDir)) baseDir = "./";

            if (!baseDir.EndsWith("/")) baseDir += "/";
            baseDir += user + "/" + name + "/";

            UserGUID = user;
            Name = name;
            ObjectsDirectory = baseDir; 
            EnableAuditLogging = false;
            IsPublicRead = true;
            IsPublicWrite = false; 
        } 
    }
}
