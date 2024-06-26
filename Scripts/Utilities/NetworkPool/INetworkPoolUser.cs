namespace Armony.Scripts.Utilities.NetworkPool
{
    public interface INetworkPoolUser
    {
        NetworkPool NetworkPool { get; set; }
        public void ReleaseProjectile(int index);
    }
}