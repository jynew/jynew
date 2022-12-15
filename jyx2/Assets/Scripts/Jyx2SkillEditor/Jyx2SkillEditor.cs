/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using Cysharp.Threading.Tasks;
using UnityEngine;
using Jyx2;
using Sirenix.OdinInspector;

public class Jyx2SkillEditor : MonoBehaviour
{
    public Jyx2SkillEditorEnemy player;
    public Jyx2SkillEditorEnemy[] enemys;

    public Transform[] faceTrans;
    public Transform[] lineTrans;
    public Transform[] crossTrans;
    
    
    [LabelText("测试技能覆盖范围")]
    public SkillCoverType coverType = SkillCoverType.POINT;


    private async UniTask TryDisplaySkill(Jyx2SkillDisplayAsset display)
    {
        SkillCastHelper helper = new SkillCastHelper();
        helper.Source = player;
        helper.Targets = enemys;
        
        helper.SkillDisplay = display;

        //根据不同的技能覆盖类型，显示不同的效果
        Transform[] blocks = null;

        switch (coverType)
        {
            case SkillCoverType.RECT:
                blocks = faceTrans;
                break;
            case SkillCoverType.RHOMBUS:
                blocks = faceTrans;
                break;
            case SkillCoverType.LINE:
                blocks = lineTrans;
                break;
            case SkillCoverType.CROSS:
                blocks = crossTrans;
                break;
            case SkillCoverType.POINT:
                
                //任选一个敌人受击
                blocks = new Transform[1] {Jyx2.Middleware.Tools.GetRandomElement(enemys).transform};
                
                //直接在每个敌人身上受击
                /*blocks = new Transform[enemys.Length];
                int index = 0;
                foreach(var e in enemys)
                {
                    blocks[index++] = e.transform;
                }*/
                break;
            case SkillCoverType.INVALID:
            default:
                Debug.LogError("invalid skill cover type!");
                break;                
        }
        
        helper.CoverBlocks = blocks; 
        

        await helper.Play();
    }
 

    /// <summary>
    /// 预览技能
    /// </summary>
    /// <param name="skill"></param>
    public void PreviewSkill(Jyx2SkillDisplayAsset skill)
    {
        TryDisplaySkill(skill).Forget();
    }
}
