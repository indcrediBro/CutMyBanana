using System;
using Utilities;
using UnityEngine;

public class Banana : MonoBehaviour
{
    public bool shouldDestroy = false;
    
    public GameObject whole;
    public GameObject sliced;

    private Rigidbody fruitRigidbody;
    private Collider fruitCollider;
    
    private Vector3 originalLocalPositionWhole;
    private Vector3 originalLocalPositionHalfA;
    private Vector3 originalLocalPositionHalfB;
    
    
    private void OnEnable()
    {
        fruitRigidbody = GetComponent<Rigidbody>();
        fruitCollider = GetComponent<Collider>();
        if (sliced) sliced.SetActive(false);
        originalLocalPositionWhole = whole.transform.localPosition;
        var halves = sliced.GetComponentsInChildren<Rigidbody>();
        originalLocalPositionHalfA = halves[0].transform.localPosition;
        originalLocalPositionHalfB = halves[1].transform.localPosition;
        
        Invoke(nameof(DisableAfterTime), 6f);
    }

    private void Slice(Vector3 direction, Vector3 position, float force)
    {
        
        CMB.GameEvents.OnSlice.Invoke();
        CMB.GameEvents.OnCurrencyGained.Invoke();
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
        }
        Invoke(nameof(DisableAfterTime), 3f);
    }
    
    private void DisableAfterTime()
    {
        gameObject.SetActive(false);
        
        // Reset state
        if (fruitCollider) fruitCollider.enabled = true;
        fruitRigidbody.linearVelocity = Vector3.zero;
        if (whole) whole.SetActive(true);
        if (sliced) sliced.SetActive(false);
        whole.transform.localPosition = originalLocalPositionWhole;
        var halves = sliced.GetComponentsInChildren<Rigidbody>();
        halves[0].linearVelocity = Vector3.zero;
        halves[1].linearVelocity = Vector3.zero;
        halves[0].transform.localPosition = originalLocalPositionHalfA;
        halves[1].transform.localPosition = originalLocalPositionHalfB;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var blade = other.GetComponent<Slicer>();
        if (blade == null) return;
        Slice(blade.direction, blade.transform.position, blade.sliceForce);
    }
    private bool isReturnedToPool = false;

    private void OnDisable()
    {
        if (!isReturnedToPool)
        {
            ObjectPooler.ReturnObjectToPool(gameObject);
            isReturnedToPool = true;
        }
    }
}