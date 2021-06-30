using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChatBot.ThreadHandler {
    public class Dispatcher : MonoBehaviour {
        private static readonly HashSet<AsyncTask> ThreadedTasks = new HashSet<AsyncTask>();
        private static readonly Dictionary<string, AsyncTask> BackgroundTasks = new Dictionary<string, AsyncTask>();

        private static Dispatcher _instance;

        public static Dispatcher Instance {
            get {
                if (_instance != null) return _instance;
                var dispatcherObject = new GameObject("TheadDispatcher");
                _instance = dispatcherObject.AddComponent<Dispatcher>();
                DontDestroyOnLoad(dispatcherObject);

                return _instance;
            }
        }

        private readonly List<AsyncTask> _deadTasks = new List<AsyncTask>();

        private void Update() {
            _deadTasks.Clear();
            foreach (var threadedTask in ThreadedTasks.Where(threadedTask => threadedTask.IsFinished)) {
                threadedTask.OnTaskFinished();
                _deadTasks.Add(threadedTask);
            }

            foreach (var threadedTask in _deadTasks) {
                ThreadedTasks.Remove(threadedTask);
            }

            foreach (var backgroundTask in BackgroundTasks) {
                backgroundTask.Value.OnTaskFinished();
            }
        }

        public void Reset() {
            foreach (var threadedTask in ThreadedTasks) {
                threadedTask.Thread.Abort();
            }

            ThreadedTasks.Clear();

            foreach (var backgroundTask in BackgroundTasks) {
                backgroundTask.Value.Thread.Abort();
            }

            BackgroundTasks.Clear();
        }

        public void RegisterTask(AsyncTask asyncTask) {
            ThreadedTasks.Add(asyncTask);
        }

        public bool HasBackgroundTask(string taskName) {
            return BackgroundTasks.ContainsKey(taskName);
        }

        public AsyncTask GetBackgroundTask(string taskName) {
            BackgroundTasks.TryGetValue(taskName, out var asyncTask);
            return asyncTask;
        }

        public void RegisterBackgroundTask(string taskName, AsyncTask asyncTask) {
            BackgroundTasks.Add(taskName, asyncTask);
        }
    }
}
