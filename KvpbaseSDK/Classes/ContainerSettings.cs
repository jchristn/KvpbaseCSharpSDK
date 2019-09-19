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
        #region Public-Members

        /// <summary>
        /// Row ID in the database.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Object GUID.
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// The username of the ontainer owner.
        /// </summary>
        public string UserGuid { get; set; }

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

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object using default settings.
        /// </summary>
        public Container()
        {
            DefaultSettings("Default", "Default", "./");
        }

        /// <summary>
        /// Instantiate the object with a specific name (propagates to child members).
        /// </summary>
        /// <param name="user">The name of the user that owns the container.</param>
        /// <param name="name">The name of the container.</param>
        /// <param name="baseDir">The base directory under which the container subdirectory will be created.</param>
        public Container(string user, string name, string baseDir)
        {
            if (String.IsNullOrEmpty(user)) throw new ArgumentNullException(nameof(user));
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (String.IsNullOrEmpty(baseDir)) throw new ArgumentNullException(nameof(baseDir));
            DefaultSettings(user, name, baseDir);
        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        private void DefaultSettings(string user, string name, string baseDir)
        {
            if (String.IsNullOrEmpty(user)) user = "Default";
            if (String.IsNullOrEmpty(name)) name = "Default";
            if (String.IsNullOrEmpty(baseDir)) baseDir = "./";

            if (!baseDir.EndsWith("/")) baseDir += "/";
            baseDir += user + "/" + name + "/";

            UserGuid = user;
            Name = name;
            ObjectsDirectory = baseDir; 
            EnableAuditLogging = false;
            IsPublicRead = true;
            IsPublicWrite = false; 
        }

        #endregion
    }
}
