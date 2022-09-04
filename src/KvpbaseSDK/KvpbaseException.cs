﻿using System;
using System.Collections.Generic;
using System.Text;
using RestWrapper;

namespace KvpbaseSDK
{
    /// <summary>
    /// Kvpbase exception.
    /// </summary>
    public class KvpbaseException : Exception
    {
        /// <summary>
        /// HTTP status code.
        /// </summary>
        public int StatusCode = 0; 
         
        /// <summary>
        /// Response data from Kvpbase.
        /// </summary>
        public byte[] ResponseData = null;

        /// <summary>
        /// The type of exception.
        /// </summary>
        public ExceptionType Type = ExceptionType.Unknown;

        internal static KvpbaseException FromRestResponse(RestResponse resp)
        {
            KvpbaseException e = new KvpbaseException();

            if (resp == null)
            {
                e.StatusCode = 0;
                e.ResponseData = null;
                e.Type = ExceptionType.CannotConnect;
                return e;
            }

            if (resp.StatusCode >= 500)
            {
                e.StatusCode = resp.StatusCode;
                if (resp.ContentLength > 0) e.ResponseData = KvpbaseCommon.StreamToBytes(resp.Data);
                else e.ResponseData = null;
                e.Type = ExceptionType.InternalServerError;
                return e;
            }

            if (resp.StatusCode == 413)
            {
                e.StatusCode = resp.StatusCode;
                if (resp.ContentLength > 0) e.ResponseData = KvpbaseCommon.StreamToBytes(resp.Data);
                else e.ResponseData = null;
                e.Type = ExceptionType.PayloadTooLarge;
                return e;
            }

            if (resp.StatusCode == 409)
            {
                e.StatusCode = resp.StatusCode;
                if (resp.ContentLength > 0) e.ResponseData = KvpbaseCommon.StreamToBytes(resp.Data);
                else e.ResponseData = null;
                e.Type = ExceptionType.Conflict;
                return e;
            }

            if (resp.StatusCode == 404)
            {
                e.StatusCode = resp.StatusCode;
                if (resp.ContentLength > 0) e.ResponseData = KvpbaseCommon.StreamToBytes(resp.Data);
                else e.ResponseData = null;
                e.Type = ExceptionType.NotFound;
                return e;
            }

            if (resp.StatusCode == 401)
            {
                e.StatusCode = resp.StatusCode;
                if (resp.ContentLength > 0) e.ResponseData = KvpbaseCommon.StreamToBytes(resp.Data);
                else e.ResponseData = null;
                e.Type = ExceptionType.Unauthorized;
                return e;
            }

            if (resp.StatusCode == 400)
            {
                e.StatusCode = resp.StatusCode;
                if (resp.ContentLength > 0) e.ResponseData = KvpbaseCommon.StreamToBytes(resp.Data);
                else e.ResponseData = null;
                e.Type = ExceptionType.BadRequest;
                return e;
            }

            return null;
        }
    }

    /// <summary>
    /// The type of Kvpbase exception.
    /// </summary>
    public enum ExceptionType
    {
        /// <summary>
        /// Unknown exception type.
        /// </summary>
        Unknown,
        /// <summary>
        /// Could not connect to the server or could not retrieve a response.
        /// </summary> 
        CannotConnect,
        /// <summary>
        /// A server-side error was encountered.
        /// </summary>
        InternalServerError,
        /// <summary>
        /// Request data too large.
        /// </summary>
        PayloadTooLarge,
        /// <summary>
        /// A conflict exists, for example, attempting to write an object using a key that already exists.
        /// </summary>
        Conflict,
        /// <summary>
        /// The requested resource was not found.
        /// </summary>
        NotFound,
        /// <summary>
        /// You were not authorized to perform the request.
        /// </summary>
        Unauthorized,
        /// <summary>
        /// Your request was malformed.
        /// </summary>
        BadRequest
    }
}
