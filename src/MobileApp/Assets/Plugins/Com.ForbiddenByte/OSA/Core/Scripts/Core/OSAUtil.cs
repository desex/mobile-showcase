

using frame8.Logic.Misc.Visual.UI.MonoBehaviours;
using UnityEngine;

namespace Com.ForbiddenByte.OSA.Core
{
	public class OSAUtil
	{
		public static bool DotNETCoreCompat_IsAssignableFrom(System.Type to, System.Type from)
		{
#if NETFX_CORE
			if (!to.GetTypeInfo().IsAssignableFrom(from.Ge‌​tTypeInfo()))
#else
			if (!to.IsAssignableFrom(from))
#endif
				return false;

			return true;
		}

		/// <summary>
		/// Only for when any instance will do (existence checks, singletons), as it's unspecified which one is returned if there are more.
		/// Otherwise, use <see cref="FindFirstObjectInHierarchyOrder{T}"/>
		/// </summary>
		public static T UnityCompat_FindAnyObjectByType<T>() where T : UnityEngine.Object
		{
#if UNITY_2023_1_OR_NEWER
			// FindObjectOfType is obsolete since 2023.1, and FindFirstObjectByType since 6.4
			return UnityEngine.Object.FindAnyObjectByType<T>();
#else
			return UnityEngine.Object.FindObjectOfType<T>();
#endif
		}

		/// <summary>
		/// Finds the active loaded objects of type T, in the order they appear in the Hierarchy window: scene by scene, depth-first.
		/// Unity's own Find methods don't guarantee any order (the instance ID ordering was dropped in 6.4), while this is deterministic and the same on all Unity versions
		/// </summary>
		public static T[] FindObjectsInHierarchyOrder<T>() where T : Component
		{
			var objects = UnityCompat_FindObjectsByType<T>();
			var keys = new HierarchyOrderKey[objects.Length];
			for (int i = 0; i < objects.Length; i++)
				keys[i] = new HierarchyOrderKey(objects[i], System.Array.IndexOf(objects[i].GetComponents<T>(), objects[i]));
			System.Array.Sort(keys, objects);

			return objects;
		}

		/// <summary>See <see cref="FindObjectsInHierarchyOrder{T}"/></summary>
		public static T FindFirstObjectInHierarchyOrder<T>() where T : Component
		{
			var objects = FindObjectsInHierarchyOrder<T>();
			return objects.Length > 0 ? objects[0] : null;
		}

		static T[] UnityCompat_FindObjectsByType<T>() where T : UnityEngine.Object
		{
#if UNITY_6000_4_OR_NEWER
			// The overloads taking a FindObjectsSortMode are obsolete since 6.4
			return UnityEngine.Object.FindObjectsByType<T>();
#elif UNITY_2023_1_OR_NEWER
			// FindObjectsOfType is obsolete since 2023.1
			return UnityEngine.Object.FindObjectsByType<T>(FindObjectsSortMode.None);
#else
			return UnityEngine.Object.FindObjectsOfType<T>();
#endif
		}

		public static ScrollbarFixer8 ConfigureDinamicallyCreatedScrollbar(UnityEngine.UI.Scrollbar scrollbar, IOSA iAdapter, RectTransform viewport, bool addFixerIfDoesntExist = true)
		{
			var instanceScollbarFixer = scrollbar.GetComponent<ScrollbarFixer8>();
			if (!instanceScollbarFixer)
			{
				if (!addFixerIfDoesntExist)
					return null;
				instanceScollbarFixer = scrollbar.gameObject.AddComponent<ScrollbarFixer8>();
			}
			//instanceScollbarFixer.scrollRect = _WindowParams.scrollRect;
			instanceScollbarFixer.externalScrollRectProxy = iAdapter;
			instanceScollbarFixer.viewport = viewport;

			return instanceScollbarFixer;
		}


		class HierarchyOrderKey : System.IComparable<HierarchyOrderKey>
		{
			// The loaded scene's index, followed by the sibling index of each transform from the root down to the component's own
			readonly int[] _Path;
			// Differentiates components on the same GameObject
			readonly int _ComponentIndex;


			public HierarchyOrderKey(Component component, int componentIndex)
			{
				var tr = component.transform;
				int depth = 0;
				for (var t = tr; t; t = t.parent)
					++depth;

				_Path = new int[depth + 1];
				_Path[0] = GetLoadedSceneIndex(tr.gameObject.scene);
				for (var t = tr; t; t = t.parent)
					_Path[depth--] = t.GetSiblingIndex();

				_ComponentIndex = componentIndex;
			}


			public int CompareTo(HierarchyOrderKey other)
			{
				int minLength = Mathf.Min(_Path.Length, other._Path.Length);
				for (int i = 0; i < minLength; i++)
					if (_Path[i] != other._Path[i])
						return _Path[i].CompareTo(other._Path[i]);

				// Either one is the ancestor of the other (which comes first), or both are on the same GameObject
				if (_Path.Length != other._Path.Length)
					return _Path.Length.CompareTo(other._Path.Length);

				return _ComponentIndex.CompareTo(other._ComponentIndex);
			}


			static int GetLoadedSceneIndex(UnityEngine.SceneManagement.Scene scene)
			{
				for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
					if (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i) == scene)
						return i;

				// Not among the loaded scenes (e.g. DontDestroyOnLoad, which the Hierarchy also shows last)
				return int.MaxValue;
			}
		}
	}
}
