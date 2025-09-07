using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KnockBack_Effect : MonoBehaviour
{
    public float KnockBackTime = 0.2f;
    public float hitDirectionForce = 10f;
    public float constForce = 1.0f;

    public bool IsBeingKnockedBack { get; private set; }

    public IEnumerator KnockBackAction(Vector2 hitDirection, Vector2 ConstantForceDirection)
    {
        IsBeingKnockedBack = true;

        float elapsedTime = 0f;
        while(elapsedTime < KnockBackTime)
        {

            //If the condition is less than KnockBackTImer we are gonna iterate the timer
            elapsedTime += Time.fixedDeltaTime;

            // We cant run a coroutine in a fixedUpdate but we can make this run as exactly as fixedUpdate() using WaitForFixedUpdate() now the iteration and return syncs up
            yield return new WaitForFixedUpdate();

        }

        IsBeingKnockedBack = false;
    }
}
