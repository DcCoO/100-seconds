using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VFXController : SingletonMonoBehaviour<VFXController>
{
    [SerializeField] private Transform _field;
    
    [SerializeField] private ParticleSystem _smoke;
    [SerializeField] private ParticleSystem _fieldSmoke;
    [SerializeField] private ParticleSystem _dodge;

    private enum EVFXType
    {
        Smoke,
        FieldSmoke,
        Dodge
    }

    private Dictionary<EVFXType, List<ParticleSystem>> _pool = new();

    private Transform _parent;
    
    private void OnEnable()
    {
        EventController.OnGameStart += Setup;
        EventController.OnGameWon += DestroyParent;
        EventController.OnGameLost += DestroyParent;
    }

    private void OnDisable()
    {
        EventController.OnGameStart -= Setup;
        EventController.OnGameWon -= DestroyParent;
        EventController.OnGameLost -= DestroyParent;
    }
    
    

    private void Setup()
    {
        _parent = new GameObject("VFX Parent").transform;
        _parent.SetParent(_field);
        _parent.localPosition = Vector3.zero;
        
        foreach (EVFXType vfxType in System.Enum.GetValues(typeof(EVFXType)))
        {
            _pool[vfxType] = new List<ParticleSystem>();
        }
    }
    
    private void DestroyParent()
    {
        foreach (EVFXType vfxType in System.Enum.GetValues(typeof(EVFXType)))
        {
            if (_pool.TryGetValue(vfxType, out var value))
            {
                foreach (var vfx in value)
                {
                    Destroy(vfx.gameObject);
                }
            }
        }
        _pool.Clear();
        Destroy(_parent.gameObject);
        _parent = null;
    }
    
    private void DestroyParent(int seconds, float exactTime)
    {
        DestroyParent();
    }
    
    private void PlayEffect(ParticleSystem particle, EVFXType type, Vector3 position)
    {
        if (!_parent) return;
        
        var effect = _pool[type].FirstOrDefault(p => !p.isPlaying);
        if (!effect)
        {
            effect = Instantiate(particle, position, Quaternion.identity, _parent);
            particle.Play();
            _pool[type].Add(effect);
        }
        else
        {
            effect.transform.position = position;
            effect.Play();
        }
    }

    public void Smoke(Vector3 position) => PlayEffect(_smoke, EVFXType.Smoke, position);
    
    public void FieldSmoke() => PlayEffect(_fieldSmoke, EVFXType.FieldSmoke, Vector3.zero);
    
    public void Dodge(Vector3 position) => PlayEffect(_dodge, EVFXType.Dodge, position);
    
    
}
