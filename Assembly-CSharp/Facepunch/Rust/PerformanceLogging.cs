using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ConVar;
using Epic.OnlineServices;
using Epic.OnlineServices.Version;
using Network;
using UnityEngine;

namespace Facepunch.Rust
{
	// Token: 0x02000B0B RID: 2827
	public class PerformanceLogging
	{
		// Token: 0x060044F4 RID: 17652 RVA: 0x00193A74 File Offset: 0x00191C74
		public PerformanceLogging(bool client)
		{
			this.isClient = client;
		}

		// Token: 0x060044F5 RID: 17653 RVA: 0x00193B36 File Offset: 0x00191D36
		private TimeSpan GetLagSpikeThreshold()
		{
			if (!this.isClient)
			{
				return TimeSpan.FromMilliseconds(200.0);
			}
			return TimeSpan.FromMilliseconds(100.0);
		}

		// Token: 0x060044F6 RID: 17654 RVA: 0x00193B60 File Offset: 0x00191D60
		public void OnFrame()
		{
			TimeSpan elapsed = this.frameWatch.Elapsed;
			this.Frametimes.Add(elapsed);
			this.frameWatch.Restart();
			DateTime utcNow = DateTime.UtcNow;
			int num = System.GC.CollectionCount(0);
			bool flag = this.lastFrameGC != num;
			this.lastFrameGC = num;
			if (flag)
			{
				this.garbageCollections.Add(new PerformanceLogging.GarbageCollect
				{
					FrameIndex = this.Frametimes.Count - 1,
					Time = elapsed
				});
			}
			if (elapsed > this.GetLagSpikeThreshold())
			{
				this.lagSpikes.Add(new PerformanceLogging.LagSpike
				{
					FrameIndex = this.Frametimes.Count - 1,
					Time = elapsed,
					WasGC = flag
				});
			}
			if (utcNow > this.nextFlushTime)
			{
				if (this.nextFlushTime == default(DateTime))
				{
					this.nextFlushTime = DateTime.UtcNow.Add(this.GetFlushInterval());
					return;
				}
				this.Flush();
			}
		}

		// Token: 0x060044F7 RID: 17655 RVA: 0x00193C70 File Offset: 0x00191E70
		public void Flush()
		{
			PerformanceLogging.<>c__DisplayClass31_0 CS$<>8__locals1 = new PerformanceLogging.<>c__DisplayClass31_0();
			CS$<>8__locals1.<>4__this = this;
			this.nextFlushTime = DateTime.UtcNow.Add(this.GetFlushInterval());
			if (!this.isClient && BasePlayer.activePlayerList.Count == 0 && !Analytics.Azure.Stats)
			{
				this.ResetMeasurements();
				return;
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			CS$<>8__locals1.record = EventRecord.New(this.isClient ? "client_performance" : "server_performance", !this.isClient);
			CS$<>8__locals1.record.AddField("lag_spike_count", this.lagSpikes.Count).AddField("lag_spike_threshold", this.GetLagSpikeThreshold()).AddField("gc_count", this.garbageCollections.Count)
				.AddField("ram_managed", System.GC.GetTotalMemory(false))
				.AddField("ram_total", SystemInfoEx.systemMemoryUsed)
				.AddField("total_session_id", this.totalSessionId.ToString("N"))
				.AddField("uptime", (int)UnityEngine.Time.realtimeSinceStartup)
				.AddField("map_url", global::World.Url)
				.AddField("world_size", global::World.Size)
				.AddField("world_seed", global::World.Seed)
				.AddField("active_scene", LevelManager.CurrentLevelName);
			if (!this.isClient && !this.isClient)
			{
				int num = ((Network.Net.sv == null) ? 0 : ((int)Network.Net.sv.GetStat(null, BaseNetwork.StatTypeLong.BytesReceived_LastSecond)));
				int num2 = ((Network.Net.sv == null) ? 0 : ((int)Network.Net.sv.GetStat(null, BaseNetwork.StatTypeLong.BytesSent_LastSecond)));
				CS$<>8__locals1.record.AddField("is_official", ConVar.Server.official && ConVar.Server.stats).AddField("bot_count", BasePlayer.bots.Count).AddField("player_count", BasePlayer.activePlayerList.Count)
					.AddField("max_players", ConVar.Server.maxplayers)
					.AddField("ent_count", BaseNetworkable.serverEntities.Count)
					.AddField("hostname", ConVar.Server.hostname)
					.AddField("net_in", num)
					.AddField("net_out", num2);
			}
			if (!this.isClient)
			{
				try
				{
					if (!this.hasOxideType)
					{
						this.oxideType = Type.GetType("Oxide.Core.Interface,Oxide.Core");
						this.hasOxideType = true;
					}
					if (this.oxideType != null)
					{
						CS$<>8__locals1.record.AddField("is_oxide", true);
						PropertyInfo property = this.oxideType.GetProperty("Oxide", BindingFlags.Static | BindingFlags.Public);
						object obj = ((property != null) ? property.GetValue(null) : null);
						if (obj != null)
						{
							PropertyInfo property2 = obj.GetType().GetProperty("RootPluginManager", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
							object obj2 = ((property2 != null) ? property2.GetValue(obj) : null);
							if (obj2 != null)
							{
								List<PerformanceLogging.PluginInfo> list = new List<PerformanceLogging.PluginInfo>();
								MethodInfo method = obj2.GetType().GetMethod("GetPlugins");
								foreach (object obj3 in (((method != null) ? method.Invoke(obj2, null) : null) as IEnumerable))
								{
									if (obj3 != null)
									{
										PropertyInfo property3 = obj3.GetType().GetProperty("Name");
										string text = ((property3 != null) ? property3.GetValue(obj3) : null) as string;
										PropertyInfo property4 = obj3.GetType().GetProperty("Author");
										string text2 = ((property4 != null) ? property4.GetValue(obj3) : null) as string;
										PropertyInfo property5 = obj3.GetType().GetProperty("Version");
										string text3;
										if (property5 == null)
										{
											text3 = null;
										}
										else
										{
											object value = property5.GetValue(obj3);
											text3 = ((value != null) ? value.ToString() : null);
										}
										string text4 = text3;
										list.Add(new PerformanceLogging.PluginInfo
										{
											Name = text,
											Author = text2,
											Version = text4
										});
									}
								}
								CS$<>8__locals1.record.AddObject("oxide_plugins", list);
								CS$<>8__locals1.record.AddField("oxide_plugin_count", list.Count);
							}
						}
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogError(string.Format("Failed to get oxide when flushing server performance: {0}", ex));
				}
				try
				{
					List<PerformanceLogging.ProcessInfo> list2 = new List<PerformanceLogging.ProcessInfo>();
					Process[] processes = Process.GetProcesses();
					Process currentProcess = Process.GetCurrentProcess();
					foreach (Process process in processes)
					{
						try
						{
							if (currentProcess.Id != process.Id)
							{
								if (process.ProcessName.Contains("RustDedicated"))
								{
									list2.Add(new PerformanceLogging.ProcessInfo
									{
										Name = process.ProcessName,
										WorkingSet = process.WorkingSet64
									});
								}
							}
						}
						catch (Exception ex2)
						{
							if (!(ex2 is InvalidOperationException))
							{
								UnityEngine.Debug.LogWarning(string.Format("Failed to get memory from process when flushing performance info: {0}", ex2));
								list2.Add(new PerformanceLogging.ProcessInfo
								{
									Name = process.ProcessName,
									WorkingSet = -1L
								});
							}
						}
					}
					CS$<>8__locals1.record.AddObject("other_servers", list2);
					CS$<>8__locals1.record.AddField("other_server_count", list2.Count);
				}
				catch (Exception ex3)
				{
					UnityEngine.Debug.LogError(string.Format("Failed to log processes when flushing performance info: {0}", ex3));
				}
			}
			if (!this.isClient)
			{
				IEnumerable<HarmonyModInfo> harmonyMods = HarmonyLoader.GetHarmonyMods();
				CS$<>8__locals1.record.AddObject("harmony_mods", harmonyMods);
				CS$<>8__locals1.record.AddField("harmony_mod_count", harmonyMods.Count<HarmonyModInfo>());
			}
			string text5;
			using (SHA256 sha = SHA256.Create())
			{
				text5 = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(SystemInfo.deviceUniqueIdentifier)));
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["device_name"] = SystemInfo.deviceName;
			dictionary["device_hash"] = text5;
			dictionary["gpu_name"] = SystemInfo.graphicsDeviceName;
			string text6 = "gpu_ram";
			int i = SystemInfo.graphicsMemorySize;
			dictionary[text6] = i.ToString();
			dictionary["gpu_vendor"] = SystemInfo.graphicsDeviceVendor;
			dictionary["gpu_version"] = SystemInfo.graphicsDeviceVersion;
			string text7 = "cpu_cores";
			i = SystemInfo.processorCount;
			dictionary[text7] = i.ToString();
			string text8 = "cpu_frequency";
			i = SystemInfo.processorFrequency;
			dictionary[text8] = i.ToString();
			dictionary["cpu_name"] = SystemInfo.processorType.Trim();
			string text9 = "system_memory";
			i = SystemInfo.systemMemorySize;
			dictionary[text9] = i.ToString();
			dictionary["os"] = SystemInfo.operatingSystem;
			Dictionary<string, string> dictionary2 = dictionary;
			Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
			dictionary3["unity"] = Application.unityVersion ?? "editor";
			string text10 = "changeset";
			BuildInfo buildInfo = BuildInfo.Current;
			dictionary3[text10] = ((buildInfo != null) ? buildInfo.Scm.ChangeId : null) ?? "editor";
			string text11 = "branch";
			BuildInfo buildInfo2 = BuildInfo.Current;
			dictionary3[text11] = ((buildInfo2 != null) ? buildInfo2.Scm.Branch : null) ?? "editor";
			string text12 = "network_version";
			i = 2397;
			dictionary3[text12] = i.ToString();
			Dictionary<string, string> dictionary4 = dictionary3;
			Utf8String version = VersionInterface.GetVersion();
			dictionary4["eos_sdk"] = ((version != null) ? version.ToString() : null) ?? "disabled";
			CS$<>8__locals1.record.AddObject("hardware", dictionary2).AddObject("application", dictionary4);
			stopwatch.Stop();
			CS$<>8__locals1.record.AddField("flush_ms", stopwatch.ElapsedMilliseconds);
			CS$<>8__locals1.frametimes = this.Frametimes;
			CS$<>8__locals1.ping = this.PingHistory;
			Task.Run(delegate
			{
				PerformanceLogging.<>c__DisplayClass31_0.<<Flush>b__0>d <<Flush>b__0>d;
				<<Flush>b__0>d.<>4__this = CS$<>8__locals1;
				<<Flush>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<Flush>b__0>d.<>1__state = -1;
				AsyncTaskMethodBuilder <>t__builder = <<Flush>b__0>d.<>t__builder;
				<>t__builder.Start<PerformanceLogging.<>c__DisplayClass31_0.<<Flush>b__0>d>(ref <<Flush>b__0>d);
				return <<Flush>b__0>d.<>t__builder.Task;
			});
			this.ResetMeasurements();
		}

		// Token: 0x060044F8 RID: 17656 RVA: 0x00194480 File Offset: 0x00192680
		private TimeSpan GetFlushInterval()
		{
			if (this.isClient)
			{
				return TimeSpan.FromHours(1.0);
			}
			if (Analytics.Azure.Stats)
			{
				return this.ServerInterval;
			}
			return this.PublicServerInterval;
		}

		// Token: 0x060044F9 RID: 17657 RVA: 0x001944B0 File Offset: 0x001926B0
		private void ResetMeasurements()
		{
			this.nextFlushTime = DateTime.UtcNow.Add(this.GetFlushInterval());
			if (this.Frametimes.Count == 0)
			{
				return;
			}
			PerformanceLogging.PerformancePool performancePool;
			while (this.pool.TryDequeue(out performancePool))
			{
				Pool.FreeList<TimeSpan>(ref performancePool.Frametimes);
				Pool.FreeList<int>(ref performancePool.Ping);
			}
			this.Frametimes = Pool.GetList<TimeSpan>();
			this.PingHistory = Pool.GetList<int>();
			this.garbageCollections.Clear();
		}

		// Token: 0x060044FA RID: 17658 RVA: 0x0019452C File Offset: 0x0019272C
		private Task ProcessPerformanceData(EventRecord record, List<TimeSpan> frametimes, List<int> ping)
		{
			if (frametimes.Count <= 1)
			{
				return Task.CompletedTask;
			}
			this.sortedList.Clear();
			this.sortedList.AddRange(frametimes);
			this.sortedList.Sort();
			int count = frametimes.Count;
			Mathf.Max(1, frametimes.Count / 100);
			Mathf.Max(1, frametimes.Count / 1000);
			TimeSpan timeSpan = default(TimeSpan);
			for (int i = 0; i < count; i++)
			{
				TimeSpan timeSpan2 = this.sortedList[i];
				timeSpan += timeSpan2;
			}
			double frametime_average = timeSpan.TotalMilliseconds / (double)count;
			double num = Math.Sqrt(this.sortedList.Sum((TimeSpan x) => Math.Pow(x.TotalMilliseconds - frametime_average, 2.0)) / (double)this.sortedList.Count - 1.0);
			record.AddField("total_time", timeSpan).AddField("frames", count).AddField("frametime_average", timeSpan.TotalSeconds / (double)count)
				.AddField("frametime_99_9", this.sortedList[Mathf.Clamp(count - count / 1000, 0, count - 1)])
				.AddField("frametime_99", this.sortedList[Mathf.Clamp(count - count / 100, 0, count - 1)])
				.AddField("frametime_90", this.sortedList[Mathf.Clamp(count - count / 10, 0, count - 1)])
				.AddField("frametime_75", this.sortedList[Mathf.Clamp(count - count / 4, 0, count - 1)])
				.AddField("frametime_50", this.sortedList[count / 2])
				.AddField("frametime_25", this.sortedList[count / 4])
				.AddField("frametime_10", this.sortedList[count / 10])
				.AddField("frametime_1", this.sortedList[count / 100])
				.AddField("frametime_0_1", this.sortedList[count / 1000])
				.AddField("frametime_std_dev", num)
				.AddField("gc_generations", System.GC.MaxGeneration)
				.AddField("gc_total", System.GC.CollectionCount(System.GC.MaxGeneration));
			if (this.isClient)
			{
				record.AddField("ping_average", (ping.Count == 0) ? 0 : ((int)ping.Average())).AddField("ping_count", ping.Count);
			}
			record.Submit();
			frametimes.Clear();
			ping.Clear();
			this.pool.Enqueue(new PerformanceLogging.PerformancePool
			{
				Frametimes = frametimes,
				Ping = ping
			});
			return Task.CompletedTask;
		}

		// Token: 0x04003D57 RID: 15703
		public static PerformanceLogging server = new PerformanceLogging(false);

		// Token: 0x04003D58 RID: 15704
		public static PerformanceLogging client = new PerformanceLogging(true);

		// Token: 0x04003D59 RID: 15705
		private readonly TimeSpan ClientInterval = TimeSpan.FromMinutes(10.0);

		// Token: 0x04003D5A RID: 15706
		private readonly TimeSpan ServerInterval = TimeSpan.FromMinutes(1.0);

		// Token: 0x04003D5B RID: 15707
		private readonly TimeSpan PublicServerInterval = TimeSpan.FromHours(1.0);

		// Token: 0x04003D5C RID: 15708
		private readonly TimeSpan PingInterval = TimeSpan.FromSeconds(5.0);

		// Token: 0x04003D5D RID: 15709
		private List<TimeSpan> Frametimes = new List<TimeSpan>();

		// Token: 0x04003D5E RID: 15710
		private List<int> PingHistory = new List<int>();

		// Token: 0x04003D5F RID: 15711
		private List<PerformanceLogging.LagSpike> lagSpikes = new List<PerformanceLogging.LagSpike>();

		// Token: 0x04003D60 RID: 15712
		private List<PerformanceLogging.GarbageCollect> garbageCollections = new List<PerformanceLogging.GarbageCollect>();

		// Token: 0x04003D61 RID: 15713
		private bool isClient;

		// Token: 0x04003D62 RID: 15714
		private Stopwatch frameWatch = new Stopwatch();

		// Token: 0x04003D63 RID: 15715
		private DateTime nextPingTime;

		// Token: 0x04003D64 RID: 15716
		private DateTime nextFlushTime;

		// Token: 0x04003D65 RID: 15717
		private DateTime connectedTime;

		// Token: 0x04003D66 RID: 15718
		private int serverIndex;

		// Token: 0x04003D67 RID: 15719
		private Guid totalSessionId = Guid.NewGuid();

		// Token: 0x04003D68 RID: 15720
		private Guid sessionId;

		// Token: 0x04003D69 RID: 15721
		private int lastFrameGC;

		// Token: 0x04003D6A RID: 15722
		private ConcurrentQueue<PerformanceLogging.PerformancePool> pool = new ConcurrentQueue<PerformanceLogging.PerformancePool>();

		// Token: 0x04003D6B RID: 15723
		private Type oxideType;

		// Token: 0x04003D6C RID: 15724
		private bool hasOxideType;

		// Token: 0x04003D6D RID: 15725
		private List<TimeSpan> sortedList = new List<TimeSpan>();

		// Token: 0x02000F9A RID: 3994
		private struct LagSpike
		{
			// Token: 0x040050C5 RID: 20677
			public int FrameIndex;

			// Token: 0x040050C6 RID: 20678
			public TimeSpan Time;

			// Token: 0x040050C7 RID: 20679
			public bool WasGC;
		}

		// Token: 0x02000F9B RID: 3995
		private struct GarbageCollect
		{
			// Token: 0x040050C8 RID: 20680
			public int FrameIndex;

			// Token: 0x040050C9 RID: 20681
			public TimeSpan Time;
		}

		// Token: 0x02000F9C RID: 3996
		private class PerformancePool
		{
			// Token: 0x040050CA RID: 20682
			public List<TimeSpan> Frametimes;

			// Token: 0x040050CB RID: 20683
			public List<int> Ping;
		}

		// Token: 0x02000F9D RID: 3997
		private struct PluginInfo
		{
			// Token: 0x040050CC RID: 20684
			public string Name;

			// Token: 0x040050CD RID: 20685
			public string Author;

			// Token: 0x040050CE RID: 20686
			public string Version;
		}

		// Token: 0x02000F9E RID: 3998
		private struct ProcessInfo
		{
			// Token: 0x040050CF RID: 20687
			public string Name;

			// Token: 0x040050D0 RID: 20688
			public long WorkingSet;
		}
	}
}
