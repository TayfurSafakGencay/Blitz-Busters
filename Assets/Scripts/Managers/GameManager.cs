using System;
using UnityEngine;

namespace Managers
{
  public class GameManager : MonoBehaviour
  {
    public static GameManager Instance;
    
    public ParticleManager ParticleManager;

    public SoundManager SoundManager;

    public LevelManager LevelManager;

    public PanelManager PanelManager;

    public static int Level = 1;

    private void Awake()
    {
      Instance = this;
    }

    private void Start()
    {
      GameStarted();
    }

    public static Action OnGameStarted;
    
    public void GameStarted()
    {

      OnGameStarted?.Invoke();  
    }

    public static Action<bool> OnGameFinished;
    
    public void GameFinished(bool success)
    {
      OnGameFinished?.Invoke(success);

      if (success) Level++;
    }
  }
}