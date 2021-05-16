using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayableDirectorHelper : MonoBehaviour
{
    public Animator m_PreviewRole;

    [Header("特殊：金蛇剑")]
    public GameObject m_JinSheJian;
    public string m_HandBoneName;

    private void Start()
    {
        m_PreviewRole.gameObject.SetActive(false);
    }

    public void BindClonePlayer(Animator clonePlayer)
    {
        clonePlayer.transform.position = m_PreviewRole.transform.position;
        clonePlayer.transform.rotation = m_PreviewRole.transform.rotation;
        clonePlayer.gameObject.SetActive(true);
        BindSignals(clonePlayer.gameObject);
    }

    private void BindSignals(GameObject clonePlayer)
    {
        var signalReceiver = GetComponent<SignalReceiver>();
        if(m_JinSheJian != null)
        {
            m_JinSheJian.gameObject.SetActive(false);
            var jinSheJian = Instantiate(m_JinSheJian);
            jinSheJian.gameObject.SetActive(true);

            var signalReaction = signalReceiver.GetReactionAtIndex(0);
            signalReaction.RemoveAllListeners();
            signalReaction.AddListener(delegate
            {
                var handBoneTransform = FindTransform(clonePlayer.transform, m_HandBoneName);
                if(handBoneTransform != null)
                {
                  
                    jinSheJian.transform.SetParent(handBoneTransform);
                }
            });
        }
    }

    private Transform FindTransform(Transform parent, string name)
    {
        Transform rst = null;
        foreach (Transform tf in parent)
        {
            Debug.Log(tf.name);
            if (tf.name == name)
            {
                return tf;
            }
            rst = FindTransform(tf, name);
        }
        return rst;
    }
}
