using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace HuntroxGames.Utils
{
    public static class UtilsEditor
    {
        public static string[] GetCurrentScenesName()
        {
            List<string> scenes = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                scenes.Add(Path.GetFileNameWithoutExtension(scene.path));
            }
            return scenes.ToArray();
        }
        public static void CreateNewAudioMixer(string path)
        {
            var doCreateAudioMixer = typeof(EndNameEditAction).Assembly.GetType("UnityEditor.ProjectWindowCallback.DoCreateAudioMixer");
            var icon = EditorGUIUtility.IconContent("d_Audio Mixer@2x").image as Texture2D;
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, 
                (EndNameEditAction)ScriptableObject.CreateInstance(doCreateAudioMixer), path, icon, null);
        }
        /// <summary>
        /// Get All Sprites from Texture
        /// </summary>
        /// <param name="texture2D"></param>
        /// <returns></returns>
        public static List<Sprite> LoadTextureSprites(Texture2D texture2D)
        {
            List<Sprite> spritesList = new List<Sprite>(); 
            string texturePath = AssetDatabase.GetAssetPath(texture2D);
            Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(texturePath)
                .OfType<Sprite>().ToArray();
            foreach (var sprite in sprites)
                spritesList.Add(sprite);
            return spritesList;
        }

    }
}
