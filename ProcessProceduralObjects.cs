using System;
using System.Collections.Generic;

// Token: 0x020006ED RID: 1773
public class ProcessProceduralObjects : ProceduralComponent
{
	// Token: 0x0600324F RID: 12879 RVA: 0x00136698 File Offset: 0x00134898
	public override void Process(uint seed)
	{
		List<ProceduralObject> proceduralObjects = SingletonComponent<WorldSetup>.Instance.ProceduralObjects;
		if (!World.Cached)
		{
			for (int i = 0; i < proceduralObjects.Count; i++)
			{
				ProceduralObject proceduralObject = proceduralObjects[i];
				if (proceduralObject)
				{
					proceduralObject.Process();
				}
			}
		}
		proceduralObjects.Clear();
	}

	// Token: 0x17000414 RID: 1044
	// (get) Token: 0x06003250 RID: 12880 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
