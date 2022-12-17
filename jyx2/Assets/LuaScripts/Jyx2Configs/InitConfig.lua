require "Jyx2Configs/Jyx2ConfigMgr"

local configMgr = Jyx2ConfigMgr:Instance()
local tmpCfg = configMgr:GetConfig("Character")
print(tmpCfg[1].Id)
print(configMgr.Character[1].Name)
print(configMgr["Character"][1].Id)
local role = configMgr["Character"][2]
print(role.LeaveStoryId)
print(role["Items"][1][1])
