using System;
using UnityEngine;

namespace Rust.Interpolation
{
	// Token: 0x02000B43 RID: 2883
	public struct TransformSnapshot : ISnapshot<TransformSnapshot>
	{
		// Token: 0x1700065A RID: 1626
		// (get) Token: 0x060045C1 RID: 17857 RVA: 0x00197154 File Offset: 0x00195354
		// (set) Token: 0x060045C2 RID: 17858 RVA: 0x0019715C File Offset: 0x0019535C
		public float Time { get; set; }

		// Token: 0x060045C3 RID: 17859 RVA: 0x00197165 File Offset: 0x00195365
		public TransformSnapshot(float time, Vector3 pos, Quaternion rot)
		{
			this.Time = time;
			this.pos = pos;
			this.rot = rot;
		}

		// Token: 0x060045C4 RID: 17860 RVA: 0x0019717C File Offset: 0x0019537C
		public void MatchValuesTo(TransformSnapshot entry)
		{
			this.pos = entry.pos;
			this.rot = entry.rot;
		}

		// Token: 0x060045C5 RID: 17861 RVA: 0x00197196 File Offset: 0x00195396
		public void Lerp(TransformSnapshot prev, TransformSnapshot next, float delta)
		{
			this.pos = Vector3.LerpUnclamped(prev.pos, next.pos, delta);
			this.rot = Quaternion.SlerpUnclamped(prev.rot, next.rot, delta);
		}

		// Token: 0x060045C6 RID: 17862 RVA: 0x001971C8 File Offset: 0x001953C8
		public TransformSnapshot GetNew()
		{
			return default(TransformSnapshot);
		}

		// Token: 0x04003EB6 RID: 16054
		public Vector3 pos;

		// Token: 0x04003EB7 RID: 16055
		public Quaternion rot;
	}
}
