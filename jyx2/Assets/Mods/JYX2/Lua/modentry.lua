local function UpdateArchive(runtime, oldVersion)
    --修复主角与船在大地图的位置
    if (runtime.WorldData == nil) then
        runtime.WorldData = CS.WorldMapSaveData()
    end
    runtime.WorldData.WorldPosition = CS.UnityEngine.Vector3(234.82, 5.2, 357.46)
    runtime.WorldData.BoatWorldPos = CS.UnityEngine.Vector3(100, 4.9, 109)
    runtime.WorldData.OnBoat = 0
    --强制离开小地图
    runtime.SubMapData = CS.SubMapSaveData(1000)
end

local function OnBeforeBattle(battleStartParams)
    print("战斗参数可以在这修改")
    print("当前战斗角色数量:" .. battleStartParams.roles.Count)
end


function LuaMod_Init()
    print("Mod Lua初始化")
    LuaEventDispatcher:addListener("OnArchiveOutdated", UpdateArchive)
    LuaEventDispatcher:addListener("OnBeforeBattle", OnBeforeBattle)
end


function LuaMod_DeInit()
    print("Mod Lua析构")
    LuaEventDispatcher:removeListener("OnArchiveOutdated", UpdateArchive)
    LuaEventDispatcher:removeListener("OnBeforeBattle", OnBeforeBattle)
end
--[[ 测试mod端LuaScripts功能
local configMgr = Jyx2:GetModule("ConfigMgr")
local modTest = Jyx2:GetModule("ModTest")
print(configMgr.Character[122].Name)
modTest:PrintTest()]]
