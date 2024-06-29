using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWebSpiderAgent : MonoBehaviour
{
    [SerializeField] private SpiderWebNetwork _network;
    private Transform targetTransform;
    private SpiderAgentLeg[] _legs;
    private float _legCounter = 0;
    private int _legIndex = 0;
    [SerializeField] private Rigidbody _spiderHead;
    [SerializeField] private float _uprightForce;
    [SerializeField] private Transform []  _targets;
    private int _targetIndex = 0;
    private float _targetCounter = 3;
    //
    private bool _hasTalked = false;
    [SerializeField] private Transform _zoomInAndTalkTransform;
    [SerializeField] private GameObject _speechBubble  ;
    [SerializeField] private TMPro.TextMeshPro _speechText;
    [SerializeField] private string _words1;
    [SerializeField] private string _words2;
    [SerializeField] private string _words3;
    [SerializeField] private AnimatedSprite  _faceAnimated;
    void Start()
    {
        targetTransform = _targets[0];
        _legs = GetComponentsInChildren<SpiderAgentLeg>();
        _speechBubble.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        //
        if (_targets.Length > _targetIndex)
        {
            _targetCounter += Time.deltaTime;
            if (_targetCounter > 4)
            {
                _targetCounter -= 4;
                
                targetTransform = _targets[_targetIndex];
                _targetIndex++;
            }
        }
        //
        if (targetTransform != null)
        {
            Vector3 diff = (targetTransform.position - _spiderHead.transform.position).WithZ(0);
            if (diff.magnitude > 0.5f)
            {

                //
                if (!_legs[_legIndex % _legs.Length].HasFullyAttached())
                {
                    _legCounter += Time.deltaTime * 0.5f;
                }
                else
                {
                    _legCounter += Time.deltaTime;
                }
                //
                if (_legCounter > 0.2f)
                {
                    _legCounter -= 0.2f;

                    _legs[_legIndex % _legs.Length].TryFindNewNode(_network, _spiderHead.transform, diff.normalized);
                    _legIndex++;
                }
            }
            else
            {
                if (!_hasTalked && _targetIndex >= _targets.Length)
                {
                    _hasTalked = true;
                    StartCoroutine(TalkRoutine());
                }
            }
        }
    }
    private IEnumerator TalkRoutine()
    {
        ZoomCamera.Instance.ZoomIn(_zoomInAndTalkTransform);
   yield return new WaitForSeconds(1.3f);
        _speechBubble.SetActive(true);
        _speechText.text = "";
       yield return new WaitForSeconds(0.3f);
        _speechText.text = _words1;
        _faceAnimated.enabled = true;
        yield return new WaitForSeconds(1.3f);
        _faceAnimated.enabled = false;
        yield return new WaitForSeconds(1f);
        _speechBubble.SetActive(false);
        _speechText.text = "";
        yield return new WaitForSeconds(0.3f);
        _speechBubble.SetActive(true);
        _faceAnimated.enabled = true;
        _speechText.text = _words2;
        yield return new WaitForSeconds(1.3f);
        _faceAnimated.enabled = false;
        yield return new WaitForSeconds(1f);
        _speechBubble.SetActive(false);
        _speechText.text = "";
        _zoomInAndTalkTransform.Rotate(0, 0, 15);
        yield return new WaitForSeconds(0.3f);
        _faceAnimated.enabled = true;
        _speechBubble.SetActive(true);
        _speechText.text = _words3;
        _speechText.fontSize = _speechText.fontSize * 1.23f;
        yield return new WaitForSeconds(1.3f);
        _faceAnimated.enabled = false;
        yield return new WaitForSeconds(1f);
        _speechBubble.SetActive(false);
        ZoomCamera.Instance.Release();
    }
    private void FixedUpdate()
    {
        _spiderHead.AddForceAtPosition(Vector3.up * _uprightForce * Time.deltaTime, _spiderHead.transform.TransformPoint(Vector3.up), ForceMode.VelocityChange);
        _spiderHead.AddForceAtPosition(Vector3.down * _uprightForce * Time.deltaTime, _spiderHead.transform.TransformPoint(Vector3.down), ForceMode.VelocityChange);
    }
}
