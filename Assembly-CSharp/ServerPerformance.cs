using System;
using System.IO;
using System.Linq;
using Facepunch;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;

// Token: 0x0200074A RID: 1866
public class ServerPerformance : BaseMonoBehaviour
{
	// Token: 0x0600340C RID: 13324 RVA: 0x00141324 File Offset: 0x0013F524
	private void Start()
	{
		if (!Profiler.supported)
		{
			return;
		}
		if (!CommandLine.HasSwitch("-perf"))
		{
			return;
		}
		this.fileName = "perfdata." + DateTime.Now.ToString() + ".txt";
		this.fileName = this.fileName.Replace('\\', '-');
		this.fileName = this.fileName.Replace('/', '-');
		this.fileName = this.fileName.Replace(' ', '_');
		this.fileName = this.fileName.Replace(':', '.');
		this.lastFrame = Time.frameCount;
		File.WriteAllText(this.fileName, "MemMono,MemUnity,Frame,PlayerCount,Sleepers,CollidersDisabled,BehavioursDisabled,GameObjects,Colliders,RigidBodies,BuildingBlocks,nwSend,nwRcv,cnInit,cnApp,cnRej,deaths,spawns,poschange\r\n");
		base.InvokeRepeating(new Action(this.WriteLine), 1f, 60f);
	}

	// Token: 0x0600340D RID: 13325 RVA: 0x001413F4 File Offset: 0x0013F5F4
	private void WriteLine()
	{
		Rust.GC.Collect();
		uint monoUsedSize = Profiler.GetMonoUsedSize();
		uint usedHeapSize = Profiler.usedHeapSize;
		int count = BasePlayer.activePlayerList.Count;
		int count2 = BasePlayer.sleepingPlayerList.Count;
		int num = UnityEngine.Object.FindObjectsOfType<GameObject>().Length;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = Time.frameCount - this.lastFrame;
		File.AppendAllText(this.fileName, string.Concat(new object[]
		{
			monoUsedSize,
			",",
			usedHeapSize,
			",",
			num7,
			",",
			count,
			",",
			count2,
			",",
			NetworkSleep.totalCollidersDisabled,
			",",
			NetworkSleep.totalBehavioursDisabled,
			",",
			num,
			",",
			UnityEngine.Object.FindObjectsOfType<Collider>().Length,
			",",
			UnityEngine.Object.FindObjectsOfType<Rigidbody>().Length,
			",",
			UnityEngine.Object.FindObjectsOfType<BuildingBlock>().Length,
			",",
			num2,
			",",
			num3,
			",",
			num4,
			",",
			num5,
			",",
			num6,
			",",
			ServerPerformance.deaths,
			",",
			ServerPerformance.spawns,
			",",
			ServerPerformance.position_changes,
			"\r\n"
		}));
		this.lastFrame = Time.frameCount;
		ServerPerformance.deaths = 0UL;
		ServerPerformance.spawns = 0UL;
		ServerPerformance.position_changes = 0UL;
	}

	// Token: 0x0600340E RID: 13326 RVA: 0x00141614 File Offset: 0x0013F814
	public static void DoReport()
	{
		string text = "report." + DateTime.Now.ToString() + ".txt";
		text = text.Replace('\\', '-');
		text = text.Replace('/', '-');
		text = text.Replace(' ', '_');
		text = text.Replace(':', '.');
		File.WriteAllText(text, "Report Generated " + DateTime.Now.ToString() + "\r\n");
		string text2 = text;
		string text3 = "All Objects";
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType<Transform>();
		ServerPerformance.ComponentReport(text2, text3, array);
		string text4 = text;
		string text5 = "Entities";
		array = UnityEngine.Object.FindObjectsOfType<BaseEntity>();
		ServerPerformance.ComponentReport(text4, text5, array);
		string text6 = text;
		string text7 = "Rigidbodies";
		array = UnityEngine.Object.FindObjectsOfType<Rigidbody>();
		ServerPerformance.ComponentReport(text6, text7, array);
		string text8 = text;
		string text9 = "Disabled Colliders";
		array = (from x in UnityEngine.Object.FindObjectsOfType<Collider>()
			where !x.enabled
			select x).ToArray<Collider>();
		ServerPerformance.ComponentReport(text8, text9, array);
		string text10 = text;
		string text11 = "Enabled Colliders";
		array = (from x in UnityEngine.Object.FindObjectsOfType<Collider>()
			where x.enabled
			select x).ToArray<Collider>();
		ServerPerformance.ComponentReport(text10, text11, array);
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			SingletonComponent<SpawnHandler>.Instance.DumpReport(text);
		}
	}

	// Token: 0x0600340F RID: 13327 RVA: 0x00141750 File Offset: 0x0013F950
	public static string WorkoutPrefabName(GameObject obj)
	{
		if (obj == null)
		{
			return "null";
		}
		string text = (obj.activeSelf ? "" : " (inactive)");
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity)
		{
			return baseEntity.PrefabName + text;
		}
		return obj.name + text;
	}

	// Token: 0x06003410 RID: 13328 RVA: 0x001417AC File Offset: 0x0013F9AC
	public static void ComponentReport(string filename, string Title, UnityEngine.Object[] objects)
	{
		File.AppendAllText(filename, "\r\n\r\n" + Title + ":\r\n\r\n");
		foreach (IGrouping<string, UnityEngine.Object> grouping in from x in objects
			group x by ServerPerformance.WorkoutPrefabName((x as Component).gameObject) into x
			orderby x.Count<UnityEngine.Object>() descending
			select x)
		{
			File.AppendAllText(filename, string.Concat(new object[]
			{
				"\t",
				ServerPerformance.WorkoutPrefabName((grouping.ElementAt(0) as Component).gameObject),
				" - ",
				grouping.Count<UnityEngine.Object>(),
				"\r\n"
			}));
		}
		File.AppendAllText(filename, "\r\nTotal: " + objects.Count<UnityEngine.Object>() + "\r\n\r\n\r\n");
	}

	// Token: 0x04002A88 RID: 10888
	public static ulong deaths;

	// Token: 0x04002A89 RID: 10889
	public static ulong spawns;

	// Token: 0x04002A8A RID: 10890
	public static ulong position_changes;

	// Token: 0x04002A8B RID: 10891
	private string fileName;

	// Token: 0x04002A8C RID: 10892
	private int lastFrame;
}
