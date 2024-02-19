---
runme:
  id: 01HQ1N8N8JYWYRSN9AH9K0JCP8
  version: v3
---

  #####  Version 1.0 - (Basic Template)
    - Works for stocks and Forext
    - SL = Calculated in ATR
    - TP = Multiply of SL
    - Breakeven
    - Trailing (just as price moves) +2 dollars moves the sl 2 dollars.
    - Template only allowes 1 trade at a time.
    - Moving average filter = No Long under and no short above
    - Rsi Consolidation filter = No trades between the levels
    - Max Spread allowed can be choosen (No trades if spread is higher)
    
  ##### Video
  [Link](https://www.youtube.com/watch?v=jAI6s1WuEus&t=587s)
  
  ##### Strategy conditions  
    ENTRIES
    - If close is in < 20% of range and close < SMA 50 then Buy on close
    - If close is in > 20% of range and close > SMA 50 then Sell Short on close
    
    TRADE EXITS
    1. Stop & Reverse
    2. If close crosses above SMA 50 then sell on close
    3. If close crosses below SMA 50 then buy on close
    4. Stop Loss (tested)
    5. Profit target (tested)