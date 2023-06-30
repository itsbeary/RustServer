using System;
using System.Collections.Generic;
using UnityEngine.Sprites;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A3D RID: 2621
	[AddComponentMenu("UI/Extensions/Primitives/UILineRendererList")]
	[RequireComponent(typeof(RectTransform))]
	public class UILineRendererList : UIPrimitiveBase
	{
		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x06003E88 RID: 16008 RVA: 0x0016E58E File Offset: 0x0016C78E
		// (set) Token: 0x06003E89 RID: 16009 RVA: 0x0016E596 File Offset: 0x0016C796
		public float LineThickness
		{
			get
			{
				return this.lineThickness;
			}
			set
			{
				this.lineThickness = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x06003E8A RID: 16010 RVA: 0x0016E5A5 File Offset: 0x0016C7A5
		// (set) Token: 0x06003E8B RID: 16011 RVA: 0x0016E5AD File Offset: 0x0016C7AD
		public bool RelativeSize
		{
			get
			{
				return this.relativeSize;
			}
			set
			{
				this.relativeSize = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x06003E8C RID: 16012 RVA: 0x0016E5BC File Offset: 0x0016C7BC
		// (set) Token: 0x06003E8D RID: 16013 RVA: 0x0016E5C4 File Offset: 0x0016C7C4
		public bool LineList
		{
			get
			{
				return this.lineList;
			}
			set
			{
				this.lineList = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x06003E8E RID: 16014 RVA: 0x0016E5D3 File Offset: 0x0016C7D3
		// (set) Token: 0x06003E8F RID: 16015 RVA: 0x0016E5DB File Offset: 0x0016C7DB
		public bool LineCaps
		{
			get
			{
				return this.lineCaps;
			}
			set
			{
				this.lineCaps = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x06003E90 RID: 16016 RVA: 0x0016E5EA File Offset: 0x0016C7EA
		// (set) Token: 0x06003E91 RID: 16017 RVA: 0x0016E5F2 File Offset: 0x0016C7F2
		public int BezierSegmentsPerCurve
		{
			get
			{
				return this.bezierSegmentsPerCurve;
			}
			set
			{
				this.bezierSegmentsPerCurve = value;
			}
		}

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x06003E92 RID: 16018 RVA: 0x0016E5FB File Offset: 0x0016C7FB
		// (set) Token: 0x06003E93 RID: 16019 RVA: 0x0016E603 File Offset: 0x0016C803
		public List<Vector2> Points
		{
			get
			{
				return this.m_points;
			}
			set
			{
				if (this.m_points == value)
				{
					return;
				}
				this.m_points = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x06003E94 RID: 16020 RVA: 0x0016E61C File Offset: 0x0016C81C
		public void AddPoint(Vector2 pointToAdd)
		{
			this.m_points.Add(pointToAdd);
			this.SetAllDirty();
		}

		// Token: 0x06003E95 RID: 16021 RVA: 0x0016E630 File Offset: 0x0016C830
		public void RemovePoint(Vector2 pointToRemove)
		{
			this.m_points.Remove(pointToRemove);
			this.SetAllDirty();
		}

		// Token: 0x06003E96 RID: 16022 RVA: 0x0016E645 File Offset: 0x0016C845
		public void ClearPoints()
		{
			this.m_points.Clear();
			this.SetAllDirty();
		}

		// Token: 0x06003E97 RID: 16023 RVA: 0x0016E658 File Offset: 0x0016C858
		private void PopulateMesh(VertexHelper vh, List<Vector2> pointsToDraw)
		{
			if (this.BezierMode != UILineRendererList.BezierType.None && this.BezierMode != UILineRendererList.BezierType.Catenary && pointsToDraw.Count > 3)
			{
				BezierPath bezierPath = new BezierPath();
				bezierPath.SetControlPoints(pointsToDraw);
				bezierPath.SegmentsPerCurve = this.bezierSegmentsPerCurve;
				UILineRendererList.BezierType bezierMode = this.BezierMode;
				List<Vector2> list;
				if (bezierMode != UILineRendererList.BezierType.Basic)
				{
					if (bezierMode != UILineRendererList.BezierType.Improved)
					{
						list = bezierPath.GetDrawingPoints2();
					}
					else
					{
						list = bezierPath.GetDrawingPoints1();
					}
				}
				else
				{
					list = bezierPath.GetDrawingPoints0();
				}
				pointsToDraw = list;
			}
			if (this.BezierMode == UILineRendererList.BezierType.Catenary && pointsToDraw.Count == 2)
			{
				CableCurve cableCurve = new CableCurve(pointsToDraw);
				cableCurve.slack = base.Resoloution;
				cableCurve.steps = this.BezierSegmentsPerCurve;
				pointsToDraw.Clear();
				pointsToDraw.AddRange(cableCurve.Points());
			}
			if (base.ImproveResolution != ResolutionMode.None)
			{
				pointsToDraw = base.IncreaseResolution(pointsToDraw);
			}
			float num = ((!this.relativeSize) ? 1f : base.rectTransform.rect.width);
			float num2 = ((!this.relativeSize) ? 1f : base.rectTransform.rect.height);
			float num3 = -base.rectTransform.pivot.x * num;
			float num4 = -base.rectTransform.pivot.y * num2;
			List<UIVertex[]> list2 = new List<UIVertex[]>();
			if (this.lineList)
			{
				for (int i = 1; i < pointsToDraw.Count; i += 2)
				{
					Vector2 vector = pointsToDraw[i - 1];
					Vector2 vector2 = pointsToDraw[i];
					vector = new Vector2(vector.x * num + num3, vector.y * num2 + num4);
					vector2 = new Vector2(vector2.x * num + num3, vector2.y * num2 + num4);
					if (this.lineCaps)
					{
						list2.Add(this.CreateLineCap(vector, vector2, UILineRendererList.SegmentType.Start));
					}
					list2.Add(this.CreateLineSegment(vector, vector2, UILineRendererList.SegmentType.Middle));
					if (this.lineCaps)
					{
						list2.Add(this.CreateLineCap(vector, vector2, UILineRendererList.SegmentType.End));
					}
				}
			}
			else
			{
				for (int j = 1; j < pointsToDraw.Count; j++)
				{
					Vector2 vector3 = pointsToDraw[j - 1];
					Vector2 vector4 = pointsToDraw[j];
					vector3 = new Vector2(vector3.x * num + num3, vector3.y * num2 + num4);
					vector4 = new Vector2(vector4.x * num + num3, vector4.y * num2 + num4);
					if (this.lineCaps && j == 1)
					{
						list2.Add(this.CreateLineCap(vector3, vector4, UILineRendererList.SegmentType.Start));
					}
					list2.Add(this.CreateLineSegment(vector3, vector4, UILineRendererList.SegmentType.Middle));
					if (this.lineCaps && j == pointsToDraw.Count - 1)
					{
						list2.Add(this.CreateLineCap(vector3, vector4, UILineRendererList.SegmentType.End));
					}
				}
			}
			for (int k = 0; k < list2.Count; k++)
			{
				if (!this.lineList && k < list2.Count - 1)
				{
					Vector3 vector5 = list2[k][1].position - list2[k][2].position;
					Vector3 vector6 = list2[k + 1][2].position - list2[k + 1][1].position;
					float num5 = Vector2.Angle(vector5, vector6) * 0.017453292f;
					float num6 = Mathf.Sign(Vector3.Cross(vector5.normalized, vector6.normalized).z);
					float num7 = this.lineThickness / (2f * Mathf.Tan(num5 / 2f));
					Vector3 vector7 = list2[k][2].position - vector5.normalized * num7 * num6;
					Vector3 vector8 = list2[k][3].position + vector5.normalized * num7 * num6;
					UILineRendererList.JoinType joinType = this.LineJoins;
					if (joinType == UILineRendererList.JoinType.Miter)
					{
						if (num7 < vector5.magnitude / 2f && num7 < vector6.magnitude / 2f && num5 > 0.2617994f)
						{
							list2[k][2].position = vector7;
							list2[k][3].position = vector8;
							list2[k + 1][0].position = vector8;
							list2[k + 1][1].position = vector7;
						}
						else
						{
							joinType = UILineRendererList.JoinType.Bevel;
						}
					}
					if (joinType == UILineRendererList.JoinType.Bevel)
					{
						if (num7 < vector5.magnitude / 2f && num7 < vector6.magnitude / 2f && num5 > 0.5235988f)
						{
							if (num6 < 0f)
							{
								list2[k][2].position = vector7;
								list2[k + 1][1].position = vector7;
							}
							else
							{
								list2[k][3].position = vector8;
								list2[k + 1][0].position = vector8;
							}
						}
						UIVertex[] array = new UIVertex[]
						{
							list2[k][2],
							list2[k][3],
							list2[k + 1][0],
							list2[k + 1][1]
						};
						vh.AddUIVertexQuad(array);
					}
				}
				vh.AddUIVertexQuad(list2[k]);
			}
			if (vh.currentVertCount > 64000)
			{
				Debug.LogError("Max Verticies size is 64000, current mesh vertcies count is [" + vh.currentVertCount + "] - Cannot Draw");
				vh.Clear();
				return;
			}
		}

		// Token: 0x06003E98 RID: 16024 RVA: 0x0016EC59 File Offset: 0x0016CE59
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (this.m_points != null && this.m_points.Count > 0)
			{
				this.GeneratedUVs();
				vh.Clear();
				this.PopulateMesh(vh, this.m_points);
			}
		}

		// Token: 0x06003E99 RID: 16025 RVA: 0x0016EC8C File Offset: 0x0016CE8C
		private UIVertex[] CreateLineCap(Vector2 start, Vector2 end, UILineRendererList.SegmentType type)
		{
			if (type == UILineRendererList.SegmentType.Start)
			{
				Vector2 vector = start - (end - start).normalized * this.lineThickness / 2f;
				return this.CreateLineSegment(vector, start, UILineRendererList.SegmentType.Start);
			}
			if (type == UILineRendererList.SegmentType.End)
			{
				Vector2 vector2 = end + (end - start).normalized * this.lineThickness / 2f;
				return this.CreateLineSegment(end, vector2, UILineRendererList.SegmentType.End);
			}
			Debug.LogError("Bad SegmentType passed in to CreateLineCap. Must be SegmentType.Start or SegmentType.End");
			return null;
		}

		// Token: 0x06003E9A RID: 16026 RVA: 0x0016ED18 File Offset: 0x0016CF18
		private UIVertex[] CreateLineSegment(Vector2 start, Vector2 end, UILineRendererList.SegmentType type)
		{
			Vector2 vector = new Vector2(start.y - end.y, end.x - start.x).normalized * this.lineThickness / 2f;
			Vector2 vector2 = start - vector;
			Vector2 vector3 = start + vector;
			Vector2 vector4 = end + vector;
			Vector2 vector5 = end - vector;
			switch (type)
			{
			case UILineRendererList.SegmentType.Start:
				return base.SetVbo(new Vector2[] { vector2, vector3, vector4, vector5 }, UILineRendererList.startUvs);
			case UILineRendererList.SegmentType.End:
				return base.SetVbo(new Vector2[] { vector2, vector3, vector4, vector5 }, UILineRendererList.endUvs);
			case UILineRendererList.SegmentType.Full:
				return base.SetVbo(new Vector2[] { vector2, vector3, vector4, vector5 }, UILineRendererList.fullUvs);
			}
			return base.SetVbo(new Vector2[] { vector2, vector3, vector4, vector5 }, UILineRendererList.middleUvs);
		}

		// Token: 0x06003E9B RID: 16027 RVA: 0x0016EE6C File Offset: 0x0016D06C
		protected override void GeneratedUVs()
		{
			if (base.activeSprite != null)
			{
				Vector4 outerUV = DataUtility.GetOuterUV(base.activeSprite);
				Vector4 innerUV = DataUtility.GetInnerUV(base.activeSprite);
				UILineRendererList.UV_TOP_LEFT = new Vector2(outerUV.x, outerUV.y);
				UILineRendererList.UV_BOTTOM_LEFT = new Vector2(outerUV.x, outerUV.w);
				UILineRendererList.UV_TOP_CENTER_LEFT = new Vector2(innerUV.x, innerUV.y);
				UILineRendererList.UV_TOP_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.y);
				UILineRendererList.UV_BOTTOM_CENTER_LEFT = new Vector2(innerUV.x, innerUV.w);
				UILineRendererList.UV_BOTTOM_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.w);
				UILineRendererList.UV_TOP_RIGHT = new Vector2(outerUV.z, outerUV.y);
				UILineRendererList.UV_BOTTOM_RIGHT = new Vector2(outerUV.z, outerUV.w);
			}
			else
			{
				UILineRendererList.UV_TOP_LEFT = Vector2.zero;
				UILineRendererList.UV_BOTTOM_LEFT = new Vector2(0f, 1f);
				UILineRendererList.UV_TOP_CENTER_LEFT = new Vector2(0.5f, 0f);
				UILineRendererList.UV_TOP_CENTER_RIGHT = new Vector2(0.5f, 0f);
				UILineRendererList.UV_BOTTOM_CENTER_LEFT = new Vector2(0.5f, 1f);
				UILineRendererList.UV_BOTTOM_CENTER_RIGHT = new Vector2(0.5f, 1f);
				UILineRendererList.UV_TOP_RIGHT = new Vector2(1f, 0f);
				UILineRendererList.UV_BOTTOM_RIGHT = Vector2.one;
			}
			UILineRendererList.startUvs = new Vector2[]
			{
				UILineRendererList.UV_TOP_LEFT,
				UILineRendererList.UV_BOTTOM_LEFT,
				UILineRendererList.UV_BOTTOM_CENTER_LEFT,
				UILineRendererList.UV_TOP_CENTER_LEFT
			};
			UILineRendererList.middleUvs = new Vector2[]
			{
				UILineRendererList.UV_TOP_CENTER_LEFT,
				UILineRendererList.UV_BOTTOM_CENTER_LEFT,
				UILineRendererList.UV_BOTTOM_CENTER_RIGHT,
				UILineRendererList.UV_TOP_CENTER_RIGHT
			};
			UILineRendererList.endUvs = new Vector2[]
			{
				UILineRendererList.UV_TOP_CENTER_RIGHT,
				UILineRendererList.UV_BOTTOM_CENTER_RIGHT,
				UILineRendererList.UV_BOTTOM_RIGHT,
				UILineRendererList.UV_TOP_RIGHT
			};
			UILineRendererList.fullUvs = new Vector2[]
			{
				UILineRendererList.UV_TOP_LEFT,
				UILineRendererList.UV_BOTTOM_LEFT,
				UILineRendererList.UV_BOTTOM_RIGHT,
				UILineRendererList.UV_TOP_RIGHT
			};
		}

		// Token: 0x06003E9C RID: 16028 RVA: 0x0016F0D0 File Offset: 0x0016D2D0
		protected override void ResolutionToNativeSize(float distance)
		{
			if (base.UseNativeSize)
			{
				this.m_Resolution = distance / (base.activeSprite.rect.width / base.pixelsPerUnit);
				this.lineThickness = base.activeSprite.rect.height / base.pixelsPerUnit;
			}
		}

		// Token: 0x04003836 RID: 14390
		private const float MIN_MITER_JOIN = 0.2617994f;

		// Token: 0x04003837 RID: 14391
		private const float MIN_BEVEL_NICE_JOIN = 0.5235988f;

		// Token: 0x04003838 RID: 14392
		private static Vector2 UV_TOP_LEFT;

		// Token: 0x04003839 RID: 14393
		private static Vector2 UV_BOTTOM_LEFT;

		// Token: 0x0400383A RID: 14394
		private static Vector2 UV_TOP_CENTER_LEFT;

		// Token: 0x0400383B RID: 14395
		private static Vector2 UV_TOP_CENTER_RIGHT;

		// Token: 0x0400383C RID: 14396
		private static Vector2 UV_BOTTOM_CENTER_LEFT;

		// Token: 0x0400383D RID: 14397
		private static Vector2 UV_BOTTOM_CENTER_RIGHT;

		// Token: 0x0400383E RID: 14398
		private static Vector2 UV_TOP_RIGHT;

		// Token: 0x0400383F RID: 14399
		private static Vector2 UV_BOTTOM_RIGHT;

		// Token: 0x04003840 RID: 14400
		private static Vector2[] startUvs;

		// Token: 0x04003841 RID: 14401
		private static Vector2[] middleUvs;

		// Token: 0x04003842 RID: 14402
		private static Vector2[] endUvs;

		// Token: 0x04003843 RID: 14403
		private static Vector2[] fullUvs;

		// Token: 0x04003844 RID: 14404
		[SerializeField]
		[Tooltip("Points to draw lines between\n Can be improved using the Resolution Option")]
		internal List<Vector2> m_points;

		// Token: 0x04003845 RID: 14405
		[SerializeField]
		[Tooltip("Thickness of the line")]
		internal float lineThickness = 2f;

		// Token: 0x04003846 RID: 14406
		[SerializeField]
		[Tooltip("Use the relative bounds of the Rect Transform (0,0 -> 0,1) or screen space coordinates")]
		internal bool relativeSize;

		// Token: 0x04003847 RID: 14407
		[SerializeField]
		[Tooltip("Do the points identify a single line or split pairs of lines")]
		internal bool lineList;

		// Token: 0x04003848 RID: 14408
		[SerializeField]
		[Tooltip("Add end caps to each line\nMultiple caps when used with Line List")]
		internal bool lineCaps;

		// Token: 0x04003849 RID: 14409
		[SerializeField]
		[Tooltip("Resolution of the Bezier curve, different to line Resolution")]
		internal int bezierSegmentsPerCurve = 10;

		// Token: 0x0400384A RID: 14410
		[Tooltip("The type of Join used between lines, Square/Mitre or Curved/Bevel")]
		public UILineRendererList.JoinType LineJoins;

		// Token: 0x0400384B RID: 14411
		[Tooltip("Bezier method to apply to line, see docs for options\nCan't be used in conjunction with Resolution as Bezier already changes the resolution")]
		public UILineRendererList.BezierType BezierMode;

		// Token: 0x0400384C RID: 14412
		[HideInInspector]
		public bool drivenExternally;

		// Token: 0x02000F25 RID: 3877
		private enum SegmentType
		{
			// Token: 0x04004EE6 RID: 20198
			Start,
			// Token: 0x04004EE7 RID: 20199
			Middle,
			// Token: 0x04004EE8 RID: 20200
			End,
			// Token: 0x04004EE9 RID: 20201
			Full
		}

		// Token: 0x02000F26 RID: 3878
		public enum JoinType
		{
			// Token: 0x04004EEB RID: 20203
			Bevel,
			// Token: 0x04004EEC RID: 20204
			Miter
		}

		// Token: 0x02000F27 RID: 3879
		public enum BezierType
		{
			// Token: 0x04004EEE RID: 20206
			None,
			// Token: 0x04004EEF RID: 20207
			Quick,
			// Token: 0x04004EF0 RID: 20208
			Basic,
			// Token: 0x04004EF1 RID: 20209
			Improved,
			// Token: 0x04004EF2 RID: 20210
			Catenary
		}
	}
}
