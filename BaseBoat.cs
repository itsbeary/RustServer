using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;
using VacuumBreather;

// Token: 0x02000474 RID: 1140
public class BaseBoat : BaseVehicle
{
	// Token: 0x060025B4 RID: 9652 RVA: 0x000ED8DC File Offset: 0x000EBADC
	public bool InDryDock()
	{
		return base.GetParentEntity() != null;
	}

	// Token: 0x060025B5 RID: 9653 RVA: 0x000ED8EA File Offset: 0x000EBAEA
	public override float MaxVelocity()
	{
		return 25f;
	}

	// Token: 0x060025B6 RID: 9654 RVA: 0x000ED8F4 File Offset: 0x000EBAF4
	public override void ServerInit()
	{
		base.ServerInit();
		this.rigidBody.isKinematic = false;
		if (this.rigidBody == null)
		{
			Debug.LogWarning("Boat rigidbody null");
			return;
		}
		if (this.centerOfMass == null)
		{
			Debug.LogWarning("boat COM null");
			return;
		}
		this.rigidBody.centerOfMass = this.centerOfMass.localPosition;
		if (this.planeFitPoints == null || this.planeFitPoints.Length != 3)
		{
			Debug.LogWarning("Boats require 3 plane fit points");
			return;
		}
		this.worldAnchors = new Vector3[3];
		this.pidController = new PidQuaternionController(this.wavePID.x, this.wavePID.y, this.wavePID.z);
	}

	// Token: 0x060025B7 RID: 9655 RVA: 0x000ED9B1 File Offset: 0x000EBBB1
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (base.IsDriver(player))
		{
			this.DriverInput(inputState, player);
		}
	}

	// Token: 0x060025B8 RID: 9656 RVA: 0x000ED9C4 File Offset: 0x000EBBC4
	public virtual void DriverInput(InputState inputState, BasePlayer player)
	{
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			this.gasPedal = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			this.gasPedal = -0.5f;
		}
		else
		{
			this.gasPedal = 0f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			this.steering = 1f;
			return;
		}
		if (inputState.IsDown(BUTTON.RIGHT))
		{
			this.steering = -1f;
			return;
		}
		this.steering = 0f;
	}

	// Token: 0x060025B9 RID: 9657 RVA: 0x00029C50 File Offset: 0x00027E50
	public void OnPoolDestroyed()
	{
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x060025BA RID: 9658 RVA: 0x0004DDC7 File Offset: 0x0004BFC7
	public void WakeUp()
	{
		if (this.rigidBody != null)
		{
			this.rigidBody.WakeUp();
			this.rigidBody.AddForce(Vector3.up * 0.1f, ForceMode.Impulse);
		}
	}

	// Token: 0x060025BB RID: 9659 RVA: 0x000EDA3E File Offset: 0x000EBC3E
	protected override void OnServerWake()
	{
		if (this.buoyancy != null)
		{
			this.buoyancy.Wake();
		}
	}

	// Token: 0x060025BC RID: 9660 RVA: 0x000EDA59 File Offset: 0x000EBC59
	public virtual bool EngineOn()
	{
		return base.HasDriver() && !base.IsFlipped();
	}

	// Token: 0x060025BD RID: 9661 RVA: 0x000EDA70 File Offset: 0x000EBC70
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (!this.EngineOn())
		{
			this.gasPedal = 0f;
			this.steering = 0f;
		}
		base.VehicleFixedUpdate();
		this.ApplyCorrectionForces();
		bool flag = WaterLevel.Test(this.thrustPoint.position, true, true, this);
		if (this.gasPedal != 0f && flag && this.buoyancy.submergedFraction > 0.3f)
		{
			Vector3 vector = (base.transform.forward + base.transform.right * this.steering * this.steeringScale).normalized * this.gasPedal * this.engineThrust;
			this.rigidBody.AddForceAtPosition(vector, this.thrustPoint.position, ForceMode.Force);
		}
		if (this.AnyMounted() && base.IsFlipped())
		{
			this.DismountAllPlayers();
		}
	}

	// Token: 0x060025BE RID: 9662 RVA: 0x000EDB68 File Offset: 0x000EBD68
	protected void ApplyCorrectionForces()
	{
		if (this.planeFitPoints == null || this.planeFitPoints.Length != 3)
		{
			return;
		}
		if (!base.HasDriver())
		{
			return;
		}
		Matrix4x4 matrix4x = Matrix4x4.TRS(base.transform.position, Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f), Vector3.one);
		for (int i = 0; i < this.planeFitPoints.Length; i++)
		{
			Vector3 vector = matrix4x.MultiplyPoint(this.planeFitPoints[i].localPosition);
			vector.y = WaterSystem.GetHeight(vector);
			this.worldAnchors[i] = vector;
		}
		Plane plane = new Plane(this.worldAnchors[0], this.worldAnchors[1], this.worldAnchors[2]);
		Vector3 normal = plane.normal;
		Vector3 vector2 = Vector3.Normalize(this.worldAnchors[2] - this.worldAnchors[1]);
		Quaternion quaternion = Quaternion.LookRotation(Vector3.Cross(normal, vector2), normal);
		float y = this.planeFitPoints[0].localPosition.y;
		float num = (this.worldAnchors[0].y + this.worldAnchors[1].y + this.worldAnchors[2].y) / 3f - y;
		float y2 = base.transform.position.y;
		float num2 = num - y2;
		Vector3 velocity = this.rigidBody.velocity;
		if (y2 > num + this.correctionRange.x && y2 < num + this.correctionRange.y)
		{
			float num3 = num2 * this.correctionSpringForce;
			float num4 = -velocity.y * this.correctionSpringDamping;
			this.rigidBody.AddForce(0f, num3 + num4, 0f, ForceMode.Force);
		}
		if (y2 > num + this.correctionRange.y)
		{
			quaternion = Quaternion.Euler(this.inAirDesiredPitch, base.transform.eulerAngles.y, 0f);
			this.pidController.Kp = this.inAirPID.x;
			this.pidController.Ki = this.inAirPID.y;
			this.pidController.Kd = this.inAirPID.z;
			Vector3 vector3 = this.pidController.ComputeRequiredAngularAcceleration(base.transform.rotation, quaternion, this.rigidBody.angularVelocity, UnityEngine.Time.fixedDeltaTime);
			this.rigidBody.AddTorque(vector3, ForceMode.Acceleration);
			return;
		}
		if (y2 > num + this.correctionRange.x)
		{
			this.pidController.Kp = this.wavePID.x;
			this.pidController.Ki = this.wavePID.y;
			this.pidController.Kd = this.wavePID.z;
			Vector3 vector4 = this.pidController.ComputeRequiredAngularAcceleration(base.transform.rotation, quaternion, this.rigidBody.angularVelocity, UnityEngine.Time.fixedDeltaTime);
			vector4.y = 0f;
			this.rigidBody.AddTorque(vector4, ForceMode.Acceleration);
		}
	}

	// Token: 0x060025BF RID: 9663 RVA: 0x000EDE9C File Offset: 0x000EC09C
	public static void WaterVehicleDecay(BaseCombatEntity entity, float decayTickRate, float timeSinceLastUsed, float outsideDecayMinutes, float deepWaterDecayMinutes)
	{
		if (entity.healthFraction == 0f)
		{
			return;
		}
		if (timeSinceLastUsed < 2700f)
		{
			return;
		}
		float overallWaterDepth = WaterLevel.GetOverallWaterDepth(entity.transform.position, true, false, null, false);
		float num = (entity.IsOutside() ? outsideDecayMinutes : float.PositiveInfinity);
		if (overallWaterDepth > 12f)
		{
			float num2 = Mathf.InverseLerp(12f, 16f, overallWaterDepth);
			float num3 = Mathf.Lerp(0.1f, 1f, num2);
			num = Mathf.Min(num, deepWaterDecayMinutes / num3);
		}
		if (!float.IsPositiveInfinity(num))
		{
			float num4 = decayTickRate / 60f / num;
			entity.Hurt(entity.MaxHealth() * num4, DamageType.Decay, entity, false);
		}
	}

	// Token: 0x060025C0 RID: 9664 RVA: 0x000EDF41 File Offset: 0x000EC141
	public virtual bool EngineInWater()
	{
		return TerrainMeta.WaterMap.GetHeight(this.thrustPoint.position) > this.thrustPoint.position.y;
	}

	// Token: 0x060025C1 RID: 9665 RVA: 0x000EDF6A File Offset: 0x000EC16A
	public override float WaterFactorForPlayer(BasePlayer player)
	{
		if (TerrainMeta.WaterMap.GetHeight(player.eyes.position) >= player.eyes.position.y)
		{
			return 1f;
		}
		return 0f;
	}

	// Token: 0x060025C2 RID: 9666 RVA: 0x000EDFA4 File Offset: 0x000EC1A4
	public static float GetWaterDepth(Vector3 pos)
	{
		if (UnityEngine.Application.isPlaying && !(TerrainMeta.WaterMap == null))
		{
			return TerrainMeta.WaterMap.GetDepth(pos);
		}
		RaycastHit raycastHit;
		if (!UnityEngine.Physics.Raycast(pos, Vector3.down, out raycastHit, 100f, 8388608))
		{
			return 100f;
		}
		return raycastHit.distance;
	}

	// Token: 0x060025C3 RID: 9667 RVA: 0x000EDFF8 File Offset: 0x000EC1F8
	public static List<Vector3> GenerateOceanPatrolPath(float minDistanceFromShore = 50f, float minWaterDepth = 8f)
	{
		float x = TerrainMeta.Size.x;
		float num = x * 2f * 3.1415927f;
		float num2 = 30f;
		int num3 = Mathf.CeilToInt(num / num2);
		List<Vector3> list = new List<Vector3>();
		float num4 = x;
		float num5 = 0f;
		for (int i = 0; i < num3; i++)
		{
			float num6 = (float)i / (float)num3 * 360f;
			list.Add(new Vector3(Mathf.Sin(num6 * 0.017453292f) * num4, num5, Mathf.Cos(num6 * 0.017453292f) * num4));
		}
		float num7 = 4f;
		float num8 = 200f;
		bool flag = true;
		int num9 = 0;
		while (num9 < AI.ocean_patrol_path_iterations && flag)
		{
			flag = false;
			for (int j = 0; j < num3; j++)
			{
				Vector3 vector = list[j];
				int num10 = ((j == 0) ? (num3 - 1) : (j - 1));
				int num11 = ((j == num3 - 1) ? 0 : (j + 1));
				Vector3 vector2 = list[num11];
				Vector3 vector3 = list[num10];
				Vector3 vector4 = vector;
				Vector3 normalized = (Vector3.zero - vector).normalized;
				Vector3 vector5 = vector + normalized * num7;
				if (Vector3.Distance(vector5, vector2) <= num8 && Vector3.Distance(vector5, vector3) <= num8)
				{
					bool flag2 = true;
					int num12 = 16;
					for (int k = 0; k < num12; k++)
					{
						float num13 = (float)k / (float)num12 * 360f;
						Vector3 normalized2 = new Vector3(Mathf.Sin(num13 * 0.017453292f), num5, Mathf.Cos(num13 * 0.017453292f)).normalized;
						Vector3 vector6 = vector5 + normalized2 * 1f;
						BaseBoat.GetWaterDepth(vector6);
						Vector3 vector7 = normalized;
						if (vector6 != Vector3.zero)
						{
							vector7 = (vector6 - vector5).normalized;
						}
						RaycastHit raycastHit;
						if (UnityEngine.Physics.SphereCast(vector4, 3f, vector7, out raycastHit, minDistanceFromShore, 1218511105))
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						flag = true;
						list[j] = vector5;
					}
				}
			}
			num9++;
		}
		if (flag)
		{
			Debug.LogWarning("Failed to generate ocean patrol path");
			return null;
		}
		List<int> list2 = new List<int>();
		LineUtility.Simplify(list, 5f, list2);
		List<Vector3> list3 = list;
		list = new List<Vector3>();
		foreach (int num14 in list2)
		{
			list.Add(list3[num14]);
		}
		Debug.Log("Generated ocean patrol path with node count: " + list.Count);
		return list;
	}

	// Token: 0x04001DCF RID: 7631
	public float engineThrust = 10f;

	// Token: 0x04001DD0 RID: 7632
	public float steeringScale = 0.1f;

	// Token: 0x04001DD1 RID: 7633
	public Transform thrustPoint;

	// Token: 0x04001DD2 RID: 7634
	public Transform centerOfMass;

	// Token: 0x04001DD3 RID: 7635
	public Buoyancy buoyancy;

	// Token: 0x04001DD4 RID: 7636
	[Header("Correction Forces")]
	public Transform[] planeFitPoints;

	// Token: 0x04001DD5 RID: 7637
	public Vector3 inAirPID;

	// Token: 0x04001DD6 RID: 7638
	public float inAirDesiredPitch = -15f;

	// Token: 0x04001DD7 RID: 7639
	public Vector3 wavePID;

	// Token: 0x04001DD8 RID: 7640
	public MinMax correctionRange;

	// Token: 0x04001DD9 RID: 7641
	public float correctionSpringForce;

	// Token: 0x04001DDA RID: 7642
	public float correctionSpringDamping;

	// Token: 0x04001DDB RID: 7643
	private Vector3[] worldAnchors;

	// Token: 0x04001DDC RID: 7644
	private PidQuaternionController pidController;

	// Token: 0x04001DDD RID: 7645
	[ServerVar]
	public static bool generate_paths = true;

	// Token: 0x04001DDE RID: 7646
	[NonSerialized]
	public float gasPedal;

	// Token: 0x04001DDF RID: 7647
	[NonSerialized]
	public float steering;
}
