using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace GameFrameWork
{
    /// <summary>
    /// 非Mono的单例泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton_Static<T> where T:class,new()
    {
        protected static T S_Instance = default(T);
        private static object obj = new object();
        public static T Instance {
            get
            {
                if (S_Instance == null)
                {
                    lock (obj)
                    {
                        //***反射获取 非静态 InitialSingleton 方法 并调用
                        Type _type = typeof(T);
                        BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Instance;
                        MethodInfo _infor = _type.GetMethod("InitialSingleton", flag); //反射获得InitialSingleton 方法
                        object obj = Activator.CreateInstance(_type);
                        S_Instance = (T)obj;
                        //  Debug.Log(_type + "  instance=" + instance);
                        if (S_Instance == null)
                            Debug.LogError("GetInstance Fail .... " + _type);

                        _infor.Invoke(obj, null); //调用方法
                    }
                }
                return S_Instance;
            }
        }



        /// <summary>
        /// 初始化单例实例的接口(只会调用一次除非对象被销毁)
        /// </summary>
        protected virtual void InitialSingleton()  { }

        public virtual void DisposeInstance()
        {
            S_Instance = null;
        }

    }
}