﻿using System;
using System.Collections.Generic;
using System.Text;

namespace KvpbaseSDK.Classes
{
    /// <summary>
    /// Serialization helper.
    /// </summary>
    public interface ISerializationHelper
    {
        /// <summary>
        /// Deserialize from JSON to an object of the specified type.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="json">JSON string.</param>
        /// <returns>Instance.</returns>
        T DeserializeJson<T>(string json) where T : class, new();

        /// <summary>
        /// Deserialize from JSON to an object of the specified type.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="json">Bytes containing JSON.</param>
        /// <returns>Instance.</returns>
        T DeserializeJson<T>(byte[] json) where T : class, new();

        /// <summary>
        /// Serialize from an object to a JSON string.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="pretty">Pretty print.</param>
        /// <returns>JSON string.</returns>
        string SerializeJson(object obj, bool pretty = true);
    }
}
