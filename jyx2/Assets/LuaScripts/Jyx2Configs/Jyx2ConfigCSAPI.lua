--[[
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 ]]--
-- 本脚本负责为ConfigMgr模块添加供C#侧使用的API
local bridge = {}
function bridge.GetConfigValue(cfgName, key, fieldName)
    local cfg = Jyx2:GetModule("ConfigMgr"):GetItem(cfgName, key)
    return cfg[fieldName]
end

function bridge.BindKey(role, id)
    --print("In BindKey Lua")
    local r = Jyx2:GetModule("ConfigMgr"):GetItem("Character", id)
    role.Key = r.Id
    if role.Wugongs.Count == 0 then
        for i,s in pairs(r.Skills) do
            if (#s == 2) then
                local wugong  = CS.Jyx2.SkillInstance()
                wugong.Key = s[1]
                wugong.Level = s[2]
                role.Wugongs:Add(wugong)
            end
        end
    end
    role:ResetSkillCasts()

    role.Items:Clear()
    for i,s in pairs(r.Items) do
        if (s~= -1 and #s == 2) then
            local item = CS.Jyx2Configs.Jyx2ConfigCharacterItem()
            item.Id = s[1]
            item.Count = s[2]
            role.Items:Add(item)
        end
    end
end
function bridge.InitRole(role, id)
    --print("In InitRole:"..id)
    local r = Jyx2:GetModule("ConfigMgr"):GetItem("Character", id)
    role.Key = r.Id
    if role.Wugongs.Count == 0 then
        for i,s in pairs(r.Skills) do
            if (#s == 2) then
                local wugong  = CS.Jyx2.SkillInstance()
                wugong.Key = s[1]
                wugong.Level = s[2]
                role.Wugongs:Add(wugong)
            end
        end
    end
    role:ResetSkillCasts()

    role.Items:Clear()
    for i,s in pairs(r.Items) do
        if (s~= -1 and #s == 2) then
            local item = CS.Jyx2Configs.Jyx2ConfigCharacterItem()
            item.Id = s[1]
            item.Count = s[2]
            role.Items:Add(item)
        end
    end

    role.Name = r.Name
    role.Sex = r.Sexual
    role.Level = r.Level
    role.Exp = r.Exp
    role.Hp = r.MaxHp
    role.PreviousRoundHp = r.MaxHp
    role.MaxHp = r.MaxHp
    role.Mp = r.MaxMp
    role.MaxMp = r.MaxMp
    role.Tili = CS.GameConst.Max_ROLE_TILI
    role.Weapon = r.Weapon
    role.Armor = r.Armor
    role.MpType = r.MpType
    role.Attack = r.Attack
    role.Qinggong = r.Qinggong
    role.Defence = r.Defence
    role.Heal = r.Heal
    role.UsePoison = r.UsePoison
    role.DePoison = r.DePoison
    role.AntiPoison = r.AntiPoison
    role.Quanzhang = r.Quanzhang
    role.Yujian = r.Yujian
    role.Shuadao = r.Shuadao
    role.Qimen = r.Qimen
    role.Anqi = r.Anqi
    role.Wuxuechangshi = r.Wuxuechangshi
    role.Pinde = r.Pinde
    role.AttackPoison = r.AttackPoison
    role.Zuoyouhubo = r.Zuoyouhubo
    role.IQ = r.IQ
    role.HpInc = r.HpInc
end

function bridge.InitAllRole(roleList)
    --print("In InitAllRole Lua")
    local charCfg = Jyx2:GetModule("ConfigMgr"):GetConfig("Character")
    for i,rCfg in pairs(charCfg) do
        if i ~= "ItemNum" and rCfg.Id ~= nil then
            local role = CS.Jyx2.RoleInstance(rCfg.Id)
            if role ~= nil then
                roleList:Add(rCfg.Id, role)
            end
        end
    end
end

return bridge
