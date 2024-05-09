//  __  __                  ___  _              _       _                                             
// |  \/  | ___  _ _   ___ / __|(_) _ _   __ _ | | ___ | |_  ___  _ _                                 
// | |\/| |/ _ \| ' \ / _ \\__ \| || ' \ / _` || |/ -_)|  _|/ _ \| ' \                                
// |_|  |_|\___/|_||_|\___/|___/|_||_||_|\__, ||_|\___| \__|\___/|_||_|                               
//                                       |___/                                                        
//----------------------------------------------------------------------------------------------------

#region Usings
using UnityEngine;
#endregion

namespace Mainframe
{
public abstract class MonoSingleton<T> : MonoBehaviour, ISingelton where T : MonoBehaviour
{
  [SerializeField] bool _dntDestroyOnLoad = false;
  public bool dntDestroyOnLoad => _dntDestroyOnLoad;

  static string _typeID = typeof(T).Name;
  static string _logID = $"{_typeID} Singleton";

  static readonly object _lock = new object();

  // instance
  //----------------------------------------------------------------------------------------------------
  static T _inst;
  public static T inst
  {
    get
    {
      lock(_lock)
      {
        if(_inst == null)
          GetInstance();
        return _inst;
      }
    }
  }

  // Get Instance
  //----------------------------------------------------------------------------------------------------
  static void GetInstance()
  {
    T[] instances = FindObjectsOfType<T>(true);

    if(instances.Length > 1)
    {
      Debug.LogWarning($"{_logID.Yellow().Bold()}: Found {instances.Length} instances of {_typeID} in the scene.");
      for(int i = instances.Length - 1; i >= 0; i--)
      {
        Destroy(instances[i].gameObject);
      }
    }

    _inst = instances.Length > 0 ? instances[0] : null;

    if(_inst == null)
    {
      string resourcesPath = "Singleton";
      T[] prefabs = Resources.LoadAll<T>(resourcesPath);
      if(prefabs.IsNullOrEmpty())
      {
        Debug.LogError($"{_logID.Red().Bold()}: An instance of {_typeID} doesn't exist in the scene and\n" +
                       $"a reference was not located in \"Resources/{resourcesPath}.\"");
      }
      else
      {
        T prefab = prefabs[0];
        // Debug.Log($"{_logID.Grey().Bold()}: Self instantiating.");
        _inst = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        _inst.name = prefab.name;
      }
    }

    if(_inst != null)
    {
      // Cast to access local properties if _inst
      MonoSingleton<T> inst = _inst as MonoSingleton<T>;
      if(inst != null)
      {
        if(inst.dntDestroyOnLoad)
        {
          DontDestroyOnLoad(_inst);
        }
        inst.OnSingletonFirstLoad();
      }
    }
  }

  // Peek - Force get instance
  //----------------------------------------------------------------------------------------------------
  public void Peek() { }

  // The first time instance was accessed 
  //----------------------------------------------------------------------------------------------------
  protected virtual void OnSingletonFirstLoad() { }
}
}
