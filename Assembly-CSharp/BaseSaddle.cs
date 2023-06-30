using System;

// Token: 0x02000470 RID: 1136
public class BaseSaddle : BaseMountable
{
	// Token: 0x06002597 RID: 9623 RVA: 0x000ED2F6 File Offset: 0x000EB4F6
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (player != this._mounted)
		{
			return;
		}
		if (this.animal)
		{
			this.animal.RiderInput(inputState, player);
		}
	}

	// Token: 0x06002598 RID: 9624 RVA: 0x000ED321 File Offset: 0x000EB521
	public void SetAnimal(BaseRidableAnimal newAnimal)
	{
		this.animal = newAnimal;
	}

	// Token: 0x04001DB7 RID: 7607
	public BaseRidableAnimal animal;
}
