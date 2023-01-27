--[[
 * 金庸群侠传3D重制版
 * https:--github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 ]]--
-- 本脚本为Lua侧战斗格子范围计算模块（还没完成，与c#侧共存中）
local range = {}

-- 局部变量
local MAX_COMPUTE_BLOCKS_SIZE = 10

local dx = {1,0,-1,0}
local dy = {0,1,0,-1}

local MOVEBLOCK_MAX_X = 500
local MOVEBLOCK_MAX_Y = 500

local SkillCoverType

range.cached_nearBlocks_0_0 = {}
range.cached_nearBlocks_0_1 = {}

local function GenerateNearBlocks(x, y, maxDist)
    local rst = {}
    local idx = 0

    for i=0,maxDist do
        for j=i-maxDist,maxDist-i do
            idx = idx + 1
            rst[idx] = {X = x + i, Y = y + j}
        end
    end
    for i=-1,-maxDist,-1 do
        for j=-i-maxDist,maxDist+i do
            idx = idx + 1
            rst[idx] = {X = x + i, Y = y + j}
        end
    end
    return rst
end

local cached_valid_blocks = {}
local battleboxHelper

local function IsValidBlock(x, y)

    local pos = x * 10000 + y
    local blk = cached_valid_blocks[pos]

    if (blk ~= nil) then
        return blk
    else
        cached_valid_blocks[pos] = battleboxHelper:IsBlockExists(x,y)
        return cached_valid_blocks[pos]
    end
end

-- 初始化
function range.Init()
    if (range.inited == true) then
        return
    end

    SkillCoverType = Jyx2.Battle.SkillCoverType

    for i=1,MAX_COMPUTE_BLOCKS_SIZE do
        range.cached_nearBlocks_0_0[i] = GenerateNearBlocks(0, 0, i)
        --range.cached_nearBlocks_0_1[i] = GenerateNearBlocks(0, 1, i)
    end

    cached_valid_blocks = {}
    battleboxHelper = CS.BattleboxHelper.Instance

    range.inited = true
end

function range.DeInit()
    if (range.inited == false) then
        return
    end

    SkillCoverType = nil

    for i=1,MAX_COMPUTE_BLOCKS_SIZE do
        range.cached_nearBlocks_0_0[i] = nil
        --range.cached_nearBlocks_0_1[i] = GenerateNearBlocks(0, 1, i)
    end

    cached_valid_blocks = {}
    battleboxHelper = nil

    range.inited = false
end

-- 返回附近距离为1的格子
local function GetNearBlocks(x, y)

    local rstList = {}
    local count = 0
    local xx
    local yy
    for i = 1,4 do

        xx = x + dx[i];
        yy = y + dy[i];
        if (xx >= 0 and xx < MOVEBLOCK_MAX_X and yy >= 0 and yy < MOVEBLOCK_MAX_Y) then

            count = count + 1
            rstList[count] = {X = xx, Y = yy}
        end
    end
    return rstList
end

--获取距离某格子距离为maxdistance的所有格子
local function GetNearAreaBlocks(x, y, maxdistance)

    if (maxdistance <= 1) then

        return GetNearBlocks(x, y);
    end

    if (maxdistance > MAX_COMPUTE_BLOCKS_SIZE) then
        maxdistance = MAX_COMPUTE_BLOCKS_SIZE;
    end

    local blocks = range.cached_nearBlocks_0_0[maxdistance]

    local rst = {}
    local idx = 0

    for _,b in pairs(blocks) do

        idx = idx + 1
        --rst[idx] = CS.Jyx2.BattleBlockVector(newX, newY)
        rst[idx] = {X = b.X + x, Y = b.Y + y}
    end
    return rst;
end

-- 返回角色技能的施展距离
function range.GetSkillCastBlocks(x, y, coverType, castSize, source)

    local rst = {}
    local idx = 0

    if (coverType == SkillCoverType.LINE) then

        for _,loc in pairs(GetNearBlocks(x, y)) do

            if (IsValidBlock(loc.X, loc.Y)) then
                idx = idx + 1
                rst[idx] = loc
            end
        end

        --central is inaccessible, but need to allow selection for gamepad moves
        idx = idx + 1
        --tmpblk.Inaccessible = true
        rst[idx] = {X = x, Y = y}

        return rst;
    end

    idx = idx + 1
    rst[idx] = {X = x, Y = y}

    if (castSize == 0) then

        return rst;
    end

    for _,loc in pairs(GetNearAreaBlocks(x, y, castSize)) do

        if (IsValidBlock(loc.X, loc.Y)) then
            idx = idx + 1
            rst[idx] = loc
        end
    end
    return rst;
end

return range
