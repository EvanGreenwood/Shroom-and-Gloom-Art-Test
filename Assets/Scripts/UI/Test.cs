#region Usings
using UnityEngine;
using MathBad;
#endregion

public class Test : MonoBehaviour
{
    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Start()
    {
        SceneTransition.inst.Transition(() => { }, 1f,
                                        false, true, 3f,
                                        "MUSHROOM PARTY", "Bugger nothing is not short of something.");
    }
    void Update() { }
}
