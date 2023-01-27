--[[
/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
 ]]--
-- 封装一个自己的 unity 协程
local util = require 'xlua.util'

local gameobject = CS.UnityEngine.GameObject.Find('Coroutine_Runner') or CS.UnityEngine.GameObject('Coroutine_Runner')

if CS.UnityEngine.Application.isPlaying then
    print("游戏没有在运行，unity协程没法正常工作")
    CS.UnityEngine.Object.DontDestroyOnLoad(gameobject)
end

local cs_coroutine_runner = gameobject:GetComponent(typeof(CS.Jyx2.Coroutine_Runner))
if cs_coroutine_runner == nil then
    cs_coroutine_runner = gameobject:AddComponent(typeof(CS.Jyx2.Coroutine_Runner))
end

return {
    start = function(...)
        return cs_coroutine_runner:StartCoroutine(util.cs_generator(...))
    end;

    stop = function(coroutine)
        cs_coroutine_runner:StopCoroutine(coroutine)
    end
}
