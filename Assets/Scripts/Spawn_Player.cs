using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Player : MonoBehaviour
{
    [SerializeField] Transform SpawnPoint;
    [SerializeField] GameObject PlayerPrefab;
    [SerializeField] bool playerDead = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(playerDead)
        {
            StartCoroutine(Respawn());
            playerDead = false;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Triggered with: " + collision.name + " | Tag: " + collision.tag);

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Triggered");
            Destroy(collision.transform.root.gameObject);
            playerDead = true;
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 spawnPosition = SpawnPoint.position;    
        Instantiate(PlayerPrefab, spawnPosition, Quaternion.identity);
    }
}
