using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayableDirectorHelper : MonoBehaviour
{
    public Animator m_PreviewRole;

    [Header("绑定物体到指定节点上")] public GameObject m_BindObject;
    public string m_BindBoneName;
    public bool m_IsLocalTransform;

    private readonly List<GameObject> _tempObjs = new List<GameObject>();

    private void Start()
    {
        m_PreviewRole?.gameObject.SetActive(false);
    }

    public void BindPlayer(GameObject player)
    {
        if (m_PreviewRole != null)
        {
            player.transform.position = m_PreviewRole.transform.position;
            player.transform.rotation = m_PreviewRole.transform.rotation;
        }

        player.SetActive(true);
        BindSignals(player);
    }

    private void BindSignals(GameObject player)
    {
        var signalReceiver = GetComponent<SignalReceiver>();
        if (m_BindObject != null && signalReceiver != null)
        {
            m_BindObject.gameObject.SetActive(false);
            var bindObject = Instantiate(m_BindObject);
            _tempObjs.Add(bindObject);
            bindObject.gameObject.SetActive(true);

            var signalReaction = signalReceiver.GetReactionAtIndex(0);
            signalReaction.RemoveAllListeners();
            signalReaction.AddListener(delegate
            {
                var bindBoneTransform = FindChild(player.transform, m_BindBoneName);
                if (bindBoneTransform != null)
                {
                    bindObject.transform.SetParent(bindBoneTransform);
                    if (m_IsLocalTransform)
                    {
                        bindObject.transform.localPosition = m_BindObject.transform.localPosition;
                        bindObject.transform.localRotation = m_BindObject.transform.localRotation;
                    }
                    else
                    {
                        bindObject.transform.position = m_BindObject.transform.position;
                        bindObject.transform.rotation = m_BindObject.transform.rotation;
                    }
                }
            });
        }
    }

    Transform FindChild(Transform parent, string name)
    {
        Transform tf = parent.Find(name);
        if (tf != null)
        {
            return tf;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform target = FindChild(parent.GetChild(i), name);
            if (target != null)
            {
                return target;
            }
        }

        return null;
    }

    public void ClearTempObjects()
    {
        _tempObjs.ForEach(delegate(GameObject go) { GameObject.Destroy(go); });
        _tempObjs.Clear();
    }
}
