using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x0200061B RID: 1563
public class MissionPoint : MonoBehaviour
{
	// Token: 0x06002E46 RID: 11846 RVA: 0x001161C8 File Offset: 0x001143C8
	public static int TypeToIndex(int id)
	{
		return MissionPoint.type2index[id];
	}

	// Token: 0x06002E47 RID: 11847 RVA: 0x001161D5 File Offset: 0x001143D5
	public static int IndexToType(int idx)
	{
		return 1 << idx;
	}

	// Token: 0x06002E48 RID: 11848 RVA: 0x001161DD File Offset: 0x001143DD
	public void Awake()
	{
		MissionPoint.all.Add(this);
	}

	// Token: 0x06002E49 RID: 11849 RVA: 0x001161EA File Offset: 0x001143EA
	private void Start()
	{
		if (this.dropToGround)
		{
			SingletonComponent<InvokeHandler>.Instance.Invoke(new Action(this.DropToGround), 0.5f);
		}
	}

	// Token: 0x06002E4A RID: 11850 RVA: 0x00116210 File Offset: 0x00114410
	private void DropToGround()
	{
		if (Rust.Application.isLoading)
		{
			SingletonComponent<InvokeHandler>.Instance.Invoke(new Action(this.DropToGround), 0.5f);
			return;
		}
		Vector3 position = base.transform.position;
		base.transform.DropToGround(false, 100f);
	}

	// Token: 0x06002E4B RID: 11851 RVA: 0x0011625E File Offset: 0x0011445E
	public void OnDisable()
	{
		if (MissionPoint.all.Contains(this))
		{
			MissionPoint.all.Remove(this);
		}
	}

	// Token: 0x06002E4C RID: 11852 RVA: 0x0002C887 File Offset: 0x0002AA87
	public virtual Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06002E4D RID: 11853 RVA: 0x00116279 File Offset: 0x00114479
	public virtual Quaternion GetRotation()
	{
		return base.transform.rotation;
	}

	// Token: 0x06002E4E RID: 11854 RVA: 0x00116288 File Offset: 0x00114488
	public static bool GetMissionPoints(ref List<MissionPoint> points, Vector3 near, float minDistance, float maxDistance, int flags, int exclusionFlags)
	{
		List<MissionPoint> list = Pool.GetList<MissionPoint>();
		foreach (MissionPoint missionPoint in MissionPoint.all)
		{
			if ((missionPoint.Flags & (MissionPoint.MissionPointEnum)flags) == (MissionPoint.MissionPointEnum)flags && (exclusionFlags == 0 || (missionPoint.Flags & (MissionPoint.MissionPointEnum)exclusionFlags) == (MissionPoint.MissionPointEnum)0))
			{
				float num = Vector3.Distance(missionPoint.transform.position, near);
				if (num <= maxDistance && num > minDistance)
				{
					if (BaseMission.blockedPoints.Count > 0)
					{
						bool flag = false;
						using (List<Vector3>.Enumerator enumerator2 = BaseMission.blockedPoints.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (Vector3.Distance(enumerator2.Current, missionPoint.transform.position) < 5f)
								{
									flag = true;
									break;
								}
							}
						}
						if (flag)
						{
							continue;
						}
					}
					list.Add(missionPoint);
				}
			}
		}
		if (list.Count == 0)
		{
			return false;
		}
		foreach (MissionPoint missionPoint2 in list)
		{
			points.Add(missionPoint2);
		}
		Pool.FreeList<MissionPoint>(ref list);
		return true;
	}

	// Token: 0x040025F5 RID: 9717
	public bool dropToGround = true;

	// Token: 0x040025F6 RID: 9718
	public const int COUNT = 8;

	// Token: 0x040025F7 RID: 9719
	public const int EVERYTHING = -1;

	// Token: 0x040025F8 RID: 9720
	public const int NOTHING = 0;

	// Token: 0x040025F9 RID: 9721
	public const int EASY_MONUMENT = 1;

	// Token: 0x040025FA RID: 9722
	public const int MED_MONUMENT = 2;

	// Token: 0x040025FB RID: 9723
	public const int HARD_MONUMENT = 4;

	// Token: 0x040025FC RID: 9724
	public const int ITEM_HIDESPOT = 8;

	// Token: 0x040025FD RID: 9725
	public const int UNDERWATER = 128;

	// Token: 0x040025FE RID: 9726
	public const int EASY_MONUMENT_IDX = 0;

	// Token: 0x040025FF RID: 9727
	public const int MED_MONUMENT_IDX = 1;

	// Token: 0x04002600 RID: 9728
	public const int HARD_MONUMENT_IDX = 2;

	// Token: 0x04002601 RID: 9729
	public const int ITEM_HIDESPOT_IDX = 3;

	// Token: 0x04002602 RID: 9730
	public const int FOREST_IDX = 4;

	// Token: 0x04002603 RID: 9731
	public const int ROADSIDE_IDX = 5;

	// Token: 0x04002604 RID: 9732
	public const int BEACH = 6;

	// Token: 0x04002605 RID: 9733
	public const int UNDERWATER_IDX = 7;

	// Token: 0x04002606 RID: 9734
	private static Dictionary<int, int> type2index = new Dictionary<int, int>
	{
		{ 1, 0 },
		{ 2, 1 },
		{ 4, 2 },
		{ 8, 3 },
		{ 128, 7 }
	};

	// Token: 0x04002607 RID: 9735
	public static List<MissionPoint> all = new List<MissionPoint>();

	// Token: 0x04002608 RID: 9736
	[global::InspectorFlags]
	public MissionPoint.MissionPointEnum Flags = (MissionPoint.MissionPointEnum)(-1);

	// Token: 0x02000DAF RID: 3503
	public enum MissionPointEnum
	{
		// Token: 0x040048F9 RID: 18681
		EasyMonument = 1,
		// Token: 0x040048FA RID: 18682
		MediumMonument,
		// Token: 0x040048FB RID: 18683
		HardMonument = 4,
		// Token: 0x040048FC RID: 18684
		Item_Hidespot = 8,
		// Token: 0x040048FD RID: 18685
		Underwater = 128
	}
}
