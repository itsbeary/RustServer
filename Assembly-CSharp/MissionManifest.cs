using System;
using UnityEngine;

// Token: 0x0200061A RID: 1562
[CreateAssetMenu(menuName = "Rust/MissionManifest")]
public class MissionManifest : ScriptableObject
{
	// Token: 0x06002E42 RID: 11842 RVA: 0x001160D8 File Offset: 0x001142D8
	public static MissionManifest Get()
	{
		if (MissionManifest.instance == null)
		{
			MissionManifest.instance = Resources.Load<MissionManifest>("MissionManifest");
			foreach (WorldPositionGenerator worldPositionGenerator in MissionManifest.instance.positionGenerators)
			{
				if (worldPositionGenerator != null)
				{
					worldPositionGenerator.PrecalculatePositions();
				}
			}
		}
		return MissionManifest.instance;
	}

	// Token: 0x06002E43 RID: 11843 RVA: 0x00116134 File Offset: 0x00114334
	public static BaseMission GetFromShortName(string shortname)
	{
		ScriptableObjectRef[] array = MissionManifest.Get().missionList;
		for (int i = 0; i < array.Length; i++)
		{
			BaseMission baseMission = array[i].Get() as BaseMission;
			if (baseMission.shortname == shortname)
			{
				return baseMission;
			}
		}
		return null;
	}

	// Token: 0x06002E44 RID: 11844 RVA: 0x0011617C File Offset: 0x0011437C
	public static BaseMission GetFromID(uint id)
	{
		MissionManifest missionManifest = MissionManifest.Get();
		if (missionManifest.missionList == null)
		{
			return null;
		}
		ScriptableObjectRef[] array = missionManifest.missionList;
		for (int i = 0; i < array.Length; i++)
		{
			BaseMission baseMission = array[i].Get() as BaseMission;
			if (baseMission.id == id)
			{
				return baseMission;
			}
		}
		return null;
	}

	// Token: 0x040025F2 RID: 9714
	public ScriptableObjectRef[] missionList;

	// Token: 0x040025F3 RID: 9715
	public WorldPositionGenerator[] positionGenerators;

	// Token: 0x040025F4 RID: 9716
	public static MissionManifest instance;
}
