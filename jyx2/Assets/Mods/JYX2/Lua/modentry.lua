local function UpdateArchive(runtime, oldVersion)
    --修复主角与船在大地图的位置

    runtime.WorldData.WorldPosition = CS.UnityEngine.Vector3(234.82, 5.2, 357.46)
    runtime.WorldData.BoatWorldPos = CS.UnityEngine.Vector3(100, 4.9, 109)
    runtime.WorldData.OnBoat = 0
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
