using UnityEngine;

public class VisibilityArea : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayer;

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    public bool RaycastTarget(Transform target)
    {
        RaycastHit hit;

        Vector3 convertTarget = new Vector3(target.position.x, target.position.y + 1f, target.position.z);
        Vector3 convertPosition = new Vector3(_transform.position.x, _transform.position.y + 1f, _transform.position.z);
        Vector3 direction = convertTarget - convertPosition;

        if (Physics.Raycast(convertPosition, direction, out hit, 100f, _targetLayer))
        {
            if (hit.transform == target)
            {
                LastPoint = hit.transform.position;
                Debug.DrawRay(convertPosition, direction, Color.red);
                return IsVisible = true;
            }
            else
            {
                Debug.DrawRay(convertPosition, direction, Color.black);
                return IsVisible = false;
            }
        } 
        else
        {
            return IsVisible = false;
        }
    }

    public Vector3 LastPoint { get; set; }
    public bool IsVisible { get; set; }
}
