using System;
using UnityEngine;

namespace ZeroGame
{
    [Serializable]
    internal class ReporterSettings
    {
        [SerializeField] internal LogMatchData[] ignoredMessages;

        [Serializable]
        public struct LogMatchData
        {
            public LogType logType;
            public StringCheckType checkType;
            public string message;

            public readonly bool IsMatch(LogType logType, string message)
            {
                if (this.logType != logType) return false;
                if (checkType == StringCheckType.StartsWith && message.StartsWith(this.message))
                    return true;
                if (checkType == StringCheckType.Contains && message.Contains(this.message))
                    return true;
                if (checkType == StringCheckType.EndsWith && message.EndsWith(this.message))
                    return true;
                return false;
            }
        }

        public enum StringCheckType { StartsWith, Contains, EndsWith }
    }
}
