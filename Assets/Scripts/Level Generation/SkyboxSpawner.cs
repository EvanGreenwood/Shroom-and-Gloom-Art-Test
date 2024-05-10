#region Usings
using Framework;
using UnityEngine;
using MathBad;
#endregion

public class SkyboxSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] _prefabs;
    [SerializeField] int _spawnCount = 100;
    [SerializeField] FloatRange _scaleFactorX = new FloatRange(1f, 2f);
    [SerializeField] FloatRange _scaleFactorY = new FloatRange(1f, 2f);
    [SerializeField] Bounds _spawnArea;

    void Awake()
    {
        for(int i = 0; i < _spawnCount; i++)
        {
            Vector3 spawnPos = RNG.Vector3(_spawnArea.min, _spawnArea.max);
            GameObject spawned = Instantiate(_prefabs.ChooseRandom(), spawnPos, Quaternion.identity);
            Vector3 factor = new Vector3(_scaleFactorX.ChooseRandom(), _scaleFactorY.ChooseRandom(), 1f);
            spawned.transform.localScale = Vector3.Scale(spawned.transform.localScale, factor);
        }
    }
    void OnDrawGizmos()
    {
        GIZMOS.WireBoxMinMax(_spawnArea.min, _spawnArea.max, RGB.yellow.WithA(0.25f));
    }
}
