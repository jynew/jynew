/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using System.Collections.Generic;
using HanSquirrel.ResourceManager;
using Jyx2;
using HSFrameWork.Common;
using SkillEffect;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Random = UnityEngine.Random;
using Lean.Pool;
using System.Linq;
using System.Runtime.CompilerServices;
using Animancer;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
using UnityEngine.AddressableAssets;

public class MapRole : Jyx2AnimationBattleRole
{
    //数据实例
    public RoleInstance DataInstance
    {
        get
        {
            if (string.IsNullOrEmpty(m_RoleKey))
                return null;

            if (_dataInstance == null) this.CreateRoleInstance(m_RoleKey);
            return _dataInstance;
        }
        set
        {
            _dataInstance = value;
        }
    }
    private RoleInstance _dataInstance;

    public string m_RoleKey;

    public string[] m_NpcWords;

    //在非战斗模式下拿武器
    public bool m_HasWeaponNotInBattle = false;
    
    //是否关键角色（战斗中晕眩，战斗后恢复）
    public bool m_IsKeyRole = false; //主角+队友+面板打勾的

    [HideInInspector]
    public bool IsPlayingMovingAnimation = false; //跑步动作切换的标记

    [HideInInspector]
    public bool m_IsWaitingForActive = true; //GameObject Active时触发的标记

    [HideInInspector]
    public bool IsInBattle = false; //是否在战斗中

    public bool HPBarIsDirty { private set; get; } = false;//通知需要刷新Hud血条

    // private CustomOutlooking _outLooking;

    public override Animator GetAnimator()
    {
        if (_animator == null && transform.childCount == 0)
            return null;

        if (_animator == null)
        {
            for (var index = 0; index < transform.childCount; index++)
            {
                _animator = transform.GetChild(index).GetComponentInChildren<Animator>();
                if (_animator != null)
                {
                    break;
                }
            }
        }

        if (_lazyInitBattleAnimator)
        {
            if (_navMeshAgent != null) _navMeshAgent.enabled = false;

            if (_animator != null)
            {
                _animator.SetBool("IsInBattle", true);
                _animator.applyRootMotion = false; //不接受动作变化位置
                _animator.SetFloat("speed", 0);
            }

            _lazyInitBattleAnimator = false;
        }
        
        return _animator;
    }

    private Animator _animator;
    public void ForceSetAnimator(Animator a)
    {
        _animator = a;
    }

    //健康状态
    private MapRoleHealth m_Health = MapRoleHealth.Normal;

    private float _distFromPlayer;
    private CompositeDisposable _behaviourDisposable = new CompositeDisposable();

    //说话
    public void Say(string word, float time = 5f)
    {
        var hudRoot = StoryEngine.Instance.HUDRoot;

        HUDTextInfo info = new HUDTextInfo(transform, word)
        {
            Size = 12,
            Color = Color.white,
            VerticalPositionOffset = 0.5f,
            ExtraDelayTime = time
        };
        hudRoot.NewText(info);
    }

    //npc闲聊
    public void DoNpcChat()
    {
        if (m_NpcWords != null && m_NpcWords.Length > 0)
        {
            string randomWord = m_NpcWords[UnityEngine.Random.Range(0, m_NpcWords.Length)];
            HUDTextInfo info4 = new HUDTextInfo(transform, randomWord)
            {
                Color = Color.white,
                Size = 15,
                VerticalPositionOffset = 1f,
                VerticalFactorScale = UnityEngine.Random.Range(1.2f, 3),
                Side = bl_Guidance.Right,
                FadeSpeed = 500,
                ExtraDelayTime = 5,
                AnimationType = bl_HUDText.TextAnimationType.HorizontalSmall
            };
            //Send the information
            var hudRoot = StoryEngine.Instance.HUDRoot;
            hudRoot.NewText(info4);
        }

        transform.LookAt(GameRuntimeData.Instance.Player.View.transform);
    }

    //用于显示掉血效果
    private int _showDamage;
    public void SetDamage(int damage, int hp)
    {
        _showDamage = damage;
        //var msg = $"{m_RoleKey}被攻击，掉了{damage}点血，还有{hp}点血";
        //Debug.Log(msg);
    }

    public override void ShowDamage()
    {
        //JYX2逻辑，不存在MISS
        if (_showDamage <= 0)
            return;

        if (StoryEngine.Instance == null) return;

        var hudRoot = StoryEngine.Instance.HUDRoot;
        string damageText = "";
        if (_showDamage > 0)
            damageText = $"-{Math.Max(_showDamage, 1)}";
        else
            damageText = "MISS";
        HUDTextInfo info = new HUDTextInfo(transform, $"{damageText}")
        {
            Color = Color.white,
            //Size = Random.Range(1, 12),
            Speed = Random.Range(0.2f, 1),
            VerticalAceleration = Random.Range(-2, 2f),
            VerticalPositionOffset = Random.Range(0, 0.8f),
            VerticalFactorScale = Random.Range(1.2f, 10),
            Side = (Random.Range(0, 2) == 1) ? bl_Guidance.LeftDown : bl_Guidance.RightDown,
            ExtraDelayTime = 0.2f,
            AnimationType = bl_HUDText.TextAnimationType.HorizontalSmall,
            FadeSpeed = 200,
            ExtraFloatSpeed = -11
        };

        info.TextPrefab = Jyx2ResourceHelper.GetCachedPrefab("Assets/Prefabs/Jyx2/AttackInfoText.prefab");
        hudRoot.NewText(info);
    }

    //血条标记为需要刷新
    public override void MarkHpBarIsDirty()
    {
        HPBarIsDirty = true;
    }

    //取消刷新血条标记
    public override void UnmarkHpBarIsDirty()
    {
        HPBarIsDirty = false;
    }

    public void ShowBattleText(string mainText,Color textColor) 
    {
        if (StoryEngine.Instance == null) return;

        var hudRoot = StoryEngine.Instance.HUDRoot;
        HUDTextInfo info = new HUDTextInfo(transform, $"{mainText}")
        {
            Color = Color.white,
            Speed = Random.Range(0.2f, 1),
            VerticalAceleration = Random.Range(-2, 2f),
            VerticalPositionOffset = Random.Range(0, 0.8f),
            VerticalFactorScale = Random.Range(1.2f, 10),
            Side = (Random.Range(0, 2) == 1) ? bl_Guidance.LeftDown : bl_Guidance.RightDown,
            ExtraDelayTime = 0.2f,
            AnimationType = bl_HUDText.TextAnimationType.HorizontalSmall,
            FadeSpeed = 200,
            ExtraFloatSpeed = -11
        };
        info.Color = textColor;

        info.TextPrefab = Jyx2ResourceHelper.GetCachedPrefab("Assets/Prefabs/Jyx2/AttackInfoText.prefab");
        hudRoot.NewText(info);
    }

    public void ShowAttackInfo(string content)
    {
        var hudRoot = StoryEngine.Instance.HUDRoot;

        HUDTextInfo info = new HUDTextInfo(transform, content)
        {
            Color = Color.white,
            //Size = Random.Range(1, 12),
            Speed = Random.Range(0.2f, 1),
            VerticalAceleration = Random.Range(-2, 2f),
            VerticalPositionOffset = Random.Range(0, 0.8f),
            VerticalFactorScale = Random.Range(1.2f, 10),
            Side = (Random.Range(0, 2) == 1) ? bl_Guidance.LeftDown : bl_Guidance.RightDown,
            ExtraDelayTime = 0.2f,
            AnimationType = bl_HUDText.TextAnimationType.HorizontalSmall,
            FadeSpeed = 200,
            ExtraFloatSpeed = -11
        };

        info.TextPrefab = Jyx2ResourceHelper.GetCachedPrefab("Assets/Prefabs/Jyx2/AttackInfoText.prefab");
        hudRoot.NewText(info);
    }

    #region 战斗原型相关
    //播放动画指令
    public void PlayAnimationCmd(string cmd)
    {
        foreach (var c in cmd.Split(','))
        {
            string para = c.Split(':')[0];
            string value = c.Split(':')[1];
            if (para == "trigger")
            {
                GetAnimator().SetTrigger(value);
            }
            else
            {
                if (para.StartsWith("f"))
                {
                    GetAnimator().SetFloat(para, float.Parse(value));
                }
                else
                {
                    GetAnimator().SetInteger(para, int.Parse(value));
                }
            }

            if (para == "WeaponType")
            {
                GetAnimator().SetInteger(para, int.Parse(value));
            }
        }
    }

    private bool _lazyInitBattleAnimator = true;
    
    public void LazyInitAnimator()
    {
        _lazyInitBattleAnimator = true;
    }

    //切换武学
    public void SwitchSkillTo(WugongInstance skill)
    {
        var display = skill.GetDisplay();
        //切换对应武器
        var weaponCode = display.weaponCode;
        if (weaponCode >= 0)
        {
            ChangeWeapon(weaponCode);
        }

        this.CurDisplay = display;
        
        //载入动作
        ChangeAnimationController(display.GetAnimationController(), Idle);
    }

    void ChangeWeapon(ModelAsset.WeaponPartType weaponCode)
    {
        DOMountWeapon(weaponCode);
    }

    void ChangeAnimationController(RuntimeAnimatorController controller, Action callback)
    {
        if (controller == null)
            return;

        var anim = GetAnimator();
        if (anim == null)
            return;

        //当前已经是该Controller了，不需要再修改
        if (controller == anim.runtimeAnimatorController)
        {
            callback();
            return;
        }
        
        anim.runtimeAnimatorController = controller;
        callback();
    }


    //朝向一个方向
    public void LookAtBattleBlock(BattleBlockData pos)
    {
        //平视，所以y轴和自身一致
        transform.LookAt(new Vector3(pos.WorldPos.x, transform.position.y, pos.WorldPos.z));
    }

    public void LookAtWorldPosInBattle(Vector3 position)
    {
        //平视，所以y轴和自身一致
        transform.LookAt(new Vector3(position.x, transform.position.y, position.z));
    }

    public void SetPosition(Vector3 pos) 
    {
        transform.position = pos;
    }

    public void ShowDeath(int deathCode = -1)
    {
        var globalConfig = GlobalAssetConfig.Instance;
        
        //人型骨骼，播放死亡动作
        if (this._animator.runtimeAnimatorController == globalConfig.defaultAnimatorController)
        {
            var clip = Hanjiasongshu.Tools.GetRandomElement(globalConfig.defaultDieClips);
            PlayAnimation(clip, () => { Destroy(gameObject); });
        }
        else
        {
            Destroy(gameObject);  
        }
        
        
        //简单粗暴，直接删除GameObject，回头有空再实现死亡动作
        //GameObject.Destroy(this.gameObject);

        //默认播放随机死亡动作
        //if (deathCode == -1) deathCode = ToolsShared.GetRandomInt(0, 2);
        //GetAnimator().SetTrigger("Death");
        //GetAnimator().SetInteger("DeathType", (int)deathCode);
        m_Health = MapRoleHealth.Death;
    }

    public void ShowStun()
    {
        GetAnimator().SetTrigger("Stun");
        _navMeshAgent.enabled = false;
        if (m_Health == MapRoleHealth.Normal) m_Health = MapRoleHealth.Stun;
    }

    public void StopStun(bool isInBattle)
    {
        GetAnimator().ResetTrigger("Stun");
        if (isInBattle)
        {
            _navMeshAgent.enabled = false;
            GetAnimator().SetTrigger("Idle");
        }
        else
        {
            _navMeshAgent.enabled = true;
            GetAnimator().SetTrigger("LeaveBattle");
        }
        if (m_Health != MapRoleHealth.Death) m_Health = MapRoleHealth.Stun;
    }
    #endregion

    private void Awake()
    {
        // _outLooking = GetComponent<CustomOutlooking>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private async void Start()
    {
        await BeforeSceneLoad.loadFinishTask;
        
        if (string.IsNullOrEmpty(m_RoleKey))
            return;

        //场景没有LevelMaster
        if (LevelMaster.Instance == null && DataInstance == null) this.CreateRoleInstance(m_RoleKey);
        
        //脚步声
        //Observable.EveryFixedUpdate()
        //    .Where(_ => _navMeshAgent != null)
        //    .Subscribe(xs =>
        //    {
        //        PlayFootStepSoundEffect(_distFromPlayer);
        //    });
        
        if (!m_IsWaitingForActive && m_RoleKey != "testman" && m_RoleKey != "主角")
        {
            m_IsWaitingForActive = false;
            await RefreshModel();
        }
    }

    //队友寻路
    private NavMeshAgent _navMeshAgent;

    #region 脚步声相关

    //播放脚步声，TODO:根据所处地貌不同改为不同的脚步声
    void PlayFootStepSoundEffect(float dist)
    {
        //屏蔽远处敌人脚步声
        if (dist > 16) return;

        if (!_initWalkSoundEffect)
        {
            _initWalkSoundEffect = true;
            
            Jyx2ResourceHelper.LoadAsset<AudioClip>("Assets/BuildSource/SoundEffect/walk_on_grass.mp3", audio=> {
                walkSoundEffect = audio;
            });
        }

        if (walkSoundEffect == null)
            return;

        if (_navMeshAgent.velocity.magnitude > 0.1f)
        {
            playSoundWaitFrame += Time.deltaTime;

            if (playSoundWaitFrame > m_FootStepTimespan) //每间隔一小段时间播放一次
            {
                playSoundWaitFrame = 0;
                AudioSource.PlayClipAtPoint(walkSoundEffect, transform.position, 0.3f);
            }
        }
    }

    //缓存脚步声音频
    static AudioClip walkSoundEffect;

    static bool _initWalkSoundEffect = false;
    //播放脚步声计时
    float playSoundWaitFrame = 0;
    //脚步声播放间隔
    public float m_FootStepTimespan = 0.4f;
    #endregion
    
    public async UniTask RefreshModel()
    {
        if (DataInstance == null) return;

        if (string.IsNullOrEmpty(DataInstance.ModelAsset)) return;
        
        await RefreshModelByModelAvata(DataInstance.ModelAsset);
        
        var animator = GetAnimator();
        if (animator != null)
        {
            animator.SetBool("InBattle", IsInBattle);
        }
        
        if (IsInBattle)
        {
            //直接用第一个武功的姿势
            DataInstance.SwitchAnimationToSkill(DataInstance.Wugongs[0]);
        }
        else
        {
            animator.SetTrigger("move");
        }
    }
    
    GameObject m_CurrentWeapon = null;
    
    /// <summary>
    /// 挂载武器
    /// </summary>
    /// <param name="weaponStr"></param>
    void DOMountWeapon(ModelAsset.WeaponPartType weaponCode)
    {
        //if(!IsInBattle) return;
        UnMountCurrentWeapon();

        if (weaponCode == 0)
            return;
        if(modelAsset == null) return;
        
        var weapon = modelAsset.GetWeaponPart(weaponCode);
        if(weapon == null) return;
        if(weapon.m_PartView == null) return;

        m_CurrentWeapon = Instantiate(weapon.m_PartView);
        var parent = UnityTools.DeepFindChild(transform, weapon.m_BindBone);
        if(parent != null)
        {
            m_CurrentWeapon.transform.SetParent(parent.transform);
            m_CurrentWeapon.transform.localScale = weapon.m_OffsetScale;
            m_CurrentWeapon.transform.localPosition = weapon.m_OffsetPosition;
            m_CurrentWeapon.transform.localRotation = Quaternion.Euler(weapon.m_OffsetRotation);
        }
        else
        {
            Debug.LogError("武器挂载到了不存在的节点：" + weapon.m_BindBone);
        }
    }

    void UnMountCurrentWeapon()
    {
        if(m_CurrentWeapon == null) return;
        
        m_CurrentWeapon.transform.localScale = Vector3.zero;
    }

    private bool _isRefreshingModel = false;

    private string modelId;
    private ModelAsset modelAsset;
    
    async UniTask RefreshModelByModelAvata(string modelAvataCode)
    {
        if (_isRefreshingModel)
        {
            //Debug.LogError("代码编写错误：重复调用了同一个RefreshModelByModelAvata");
            return;
        }
        _isRefreshingModel = true;
        var tmp = modelAvataCode.Split('#');

        string modelId = tmp[0];
        string weaponId = "";
        if(tmp.Length >= 2)
        {
            weaponId = tmp[1];
        }

        if (modelId == String.Empty)
        {
            _isRefreshingModel = false;
            return;
        }

        //跟当前一致，不需要替换
        if (this.modelId == modelId)
        {
            _isRefreshingModel = false;
            return;
        }

        this.modelId = modelId;
        await OnChange();
        _isRefreshingModel = false;
    }
    
    async UniTask OnChange()
    {
        if (Application.isPlaying)
        {
            //销毁所有的孩子
            HSUnityTools.DestroyChildren(transform);
        }
        else
        {
            int childCount = transform.childCount;
            for (int i = childCount - 1; i >= 0; --i)
            {
                var go = transform.GetChild(i).gameObject;
                go.SetActive(false);
                DestroyImmediate(go);
            }
            transform.DetachChildren();

            //适应老的代码。。清理残留的合并Mesh
            var oldMesh = GetComponent<SkinnedMeshRenderer>();
            if (oldMesh != null)
            {
                DestroyImmediate(oldMesh);
            }
        }

        string path = $"Assets/BuildSource/Jyx2RoleModelAssets/{modelId}.asset";

        var modelAsset = await Addressables.LoadAssetAsync<ModelAsset>(path).Task;
        if (modelAsset == null) return;
        var modelView = Instantiate(modelAsset.m_View);
        modelView.transform.SetParent(gameObject.transform, false);
        modelView.transform.localPosition = Vector3.zero;
        DataInstance.Model = modelAsset;
        
        var animator = GetComponent<Animator>();
        if(animator != null)
            animator.enabled = false;

        this.modelAsset = modelAsset;
    }
    #region 角色残影

    GhostShadow m_ghostShadow;

    public void BeginGhostShadow(Color color)
    {
        m_ghostShadow = GetComponent<GhostShadow>();
        if (m_ghostShadow == null)
            m_ghostShadow = gameObject.AddComponent<GhostShadow>();

        m_ghostShadow.m_Color = color;
        m_ghostShadow.m_bOpenGhost = true;
    }

    public void StopGhostShadow()
    {
        if (m_ghostShadow != null)
        {
            m_ghostShadow.enabled = false;
            GameObject.Destroy(m_ghostShadow);
            m_ghostShadow = null;
        }
    }
    #endregion
    
    public override void DeadOrIdle()
    {
        if (DataInstance.IsDead())
        {
            ShowDeath();
        }
        else
        {
            Idle();
        }
    }
}

public static class MapRoleTools
{
    public static IEnumerable<MapRole> ToMapRoles(this List<RoleInstance> roles)
    {
        foreach (var role in roles)
        {
            yield return role.View;
        }
    }
}
