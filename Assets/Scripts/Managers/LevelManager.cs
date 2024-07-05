using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Level;
using UnityEngine;
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
    private async void GetLevelObjectsReady()
    {
      _levelItems.Clear();

      List<int> itemIds = new();

      while (true)
      {
        int index = Random.Range(0, AllItems.Count);
        if (itemIds.Contains(index)) continue;
        
        itemIds.Add(index);
        _levelItems.Add(AllItems[index]);
        if (itemIds.Count == 15) break;
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
        }
        
        InstantiateItem(_levelItems[i], count);

        await Task.Delay(50);
      }
    }

    private const float _lastPointX = 0.4f;
    
    private const float _lastPointZ = 0.75f;

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

    private Dictionary<int, LevelVo> _levelVos = new();
    
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
  }
}