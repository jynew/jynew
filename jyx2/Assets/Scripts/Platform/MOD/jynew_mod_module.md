# jynew MOD模块设计

知大虾  2022/12/03  CREATE

------------

原有的MOD模块在载入和管理上有点混乱，并且扩展性不好。现在将其逻辑重构如下：

每个MOD包含三个文件：
- {mod_id}.xml  MOD描述文件

    其中MOD描述文件中包含字段
        id                MOD的id
-       name              MOD名字
        author            作者
        version           MOD版本
        client_ver        匹配的客户端主体版本
        mod_md5           MOD主体文件MD5
        maps_md5          地图文件MD5
        create_date       打包时间


- {mod_id}_mod        MOD主体文件
- {mod_id}_maps       MOD地图资源文件


## GameModNative
原生打包自带MOD（PC、安卓、IOS）
来自StreamingAssets，随包发布
将在MOD列表中置顶显示，并标记 [原生] 标签

## GameModManualInstalled
通过外部导入（PC、安卓、IOS）
支持文件、HTTP，逻辑为拷贝并解压缩到 persistDataPath下
将标记 [手动安装] 标签

## GameModWorkshop
通过Steam创意工坊安装（仅PC-Steam渠道）
直接读取创意工坊安装的MOD文件
将标记 [创意工坊] 标签

## GameModEditor
直接在编辑器代码中引用（仅Editor下）
标记 [Editor] 标签