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
using Jyx2.Middleware;

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

    //行为
    public MapRoleBehavior m_Behavior = MapRoleBehavior.Npc;

    //行为参数
    public List<float> m_BehaviorParas = new List<float>();

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

        if(_animator == null)
        {
			for(var index=0;index<transform.childCount;index++){
				_animator = transform.GetChild(index).GetComponentInChildren<Animator>();
				if(_animator!=null){
					break;
				}
			}
        }
        return _animator;
    }
    private Animator _animator;
    public void ForceSetAnimator(Animator a)
    {
        _animator = a;
    }

    //状态
    private MapRoleStatus m_Status = MapRoleStatus.Normal;

    //健康状态
    private MapRoleHealth m_Health = MapRoleHealth.Normal;

    private float _distFromPlayer;
    private CompositeDisposable _behaviourDisposable = new CompositeDisposable();

    public void SetBehavior(MapRoleBehavior behavior)
    {
        m_Behavior = behavior;
    }


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

        CheckDeath();
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

    void CheckDeath()
    {
        if (DataInstance.IsDead())
        {
            this.gameObject.SetActive(false); //TODO，播放一个死亡特效
        }
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

    public void SwitchStatus(MapRoleStatus status)
    {
        m_Status = status;
        switch (status)
        {
            case MapRoleStatus.Normal:
                {
                    if (m_IsKeyRole)
                    {
                        if (_navMeshAgent != null) _navMeshAgent.enabled = true;
                        GetAnimator().SetTrigger("LeaveBattle");
                    }
                    GetAnimator().SetBool("IsInBattle", false);
                    //接受动作变化位置
                    GetAnimator().applyRootMotion = true;
                    break;
                }
            case MapRoleStatus.Battle:
                {
                    if (_navMeshAgent != null) _navMeshAgent.enabled = false;
                    GetAnimator().SetBool("IsInBattle", true);
                    //不接受动作变化位置
                    GetAnimator().applyRootMotion = false;
                    break;
                }
        }
    }

    //切换武学
    public void SwitchSkillTo(WugongInstance skill)
    {
        var display = skill.GetDisplay();
        //切换对应武器
        var weaponCode = display.WeaponCode;
        if (!string.IsNullOrEmpty(weaponCode))
        {
            ChangeWeapon(weaponCode);
        }

        this.CurDisplay = display;
        
        //载入动作
        var animationController = display.GetAnimationController();

        if (string.IsNullOrEmpty(animationController))
        {
            Debug.LogError($"技能{skill.Name}没有配置动画控制器！");
            return;
        }
 
        //载入动画控制器
        ChangeAnimationController(animationController,()=> {
            Idle();
        });
    
    }

    string m_currentAnimatorController = "";

    void ChangeWeapon(string weaponCode)
    {
        DOMountWeapon(weaponCode);
    }

    void ChangeAnimationController(string path, Action callback)
    {
        if (string.IsNullOrEmpty(path))
            return;

        //当前已经是该Controller了，不需要再修改
        if (path.Equals(m_currentAnimatorController))
        {
            callback();
            return;
        }
        
        var anim = GetAnimator();
        if (anim == null)
            return;

        //否则载入该Controller
        Jyx2ResourceHelper.LoadAsset<RuntimeAnimatorController>(path, rst =>
        {
            if (rst == null)
                return;
            anim.runtimeAnimatorController = rst;
            m_currentAnimatorController = path;
            callback();
        });
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
        //简单粗暴，直接删除GameObject，回头有空再实现死亡动作
        GameObject.Destroy(this.gameObject);

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

    private void Start()
    {
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
            RefreshModel();
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
    
    public void RefreshModel(Action callback = null)
    {
        if (DataInstance == null) return;

        if (string.IsNullOrEmpty(DataInstance.ModelAsset)) return;
        
        RefreshModelByModelAvata(DataInstance.ModelAsset, ()=> {
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
            
            if(callback != null) callback();
        });
    }
    
    GameObject m_CurrentWeapon = null;
    
    /// <summary>
    /// 挂载武器
    /// </summary>
    /// <param name="weaponStr"></param>
    void DOMountWeapon(string weaponCode)
    {
        if(!IsInBattle) return;
        UnMountCurrentWeapon();
        
        if(string.IsNullOrEmpty(weaponCode)) return;
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
    
    public void RefreshModelByModelAvata(string modelAvataCode, Action callback)
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
        OnChange(() =>
        {
            _isRefreshingModel = false;

            if (callback != null) callback();
        });
    }
    
    private void OnChange(Action callback = null)
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
        Jyx2ResourceHelper.LoadAsset<ModelAsset>(path, modelAsset =>
        {
            if (modelAsset == null) return;
        
            var modelView = Instantiate(modelAsset.m_View);
            modelView.transform.SetParent(gameObject.transform, false);
            modelView.transform.localPosition = Vector3.zero;
            DataInstance.Model = modelAsset;
        
            var animator = GetComponent<Animator>();
            if(animator != null)
                animator.enabled = false;

            callback?.Invoke();
        });
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
    
    public void HitEffect(string effectName, float deltaTime = 0f, bool showDeath = false)
    {
        /*if (DataInstance.IsDead() && showDeath)
        {
            ShowDeath();
        }
        else
        {
            GetAnimator().SetTrigger("hit");
            GetAnimator().Update(deltaTime);
            if (!effectName.EndsWith(".prefab")) effectName += ".prefab";
            Jyx2ResourceHelper.LoadPrefab($"Assets/Effects/Prefabs/{effectName}", effect=> {
                CastSkillFXAndWait(effect, 1f);
            });
        }*/
    }

    public void HitVoice(string musicName)
    {
        Jyx2ResourceHelper.LoadAsset<AudioClip>($"Assets/BuildSource/SoundEffect/{musicName}", soundEffect=> {
            //声源位置与摄像机Y轴一致
            float x = Camera.main.transform.position.x - (Camera.main.transform.position.x - transform.position.x) / 5;
            float y = Camera.main.transform.position.y;
            float z = Camera.main.transform.position.z - (Camera.main.transform.position.z - transform.position.z) / 5;
            Vector3 voicePos = new Vector3(x, y, z);
            AudioSource.PlayClipAtPoint(soundEffect, voicePos, 1f);
        });
    }

    private void CastSkillFXAndWait(GameObject pre, float time, Action callback = null)
    {
        if (pre == null) return;
        GameObject obj = GameObject.Instantiate(pre);
        Transform _hitPoint = transform;
        obj.transform.rotation = _hitPoint.rotation;
        obj.transform.position = _hitPoint.position;
        Observable.TimerFrame(Convert.ToInt32(time * 60), FrameCountType.Update)
        .Subscribe(ms =>
        {
            GameObject.Destroy(obj);
            callback?.Invoke();
        });
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
