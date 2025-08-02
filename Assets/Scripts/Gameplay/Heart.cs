using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] GameObject _fullHeart;

    public void SetFull()
    {
        _fullHeart.SetActive(true);
    }
    
    public void SetEmpty()
    {
        _fullHeart.SetActive(false);
    }
}
