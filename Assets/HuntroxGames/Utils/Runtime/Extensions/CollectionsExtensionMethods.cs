using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HuntroxGames.Utils
{
    public static class CollectionsExtensionMethods
    {
        public static T Closest<T>(this IEnumerable<T> list, Vector3 currentPosition) where T : Component
        {
            T best = null;
            var closestDistance = Mathf.Infinity;
            foreach (var target in list)
            {
                var directionToTarget = target.transform.position - currentPosition;
                var disSqr = directionToTarget.sqrMagnitude;
                if (disSqr >= closestDistance) continue;
                closestDistance = disSqr;
                best = target;
            }
            return best;
        }
        public static GameObject Closest(this IEnumerable<GameObject> list, Vector3 currentPosition)
        {
            GameObject best = null;
            var closestDistance = Mathf.Infinity;
            foreach (var target in list)
            {
                var directionToTarget = target.transform.position - currentPosition;
                var disSqr = directionToTarget.sqrMagnitude;
                if (disSqr >= closestDistance) continue;
                closestDistance = disSqr;
                best = target;
            }
            return best;
        }
        
        public static IEnumerable<T> Page<T>(this IEnumerable<T> list, int pageSize, int page)
            => list.Skip(page * pageSize).Take(pageSize);
        
        public static void DestroyAll<T>(this IEnumerable<T> arr) where T : MonoBehaviour
        {
            foreach (var item in arr) 
                Object.Destroy(item.gameObject);
        }

        public static void Then<T>(this ICollection<T> array, Action<T> callback)
        {
            foreach (var item in array)
            {
                callback?.Invoke(item);
            }
        }
        
        public static T RemoveRandom<T>(this IList<T> list)
        {
            if (list.IsNullOrEmpty()) throw new System.IndexOutOfRangeException("Cannot remove a random item from an empty list");
            int index = UnityEngine.Random.Range(0, list.Count);
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }
        
        public static T RandomElement<T>(this T[] array)
        {
            if (array.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(array));
            return array[UnityEngine.Random.Range(0, array.Length)];
        }
        public static T RandomElement<T>(this List<T> list)
        {
            if (list.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(list));
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        public static T Random<T>(this IList<T> list) 
            => list[UnityEngine.Random.Range(0, list.Count)];
        
        public static List<T> ListDotClone<T>(this List<T> list) where T : ICloneable
        {
            List<T> _list = new List<T>();
            foreach (var item in list)
            {
                object clone = item.Clone();
                _list.Add((T) clone);
            }

            return _list;
        }
        
        public static void Resize<T>(this List<T> list, int size, T element = default(T))
        {
            int count = list.Count;

            if (size < count)
            {
                list.RemoveRange(size, count - size);
            }
            else if (size > count)
            {
                if (size > list.Capacity)
                    list.Capacity = size;

                list.AddRange(Enumerable.Repeat(element, size - count));
            }
        }
        public static void RemoveAt<T>(ref T[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                arr[a] = arr[a + 1];
            }
            Array.Resize(ref arr, arr.Length - 1);
        }
        
        public static void SwapItems<T>(this List<T> list, int index1, int index2)
            => (list[index1], list[index2]) = (list[index2], list[index1]);
        public static List<T> GetEveryOther<T>(this IEnumerable<T> list) 
            => list.Where((t, i) => i % 2 == 0).ToList();
        public static List<T> GetEveryX<T>(this IEnumerable<T> list,int x) 
            => list.Where((t, i) => i % x == 0).ToList();
        public static List<T> GetEveryX<T>(this IEnumerable<T> list,int x, int startIndex) 
            => list.Where((t, i) => i >= startIndex && i % x == 0).ToList();
        
        

        #region Shuffle
        
        public static void Shuffle<T>(this IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var r = UnityEngine.Random.Range(i, list.Count);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }
        public static void Shuffle<T>(this IList<T> list, System.Random rng)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var r = rng.Next(i, list.Count);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }
        public static void Shuffle<T>(this IList<T> list, int seed)
        {
            var rng = new System.Random(seed);
            for (var i = 0; i < list.Count; i++)
            {
                var r = rng.Next(i, list.Count);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }
        public static void Shuffle<T>(this IList<T> list, System.Random rng, int seed)
        {
            var rng2 = new System.Random(seed);
            for (var i = 0; i < list.Count; i++)
            {
                var r = rng.Next(i, list.Count);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }

        #endregion
        #region IsNullOrEmpty
        public static bool IsNullOrEmpty<T>(this T[] array) => array == null || array.Length < 1;
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> array) => array == null || !array.Any();
        public static bool IsNullOrEmpty<T>(this List<T> list) => list == null || list.Count < 1;
        public static bool IsNullOrEmpty<T>(this Queue<T> queue) => queue == null || queue.Count < 1;
        public static bool IsNullOrEmpty<T1, T2>(this Dictionary<T1, T2> dictionary) =>
            dictionary == null || dictionary.Count < 1;
        //IList is null of empty
        public static bool IsNullOrEmpty<T>(this IList<T> list) => list == null || list.Count < 1;
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection) => collection == null || collection.Count < 1;
        #endregion
        #region Dictionary ForEach
        
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