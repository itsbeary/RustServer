using System;
using UnityEngine;

// Token: 0x020004C4 RID: 1220
public class VehicleTerrainHandler
{
	// Token: 0x17000364 RID: 868
	// (get) Token: 0x060027F2 RID: 10226 RVA: 0x000F9081 File Offset: 0x000F7281
	public bool IsOnSnowOrIce
	{
		get
		{
			return this.OnSurface == VehicleTerrainHandler.Surface.Snow || this.OnSurface == VehicleTerrainHandler.Surface.Ice;
		}
	}

	// Token: 0x060027F3 RID: 10227 RVA: 0x000F9098 File Offset: 0x000F7298
	public VehicleTerrainHandler(BaseVehicle vehicle)
	{
		this.vehicle = vehicle;
	}

	// Token: 0x060027F4 RID: 10228 RVA: 0x000F90F1 File Offset: 0x000F72F1
	public void FixedUpdate()
	{
		if (!this.vehicle.IsStationary() && this.timeSinceTerrainCheck > 0.25f)
		{
			this.DoTerrainCheck();
		}
	}

	// Token: 0x060027F5 RID: 10229 RVA: 0x000F9118 File Offset: 0x000F7318
	private void DoTerrainCheck()
	{
		this.timeSinceTerrainCheck = UnityEngine.Random.Range(-0.025f, 0.025f);
		Transform transform = this.vehicle.transform;
		RaycastHit raycastHit;
		if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.up, out raycastHit, this.RayLength, 27328513, QueryTriggerInteraction.Ignore))
		{
			this.CurGroundPhysicsMatName = raycastHit.collider.GetMaterialAt(raycastHit.point).GetNameLower();
			if (this.GetOnRoad(this.CurGroundPhysicsMatName))
			{
				this.OnSurface = VehicleTerrainHandler.Surface.Road;
			}
			else if (this.CurGroundPhysicsMatName == "snow")
			{
				if (raycastHit.collider.CompareTag("TreatSnowAsIce"))
				{
					this.OnSurface = VehicleTerrainHandler.Surface.Ice;
				}
				else
				{
					this.OnSurface = VehicleTerrainHandler.Surface.Snow;
				}
			}
			else if (this.CurGroundPhysicsMatName == "sand")
			{
				this.OnSurface = VehicleTerrainHandler.Surface.Sand;
			}
			else if (this.CurGroundPhysicsMatName.Contains("zero friction"))
			{
				this.OnSurface = VehicleTerrainHandler.Surface.Frictionless;
			}
			else
			{
				this.OnSurface = VehicleTerrainHandler.Surface.Default;
			}
			this.IsGrounded = true;
			return;
		}
		this.CurGroundPhysicsMatName = "concrete";
		this.OnSurface = VehicleTerrainHandler.Surface.Default;
		this.IsGrounded = false;
	}

	// Token: 0x060027F6 RID: 10230 RVA: 0x000F9254 File Offset: 0x000F7454
	private bool GetOnRoad(string physicMat)
	{
		for (int i = 0; i < this.TerrainRoad.Length; i++)
		{
			if (this.TerrainRoad[i] == physicMat)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04002089 RID: 8329
	public string CurGroundPhysicsMatName;

	// Token: 0x0400208A RID: 8330
	public VehicleTerrainHandler.Surface OnSurface;

	// Token: 0x0400208B RID: 8331
	public bool IsGrounded;

	// Token: 0x0400208C RID: 8332
	public float RayLength = 1.5f;

	// Token: 0x0400208D RID: 8333
	private readonly string[] TerrainRoad = new string[] { "rock", "concrete", "gravel", "metal", "path" };

	// Token: 0x0400208E RID: 8334
	private const float SECONDS_BETWEEN_TERRAIN_SAMPLE = 0.25f;

	// Token: 0x0400208F RID: 8335
	private TimeSince timeSinceTerrainCheck;

	// Token: 0x04002090 RID: 8336
	private readonly BaseVehicle vehicle;

	// Token: 0x02000D34 RID: 3380
	public enum Surface
	{
		// Token: 0x04004714 RID: 18196
		Default,
		// Token: 0x04004715 RID: 18197
		Road,
		// Token: 0x04004716 RID: 18198
		Snow,
		// Token: 0x04004717 RID: 18199
		Ice,
		// Token: 0x04004718 RID: 18200
		Sand,
		// Token: 0x04004719 RID: 18201
		Frictionless
	}
}
