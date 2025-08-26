using UnityEngine;

public class Banana : MonoBehaviour
{
    public GameObject whole;
    public GameObject sliced;
    public double points = 1.0;

    private Rigidbody fruitRigidbody;
    private Collider fruitCollider;
    private CMBBootstrap bootstrap;

    private void Awake()
    {
        fruitRigidbody = GetComponent<Rigidbody>();
        fruitCollider = GetComponent<Collider>();
        bootstrap = FindFirstObjectByType<CMBBootstrap>();
        if (sliced) sliced.SetActive(false);
    }

    public void Slice(Vector3 direction, Vector3 position, float force)
    {
        var save = SaveSystem.Load() ?? new SaveData();
        double perSliceValue = save.perSlice;
        // give currency & slice count
        var gs = FindFirstObjectByType<GameState>();
        gs.AddCurrency((int)perSliceValue);
        gs.AddSlice(1);

        // Update tasks: only for active tier & only tasks that mention "Slice" in description OR have specific design
        int tier = FindFirstObjectByType<TierManager>().GetCurrentTier();
        var tm = FindFirstObjectByType<TaskManager>();
        tm.AddProgressForTier(tier,
            def => def.description.ToLower().Contains("slice") || def.title.ToLower().Contains("slice"), 1);

        // Visual swap
        if (fruitCollider) fruitCollider.enabled = false;
        if (whole) whole.SetActive(false);
        if (sliced) sliced.SetActive(true);

        // Rotate based on the slice angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        sliced.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();
        // Add a force to each slice based on the blade direction
        foreach (Rigidbody slice in slices)
        {
            slice.linearVelocity = fruitRigidbody.linearVelocity;
            slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
            Destroy(gameObject, 3f); // let slices play then cleanup
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var blade = other.GetComponent<Slicer>();
        if (blade == null) return;
        Slice(blade.direction, blade.transform.position, blade.sliceForce);
    }
}