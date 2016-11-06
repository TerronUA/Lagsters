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

    // Use this for initialization
    void Start()
    {
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

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smooth);

        Vector3 targetDir = follow.position - transform.position + offset;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, smoothRotation * Time.deltaTime, 0.0F);

        Debug.DrawRay(transform.position, newDir, Color.red);

        transform.rotation = Quaternion.LookRotation(newDir);

        transform.up = follow.up;
        Debug.DrawRay(transform.position, transform.up, Color.yellow);

        transform.LookAt(follow.position + offset, transform.up);
    }
}
