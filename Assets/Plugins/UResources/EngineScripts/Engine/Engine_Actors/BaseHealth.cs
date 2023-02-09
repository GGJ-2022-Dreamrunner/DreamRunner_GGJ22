using System;
using UnityEngine;
using UnityEngine.Events;

namespace Engine.Engine_Actors
{
    public class BaseHealth : MonoBehaviour
    {
        #region public variables

        /// <summary>
        /// What to call when the entity's health is below zero.
        /// </summary>
        public UnityEvent onDeath;

        #endregion
        #region protected variables

        [SerializeField] public float maxHealth;
        protected float CurrentHealth;

        #endregion

        protected void Awake()
        {
            CurrentHealth = maxHealth;
        }

        /// <summary>
        /// Damage this actor. Only takes positive values.
        /// </summary>
        /// <param name="damage">A positive value of damage to deal (subtract)</param>
        /// <exception cref="ArgumentOutOfRangeException">Exception on negative overload.</exception>
        public void DealDamage(float damage)
        {
            if (damage < 0)
                throw new ArgumentOutOfRangeException((string) "Only positive values allowed");

            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                onDeath.Invoke();
            }
        }

        /// <summary>
        /// Heal this actor. Only takes positive values.
        /// </summary>
        /// <param name="restored">A positive value of damage to heal (add)</param>
        /// <exception cref="ArgumentOutOfRangeException">Exception on negative overload.</exception>
        public void HealDamage(float restored)
        {
            if (restored < 0)
                throw new ArgumentOutOfRangeException((string) "Only positive values allowed");

            CurrentHealth += restored;
            Mathf.Clamp(CurrentHealth, 0, maxHealth);
        }
    }
}