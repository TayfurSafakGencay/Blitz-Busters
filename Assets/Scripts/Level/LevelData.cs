using System;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
  [CreateAssetMenu(fileName = "Level Data", menuName = "Create Level Data", order = 0)]
  public class LevelData : ScriptableObject
  {
    [SerializeField]
    private List<LevelVo> LevelList;

    public List<LevelVo> GetLevelList()
    {
      return LevelList;
    }
  }

  [Serializable]
  public struct LevelVo
  {
    public List<int> NecessaryItemCount;
  }
}