using System;
using System.Collections.Generic;

// Token: 0x020006EC RID: 1772
public class ProcessMonumentNodes : ProceduralComponent
{
	// Token: 0x0600324D RID: 12877 RVA: 0x00136654 File Offset: 0x00134854
	public override void Process(uint seed)
	{
		List<MonumentNode> monumentNodes = SingletonComponent<WorldSetup>.Instance.MonumentNodes;
		if (!World.Cached)
		{
			for (int i = 0; i < monumentNodes.Count; i++)
			{
				monumentNodes[i].Process(ref seed);
			}
		}
		monumentNodes.Clear();
	}
}
