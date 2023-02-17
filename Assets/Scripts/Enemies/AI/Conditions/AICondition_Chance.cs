using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AICondition_Chance : BaseAICondition
{
    public int ChanceInPercentage;

    public override bool CheckCondition()
    {
        return UnityEngine.Random.value <= ChanceInPercentage * 0.01f;
    }
}
