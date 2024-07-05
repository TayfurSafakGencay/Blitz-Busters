using System;
using System.Threading.Tasks;
using Enum;
using Managers;
using UnityEngine;

namespace View
{
  public class Item : MonoBehaviour
  {
    [SerializeField]
    private ItemKey _itemKey;

    [SerializeField]
    private Sprite _sprite;

    private bool _clicked;

    private void Awake()
    {
      GameManager.OnGameStarted += OnGameStarted;
    }

    private void OnGameStarted()
    {
      Destroy(gameObject);
    }

    private void Start()
    {
      if (_itemKey == ItemKey.Empty)
      {
        Debug.LogError("There is an empty key!");
      }
    }

    public void Clicked()
    {
      if (_clicked) return;
      _clicked = true;
      
      GameManager.Instance.SoundManager.PlaySound(SoundKey.Click);

      Destroy(gameObject.GetComponent<MeshCollider>());
      Destroy(gameObject.GetComponent<Rigidbody>());
      
      GameManager.Instance.Collector.ItemCollected(this);
    }

    public ItemKey GetItemKey()
    {
      return _itemKey;
    }

    public Sprite GetSprite()
    {
      return _sprite;
    }
  }
}