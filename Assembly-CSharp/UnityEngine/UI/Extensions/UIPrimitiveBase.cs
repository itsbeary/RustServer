using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A42 RID: 2626
	public class UIPrimitiveBase : MaskableGraphic, ILayoutElement, ICanvasRaycastFilter
	{
		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x06003EAF RID: 16047 RVA: 0x0016FC01 File Offset: 0x0016DE01
		// (set) Token: 0x06003EB0 RID: 16048 RVA: 0x0016FC09 File Offset: 0x0016DE09
		public Sprite sprite
		{
			get
			{
				return this.m_Sprite;
			}
			set
			{
				if (SetPropertyUtility.SetClass<Sprite>(ref this.m_Sprite, value))
				{
					this.GeneratedUVs();
				}
				this.SetAllDirty();
			}
		}

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x06003EB1 RID: 16049 RVA: 0x0016FC25 File Offset: 0x0016DE25
		// (set) Token: 0x06003EB2 RID: 16050 RVA: 0x0016FC2D File Offset: 0x0016DE2D
		public Sprite overrideSprite
		{
			get
			{
				return this.activeSprite;
			}
			set
			{
				if (SetPropertyUtility.SetClass<Sprite>(ref this.m_OverrideSprite, value))
				{
					this.GeneratedUVs();
				}
				this.SetAllDirty();
			}
		}

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x06003EB3 RID: 16051 RVA: 0x0016FC49 File Offset: 0x0016DE49
		protected Sprite activeSprite
		{
			get
			{
				if (!(this.m_OverrideSprite != null))
				{
					return this.sprite;
				}
				return this.m_OverrideSprite;
			}
		}

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x06003EB4 RID: 16052 RVA: 0x0016FC66 File Offset: 0x0016DE66
		// (set) Token: 0x06003EB5 RID: 16053 RVA: 0x0016FC6E File Offset: 0x0016DE6E
		public float eventAlphaThreshold
		{
			get
			{
				return this.m_EventAlphaThreshold;
			}
			set
			{
				this.m_EventAlphaThreshold = value;
			}
		}

		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x06003EB6 RID: 16054 RVA: 0x0016FC77 File Offset: 0x0016DE77
		// (set) Token: 0x06003EB7 RID: 16055 RVA: 0x0016FC7F File Offset: 0x0016DE7F
		public ResolutionMode ImproveResolution
		{
			get
			{
				return this.m_improveResolution;
			}
			set
			{
				this.m_improveResolution = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x06003EB8 RID: 16056 RVA: 0x0016FC8E File Offset: 0x0016DE8E
		// (set) Token: 0x06003EB9 RID: 16057 RVA: 0x0016FC96 File Offset: 0x0016DE96
		public float Resoloution
		{
			get
			{
				return this.m_Resolution;
			}
			set
			{
				this.m_Resolution = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x06003EBA RID: 16058 RVA: 0x0016FCA5 File Offset: 0x0016DEA5
		// (set) Token: 0x06003EBB RID: 16059 RVA: 0x0016FCAD File Offset: 0x0016DEAD
		public bool UseNativeSize
		{
			get
			{
				return this.m_useNativeSize;
			}
			set
			{
				this.m_useNativeSize = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x06003EBC RID: 16060 RVA: 0x0016FCBC File Offset: 0x0016DEBC
		protected UIPrimitiveBase()
		{
			base.useLegacyMeshGeneration = false;
		}

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x06003EBD RID: 16061 RVA: 0x0016FCE1 File Offset: 0x0016DEE1
		public static Material defaultETC1GraphicMaterial
		{
			get
			{
				if (UIPrimitiveBase.s_ETC1DefaultUI == null)
				{
					UIPrimitiveBase.s_ETC1DefaultUI = Canvas.GetETC1SupportedCanvasMaterial();
				}
				return UIPrimitiveBase.s_ETC1DefaultUI;
			}
		}

		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x06003EBE RID: 16062 RVA: 0x0016FD00 File Offset: 0x0016DF00
		public override Texture mainTexture
		{
			get
			{
				if (!(this.activeSprite == null))
				{
					return this.activeSprite.texture;
				}
				if (this.material != null && this.material.mainTexture != null)
				{
					return this.material.mainTexture;
				}
				return Graphic.s_WhiteTexture;
			}
		}

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x06003EBF RID: 16063 RVA: 0x0016FD5C File Offset: 0x0016DF5C
		public bool hasBorder
		{
			get
			{
				return this.activeSprite != null && this.activeSprite.border.sqrMagnitude > 0f;
			}
		}

		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x06003EC0 RID: 16064 RVA: 0x0016FD94 File Offset: 0x0016DF94
		public float pixelsPerUnit
		{
			get
			{
				float num = 100f;
				if (this.activeSprite)
				{
					num = this.activeSprite.pixelsPerUnit;
				}
				float num2 = 100f;
				if (base.canvas)
				{
					num2 = base.canvas.referencePixelsPerUnit;
				}
				return num / num2;
			}
		}

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x06003EC1 RID: 16065 RVA: 0x0016FDE4 File Offset: 0x0016DFE4
		// (set) Token: 0x06003EC2 RID: 16066 RVA: 0x0016FE32 File Offset: 0x0016E032
		public override Material material
		{
			get
			{
				if (this.m_Material != null)
				{
					return this.m_Material;
				}
				if (this.activeSprite && this.activeSprite.associatedAlphaSplitTexture != null)
				{
					return UIPrimitiveBase.defaultETC1GraphicMaterial;
				}
				return this.defaultMaterial;
			}
			set
			{
				base.material = value;
			}
		}

		// Token: 0x06003EC3 RID: 16067 RVA: 0x0016FE3C File Offset: 0x0016E03C
		protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
		{
			UIVertex[] array = new UIVertex[4];
			for (int i = 0; i < vertices.Length; i++)
			{
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.color = this.color;
				simpleVert.position = vertices[i];
				simpleVert.uv0 = uvs[i];
				array[i] = simpleVert;
			}
			return array;
		}

		// Token: 0x06003EC4 RID: 16068 RVA: 0x0016FEA0 File Offset: 0x0016E0A0
		protected Vector2[] IncreaseResolution(Vector2[] input)
		{
			return this.IncreaseResolution(new List<Vector2>(input)).ToArray();
		}

		// Token: 0x06003EC5 RID: 16069 RVA: 0x0016FEB4 File Offset: 0x0016E0B4
		protected List<Vector2> IncreaseResolution(List<Vector2> input)
		{
			this.outputList.Clear();
			ResolutionMode improveResolution = this.ImproveResolution;
			if (improveResolution != ResolutionMode.PerSegment)
			{
				if (improveResolution == ResolutionMode.PerLine)
				{
					float num = 0f;
					for (int i = 0; i < input.Count - 1; i++)
					{
						num += Vector2.Distance(input[i], input[i + 1]);
					}
					this.ResolutionToNativeSize(num);
					float num2 = num / this.m_Resolution;
					int num3 = 0;
					for (int j = 0; j < input.Count - 1; j++)
					{
						Vector2 vector = input[j];
						this.outputList.Add(vector);
						Vector2 vector2 = input[j + 1];
						float num4 = Vector2.Distance(vector, vector2) / num2;
						float num5 = 1f / num4;
						int num6 = 0;
						while ((float)num6 < num4)
						{
							this.outputList.Add(Vector2.Lerp(vector, vector2, (float)num6 * num5));
							num3++;
							num6++;
						}
						this.outputList.Add(vector2);
					}
				}
			}
			else
			{
				for (int k = 0; k < input.Count - 1; k++)
				{
					Vector2 vector3 = input[k];
					this.outputList.Add(vector3);
					Vector2 vector4 = input[k + 1];
					this.ResolutionToNativeSize(Vector2.Distance(vector3, vector4));
					float num2 = 1f / this.m_Resolution;
					for (float num7 = 1f; num7 < this.m_Resolution; num7 += 1f)
					{
						this.outputList.Add(Vector2.Lerp(vector3, vector4, num2 * num7));
					}
					this.outputList.Add(vector4);
				}
			}
			return this.outputList;
		}

		// Token: 0x06003EC6 RID: 16070 RVA: 0x000063A5 File Offset: 0x000045A5
		protected virtual void GeneratedUVs()
		{
		}

		// Token: 0x06003EC7 RID: 16071 RVA: 0x000063A5 File Offset: 0x000045A5
		protected virtual void ResolutionToNativeSize(float distance)
		{
		}

		// Token: 0x06003EC8 RID: 16072 RVA: 0x000063A5 File Offset: 0x000045A5
		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		// Token: 0x06003EC9 RID: 16073 RVA: 0x000063A5 File Offset: 0x000045A5
		public virtual void CalculateLayoutInputVertical()
		{
		}

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x06003ECA RID: 16074 RVA: 0x00029EBC File Offset: 0x000280BC
		public virtual float minWidth
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x06003ECB RID: 16075 RVA: 0x0017006C File Offset: 0x0016E26C
		public virtual float preferredWidth
		{
			get
			{
				if (this.overrideSprite == null)
				{
					return 0f;
				}
				return this.overrideSprite.rect.size.x / this.pixelsPerUnit;
			}
		}

		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x06003ECC RID: 16076 RVA: 0x00041069 File Offset: 0x0003F269
		public virtual float flexibleWidth
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x06003ECD RID: 16077 RVA: 0x00029EBC File Offset: 0x000280BC
		public virtual float minHeight
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x06003ECE RID: 16078 RVA: 0x001700AC File Offset: 0x0016E2AC
		public virtual float preferredHeight
		{
			get
			{
				if (this.overrideSprite == null)
				{
					return 0f;
				}
				return this.overrideSprite.rect.size.y / this.pixelsPerUnit;
			}
		}

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x06003ECF RID: 16079 RVA: 0x00041069 File Offset: 0x0003F269
		public virtual float flexibleHeight
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x06003ED0 RID: 16080 RVA: 0x00007A44 File Offset: 0x00005C44
		public virtual int layoutPriority
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06003ED1 RID: 16081 RVA: 0x001700EC File Offset: 0x0016E2EC
		public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			if (this.m_EventAlphaThreshold >= 1f)
			{
				return true;
			}
			Sprite overrideSprite = this.overrideSprite;
			if (overrideSprite == null)
			{
				return true;
			}
			Vector2 vector;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, screenPoint, eventCamera, out vector);
			Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
			vector.x += base.rectTransform.pivot.x * pixelAdjustedRect.width;
			vector.y += base.rectTransform.pivot.y * pixelAdjustedRect.height;
			vector = this.MapCoordinate(vector, pixelAdjustedRect);
			Rect textureRect = overrideSprite.textureRect;
			Vector2 vector2 = new Vector2(vector.x / textureRect.width, vector.y / textureRect.height);
			float num = Mathf.Lerp(textureRect.x, textureRect.xMax, vector2.x) / (float)overrideSprite.texture.width;
			float num2 = Mathf.Lerp(textureRect.y, textureRect.yMax, vector2.y) / (float)overrideSprite.texture.height;
			bool flag;
			try
			{
				flag = overrideSprite.texture.GetPixelBilinear(num, num2).a >= this.m_EventAlphaThreshold;
			}
			catch (UnityException ex)
			{
				Debug.LogError("Using clickAlphaThreshold lower than 1 on Image whose sprite texture cannot be read. " + ex.Message + " Also make sure to disable sprite packing for this sprite.", this);
				flag = true;
			}
			return flag;
		}

		// Token: 0x06003ED2 RID: 16082 RVA: 0x00170254 File Offset: 0x0016E454
		private Vector2 MapCoordinate(Vector2 local, Rect rect)
		{
			Rect rect2 = this.sprite.rect;
			return new Vector2(local.x * rect.width, local.y * rect.height);
		}

		// Token: 0x06003ED3 RID: 16083 RVA: 0x00170284 File Offset: 0x0016E484
		private Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
		{
			for (int i = 0; i <= 1; i++)
			{
				float num = border[i] + border[i + 2];
				if (rect.size[i] < num && num != 0f)
				{
					float num2 = rect.size[i] / num;
					ref Vector4 ptr = ref border;
					int num3 = i;
					ptr[num3] *= num2;
					ptr = ref border;
					num3 = i + 2;
					ptr[num3] *= num2;
				}
			}
			return border;
		}

		// Token: 0x06003ED4 RID: 16084 RVA: 0x0017031B File Offset: 0x0016E51B
		protected override void OnEnable()
		{
			base.OnEnable();
			this.SetAllDirty();
		}

		// Token: 0x0400385D RID: 14429
		protected static Material s_ETC1DefaultUI;

		// Token: 0x0400385E RID: 14430
		private List<Vector2> outputList = new List<Vector2>();

		// Token: 0x0400385F RID: 14431
		[SerializeField]
		private Sprite m_Sprite;

		// Token: 0x04003860 RID: 14432
		[NonSerialized]
		private Sprite m_OverrideSprite;

		// Token: 0x04003861 RID: 14433
		internal float m_EventAlphaThreshold = 1f;

		// Token: 0x04003862 RID: 14434
		[SerializeField]
		private ResolutionMode m_improveResolution;

		// Token: 0x04003863 RID: 14435
		[SerializeField]
		protected float m_Resolution;

		// Token: 0x04003864 RID: 14436
		[SerializeField]
		private bool m_useNativeSize;
	}
}
