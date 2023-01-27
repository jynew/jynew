--[[
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 ]]--
-- 本脚本负责添加为编写代码服务的功能函数

-- prequire 用于载入模块，如果模块不存在，返回空表
local function prequire(name)
    local rst = {}
    if pcall(function(x) rst = require(x) end, name) then
        return rst
    else
        print(name.."载入失败")
        return rst
    end
end

local cs_coroutine = require('Jyx2Coroutine')

-- 用于捕捉错误信息
local function errorcatch(func)
    return function(callback, ...)
        local rst, err = pcall(func, callback, ...)
        if rst == false then
            callback(false, "", err)
        end
    end
end

-- c#侧接口：等待Lua侧的异步函数
local function cs_await(asyncFunName, callback, ...)
    -- 根据函数名称获取该函数
    local asyncFun = load("return "..asyncFunName)()
    if (asyncFun == nil) then
        error("未找到Lua函数:"..asyncFunName)
    end
    -- 使用unity协程运行被调用的异步函数
    local co = cs_coroutine.start(errorcatch(asyncFun), callback, ...)
end

-- c#侧接口：调用Lua侧同步函数
local function cs_calllua(luaFunName, ...)
    local luaFun = load("return "..luaFunName)()
    if (luaFun == nil) then
        error("未找到Lua函数:"..luaFunName)
    end
    return luaFun(...)
end

return {prequire = prequire, cs_await = cs_await, cs_calllua = cs_calllua}
