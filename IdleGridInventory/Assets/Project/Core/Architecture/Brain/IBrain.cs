public interface IBrain
{
    T Get<T>() where T : class;
}
