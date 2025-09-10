using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Slicer : MonoBehaviour
{
    public float sliceForce = 5f;
    public float minSliceVelocity = 0.01f;
    public float sliceTimeLimit = 0.5f;
    
    private Camera mainCamera;
    private Collider sliceCollider;
    private TrailRenderer sliceTrail;

    public Vector3 direction { get; private set; }
    public bool slicing { get; private set; }
    private float currentSliceTime = 0f;

    private void Awake()
    {
        mainCamera = Camera.main;
        sliceCollider = GetComponent<Collider>();
        sliceTrail = GetComponentInChildren<TrailRenderer>();
    }

    private void OnEnable() => StopSlice();
    private void OnDisable() => StopSlice();

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartSlice();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopSlice();
        }
        else if (slicing)
        {
            ContinueSlice();
        }
    }

    private void StartSlice()
    {
        transform.position = GetMouseWorldPosition();
        slicing = true;
        currentSliceTime = 0f;
        
        if (sliceCollider)
        {
            sliceCollider.enabled = true;
        }

        if (sliceTrail)
        {
            sliceTrail.enabled = true;
            sliceTrail.Clear();
            sliceTrail.gameObject.SetActive((true));
        }
    }

    private bool SliceTimerReached()
    {
        if(currentSliceTime>sliceTimeLimit)
        {
            currentSliceTime = 0f;
            return true;
        }
        return false;
    }

    private void StopSlice()
    {
        slicing = false;
        if (sliceCollider)
        {
            sliceCollider.enabled = false;
        }
        
        if (sliceTrail)
        {
            sliceTrail.enabled = false;
            sliceTrail.Clear();
            sliceTrail.gameObject.SetActive((false));
        }
        
        
    }

    private void ContinueSlice()
    {
        currentSliceTime+= Time.deltaTime;
        
        if (SliceTimerReached())
        {
            StopSlice();
            return;
        }
        sliceCollider.enabled = true;
        var newPos = GetMouseWorldPosition();
        direction = newPos - transform.position;
        float velocity = direction.magnitude / Time.deltaTime;
        if (sliceCollider) sliceCollider.enabled = velocity > minSliceVelocity;
        transform.position = newPos;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0f;
        return position;
    }
}