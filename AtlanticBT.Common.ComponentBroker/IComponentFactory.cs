namespace AtlanticBT.Common.ComponentBroker
{
    public interface IComponentFactory<out T>
    {
        T Create();
    }
}
