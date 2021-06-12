using System;
using Jyx2;
using Jyx2.Middleware;
using HSFrameWork.ConfigTable;
using HSFrameWork.SPojo;
using UnityEngine;
using UnityEngine.UI;

public class BagPanel : MonoBehaviour
{
    public Button m_BackButton;

    public Transform m_Container;
    public Text m_InfoText;
    public Button m_UseButton;

    Action<int> callback;

    /// <summary>
    /// 创建背包面板
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="items"></param>
    /// <param name="selectCallback">如果取消，则返回-1</param>
    public static void Create(Transform parent, SaveableNumberDictionary<int> items, Action<int> selectCallback = null, Func<Jyx2Item, bool> filter = null)
    {
        var prefab = Jyx2ResourceHelper.GetCachedPrefab("Assets/Prefabs/BagPanel.prefab");
        var obj = Instantiate(prefab);
        obj.transform.SetParent(parent);

        var rt = obj.GetComponent<RectTransform>();
        rt.localPosition = Vector3.zero;
        rt.localScale = Vector3.one;

        var bagPanel = obj.GetComponent<BagPanel>();
        bagPanel.Show(items, selectCallback, filter);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_BackButton.onClick.RemoveAllListeners();
        m_BackButton.onClick.AddListener(() =>
        {
            Destroy(this.gameObject);
            if (callback != null)
                callback(-1);
        });

        m_UseButton.onClick.RemoveAllListeners();
        m_UseButton.onClick.AddListener(() => {
            Destroy(this.gameObject);
            if (callback != null)
            {
                if (currentItem != null)
                {
                    callback(int.Parse(currentItem.Id));
                }   
                else
                {
                    callback(-1);
                }
            } 
        });

        m_InfoText.text = "";
    }

    private Jyx2Item currentItem = null;

    public void Show(SaveableNumberDictionary<int> items, Action<int> selectCallback = null, Func<Jyx2Item, bool> filter = null)
    {
        HSUnityTools.DestroyChildren(m_Container);
        callback = selectCallback;
        foreach(var kv in items)
        {
            string id = kv.Key;
            int count = kv.Value;

            var item = ConfigTable.Get<Jyx2Item>(id);
            if(item == null)
            {
                Debug.LogError("调用了错误的物品，id=" + id);
                continue;
            }

            //过滤器逻辑
            if (filter != null && filter(item) == false)
                continue;

            var itemUI = Jyx2ItemUI.Create(int.Parse(id), count);
            itemUI.transform.SetParent(m_Container);
            var btn = itemUI.GetComponent<Button>();

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => {
                SelectItem(itemUI);
            });
        }
    }

    void SelectItem(Jyx2ItemUI itemUI)
    {
        var item = itemUI.GetItem();
        m_InfoText.text = item.Name + "\n\n" + item.Desc;
        currentItem = item;
    }
}
