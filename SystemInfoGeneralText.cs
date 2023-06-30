using System;
using System.Text;
using Rust;
using TMPro;
using UnityEngine;

// Token: 0x02000318 RID: 792
public class SystemInfoGeneralText : MonoBehaviour
{
	// Token: 0x17000286 RID: 646
	// (get) Token: 0x06001F06 RID: 7942 RVA: 0x000D2DAC File Offset: 0x000D0FAC
	public static string currentInfo
	{
		get
		{
			BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(false);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("System");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tName: ");
			stringBuilder.Append(SystemInfo.deviceName);
			stringBuilder.AppendLine();
			stringBuilder.Append("\tOS:   ");
			stringBuilder.Append(SystemInfo.operatingSystem);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.Append("CPU");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tModel:  ");
			stringBuilder.Append(SystemInfo.processorType);
			stringBuilder.AppendLine();
			stringBuilder.Append("\tCores:  ");
			stringBuilder.Append(SystemInfo.processorCount);
			stringBuilder.AppendLine();
			stringBuilder.Append("\tMemory: ");
			stringBuilder.Append(SystemInfo.systemMemorySize);
			stringBuilder.Append(" MB");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.Append("GPU");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tModel:  ");
			stringBuilder.Append(SystemInfo.graphicsDeviceName);
			stringBuilder.AppendLine();
			stringBuilder.Append("\tAPI:    ");
			stringBuilder.Append(SystemInfo.graphicsDeviceVersion);
			stringBuilder.AppendLine();
			stringBuilder.Append("\tMemory: ");
			stringBuilder.Append(SystemInfo.graphicsMemorySize);
			stringBuilder.Append(" MB");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tSM:     ");
			stringBuilder.Append(SystemInfo.graphicsShaderLevel);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.Append("Process");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tMemory:   ");
			stringBuilder.Append(SystemInfoEx.systemMemoryUsed);
			stringBuilder.Append(" MB");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.Append("Mono");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tCollects: ");
			stringBuilder.Append(Rust.GC.CollectionCount());
			stringBuilder.AppendLine();
			stringBuilder.Append("\tMemory:   ");
			stringBuilder.Append(Rust.GC.GetTotalMemory());
			stringBuilder.Append(" MB");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			if (World.Seed > 0U && World.Size > 0U)
			{
				stringBuilder.Append("World");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tSeed:        ");
				if (activeGameMode != null && !activeGameMode.ingameMap)
				{
					stringBuilder.Append("?");
				}
				else
				{
					stringBuilder.Append(World.Seed);
				}
				stringBuilder.AppendLine();
				stringBuilder.Append("\tSize:        ");
				stringBuilder.Append(SystemInfoGeneralText.KM2(World.Size));
				stringBuilder.Append(" km²");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tHeightmap:   ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tWatermap:    ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.WaterMap ? TerrainMeta.WaterMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tSplatmap:    ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.SplatMap ? TerrainMeta.SplatMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tBiomemap:    ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tTopologymap: ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.TopologyMap ? TerrainMeta.TopologyMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tAlphamap:    ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.AlphaMap ? TerrainMeta.AlphaMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine();
			if (!string.IsNullOrEmpty(World.Checksum))
			{
				stringBuilder.AppendLine("Checksum");
				stringBuilder.Append('\t');
				stringBuilder.AppendLine(World.Checksum);
			}
			return stringBuilder.ToString();
		}
	}

	// Token: 0x06001F07 RID: 7943 RVA: 0x000D325D File Offset: 0x000D145D
	protected void Update()
	{
		this.text.text = SystemInfoGeneralText.currentInfo;
	}

	// Token: 0x06001F08 RID: 7944 RVA: 0x000D326F File Offset: 0x000D146F
	private static long MB(long bytes)
	{
		return bytes / 1048576L;
	}

	// Token: 0x06001F09 RID: 7945 RVA: 0x000D3279 File Offset: 0x000D1479
	private static long MB(ulong bytes)
	{
		return SystemInfoGeneralText.MB((long)bytes);
	}

	// Token: 0x06001F0A RID: 7946 RVA: 0x000D3281 File Offset: 0x000D1481
	private static int KM2(float meters)
	{
		return Mathf.RoundToInt(meters * meters * 1E-06f);
	}

	// Token: 0x040017E7 RID: 6119
	public TextMeshProUGUI text;
}
