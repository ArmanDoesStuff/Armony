#if ARMONY_NETCODE
namespace Armony.Scripts.Utilities.NetworkPool
{
    public interface INetworkPoolUser
    {
        NetworkPool NetworkPool { get; set; }
        void ReleaseProjectile(int index);
        void ClearFromPool(int index);
    }
}
#endif