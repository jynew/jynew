local function OnBeforeBattle(battleStartParams)
    print("战斗参数可以在这修改")
    print("当前战斗角色数量:" .. battleStartParams.roles.Count)
end


function LuaMod_Init()
    print("Mod Lua初始化")
    LuaEventDispatcher:addListener("OnBeforeBattle", OnBeforeBattle)
end


function LuaMod_DeInit()
    print("Mod Lua析构")
    LuaEventDispatcher:removeListener("OnBeforeBattle", OnBeforeBattle)
end