# About This Project

Chester is a framework for developing, evaluating, and running **algorithmic** trading strategies.

<img width="450" height="450" alt="ChatGPT Image Oct 1, 2025, 07_49_32 PM" src="https://github.com/user-attachments/assets/13504cee-52e1-4d26-8afc-804bff7cf7a9" />

## Features

- download historical data and backtest your strategy on it
- easy strategy creation by defining trading logic in one file
- easy usage of statistical indicators
- advanced risk metrics and detailed information for every trade
- visualization of price, entry, and exit positions
- Monte Carlo testing for evaluating statistical significance
- paper trading with simulated money on live-market data
- automatic order placement via direct WebSocket connection to your exchange
- start and stop up to 300 independent instances for live and paper trading

## Welcome!

This is my first real project published on GitHub, and it’s still under active development.

- No AI or copy-pasted code was used; this project is built from scratch for **educational purposes**.
- The goal is to learn, experiment, and create a personal trading framework.
- Feedback, suggestions, and help are **always welcome** — feel free to open issues or submit pull requests.

---

## Screenshots

### Visualization of price action with buy and sell positions
<img width="600" height="400" alt="graph" src="https://github.com/user-attachments/assets/aa2b4866-c114-4cbc-88b5-3b255d57c310" />

### Detailed risk metrics
<img width="600" height="400" alt="full metrics" src="https://github.com/user-attachments/assets/d7014250-3e6d-434f-82b6-11e5cf4f7e70" />

### Simulator calculations
<img width="600" height="400" alt="simulator" src="https://github.com/user-attachments/assets/84fb698e-e9ea-4527-ad09-760acd41f4cb" />

### Monitoring running instances
<img width="600" height="400" alt="more" src="https://github.com/user-attachments/assets/7b212442-2b36-455f-b407-1732fc7566dd" />

### Distribution of Sharpe ratios
<img width="600" height="400" alt="monte carlo" src="https://github.com/user-attachments/assets/e209d835-fddf-4540-9bb3-67f9de0d1d19" />

---

## How to Start

To start the **program**, just type:

```bash
dotnet run
```


# Controls

- Move **selection**: <kbd>↑</kbd> / <kbd>↓</kbd>
- Cycle through logs: <kbd>←</kbd> / <kbd>→</kbd>
- Confirm selection: <kbd>Enter</kbd>
- Go back: <kbd>Return</kbd>

### Logging

Anywhere in your code, you can use the `Logger` class to create dynamic log channels:

`using Logger;`

`log("your_log", "your_channel_name");`

- Dynamically creates a new log channel named `"your_channel_name"`.  
- You can create **as many channels as you like**.  
- If no channel is provided, logs go to the **default CLI channel**.

# Downloading Data

Before running a backtest, you must **download market data**:

1. Select **Download Data** and press <kbd>Enter</kbd>.
2. When asked to **filter symbols**:
   - Press <kbd>Enter</kbd> to skip filtering, **or**
   - Enter a **quote asset** (e.g., `XVG`, `BTC`) to filter pairs.
3. After choosing a pair, enter your **date range**:
   - **From:** e.g., `2024.1.1`
   - **To:** e.g., `2025.1.1` (any datetime format is accepted)

> Note: Data is downloaded in **1-minute candles**. You can backtest on **any timeframe** using this data.

# Backtest Flow

- To run a backtest, you must select:  
  1. A **dataset** (downloaded market data)  
  2. A **strategy**  
  3. A **timeframe** in **minutes** (1 = 1m, 60 = 1h)
- After a backtest finishes and the **risk metrics** are displayed, press <kbd>Enter</kbd>.
- Before returning to the menu, you will be asked whether you want to run a **Monte Carlo simulation**:
  - Press **anything or nothing**, then <kbd>Enter</kbd> to **skip**.
  - Type **`y`** and press <kbd>Enter</kbd> to **run the Monte Carlo simulation**.
- After the Monte Carlo prompt, you return to the main menu.
- Press <kbd>Enter</kbd> again to **inspect the selected trade from the list**.
- While viewing a trade, press <kbd>Enter</kbd> to **close the trade info** and return.

# Starting Live Instances

When starting a live instance (Paper Trader or Live Trader):

- You **cannot filter or select asset pairs from a menu**.  
  - You must type the asset pair manually, e.g., `BTCUSDC` or `ETHBTC`.
- Once the instance receives its **first candle**, it will ask if you want to **preheat**:  
  - Allows downloading historical data to “warm up” the trader.  
  - Specify the **history length** as an integer (number of candles to download).  
  - Avoids waiting for warmup during live trading.
- **Timeframe prompt**: Live instances also require entering the timeframe in **minutes** (1 = 1m, 60 = 1h).

### License
This project is licensed under the Non-Commercial License found in the LICENSE file.

### Disclaimer
This project is for educational and personal use only.
It does not provide financial advice, trading advice, or investment recommendations.
All trading involves risk. You are responsible for your own decisions.

### Credits / Inspiration
The project was inspired by the work of Mike van Rossum and his Gekko trading bot.
No original Gekko code is included.
