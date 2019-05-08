using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

namespace GameFrameWork
{

    public class Loom : MonoBehaviour
    {
        public static int _maxThreads = 5;
        private static int _numThreads = 0;
        private static bool _initialized = false;
        private static Loom _current;

        List<Action> _actions = new List<Action>();
        List<Action> _currentActions = new List<Action>();

        public struct DelayedQueueItem
        {
            public float time;
            public Action action;
        }

        List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();
        List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();


        public static Loom GetInstance()
        {
            Initialize();
            return _current;
        }


        void Awake()
        {
            Initialize();
            _current = this;
            _initialized = true;
            DontDestroyOnLoad(gameObject);
        }


        static void Initialize()
        {
            if (!_initialized)
            {
                if (!Application.isPlaying)
                    return;
                _initialized = true;
                _current = EventCenter.Instance.gameObject.AddComponent<Loom>();
            }
        }

        public static void QueueOnMainThread(Action action)
        {
            QueueOnMainThread(action, 0f);
        }

        public static void QueueOnMainThread(Action action, float time)
        {
            if (time != 0)
            {
                lock (_current._delayed)
                {
                    _current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
                }
            }
            else
            {
                lock (_current._actions)
                {
                    _current._actions.Add(action);
                }
            }
        }

        public static Thread RunAsync(Action a)
        {
            Initialize();
            while (_numThreads >= _maxThreads)
            {
                Thread.Sleep(1);
            }
            Interlocked.Increment(ref _numThreads);
            ThreadPool.QueueUserWorkItem(RunAction, a);
            return null;
        }

        private static void RunAction(object action)
        {
            try
            {
                ((Action)action)();
            }
            catch
            {
            }
            finally
            {
                Interlocked.Decrement(ref _numThreads);
            }
        }
        void OnDisable()
        {
            if (_current == this)
            {
                _current = null;
            }
        }

        void Update()
        {
            if (_actions.Count > 0)
            {
                lock (_actions)
                {
                    _currentActions.Clear();
                    _currentActions.AddRange(_actions);
                    _actions.Clear();
                }

                for (int _dex = 0; _dex < _currentActions.Count; ++_dex)
                    _currentActions[_dex]();

            }

            if (_delayed.Count > 0)
            {
                lock (_delayed)
                {
                    _currentDelayed.Clear();
                    _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
                    for (int _dex = 0; _dex < _currentDelayed.Count; ++_dex)
                        _delayed.RemoveAt(_dex);
                }
                for (int _dex = 0; _dex < _currentDelayed.Count; ++_dex)
                    _currentDelayed[_dex].action();
            }//if


        }
    }
}
