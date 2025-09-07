using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    Vector3 _startposition;
    Vector3 _targetposition;
    float _distance;
    float progress;
    public bool ScriptIsBeingCalled = true;

    [SerializeField] float _speed = 80f;
    [SerializeField] float _trialLife = 0.1f;

    void Start()
    {
    }

    public void initialize(Vector3 startPosition, Vector3 targetPosition)
    {
        _startposition = new Vector3(startPosition.x, startPosition.y, -1);
        _targetposition = new Vector3(targetPosition.x, targetPosition.y, -1);
        _distance = Vector3.Distance(_startposition, _targetposition);

        progress = 0f;
        transform.position = _startposition;
    }

    void Update()
    {
        progress += (_speed * Time.deltaTime) / _distance;
        transform.position = Vector3.Lerp(_startposition, _targetposition, progress);

        if(progress >= 1f)
        {
            //StopAllCoroutines();
            StartCoroutine(DisableAfterTrail());
        }
    }

    IEnumerator DisableAfterTrail()
    {
        yield return new WaitForSeconds(_trialLife + 1.5f);
        PoolManager.ReturnObjectToPool(gameObject);
    }

    public IEnumerator DisableWhenHit()
    {
        yield return new WaitForSeconds(_trialLife + 0.1f);
        PoolManager.ReturnObjectToPool(gameObject);
    }
}
