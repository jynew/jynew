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
--类的声明，这里声明了类名还有属性，并且给出了属性的初始值
LuaClass = {}
--设置元表的索引，想模拟类的话，这步操作很关键
LuaClass.__index = LuaClass
--构造方法，构造方法的名字是随便起的，习惯性命名为new()
function LuaClass:new()
	 local self = {}  --初始化self，如果没有这句，那么类所建立的对象如果有一个改变，其他对象都会改变
	 setmetatable(self, LuaClass)  --将self的元表设定为Class
	 return self  --返回自身
end
-- todo:可以新增一些基类方法
