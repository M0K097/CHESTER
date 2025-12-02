//interface for all indicators
public interface IIndicator
{
		public string name { get; set; }
		public Dictionary<string, decimal> get_result();
		public void update(Candle c);


}

//base class for all indicators
public class Indicator : IIndicator
{
		public string name { get; set; }
		public Dictionary<string, decimal> get_result()
		{

				return new Dictionary<string, decimal>();
		}

		public void update(Candle c)
		{
				Console.WriteLine($"update method of {name} is empty");
		}

		public Indicator(string name)
		{
				this.name = name;
		}
}

