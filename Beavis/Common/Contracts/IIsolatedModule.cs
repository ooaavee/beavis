namespace Beavis.Isolation.Contracts
{
    public interface IIsolatedModule
    {
        ModuleResponse Ping(ModuleRequest request);

        ModuleResponse HandleRequest(ModuleRequest request);
    }
}
