using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeoSteelRift.Scripts.Logger
{
    public class LogExample : MonoBehaviour
    {
        private void Start()
        {
            CustomLogger.LogInfo("This is an info message.");
            CustomLogger.LogWarning("This is a warning message.");
            CustomLogger.LogError("This is an error message.");
            CustomLogger.LogAssert(1 + 1 == 3, "Test false", this);
            CustomLogger.LogAssertion("This is an assertion message.");
            CustomLogger.LogException(new System.Exception("This is an exception message."));
        }
    }
}