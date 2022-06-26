using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HuntroxGames.Utils
{
    public static class Utils
    {
        public static Texture2D TextureFromSprite(this Sprite sprite)
        {
            if (sprite == null || sprite.texture == null) return null;
            if (sprite.rect.width != sprite.texture.width)
            {
                try
                {
                    if (sprite.rect.width != sprite.texture.width)
                    {
                        Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                        newText.filterMode = sprite.texture.filterMode;
                        //Color[] colors = newText.GetPixels();
                        Color[] newColors = sprite.texture.GetPixels((int)Mathf.CeilToInt(sprite.rect.x),
                                                                     (int)Mathf.CeilToInt(sprite.rect.y),
                                                                     (int)Mathf.CeilToInt(sprite.rect.width),
                                                                     (int)Mathf.CeilToInt(sprite.rect.height));
                        newText.SetPixels(newColors);
                        newText.Apply();
                        return newText;
                    }
                }
                catch
                {
                    return sprite.texture;
                }
            }
            return sprite.texture;
        }
        
        public static bool IsNotOverGameObject()
            => !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();


        public static T FindClosest<T>(Vector2 position, float radius, IEnumerable<T> others, T exclude = null) where T: MonoBehaviour
        {
            if (others.IsNullOrEmpty())
                return null;
        
            T best = null;
            float best_dist = Mathf.Infinity;
            foreach (T t in others)
            {
                if (exclude != null && t == exclude) continue;
                var dist = Vector2.Distance(t.transform.position, position);
                if (t && dist <= radius && dist < best_dist) best = t;
            }
            return best;
        }
        
        
        
        
        public static Vector2 WorldToCanvas(Vector3 worldPosition)=>WorldToCanvas(worldPosition,null,Camera.main);
        public static Vector2 WorldToCanvas(Vector3 worldPosition,RectTransform canvas)=> WorldToCanvas(worldPosition,canvas,Camera.main);
        public static Vector2 WorldToCanvas(Vector3 worldPosition, RectTransform canvas, Camera camera)
        {
            if (canvas == null)
                canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
            if (canvas == null)
                Debug.LogError("No Canvas was  found.");
            if (camera == null)
                camera = Camera.main;

            Vector2 offset = new Vector2((float)canvas.sizeDelta.x / 2f, (float)canvas.sizeDelta.y / 2f);
            Vector2 viewportPosition = camera.WorldToViewportPoint(worldPosition);
            Vector2 screenPosition = new Vector2(
                (viewportPosition.x * canvas.sizeDelta.x),
                (viewportPosition.y * canvas.sizeDelta.y)
                );
            return screenPosition - offset;
        }


        public static Vector2 GetRandomWorldPositionOnScreen() => GetRandomWorldPositionOnScreen(Vector2.zero, Camera.main);
        public static Vector2 GetRandomWorldPositionOnScreen(Camera cam) => GetRandomWorldPositionOnScreen(Vector2.zero, cam);
        public static Vector2 GetRandomWorldPositionOnScreen(Vector2 offset) => GetRandomWorldPositionOnScreen(offset, Camera.main);
        public static Vector2 GetRandomWorldPositionOnScreen(Vector2 offset, Camera cam) => new Vector2(
                UnityEngine.Random.Range(cam.ScreenToWorldPoint(new Vector2(0, 0)).x + offset.x,
                 cam.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x - offset.x),
                UnityEngine.Random.Range(cam.ScreenToWorldPoint(new Vector2(0, 0)).y + offset.y,
                 cam.ScreenToWorldPoint(new Vector2(0, Screen.height)).y - offset.y)
            );

        public static Vector3 RandomDirection()
             => new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0).normalized;
        public static Vector3 RandomDirection3DForward() =>
            new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;

        public static Vector3 RandomDirection(float minDis = 0.4f)
        {
            Vector3 direction = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0).normalized;
            direction *= UnityEngine.Random.Range(minDis, 1);
            return direction;
        }

        public static Vector2 GetVectorFromAngle(int angle)
        {
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static float GetAngleFromVector3(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            return n;
        }
        public static float GetAngleFromVector(Vector2 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            return n;
        }
        public static float GetAngle(Vector3 sender, Vector3 target)
        {
            Vector2 diff = (target - sender).normalized;
            return -Mathf.Rad2Deg * Mathf.Atan2(diff.x, diff.y);
        }
        public static Vector2 V3ToV2(Vector3 vec3) => new Vector2(vec3.x, vec3.z);
        public static Vector3 V2ToV3(Vector2 vec2, float y = 1f) => new Vector3(vec2.x, 0, vec2.y);
        
        public static Vector3 PredictedPosition(Vector3 targetPosition, Vector3 shooterPosition, Vector3 targetVelocity, float projectileSpeed)
        {
            Vector3 displacement = targetPosition - shooterPosition;
            float targetMoveAngle = Vector3.Angle(-displacement, targetVelocity) * Mathf.Deg2Rad;
            //if the target is stopping or if it is impossible for the projectile to catch up with the target (Sine Formula)
            if (targetVelocity.magnitude == 0 || 
                targetVelocity.magnitude > projectileSpeed && Mathf.Sin(targetMoveAngle) 
                / projectileSpeed > Mathf.Cos(targetMoveAngle) / targetVelocity.magnitude)
            {
                return targetPosition;
            }
            //also Sine Formula
            float shootAngle = Mathf.Asin(Mathf.Sin(targetMoveAngle) * targetVelocity.magnitude / projectileSpeed);
            return targetPosition + targetVelocity 
                * displacement.magnitude / Mathf.Sin(Mathf.PI - targetMoveAngle - shootAngle) 
                * Mathf.Sin(shootAngle) / targetVelocity.magnitude;
        }
        public static void GeneratePolyCollFromSpriteShape(SpriteRenderer renderer)
        {
            if (renderer && renderer.TryGetComponent(out PolygonCollider2D polygon))
            {
                var physicsCount = renderer.sprite.GetPhysicsShapeCount();
                polygon.pathCount = physicsCount;
                for (int i = 0; i < physicsCount; i++)
                {
                    var list = new List<Vector2>();
                    renderer.sprite.GetPhysicsShape(i, list);
                    polygon.SetPath(i, list.ToArray());
                }
            }
        }
        
        public static string NewGuid(string format = "")
            => Guid.NewGuid().ToString(format);
        public static string GenGuid(int length = 16)
            => new string(Guid.NewGuid().ToString("N").Take(length).ToArray());
        
        public static void SetAudioGroupVolume(UnityEngine.Audio.AudioMixer target,float value,string volumeParameter)
        {
            float volume = Mathf.Clamp01(value);
            if (volume > 0f)
                volume = 45 * Mathf.Log10(volume);
            else
                volume = - 144f;
            target.SetFloat(volumeParameter, volume);
        }
    }
}