using static Logger;
//based on the famous turtle traders strategie
class NinjaTurtle : Strategy
{
		//gloabl variables
		decimal stop_loss;
		decimal price;
		decimal at;
		//trade
		bool buy = false;
		bool sell = false;
		bool holding = false;
		//safed values
		decimal last_dcu;
		decimal last_dcl;
		decimal safed_rsi;
		//strategy state
		bool setup;
		bool pullback;
		bool trending;
		int dc_dynamic;
		int stop_loss_atr;


		public override void init()
		{
				var parameters = core_configurations.settings;
				var ema_length = Convert.ToInt32(parameters["NinjaTurtle:ema_length"]);
				//add indicators
				indicators.Add(new ATR(Convert.ToInt32(parameters["NinjaTurtle:atr_length"]), "at"));
				indicators.Add(new donchain_channel(Convert.ToInt32(parameters["NinjaTurtle:dc_length"]), "dc1"));
				indicators.Add(new RSI("rsi1", Convert.ToInt32(parameters["NinjaTurtle:rsi_length"])));
				indicators.Add(new exponential_moving_average(ema_length, "ema1"));
				dc_dynamic = Convert.ToInt32(parameters["NinjaTurtle:dynamic_dc_multi"]);
				stop_loss_atr = Convert.ToInt32(parameters["NinjaTurtle:stop_loss_atr_multiplier"]);

				warmup = ema_length;
		}

		public override void update(Candle c)
		{
				//get indicators
				at = get_result_of("at")["result"];
				decimal dcu = get_result_of("dc1")["upper band"];
				decimal dcl = get_result_of("dc1")["lower band"];
				decimal rsi = get_result_of("rsi1")["index"];
				decimal ema = get_result_of("ema1")["Median"];

				//update to new price
				price = c.close;

				//check for uptrend?
				if (price > ema)
				{
						trending = true;

				}
				else
				{
						trending = false;
				}


				//breakout?
				if (trending && !setup && c.close > last_dcu)
				{
						log("breakout", "NINJATURTLE");
						//do we have momentum?	
						if (rsi > 60)
						{
								log($"safed rsi valie:{rsi}", "NINJATURTLE");
								safed_rsi = rsi;
								setup = true;

						}
				}

				//buy after a higher low or exit setup
				if (setup)
				{
						if (rsi < safed_rsi && !pullback)
						{
								log("momentum is pulling back", "NINJATURTLE");
								pullback = true;
						}
						if (pullback && rsi < 50)
						{
								log($"rsi{rsi} fell below 50 = canceling setup", "NINJATURTLE");
								setup = false;
								pullback = false;

						}
						if (pullback && rsi > safed_rsi)
						{
								buy = true;
								setup = false;
								pullback = false;
						}


				}
				if (holding && c.close < last_dcl - dc_dynamic * at)
				{
						log("trend end", "NINJATURTLE");
						sell = true;

				}
				if (price < stop_loss)
				{
						log("stop loss is hit!", "NINJATURTLE");
						sell = true;
				}


				last_dcl = dcl;
				last_dcu = dcu;

		}

		public override int check()
		{
				if (buy && !holding)
				{
						buy = false;
						holding = true;
						stop_loss = price - (stop_loss_atr * at);
						log($"stop loss = {stop_loss}", "NINJATURTLE");
						return 99;
				}
				if (sell && holding)
				{
						sell = false;
						holding = false;
						stop_loss = -1000;
						return -99;
				}


				return 0;
		}
}
