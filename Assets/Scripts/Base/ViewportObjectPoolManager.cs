using Licht.Unity.Pooling;

public class ViewportObjectPoolManager : CustomPrefabManager<ViewportObjectPool, CameraPosSync>
{
}

public class ViewportObjectPool : GenericPrefabPool<CameraPosSync>
{

}
