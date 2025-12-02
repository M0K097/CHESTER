using ScottPlot;

//----->>visualize data with a graph
public class Plotter
{
		//make a new graph
		ScottPlot.Plot graph = new();
		string safe_path { get; set; }

		public void mark_points(Dictionary<DateTime, decimal> data, string kind_of_point)
		{

				//calculate the data-size
				int data_length = data.Count();

				//create price data points
				decimal[] price = new decimal[data_length];
				DateTime[] date = new DateTime[data_length];

				int counter = 0;

				//fill with values
				foreach (KeyValuePair<DateTime, decimal> d in data)
				{
						price[counter] = d.Value;
						date[counter] = d.Key;
						counter++;
				}

				//add to graph
				var points = graph.Add.Markers(date, price);
				if (kind_of_point == "buys")
				{
						points.Color = ScottPlot.Colors.Green;
				}
				if (kind_of_point == "sells")
				{
						points.Color = ScottPlot.Colors.Red;
				}
		}

		public void show_candles(List<Candle> candle_data)
		{
				int length = candle_data.Count();

				decimal[] price = new decimal[length];
				DateTime[] date = new DateTime[length];


				int counter = 0;
				foreach (Candle c in candle_data)
				{
						price[counter] = c.close;
						date[counter] = c.close_time;

						counter++;

				}

				//scatter data to graph
				var scatter = graph.Add.Scatter(date, price);
				scatter.Color = ScottPlot.Colors.LightBlue;
				scatter.LegendText = "PRICE";
				scatter.LineWidth = 2;
				scatter.MarkerSize = 5;
		}
		ScottPlot.Color default_color = ScottPlot.Colors.Brown;

		public void scatter_data(List<DateTime> time_points, List<decimal> data, int color_number = 0)
		{
				var YAxis2 = graph.Axes.Right;
				List<double> converted_date = new List<double>();
				foreach (DateTime t in time_points)
				{
						converted_date.Add(t.ToOADate());
				}
				var scattered = graph.Add.Scatter(converted_date, data);
				if (color_number == 0)
				{
						scattered.Color = ScottPlot.Colors.LightGreen;
				}
				else if (color_number == 1)
				{

						scattered.Color = ScottPlot.Colors.RebeccaPurple;

				}
				scattered.MarkerSize = 0;
				scattered.LineWidth = 1;
				scattered.Axes.YAxis = YAxis2;
				YAxis2.TickGenerator = new ScottPlot.TickGenerators.NumericAutomatic();

		}

		//safe to file	
		public void safe_plot()
		{

				Console.WriteLine($"saving plot to path: {safe_path}");
				graph.Axes.DateTimeTicksBottom();
				graph.SavePng(safe_path, 1200, 800);

		}
		public Plotter(string path)
		{
				this.safe_path = path;
		}


}

public static class Histrogramm_maker
{

		public static void make_sharp_distribution(List<double> simulated_sharps, double original_sharp)
		{
				ScottPlot.Plot plot = new();

				double[] converted_data = simulated_sharps.ToArray();
				plot.Add.VerticalLine(original_sharp, 2, ScottPlot.Colors.RebeccaPurple);
				var hist = ScottPlot.Statistics.Histogram.WithBinCount(20, converted_data);
				var barPlot = plot.Add.Bars(hist.Bins, hist.Counts);

				barPlot.Color = ScottPlot.Colors.LightGreen;

				int counter = 0;
				Console.WriteLine("visualizing normaldistribution of sharps...");
				foreach (var bar in barPlot.Bars)
				{
						bar.Size = hist.FirstBinSize * .8;
						counter ++;
						if(counter % 1000 == 0)
						{
							Console.WriteLine($"[{counter}/{barPlot.Bars.Count()}]");
						}
				}

				plot.Grid.IsVisible = false;
				plot.Axes.AutoScale();

				static string CustomFormatter(double position)
				{
						return position.ToString("F20");
				}

				ScottPlot.TickGenerators.NumericAutomatic myTickGenerator = new()
				{
						LabelFormatter = CustomFormatter
				};

				// Assign the custom tick generator to the bottom axis
				plot.Axes.Bottom.TickGenerator = myTickGenerator;


				plot.SavePng("visualization/sharp_distribution.png", 800, 600);

		}

}

