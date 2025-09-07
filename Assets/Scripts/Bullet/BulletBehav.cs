using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehav : MonoBehaviour
{
    [SerializeField]LayerMask WhatShouldtheBulletCollide;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Unity Stores the LayerMask as BitMask, and each layer is just an int.
        // 1 >> collision.gameObject.layer: creats a bitmask with only the layers bit set
        // WhatShouldtheBulletCollide: performs bitwise AND with selected layerMask
        if (collision.gameObject.CompareTag("BulletInteractable"))
        {
            PoolManager.ReturnObjectToPool(gameObject);
        }
    }
}
