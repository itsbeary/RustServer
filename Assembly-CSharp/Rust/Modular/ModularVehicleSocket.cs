using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B38 RID: 2872
	[Serializable]
	public class ModularVehicleSocket
	{
		// Token: 0x17000651 RID: 1617
		// (get) Token: 0x06004596 RID: 17814 RVA: 0x0019668C File Offset: 0x0019488C
		public Vector3 WorldPosition
		{
			get
			{
				return this.socketTransform.position;
			}
		}

		// Token: 0x17000652 RID: 1618
		// (get) Token: 0x06004597 RID: 17815 RVA: 0x00196699 File Offset: 0x00194899
		public Quaternion WorldRotation
		{
			get
			{
				return this.socketTransform.rotation;
			}
		}

		// Token: 0x17000653 RID: 1619
		// (get) Token: 0x06004598 RID: 17816 RVA: 0x001966A6 File Offset: 0x001948A6
		public ModularVehicleSocket.SocketWheelType WheelType
		{
			get
			{
				return this.wheelType;
			}
		}

		// Token: 0x17000654 RID: 1620
		// (get) Token: 0x06004599 RID: 17817 RVA: 0x001966AE File Offset: 0x001948AE
		public ModularVehicleSocket.SocketLocationType LocationType
		{
			get
			{
				return this.locationType;
			}
		}

		// Token: 0x0600459A RID: 17818 RVA: 0x001966B8 File Offset: 0x001948B8
		public bool ShouldBeActive(ConditionalSocketSettings modelSettings)
		{
			bool flag = true;
			if (modelSettings.restrictOnLocation)
			{
				ConditionalSocketSettings.LocationCondition locationRestriction = modelSettings.locationRestriction;
				switch (this.LocationType)
				{
				case ModularVehicleSocket.SocketLocationType.Middle:
					flag = locationRestriction == ConditionalSocketSettings.LocationCondition.Middle || locationRestriction == ConditionalSocketSettings.LocationCondition.NotFront || locationRestriction == ConditionalSocketSettings.LocationCondition.NotBack;
					break;
				case ModularVehicleSocket.SocketLocationType.Front:
					flag = locationRestriction == ConditionalSocketSettings.LocationCondition.Front || locationRestriction == ConditionalSocketSettings.LocationCondition.NotBack || locationRestriction == ConditionalSocketSettings.LocationCondition.NotMiddle;
					break;
				case ModularVehicleSocket.SocketLocationType.Back:
					flag = locationRestriction == ConditionalSocketSettings.LocationCondition.Back || locationRestriction == ConditionalSocketSettings.LocationCondition.NotFront || locationRestriction == ConditionalSocketSettings.LocationCondition.NotMiddle;
					break;
				}
			}
			if (flag && modelSettings.restrictOnWheel)
			{
				flag = this.WheelType == modelSettings.wheelRestriction;
			}
			return flag;
		}

		// Token: 0x04003E8F RID: 16015
		[SerializeField]
		private Transform socketTransform;

		// Token: 0x04003E90 RID: 16016
		[SerializeField]
		private ModularVehicleSocket.SocketWheelType wheelType;

		// Token: 0x04003E91 RID: 16017
		[SerializeField]
		private ModularVehicleSocket.SocketLocationType locationType;

		// Token: 0x02000FA9 RID: 4009
		public enum SocketWheelType
		{
			// Token: 0x040050F8 RID: 20728
			NoWheel,
			// Token: 0x040050F9 RID: 20729
			ForwardWheel,
			// Token: 0x040050FA RID: 20730
			BackWheel
		}

		// Token: 0x02000FAA RID: 4010
		public enum SocketLocationType
		{
			// Token: 0x040050FC RID: 20732
			Middle,
			// Token: 0x040050FD RID: 20733
			Front,
			// Token: 0x040050FE RID: 20734
			Back
		}
	}
}
