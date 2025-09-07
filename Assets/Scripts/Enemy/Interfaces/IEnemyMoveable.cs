using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyMovable
{
    Rigidbody2D RB { get; set; }

    bool isFacingRight { get; set; }

    void MoveEnemy(Vector3 velocity);

    void CheckForLeftorRightFacing(Vector2 velocity);
}

