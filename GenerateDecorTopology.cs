using System;
using UnityEngine;

// Token: 0x020006C2 RID: 1730
public class GenerateDecorTopology : ProceduralComponent
{
	// Token: 0x060031C9 RID: 12745 RVA: 0x00129DF0 File Offset: 0x00127FF0
	public override void Process(uint seed)
	{
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		int topores = topomap.res;
		Parallel.For(0, topores, delegate(int z)
		{
			for (int i = 0; i < topores; i++)
			{
				if (topomap.GetTopology(i, z, 4194306))
				{
					topomap.AddTopology(i, z, 512);
				}
				else if (!this.KeepExisting)
				{
					topomap.RemoveTopology(i, z, 512);
				}
			}
		});
	}

	// Token: 0x04002868 RID: 10344
	public bool KeepExisting = true;
}
