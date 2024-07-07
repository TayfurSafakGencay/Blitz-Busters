using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Enum;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using View;
using Vo;

namespace UI.InGame
{
  public class Collector : MonoBehaviour
  {
    private readonly Dictionary<Transform, ItemVo> _slots = new();

    [SerializeField]
    private Transform _slotsParent;

    [SerializeField]
    private Transform _itemPool;

    [SerializeField]
    private Camera _camera;

    private readonly Queue<Image> _itemImageQueue = new();

    private void Awake()
    {
      for (int i = 0; i < _slotsParent.childCount; i++)
      {
        _slots.Add(_slotsParent.GetChild(i).GetComponent<Transform>(), new ItemVo
        {
          Object = null,
          Key = ItemKey.Empty
        });
      }

      for (int i = 0; i < _itemPool.childCount; i++)
      {
        GameObject item = _itemPool.GetChild(i).gameObject;
        _itemImageQueue.Enqueue(item.GetComponent<Image>());
        item.SetActive(false);
      }
    }


    private const float _newScale = 1750;
    public async void ItemCollected(Item itemScript)
    {
      itemScript.gameObject.layer = 5;
      itemScript.transform.parent = _itemPool;
      itemScript.transform.localScale = new Vector3(_newScale, _newScale, _newScale);
      itemScript.transform.DORotateQuaternion(new Quaternion(0, 0, 0, 0), 0.5f);

      WorldPositionToUI(itemScript.transform.position, itemScript.gameObject);

      GameManager.Instance.LevelManager.MatchedNecessaryItem(itemScript.GetItemKey());

      await CheckSameKey(itemScript);

      CheckLosingConditions();
    }
    
    private async Task CheckSameKey(Item itemScript)
    {
      ItemKey itemKey = itemScript.GetItemKey();
      
      for (int i = 0; i < _slots.Count; i++)
      {
        if (_slots.ElementAt(i).Value.Key == itemKey)
        {
          if (_slots.ElementAt(i + 1).Value.Key == ItemKey.Empty)
          { 
            AddItemToEmptySlot(i + 1, itemScript);
            return;
          }
          
          if (_slots.ElementAt(i + 1).Value.Key == itemKey)
          {
            Task task = MoveToRight(i + 2, itemScript);
            await task;
            CheckThreeMatching(i);
            return;
          }
          else
          {
            Task task = MoveToRight(i + 1, itemScript);
            await task;
            
            return;
          }
        }

        if (_slots.ElementAt(i).Value.Key != ItemKey.Empty) continue;
        
        AddItemToEmptySlot(i, itemScript);
        return;
      }
    }

    private void AddItemToEmptySlot(int index, Item itemScript)
    {
      Transform slotTransform = _slots.ElementAt(index).Key;
      
      InitialItemAnimation(itemScript, slotTransform);
    }
    
    private Task MoveToRight(int index, Item itemScript)
    {
      ItemVo itemVo = new()
      {
        Key = itemScript.GetItemKey(),
        Object = itemScript.gameObject
      };
      
      for (int i = _slots.Count - 1; i >= 0; i--)
      {
        if (i < index) break;

        KeyValuePair<Transform, ItemVo> itemPair = _slots.ElementAt(i);

        if (itemPair.Value.Key == ItemKey.Empty) continue;
        if (itemPair.Value.Key == itemVo.Key) continue;

        Transform key = _slots.ElementAt(i + 1).Key;
        ItemAnimation(key, itemPair.Value);
      }

      Transform targetKey = _slots.ElementAt(index).Key;
      InitialItemAnimation(itemScript, targetKey);
      
      return Task.Delay(0);
    }
    
    private async void CheckThreeMatching(int index)
    {
      for (int i = index; i < index + 3; i++)
      {
        KeyValuePair<Transform, ItemVo> item = _slots.ElementAt(i);
        
        EnqueueItem(item.Value.Object);
        ItemVo itemVo = new()
        {
          Key = ItemKey.Empty,
          Object = null
        };
        _slots[item.Key] = itemVo;
      }
      
      await Task.Delay((int)(_moveTime * 1000));
      
      GameManager.Instance.SoundManager.PlaySound(SoundKey.Match);
      
      MoveToLeft(index, 3);
    }

    private void MoveToLeft(int index, int moveAmount)
    {
      for (int i = index + moveAmount; i < _slots.Count; i++)
      {
        if (_slots.ElementAt(i).Value.Key == ItemKey.Empty) continue;
        
        KeyValuePair<Transform, ItemVo> newSlot = _slots.ElementAt(i - moveAmount);
        KeyValuePair<Transform, ItemVo> oldSlot = _slots.ElementAt(i);
      
        ItemAnimation(newSlot.Key, oldSlot.Value);
      
        ItemVo itemVo = new()
        {
          Object = null,
          Key = ItemKey.Empty
        };
        _slots[oldSlot.Key] = itemVo;
      }
    }

    private const float _moveTime = 0.7f;
    
    private void InitialItemAnimation(Item item, Transform slotTransform)
    {
      Image itemImage = _itemImageQueue.Dequeue();
      itemImage.gameObject.SetActive(false);
      itemImage.color = new Color(1, 1, 1, 1);

      ItemVo itemVo = new()
      {
        Object = itemImage.gameObject,
        Key = item.GetItemKey()
      };

      _slots[slotTransform] = itemVo;
      
      item.transform.DOMove(slotTransform.position, _moveTime).SetEase(Ease.InBack).OnComplete(() =>
      {
        itemImage.transform.position = slotTransform.position;
        itemImage.gameObject.SetActive(true);
        itemImage.sprite = item.GetSprite();
        itemImage.name = item.GetItemKey().ToString();
        
        Destroy(item.gameObject);
      });
    }

    private void ItemAnimation(Transform itemTransform, ItemVo itemVo)
    {
      _slots[itemTransform] = itemVo;
      itemVo.Object.transform.DOMove(itemTransform.position, _moveTime).SetEase(Ease.InOutQuart);
    }

    private async void EnqueueItem(GameObject item)
    {
      item.TryGetComponent(out Image itemImage);
      await Task.Delay((int)(_moveTime * 1000));
        
      GameManager.Instance.ParticleManager.PlayParticleEffectFromPool(item.transform.position, VFX.Match);
      
      itemImage.DOFade(0, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
      {
        item.transform.position = Vector3.zero;
        item.gameObject.SetActive(false);
        _itemImageQueue.Enqueue(itemImage);
      });
    }

    private void CheckLosingConditions()
    {
      for (int i = 0; i < _slots.Count; i++)
      {
        if (_slots.ElementAt(i).Value.Key == ItemKey.Empty) return;
      }
      
      GameManager.Instance.GameFinished(false);
    }

    [SerializeField]
    private Canvas _canvas;

    private void WorldPositionToUI(Vector3 position, GameObject item)
    {
      item.AddComponent<RectTransform>();
      Vector3 screenPos = _camera.WorldToScreenPoint(position);

      RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, screenPos, _canvas.worldCamera, out Vector2 uiPos);
      item.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(uiPos.x, uiPos.y, 0);
      // item.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 0, 0);
    }
  }
}