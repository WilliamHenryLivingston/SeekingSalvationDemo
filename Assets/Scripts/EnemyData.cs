using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "AI/Enemy Variant")]
public class EnemyData : ScriptableObject
{
    public string enemyName = "Default Enemy";
    public float moveSpeed = 3f;
    public Color enemyColor = Color.white; // Optional for visual difference
}
