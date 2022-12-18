require "Jyx2Configs/Jyx2ConfigMgr"

local configMgr = Jyx2ConfigMgr:Instance()
local tmpCfg = configMgr:GetConfig("Battle")
print(tmpCfg[1].Id)
