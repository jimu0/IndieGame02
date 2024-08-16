using System;
using System.Collections.Generic;

namespace File_jim.Scripts.ObjectPool
{
  /// <summary>
  ///   <para>A stack based Pool.IObjectPool_1.</para>
  /// </summary>
  public class ObjectPool<T> : IDisposable, IObjectPool<T> where T : class
  {
    internal readonly Stack<T> m_Stack;
    private readonly Func<T> m_CreateFunc;
    private readonly Action<T> m_ActionOnGet;
    private readonly Action<T> m_ActionOnRelease;
    private readonly Action<T> m_ActionOnDestroy;
    private readonly int m_MaxSize;
    internal bool m_CollectionCheck;

    public int CountAll { get; private set; }

    public int CountActive => this.CountAll - this.CountInactive;

    public int CountInactive => this.m_Stack.Count;

    public ObjectPool(
      Func<T> createFunc,
      Action<T> actionOnGet = null,
      Action<T> actionOnRelease = null,
      Action<T> actionOnDestroy = null,
      bool collectionCheck = true,
      int defaultCapacity = 10,
      int maxSize = 10000)
    {
      if (createFunc == null)
        throw new ArgumentNullException(nameof (createFunc));
      if (maxSize <= 0)
        throw new ArgumentException("Max Size must be greater than 0", nameof (maxSize));
      this.m_Stack = new Stack<T>(defaultCapacity);
      this.m_CreateFunc = createFunc;
      this.m_MaxSize = maxSize;
      this.m_ActionOnGet = actionOnGet;
      this.m_ActionOnRelease = actionOnRelease;
      this.m_ActionOnDestroy = actionOnDestroy;
      this.m_CollectionCheck = collectionCheck;
    }

    public T Get()
    {
      T obj;
      if (this.m_Stack.Count == 0)
      {
        obj = this.m_CreateFunc();
        ++this.CountAll;
      }
      else
        obj = this.m_Stack.Pop();
      Action<T> actionOnGet = this.m_ActionOnGet;
      if (actionOnGet != null)
        actionOnGet(obj);
      return obj;
    }

    public PooledObject<T> Get(out T v) => new PooledObject<T>(v = this.Get(), (IObjectPool<T>) this);

    public void Release(T element)
    {
      if (this.m_CollectionCheck && this.m_Stack.Count > 0 && this.m_Stack.Contains(element))
        throw new InvalidOperationException("Trying to release an object that has already been released to the pool.");
      Action<T> actionOnRelease = this.m_ActionOnRelease;
      if (actionOnRelease != null)
        actionOnRelease(element);
      if (this.CountInactive < this.m_MaxSize)
      {
        this.m_Stack.Push(element);
      }
      else
      {
        Action<T> actionOnDestroy = this.m_ActionOnDestroy;
        if (actionOnDestroy != null)
          actionOnDestroy(element);
      }
    }

    public void Clear()
    {
      if (this.m_ActionOnDestroy != null)
      {
        foreach (T obj in this.m_Stack)
          this.m_ActionOnDestroy(obj);
      }
      this.m_Stack.Clear();
      this.CountAll = 0;
    }

    public void Dispose() => this.Clear();
  }
}