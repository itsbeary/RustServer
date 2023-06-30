using System;
using System.IO;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AB8 RID: 2744
	[ConsoleSystem.Factory("data")]
	public class Data : ConsoleSystem
	{
		// Token: 0x0600419D RID: 16797 RVA: 0x00184868 File Offset: 0x00182A68
		[ServerVar]
		[ClientVar]
		public static void export(ConsoleSystem.Arg args)
		{
			string @string = args.GetString(0, "none");
			string text = Path.Combine(Application.persistentDataPath, @string + ".raw");
			if (!(@string == "splatmap"))
			{
				if (!(@string == "heightmap"))
				{
					if (!(@string == "biomemap"))
					{
						if (!(@string == "topologymap"))
						{
							if (!(@string == "alphamap"))
							{
								if (!(@string == "watermap"))
								{
									args.ReplyWith("Unknown export source: " + @string);
									return;
								}
								if (TerrainMeta.WaterMap)
								{
									RawWriter.Write(TerrainMeta.WaterMap.ToEnumerable(), text);
								}
							}
							else if (TerrainMeta.AlphaMap)
							{
								RawWriter.Write(TerrainMeta.AlphaMap.ToEnumerable(), text);
							}
						}
						else if (TerrainMeta.TopologyMap)
						{
							RawWriter.Write(TerrainMeta.TopologyMap.ToEnumerable(), text);
						}
					}
					else if (TerrainMeta.BiomeMap)
					{
						RawWriter.Write(TerrainMeta.BiomeMap.ToEnumerable(), text);
					}
				}
				else if (TerrainMeta.HeightMap)
				{
					RawWriter.Write(TerrainMeta.HeightMap.ToEnumerable(), text);
				}
			}
			else if (TerrainMeta.SplatMap)
			{
				RawWriter.Write(TerrainMeta.SplatMap.ToEnumerable(), text);
			}
			args.ReplyWith("Export written to " + text);
		}
	}
}
