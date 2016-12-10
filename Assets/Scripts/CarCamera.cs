using UnityEngine;
using System.Collections;

public class CarCamera : MonoBehaviour
{
    [SerializeField]
    private float distanceAway;
    [SerializeField]
    private float distanceUp;
    [SerializeField]
    private float smooth;
    [SerializeField]
    private float smoothRotation;
    public Transform follow;
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 1.5f, 0f);

    private Vector3 lookDir;
    private Vector3 targetPosition;
    private Transform cameraTransform;

    // Use this for initialization
    void Start()
    {
        cameraTransform = transform;

        follow = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Debugging information should be put here.
    /// </summary>
    void OnDrawGizmos()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        targetPosition = follow.position + follow.up * distanceUp - follow.forward * distanceAway;

        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime * smooth);
        
        Vector3 targetDir = follow.position - cameraTransform.position + offset;
        Vector3 newDir = Vector3.RotateTowards(cameraTransform.forward, targetDir, smoothRotation * Time.deltaTime, 0.0F);

        Debug.DrawRay(cameraTransform.position, newDir, Color.red);

        cameraTransform.rotation = Quaternion.LookRotation(newDir);

        Debug.DrawRay(cameraTransform.position, cameraTransform.up, Color.yellow);

        cameraTransform.up = follow.up;

        Quaternion cameraRotation = Quaternion.LookRotation(cameraTransform.forward, cameraTransform.up);

        Vector3 newOffset = cameraRotation * offset;
        cameraTransform.LookAt(follow.position + newOffset, cameraTransform.up);
    }
}
