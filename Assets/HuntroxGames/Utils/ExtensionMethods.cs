using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HuntroxGames.Utils
{
    public static class ExtensionMethods
    {
        public static int GetIndex<T>(this T[] array, T item) => Array.FindIndex(array, val => val.Equals(item));

        public static int ToInt(this string str, int defaultValue = default(int))
        {
            if (string.IsNullOrEmpty(str))
                return defaultValue;
            return !int.TryParse(str, out int s) ? s : defaultValue;
        }

        public static float ToFloat(this string str, float defaultValue = default(float))
        {
            if (string.IsNullOrEmpty(str))
                return defaultValue;
            return !float.TryParse(str, out float s) ? s : defaultValue;
        }
        public static float Snap(this float value, float snappingValue)
        {
            if (snappingValue == 0) return value;
            return Mathf.Round(value / snappingValue) * snappingValue;
        }
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (go == null)
                return null;

            var type = typeof(T);
            var component = go.GetComponent(type);
            if (component == null) component = go.AddComponent(type);

            return component as T;
        }
        public static T GetOrAddComponent<T>(this Component mComponent) where T : Component
        {
            if (mComponent == null)
                return null;

            var type = typeof(T);
            var component = mComponent.gameObject.GetComponent(type);
            if (component == null) component = mComponent.gameObject.AddComponent(type);

            return component as T;
        }
        public static bool IsNullOrEmpty<T>(this T[] array) => array == null || array.Length < 1;
        public static bool IsNullOrEmpty<T>(this List<T> list) => list == null || list.Count < 1;
        public static bool IsNullOrEmpty<T>(this Queue<T> queue) => queue == null || queue.Count < 1;
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

        public static bool IsNullOrEmpty<T1, T2>(this Dictionary<T1, T2> dictionary) =>
            dictionary == null || dictionary.Count < 1;

        public static bool IsNull(this GameObject go) => (go == null);
        public static bool IsNull(this Component component) => (component == null);

        public static void Resize<T>(this List<T> list, int size, T element = default(T))
        {
            int count = list.Count;

            if (size < count)
                list.RemoveRange(size, count - size);
            else if (size > count)
            {
                if (size > list.Capacity)
                    list.Capacity = size;

                list.AddRange(Enumerable.Repeat(element, size - count));
            }
        }
        public static void SwapItems<T>(this List<T> list, int index1, int index2)
        {
            T tempElement = list[index1];
            list[index1] = list[index2];
            list[index2] = tempElement;
        }

        public static string ReFormat(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (str.Contains(@"\n"))
                    str = str.Replace(@"\n", "\n");
                if (str.Contains(@"\t"))
                    str = str.Replace(@"\t", "\t");
                if (str.Contains(@"\r"))
                    str = str.Replace(@"\r", "\r");
            }

            return str;
        }

        public static bool CaseInsensitiveContains(this string text, string value,
            StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return text.IndexOf(value, stringComparison) >= 0;
        }

        public static void RemoveCloneName(this GameObject gameObject) =>
            gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);

        #region Dictionary.ForEach

        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dict,
            Action<KeyValuePair<TKey, TValue>> action)
        {
            foreach (KeyValuePair<TKey, TValue> entry in dict)
            {
                action?.Invoke(entry);
            }
        }

        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dict, Action<TKey, TValue> action)
        {
            foreach (KeyValuePair<TKey, TValue> mType in dict)
            {
                action?.Invoke(mType.Key, mType.Value);
            }
        }

        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dict, Action<TValue> value)
        {
            foreach (KeyValuePair<TKey, TValue> mType in dict)
            {
                value?.Invoke(mType.Value);
            }
        }

        #endregion
    }
}