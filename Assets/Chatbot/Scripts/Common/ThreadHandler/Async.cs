using System;
using System.Collections.Generic;

namespace ChatBot.ThreadHandler {
    public static class Async {
        private static Dispatcher Dispatcher => Dispatcher.Instance;

        private static readonly Dictionary<string, object> Locks = new Dictionary<string, object>();

        public static AsyncTask Run(Action action) {
            var asyncTask = new AsyncTask(action);
            Dispatcher.RegisterTask(asyncTask);
            return asyncTask;
        }

        public static AsyncTask<T> Run<T>(Func<T> func) {
            var asyncTask = new AsyncTask<T>(func);
            Dispatcher.RegisterTask(asyncTask);
            return asyncTask;
        }

        public static AsyncTask RunInBackground(string taskName, int runFrequencyMs, Action action) {
            if (Dispatcher.HasBackgroundTask(taskName)) return Dispatcher.GetBackgroundTask(taskName);
            var asyncTask = new AsyncTask(action, runFrequencyMs);
            Dispatcher.RegisterBackgroundTask(taskName, asyncTask);
            return asyncTask;

        }

        public static AsyncTask<T> RunInBackground<T>(string taskName, int runFrequencyMs, Func<T> func) {
            if (Dispatcher.HasBackgroundTask(taskName) == false) {
                var asyncTask = new AsyncTask<T>(func, runFrequencyMs);
                Dispatcher.RegisterBackgroundTask(taskName, asyncTask);
                return asyncTask;
            }

            var genericAsyncTask = Dispatcher.GetBackgroundTask(taskName) as AsyncTask<T>;
            if (genericAsyncTask == null) {
                throw new InvalidOperationException("Cannot find requested generic AsyncTask with name " + taskName);
            }

            return genericAsyncTask;
        }

        public static object GetLock(string key) {
            Locks.TryGetValue(key, out var lockObj);

            if (lockObj != null) return lockObj;
            lockObj = new object();
            Locks.Add(key, lockObj);

            return lockObj;
        }
    }
}
