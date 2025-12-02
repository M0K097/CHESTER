using static Logger;
class MACDMOMENTUM : Strategy
{
		bool holding = false;
		bool buy = false;
		bool sell = false;
		decimal buy_price = 0;

		public override void init()
		{
			
			indicators.Add(new MACD("macd1", 12, 26, 9));
		}

		public override void update(Candle c)
		{

			var macd_hist = get_result_of("macd1")["histogram"];


				if (!holding && macd_hist > 0)
				{
					buy = true;

					log($"buy signal at time {c.close_time} price_close: {c.close} macd histogram value: {macd_hist}","test");
					buy_price = c.close;

				}
				else if (holding && macd_hist < 0)
				{
					sell = true;
				}


		}

		public override int check()
		{
				if (buy)
				{
						buy = false;
						holding = true;
						return 99;
				}

				if (sell)
				{
						sell = false;
						holding = false;
						return -99;
				}

				return 0;
		}
}

