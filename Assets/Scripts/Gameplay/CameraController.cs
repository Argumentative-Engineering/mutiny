using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float _followSpeed = 10;
    [SerializeField] Vector2 _zoomLimits;
    [SerializeField] Vector3 _offset;

    float _boundsSize;

    private bool _zoomLocked = false;
    private Vector3 _lockedPosition;
    private float _lockedZ;

    private void LateUpdate()
    {
        if (_zoomLocked)
        {
            Vector3 target = _lockedPosition + _offset;
            target.z = _lockedZ;
            transform.position = Vector3.Lerp(transform.position, target, _followSpeed * Time.unscaledDeltaTime);
            return;
        }

        if (GameManager.Instance.AlivePlayers.Count == 0) return;

        var center = GetBoundsCenter();
        center.z = -Mathf.Lerp(_zoomLimits.x, _zoomLimits.y, _boundsSize / _zoomLimits.y);
        var pos = Vector3.Lerp(transform.position, center + _offset, _followSpeed * Time.deltaTime);
        transform.position = pos;
    }

    Vector3 GetBoundsCenter()
    {
        var targets = GameManager.Instance.AlivePlayers;
        if (targets.Count == 1) return targets[0].transform.position;

        Bounds bounds = new(targets[0].transform.position, Vector3.zero);
        foreach (var target in targets)
        {
            bounds.Encapsulate(target.transform.position);
        }

        _boundsSize = bounds.size.x;
        return bounds.center;
    }

    public void ZoomToPoint(Vector3 worldPosition, float zoomZOffset)
    {
        _zoomLocked = true;
        _lockedPosition = worldPosition;
        _lockedZ = -zoomZOffset;
    }

    public void ResetZoom()
    {
        _zoomLocked = false;
    }
}
