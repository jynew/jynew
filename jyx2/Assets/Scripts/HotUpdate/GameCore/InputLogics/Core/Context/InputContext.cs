
namespace Jyx2.InputCore
{
    public interface IJyx2_InputContext
    {
        bool CanUpdate { get; }
        void OnUpdate();
    }
}
