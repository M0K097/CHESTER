
public class Trade
{
		public DateTime enter_time { get; set; }
		public DateTime exit_time { get; set; }
		public decimal enter_price { get; set; }
		public decimal exit_price { get; set; }
		public decimal enter_capital { get; set; }
		public decimal exit_capital { get; set; }
		public decimal trades_return { get; set; }
		public int index { get; set; }
		public TimeSpan trade_timespan { get; set; }

		public void set_entry(DateTime enter_time, decimal enter_price, decimal enter_capital)
		{
				this.enter_time = enter_time;
				this.enter_price = enter_price;
				this.enter_capital = enter_capital;
		}

		public void set_exit(DateTime exit_time, decimal exit_price, decimal exit_capital)
		{
				this.exit_time = exit_time;
				this.exit_price = exit_price;
				this.exit_capital = exit_capital;
		}
		public void calculate_return()
		{
				trades_return = ((exit_capital / enter_capital) * 100) - 100;
				trade_timespan = (exit_time - enter_time);
		}

		public void show_trade()
		{
				Console.WriteLine($"{"Trade Nr:",20}{"",-20}{index,0}\n" +
						$"{"PROFIT:",20}{"",-20}{$"{Math.Round(trades_return, 2)}%",0}\n" +
						$"{"TIME_ENTRY:",20}{"",-20}{enter_time,0}\n" +
						$"{"TIME_EXIT:",20}{"",-20}{exit_time,0}\n" +
						$"{"PRICE:",20}{"",-20}{enter_price,0}\n" +
						$"{"PRICE_EXIT",20}{"",-20}{exit_price,0}\n" +
						$"{"CAPITAL_ENTER:",20}{"",-20}{enter_capital,0}\n" +
						$"{"CAPITAL_EXIT:",20}{"",-20}{exit_capital,0}\n" +
						$"{"TIMESPAN:",20}{"",-20}{trade_timespan,0}\n");
		}
		public Trade(int index)
		{
				this.index = index;
		}

}


