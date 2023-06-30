using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B31 RID: 2865
	[Serializable]
	public class ConditionalObject
	{
		// Token: 0x17000647 RID: 1607
		// (get) Token: 0x06004558 RID: 17752 RVA: 0x001959CF File Offset: 0x00193BCF
		// (set) Token: 0x06004559 RID: 17753 RVA: 0x001959D7 File Offset: 0x00193BD7
		public bool? IsActive { get; private set; }

		// Token: 0x0600455A RID: 17754 RVA: 0x001959E0 File Offset: 0x00193BE0
		public ConditionalObject(GameObject conditionalGO, GameObject ownerGO, int socketsTaken)
		{
			this.gameObject = conditionalGO;
			this.ownerGameObject = ownerGO;
			this.socketSettings = new ConditionalSocketSettings[socketsTaken];
		}

		// Token: 0x0600455B RID: 17755 RVA: 0x00195A04 File Offset: 0x00193C04
		public void SetActive(bool active)
		{
			if (this.IsActive != null && active == this.IsActive.Value)
			{
				return;
			}
			this.gameObject.SetActive(active);
			this.IsActive = new bool?(active);
		}

		// Token: 0x0600455C RID: 17756 RVA: 0x00195A4C File Offset: 0x00193C4C
		public void RefreshActive()
		{
			if (this.IsActive == null)
			{
				return;
			}
			this.gameObject.SetActive(this.IsActive.Value);
		}

		// Token: 0x04003E5A RID: 15962
		public GameObject gameObject;

		// Token: 0x04003E5B RID: 15963
		public GameObject ownerGameObject;

		// Token: 0x04003E5C RID: 15964
		public ConditionalSocketSettings[] socketSettings;

		// Token: 0x04003E5D RID: 15965
		public bool restrictOnHealth;

		// Token: 0x04003E5E RID: 15966
		public float healthRestrictionMin;

		// Token: 0x04003E5F RID: 15967
		public float healthRestrictionMax;

		// Token: 0x04003E60 RID: 15968
		public bool restrictOnAdjacent;

		// Token: 0x04003E61 RID: 15969
		public ConditionalObject.AdjacentCondition adjacentRestriction;

		// Token: 0x04003E62 RID: 15970
		public ConditionalObject.AdjacentMatchType adjacentMatch;

		// Token: 0x04003E63 RID: 15971
		public bool restrictOnLockable;

		// Token: 0x04003E64 RID: 15972
		public bool lockableRestriction;

		// Token: 0x02000FA4 RID: 4004
		public enum AdjacentCondition
		{
			// Token: 0x040050DE RID: 20702
			SameInFront,
			// Token: 0x040050DF RID: 20703
			SameBehind,
			// Token: 0x040050E0 RID: 20704
			DifferentInFront,
			// Token: 0x040050E1 RID: 20705
			DifferentBehind,
			// Token: 0x040050E2 RID: 20706
			BothDifferent,
			// Token: 0x040050E3 RID: 20707
			BothSame
		}

		// Token: 0x02000FA5 RID: 4005
		public enum AdjacentMatchType
		{
			// Token: 0x040050E5 RID: 20709
			GroupOrExact,
			// Token: 0x040050E6 RID: 20710
			ExactOnly,
			// Token: 0x040050E7 RID: 20711
			GroupNotExact
		}
	}
}
