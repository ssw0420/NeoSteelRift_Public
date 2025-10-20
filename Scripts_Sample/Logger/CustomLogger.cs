using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.CompilerServices;

namespace NeoSteelRift.Scripts.Logger
{
    public static class CustomLogger
    {
        private const string COLOR_INFO = "#00ff00"; // Green
        private const string COLOR_WARNING = "yellow";
        private const string COLOR_ERROR = "red";
        private const string COLOR_ASSERT = "magenta";
        private const string COLOR_EXCEPTION = "orange";

        /// <summary>
        /// ex) [INFO] [MyScript.cs -> MyMethod -> line 10] This is an info message.
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="colorCode"></param>
        /// <param name="message"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        private static string FormatLogMessage(
            string logType,
            string colorCode,
            string message,
            string filePath,
            int lineNumber,
            string memberName)
        {
            string fileName = Path.GetFileName(filePath);

            return
                $"<color={colorCode}>[{logType}]</color> " +
                $"[{fileName} -> {memberName} -> line {lineNumber}] " +
                $"{message}";
        }

        /// <summary>
        /// Log an info message to the console. (Debug.Log)
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG_MODE")]
        public static void LogInfo(
            string message,
            UnityEngine.Object context = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = "")
        {
            string finalMsg = FormatLogMessage("INFO", COLOR_INFO, message, filePath, lineNumber, memberName);
            if (context == null) Debug.Log(finalMsg);
            else Debug.Log(finalMsg, context);
        }

        /// <summary>
        /// Log a warning message to the console. (Debug.LogWarning)
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG_MODE")]
        public static void LogWarning(
            string message,
            UnityEngine.Object context = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = "")
        {
            string finalMsg = FormatLogMessage("WARNING", COLOR_WARNING, message, filePath, lineNumber, memberName);
            if (context == null) Debug.LogWarning(finalMsg);
            else Debug.LogWarning(finalMsg, context);
        }

        /// <summary>
        /// Log an error message to the console. (Debug.LogError)
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG_MODE")]
        public static void LogError(
            string message,
            UnityEngine.Object context = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = "")
        {
            string finalMsg = FormatLogMessage("ERROR", COLOR_ERROR, message, filePath, lineNumber, memberName);
            if (context == null) Debug.LogError(finalMsg);
            else Debug.LogError(finalMsg, context);
        }


        /// <summary>
        /// Log an assert message to the console. (Debug.LogAssertion)
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG_MODE")]
        public static void LogAssertion(
            string message,
            UnityEngine.Object context = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = "")
        {
            string finalMsg = FormatLogMessage("ASSERT", COLOR_ASSERT, message, filePath, lineNumber, memberName);
            if (context == null) Debug.LogAssertion(finalMsg);
            else Debug.LogAssertion(finalMsg, context);
        }

        /// <summary>
        /// Log an exception message to the console. (Debug.LogException)
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG_MODE")]
        public static void LogException(
            Exception exception,
            UnityEngine.Object context = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = "")
        {
            string finalMsg = FormatLogMessage("EXCEPTION", COLOR_EXCEPTION, exception.Message, filePath, lineNumber, memberName);
            // User Log Error because LogException does not support context
            if (context == null) Debug.LogError(finalMsg, context);
            else Debug.LogError(finalMsg, context);
        }

        /// <summary>
        /// Assert a condition and log a message if the condition is false. (Debug.LogAssert)
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG_MODE")]
        public static void LogAssert(
            bool condition,
            string message,
            UnityEngine.Object context = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = "")
        {
            if (!condition)
            {
                string finalMsg = FormatLogMessage("ASSERT_FAIL", COLOR_ASSERT, message, filePath, lineNumber, memberName);
                if (context == null) Debug.LogAssertion(finalMsg);
                else Debug.LogAssertion(finalMsg, context);
            }
        }
        
    #if DEBUG_MODE
        public static bool IsDebugMode = true;
    #else
        public static bool IsDebugMode = false;
    #endif
        public static void Log(string message)
        {
            if (IsDebugMode)
            {
                Debug.Log(message);
            }
        }
    }
}