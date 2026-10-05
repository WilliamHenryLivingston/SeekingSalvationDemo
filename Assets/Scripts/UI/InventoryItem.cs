// Copyright 2025 William Livingston
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "NewInventoryItem", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    [Header("General Item Data")]
    public string itemName = "New Item";
    public Sprite itemIcon = null;
    public GameObject itemPrefab = null;

    [Header("Energy Item Properties")]
    [Tooltip("How heavy this item is. Higher values slow the player down more.")]
    public float weight = 0f;

    [Tooltip("How much spiritual energy this item contributes when offered to a shrine.")]
    public int spiritualSignificance = 0;
}