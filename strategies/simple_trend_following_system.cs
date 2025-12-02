using static Logger;
class SimpleTrendFollowingSystem : Strategy
{
		//variables
		bool holding = false;
		bool buy = false;
		bool sell = false;
		int trend_counter = 0;
		int trend_confirm_parameter = 10;
		

		public override void init()
		{
				
				indicators.Add(new exponential_moving_average(200, "ema1"));
				warmup = 200;
		}

		public override void update(Candle c)
		{
				//-------------get indicator results
				decimal ema = get_result_of("ema1")["Median"];

				//-------------count if uptrend
				if (c.close > ema)
				{
						trend_counter++;
						log($"trend counter increased to: {trend_counter}", "stf");
				}
				else
				{
						trend_counter = 0;
						log($"trend counter reset to {trend_counter}", "stf");
				}

				//---------------->> confirm uptrend
				if (trend_counter >= trend_confirm_parameter)
				{
						log($"uptrend confirmed with {trend_counter} data points", "stf");

						//check for breakouts
						if (!holding)
						{
								buy = true;
						}
				}
				//------> or sell
				else
				{
						log($"uptrend endet", "stf");
						if(holding)
						{
							sell = true;
						}
				}


		}

		public override int check()
		{
				if (buy)
				{
						buy = false;
						holding = true;
						return 10;
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

