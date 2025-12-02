//creates and manages all instances
using System.Collections.Concurrent;
using static Logger;
static class Instance_Manager
{
		static public Dictionary<string, ConcurrentDictionary<CancellationTokenSource, Task>> instances
		= new Dictionary<string, ConcurrentDictionary<CancellationTokenSource, Task>>();

		static public void start_instance()
		{

				Console.WriteLine("enter name for new instance...");
				string name = "none";
				string? input = Console.ReadLine();
				if (input != null)
				{
						name = input;
				}
				if (instances.ContainsKey(name))
				{
						Console.WriteLine("name already exists");
				}
				else
				{
						CancellationTokenSource source = new CancellationTokenSource();
						ConcurrentDictionary<CancellationTokenSource, Task> instance = new ConcurrentDictionary<CancellationTokenSource, Task>();
						Chester chester = create_Chester();
						instance.TryAdd(source, Task.Run(() => worker(source.Token, name, chester)));
						instances.TryAdd(name, instance);
				}
		}



		static Chester create_Chester()
		{
				List<string> options = new List<string> { "PAPERTRADER", "BROKER" };
				string choosen_option = CLU_UI.run(options, "--->>OPTIONS<<---");
				string default_symbol = "BTCUSD";
				int default_candlesize = 60;
				Console.WriteLine("Enter SYMBOLPAR: e.g.'BTCUSDT'");
				string? symbol = Console.ReadLine();
				if (symbol != null)
				{
						default_symbol = symbol;
				}
				Console.WriteLine("Enter CandleSize:");
				int candlesize = Convert.ToInt32(Console.ReadLine());
				if (candlesize > 0)
				{
						default_candlesize = candlesize;
				}

				IStrategy strategy = Strategy_Manager.select_strategy();
				Chester Chester = new Chester(choosen_option, default_symbol, strategy, default_candlesize);
				return Chester;

		}

		static async Task worker(CancellationToken token, string name, Chester instance)
		{
				await instance.start();
				while (!token.IsCancellationRequested)
				{
						log($"{name} is running", "INSTANCES");
						if (!Monitor.all_callbacks.ContainsKey(name))
						{
								Monitor.all_callbacks.TryAdd
								(name, new Status(name, DateTime.Now, instance));
								log($"Added new MONITOR for {name}", "MONITOR");

						}
						else
						{
								Monitor.all_callbacks[name].last_update = DateTime.Now;
								Monitor.all_callbacks[name].WORKING_CHESTER = instance;
								log($"{name} got status update", "MONITOR");
						}

						await Task.Delay(10000);
				}

				log($"Task[{name}] got cancelled, closing web_socket and awaiting end of Task...", "MONITOR");
				instance.client.stop();
		}

		static public async Task stop_instance()
		{
				List<string> names = new List<string>();
				foreach (KeyValuePair<string, ConcurrentDictionary<CancellationTokenSource, Task>> i in instances)
				{
						names.Add(i.Key);
				}
				try
				{
						if (names.Count() < 1)
						{
								throw new Exception("no running instances");
						}
						string selected = CLU_UI.run(names, "SELECT TO STOP");

						if (selected != "return")
						{
								ConcurrentDictionary<CancellationTokenSource, Task> selected_instance = instances[selected];
								instances.Remove(selected);
								foreach (KeyValuePair<CancellationTokenSource, Task> i in selected_instance)
								{
										CancellationTokenSource source = i.Key;
										Task task = i.Value;

										source.Cancel();
										source.Dispose();
										await task;
										task.Dispose();

										log($"Task was successfully disposed","INSTANCES");
										break;
								}
						}
				}
				catch (Exception)
				{
						log("no instances running");
				}
		}
}

