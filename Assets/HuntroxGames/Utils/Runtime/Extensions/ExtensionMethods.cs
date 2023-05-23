using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace HuntroxGames.Utils
{
    public static class ExtensionMethods
    {
        #region FloatExtension

        public static float Snap(this float value, float snappingValue)
        {
            if (snappingValue == 0) return value;
            return Mathf.Round(value / snappingValue) * snappingValue;
        }

        /// <summary>Round to nearest number of float points<br></br>
        /// <param><b>roundVal:</b> Number of decimal points to round value.(i.e. 2 = .00, 3 = .000, 4 = .0000, etc.)</param></summary>
        public static float RoundF(this float num, int roundVal)
            => (float)Math.Round((decimal)num, roundVal);

        /// <summary>Round float value to nearest half fraction (i.e. 2 = half, 3 = third, 4 = quarter, etc.)</summary>
        public static float RoundHalf(this float num, int roundVal) =>
            (float)Math.Round(num * roundVal, MidpointRounding.AwayFromZero) / roundVal;

        public static float ToSeconds(this float minutes)
            => minutes * 60f;

        /// <summary>
        /// Awards or penalizes percentage points based on input
        /// </summary>
        /// <param name="value"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static int AdjustByPercent(this int value, float percent)
            => Mathf.RoundToInt(((float)value * percent) + value);

        public static float Ratio(this float value, float maxValue)
            => (int)value / Mathf.Clamp(maxValue, Mathf.Epsilon, int.MaxValue);

        public static float Ratio(this float value, float minValue, float maxValue)
            => (int)value / Mathf.Clamp(maxValue - minValue, Mathf.Epsilon, int.MaxValue);

        public static float LinearRemap(this float value,
            float valueRangeMin, float valueRangeMax,
            float newRangeMin, float newRangeMax) =>
            (value - valueRangeMin) / (valueRangeMax - valueRangeMin) * (newRangeMax - newRangeMin) + newRangeMin;



        #endregion

        public static bool MaskContains(this LayerMask mask, int layerNumber)
            => mask == (mask | (1 << layerNumber));

        public static int WithRandomSign(this int value, float negativeProbability = 0.5f)
            => Random.value < negativeProbability ? -value : value;

        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            if (component == null)
                return null;

            var type = typeof(T);
            var compon = component.gameObject.GetComponent(type);
            if (compon == null) compon = component.gameObject.AddComponent(type);

            return compon as T;
        }

        public static void SetAlpha(this SpriteRenderer spriteRenderer, float alpha)
        {
            var color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }

        public static void SetAlpha(this UnityEngine.UI.Graphic graphic, float alpha)
        {
            var color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }

        public static Color SetAlpha(this Color color, float alpha)
            => new Color(color.r, color.g, color.b, alpha);



        public static bool IsTouching(this Collider2D collider, Collider2D other)
            => collider.IsTouching(other.bounds);

        public static bool IsTouching(this Collider2D collider, Bounds other)
            => collider.bounds.Intersects(other);

        #region Transform

        public static bool IsFacingTarget(this Transform transform, Transform target, float dotThreshold = 0.5f)
        {
            var vectorToTarget = target.position - transform.position;
            vectorToTarget.Normalize();
            var dot = Vector3.Dot(transform.forward, vectorToTarget);
            return dot >= dotThreshold;
        }

        public static bool IsNull(this Component component) => (component == null);

        public static void LocalReset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void Reset(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void DestroyAllChildren(this Transform transform)
        {
            foreach (Transform child in transform) UnityEngine.Object.Destroy(child.gameObject);
        }

        #endregion

        #region Events

        public static void AddListener(this EventTrigger eventTrigger, EventTriggerType triggerType,
            UnityAction<BaseEventData> call)
        {
            if (eventTrigger == null)
                throw new ArgumentNullException(nameof(eventTrigger));
            if (call == null)
                throw new ArgumentNullException(nameof(call));
            var entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
            if (entry == null)
            {
                entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
                eventTrigger.triggers.Add(entry);
            }

            entry.callback.AddListener(call);
        }

        public static void RemoveListener(this EventTrigger eventTrigger, EventTriggerType triggerType,
            UnityAction<BaseEventData> call)
        {
            if (eventTrigger == null)
                throw new ArgumentNullException(nameof(eventTrigger));
            if (call == null)
                throw new ArgumentNullException(nameof(call));
            var entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
            entry?.callback.RemoveListener(call);
        }

        public static void RemoveAllListeners(this EventTrigger eventTrigger, EventTriggerType triggerType)
        {
            if (eventTrigger == null)
                throw new ArgumentNullException(nameof(eventTrigger));
            var entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
            entry?.callback.RemoveAllListeners();
        }

        #endregion
    }
}