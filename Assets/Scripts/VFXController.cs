using UnityEngine;

public class VFXController : SingletonMonoBehaviour<VFXController>
{
    [SerializeField] private Transform _field;
    
    [SerializeField] private GameObject _smoke;
    [SerializeField] private GameObject _fieldSmoke;

    private Transform _parent;
    private void OnEnable()
    {
        EventController.OnGameStart += CreateParent;
        EventController.OnGameWon += DestroyParent;
        EventController.OnGameLost += DestroyParent;
    }

    private void OnDisable()
    {
        EventController.OnGameStart -= CreateParent;
        EventController.OnGameWon -= DestroyParent;
        EventController.OnGameLost -= DestroyParent;
    }

    private void CreateParent()
    {
        _parent = new GameObject("VFX Parent").transform;
        _parent.SetParent(_field);
        _parent.localPosition = Vector3.zero;
    }
    
    private void DestroyParent()
    {
        Destroy(_parent.gameObject);
        _parent = null;
    }
    
    private void DestroyParent(int seconds, float exactTime)
    {
        DestroyParent();
    }

    public void Smoke(Vector3 position)
    {
        var smoke = Instantiate(_smoke, position, Quaternion.identity, _parent);
        Destroy(smoke, 2f);
    }
    
    public void FieldSmoke()
    {
        var smoke = Instantiate(_fieldSmoke, _parent);
        Destroy(smoke, 5f);
    }
}
