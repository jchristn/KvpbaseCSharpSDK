using System;
using System.Collections.Generic; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvpbaseSDK
{
    /// <summary>
    /// Details about an audit log entry.
    /// </summary>
    public class AuditLogEntry
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
        /// Container GUID.
        /// </summary>
        public string ContainerGUID { get; set; }

        /// <summary>
        /// Object GUID.
        /// </summary>
        public string ObjectGUID { get; set; }

        /// <summary>
        /// Action performed by the requestor.
        /// </summary>
        public AuditLogEntryType Action { get; set; }

        /// <summary>
        /// Metadata associated with the action.
        /// </summary>
        public string Metadata { get; set; }

        /// <summary>
        /// Timestamp of the action.
        /// </summary>
        public DateTime CreatedUtc { get; set; }
         
        /// <summary>
        /// Instantiates the object.
        /// </summary>
        public AuditLogEntry()
        {

        } 
    }
}
