﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SAEON.Extensions.Logging
{
    public class LoggerParameters : Dictionary<string, object> { }

    public static class ILoggerExtensions
    {
        public static bool UseFullName { get; set; } = true;

        private static string GetTypeName(Type type, bool onlyName = false)
        {
            return UseFullName && !onlyName ? type.FullName : type.Name;
        }

        private static string GetParameters(LoggerParameters parameters)
        {
            string result = string.Empty;
            if (parameters != null)
            {
                bool isFirst = true;
                foreach (var kvPair in parameters)
                {
                    if (!isFirst) result += ", ";
                    isFirst = false;
                    result += kvPair.Key + "=";
                    if (kvPair.Value == null)
                        result += "Null";
                    else if (kvPair.Value is string)
                        result += string.Format("'{0}'", kvPair.Value);
                    else if (kvPair.Value is Guid)
                        result += string.Format("{0}", kvPair.Value);
                    else
                        result += kvPair.Value.ToString();
                }
            }
            return result;
        }

        private static string MethodSignature(Type type, string methodName, LoggerParameters parameters = null)
        {
            return $"{GetTypeName(type)}.{methodName}({GetParameters(parameters)})";
        }

        private static string MethodSignature(Type type, string methodName, string entityTypeName, LoggerParameters parameters = null)
        {
            return $"{GetTypeName(type)}.{methodName}<{entityTypeName}>({GetParameters(parameters)})";
        }

        public static MethodCall MethodCall(this ILogger log, Type type, LoggerParameters parameters = null, [CallerMemberName] string methodName = "")
        {
            return new MethodCall(log, MethodSignature(type, methodName, parameters));
        }

        public static void Exception(this ILogger log, string methodCall, Exception ex, string message = "", params object[] values)
        {
            log.LogError(ex, string.IsNullOrEmpty(message) ? "An exception occurred" : message, values);
        }

        public static void Error(this ILogger log, string methodCall, string message = "", params object[] values)
        {
            log.LogError(string.IsNullOrEmpty(message) ? "An error occurred" : message, values);
        }

        public static void Information(this ILogger log, string methodCall, string message, params object[] values)
        {
            log.LogInformation(message, values);
        }

        public static void Warning(this ILogger log, string methodCall, string message, params object[] values)
        {
            log.LogWarning(message, values);
        }

        public static void Verbose(this ILogger log, string methodCall, string message, params object[] values)
        {
            log.LogDebug(message, values);
        }
    }

    public class MethodCall : IDisposable
    {
        private readonly ILogger log = null;
        private readonly string methodCall;

        public MethodCall(ILogger log, string methodCall)
        {
            this.log = log;
            this.methodCall = methodCall;
            log.Verbose(methodCall, "Start");
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                log.Verbose(methodCall, "Done");
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MethodCall() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        public void Exception(Exception ex, string message = "", params object[] values)
        {
            log.Exception(methodCall, ex, message, values);
        }

        public void Error(string message = "", params object[] values)
        {
            log.Error(methodCall, message, values);
        }

        public void Information(string message, params object[] values)
        {
            log.Information(methodCall, message, values);
        }

        public void Warning(string message, params object[] values)
        {
            log.Warning(methodCall, message, values);
        }

        public void Verbose(string message, params object[] values)
        {
            log.Verbose(methodCall, message, values);
        }

    }
}