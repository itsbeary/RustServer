using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B48 RID: 2888
	public class ManualCoverPoint : FacepunchBehaviour
	{
		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x060045EC RID: 17900 RVA: 0x0002C887 File Offset: 0x0002AA87
		public Vector3 Position
		{
			get
			{
				return base.transform.position;
			}
		}

		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x060045ED RID: 17901 RVA: 0x00197A4D File Offset: 0x00195C4D
		public float DirectionMagnitude
		{
			get
			{
				if (this.Volume != null)
				{
					return this.Volume.CoverPointRayLength;
				}
				return 1f;
			}
		}

		// Token: 0x060045EE RID: 17902 RVA: 0x00197A6E File Offset: 0x00195C6E
		private void Awake()
		{
			if (base.transform.parent != null)
			{
				this.Volume = base.transform.parent.GetComponent<CoverPointVolume>();
			}
		}

		// Token: 0x060045EF RID: 17903 RVA: 0x00197A9C File Offset: 0x00195C9C
		public CoverPoint ToCoverPoint(CoverPointVolume volume)
		{
			this.Volume = volume;
			if (this.IsDynamic)
			{
				CoverPoint coverPoint = new CoverPoint(this.Volume, this.Score);
				coverPoint.IsDynamic = true;
				coverPoint.SourceTransform = base.transform;
				coverPoint.NormalCoverType = this.NormalCoverType;
				Transform transform = base.transform;
				coverPoint.Position = ((transform != null) ? transform.position : Vector3.zero);
				return coverPoint;
			}
			Vector3 normalized = (base.transform.rotation * this.Normal).normalized;
			return new CoverPoint(this.Volume, this.Score)
			{
				IsDynamic = false,
				Position = base.transform.position,
				Normal = normalized,
				NormalCoverType = this.NormalCoverType
			};
		}

		// Token: 0x04003ECF RID: 16079
		public bool IsDynamic;

		// Token: 0x04003ED0 RID: 16080
		public float Score = 2f;

		// Token: 0x04003ED1 RID: 16081
		public CoverPointVolume Volume;

		// Token: 0x04003ED2 RID: 16082
		public Vector3 Normal;

		// Token: 0x04003ED3 RID: 16083
		public CoverPoint.CoverType NormalCoverType;
	}
}
