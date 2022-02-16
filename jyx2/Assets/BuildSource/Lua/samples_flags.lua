-- 设置flag的样例

SetFlag("test", "ok")
if GetFlag("test") == "ok" then
    print("test Set/GetFlag success.")
end

SetFlagInt("testInt", 99)
if GetFlagInt("testInt") == 99 then
    print("test Set/GetFlagInt success.")
end 

