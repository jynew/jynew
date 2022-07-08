//#define JYX2_USE_HSFRAMEWORK
#if JYX2_USE_HSFRAMEWORK
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Jyx2;
using Jyx2Configs;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Jyx2PojoToScriptTool : Editor
{
    [MenuItem("Tools/金庸/配置表转换ALL")]
    public static void ConvertAllPojos()
    {
        ConfigTable.InitSync();

        //顺序不能乱，因为有依赖关系
        ConvertSkills();
        ConvertItems();
        ConvertCharacters();
        ConvertMapsPojos();
        ConvertBattlePojos();
        ConvertShops();
        AssetDatabase.SaveAssets();
    }
    
    [MenuItem("Tools/金庸/配置表转换（技能）")]
    public static void ConvertSkillPojos()
    {
        ConfigTable.InitSync();

        ConvertSkills();
        AssetDatabase.SaveAssets();
    }
    
    [MenuItem("Tools/金庸/配置表转换（道具）")]
    public static void ConvertItemsPojos()
    {
        ConfigTable.InitSync();

        ConvertItems();
        AssetDatabase.SaveAssets();
    }
    
    [MenuItem("Tools/金庸/配置表转换（角色）")]
    public static void ConvertCharacterPojos()
    {
        ConfigTable.InitSync();
        ConvertCharacters();
        AssetDatabase.SaveAssets();
    }
    
    [MenuItem("Tools/金庸/配置表转换（地图）")]
    public static void ConvertMapsPojos()
    {
        ConfigTable.InitSync();

        ConvertMaps();
        AssetDatabase.SaveAssets();
    }
    
    [MenuItem("Tools/金庸/配置表转换（战斗）")]
    public static void ConvertBattlePojos()
    {
        ConfigTable.InitSync();

        ConvertBattles();
        AssetDatabase.SaveAssets();
    }
    

    static void ConvertItems()
    {
        AssetDatabase.DeleteAsset("Assets/BuildSource/Configs/Items");
        AssetDatabase.CreateFolder("Assets/BuildSource/Configs", "Items");
        //先生成所有的道具
        foreach (var i in ConfigTable.GetAll<Jyx2Item>())
        {
            var c = ScriptableObject.CreateInstance<Jyx2ConfigItem>();

            c.Id = int.Parse(i.Id);
            c.Name = i.Name;
            var iconAsset = AssetDatabase.GUIDFromAssetPath($"Assets/BuildSource/Items/{c.Id}.png");
            c.Pic = new AssetReferenceTexture2D(iconAsset.ToString());
            c.Desc = i.Desc;
            c.ItemType = (Jyx2ConfigItem.Jyx2ConfigItemType) i.ItemType;
            c.EquipmentType = (Jyx2ConfigItem.Jyx2ConfigItemEquipmentType) i.EquipmentType;
            if (i.Wugong != -1)
            {
                c.SkillCast = GetSkillAsset(i.Wugong);
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
            c.ConditionAttack = i.ConditionAttack;
            c.ConditionQinggong = i.ConditionQinggong;
            c.ConditionPoison = i.ConditionPoison;
            c.ConditionHeal = i.ConditionHeal;
            c.ConditionDePoison = i.ConditionDePoison;
            c.ConditionQuanzhang = i.ConditionQuanzhang;
            c.ConditionYujian = i.ConditionYujian;
            c.ConditionShuadao = i.ConditionShuadao;
            c.ConditionQimen = i.ConditionQimen;
            c.ConditionAnqi = i.ConditionAnqi;
            c.ConditionIQ = i.ConditionIQ;
            c.NeedExp = i.NeedExp;
            c.GenerateItemNeedExp = i.GenerateItemNeedExp;
            
            AssetDatabase.CreateAsset(c, $"Assets/BuildSource/Configs/Items/{c.Id}_{c.Name}.asset");
        }
        
        AssetDatabase.SaveAssets();
        
        //然后生成道具的引用
        foreach (var i in ConfigTable.GetAll<Jyx2Item>())
        {
            var c = GetItemAsset(int.Parse(i.Id));

            //需材料
            if (i.GenerateItemNeedCost >= 0)
            {
                c.GenerateItemNeedCost = GetItemAsset(i.GenerateItemNeedCost);    
            }

            c.GenerateItems = new List<Jyx2ConfigCharacterItem>();
            foreach (var item in i.GenerateItems)
            {
                if (item.Count == 0 || item.Id < 0) continue;
                Jyx2ConfigCharacterItem generateItem = new Jyx2ConfigCharacterItem();
                generateItem.Item = GetItemAsset(item.Id);
                generateItem.Count = item.Count;
                c.GenerateItems.Add(generateItem);
            }
        }
    }
    
    static void ConvertSkills()
    {
        AssetDatabase.DeleteAsset("Assets/BuildSource/Configs/Skills");
        AssetDatabase.CreateFolder("Assets/BuildSource/Configs", "Skills");
        AssetDatabase.SaveAssets();
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
        AssetDatabase.DeleteAsset("Assets/BuildSource/Configs/Characters");
        AssetDatabase.CreateFolder("Assets/BuildSource/Configs", "Characters");
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
                newS.SkillCast = asset;
                c.Skills.Add(newS);
            }

            var mapping = ConfigTable.Get<Jyx2RoleHeadMapping>(r.Head);
            if (mapping != null)
            {
                string path = $"Assets/BuildSource/Jyx2RoleModelAssets/{mapping.ModelAsset}.asset";
                c.Model = AssetDatabase.LoadAssetAtPath<ModelAsset>(path);
                
                var headPicAsset = AssetDatabase.GUIDFromAssetPath($"Assets/BuildSource/head/{r.Head}.png");
                c.Pic = new AssetReferenceTexture2D(headPicAsset.ToString());
            }
            else
            {
                Assert.Fail();
                /*string path = $"Assets/BuildSource/Jyx2RoleModelAssets/{r.Name}.asset";
                c.Model = AssetDatabase.LoadAssetAtPath<ModelAsset>(path);
                
                var headPicAsset = AssetDatabase.GUIDFromAssetPath($"Assets/BuildSource/head/{r.Head}.png");
                c.Pic = new AssetReferenceTexture2D(headPicAsset.ToString());*/
            }

            //武器
            if (r.Weapon != -1)
            {
                c.Weapon = GetItemAsset(r.Weapon);
            }

            //防具
            if (r.Armor != -1)
            {
                c.Armor = GetItemAsset(r.Armor);
            }
            
            //生成携带道具
            c.Items = new List<Jyx2ConfigCharacterItem>();
            foreach (var item in r.Items)
            {
                if (item.Count == 0 || item.Id < 0) continue;
                Jyx2ConfigCharacterItem generateItem = new Jyx2ConfigCharacterItem();
                generateItem.Item = GetItemAsset(item.Id);
                generateItem.Count = item.Count;
                c.Items.Add(generateItem);
            }
            
            AssetDatabase.CreateAsset(c, $"Assets/BuildSource/Configs/Characters/{r.Id}_{r.Name}.asset");
        }
    }

    static void ConvertMaps()
    {
        AssetDatabase.DeleteAsset("Assets/BuildSource/Configs/Maps");
        AssetDatabase.CreateFolder("Assets/BuildSource/Configs", "Maps");
        generateMaps = new Dictionary<int, Jyx2ConfigMap>();
        foreach (var map in ConfigTable.GetAll<GameMap>())
        {
            if (string.IsNullOrEmpty(map.Jyx2MapId)) continue;

            var jyx2Map = ConfigTable.Get<Jyx2Map>(map.Jyx2MapId);
            Assert.IsNotNull(jyx2Map);
            
            var c = ScriptableObject.CreateInstance<Jyx2ConfigMap>();

            c.Id = int.Parse(jyx2Map.Id);
            c.Name = jyx2Map.Name; 
                
            //地图引用
            var sceneAsset = AssetDatabase.GUIDFromAssetPath($"Assets/Jyx2Scenes/{map.Key}.unity");
            Assert.IsNotNull(sceneAsset);
            c.MapScene = new AssetReference(sceneAsset.ToString());
             
            //音乐引用
            c.InMusic = GetAudioClip(jyx2Map.InMusic);
            c.OutMusic = GetAudioClip(jyx2Map.OutMusic);

            c.EnterCondition = jyx2Map.EnterCondition;
            c.Tags = map.Tags;
            generateMaps[c.Id] = c;
            AssetDatabase.CreateAsset(c, $"Assets/BuildSource/Configs/Maps/{c.Id}_{c.Name}.asset");
        }
    }

    static private Dictionary<int, Jyx2ConfigMap> generateMaps;

    static void ConvertBattles()
    {
        AssetDatabase.DeleteAsset("Assets/BuildSource/Configs/Battles");
        AssetDatabase.CreateFolder("Assets/BuildSource/Configs", "Battles");
        foreach (var b in ConfigTable.GetAll<Jyx2Battle>())
        {
            var c = ScriptableObject.CreateInstance<Jyx2ConfigBattle>();

            c.Id = int.Parse(b.Id);
            c.Name = b.Name;
            
            //地图引用
            var sceneAsset = AssetDatabase.GUIDFromAssetPath($"Assets/Jyx2BattleScene/Jyx2Battle_{b.MapId}.unity");
            Assert.IsNotNull(sceneAsset);
            c.MapScene = new AssetReference(sceneAsset.ToString());
            
            c.Music = GetAudioClip(b.Music);

            c.Exp = b.Exp;

            c.TeamMates = new List<Jyx2ConfigCharacter>();
            foreach (var role in b.TeamMates)
            {
                if (role.Value == -1) continue;
                var asset = GetCharacterAsset(role.Value);
                Assert.IsNotNull(asset);
                c.TeamMates.Add(asset);
            }
            
            c.AutoTeamMates = new List<Jyx2ConfigCharacter>();
            foreach (var role in b.AutoTeamMates)
            {
                if (role.Value == -1) continue;
                var asset = GetCharacterAsset(role.Value);
                Assert.IsNotNull(asset);
                c.AutoTeamMates.Add(asset);
            }

            c.Enemies = new List<Jyx2ConfigCharacter>();
            foreach (var role in b.Enemies)
            {
                if (role.Value == -1) continue;
                var asset = GetCharacterAsset(role.Value);
                Assert.IsNotNull(asset);
                c.Enemies.Add(asset);
            }
            
            AssetDatabase.CreateAsset(c, $"Assets/BuildSource/Configs/Battles/{c.Id}_{c.Name}.asset");
        }
    }

    static void ConvertShops()
    {
        AssetDatabase.DeleteAsset("Assets/BuildSource/Configs/Shops");
        AssetDatabase.CreateFolder("Assets/BuildSource/Configs", "Shops");

        foreach (var b in ConfigTable.GetAll<Jyx2Shop>())
        {
            var c = ScriptableObject.CreateInstance<Jyx2ConfigShop>();
            c.Id = int.Parse(b.Id);
            c.Trigger = b.Trigger;
            c.ShopItems = new List<Jyx2ConfigShopItem>();
            
            foreach (var item in b.ShopItems)
            {
                Jyx2ConfigShopItem genItem = new Jyx2ConfigShopItem();
                genItem.Item = GetItemAsset(item.Id);
                genItem.Count = item.Count;
                genItem.Price = item.Price;
                c.ShopItems.Add(genItem);
            }
            
            AssetDatabase.CreateAsset(c, $"Assets/BuildSource/Configs/Shops/{c.Id}_{generateMaps[c.Id].Name}.asset");
        }
    }
    
    static Jyx2ConfigSkill GetSkillAsset(int id)
    {
        var pojo = ConfigTable.Get<Jyx2Skill>(id);
        var asset = AssetDatabase.LoadAssetAtPath<Jyx2ConfigSkill>(
            $"Assets/BuildSource/Configs/Skills/{id}_{pojo.Name}.asset");
        return asset;
    }

    static Jyx2ConfigItem GetItemAsset(int id)
    {
        var pojo = ConfigTable.Get<Jyx2Item>(id);
        var asset = AssetDatabase.LoadAssetAtPath<Jyx2ConfigItem>(
            $"Assets/BuildSource/Configs/Items/{id}_{pojo.Name}.asset");
        return asset;
    }
    
    static Jyx2ConfigCharacter GetCharacterAsset(int id)
    {
        var pojo = ConfigTable.Get<Jyx2Role>(id);
        var asset = AssetDatabase.LoadAssetAtPath<Jyx2ConfigCharacter>(
            $"Assets/BuildSource/Configs/Characters/{id}_{pojo.Name}.asset");
        return asset;
    }

    static AssetReferenceT<AudioClip> GetAudioClip(int id)
    {
        if (id == -1) return null;
        var audioClip = AssetDatabase.GUIDFromAssetPath($"Assets/BuildSource/Musics/{id}.mp3");
        Assert.IsNotNull(audioClip);
        return new AssetReferenceT<AudioClip>(audioClip.ToString());
    }
}
#endif