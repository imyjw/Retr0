using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float spawnRateMin = 0.5f;
    public float spawnRateMax = 3f;

    public Transform target;
    private float spawnRate;
    private float timeAfterSpawn;

    void Start()
    {
        timeAfterSpawn = 0f;
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        target=FindFirstObjectByType<PlayerController>().transform; //PlayerController컴포넌트를 가진 오브젝트를 찾아서 그 트랜스폼 컴포넌트를 타겟으로 설정

    }

    // Update is called once per frame
    void Update()
    {
        
        timeAfterSpawn += Time.deltaTime;

        if (timeAfterSpawn >= spawnRate)
        {
            timeAfterSpawn = 0f;
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.transform.LookAt(target); //생성된 총알이 타겟을 바라보도록 설정

            spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        }
    }
}
