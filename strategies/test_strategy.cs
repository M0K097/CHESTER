using static Logger;
class test_strategy : Strategy
{
		bool holding = false;
		bool buy = false;
		bool sell = false;
		decimal last_buy = 0;

		public override void init()
		{

		}


		public override void update(Candle c)
		{

			
			
			if (!holding)
			{
				buy = true;
				last_buy = c.close;
			}
			else if (holding )
			{
				sell = true;
			}
		}

		public override int check()
		{
			if(buy)
			{
				buy = false;
				holding = true;
				return 100;
			}
			if (sell)
			{
				sell = false;
				holding = false;
				return -100;
			}

				return 0;
		}
}

