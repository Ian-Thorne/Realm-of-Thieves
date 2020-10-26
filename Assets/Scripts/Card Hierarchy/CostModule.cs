using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ManaColorEnum {
    GREEN = 0,
    RED = 1,
    PURPLE = 2,
    BLUE = 3,
    COLORLESS = 4
}

public class CostModule {

    private Dictionary<ManaColorEnum, uint> costs;

    public CostModule(uint greenCost, uint redCost, uint purpleCost, uint blueCost, uint colorlessCost) {
        costs = new Dictionary<ManaColorEnum, uint>();
        costs.Add(ManaColorEnum.GREEN, greenCost);
        costs.Add(ManaColorEnum.RED, redCost);
        costs.Add(ManaColorEnum.PURPLE, purpleCost);
        costs.Add(ManaColorEnum.BLUE, blueCost);
        costs.Add(ManaColorEnum.COLORLESS, colorlessCost);
    }
}
