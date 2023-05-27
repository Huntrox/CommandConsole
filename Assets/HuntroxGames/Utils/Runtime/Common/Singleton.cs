using UnityEngine;

namespace HuntroxGames.Utils
{
	public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		
		protected static T instance;
		[SerializeField] protected bool dontDestroyOnLoad = false;
		
		public static T Instance
		{
			get
			{
				if (instance) return instance;
				instance = FindObjectOfType<T>();

				if (!instance)
					instance = new GameObject(typeof(T).ToString()).AddComponent<T>();

				return instance;
			}
		}

		public static bool HasInstance
			=> instance != null;

		protected virtual void Awake()
		{
			if (instance == null)
			{
				instance = (T)this;

#if UNITY_EDITOR
				if (UnityEditor.EditorApplication.isPlaying)
#endif
					if (dontDestroyOnLoad)
						DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}
}