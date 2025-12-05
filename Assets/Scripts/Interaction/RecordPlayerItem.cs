using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class RecordPlayerItem : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;

    private void Reset()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void AttachToTransform(Transform parent, Vector3 localPosition, Quaternion localRotation)
    {
        if (parent == null)
            return;

        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = true;
            _rigidbody.detectCollisions = false;
        }

        transform.SetParent(parent);
        transform.localPosition = localPosition;
        transform.localRotation = localRotation;
    }

    public void Drop(Vector3 worldPosition, Quaternion worldRotation)
    {
        transform.SetParent(null);
        transform.SetPositionAndRotation(worldPosition, worldRotation);

        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = false;
            _rigidbody.detectCollisions = true;
        }
    }
}