using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enum;
using Level;
using UnityEngine;
using View;
using Random = UnityEngine.Random;

namespace Managers
{
  public class LevelManager : MonoBehaviour
  {
    public List<GameObject> AllItems;

    [SerializeField]
    private Transform _itemPool;

    private void Awake()
    {
      LoadLevelData();

      GameManager.OnGameStarted += OnGameStarted;
    }

    private void OnGameStarted()
    {
      GetLevelObjectsReady();
    }

    private readonly List<GameObject> _levelItems = new();

    private readonly Dictionary<Item, int> _levelNecessaryItemsInitial = new();

    private readonly Dictionary<ItemKey, int> _levelNecessaryItems = new();
    private async void GetLevelObjectsReady()
    {
      _levelItems.Clear();
      _levelNecessaryItemsInitial.Clear();
      _levelNecessaryItems.Clear();

      List<int> itemIds = new();

      while (true)
      {
        int index = Random.Range(0, AllItems.Count);
        if (itemIds.Contains(index)) continue;
        
        itemIds.Add(index);
        _levelItems.Add(AllItems[index]);
        if (itemIds.Count == 5 + GameManager.Level * 2) break;
      }

      List<int> levelData = _levelVos.ElementAt(GameManager.Level).Value.NecessaryItemCount;
      List<int> counts = new() {3, 6, 6, 9, 9, 9, 12};
      
      for (int i = 0; i < _levelItems.Count; i++)
      {
        System.Random random = new();
        int index = random.Next(counts.Count);
        int count = counts[index];
      
        if (i < levelData.Count)
        {
          count = levelData[i];
          _levelNecessaryItemsInitial.Add(_levelItems[i].GetComponent<Item>(), count);
          _levelNecessaryItems.Add(_levelItems[i].GetComponent<Item>().GetItemKey(), count);
        }

        if (i == levelData.Count)
          GameManager.Instance.PanelManager.InGamePanel.SetNecessaryItemsInitial(GetLevelNecessities());

        InstantiateItem(_levelItems[i], count);

        await Task.Delay(50);
      }
    }

    private const float _lastPointX = 0.3f;
    
    private const float _lastPointZ = 0.6f;

    private void InstantiateItem(GameObject item, int count)
    {
      for (int i = 0; i < count; i++)
      {
        float limitX = Random.Range(-_lastPointX, _lastPointX);
        float limitZ = Random.Range(-_lastPointZ, _lastPointZ);
        Quaternion quaternion = new (Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360), 1);
      
        Instantiate(item, new Vector3(limitX, 2, limitZ), quaternion, _itemPool);
      }
    }

    private readonly Dictionary<int, LevelVo> _levelVos = new();
    
    private const string _dataPath = "Data/Level/Level Data";
    private void LoadLevelData()
    {
      LevelData data = Resources.Load<LevelData>(_dataPath);
      List<LevelVo> levelData = data.GetLevelList();
      
      for (int i = 0; i < levelData.Count; i++)
      {
        _levelVos.Add(i, levelData[i]);
      }
    }

    public LevelVo GetLevelVo(int level)
    {
      return _levelVos[level];
    }

    public Dictionary<Item, int> GetLevelNecessities()
    {
      return _levelNecessaryItemsInitial;
    }

    public Action<ItemKey, int> OnDataChanged;

    public void MatchedNecessaryItem(ItemKey itemKey)
    {
      if (!_levelNecessaryItems.ContainsKey(itemKey)) return;
      
      OnDataChanged?.Invoke(itemKey, 1);

      _levelNecessaryItems[itemKey]--;
      if (_levelNecessaryItems[itemKey] <= 0)
      {
        _levelNecessaryItems.Remove(itemKey);
      }
      
      if (_levelNecessaryItems.Count == 0)
      {
        GameManager.Instance.GameFinished(true);
      }
    }
  }
}