local tmp = Jyx2.ConfigMgr:GetConfig("Item")
local tmp1 = tmp[1]
print(tmp1.IsBeingUsedBy(CS.Jyx2.RoleInstance(1)))
