using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020009A2 RID: 2466
public class OccludeeState : OcclusionCulling.SmartListValue
{
	// Token: 0x1700049A RID: 1178
	// (get) Token: 0x06003A7C RID: 14972 RVA: 0x00159032 File Offset: 0x00157232
	public bool isVisible
	{
		get
		{
			return this.states[this.slot].isVisible > 0;
		}
	}

	// Token: 0x06003A7D RID: 14973 RVA: 0x00159050 File Offset: 0x00157250
	public OccludeeState Initialize(OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.BufferSet set, int slot, Vector4 sphereBounds, bool isVisible, float minTimeVisible, bool isStatic, int layer, OcclusionCulling.OnVisibilityChanged onVisibilityChanged)
	{
		states[slot] = new OccludeeState.State
		{
			sphereBounds = sphereBounds,
			minTimeVisible = minTimeVisible,
			waitTime = (isVisible ? (Time.time + minTimeVisible) : 0f),
			waitFrame = (uint)(Time.frameCount + 1),
			isVisible = (isVisible ? 1 : 0),
			active = 1,
			callback = ((onVisibilityChanged != null) ? 1 : 0)
		};
		this.slot = slot;
		this.isStatic = isStatic;
		this.layer = layer;
		this.onVisibilityChanged = onVisibilityChanged;
		this.cell = null;
		this.states = states;
		return this;
	}

	// Token: 0x06003A7E RID: 14974 RVA: 0x001590FB File Offset: 0x001572FB
	public void Invalidate()
	{
		this.states[this.slot] = OccludeeState.State.Unused;
		this.slot = -1;
		this.onVisibilityChanged = null;
		this.cell = null;
	}

	// Token: 0x06003A7F RID: 14975 RVA: 0x00159128 File Offset: 0x00157328
	public void MakeVisible()
	{
		this.states.array[this.slot].waitTime = Time.time + this.states[this.slot].minTimeVisible;
		this.states.array[this.slot].isVisible = 1;
		if (this.onVisibilityChanged != null)
		{
			this.onVisibilityChanged(true);
		}
	}

	// Token: 0x0400353F RID: 13631
	public int slot;

	// Token: 0x04003540 RID: 13632
	public bool isStatic;

	// Token: 0x04003541 RID: 13633
	public int layer;

	// Token: 0x04003542 RID: 13634
	public OcclusionCulling.OnVisibilityChanged onVisibilityChanged;

	// Token: 0x04003543 RID: 13635
	public OcclusionCulling.Cell cell;

	// Token: 0x04003544 RID: 13636
	public OcclusionCulling.SimpleList<OccludeeState.State> states;

	// Token: 0x02000EE6 RID: 3814
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
	public struct State
	{
		// Token: 0x04004DC7 RID: 19911
		[FieldOffset(0)]
		public Vector4 sphereBounds;

		// Token: 0x04004DC8 RID: 19912
		[FieldOffset(16)]
		public float minTimeVisible;

		// Token: 0x04004DC9 RID: 19913
		[FieldOffset(20)]
		public float waitTime;

		// Token: 0x04004DCA RID: 19914
		[FieldOffset(24)]
		public uint waitFrame;

		// Token: 0x04004DCB RID: 19915
		[FieldOffset(28)]
		public byte isVisible;

		// Token: 0x04004DCC RID: 19916
		[FieldOffset(29)]
		public byte active;

		// Token: 0x04004DCD RID: 19917
		[FieldOffset(30)]
		public byte callback;

		// Token: 0x04004DCE RID: 19918
		[FieldOffset(31)]
		public byte pad1;

		// Token: 0x04004DCF RID: 19919
		public static OccludeeState.State Unused = new OccludeeState.State
		{
			active = 0
		};
	}
}
