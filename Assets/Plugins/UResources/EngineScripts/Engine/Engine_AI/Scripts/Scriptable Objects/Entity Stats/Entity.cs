using UnityEngine;

namespace Engine.AI
{
    [CreateAssetMenu(menuName = "AI/Stats")]
    public class Entity : ScriptableObject
    {
        [SerializeField] internal float maxHealth;
        [SerializeField] internal float speed;
    }
}