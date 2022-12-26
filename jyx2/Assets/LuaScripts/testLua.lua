local tmp = Jyx2.ConfigMgr:GetConfig("Map")
local tmp2 = tmp[70]
local tmp3 = tmp[7]
print(tmp2:GetOutMusic())
tmp2.ForceSetLeaveMusicId = 10
print(tmp2:GetOutMusic())
tmp2.ForceSetLeaveMusicId = -1
print(tmp2:GetOutMusic())
print(tmp3.Name)
tmp2.ForceSetLeaveMusicId = 10
print(tmp2:GetShowName())
print(tmp3.ForceSetLeaveMusicId)
