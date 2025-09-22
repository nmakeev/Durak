using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _faceSprite;
    [SerializeField] private Sprite _backSprite;
    [SerializeField] private bool _opened;

    //TODO: move to scriptable object
    [SerializeField] private float _openTimeSeconds = 0.3f;
    [SerializeField] private float _followSpeed = 15f;

    [SerializeField] private float _tiltAmountZ = 12f;
    [SerializeField] private float _tiltFromYFactor = 0.3f;
    [SerializeField] private float _springDamping = 6f;
    [SerializeField] private float _returnDamping = 12f;

    [SerializeField] float _idleSpeedThreshold = 60f;
    [SerializeField] float _deadZone = 20f;

    private Tweener _scaleTweener;

    private bool _isDragging;
    private Vector3 _targetPosition;
    private Vector3 _lastPosition;

    private float _currentTiltZ;
    private float _tiltVelZ;

    private void Start()
    {
        SyncImageState();
    }

    private void Update()
    {
        if (_isDragging)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _followSpeed);

            var delta = transform.position - _lastPosition;
            var speed = (float)(delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f));
            _lastPosition = transform.position;

        
            var raw = (float)(-delta.x / Mathf.Max(Time.deltaTime, 0.0001f) + (delta.y / Mathf.Max(Time.deltaTime, 0.0001f)) * _tiltFromYFactor);
            var targetTiltZ = (speed < _deadZone) ? 0f : Mathf.Clamp(raw, -_idleSpeedThreshold, _idleSpeedThreshold) / _idleSpeedThreshold * _tiltAmountZ;
            var damping = (speed <= _idleSpeedThreshold) ? _returnDamping : _springDamping;

            _currentTiltZ = Mathf.SmoothDampAngle(_currentTiltZ, targetTiltZ, ref _tiltVelZ, 1f / damping);
        }
        else
        {
            _currentTiltZ = Mathf.SmoothDampAngle(_currentTiltZ, 0f, ref _tiltVelZ, 1f / _returnDamping);
        }

        var euler = transform.localEulerAngles;
        euler.x = 0f; 
        euler.y = 0f;
        euler.z = _currentTiltZ;
        transform.localEulerAngles = euler;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;
        _targetPosition = transform.position;
        _lastPosition = transform.position;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        _targetPosition = eventData.position;
    }

    public void SwitchState()
    {
        if (_scaleTweener != null)
        {
            _scaleTweener.Kill();
        }


        var target = _opened ? -1 : 1;
        _scaleTweener = transform.DOScaleX(target, _openTimeSeconds)
            .OnUpdate(() =>
        {
            var opened = transform.localScale.x > 0f;
            if (opened == _opened)
            {
                return;
            }

            _opened = opened;
            _image.sprite = _opened ? _faceSprite : _backSprite;
        });
    }

    private void SyncImageState()
    {
        _image.sprite = _opened ? _faceSprite : _backSprite;
    }
}
