using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float _followSpeed = 10;
    [SerializeField] Vector2 _zoomLimits;

    float _boundsSize;
    GameManager _game;
    Vector3 _offset;

    void Start()
    {
        _game = GameManager.Instance;
        _offset = transform.position;
    }

    private void LateUpdate()
    {
        if (_game.Players.Count == 0) return;

        var center = GetBoundsCenter();
        center.z = -Mathf.Lerp(_zoomLimits.x, _zoomLimits.y, _boundsSize / _zoomLimits.y);
        var pos = Vector3.Lerp(transform.position, center, _followSpeed * Time.deltaTime);
        transform.position = pos;
    }

    Vector3 GetBoundsCenter()
    {
        var targets = _game.Players;
        if (targets.Count == 1) return targets[0].transform.position;

        Bounds bounds = new(targets[0].transform.position, Vector3.zero);

        foreach (var target in targets)
        {
            bounds.Encapsulate(target.transform.position);
        }
        _boundsSize = bounds.size.x;

        return bounds.center;
    }
}
