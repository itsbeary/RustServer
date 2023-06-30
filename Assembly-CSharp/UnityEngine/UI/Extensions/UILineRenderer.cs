using System;
using System.Collections.Generic;
using UnityEngine.Sprites;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A3C RID: 2620
	[AddComponentMenu("UI/Extensions/Primitives/UILineRenderer")]
	[RequireComponent(typeof(RectTransform))]
	public class UILineRenderer : UIPrimitiveBase
	{
		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x06003E6F RID: 15983 RVA: 0x0016D7B5 File Offset: 0x0016B9B5
		// (set) Token: 0x06003E70 RID: 15984 RVA: 0x0016D7BD File Offset: 0x0016B9BD
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

		// Token: 0x17000534 RID: 1332
		// (get) Token: 0x06003E71 RID: 15985 RVA: 0x0016D7CC File Offset: 0x0016B9CC
		// (set) Token: 0x06003E72 RID: 15986 RVA: 0x0016D7D4 File Offset: 0x0016B9D4
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

		// Token: 0x17000535 RID: 1333
		// (get) Token: 0x06003E73 RID: 15987 RVA: 0x0016D7E3 File Offset: 0x0016B9E3
		// (set) Token: 0x06003E74 RID: 15988 RVA: 0x0016D7EB File Offset: 0x0016B9EB
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

		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x06003E75 RID: 15989 RVA: 0x0016D7FA File Offset: 0x0016B9FA
		// (set) Token: 0x06003E76 RID: 15990 RVA: 0x0016D802 File Offset: 0x0016BA02
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

		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x06003E77 RID: 15991 RVA: 0x0016D811 File Offset: 0x0016BA11
		// (set) Token: 0x06003E78 RID: 15992 RVA: 0x0016D819 File Offset: 0x0016BA19
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

		// Token: 0x17000538 RID: 1336
		// (get) Token: 0x06003E79 RID: 15993 RVA: 0x0016D822 File Offset: 0x0016BA22
		// (set) Token: 0x06003E7A RID: 15994 RVA: 0x0016D82A File Offset: 0x0016BA2A
		public Vector2[] Points
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

		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x06003E7B RID: 15995 RVA: 0x0016D843 File Offset: 0x0016BA43
		// (set) Token: 0x06003E7C RID: 15996 RVA: 0x0016D84B File Offset: 0x0016BA4B
		public List<Vector2[]> Segments
		{
			get
			{
				return this.m_segments;
			}
			set
			{
				this.m_segments = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x06003E7D RID: 15997 RVA: 0x0016D85C File Offset: 0x0016BA5C
		private void PopulateMesh(VertexHelper vh, Vector2[] pointsToDraw)
		{
			if (this.BezierMode != UILineRenderer.BezierType.None && this.BezierMode != UILineRenderer.BezierType.Catenary && pointsToDraw.Length > 3)
			{
				BezierPath bezierPath = new BezierPath();
				bezierPath.SetControlPoints(pointsToDraw);
				bezierPath.SegmentsPerCurve = this.bezierSegmentsPerCurve;
				UILineRenderer.BezierType bezierMode = this.BezierMode;
				List<Vector2> list;
				if (bezierMode != UILineRenderer.BezierType.Basic)
				{
					if (bezierMode != UILineRenderer.BezierType.Improved)
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
				pointsToDraw = list.ToArray();
			}
			if (this.BezierMode == UILineRenderer.BezierType.Catenary && pointsToDraw.Length == 2)
			{
				pointsToDraw = new CableCurve(pointsToDraw)
				{
					slack = base.Resoloution,
					steps = this.BezierSegmentsPerCurve
				}.Points();
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
				for (int i = 1; i < pointsToDraw.Length; i += 2)
				{
					Vector2 vector = pointsToDraw[i - 1];
					Vector2 vector2 = pointsToDraw[i];
					vector = new Vector2(vector.x * num + num3, vector.y * num2 + num4);
					vector2 = new Vector2(vector2.x * num + num3, vector2.y * num2 + num4);
					if (this.lineCaps)
					{
						list2.Add(this.CreateLineCap(vector, vector2, UILineRenderer.SegmentType.Start));
					}
					list2.Add(this.CreateLineSegment(vector, vector2, UILineRenderer.SegmentType.Middle, (list2.Count > 1) ? list2[list2.Count - 2] : null));
					if (this.lineCaps)
					{
						list2.Add(this.CreateLineCap(vector, vector2, UILineRenderer.SegmentType.End));
					}
				}
			}
			else
			{
				for (int j = 1; j < pointsToDraw.Length; j++)
				{
					Vector2 vector3 = pointsToDraw[j - 1];
					Vector2 vector4 = pointsToDraw[j];
					vector3 = new Vector2(vector3.x * num + num3, vector3.y * num2 + num4);
					vector4 = new Vector2(vector4.x * num + num3, vector4.y * num2 + num4);
					if (this.lineCaps && j == 1)
					{
						list2.Add(this.CreateLineCap(vector3, vector4, UILineRenderer.SegmentType.Start));
					}
					list2.Add(this.CreateLineSegment(vector3, vector4, UILineRenderer.SegmentType.Middle, null));
					if (this.lineCaps && j == pointsToDraw.Length - 1)
					{
						list2.Add(this.CreateLineCap(vector3, vector4, UILineRenderer.SegmentType.End));
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
					UILineRenderer.JoinType joinType = this.LineJoins;
					if (joinType == UILineRenderer.JoinType.Miter)
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
							joinType = UILineRenderer.JoinType.Bevel;
						}
					}
					if (joinType == UILineRenderer.JoinType.Bevel)
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

		// Token: 0x06003E7E RID: 15998 RVA: 0x0016DE64 File Offset: 0x0016C064
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (this.m_points != null && this.m_points.Length != 0)
			{
				this.GeneratedUVs();
				vh.Clear();
				this.PopulateMesh(vh, this.m_points);
				return;
			}
			if (this.m_segments != null && this.m_segments.Count > 0)
			{
				this.GeneratedUVs();
				vh.Clear();
				for (int i = 0; i < this.m_segments.Count; i++)
				{
					Vector2[] array = this.m_segments[i];
					this.PopulateMesh(vh, array);
				}
			}
		}

		// Token: 0x06003E7F RID: 15999 RVA: 0x0016DEEC File Offset: 0x0016C0EC
		private UIVertex[] CreateLineCap(Vector2 start, Vector2 end, UILineRenderer.SegmentType type)
		{
			if (type == UILineRenderer.SegmentType.Start)
			{
				Vector2 vector = start - (end - start).normalized * this.lineThickness / 2f;
				return this.CreateLineSegment(vector, start, UILineRenderer.SegmentType.Start, null);
			}
			if (type == UILineRenderer.SegmentType.End)
			{
				Vector2 vector2 = end + (end - start).normalized * this.lineThickness / 2f;
				return this.CreateLineSegment(end, vector2, UILineRenderer.SegmentType.End, null);
			}
			Debug.LogError("Bad SegmentType passed in to CreateLineCap. Must be SegmentType.Start or SegmentType.End");
			return null;
		}

		// Token: 0x06003E80 RID: 16000 RVA: 0x0016DF78 File Offset: 0x0016C178
		private UIVertex[] CreateLineSegment(Vector2 start, Vector2 end, UILineRenderer.SegmentType type, UIVertex[] previousVert = null)
		{
			Vector2 vector = new Vector2(start.y - end.y, end.x - start.x).normalized * this.lineThickness / 2f;
			Vector2 vector2 = Vector2.zero;
			Vector2 vector3 = Vector2.zero;
			if (previousVert != null)
			{
				vector2 = new Vector2(previousVert[3].position.x, previousVert[3].position.y);
				vector3 = new Vector2(previousVert[2].position.x, previousVert[2].position.y);
			}
			else
			{
				vector2 = start - vector;
				vector3 = start + vector;
			}
			Vector2 vector4 = end + vector;
			Vector2 vector5 = end - vector;
			switch (type)
			{
			case UILineRenderer.SegmentType.Start:
				return base.SetVbo(new Vector2[] { vector2, vector3, vector4, vector5 }, UILineRenderer.startUvs);
			case UILineRenderer.SegmentType.End:
				return base.SetVbo(new Vector2[] { vector2, vector3, vector4, vector5 }, UILineRenderer.endUvs);
			case UILineRenderer.SegmentType.Full:
				return base.SetVbo(new Vector2[] { vector2, vector3, vector4, vector5 }, UILineRenderer.fullUvs);
			}
			return base.SetVbo(new Vector2[] { vector2, vector3, vector4, vector5 }, UILineRenderer.middleUvs);
		}

		// Token: 0x06003E81 RID: 16001 RVA: 0x0016E134 File Offset: 0x0016C334
		protected override void GeneratedUVs()
		{
			if (base.activeSprite != null)
			{
				Vector4 outerUV = DataUtility.GetOuterUV(base.activeSprite);
				Vector4 innerUV = DataUtility.GetInnerUV(base.activeSprite);
				UILineRenderer.UV_TOP_LEFT = new Vector2(outerUV.x, outerUV.y);
				UILineRenderer.UV_BOTTOM_LEFT = new Vector2(outerUV.x, outerUV.w);
				UILineRenderer.UV_TOP_CENTER_LEFT = new Vector2(innerUV.x, innerUV.y);
				UILineRenderer.UV_TOP_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.y);
				UILineRenderer.UV_BOTTOM_CENTER_LEFT = new Vector2(innerUV.x, innerUV.w);
				UILineRenderer.UV_BOTTOM_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.w);
				UILineRenderer.UV_TOP_RIGHT = new Vector2(outerUV.z, outerUV.y);
				UILineRenderer.UV_BOTTOM_RIGHT = new Vector2(outerUV.z, outerUV.w);
			}
			else
			{
				UILineRenderer.UV_TOP_LEFT = Vector2.zero;
				UILineRenderer.UV_BOTTOM_LEFT = new Vector2(0f, 1f);
				UILineRenderer.UV_TOP_CENTER_LEFT = new Vector2(0.5f, 0f);
				UILineRenderer.UV_TOP_CENTER_RIGHT = new Vector2(0.5f, 0f);
				UILineRenderer.UV_BOTTOM_CENTER_LEFT = new Vector2(0.5f, 1f);
				UILineRenderer.UV_BOTTOM_CENTER_RIGHT = new Vector2(0.5f, 1f);
				UILineRenderer.UV_TOP_RIGHT = new Vector2(1f, 0f);
				UILineRenderer.UV_BOTTOM_RIGHT = Vector2.one;
			}
			UILineRenderer.startUvs = new Vector2[]
			{
				UILineRenderer.UV_TOP_LEFT,
				UILineRenderer.UV_BOTTOM_LEFT,
				UILineRenderer.UV_BOTTOM_CENTER_LEFT,
				UILineRenderer.UV_TOP_CENTER_LEFT
			};
			UILineRenderer.middleUvs = new Vector2[]
			{
				UILineRenderer.UV_TOP_CENTER_LEFT,
				UILineRenderer.UV_BOTTOM_CENTER_LEFT,
				UILineRenderer.UV_BOTTOM_CENTER_RIGHT,
				UILineRenderer.UV_TOP_CENTER_RIGHT
			};
			UILineRenderer.endUvs = new Vector2[]
			{
				UILineRenderer.UV_TOP_CENTER_RIGHT,
				UILineRenderer.UV_BOTTOM_CENTER_RIGHT,
				UILineRenderer.UV_BOTTOM_RIGHT,
				UILineRenderer.UV_TOP_RIGHT
			};
			UILineRenderer.fullUvs = new Vector2[]
			{
				UILineRenderer.UV_TOP_LEFT,
				UILineRenderer.UV_BOTTOM_LEFT,
				UILineRenderer.UV_BOTTOM_RIGHT,
				UILineRenderer.UV_TOP_RIGHT
			};
		}

		// Token: 0x06003E82 RID: 16002 RVA: 0x0016E398 File Offset: 0x0016C598
		protected override void ResolutionToNativeSize(float distance)
		{
			if (base.UseNativeSize)
			{
				this.m_Resolution = distance / (base.activeSprite.rect.width / base.pixelsPerUnit);
				this.lineThickness = base.activeSprite.rect.height / base.pixelsPerUnit;
			}
		}

		// Token: 0x06003E83 RID: 16003 RVA: 0x0016E3F0 File Offset: 0x0016C5F0
		private int GetSegmentPointCount()
		{
			List<Vector2[]> segments = this.Segments;
			if (segments != null && segments.Count > 0)
			{
				int num = 0;
				foreach (Vector2[] array in this.Segments)
				{
					num += array.Length;
				}
				return num;
			}
			return this.Points.Length;
		}

		// Token: 0x06003E84 RID: 16004 RVA: 0x0016E468 File Offset: 0x0016C668
		public Vector2 GetPosition(int index, int segmentIndex = 0)
		{
			if (segmentIndex > 0)
			{
				return this.Segments[segmentIndex - 1][index - 1];
			}
			if (this.Segments.Count > 0)
			{
				int num = 0;
				int num2 = index;
				foreach (Vector2[] array in this.Segments)
				{
					if (num2 - array.Length <= 0)
					{
						break;
					}
					num2 -= array.Length;
					num++;
				}
				return this.Segments[num][num2 - 1];
			}
			return this.Points[index - 1];
		}

		// Token: 0x06003E85 RID: 16005 RVA: 0x0016E518 File Offset: 0x0016C718
		public Vector2 GetPositionBySegment(int index, int segment)
		{
			return this.Segments[segment][index - 1];
		}

		// Token: 0x06003E86 RID: 16006 RVA: 0x0016E530 File Offset: 0x0016C730
		public Vector2 GetClosestPoint(Vector2 p1, Vector2 p2, Vector2 p3)
		{
			Vector2 vector = p3 - p1;
			Vector2 vector2 = p2 - p1;
			float num = Mathf.Clamp01(Vector2.Dot(vector, vector2.normalized) / vector2.magnitude);
			return p1 + vector2 * num;
		}

		// Token: 0x0400381E RID: 14366
		private const float MIN_MITER_JOIN = 0.2617994f;

		// Token: 0x0400381F RID: 14367
		private const float MIN_BEVEL_NICE_JOIN = 0.5235988f;

		// Token: 0x04003820 RID: 14368
		private static Vector2 UV_TOP_LEFT;

		// Token: 0x04003821 RID: 14369
		private static Vector2 UV_BOTTOM_LEFT;

		// Token: 0x04003822 RID: 14370
		private static Vector2 UV_TOP_CENTER_LEFT;

		// Token: 0x04003823 RID: 14371
		private static Vector2 UV_TOP_CENTER_RIGHT;

		// Token: 0x04003824 RID: 14372
		private static Vector2 UV_BOTTOM_CENTER_LEFT;

		// Token: 0x04003825 RID: 14373
		private static Vector2 UV_BOTTOM_CENTER_RIGHT;

		// Token: 0x04003826 RID: 14374
		private static Vector2 UV_TOP_RIGHT;

		// Token: 0x04003827 RID: 14375
		private static Vector2 UV_BOTTOM_RIGHT;

		// Token: 0x04003828 RID: 14376
		private static Vector2[] startUvs;

		// Token: 0x04003829 RID: 14377
		private static Vector2[] middleUvs;

		// Token: 0x0400382A RID: 14378
		private static Vector2[] endUvs;

		// Token: 0x0400382B RID: 14379
		private static Vector2[] fullUvs;

		// Token: 0x0400382C RID: 14380
		[SerializeField]
		[Tooltip("Points to draw lines between\n Can be improved using the Resolution Option")]
		internal Vector2[] m_points;

		// Token: 0x0400382D RID: 14381
		[SerializeField]
		[Tooltip("Segments to be drawn\n This is a list of arrays of points")]
		internal List<Vector2[]> m_segments;

		// Token: 0x0400382E RID: 14382
		[SerializeField]
		[Tooltip("Thickness of the line")]
		internal float lineThickness = 2f;

		// Token: 0x0400382F RID: 14383
		[SerializeField]
		[Tooltip("Use the relative bounds of the Rect Transform (0,0 -> 0,1) or screen space coordinates")]
		internal bool relativeSize;

		// Token: 0x04003830 RID: 14384
		[SerializeField]
		[Tooltip("Do the points identify a single line or split pairs of lines")]
		internal bool lineList;

		// Token: 0x04003831 RID: 14385
		[SerializeField]
		[Tooltip("Add end caps to each line\nMultiple caps when used with Line List")]
		internal bool lineCaps;

		// Token: 0x04003832 RID: 14386
		[SerializeField]
		[Tooltip("Resolution of the Bezier curve, different to line Resolution")]
		internal int bezierSegmentsPerCurve = 10;

		// Token: 0x04003833 RID: 14387
		[Tooltip("The type of Join used between lines, Square/Mitre or Curved/Bevel")]
		public UILineRenderer.JoinType LineJoins;

		// Token: 0x04003834 RID: 14388
		[Tooltip("Bezier method to apply to line, see docs for options\nCan't be used in conjunction with Resolution as Bezier already changes the resolution")]
		public UILineRenderer.BezierType BezierMode;

		// Token: 0x04003835 RID: 14389
		[HideInInspector]
		public bool drivenExternally;

		// Token: 0x02000F22 RID: 3874
		private enum SegmentType
		{
			// Token: 0x04004ED8 RID: 20184
			Start,
			// Token: 0x04004ED9 RID: 20185
			Middle,
			// Token: 0x04004EDA RID: 20186
			End,
			// Token: 0x04004EDB RID: 20187
			Full
		}

		// Token: 0x02000F23 RID: 3875
		public enum JoinType
		{
			// Token: 0x04004EDD RID: 20189
			Bevel,
			// Token: 0x04004EDE RID: 20190
			Miter
		}

		// Token: 0x02000F24 RID: 3876
		public enum BezierType
		{
			// Token: 0x04004EE0 RID: 20192
			None,
			// Token: 0x04004EE1 RID: 20193
			Quick,
			// Token: 0x04004EE2 RID: 20194
			Basic,
			// Token: 0x04004EE3 RID: 20195
			Improved,
			// Token: 0x04004EE4 RID: 20196
			Catenary
		}
	}
}
