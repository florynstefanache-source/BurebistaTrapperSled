using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using BurebistaTrapperSled;
using MelonLoader;
using Microsoft.CodeAnalysis;
using UnityEngine;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: MelonInfo(typeof(BurebistaTrapperSled.Main), "Burebista Trapper Sled", "0.7.0", "Burebista", null)]
[assembly: MelonGame("Hinterland", "TheLongDark")]
[assembly: TargetFramework(".NETCoreApp,Version=v6.0", FrameworkDisplayName = ".NET 6.0")]
[assembly: AssemblyVersion("0.0.0.0")]
namespace Microsoft.CodeAnalysis
{
	[CompilerGenerated]
	[Microsoft.CodeAnalysis.Embedded]
	internal sealed class EmbeddedAttribute : Attribute
	{
	}
}
namespace BurebistaTrapperSled
{
	public class Main : MelonMod
	{
		private const string TargetRootName = "INTERACTIVE_Travois";

		private const string VisualRootName = "BurebistaSledVisual";

		private float nextScan;

		public override void OnInitializeMelon()
		{
			MelonLogger.Msg("Burebista Trapper Sled v0.7 cargado.");
			MelonLogger.Msg("Detector: solo INTERACTIVE_Travois. Orientacion visual corregida 180 grados.");
			MelonLogger.Msg("Rig, triggers, colliders y scripts del Travois NO se tocan.");
		}

		public override void OnUpdate()
		{
			if (!(Time.realtimeSinceStartup < nextScan))
			{
				nextScan = Time.realtimeSinceStartup + 1.5f;
				FindRealInteractiveTravois();
			}
		}

		private void FindRealInteractiveTravois()
		{
			Transform[] array;
			try
			{
				array = UnityEngine.Object.FindObjectsOfType<Transform>();
			}
			catch
			{
				return;
			}
			Transform[] array2 = array;
			foreach (Transform transform in array2)
			{
				if (transform == null || transform.gameObject == null)
				{
					continue;
				}
				string text;
				try
				{
					text = transform.gameObject.name ?? "";
				}
				catch
				{
					continue;
				}
				if (IsRealInteractiveTravoisName(text) && !(transform.Find("BurebistaSledVisual") != null))
				{
					try
					{
						ConvertVisualOnly(transform);
						MelonLogger.Msg("Trineo Burebista aplicado SOLO sobre: " + text);
					}
					catch (Exception ex)
					{
						MelonLogger.Error("Error construyendo el trineo: " + ex);
					}
				}
			}
		}

		private bool IsRealInteractiveTravoisName(string n)
		{
			if (string.Equals(n, "INTERACTIVE_Travois", StringComparison.Ordinal))
			{
				return true;
			}
			if (n.StartsWith("INTERACTIVE_Travois (", StringComparison.Ordinal))
			{
				return true;
			}
			return false;
		}

		private void ConvertVisualOnly(Transform travoisRoot)
		{
			GameObject gameObject = new GameObject("BurebistaSledVisual");
			gameObject.transform.SetParent(travoisRoot, worldPositionStays: false);
			gameObject.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			Material inheritedMaterial = FindOriginalTravoisMaterial(travoisRoot);
			HideOriginalVisualMeshesOnly(travoisRoot, gameObject.transform);
			BuildSledGeometry(gameObject.transform, inheritedMaterial);
			BuildLoadedCargo(gameObject.transform, inheritedMaterial);
		}

		private Material FindOriginalTravoisMaterial(Transform root)
		{
			Renderer[] array = root.GetComponentsInChildren<Renderer>(includeInactive: true);
			Renderer[] array2 = array;
			foreach (Renderer renderer in array2)
			{
				if (!(renderer == null) && !(renderer.sharedMaterial == null))
				{
					string text = renderer.gameObject.name ?? "";
					if (text.IndexOf("OBJ_Travois", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("TravoisBlendshape", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						return renderer.sharedMaterial;
					}
				}
			}
			array2 = array;
			foreach (Renderer renderer2 in array2)
			{
				if (renderer2 != null && renderer2.sharedMaterial != null)
				{
					return renderer2.sharedMaterial;
				}
			}
			return null;
		}

		private void HideOriginalVisualMeshesOnly(Transform root, Transform ourVisualRoot)
		{
			Renderer[] array = root.GetComponentsInChildren<Renderer>(includeInactive: true);
			foreach (Renderer renderer in array)
			{
				if (!(renderer == null) && !(renderer.transform == ourVisualRoot) && !renderer.transform.IsChildOf(ourVisualRoot))
				{
					string text = renderer.gameObject.name ?? "";
					if (text.IndexOf("OBJ_Travois", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("TravoisBlendshape", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						renderer.enabled = false;
						MelonLogger.Msg("Ocultado renderer original: " + text);
					}
				}
			}
		}

		private void BuildSledGeometry(Transform parent, Material inheritedMaterial)
		{
			CreateBeam(parent, "Runner_L", new Vector3(-0.52f, 0.03f, 0f), new Vector3(0.12f, 0.12f, 2.45f), Vector3.zero, inheritedMaterial);
			CreateBeam(parent, "Runner_R", new Vector3(0.52f, 0.03f, 0f), new Vector3(0.12f, 0.12f, 2.45f), Vector3.zero, inheritedMaterial);
			CreateBeam(parent, "RunnerTip_L", new Vector3(-0.52f, 0.17f, 1.15f), new Vector3(0.12f, 0.12f, 0.55f), new Vector3(-18f, 0f, 0f), inheritedMaterial);
			CreateBeam(parent, "RunnerTip_R", new Vector3(0.52f, 0.17f, 1.15f), new Vector3(0.12f, 0.12f, 0.55f), new Vector3(-18f, 0f, 0f), inheritedMaterial);
			for (int i = 0; i < 7; i++)
			{
				float z = -0.78f + (float)i * 0.27f;
				CreateBeam(parent, "Slat_" + i, new Vector3(0f, 0.28f, z), new Vector3(1.12f, 0.07f, 0.16f), Vector3.zero, inheritedMaterial);
			}
			CreateBeam(parent, "SideRail_L", new Vector3(-0.57f, 0.52f, -0.05f), new Vector3(0.08f, 0.08f, 1.85f), Vector3.zero, inheritedMaterial);
			CreateBeam(parent, "SideRail_R", new Vector3(0.57f, 0.52f, -0.05f), new Vector3(0.08f, 0.08f, 1.85f), Vector3.zero, inheritedMaterial);
			float[] array = new float[3] { -0.78f, 0f, 0.78f };
			for (int j = 0; j < array.Length; j++)
			{
				float z2 = array[j];
				CreateBeam(parent, "PostL_" + z2, new Vector3(-0.57f, 0.48f, z2), new Vector3(0.08f, 0.48f, 0.08f), Vector3.zero, inheritedMaterial);
				CreateBeam(parent, "PostR_" + z2, new Vector3(0.57f, 0.48f, z2), new Vector3(0.08f, 0.48f, 0.08f), Vector3.zero, inheritedMaterial);
			}
			CreateBeam(parent, "Handle_L", new Vector3(-0.42f, 0.55f, -1.45f), new Vector3(0.08f, 0.08f, 1.25f), new Vector3(12f, 0f, 0f), inheritedMaterial);
			CreateBeam(parent, "Handle_R", new Vector3(0.42f, 0.55f, -1.45f), new Vector3(0.08f, 0.08f, 1.25f), new Vector3(12f, 0f, 0f), inheritedMaterial);
			CreateBeam(parent, "FrontBar", new Vector3(0f, 0.74f, -1.95f), new Vector3(0.92f, 0.08f, 0.08f), Vector3.zero, inheritedMaterial);
		}

		private void BuildLoadedCargo(Transform parent, Material inheritedMaterial)
		{
			Material material = MakeTintedMaterial(inheritedMaterial, new Color(0.46f, 0.28f, 0.14f, 1f));
			Material material2 = MakeTintedMaterial(inheritedMaterial, new Color(0.34f, 0.27f, 0.18f, 1f));
			Material material3 = MakeTintedMaterial(inheritedMaterial, new Color(0.22f, 0.16f, 0.11f, 1f));
			Material inheritedMaterial2 = MakeTintedMaterial(inheritedMaterial, new Color(0.49f, 0.39f, 0.24f, 1f));
			Material material4 = MakeTintedMaterial(inheritedMaterial, new Color(0.25f, 0.27f, 0.28f, 1f));
			CreateCargo(parent, "HideRoll_A", PrimitiveType.Capsule, new Vector3(-0.28f, 0.68f, 0.3f), new Vector3(0.52f, 0.42f, 0.72f), new Vector3(90f, 0f, 0f), material3);
			CreateCargo(parent, "HideRoll_B", PrimitiveType.Capsule, new Vector3(0.28f, 0.69f, 0.3f), new Vector3(0.52f, 0.42f, 0.72f), new Vector3(90f, 0f, 0f), material3);
			CreateCargo(parent, "Sack_A", PrimitiveType.Sphere, new Vector3(-0.3f, 0.7f, 0.78f), new Vector3(0.6f, 0.52f, 0.72f), new Vector3(0f, 14f, 0f), material2);
			CreateCargo(parent, "Sack_B", PrimitiveType.Sphere, new Vector3(0.3f, 0.72f, 0.8f), new Vector3(0.6f, 0.52f, 0.72f), new Vector3(0f, -11f, 0f), material2);
			CreateCargo(parent, "SupplyCrate", PrimitiveType.Cube, new Vector3(0f, 0.67f, -0.25f), new Vector3(0.68f, 0.5f, 0.5f), new Vector3(0f, 4f, 0f), material);
			CreateCargo(parent, "ToolBox", PrimitiveType.Cube, new Vector3(-0.34f, 0.57f, -0.7f), new Vector3(0.44f, 0.23f, 0.3f), new Vector3(0f, -6f, 0f), material4);
			CreateCargo(parent, "RolledTarp", PrimitiveType.Cylinder, new Vector3(0.34f, 0.61f, -0.7f), new Vector3(0.19f, 0.4f, 0.19f), new Vector3(0f, 0f, 90f), material3);
			CreateCargo(parent, "ToolHandle", PrimitiveType.Cube, new Vector3(0.43f, 0.82f, -0.05f), new Vector3(0.055f, 0.055f, 1.08f), new Vector3(0f, 18f, -3f), material);
			CreateCargo(parent, "ToolHead", PrimitiveType.Cube, new Vector3(0.57f, 0.86f, 0.4f), new Vector3(0.24f, 0.1f, 0.07f), new Vector3(0f, 18f, -3f), material4);
			CreateBeam(parent, "Lashing_A", new Vector3(0f, 0.91f, 0.28f), new Vector3(1.04f, 0.028f, 0.028f), new Vector3(0f, 0f, 10f), inheritedMaterial2);
			CreateBeam(parent, "Lashing_B", new Vector3(0f, 0.86f, -0.24f), new Vector3(1f, 0.028f, 0.028f), new Vector3(0f, 0f, -9f), inheritedMaterial2);
			CreateBeam(parent, "Lashing_C", new Vector3(0f, 0.91f, 0.75f), new Vector3(1f, 0.028f, 0.028f), new Vector3(0f, 0f, 7f), inheritedMaterial2);
		}

		private Material MakeTintedMaterial(Material source, Color tint)
		{
			Material material = null;
			try
			{
				material = ((!(source != null)) ? new Material(Shader.Find("Standard")) : new Material(source));
				if (material != null)
				{
					if (material.HasProperty("_BaseColor"))
					{
						material.SetColor("_BaseColor", tint);
					}
					if (material.HasProperty("_Color"))
					{
						material.SetColor("_Color", tint);
					}
				}
			}
			catch
			{
			}
			return material;
		}

		private GameObject CreateCargo(Transform parent, string name, PrimitiveType primitive, Vector3 localPosition, Vector3 localScale, Vector3 localEuler, Material material)
		{
			GameObject gameObject = GameObject.CreatePrimitive(primitive);
			gameObject.name = name;
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			gameObject.transform.localPosition = localPosition;
			gameObject.transform.localScale = localScale;
			gameObject.transform.localEulerAngles = localEuler;
			Collider component = gameObject.GetComponent<Collider>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			Renderer component2 = gameObject.GetComponent<Renderer>();
			if (component2 != null && material != null)
			{
				component2.sharedMaterial = material;
			}
			return gameObject;
		}

		private GameObject CreateBeam(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Vector3 localEuler, Material inheritedMaterial)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			gameObject.name = name;
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			gameObject.transform.localPosition = localPosition;
			gameObject.transform.localScale = localScale;
			gameObject.transform.localEulerAngles = localEuler;
			Collider component = gameObject.GetComponent<Collider>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			Renderer component2 = gameObject.GetComponent<Renderer>();
			if (component2 != null && inheritedMaterial != null)
			{
				component2.sharedMaterial = inheritedMaterial;
			}
			return gameObject;
		}
	}
}
