using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Health")]
    public int MaxHP = 3;

    [Header("Movement")]
    public float MoveSpeed = 3.5f;
    public float DetectionRange = 15f;
    public float AttackRange = 2f;
    public float FaceTargetSpeed = 8f;

    [Header("Combat")]
    public int Damage = 1;
    public float AttackRate = 1.5f;
}