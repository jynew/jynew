-- 设置flag的样例

SetFlag("test", "ok")
if GetFlag("test") == "ok" then
    print("test Set/Get Flag success.")
end

SetFlagInt("testInt", 99)
if GetFlag("testInt") == 99 then
    print("test Set/Get FlagInt success.")
end 

