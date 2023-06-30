using System;
using System.Collections;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B45 RID: 2885
	public class CoverPoint
	{
		// Token: 0x1700065C RID: 1628
		// (get) Token: 0x060045CD RID: 17869 RVA: 0x0019723E File Offset: 0x0019543E
		// (set) Token: 0x060045CE RID: 17870 RVA: 0x00197246 File Offset: 0x00195446
		public CoverPointVolume Volume { get; private set; }

		// Token: 0x1700065D RID: 1629
		// (get) Token: 0x060045CF RID: 17871 RVA: 0x0019724F File Offset: 0x0019544F
		// (set) Token: 0x060045D0 RID: 17872 RVA: 0x00197279 File Offset: 0x00195479
		public Vector3 Position
		{
			get
			{
				if (this.IsDynamic && this.SourceTransform != null)
				{
					return this.SourceTransform.position;
				}
				return this._staticPosition;
			}
			set
			{
				this._staticPosition = value;
			}
		}

		// Token: 0x1700065E RID: 1630
		// (get) Token: 0x060045D1 RID: 17873 RVA: 0x00197282 File Offset: 0x00195482
		// (set) Token: 0x060045D2 RID: 17874 RVA: 0x001972AC File Offset: 0x001954AC
		public Vector3 Normal
		{
			get
			{
				if (this.IsDynamic && this.SourceTransform != null)
				{
					return this.SourceTransform.forward;
				}
				return this._staticNormal;
			}
			set
			{
				this._staticNormal = value;
			}
		}

		// Token: 0x1700065F RID: 1631
		// (get) Token: 0x060045D3 RID: 17875 RVA: 0x001972B5 File Offset: 0x001954B5
		// (set) Token: 0x060045D4 RID: 17876 RVA: 0x001972BD File Offset: 0x001954BD
		public BaseEntity ReservedFor { get; set; }

		// Token: 0x17000660 RID: 1632
		// (get) Token: 0x060045D5 RID: 17877 RVA: 0x001972C6 File Offset: 0x001954C6
		public bool IsReserved
		{
			get
			{
				return this.ReservedFor != null;
			}
		}

		// Token: 0x17000661 RID: 1633
		// (get) Token: 0x060045D6 RID: 17878 RVA: 0x001972D4 File Offset: 0x001954D4
		// (set) Token: 0x060045D7 RID: 17879 RVA: 0x001972DC File Offset: 0x001954DC
		public bool IsCompromised { get; set; }

		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x060045D8 RID: 17880 RVA: 0x001972E5 File Offset: 0x001954E5
		// (set) Token: 0x060045D9 RID: 17881 RVA: 0x001972ED File Offset: 0x001954ED
		public float Score { get; set; }

		// Token: 0x060045DA RID: 17882 RVA: 0x001972F6 File Offset: 0x001954F6
		public bool IsValidFor(BaseEntity entity)
		{
			return !this.IsCompromised && (this.ReservedFor == null || this.ReservedFor == entity);
		}

		// Token: 0x060045DB RID: 17883 RVA: 0x0019731E File Offset: 0x0019551E
		public CoverPoint(CoverPointVolume volume, float score)
		{
			this.Volume = volume;
			this.Score = score;
		}

		// Token: 0x060045DC RID: 17884 RVA: 0x00197334 File Offset: 0x00195534
		public void CoverIsCompromised(float cooldown)
		{
			if (this.IsCompromised)
			{
				return;
			}
			if (this.Volume != null)
			{
				this.Volume.StartCoroutine(this.StartCooldown(cooldown));
			}
		}

		// Token: 0x060045DD RID: 17885 RVA: 0x00197360 File Offset: 0x00195560
		private IEnumerator StartCooldown(float cooldown)
		{
			this.IsCompromised = true;
			yield return CoroutineEx.waitForSeconds(cooldown);
			this.IsCompromised = false;
			yield break;
		}

		// Token: 0x060045DE RID: 17886 RVA: 0x00197378 File Offset: 0x00195578
		public bool ProvidesCoverFromPoint(Vector3 point, float arcThreshold)
		{
			Vector3 normalized = (this.Position - point).normalized;
			return Vector3.Dot(this.Normal, normalized) < arcThreshold;
		}

		// Token: 0x04003EBB RID: 16059
		public CoverPoint.CoverType NormalCoverType;

		// Token: 0x04003EBC RID: 16060
		public bool IsDynamic;

		// Token: 0x04003EBD RID: 16061
		public Transform SourceTransform;

		// Token: 0x04003EBE RID: 16062
		private Vector3 _staticPosition;

		// Token: 0x04003EBF RID: 16063
		private Vector3 _staticNormal;

		// Token: 0x02000FAD RID: 4013
		public enum CoverType
		{
			// Token: 0x04005106 RID: 20742
			Full,
			// Token: 0x04005107 RID: 20743
			Partial,
			// Token: 0x04005108 RID: 20744
			None
		}
	}
}
