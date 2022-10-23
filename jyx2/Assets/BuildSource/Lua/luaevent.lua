local LuaEventDispatcher = { _listeners_ = {}, }

function LuaEventDispatcher:addListener(eventName, listener)
    if self._listeners_[eventName] == nil then
        self._listeners_[eventName] = {}
    end

    for _,v in pairs(self._listeners_[eventName]) do
        if v == listener then
            return
        end
    end
    table.insert(self._listeners_[eventName], listener)
end

function LuaEventDispatcher:removeListener(eventName, listener)
    if self._listeners_[eventName] == nil then
        return;
    end
    local callbacks = self._listeners_[eventName];
    for i = #callbacks ,1, -1 do
        if callbacks[i] == listener then
            table.remove(callbacks, i)
        end
    end
end


function LuaEventDispatcher:removeListenersByEvent(eventName)
    if self._listeners_[eventName] == nil then
        return;
    end
    local arr = self._listeners_[eventName];
    for i in pairs(arr) do
        arr[i] = nil
    end
end

function LuaEventDispatcher:dispatchEvent(eventName, ...)
    if self._listeners_[eventName] == nil then
        return;
    end
    local arr = self._listeners_[eventName]
    for i = 1, #arr do
        arr[i](...)
    end
end

function LuaEventDispatcher:ToString()
    local s = ''
    for k, v in pairs(self._listeners_) do
        if #v ~= 0 then
            s = s .. k .. ','
        end
    end
    return s
end

--给C#侧调用的wrapper函数
function LuaEventDispatcher_DispatchEvent(eventName, ...)
    LuaEventDispatcher:dispatchEvent(eventName, ...)
end

return LuaEventDispatcher