using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

namespace KvpbaseSDK
{
    /// <summary>
    /// The type of object handler.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    internal enum ObjectHandlerType
    {
        [EnumMember(Value = "Disk")]
        Disk
    }
}
