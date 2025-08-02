using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour {
    [SerializeField] RawImage _image;
    [SerializeField] float _x, _y;

    Rect _initialRect;

    void Awake()
    {
        _initialRect = new Rect(_image.uvRect.position, _image. uvRect.size);
    }

    void OnEnable()
    {
        _image.uvRect = _initialRect;
    }
    
    void Update()
    {
        _image.uvRect = new Rect(_image.uvRect.position + new Vector2(_x,_y) * Time.deltaTime, _image. uvRect.size);
    }
}
