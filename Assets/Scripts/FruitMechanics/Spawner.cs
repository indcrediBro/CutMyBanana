using UnityEngine;
using System.Collections;
using CMB;
using Utilities;

public class Spawner : MonoBehaviour
{
    private Collider spawnArea;

    public GameObject bananaPrefab;

    [Range(0f,1f)] public float bombChance = 0.02f;

    public float spawnRate;
    
    public float minSpawnDelay = 0.5f;
    public float maxSpawnDelay = 1.2f;

    public float minAngle = -15f;
    public float maxAngle = 15f;

    public float minForce = 12f;
    public float maxForce = 18f;

    public float maxLifetime = 6f;

    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
        spawnRate = GameSettings.baseBananaSpawnRate;
    }

    private void OnEnable()
    {
        StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(1f);
        while (enabled)
        {
            GameObject prefab = bananaPrefab;

            Vector3 pos = new Vector3(
                Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
                Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z)
            );

            Quaternion rot = Quaternion.Euler(0,0,Random.Range(minAngle, maxAngle));
            var go = ObjectPooler.SpawnObject(prefab, pos, rot);
            // Destroy(go, maxLifetime);

            float force = Random.Range(minForce, maxForce);
            var rb = go.GetComponent<Rigidbody>();
            if (rb) rb.AddForce(go.transform.up * force, ForceMode.Impulse);

            // spawn delay can be modified by upgrades
            var upMgr = FindFirstObjectByType<UpgradeManager>();
            float spawnMultiplier = 1f;
            if (upMgr != null)
            {
                // compute average spawn multiplier for current tier's purchased upgrades (simple approach)
                var curTier = FindFirstObjectByType<TierManager>().GetCurrentTier();
                foreach (var u in upMgr.GetUpgradesForTier(curTier))
                {
                    var level = upMgr.GetUpgradeLevel(u.id);
                    if (level > 0) spawnMultiplier *= Mathf.Pow(u.spawnRateMultiplier, level);
                }
            }
            // float delay = Random.Range(minSpawnDelay, maxSpawnDelay) / spawnMultiplier;
            yield return new WaitForSeconds(spawnRate / spawnMultiplier);
        }
    }
}
