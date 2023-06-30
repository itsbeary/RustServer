using System;

// Token: 0x020004B3 RID: 1203
public interface ITrainCollidable
{
	// Token: 0x06002779 RID: 10105
	bool CustomCollision(TrainCar train, TriggerTrainCollisions trainTrigger);

	// Token: 0x0600277A RID: 10106
	bool EqualNetID(BaseNetworkable other);
}
