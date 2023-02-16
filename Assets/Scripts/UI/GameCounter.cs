using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;

public abstract class GameCounter : BaseGameObject
{
    public abstract float Value { get; }
    public abstract float MinValue { get; }
    public abstract float MaxValue { get; }

    public abstract event Action<float> OnValueChange;
}
