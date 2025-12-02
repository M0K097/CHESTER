//Copyright (c) 2025 Moritz Kolb
//Licensed for non-commercial use only. See LICENSE file for details.

using Binance.Net.Interfaces;
using static Logger;
using static Broker;
using static api_manager;

//===>>>--->>> CHESTER <<<---<<<===//OOoo>>>
//-------------------------------------->>>>
public interface IChester
{
    Task start();
}

public class Chester : IChester
{
    //properties
    public int candle_size { get; set; }
    public string option { get; set; }
    public string symbol_pair { get; set; }
    public IStrategy strategy { get; set; }
    public List<Candle> candles = new List<Candle>();
    public Simulator papertrader;

    //____INSTANCES____//
    public Analyzer live_performance_monitor;
    candle_maker maker = new candle_maker();
    candle_manager manager = new candle_manager();
    public Web_Socket client = new Web_Socket();
    public int counter = 0;
    bool initiated = false;
    bool fetch_completed = false;
    bool loadet_into_strategy = false;
    List<Candle> new_live_candles = new List<Candle>();
    Task fetch_warmup_candles;
    string path = Path.Combine("data");
    public int warmup = 0;


    //start the instance
    public async Task start()
    {
        log($"new instance of [{option}] on [{symbol_pair}] with [{strategy}] watching [{candle_size}] candles");
        papertrader.running_in_Chester = true;
        await client.connect(symbol_pair, process);

    }

    //callback function
    void process(IBinanceStreamKline BinanceCandle)
    {

        //build desired candle if enough data
        Candle processed_candle = new Candle();
        processed_candle = manager.make(BinanceCandle);

        if (!fetch_completed)
        {

            new_live_candles.Add(processed_candle);

            if (new_live_candles.Count() == 1)
            {
                Console.WriteLine("preheat?");
                warmup = Convert.ToInt32(Console.ReadLine());
                log($"starting to fetch data for warmup..", "debug");
                DateTime first_candle_time = new_live_candles.First().open_time;
                DateTime candle_before_first = first_candle_time.AddMinutes(-1);
                int warmup_in_minutes = (warmup - 1) * candle_size;
                DateTime backwards_time = candle_before_first.AddMinutes(-warmup_in_minutes);


                fetch_warmup_candles = manager.fetch_candles(symbol_pair, backwards_time, candle_before_first, path + "tmp");
                fetch_warmup_candles.Start();
            }


            if (fetch_warmup_candles.IsCompleted)
            {
                log($"finished warmup download", "debug");
                fetch_completed = true;
            }
        }


        if (fetch_completed)
        {
            if (!loadet_into_strategy)
            {
                log($"loading download data", "debug");
                List<Candle> data = maker.load_from_json(path + "tmp");
                log($"download_data_length{data.Count()}", "debug");
                log($"combining with new live data...", "debug");
                data.AddRange(new_live_candles);
                log($"download_data_length{data.Count()}", "debug");

                foreach (Candle c in data)
                {
                    log($"{c.close}");
                    feed_to_selected_instance(c);

                }

                loadet_into_strategy = true;
                log($"im hot baby!", "debug");
            }


        }



        if (loadet_into_strategy)
        {
            feed_to_selected_instance(processed_candle);
        }


    }


    void feed_to_selected_instance(Candle processed_candle)
    {

        candles.Add(processed_candle);

        if (candles.Count() >= candle_size)
        {
            processed_candle = maker.make_size(candle_size, candles)[0];
            if (option == "BROKER")
            {
                execute(processed_candle);
                log("calling execute");
            }
            else
            {
                papertrader.update(processed_candle);
                log("calling papertrader.update()");
            }

            candles.Clear();
        }
    }


    public async void execute(Candle candle)
    {

        if (!initiated)
        {
            strategy.init();
            initiated = true;
            warmup = warmup + new_live_candles.Count();
        }
        strategy.update_indicators(candle);
        strategy.update(candle);

        counter++;
        int result = 0;
        if (counter == warmup)
        {
            log($"warmup completed", "INSTANCES");
        }

        if (counter >= warmup)
        {
            result = strategy.check();
            log($"strategy_check_result = {result}", "INSTANCES");
            update_live_monitor(result, candle);

            if (result != 0)
            {
                await execute_order(result, symbol_pair);
            }

        }
        else
        {
            log($"still in warmup [{counter}/{warmup}]", "INSTANCES");
        }

    }

    public async void update_live_monitor(int result, Candle new_candle)
    {
        ACTION action = ACTION.nothing;
        if (result > 0)
        {
            action = ACTION.buy;
        }
        else if (result < 0)
        {
            action = ACTION.sell;
        }
        decimal? total_capital_of_instance = await get_total_symbol_balance(symbol_pair);
        if (total_capital_of_instance != null)
        {
            decimal total_capital = total_capital_of_instance.Value;
            log($"total capital = {total_capital}");
            live_performance_monitor.update(new_candle.close_time, new_candle.close, total_capital, action);
        }
    }


    //constructor
    public Chester(string option, string symbol_pair, IStrategy strategy, int candle_size)
    {
        this.symbol_pair = symbol_pair;
        this.strategy = strategy;
        this.option = option;
        this.candle_size = candle_size;
        this.papertrader = new Simulator(1000, (decimal)0.1, (decimal)0.1, strategy);
        this.live_performance_monitor = new Analyzer();
    }
}
