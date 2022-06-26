using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HuntroxGames.Utils
{
    public static class GameObjectExtensionsMethods
    {
        
        
        public static bool TryFindObjectOfType<T>(this Object obj, out T output) where T : Object
        {
            output = Object.FindObjectOfType<T>();
            return output != null;
        }

        
        public static void RemoveCloneName(this GameObject gameObject)
            => gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);
        
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (null == go)
                return null;
            var type = typeof(T);
            var component = go.GetComponent(type);
            if (null == component) component = go.AddComponent(type);

            return component as T;
        }

        public static bool IsNull(this GameObject go) => (go == null);
        
        public static void ChangeLayers(this GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform) ChangeLayers(child.gameObject, layer);
        }
        public static void ChangeLayers(this GameObject obj, string layerName)
        {
            obj.layer = LayerMask.NameToLayer(layerName);
            foreach (Transform child in obj.transform) ChangeLayers(child.gameObject, layerName);
        }
        
        public static T[] GetInterfaces<T>(this GameObject target)
        {
            var targetMono = target.GetComponents<MonoBehaviour>();
            return (from a in targetMono where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a)
                .ToArray();
        }

        public static void RemoveComponents(this GameObject gameObject, params System.Type[] components)
        {
            foreach (var comp in components)
                if (gameObject.TryGetComponent(comp, out var component))
                    Object.DestroyImmediate(component);
        }

        public static bool RemoveComponent<T>(this GameObject gameObject) where T : Component
        {
            if (!gameObject)
                return false;
            if (!gameObject.TryGetComponent<T>(out var component)) return false;
            Object.DestroyImmediate(component);
            return true;
        }
    }
}