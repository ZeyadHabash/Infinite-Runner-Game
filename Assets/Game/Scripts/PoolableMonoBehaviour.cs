using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Events;

namespace GDK
{
    public abstract class PoolableMonoBehaviour : MonoBehaviour
    {
        public UnityAction<PoolableMonoBehaviour> onRelease;

        private SuperObjectPoolSO pool;

        public void RegisterPool(SuperObjectPoolSO pool)
        {
            this.pool = pool;
        }

        [ContextMenu("Release")]
        public void Release()
        {

            if (pool != null)
            {
                this.pool.Release(this);
            }
            if (onRelease != null)
            {
                onRelease(this);
                onRelease = null;
            }
        }

        public virtual void OnObjectPoolCreate() { }
        public virtual void OnObjectPoolTake() { }
        public virtual void OnObjectPoolReturn() { } // Called before getting deactivated
        public virtual void OnObjectPoolDestroy() { } // Called before getting destroyed
        public virtual void OnPostGet() { } // Idea

    }
}