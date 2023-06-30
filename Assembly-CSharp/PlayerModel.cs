using System;
using UnityEngine;

// Token: 0x0200044C RID: 1100
public class PlayerModel : ListComponent<PlayerModel>
{
	// Token: 0x060024ED RID: 9453 RVA: 0x000EA26F File Offset: 0x000E846F
	private static Vector3 GetFlat(Vector3 dir)
	{
		dir.y = 0f;
		return dir.normalized;
	}

	// Token: 0x060024EE RID: 9454 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void RebuildAll()
	{
	}

	// Token: 0x1700030C RID: 780
	// (get) Token: 0x060024EF RID: 9455 RVA: 0x000EA284 File Offset: 0x000E8484
	// (set) Token: 0x060024F0 RID: 9456 RVA: 0x000EA28C File Offset: 0x000E848C
	public ulong overrideSkinSeed { get; private set; }

	// Token: 0x1700030D RID: 781
	// (get) Token: 0x060024F1 RID: 9457 RVA: 0x000EA295 File Offset: 0x000E8495
	public bool IsFemale
	{
		get
		{
			return this.skinType == 1;
		}
	}

	// Token: 0x1700030E RID: 782
	// (get) Token: 0x060024F2 RID: 9458 RVA: 0x000EA2A0 File Offset: 0x000E84A0
	public SkinSetCollection SkinSet
	{
		get
		{
			if (!this.IsFemale)
			{
				return this.MaleSkin;
			}
			return this.FemaleSkin;
		}
	}

	// Token: 0x1700030F RID: 783
	// (get) Token: 0x060024F3 RID: 9459 RVA: 0x000EA2B7 File Offset: 0x000E84B7
	// (set) Token: 0x060024F4 RID: 9460 RVA: 0x000EA2BF File Offset: 0x000E84BF
	public Quaternion AimAngles { get; set; }

	// Token: 0x17000310 RID: 784
	// (get) Token: 0x060024F5 RID: 9461 RVA: 0x000EA2C8 File Offset: 0x000E84C8
	// (set) Token: 0x060024F6 RID: 9462 RVA: 0x000EA2D0 File Offset: 0x000E84D0
	public Quaternion LookAngles { get; set; }

	// Token: 0x04001CDA RID: 7386
	public Transform[] Shoulders;

	// Token: 0x04001CDB RID: 7387
	public Transform[] AdditionalSpineBones;

	// Token: 0x04001CDC RID: 7388
	protected static int speed = Animator.StringToHash("speed");

	// Token: 0x04001CDD RID: 7389
	protected static int acceleration = Animator.StringToHash("acceleration");

	// Token: 0x04001CDE RID: 7390
	protected static int rotationYaw = Animator.StringToHash("rotationYaw");

	// Token: 0x04001CDF RID: 7391
	protected static int forward = Animator.StringToHash("forward");

	// Token: 0x04001CE0 RID: 7392
	protected static int right = Animator.StringToHash("right");

	// Token: 0x04001CE1 RID: 7393
	protected static int up = Animator.StringToHash("up");

	// Token: 0x04001CE2 RID: 7394
	protected static int ducked = Animator.StringToHash("ducked");

	// Token: 0x04001CE3 RID: 7395
	protected static int grounded = Animator.StringToHash("grounded");

	// Token: 0x04001CE4 RID: 7396
	protected static int crawling = Animator.StringToHash("crawling");

	// Token: 0x04001CE5 RID: 7397
	protected static int waterlevel = Animator.StringToHash("waterlevel");

	// Token: 0x04001CE6 RID: 7398
	protected static int attack = Animator.StringToHash("attack");

	// Token: 0x04001CE7 RID: 7399
	protected static int attack_alt = Animator.StringToHash("attack_alt");

	// Token: 0x04001CE8 RID: 7400
	protected static int deploy = Animator.StringToHash("deploy");

	// Token: 0x04001CE9 RID: 7401
	protected static int turnOn = Animator.StringToHash("turnOn");

	// Token: 0x04001CEA RID: 7402
	protected static int turnOff = Animator.StringToHash("turnOff");

	// Token: 0x04001CEB RID: 7403
	protected static int reload = Animator.StringToHash("reload");

	// Token: 0x04001CEC RID: 7404
	protected static int throwWeapon = Animator.StringToHash("throw");

	// Token: 0x04001CED RID: 7405
	protected static int holster = Animator.StringToHash("holster");

	// Token: 0x04001CEE RID: 7406
	protected static int aiming = Animator.StringToHash("aiming");

	// Token: 0x04001CEF RID: 7407
	protected static int onLadder = Animator.StringToHash("onLadder");

	// Token: 0x04001CF0 RID: 7408
	protected static int posing = Animator.StringToHash("posing");

	// Token: 0x04001CF1 RID: 7409
	protected static int poseType = Animator.StringToHash("poseType");

	// Token: 0x04001CF2 RID: 7410
	protected static int relaxGunPose = Animator.StringToHash("relaxGunPose");

	// Token: 0x04001CF3 RID: 7411
	protected static int vehicle_aim_yaw = Animator.StringToHash("vehicleAimYaw");

	// Token: 0x04001CF4 RID: 7412
	protected static int vehicle_aim_speed = Animator.StringToHash("vehicleAimYawSpeed");

	// Token: 0x04001CF5 RID: 7413
	protected static int onPhone = Animator.StringToHash("onPhone");

	// Token: 0x04001CF6 RID: 7414
	protected static int usePoseTransition = Animator.StringToHash("usePoseTransition");

	// Token: 0x04001CF7 RID: 7415
	protected static int leftFootIK = Animator.StringToHash("leftFootIK");

	// Token: 0x04001CF8 RID: 7416
	protected static int rightFootIK = Animator.StringToHash("rightFootIK");

	// Token: 0x04001CF9 RID: 7417
	protected static int vehicleSteering = Animator.StringToHash("vehicleSteering");

	// Token: 0x04001CFA RID: 7418
	protected static int sitReaction = Animator.StringToHash("sitReaction");

	// Token: 0x04001CFB RID: 7419
	protected static int forwardReaction = Animator.StringToHash("forwardReaction");

	// Token: 0x04001CFC RID: 7420
	protected static int rightReaction = Animator.StringToHash("rightReaction");

	// Token: 0x04001CFD RID: 7421
	protected static int ladderType = Animator.StringToHash("ladderType");

	// Token: 0x04001CFE RID: 7422
	public BoxCollider collision;

	// Token: 0x04001CFF RID: 7423
	public GameObject censorshipCube;

	// Token: 0x04001D00 RID: 7424
	public GameObject censorshipCubeBreasts;

	// Token: 0x04001D01 RID: 7425
	public GameObject jawBone;

	// Token: 0x04001D02 RID: 7426
	public GameObject neckBone;

	// Token: 0x04001D03 RID: 7427
	public GameObject headBone;

	// Token: 0x04001D04 RID: 7428
	public EyeController eyeController;

	// Token: 0x04001D05 RID: 7429
	public EyeBlink blinkController;

	// Token: 0x04001D06 RID: 7430
	public Transform[] SpineBones;

	// Token: 0x04001D07 RID: 7431
	public Transform leftFootBone;

	// Token: 0x04001D08 RID: 7432
	public Transform rightFootBone;

	// Token: 0x04001D09 RID: 7433
	public Transform leftHandPropBone;

	// Token: 0x04001D0A RID: 7434
	public Transform rightHandPropBone;

	// Token: 0x04001D0B RID: 7435
	public Vector3 rightHandTarget;

	// Token: 0x04001D0C RID: 7436
	[Header("IK")]
	public Vector3 leftHandTargetPosition;

	// Token: 0x04001D0D RID: 7437
	public Quaternion leftHandTargetRotation;

	// Token: 0x04001D0E RID: 7438
	public Vector3 rightHandTargetPosition;

	// Token: 0x04001D0F RID: 7439
	public Quaternion rightHandTargetRotation;

	// Token: 0x04001D10 RID: 7440
	public float steeringTargetDegrees;

	// Token: 0x04001D11 RID: 7441
	public Vector3 rightFootTargetPosition;

	// Token: 0x04001D12 RID: 7442
	public Quaternion rightFootTargetRotation;

	// Token: 0x04001D13 RID: 7443
	public Vector3 leftFootTargetPosition;

	// Token: 0x04001D14 RID: 7444
	public Quaternion leftFootTargetRotation;

	// Token: 0x04001D15 RID: 7445
	public RuntimeAnimatorController CinematicAnimationController;

	// Token: 0x04001D16 RID: 7446
	public Avatar DefaultAvatar;

	// Token: 0x04001D17 RID: 7447
	public Avatar CinematicAvatar;

	// Token: 0x04001D18 RID: 7448
	public RuntimeAnimatorController DefaultHoldType;

	// Token: 0x04001D19 RID: 7449
	public RuntimeAnimatorController SleepGesture;

	// Token: 0x04001D1A RID: 7450
	public RuntimeAnimatorController CrawlToIncapacitatedGesture;

	// Token: 0x04001D1B RID: 7451
	public RuntimeAnimatorController StandToIncapacitatedGesture;

	// Token: 0x04001D1C RID: 7452
	[NonSerialized]
	public RuntimeAnimatorController CurrentGesture;

	// Token: 0x04001D1D RID: 7453
	[Header("Skin")]
	public SkinSetCollection MaleSkin;

	// Token: 0x04001D1E RID: 7454
	public SkinSetCollection FemaleSkin;

	// Token: 0x04001D1F RID: 7455
	public SubsurfaceProfile subsurfaceProfile;

	// Token: 0x04001D20 RID: 7456
	[Header("Parameters")]
	[Range(0f, 1f)]
	public float voiceVolume;

	// Token: 0x04001D21 RID: 7457
	[Range(0f, 1f)]
	public float skinColor = 1f;

	// Token: 0x04001D22 RID: 7458
	[Range(0f, 1f)]
	public float skinNumber = 1f;

	// Token: 0x04001D23 RID: 7459
	[Range(0f, 1f)]
	public float meshNumber;

	// Token: 0x04001D24 RID: 7460
	[Range(0f, 1f)]
	public float hairNumber;

	// Token: 0x04001D25 RID: 7461
	[Range(0f, 1f)]
	public int skinType;

	// Token: 0x04001D26 RID: 7462
	public MovementSounds movementSounds;

	// Token: 0x04001D27 RID: 7463
	public bool showSash;

	// Token: 0x04001D28 RID: 7464
	public int tempPoseType;

	// Token: 0x04001D29 RID: 7465
	public uint underwearSkin;

	// Token: 0x02000D01 RID: 3329
	public enum MountPoses
	{
		// Token: 0x0400464E RID: 17998
		Chair,
		// Token: 0x0400464F RID: 17999
		Driving,
		// Token: 0x04004650 RID: 18000
		Horseback,
		// Token: 0x04004651 RID: 18001
		HeliUnarmed,
		// Token: 0x04004652 RID: 18002
		HeliArmed,
		// Token: 0x04004653 RID: 18003
		HandMotorBoat,
		// Token: 0x04004654 RID: 18004
		MotorBoatPassenger,
		// Token: 0x04004655 RID: 18005
		SitGeneric,
		// Token: 0x04004656 RID: 18006
		SitRaft,
		// Token: 0x04004657 RID: 18007
		StandDrive,
		// Token: 0x04004658 RID: 18008
		SitShootingGeneric,
		// Token: 0x04004659 RID: 18009
		SitMinicopter_Pilot,
		// Token: 0x0400465A RID: 18010
		SitMinicopter_Passenger,
		// Token: 0x0400465B RID: 18011
		ArcadeLeft,
		// Token: 0x0400465C RID: 18012
		ArcadeRight,
		// Token: 0x0400465D RID: 18013
		SitSummer_Ring,
		// Token: 0x0400465E RID: 18014
		SitSummer_BoogieBoard,
		// Token: 0x0400465F RID: 18015
		SitCarPassenger,
		// Token: 0x04004660 RID: 18016
		SitSummer_Chair,
		// Token: 0x04004661 RID: 18017
		SitRaft_NoPaddle,
		// Token: 0x04004662 RID: 18018
		Sit_SecretLab,
		// Token: 0x04004663 RID: 18019
		Sit_Workcart,
		// Token: 0x04004664 RID: 18020
		Sit_Cardgame,
		// Token: 0x04004665 RID: 18021
		Sit_Crane,
		// Token: 0x04004666 RID: 18022
		Sit_Snowmobile_Shooting,
		// Token: 0x04004667 RID: 18023
		Sit_RetroSnowmobile_Shooting,
		// Token: 0x04004668 RID: 18024
		Driving_Snowmobile,
		// Token: 0x04004669 RID: 18025
		ZiplineHold,
		// Token: 0x0400466A RID: 18026
		Sit_Locomotive,
		// Token: 0x0400466B RID: 18027
		Sit_Throne,
		// Token: 0x0400466C RID: 18028
		Standing = 128
	}
}
