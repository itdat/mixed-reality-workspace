using System;
using System.Collections.Generic;
using UnityEngine;
using DateTime = System.DateTime;

namespace LogUtil {
    public static class LogTime {
        private static readonly Dictionary<string, long> startTimes = new Dictionary<string, long>();
        private static readonly Stack<string> _stack = new Stack<string>();

        public static void StartLog(string msg) {
            Debug.Log("Start: " + msg);
            startTimes[msg] = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            _stack.Push(msg);
        }

        public static void EndLog() {
            if (_stack.Count == 0) return;
            var msg = _stack.Pop();
            var startTime = startTimes[msg];
            startTimes.Remove(msg);
            var endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            Debug.Log("End:" + msg + " - " + (endTime - startTime) + " ms");
        }
    }
}
