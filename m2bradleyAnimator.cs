using System;
using UnityEngine;

// Token: 0x0200041C RID: 1052
public class m2bradleyAnimator : MonoBehaviour
{
	// Token: 0x060023A1 RID: 9121 RVA: 0x000E3344 File Offset: 0x000E1544
	private void Start()
	{
		this.mainRigidbody = base.GetComponent<Rigidbody>();
		for (int i = 0; i < this.ShocksBones.Length; i++)
		{
			this.vecShocksOffsetPosition[i] = this.ShocksBones[i].localPosition;
		}
	}

	// Token: 0x060023A2 RID: 9122 RVA: 0x000E3389 File Offset: 0x000E1589
	private void Update()
	{
		this.TrackTurret();
		this.TrackSpotLight();
		this.TrackSideGuns();
		this.AnimateWheelsTreads();
		this.AdjustShocksHeight();
		this.m2Animator.SetBool("rocketpods", this.rocketsOpen);
	}

	// Token: 0x060023A3 RID: 9123 RVA: 0x000E33C0 File Offset: 0x000E15C0
	private void AnimateWheelsTreads()
	{
		float num = 0f;
		if (this.mainRigidbody != null)
		{
			num = Vector3.Dot(this.mainRigidbody.velocity, base.transform.forward);
		}
		float num2 = Time.time * -1f * num * this.treadConstant % 1f;
		this.treadLeftMaterial.SetTextureOffset("_MainTex", new Vector2(num2, 0f));
		this.treadLeftMaterial.SetTextureOffset("_BumpMap", new Vector2(num2, 0f));
		this.treadLeftMaterial.SetTextureOffset("_SpecGlossMap", new Vector2(num2, 0f));
		this.treadRightMaterial.SetTextureOffset("_MainTex", new Vector2(num2, 0f));
		this.treadRightMaterial.SetTextureOffset("_BumpMap", new Vector2(num2, 0f));
		this.treadRightMaterial.SetTextureOffset("_SpecGlossMap", new Vector2(num2, 0f));
		if (num >= 0f)
		{
			this.wheelAngle = (this.wheelAngle + Time.deltaTime * num * this.wheelSpinConstant) % 360f;
		}
		else
		{
			this.wheelAngle += Time.deltaTime * num * this.wheelSpinConstant;
			if (this.wheelAngle <= 0f)
			{
				this.wheelAngle = 360f;
			}
		}
		this.m2Animator.SetFloat("wheel_spin", this.wheelAngle);
		this.m2Animator.SetFloat("speed", num);
	}

	// Token: 0x060023A4 RID: 9124 RVA: 0x000E3540 File Offset: 0x000E1740
	private void AdjustShocksHeight()
	{
		Ray ray = default(Ray);
		int mask = LayerMask.GetMask(new string[] { "Terrain", "World", "Construction" });
		int num = this.ShocksBones.Length;
		float num2 = 0.55f;
		float num3 = 0.79f;
		for (int i = 0; i < num; i++)
		{
			ray.origin = this.ShockTraceLineBegin[i].position;
			ray.direction = base.transform.up * -1f;
			RaycastHit raycastHit;
			float num4;
			if (Physics.SphereCast(ray, 0.15f, out raycastHit, num3, mask))
			{
				num4 = raycastHit.distance - num2;
			}
			else
			{
				num4 = 0.26f;
			}
			this.vecShocksOffsetPosition[i].y = Mathf.Lerp(this.vecShocksOffsetPosition[i].y, Mathf.Clamp(num4 * -1f, -0.26f, 0f), Time.deltaTime * 5f);
			this.ShocksBones[i].localPosition = this.vecShocksOffsetPosition[i];
		}
	}

	// Token: 0x060023A5 RID: 9125 RVA: 0x000E366C File Offset: 0x000E186C
	private void TrackTurret()
	{
		if (this.targetTurret != null)
		{
			Vector3 normalized = (this.targetTurret.position - this.turret.position).normalized;
			float num;
			float num2;
			this.CalculateYawPitchOffset(this.turret, this.turret.position, this.targetTurret.position, out num, out num2);
			num = this.NormalizeYaw(num);
			float num3 = Time.deltaTime * this.turretTurnSpeed;
			if (num < -0.5f)
			{
				this.vecTurret.y = (this.vecTurret.y - num3) % 360f;
			}
			else if (num > 0.5f)
			{
				this.vecTurret.y = (this.vecTurret.y + num3) % 360f;
			}
			this.turret.localEulerAngles = this.vecTurret;
			float num4 = Time.deltaTime * this.cannonPitchSpeed;
			this.CalculateYawPitchOffset(this.mainCannon, this.mainCannon.position, this.targetTurret.position, out num, out num2);
			if (num2 < -0.5f)
			{
				this.vecMainCannon.x = this.vecMainCannon.x - num4;
			}
			else if (num2 > 0.5f)
			{
				this.vecMainCannon.x = this.vecMainCannon.x + num4;
			}
			this.vecMainCannon.x = Mathf.Clamp(this.vecMainCannon.x, -55f, 5f);
			this.mainCannon.localEulerAngles = this.vecMainCannon;
			if (num2 < -0.5f)
			{
				this.vecCoaxGun.x = this.vecCoaxGun.x - num4;
			}
			else if (num2 > 0.5f)
			{
				this.vecCoaxGun.x = this.vecCoaxGun.x + num4;
			}
			this.vecCoaxGun.x = Mathf.Clamp(this.vecCoaxGun.x, -65f, 15f);
			this.coaxGun.localEulerAngles = this.vecCoaxGun;
			if (this.rocketsOpen)
			{
				num4 = Time.deltaTime * this.rocketPitchSpeed;
				this.CalculateYawPitchOffset(this.rocketsPitch, this.rocketsPitch.position, this.targetTurret.position, out num, out num2);
				if (num2 < -0.5f)
				{
					this.vecRocketsPitch.x = this.vecRocketsPitch.x - num4;
				}
				else if (num2 > 0.5f)
				{
					this.vecRocketsPitch.x = this.vecRocketsPitch.x + num4;
				}
				this.vecRocketsPitch.x = Mathf.Clamp(this.vecRocketsPitch.x, -45f, 45f);
			}
			else
			{
				this.vecRocketsPitch.x = Mathf.Lerp(this.vecRocketsPitch.x, 0f, Time.deltaTime * 1.7f);
			}
			this.rocketsPitch.localEulerAngles = this.vecRocketsPitch;
		}
	}

	// Token: 0x060023A6 RID: 9126 RVA: 0x000E3940 File Offset: 0x000E1B40
	private void TrackSpotLight()
	{
		if (this.targetSpotLight != null)
		{
			Vector3 normalized = (this.targetSpotLight.position - this.spotLightYaw.position).normalized;
			float num;
			float num2;
			this.CalculateYawPitchOffset(this.spotLightYaw, this.spotLightYaw.position, this.targetSpotLight.position, out num, out num2);
			num = this.NormalizeYaw(num);
			float num3 = Time.deltaTime * this.spotLightTurnSpeed;
			if (num < -0.5f)
			{
				this.vecSpotLightBase.y = (this.vecSpotLightBase.y - num3) % 360f;
			}
			else if (num > 0.5f)
			{
				this.vecSpotLightBase.y = (this.vecSpotLightBase.y + num3) % 360f;
			}
			this.spotLightYaw.localEulerAngles = this.vecSpotLightBase;
			this.CalculateYawPitchOffset(this.spotLightPitch, this.spotLightPitch.position, this.targetSpotLight.position, out num, out num2);
			if (num2 < -0.5f)
			{
				this.vecSpotLight.x = this.vecSpotLight.x - num3;
			}
			else if (num2 > 0.5f)
			{
				this.vecSpotLight.x = this.vecSpotLight.x + num3;
			}
			this.vecSpotLight.x = Mathf.Clamp(this.vecSpotLight.x, -50f, 50f);
			this.spotLightPitch.localEulerAngles = this.vecSpotLight;
			this.m2Animator.SetFloat("sideMG_pitch", this.vecSpotLight.x, 0.5f, Time.deltaTime);
		}
	}

	// Token: 0x060023A7 RID: 9127 RVA: 0x000E3AD0 File Offset: 0x000E1CD0
	private void TrackSideGuns()
	{
		for (int i = 0; i < this.sideguns.Length; i++)
		{
			if (!(this.targetSideguns[i] == null))
			{
				Vector3 normalized = (this.targetSideguns[i].position - this.sideguns[i].position).normalized;
				float num;
				float num2;
				this.CalculateYawPitchOffset(this.sideguns[i], this.sideguns[i].position, this.targetSideguns[i].position, out num, out num2);
				num = this.NormalizeYaw(num);
				float num3 = Time.deltaTime * this.sidegunsTurnSpeed;
				if (num < -0.5f)
				{
					Vector3[] array = this.vecSideGunRotation;
					int num4 = i;
					array[num4].y = array[num4].y - num3;
				}
				else if (num > 0.5f)
				{
					Vector3[] array2 = this.vecSideGunRotation;
					int num5 = i;
					array2[num5].y = array2[num5].y + num3;
				}
				if (num2 < -0.5f)
				{
					Vector3[] array3 = this.vecSideGunRotation;
					int num6 = i;
					array3[num6].x = array3[num6].x - num3;
				}
				else if (num2 > 0.5f)
				{
					Vector3[] array4 = this.vecSideGunRotation;
					int num7 = i;
					array4[num7].x = array4[num7].x + num3;
				}
				this.vecSideGunRotation[i].x = Mathf.Clamp(this.vecSideGunRotation[i].x, -45f, 45f);
				this.vecSideGunRotation[i].y = Mathf.Clamp(this.vecSideGunRotation[i].y, -45f, 45f);
				this.sideguns[i].localEulerAngles = this.vecSideGunRotation[i];
			}
		}
	}

	// Token: 0x060023A8 RID: 9128 RVA: 0x000E3C6C File Offset: 0x000E1E6C
	public void CalculateYawPitchOffset(Transform objectTransform, Vector3 vecStart, Vector3 vecEnd, out float yaw, out float pitch)
	{
		Vector3 vector = objectTransform.InverseTransformDirection(vecEnd - vecStart);
		float num = Mathf.Sqrt(vector.x * vector.x + vector.z * vector.z);
		pitch = -Mathf.Atan2(vector.y, num) * 57.295776f;
		vector = (vecEnd - vecStart).normalized;
		Vector3 forward = objectTransform.forward;
		forward.y = 0f;
		forward.Normalize();
		float num2 = Vector3.Dot(vector, forward);
		float num3 = Vector3.Dot(vector, objectTransform.right);
		float num4 = 360f * num3;
		float num5 = 360f * -num2;
		yaw = (Mathf.Atan2(num4, num5) + 3.1415927f) * 57.295776f;
	}

	// Token: 0x060023A9 RID: 9129 RVA: 0x000E3D2C File Offset: 0x000E1F2C
	public float NormalizeYaw(float flYaw)
	{
		float num;
		if (flYaw > 180f)
		{
			num = 360f - flYaw;
		}
		else
		{
			num = flYaw * -1f;
		}
		return num;
	}

	// Token: 0x04001B8A RID: 7050
	public Animator m2Animator;

	// Token: 0x04001B8B RID: 7051
	public Material treadLeftMaterial;

	// Token: 0x04001B8C RID: 7052
	public Material treadRightMaterial;

	// Token: 0x04001B8D RID: 7053
	private Rigidbody mainRigidbody;

	// Token: 0x04001B8E RID: 7054
	[Header("GunBones")]
	public Transform turret;

	// Token: 0x04001B8F RID: 7055
	public Transform mainCannon;

	// Token: 0x04001B90 RID: 7056
	public Transform coaxGun;

	// Token: 0x04001B91 RID: 7057
	public Transform rocketsPitch;

	// Token: 0x04001B92 RID: 7058
	public Transform spotLightYaw;

	// Token: 0x04001B93 RID: 7059
	public Transform spotLightPitch;

	// Token: 0x04001B94 RID: 7060
	public Transform sideMG;

	// Token: 0x04001B95 RID: 7061
	public Transform[] sideguns;

	// Token: 0x04001B96 RID: 7062
	[Header("WheelBones")]
	public Transform[] ShocksBones;

	// Token: 0x04001B97 RID: 7063
	public Transform[] ShockTraceLineBegin;

	// Token: 0x04001B98 RID: 7064
	public Vector3[] vecShocksOffsetPosition;

	// Token: 0x04001B99 RID: 7065
	[Header("Targeting")]
	public Transform targetTurret;

	// Token: 0x04001B9A RID: 7066
	public Transform targetSpotLight;

	// Token: 0x04001B9B RID: 7067
	public Transform[] targetSideguns;

	// Token: 0x04001B9C RID: 7068
	private Vector3 vecTurret = new Vector3(0f, 0f, 0f);

	// Token: 0x04001B9D RID: 7069
	private Vector3 vecMainCannon = new Vector3(0f, 0f, 0f);

	// Token: 0x04001B9E RID: 7070
	private Vector3 vecCoaxGun = new Vector3(0f, 0f, 0f);

	// Token: 0x04001B9F RID: 7071
	private Vector3 vecRocketsPitch = new Vector3(0f, 0f, 0f);

	// Token: 0x04001BA0 RID: 7072
	private Vector3 vecSpotLightBase = new Vector3(0f, 0f, 0f);

	// Token: 0x04001BA1 RID: 7073
	private Vector3 vecSpotLight = new Vector3(0f, 0f, 0f);

	// Token: 0x04001BA2 RID: 7074
	private float sideMGPitchValue;

	// Token: 0x04001BA3 RID: 7075
	[Header("MuzzleFlash locations")]
	public GameObject muzzleflashCannon;

	// Token: 0x04001BA4 RID: 7076
	public GameObject muzzleflashCoaxGun;

	// Token: 0x04001BA5 RID: 7077
	public GameObject muzzleflashSideMG;

	// Token: 0x04001BA6 RID: 7078
	public GameObject[] muzzleflashRockets;

	// Token: 0x04001BA7 RID: 7079
	public GameObject spotLightHaloSawnpoint;

	// Token: 0x04001BA8 RID: 7080
	public GameObject[] muzzleflashSideguns;

	// Token: 0x04001BA9 RID: 7081
	[Header("MuzzleFlash Particle Systems")]
	public GameObjectRef machineGunMuzzleFlashFX;

	// Token: 0x04001BAA RID: 7082
	public GameObjectRef mainCannonFireFX;

	// Token: 0x04001BAB RID: 7083
	public GameObjectRef rocketLaunchFX;

	// Token: 0x04001BAC RID: 7084
	[Header("Misc")]
	public bool rocketsOpen;

	// Token: 0x04001BAD RID: 7085
	public Vector3[] vecSideGunRotation;

	// Token: 0x04001BAE RID: 7086
	public float treadConstant = 0.14f;

	// Token: 0x04001BAF RID: 7087
	public float wheelSpinConstant = 80f;

	// Token: 0x04001BB0 RID: 7088
	[Header("Gun Movement speeds")]
	public float sidegunsTurnSpeed = 30f;

	// Token: 0x04001BB1 RID: 7089
	public float turretTurnSpeed = 6f;

	// Token: 0x04001BB2 RID: 7090
	public float cannonPitchSpeed = 10f;

	// Token: 0x04001BB3 RID: 7091
	public float rocketPitchSpeed = 20f;

	// Token: 0x04001BB4 RID: 7092
	public float spotLightTurnSpeed = 60f;

	// Token: 0x04001BB5 RID: 7093
	public float machineGunSpeed = 20f;

	// Token: 0x04001BB6 RID: 7094
	private float wheelAngle;
}
