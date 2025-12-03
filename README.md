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
You can also visualize the distribution of Sharpe ratios to evaluate the statistical significance of your strategy.
<img width="600" height="400" alt="monte carlo" src="https://github.com/user-attachments/assets/e209d835-fddf-4540-9bb3-67f9de0d1d19" />

---

## How to Start

To start the **program**, just type:

```bash
dotnet run
```


### License
This project is licensed under the Non-Commercial License found in the LICENSE file.

### Disclaimer
This project is for educational and personal use only.
It does not provide financial advice, trading advice, or investment recommendations.
All trading involves risk. You are responsible for your own decisions.

### Credits / Inspiration
The project was inspired by the work of Mike van Rossum and his Gekko trading bot.
No original Gekko code is included.
