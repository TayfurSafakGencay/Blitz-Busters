using System;
using UI.InGame;
using UnityEngine;

namespace Managers
{
  public class GameManager : MonoBehaviour
  {
    public static GameManager Instance;
    
    public Collector Collector;

    public ParticleManager ParticleManager;

    public SoundManager SoundManager;

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
    
    public static void GameStarted()
    {
      OnGameStarted?.Invoke();  
    }

    public static Action<bool> OnGameFinished;
    
    public static void GameFinished(bool success)
    {
      OnGameFinished?.Invoke(success);
    }
  }
}