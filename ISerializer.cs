namespace Tcp.Communication
{
    public interface ISerializer
    {
        string Serialize<T>(T data);

        object Deserialize(string message);
    }
}