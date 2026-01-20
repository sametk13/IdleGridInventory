using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBrain : MonoBehaviour, IBrain
{
    private readonly Dictionary<Type, object> _map = new();
    private readonly Dictionary<Type, MonoBehaviour> _owners = new();

    protected virtual void Awake()
    {
        CollectAndRegisterModules();
        InjectModulesOncePerInstance();
    }

    private void CollectAndRegisterModules()
    {
        _map.Clear();
        _owners.Clear();

        var behaviours = GetComponentsInChildren<MonoBehaviour>(true);
        for (int i = 0; i < behaviours.Length; i++)
        {
            var mb = behaviours[i];
            if (mb == null) continue;

            // Only register modules that participate in injection.
            if (mb is not IBrainModule) continue;

            RegisterModule(mb);
        }
    }

    private void RegisterModule(MonoBehaviour module)
    {
        var concreteType = module.GetType();

        // Always register concrete type
        TryRegister(concreteType, module, module);

        // Register interfaces implemented by this module
        var interfaces = concreteType.GetInterfaces();
        for (int i = 0; i < interfaces.Length; i++)
        {
            var iface = interfaces[i];
            if (!ShouldRegisterInterface(iface)) continue;

            TryRegister(iface, module, module);
        }
    }

    private bool ShouldRegisterInterface(Type iface)
    {
        // Avoid registering infrastructure interfaces as dependencies.
        if (iface == typeof(IBrain)) return false;
        if (iface == typeof(IBrainModule)) return false;

        return true;
    }

    private void TryRegister(Type key, object instance, MonoBehaviour owner)
    {
        if (_map.TryGetValue(key, out var existing))
        {
            var existingOwner = _owners.TryGetValue(key, out var o) ? o : null;

            Debug.LogError(
                $"[{GetType().Name}] Duplicate dependency detected for key '{key.Name}'. " +
                $"Existing: '{existingOwner?.GetType().Name ?? "Unknown"}' on '{existingOwner?.name ?? "Unknown"}', " +
                $"New: '{owner.GetType().Name}' on '{owner.name}'.",
                owner
            );

            return;
        }

        _map.Add(key, instance);
        _owners.Add(key, owner);
    }

    private void InjectModulesOncePerInstance()
    {
        // IMPORTANT:
        // _map contains multiple keys (concrete + interfaces) pointing to the same instance.
        // We must ensure Inject is called only once per module instance.
        var injected = new HashSet<IBrainModule>();

        foreach (var kvp in _map)
        {
            if (kvp.Value is not IBrainModule module)
                continue;

            if (!injected.Add(module))
                continue;

            module.Inject(this);
        }
    }

    public T Get<T>() where T : class
    {
        var key = typeof(T);

        if (_map.TryGetValue(key, out var instance))
            return instance as T;

        Debug.LogError(
            $"[{GetType().Name}] Requested dependency not found: '{key.Name}'.",
            this
        );

        return null;
    }
}
