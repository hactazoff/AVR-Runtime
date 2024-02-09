#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
namespace AVR.SDK
{
	public class Editor
	{
		public static AVR.SDK.Base.Descriptor GetDescriptor() 
		{
			AVR.SDK.Base.Descriptor descriptor = null;
			var scene = SceneManager.GetActiveScene();
			foreach (var gameObject in scene.GetRootGameObjects())
				if (gameObject.activeInHierarchy && gameObject.GetComponent<AVR.SDK.Base.Descriptor>() != null)
					descriptor = gameObject.GetComponent<AVR.SDK.Base.Descriptor>();
			return descriptor;
		}

		[MenuItem("AVR/Reload")]
		public static void Refresh()
		{
			AssetDatabase.Refresh();
			if (EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
				EditorApplication.isPlaying = true;
			}
			// reload all
		}

		[MenuItem("AVR/Save")]
		public static void Save() 
		{
	        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
			AssetDatabase.SaveAssets();
		}


		[MenuItem("AVR/Build World/Avatar")]
		public static void Build()
		{
			var descriptor = GetDescriptor();
			if(descriptor == null)
				UnityEditor.EditorUtility.DisplayDialog("AVR Build", "No Avatar/World Descriptor found.", "OK");
			Refresh();
			EditorApplication.isPlaying = false;
			Save();
			switch (descriptor.AssetType)
			{
				case AVR.SDK.Base.DescriptorType.WORLD:
					AVR.SDK.Worlds.BuildWorld.Build(descriptor as AVR.SDK.Worlds.WorldDescriptor, SceneManager.GetActiveScene());
					break;
				default:
					UnityEditor.EditorUtility.DisplayDialog("AVR Build", "Invalid Descriptor.", "OK");
					return;
			}
		}

		[MenuItem("AVR/Mods/Build Mods")]
		public static void BuildMods()
		{
			// build project 
		}


	}
}
#endif