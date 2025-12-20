using UnityEngine;

public class GarageDoor : MonoBehaviour
{
    [SerializeField] private ToggleableButton button;
    [SerializeField] private AudioSource source;
    
    private bool _updateGarageDoor = false;
    private Transform _initialTransform;
    private Vector3 _newPosition;
    private Vector3 _amountToTranslate = new Vector3(0, 1.5f, 0);
    private float _speed = 1.0f;
    private bool _sfxPlayed = false;
    
    private void Start()
    {
        _initialTransform = transform;
        _newPosition = _initialTransform.position + _amountToTranslate;
        button.OnButtonPressed += () => { _updateGarageDoor = true; };
    }

    private void Update()
    {
        if (_updateGarageDoor)
        {
            if (!_sfxPlayed)
            {
                AudioManager.Instance.PlaySFX(SfxType.GarageDoorOpening);
                _sfxPlayed = true;
            }
            transform.position = Vector3.Lerp(_initialTransform.position, _newPosition, Time.deltaTime * _speed);
            if (transform.position == _newPosition) _updateGarageDoor = false;
        }
    }
}
