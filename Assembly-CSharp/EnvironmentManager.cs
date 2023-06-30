using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200050B RID: 1291
public class EnvironmentManager : SingletonComponent<EnvironmentManager>
{
	// Token: 0x06002980 RID: 10624 RVA: 0x000FEE34 File Offset: 0x000FD034
	public static EnvironmentType Get(OBB obb)
	{
		EnvironmentType environmentType = (EnvironmentType)0;
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		GamePhysics.OverlapOBB<EnvironmentVolume>(obb, list, 262144, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			environmentType |= list[i].Type;
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return environmentType;
	}

	// Token: 0x06002981 RID: 10625 RVA: 0x000FEE80 File Offset: 0x000FD080
	public static EnvironmentType Get(Vector3 pos, ref List<EnvironmentVolume> list)
	{
		EnvironmentType environmentType = (EnvironmentType)0;
		GamePhysics.OverlapSphere<EnvironmentVolume>(pos, 0.01f, list, 262144, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			environmentType |= list[i].Type;
		}
		return environmentType;
	}

	// Token: 0x06002982 RID: 10626 RVA: 0x000FEEC8 File Offset: 0x000FD0C8
	public static EnvironmentType Get(Vector3 pos)
	{
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		EnvironmentType environmentType = EnvironmentManager.Get(pos, ref list);
		Pool.FreeList<EnvironmentVolume>(ref list);
		return environmentType;
	}

	// Token: 0x06002983 RID: 10627 RVA: 0x000FEEEA File Offset: 0x000FD0EA
	public static bool Check(OBB obb, EnvironmentType type)
	{
		return (EnvironmentManager.Get(obb) & type) > (EnvironmentType)0;
	}

	// Token: 0x06002984 RID: 10628 RVA: 0x000FEEF7 File Offset: 0x000FD0F7
	public static bool Check(Vector3 pos, EnvironmentType type)
	{
		return (EnvironmentManager.Get(pos) & type) > (EnvironmentType)0;
	}
}
