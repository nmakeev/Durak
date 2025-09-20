using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _faceSprite;
    [SerializeField] private Sprite _backSprite;
    [SerializeField] private bool _opened;
    [SerializeField] private float _openTimeSeconds = 0.3f;
    [SerializeField] private float _followSpeed = 15f;
    [SerializeField] private float _tiltAmount = 10f;
    [SerializeField] private float _tiltSpeed = 5f;
    [SerializeField] private float _springDamping = 5f;

    private Tweener _scaleTweener;

    private bool _isDragging;
    private Vector3 _targetPosition;
    private Vector3 _lastPosition;

    private float _currentTiltX;
    private float _currentTiltY;
    private float _tiltVelX; // внутренние скорости для SmoothDamp
    private float _tiltVelY;

    private void Start()
    {
        SyncImageState();
    }

    private void Update()
    {
        if (_isDragging)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                _targetPosition,
                Time.deltaTime * _followSpeed
            );

            var velocity = (transform.position - _lastPosition) / Time.deltaTime;
            _lastPosition = transform.position;

            float targetTiltX = Mathf.Clamp(-velocity.y, -1, 1) * _tiltAmount;
            float targetTiltY = Mathf.Clamp(velocity.x, -1, 1) * _tiltAmount;

            // плавное приближение углов к цели
            _currentTiltX = Mathf.SmoothDampAngle(_currentTiltX, targetTiltX, ref _tiltVelX, 1f / _springDamping);
            _currentTiltY = Mathf.SmoothDampAngle(_currentTiltY, targetTiltY, ref _tiltVelY, 1f / _springDamping);
        }
        else
        {
            // если отпустили — возвращаем к нулю с пружинкой
            _currentTiltX = Mathf.SmoothDampAngle(_currentTiltX, 0, ref _tiltVelX, 1f / _springDamping);
            _currentTiltY = Mathf.SmoothDampAngle(_currentTiltY, 0, ref _tiltVelY, 1f / _springDamping);
        }

        // применяем наклон
        transform.rotation = Quaternion.Euler(_currentTiltX, _currentTiltY, 0);
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
