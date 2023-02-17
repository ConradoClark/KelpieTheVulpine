using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;

public abstract class BaseAIAction : BaseGameObject
{
    public abstract IEnumerable<IEnumerable<Action>> Execute(BaseEnemyAI source, Func<bool> breakCondition);
    public abstract void OnInterrupt(BaseEnemyAI source);
}
