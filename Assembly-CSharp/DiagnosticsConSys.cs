using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Network;
using UnityEngine;

// Token: 0x02000307 RID: 775
[ConsoleSystem.Factory("global")]
public class DiagnosticsConSys : ConsoleSystem
{
	// Token: 0x06001EBD RID: 7869 RVA: 0x000D0AA0 File Offset: 0x000CECA0
	private static void DumpAnimators(string targetFolder)
	{
		Animator[] array = UnityEngine.Object.FindObjectsOfType<Animator>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All animators");
		stringBuilder.AppendLine();
		foreach (Animator animator in array)
		{
			stringBuilder.AppendFormat("{1}\t{0}", animator.transform.GetRecursiveName(""), animator.enabled);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Animators.List.txt", stringBuilder.ToString());
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("All animators - grouped by object name");
		stringBuilder2.AppendLine();
		foreach (IGrouping<string, Animator> grouping in from x in array
			group x by x.transform.GetRecursiveName("") into x
			orderby x.Count<Animator>() descending
			select x)
		{
			stringBuilder2.AppendFormat("{1:N0}\t{0}", grouping.First<Animator>().transform.GetRecursiveName(""), grouping.Count<Animator>());
			stringBuilder2.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Animators.Counts.txt", stringBuilder2.ToString());
		StringBuilder stringBuilder3 = new StringBuilder();
		stringBuilder3.AppendLine("All animators - grouped by enabled/disabled");
		stringBuilder3.AppendLine();
		foreach (IGrouping<string, Animator> grouping2 in from x in array
			group x by x.transform.GetRecursiveName(x.enabled ? "" : " (DISABLED)") into x
			orderby x.Count<Animator>() descending
			select x)
		{
			stringBuilder3.AppendFormat("{1:N0}\t{0}", grouping2.First<Animator>().transform.GetRecursiveName(grouping2.First<Animator>().enabled ? "" : " (DISABLED)"), grouping2.Count<Animator>());
			stringBuilder3.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Animators.Counts.Enabled.txt", stringBuilder3.ToString());
	}

	// Token: 0x06001EBE RID: 7870 RVA: 0x000D0D10 File Offset: 0x000CEF10
	private static void DumpEntities(string targetFolder)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All entities");
		stringBuilder.AppendLine();
		foreach (BaseNetworkable baseNetworkable in BaseNetworkable.serverEntities)
		{
			StringBuilder stringBuilder2 = stringBuilder;
			string text = "{1}\t{0}";
			object prefabName = baseNetworkable.PrefabName;
			Networkable net = baseNetworkable.net;
			stringBuilder2.AppendFormat(text, prefabName, ((net != null) ? net.ID : default(NetworkableId)).Value);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Entity.SV.List.txt", stringBuilder.ToString());
		StringBuilder stringBuilder3 = new StringBuilder();
		stringBuilder3.AppendLine("All entities");
		stringBuilder3.AppendLine();
		foreach (IGrouping<uint, BaseNetworkable> grouping in from x in BaseNetworkable.serverEntities
			group x by x.prefabID into x
			orderby x.Count<BaseNetworkable>() descending
			select x)
		{
			stringBuilder3.AppendFormat("{1:N0}\t{0}", grouping.First<BaseNetworkable>().PrefabName, grouping.Count<BaseNetworkable>());
			stringBuilder3.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Entity.SV.Counts.txt", stringBuilder3.ToString());
		StringBuilder stringBuilder4 = new StringBuilder();
		stringBuilder4.AppendLine("Saved entities");
		stringBuilder4.AppendLine();
		foreach (IGrouping<uint, BaseEntity> grouping2 in from x in BaseEntity.saveList
			group x by x.prefabID into x
			orderby x.Count<BaseEntity>() descending
			select x)
		{
			stringBuilder4.AppendFormat("{1:N0}\t{0}", grouping2.First<BaseEntity>().PrefabName, grouping2.Count<BaseEntity>());
			stringBuilder4.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Entity.SV.Savelist.Counts.txt", stringBuilder4.ToString());
	}

	// Token: 0x06001EBF RID: 7871 RVA: 0x000D0F88 File Offset: 0x000CF188
	private static void DumpLODGroups(string targetFolder)
	{
		DiagnosticsConSys.DumpLODGroupTotals(targetFolder);
	}

	// Token: 0x06001EC0 RID: 7872 RVA: 0x000D0F90 File Offset: 0x000CF190
	private static void DumpLODGroupTotals(string targetFolder)
	{
		IEnumerable<LODGroup> enumerable = UnityEngine.Object.FindObjectsOfType<LODGroup>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("LODGroups");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, LODGroup> grouping in from x in enumerable
			group x by x.transform.GetRecursiveName("") into x
			orderby x.Count<LODGroup>() descending
			select x)
		{
			stringBuilder.AppendFormat("{1:N0}\t{0}", grouping.Key, grouping.Count<LODGroup>());
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "LODGroups.Objects.txt", stringBuilder.ToString());
	}

	// Token: 0x06001EC1 RID: 7873 RVA: 0x000D1070 File Offset: 0x000CF270
	private static void DumpNetwork(string targetFolder)
	{
		if (Net.sv.IsConnected())
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Server Network Statistics");
			stringBuilder.AppendLine();
			stringBuilder.Append(Net.sv.GetDebug(null).Replace("\n", "\r\n"));
			stringBuilder.AppendLine();
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				stringBuilder.AppendLine("Name: " + basePlayer.displayName);
				stringBuilder.AppendLine("SteamID: " + basePlayer.userID);
				stringBuilder.Append((basePlayer.net == null) ? "INVALID - NET IS NULL" : Net.sv.GetDebug(basePlayer.net.connection).Replace("\n", "\r\n"));
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
			}
			DiagnosticsConSys.WriteTextToFile(targetFolder + "Network.Server.txt", stringBuilder.ToString());
		}
	}

	// Token: 0x06001EC2 RID: 7874 RVA: 0x000D11B0 File Offset: 0x000CF3B0
	private static void DumpObjects(string targetFolder)
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType<UnityEngine.Object>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All active UnityEngine.Object, ordered by count");
		stringBuilder.AppendLine();
		foreach (IGrouping<Type, UnityEngine.Object> grouping in from x in array
			group x by x.GetType() into x
			orderby x.Count<UnityEngine.Object>() descending
			select x)
		{
			stringBuilder.AppendFormat("{1:N0}\t{0}", grouping.First<UnityEngine.Object>().GetType().Name, grouping.Count<UnityEngine.Object>());
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Object.Count.txt", stringBuilder.ToString());
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("All active UnityEngine.ScriptableObject, ordered by count");
		stringBuilder2.AppendLine();
		foreach (IGrouping<Type, UnityEngine.Object> grouping2 in from x in array
			where x is ScriptableObject
			group x by x.GetType() into x
			orderby x.Count<UnityEngine.Object>() descending
			select x)
		{
			stringBuilder2.AppendFormat("{1:N0}\t{0}", grouping2.First<UnityEngine.Object>().GetType().Name, grouping2.Count<UnityEngine.Object>());
			stringBuilder2.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.ScriptableObject.Count.txt", stringBuilder2.ToString());
	}

	// Token: 0x06001EC3 RID: 7875 RVA: 0x000D13A0 File Offset: 0x000CF5A0
	private static void DumpPhysics(string targetFolder)
	{
		DiagnosticsConSys.DumpTotals(targetFolder);
		DiagnosticsConSys.DumpColliders(targetFolder);
		DiagnosticsConSys.DumpRigidBodies(targetFolder);
	}

	// Token: 0x06001EC4 RID: 7876 RVA: 0x000D13B4 File Offset: 0x000CF5B4
	private static void DumpTotals(string targetFolder)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Physics Information");
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Total Colliders:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<Collider>().Count<Collider>());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Active Colliders:\t{0:N0}", (from x in UnityEngine.Object.FindObjectsOfType<Collider>()
			where x.enabled
			select x).Count<Collider>());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Total RigidBodys:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<Rigidbody>().Count<Rigidbody>());
		stringBuilder.AppendLine();
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Mesh Colliders:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<MeshCollider>().Count<MeshCollider>());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Box Colliders:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<BoxCollider>().Count<BoxCollider>());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Sphere Colliders:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<SphereCollider>().Count<SphereCollider>());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Capsule Colliders:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<CapsuleCollider>().Count<CapsuleCollider>());
		stringBuilder.AppendLine();
		DiagnosticsConSys.WriteTextToFile(targetFolder + "Physics.txt", stringBuilder.ToString());
	}

	// Token: 0x06001EC5 RID: 7877 RVA: 0x000D150C File Offset: 0x000CF70C
	private static void DumpColliders(string targetFolder)
	{
		IEnumerable<Collider> enumerable = UnityEngine.Object.FindObjectsOfType<Collider>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Physics Colliders");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, Collider> grouping in from x in enumerable
			group x by x.transform.GetRecursiveName("") into x
			orderby x.Count<Collider>() descending
			select x)
		{
			StringBuilder stringBuilder2 = stringBuilder;
			string text = "{1:N0}\t{0} ({2:N0} triggers) ({3:N0} enabled)";
			object[] array = new object[4];
			array[0] = grouping.Key;
			array[1] = grouping.Count<Collider>();
			array[2] = grouping.Count((Collider x) => x.isTrigger);
			array[3] = grouping.Count((Collider x) => x.enabled);
			stringBuilder2.AppendFormat(text, array);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "Physics.Colliders.Objects.txt", stringBuilder.ToString());
	}

	// Token: 0x06001EC6 RID: 7878 RVA: 0x000D1658 File Offset: 0x000CF858
	private static void DumpRigidBodies(string targetFolder)
	{
		IEnumerable<Rigidbody> enumerable = UnityEngine.Object.FindObjectsOfType<Rigidbody>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("RigidBody");
		stringBuilder.AppendLine();
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("RigidBody");
		stringBuilder2.AppendLine();
		foreach (IGrouping<string, Rigidbody> grouping in from x in enumerable
			group x by x.transform.GetRecursiveName("") into x
			orderby x.Count<Rigidbody>() descending
			select x)
		{
			StringBuilder stringBuilder3 = stringBuilder;
			string text = "{1:N0}\t{0} ({2:N0} awake) ({3:N0} kinematic) ({4:N0} non-discrete)";
			object[] array = new object[5];
			array[0] = grouping.Key;
			array[1] = grouping.Count<Rigidbody>();
			array[2] = grouping.Count((Rigidbody x) => !x.IsSleeping());
			array[3] = grouping.Count((Rigidbody x) => x.isKinematic);
			array[4] = grouping.Count((Rigidbody x) => x.collisionDetectionMode > CollisionDetectionMode.Discrete);
			stringBuilder3.AppendFormat(text, array);
			stringBuilder.AppendLine();
			foreach (Rigidbody rigidbody in grouping)
			{
				stringBuilder2.AppendFormat("{0} -{1}{2}{3}", new object[]
				{
					grouping.Key,
					rigidbody.isKinematic ? " KIN" : "",
					rigidbody.IsSleeping() ? " SLEEP" : "",
					rigidbody.useGravity ? " GRAVITY" : ""
				});
				stringBuilder2.AppendLine();
				stringBuilder2.AppendFormat("Mass: {0}\tVelocity: {1}\tsleepThreshold: {2}", rigidbody.mass, rigidbody.velocity, rigidbody.sleepThreshold);
				stringBuilder2.AppendLine();
				stringBuilder2.AppendLine();
			}
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "Physics.RigidBody.Objects.txt", stringBuilder.ToString());
		DiagnosticsConSys.WriteTextToFile(targetFolder + "Physics.RigidBody.All.txt", stringBuilder2.ToString());
	}

	// Token: 0x06001EC7 RID: 7879 RVA: 0x000D18FC File Offset: 0x000CFAFC
	private static void DumpGameObjects(string targetFolder)
	{
		Transform[] rootObjects = TransformUtil.GetRootObjects();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All active game objects");
		stringBuilder.AppendLine();
		foreach (Transform transform in rootObjects)
		{
			DiagnosticsConSys.DumpGameObjectRecursive(stringBuilder, transform, 0, false);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "GameObject.Hierarchy.txt", stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All active game objects including components");
		stringBuilder.AppendLine();
		foreach (Transform transform2 in rootObjects)
		{
			DiagnosticsConSys.DumpGameObjectRecursive(stringBuilder, transform2, 0, true);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "GameObject.Hierarchy.Components.txt", stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Root gameobjects, grouped by name, ordered by the total number of objects excluding children");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, Transform> grouping in from x in rootObjects
			group x by x.name into x
			orderby x.Count<Transform>() descending
			select x)
		{
			Transform transform3 = grouping.First<Transform>();
			stringBuilder.AppendFormat("{1:N0}\t{0}", transform3.name, grouping.Count<Transform>());
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "GameObject.Count.txt", stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Root gameobjects, grouped by name, ordered by the total number of objects including children");
		stringBuilder.AppendLine();
		foreach (KeyValuePair<Transform, int> keyValuePair in from x in rootObjects
			group x by x.name into x
			select new KeyValuePair<Transform, int>(x.First<Transform>(), x.Sum((Transform y) => y.GetAllChildren().Count)) into x
			orderby x.Value descending
			select x)
		{
			stringBuilder.AppendFormat("{1:N0}\t{0}", keyValuePair.Key.name, keyValuePair.Value);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "GameObject.Count.Children.txt", stringBuilder.ToString());
	}

	// Token: 0x06001EC8 RID: 7880 RVA: 0x000D1B98 File Offset: 0x000CFD98
	private static void DumpGameObjectRecursive(StringBuilder str, Transform tx, int indent, bool includeComponents = false)
	{
		if (tx == null)
		{
			return;
		}
		for (int i = 0; i < indent; i++)
		{
			str.Append(" ");
		}
		str.AppendFormat("{0} {1:N0}", tx.name, tx.GetComponents<Component>().Length - 1);
		str.AppendLine();
		if (includeComponents)
		{
			foreach (Component component in tx.GetComponents<Component>())
			{
				if (!(component is Transform))
				{
					for (int k = 0; k < indent + 1; k++)
					{
						str.Append(" ");
					}
					str.AppendFormat("[c] {0}", (component == null) ? "NULL" : component.GetType().ToString());
					str.AppendLine();
				}
			}
		}
		for (int l = 0; l < tx.childCount; l++)
		{
			DiagnosticsConSys.DumpGameObjectRecursive(str, tx.GetChild(l), indent + 2, includeComponents);
		}
	}

	// Token: 0x06001EC9 RID: 7881 RVA: 0x000D1C88 File Offset: 0x000CFE88
	[ServerVar]
	[ClientVar]
	public static void dump(ConsoleSystem.Arg args)
	{
		if (Directory.Exists("diagnostics"))
		{
			Directory.CreateDirectory("diagnostics");
		}
		int num = 1;
		while (Directory.Exists("diagnostics/" + num))
		{
			num++;
		}
		Directory.CreateDirectory("diagnostics/" + num);
		string text = "diagnostics/" + num + "/";
		DiagnosticsConSys.DumpLODGroups(text);
		DiagnosticsConSys.DumpSystemInformation(text);
		DiagnosticsConSys.DumpGameObjects(text);
		DiagnosticsConSys.DumpObjects(text);
		DiagnosticsConSys.DumpEntities(text);
		DiagnosticsConSys.DumpNetwork(text);
		DiagnosticsConSys.DumpPhysics(text);
		DiagnosticsConSys.DumpAnimators(text);
	}

	// Token: 0x06001ECA RID: 7882 RVA: 0x000D1D25 File Offset: 0x000CFF25
	private static void DumpSystemInformation(string targetFolder)
	{
		DiagnosticsConSys.WriteTextToFile(targetFolder + "System.Info.txt", SystemInfoGeneralText.currentInfo);
	}

	// Token: 0x06001ECB RID: 7883 RVA: 0x000D1D3C File Offset: 0x000CFF3C
	private static void WriteTextToFile(string file, string text)
	{
		File.WriteAllText(file, text);
	}
}
