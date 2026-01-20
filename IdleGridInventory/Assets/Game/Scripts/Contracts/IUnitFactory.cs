using UnityEngine;

public interface IUnitFactory<T> where T : Component
{
    T Create(Vector3 position, Quaternion rotation, Transform parent = null);
}
