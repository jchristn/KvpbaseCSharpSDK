﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

namespace KvpbaseSDK
{
    /// <summary>
    /// Entry types for the container audit log.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AuditLogEntryType
    {
        /// <summary>
        /// Enumerate.
        /// </summary>
        [EnumMember(Value = "Enumerate")]
        Enumerate,
        /// <summary>
        /// Read.
        /// </summary>
        [EnumMember(Value = "Read")]
        Read,
        /// <summary>
        /// ReadRange.
        /// </summary>
        [EnumMember(Value = "ReadRange")]
        ReadRange,
        /// <summary>
        /// Write.
        /// </summary>
        [EnumMember(Value = "Write")]
        Write,
        /// <summary>
        /// WriteKeyValue.
        /// </summary>
        [EnumMember(Value = "WriteKeyValue")]
        WriteKeyValue,
        /// <summary>
        /// WriteRange.
        /// </summary>
        [EnumMember(Value = "WriteRange")]
        WriteRange,
        /// <summary>
        /// WriteTags.
        /// </summary>
        [EnumMember(Value = "WriteTags")]
        WriteTags,
        /// <summary>
        /// Delete.
        /// </summary>
        [EnumMember(Value = "Delete")]
        Delete,
        /// <summary>
        /// DeleteKeyValue.
        /// </summary>
        [EnumMember(Value = "DeleteKeyValue")]
        DeleteKeyValue,
        /// <summary>
        /// DeleteTags.
        /// </summary>
        [EnumMember(Value = "DeleteTags")]
        DeleteTags,
        /// <summary>
        /// Rename.
        /// </summary>
        [EnumMember(Value = "Rename")]
        Rename,
        /// <summary>
        /// Exists.
        /// </summary>
        [EnumMember(Value = "Exists")]
        Exists,
        /// <summary>
        /// Configuration.
        /// </summary>
        [EnumMember(Value = "Configuration")]
        Configuration
    }
}
