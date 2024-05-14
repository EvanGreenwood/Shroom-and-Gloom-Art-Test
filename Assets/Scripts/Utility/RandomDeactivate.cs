using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDeactivate : MonoBehaviour
{
    [SerializeField] private float _deactivateChance = 0.5f;
    void Start()
    {
        if (Random.value < _deactivateChance)
        {
            gameObject.SetActive(false);
        }
    }
}
