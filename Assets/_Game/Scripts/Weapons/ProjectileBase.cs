using UnityEngine;

namespace IronTide.Weapons
{
    /// <summary>
    /// Base class for all projectiles. Handles lifetime and pool return.
    /// Subclasses implement movement and hit logic.
    /// </summary>
    public abstract class ProjectileBase : MonoBehaviour
    {
        protected WeaponData  Data;
        protected string      AttackerId;
        protected int         AttackerTeam;
        protected float       Damage;

        private float _remainingLife;
        private GameObject _poolPrefabRef;

        /// <summary>Call after retrieving from pool to (re)initialize the projectile.</summary>
        public virtual void Init(WeaponData data, string attackerId, int attackerTeam,
                                 float damage, GameObject poolPrefabRef)
        {
            Data           = data;
            AttackerId     = attackerId;
            AttackerTeam   = attackerTeam;
            Damage         = damage;
            _remainingLife = data.projectileLifetime;
            _poolPrefabRef = poolPrefabRef;
        }

        protected virtual void Update()
        {
            _remainingLife -= Time.deltaTime;
            if (_remainingLife <= 0f)
                ReturnToPool();
        }

        protected abstract void OnHit(Collider other);

        protected virtual void OnTriggerEnter(Collider other)
        {
            // Ignore self-team hits
            var health = other.GetComponent<Ships.ShipHealth>();
            if (health != null)
                OnHit(other);
        }

        protected void ReturnToPool()
        {
            if (_poolPrefabRef != null)
                Utils.ObjectPool.ReturnToPool(_poolPrefabRef, gameObject);
            else
                gameObject.SetActive(false);
        }
    }
}
