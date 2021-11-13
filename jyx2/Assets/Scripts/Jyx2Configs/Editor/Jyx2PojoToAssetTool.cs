using System.Collections;
using System.Collections.Generic;
using HSFrameWork.ConfigTable;
using Jyx2;
using Jyx2Configs;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Jyx2PojoToScriptTool : Editor
{
    [MenuItem("Tools/金庸/配置表转换")]
    public static void ConvertAllPojos()
    {
        ConfigTable.InitSync();

        ConvertSkills();
        ConvertItems();
        ConvertCharacters();
    }

    static void ConvertItems()
    {
        foreach (var i in ConfigTable.GetAll<Jyx2Item>())
        {
            var c = ScriptableObject.CreateInstance<Jyx2ConfigItem>();

            c.Id = int.Parse(i.Id);
            c.Name = i.Name;
            var iconAsset = AssetDatabase.GUIDFromAssetPath($"Assets/BuildSource/Jyx2Items/{c.Id}.png");
            c.Pic = new AssetReferenceTexture2D(iconAsset.ToString());
            c.Desc = i.Desc;
            c.ItemType = (Jyx2ConfigItem.Jyx2ConfigItemType) i.ItemType;
            c.EquipmentType = (Jyx2ConfigItem.Jyx2ConfigItemEquipmentType) i.EquipmentType;
            if (i.Wugong != -1)
            {
                c.Skill = GetSkillAsset(i.Wugong);
            }

            c.AddHp = i.AddHp;
            c.AddMaxHp = i.AddMaxHp;
            c.ChangePoisonLevel = i.ChangePoisonLevel;
            c.AddTili = i.AddTili;
            c.ChangeMPType = i.ChangeMPType;
            c.AddMp = i.AddMp;
            c.AddMaxMp = i.AddMaxMp;
            c.Attack = i.Attack;
            c.Qinggong = i.Qinggong;
            c.Defence = i.Defence;
            c.Heal = i.Heal;
            c.UsePoison = i.UsePoison;
            c.DePoison = i.DePoison;
            c.AntiPoison = i.AntiPoison;
            c.Quanzhang = i.Quanzhang;
            c.Yujian = i.Yujian;
            c.Shuadao = i.Shuadao;
            c.Qimen = i.Qimen;
            c.Anqi = i.Anqi;
            c.Wuxuechangshi = i.Wuxuechangshi;
            c.AddPinde = i.AddPinde;
            c.Zuoyouhubo = i.AttackFreq;
            c.AttackPoison = i.AttackPoison;
            c.OnlySuitableRole = i.OnlySuitableRole;
            c.NeedMPType = (Jyx2ConfigCharacter.MpTypeEnum) i.NeedMPType;

            c.ConditionMp = i.ConditionMp;
            c.ConditionAttack = i.ConditionMp;
            c.ConditionQinggong = i.ConditionMp;
            c.ConditionPoison = i.ConditionMp;
            c.ConditionHeal = i.ConditionMp;
            c.ConditionDePoison = i.ConditionMp;
            c.ConditionQuanzhang = i.ConditionMp;
            c.ConditionYujian = i.ConditionMp;
            c.ConditionShuadao = i.ConditionMp;
            c.ConditionQimen = i.ConditionMp;
            c.ConditionAnqi = i.ConditionAnqi;
            c.ConditionIQ = i.ConditionIQ;
            c.NeedExp = i.NeedExp;
            c.GenerateItemNeedExp = i.GenerateItemNeedExp;
            c.GenerateItemNeedCost = i.GenerateItemNeedCost;
            
                
            
            AssetDatabase.CreateAsset(c, $"Assets/BuildSource/Configs/Items/{c.Id}_{c.Name}.asset");
        }
    }
    
    static void ConvertSkills()
    {
        foreach (var s in ConfigTable.GetAll<Jyx2Skill>())
        {
            var c = ScriptableObject.CreateInstance<Jyx2ConfigSkill>();

            c.Id = int.Parse(s.Id);
            c.Name = s.Name;
            c.DamageType = (Jyx2ConfigSkill.Jyx2ConfigSkillDamageType) s.DamageType;
            c.SkillCoverType = (Jyx2ConfigSkill.Jyx2ConfigSkillCoverType) s.SkillCoverType;
            c.MpCost = s.MpCost;
            c.Poison = s.Poison;
            
            var display =
                AssetDatabase.LoadAssetAtPath<Jyx2SkillDisplayAsset>($"Assets/BuildSource/Skills/{s.Name}.asset");
            c.Display = display;

            c.Levels = new List<Jyx2ConfigSkillLevel>();
            foreach (var level in s.SkillLevels)
            {
                var sl = new Jyx2ConfigSkillLevel();
                sl.Attack = level.Attack;
                sl.AddMp = level.AddMp;
                sl.AttackRange = level.AttackRange;
                sl.KillMp = level.KillMp;
                sl.SelectRange = level.SelectRange;
                c.Levels.Add(sl);
            }
            
            AssetDatabase.CreateAsset(c, $"Assets/BuildSource/Configs/Skills/{s.Id}_{s.Name}.asset");
        }
    }
    
    static void ConvertCharacters()
    {
        int index = 0;
        foreach (var r in ConfigTable.GetAll<Jyx2Role>())
        {
            //if (index++ > 10) break; //TO BE DELETE
            var c = ScriptableObject.CreateInstance<Jyx2ConfigCharacter>();
            c.Name = r.Name;
            c.Id = int.Parse(r.Id);
            c.Attack = r.Attack;
            c.Sexual = (Jyx2ConfigCharacter.SexualType)(r.Sex);
            c.Pinde = r.Pinde;
            c.IQ = r.IQ;
            c.MaxHp = r.MaxHp;
            c.MaxMp = r.MaxMp;
            c.HpInc = r.HpInc;
            c.Level = r.Level;
            //c.Exp = r.Exp;
            c.MpType = (Jyx2ConfigCharacter.MpTypeEnum)r.MpType;
            c.Attack = r.Attack;
            c.Qinggong = r.Qinggong;
            c.Defence = r.Defence;
            c.Heal = r.Heal;
            c.UsePoison = r.UsePoison;
            c.DePoison = r.DePoison;
            c.AntiPoison = r.AntiPoison;
            c.Quanzhang = r.Quanzhang;
            c.Yujian = r.Yujian;
            c.Shuadao = r.Shuadao;
            c.Qimen = r.Qimen;
            c.Anqi = r.Anqi;
            c.Wuxuechangshi = r.Wuxuechangshi;
            c.AttackPoison = r.AttackPoison;
            c.Zuoyouhubo = r.Zuoyouhubo;
            c.LeaveStoryId = r.Dialogue;

            c.Skills = new List<Jyx2ConfigCharacterSkill>();
            foreach (var skill in r.Wugongs)
            {
                if (skill.Level == 0 && skill.Id == 0) continue;
                
                var newS = new Jyx2ConfigCharacterSkill();
                newS.Level = skill.Level;
                var asset = GetSkillAsset(skill.Id);
                Assert.NotNull(asset);
                newS.Skill = asset;
                c.Skills.Add(newS);
            }
            

            var headPicAsset = AssetDatabase.GUIDFromAssetPath($"Assets/BuildSource/head/{r.Head}.png");
            c.Pic = new AssetReferenceTexture2D(headPicAsset.ToString());

            var mapping = ConfigTable.Get<Jyx2RoleHeadMapping>(c.Id);
            if (mapping != null)
            {
                string path = $"Assets/BuildSource/Jyx2RoleModelAssets/{mapping.ModelAsset}.asset";
                c.Model = AssetDatabase.LoadAssetAtPath<ModelAsset>(path);
            }
            
            
            AssetDatabase.CreateAsset(c, $"Assets/BuildSource/Configs/Characters/{r.Id}_{r.Name}.asset");
        }
    }

    static Jyx2ConfigSkill GetSkillAsset(int id)
    {
        var pojo = ConfigTable.Get<Jyx2Skill>(id);
        var asset = AssetDatabase.LoadAssetAtPath<Jyx2ConfigSkill>(
            $"Assets/BuildSource/Configs/Skills/{id}_{pojo.Name}.asset");
        return asset;
    }
}
