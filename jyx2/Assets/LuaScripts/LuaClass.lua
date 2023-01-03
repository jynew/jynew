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
-- 封装创建class的方法
local function class(className, super) -- 类名className 父类super(可为空)
    -- 构建类
    local clazz = { __cname = className, super = super}
    if super then
        -- 将父类设置为子类的元表，让子类访问父类成员
        setmetatable(clazz, { __index = super })
    end
    -- new方法用来创建对象
    clazz.new = function(...)
        -- 构建一个对象
        local instance = {}
        -- 将对象元表设置为当前类，用来访问类当前类的元素
        setmetatable(instance, { __index = clazz })
        if clazz.ctor then
            clazz.ctor(instance, ...)
        end
        return instance
    end
    return clazz
end

return class
