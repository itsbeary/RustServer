using System;
using UnityEngine;

// Token: 0x02000475 RID: 1141
public class Buoyancy : ListComponent<Buoyancy>, IServerComponent
{
	// Token: 0x17000316 RID: 790
	// (get) Token: 0x060025C6 RID: 9670 RVA: 0x000EE2E5 File Offset: 0x000EC4E5
	// (set) Token: 0x060025C7 RID: 9671 RVA: 0x000EE2ED File Offset: 0x000EC4ED
	public float timeOutOfWater { get; private set; }

	// Token: 0x060025C8 RID: 9672 RVA: 0x000EE2F6 File Offset: 0x000EC4F6
	public static string DefaultWaterImpact()
	{
		return "assets/bundled/prefabs/fx/impacts/physics/water-enter-exit.prefab";
	}

	// Token: 0x060025C9 RID: 9673 RVA: 0x000EE2FD File Offset: 0x000EC4FD
	private void Awake()
	{
		base.InvokeRandomized(new Action(this.CheckSleepState), 0.5f, 5f, 1f);
	}

	// Token: 0x060025CA RID: 9674 RVA: 0x000EE320 File Offset: 0x000EC520
	public void Sleep()
	{
		if (this.rigidBody != null)
		{
			this.rigidBody.Sleep();
		}
		base.enabled = false;
	}

	// Token: 0x060025CB RID: 9675 RVA: 0x000EE342 File Offset: 0x000EC542
	public void Wake()
	{
		if (this.rigidBody != null)
		{
			this.rigidBody.WakeUp();
		}
		base.enabled = true;
	}

	// Token: 0x060025CC RID: 9676 RVA: 0x000EE364 File Offset: 0x000EC564
	public void CheckSleepState()
	{
		if (base.transform == null)
		{
			return;
		}
		if (this.rigidBody == null)
		{
			return;
		}
		bool flag = BaseNetworkable.HasCloseConnections(base.transform.position, 100f);
		if (base.enabled && (this.rigidBody.IsSleeping() || (!flag && this.timeInWater > 6f)))
		{
			base.Invoke(new Action(this.Sleep), 0f);
			return;
		}
		if (!base.enabled && (!this.rigidBody.IsSleeping() || (flag && this.timeInWater > 0f)))
		{
			base.Invoke(new Action(this.Wake), 0f);
		}
	}

	// Token: 0x060025CD RID: 9677 RVA: 0x000EE420 File Offset: 0x000EC620
	protected void DoCycle()
	{
		bool flag = this.submergedFraction > 0f;
		this.BuoyancyFixedUpdate();
		bool flag2 = this.submergedFraction > 0f;
		if (flag != flag2)
		{
			if (this.useUnderwaterDrag && this.rigidBody != null)
			{
				if (flag2)
				{
					this.defaultDrag = this.rigidBody.drag;
					this.defaultAngularDrag = this.rigidBody.angularDrag;
					this.rigidBody.drag = this.underwaterDrag;
					this.rigidBody.angularDrag = this.underwaterDrag;
				}
				else
				{
					this.rigidBody.drag = this.defaultDrag;
					this.rigidBody.angularDrag = this.defaultAngularDrag;
				}
			}
			if (this.SubmergedChanged != null)
			{
				this.SubmergedChanged(flag2);
			}
		}
	}

	// Token: 0x060025CE RID: 9678 RVA: 0x000EE4EC File Offset: 0x000EC6EC
	public static void Cycle()
	{
		Buoyancy[] buffer = ListComponent<Buoyancy>.InstanceList.Values.Buffer;
		int count = ListComponent<Buoyancy>.InstanceList.Count;
		for (int i = 0; i < count; i++)
		{
			buffer[i].DoCycle();
		}
	}

	// Token: 0x060025CF RID: 9679 RVA: 0x000EE528 File Offset: 0x000EC728
	public Vector3 GetFlowDirection(Vector2 posUV)
	{
		if (TerrainMeta.WaterMap == null)
		{
			return Vector3.zero;
		}
		Vector3 normalFast = TerrainMeta.WaterMap.GetNormalFast(posUV);
		float num = Mathf.Clamp01(Mathf.Abs(normalFast.y));
		normalFast.y = 0f;
		normalFast.FastRenormalize(num);
		return normalFast;
	}

	// Token: 0x060025D0 RID: 9680 RVA: 0x000EE57C File Offset: 0x000EC77C
	public void EnsurePointsInitialized()
	{
		if (this.points == null || this.points.Length == 0)
		{
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (component != null)
			{
				BuoyancyPoint buoyancyPoint = new GameObject("BuoyancyPoint")
				{
					transform = 
					{
						parent = component.gameObject.transform,
						localPosition = component.centerOfMass
					}
				}.AddComponent<BuoyancyPoint>();
				buoyancyPoint.buoyancyForce = component.mass * -Physics.gravity.y;
				buoyancyPoint.buoyancyForce *= 1.32f;
				buoyancyPoint.size = 0.2f;
				this.points = new BuoyancyPoint[1];
				this.points[0] = buoyancyPoint;
			}
		}
		if (this.pointData == null || this.pointData.Length != this.points.Length)
		{
			this.pointData = new Buoyancy.BuoyancyPointData[this.points.Length];
			this.pointPositionArray = new Vector2[this.points.Length];
			this.pointPositionUVArray = new Vector2[this.points.Length];
			this.pointShoreVectorArray = new Vector3[this.points.Length];
			this.pointTerrainHeightArray = new float[this.points.Length];
			this.pointWaterHeightArray = new float[this.points.Length];
			for (int i = 0; i < this.points.Length; i++)
			{
				Transform transform = this.points[i].transform;
				Transform parent = transform.parent;
				transform.SetParent(base.transform);
				Vector3 localPosition = transform.localPosition;
				transform.SetParent(parent);
				this.pointData[i].transform = transform;
				this.pointData[i].localPosition = transform.localPosition;
				this.pointData[i].rootToPoint = localPosition;
			}
		}
	}

	// Token: 0x060025D1 RID: 9681 RVA: 0x000EE744 File Offset: 0x000EC944
	public void BuoyancyFixedUpdate()
	{
		if (TerrainMeta.WaterMap == null)
		{
			return;
		}
		this.EnsurePointsInitialized();
		if (this.rigidBody == null)
		{
			return;
		}
		if (this.buoyancyScale == 0f)
		{
			base.Invoke(new Action(this.Sleep), 0f);
			return;
		}
		float time = Time.time;
		float x = TerrainMeta.Position.x;
		float z = TerrainMeta.Position.z;
		float x2 = TerrainMeta.OneOverSize.x;
		float z2 = TerrainMeta.OneOverSize.z;
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		for (int i = 0; i < this.pointData.Length; i++)
		{
			BuoyancyPoint buoyancyPoint = this.points[i];
			Vector3 vector = localToWorldMatrix.MultiplyPoint3x4(this.pointData[i].rootToPoint);
			this.pointData[i].position = vector;
			float num = (vector.x - x) * x2;
			float num2 = (vector.z - z) * z2;
			this.pointPositionArray[i] = new Vector2(vector.x, vector.z);
			this.pointPositionUVArray[i] = new Vector2(num, num2);
		}
		WaterSystem.GetHeightArray(this.pointPositionArray, this.pointPositionUVArray, this.pointShoreVectorArray, this.pointTerrainHeightArray, this.pointWaterHeightArray);
		int num3 = 0;
		for (int j = 0; j < this.points.Length; j++)
		{
			BuoyancyPoint buoyancyPoint2 = this.points[j];
			Vector3 position = this.pointData[j].position;
			Vector3 localPosition = this.pointData[j].localPosition;
			Vector2 vector2 = this.pointPositionUVArray[j];
			float num4 = this.pointTerrainHeightArray[j];
			float num5 = this.pointWaterHeightArray[j];
			if (this.ArtificialHeight != null)
			{
				num5 = this.ArtificialHeight.Value;
			}
			bool flag = this.ArtificialHeight == null;
			WaterLevel.WaterInfo buoyancyWaterInfo = WaterLevel.GetBuoyancyWaterInfo(position, vector2, num4, num5, flag, this.forEntity);
			bool flag2 = false;
			if (position.y < buoyancyWaterInfo.surfaceLevel && buoyancyWaterInfo.isValid)
			{
				flag2 = true;
				num3++;
				float currentDepth = buoyancyWaterInfo.currentDepth;
				float num6 = Mathf.InverseLerp(0f, buoyancyPoint2.size, currentDepth);
				float num7 = 1f + Mathf.PerlinNoise(buoyancyPoint2.randomOffset + time * buoyancyPoint2.waveFrequency, 0f) * buoyancyPoint2.waveScale;
				float num8 = buoyancyPoint2.buoyancyForce * this.buoyancyScale;
				Vector3 vector3 = new Vector3(0f, num7 * num6 * num8, 0f);
				Vector3 flowDirection = this.GetFlowDirection(vector2);
				if (flowDirection.y < 0.9999f && flowDirection != Vector3.up)
				{
					num8 *= 0.25f;
					vector3.x += flowDirection.x * num8 * this.flowMovementScale;
					vector3.y += flowDirection.y * num8 * this.flowMovementScale;
					vector3.z += flowDirection.z * num8 * this.flowMovementScale;
				}
				this.rigidBody.AddForceAtPosition(vector3, position, ForceMode.Force);
			}
			if (buoyancyPoint2.doSplashEffects && ((!buoyancyPoint2.wasSubmergedLastFrame && flag2) || (!flag2 && buoyancyPoint2.wasSubmergedLastFrame)) && this.doEffects && this.rigidBody.GetRelativePointVelocity(localPosition).magnitude > 1f)
			{
				string text = ((this.waterImpacts != null && this.waterImpacts.Length != 0 && this.waterImpacts[0].isValid) ? this.waterImpacts[0].resourcePath : Buoyancy.DefaultWaterImpact());
				Vector3 vector4 = new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), 0f, UnityEngine.Random.Range(-0.25f, 0.25f));
				Effect.server.Run(text, position + vector4, Vector3.up, null, false);
				buoyancyPoint2.nexSplashTime = Time.time + 0.25f;
			}
			buoyancyPoint2.wasSubmergedLastFrame = flag2;
		}
		if (this.points.Length != 0)
		{
			this.submergedFraction = (float)num3 / (float)this.points.Length;
		}
		if (this.submergedFraction > this.requiredSubmergedFraction)
		{
			this.timeInWater += Time.fixedDeltaTime;
			this.timeOutOfWater = 0f;
			return;
		}
		this.timeOutOfWater += Time.fixedDeltaTime;
		this.timeInWater = 0f;
	}

	// Token: 0x04001DE0 RID: 7648
	public BuoyancyPoint[] points;

	// Token: 0x04001DE1 RID: 7649
	public GameObjectRef[] waterImpacts;

	// Token: 0x04001DE2 RID: 7650
	public Rigidbody rigidBody;

	// Token: 0x04001DE3 RID: 7651
	public float buoyancyScale = 1f;

	// Token: 0x04001DE4 RID: 7652
	public bool doEffects = true;

	// Token: 0x04001DE5 RID: 7653
	public float flowMovementScale = 1f;

	// Token: 0x04001DE6 RID: 7654
	public float requiredSubmergedFraction;

	// Token: 0x04001DE7 RID: 7655
	public bool useUnderwaterDrag;

	// Token: 0x04001DE8 RID: 7656
	[Range(0f, 3f)]
	public float underwaterDrag = 2f;

	// Token: 0x04001DEA RID: 7658
	public Action<bool> SubmergedChanged;

	// Token: 0x04001DEB RID: 7659
	public BaseEntity forEntity;

	// Token: 0x04001DEC RID: 7660
	[NonSerialized]
	public float submergedFraction;

	// Token: 0x04001DED RID: 7661
	private Buoyancy.BuoyancyPointData[] pointData;

	// Token: 0x04001DEE RID: 7662
	private Vector2[] pointPositionArray;

	// Token: 0x04001DEF RID: 7663
	private Vector2[] pointPositionUVArray;

	// Token: 0x04001DF0 RID: 7664
	private Vector3[] pointShoreVectorArray;

	// Token: 0x04001DF1 RID: 7665
	private float[] pointTerrainHeightArray;

	// Token: 0x04001DF2 RID: 7666
	private float[] pointWaterHeightArray;

	// Token: 0x04001DF3 RID: 7667
	private float defaultDrag;

	// Token: 0x04001DF4 RID: 7668
	private float defaultAngularDrag;

	// Token: 0x04001DF5 RID: 7669
	private float timeInWater;

	// Token: 0x04001DF6 RID: 7670
	public float? ArtificialHeight;

	// Token: 0x04001DF7 RID: 7671
	public float waveHeightScale = 0.5f;

	// Token: 0x02000D11 RID: 3345
	private struct BuoyancyPointData
	{
		// Token: 0x040046A0 RID: 18080
		public Transform transform;

		// Token: 0x040046A1 RID: 18081
		public Vector3 localPosition;

		// Token: 0x040046A2 RID: 18082
		public Vector3 rootToPoint;

		// Token: 0x040046A3 RID: 18083
		public Vector3 position;
	}
}
