using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Events;

namespace GDK
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ObjectPooling/Super Object Pool", fileName = "SuperObjectPool")]
    public class SuperObjectPoolSO : ScriptableObject
    {
        public PoolableMonoBehaviour prefab;
        public int defaultCapacity = 10;
        [Space]
        public Vector3 defaultSpawnLocation;
        [Space]
        public bool useDefaultSpawnRotation;
        public Quaternion defaultSpawnRotation;

        private ObjectPool<PoolableMonoBehaviour> objectPool;

        private void OnEnable()
        {
            objectPool = new ObjectPool<PoolableMonoBehaviour>(
                CreatePooledObject,
                OnTakeFromPool,
                OnReturnPool,
                OnDestroyObject,
                false,
                defaultCapacity);
        }

        private PoolableMonoBehaviour CreatePooledObject()
        {
            PoolableMonoBehaviour pm = Instantiate(prefab, defaultSpawnLocation, useDefaultSpawnRotation ? defaultSpawnRotation : Quaternion.identity);
            pm.gameObject.SetActive(true);
            pm.RegisterPool(this);
            pm.OnObjectPoolCreate();
            return pm;
        }

        private void OnTakeFromPool(PoolableMonoBehaviour pm)
        {
            pm.gameObject.SetActive(true);
            pm.OnObjectPoolTake();
        }

        private void OnReturnPool(PoolableMonoBehaviour pm)
        {
            pm.OnObjectPoolReturn();
            pm.gameObject.SetActive(false);
        }

        private void OnDestroyObject(PoolableMonoBehaviour pm)
        {
            pm.OnObjectPoolDestroy();
            Destroy(pm);
        }

        public PoolableMonoBehaviour Get()
        {
            return objectPool.Get();
        }

        public void Release(PoolableMonoBehaviour pm)
        {
            objectPool.Release(pm);
        }
    }


}
