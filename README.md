# 金庸群侠传3D重制版

[English HomePage](https://github.com/jynew/jynew/wiki/Heros-of-Jin-Yong-3D-Remastered)

![主宣传图-616-353](https://user-images.githubusercontent.com/7448857/145429032-4cb357f9-077d-4450-acb2-bea62e9910d6.jpg)


[![license](https://img.shields.io/badge/license-%E9%87%91%E7%BE%A43D%E9%87%8D%E5%88%B6%E7%89%88%E7%A4%BE%E5%8C%BA%E5%8D%8F%E8%AE%AE-blue)](https://github.com/jynew/jynew/blob/main/LICENSE)
[![release](https://img.shields.io/badge/release-v0.1%20inner-brightgreen)](https://github.com/jynew/jynew/releases)
[![Unity Version](https://img.shields.io/badge/unity-2020.3.23.f1c1-blue)](https://unity.cn/releases/lts/2020) 
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-blue.svg)](https://github.com/jynew/jynew/pulls) 
![commit](https://img.shields.io/github/last-commit/jynew/jynew)<br>
![Contributors](https://img.shields.io/github/contributors-anon/jynew/jynew) 
![fork](https://img.shields.io/github/forks/jynew/jynew?style=social)
![star](https://img.shields.io/github/stars/jynew/jynew?style=social)
 

[包体下载](https://github.com/jynew/jynew/wiki/%E6%B8%B8%E6%88%8F%E4%B8%8B%E8%BD%BD) | [B站主页](https://space.bilibili.com/1092529660) | [联系我们](mailto://jy_remastered@163.com) | [项目Wiki](https://github.com/jynew/jynew/wiki) | [捐助项目](https://github.com/jynew/jynew/wiki/%E6%8D%90%E5%8A%A9%E9%A1%B9%E7%9B%AE) | [耻辱榜](https://github.com/jynew/jynew/wiki/%E8%80%BB%E8%BE%B1%E6%A6%9C)

金庸群侠传3D重制版是一个回合制战棋战斗开放世界RPG游戏。它是一个非盈利游戏项目，项目目标为重制经典游戏《[金庸群侠传](https://zh.wikipedia.org/wiki/%E9%87%91%E5%BA%B8%E7%BE%A4%E4%BF%A0%E5%82%B3)》（[在线玩DOS原版](https://dos.zczc.cz/games/%E9%87%91%E5%BA%B8%E7%BE%A4%E4%BE%A0%E4%BC%A0/)）并支持后续一系列MOD和二次开发。你可以在PC、MAC或移动手机平台(或其他支持平台)游玩。

> 我们承诺：本项目除了爱好者的自愿捐款以外，不会在任何渠道取得收入。最终的代码、资源、包体都仅供学习使用，请勿用于任何商业目的。一切再度商用均不被本项目允许和授权，如果有任何侵犯您的权益，欢迎与我们取得联系。
> 


## 项目目标

* 使用Unity引擎重制并致敬经典游戏《金庸群侠传》DOS版；
* 后续支持重制《金群》的一系列MOD或二次开发；
* 提供开放的能力给社区，有兴趣贡献力量的朋友可以让这个作品更加完善；
* 提供一个完备的单机游戏结构供有兴趣学习开发游戏的朋友进行参考；

------


## 游戏画面截图

![1 (1)](https://user-images.githubusercontent.com/7448857/144630410-bc1676eb-b548-41ea-ae54-90c72e9d066d.png)
![2 (1)](https://user-images.githubusercontent.com/7448857/144630415-c2c3b37e-6008-49d9-a690-fc25d995f21c.png)
![3 (1)](https://user-images.githubusercontent.com/7448857/144630418-38aa752d-332a-4e2e-a297-959b921c9316.png)
![4 (1)](https://user-images.githubusercontent.com/7448857/144630403-e35a6772-2442-465c-8a23-c1f5ae0037bc.png)
![5 (1)](https://user-images.githubusercontent.com/7448857/144630913-bb59a38f-4cb2-4312-b5e4-6051d38c3a84.png)
![6 (1)](https://user-images.githubusercontent.com/7448857/144630919-b21370e1-0783-417e-99c3-763e9563d06a.png)

## 开发计划

- [ ] 素材资产实现
  - [x] 按照新的风格绘制原版所有的角色立绘
  - [x] remix原版所有的音乐
  - [x] 制作所有角色的模型
  - [x] 制作所有武功动作   
  - [x] 所有场景制作，包括RPG部分和战斗部分 
  - [ ] 依据重制版的风格绘制新的道具图标
  - [ ] 原版开场动画重新实现
  - [x] 结局原画绘制
  - [x] 武功特效调制 
- [x] 风格化的画面渲染效果实现
- [x] 基础系统实现，包括地图、道具、角色、战斗等
- [x] 支持剧情脚本指令系统，支持可视化的剧情脚本编辑系统
- [x] 完整复刻实现金庸群侠传游戏流程，可正常通关
- [x] 支持多端输出，一键打包
- [x] 所有代码和资源全部开源，不依赖闭源库（标准第三方库除外）
- [ ] 优化游戏运行性能和代码质量（考虑使用URP渲染管线）
- [ ] 提供MOD启动器，提供金群MOD开发环境和样例
- [ ] 提供联机战斗对战、ONLINE网络游戏模式的样例和DEMO

## 技术实现简介

* 核心流程使用脚本驱动，目前支持lua和可视化图编辑两种模式，很方便编程扩展指令
* 逻辑配置数据使用ScriptableObject存储，并提供基于ODIN的可视化编辑环境，配置数据在Editor运行时可以所见即所得编辑，不需要重启游戏
* 为战棋模式，本框架提供一套简单的基于贪心算法的AI，易于扩展
* 每个地图为一个场景，地图间可以串接
* 游戏存档使用EasySave3插件
* UI方案使用Unity原生的UGUI
* 使用默认渲染管线，卡通渲染风格（考虑升级到URP管线，尚未完成）
* 动作管理部分大量使用Animancer插件，亦使用unity原生AnimatorController方案
* 资源打包和加载使用Addressable
* 游戏中大量使用基于UniTask的异步编程方案来进行逻辑串接，以及防止大量回调嵌套
* 技能特效使用了不少assetstore上的第三方库，如想使用，还请自行购买


## 项目文档导航

* 查看[开发环境和搭建](https://github.com/jynew/jynew/wiki/1.1%E5%87%86%E5%A4%87%E5%BC%80%E5%A7%8B)来将你本地的游戏环境运行起来！
* 你可以查阅[金庸群侠传3d重制版开发文档](https://github.com/jynew/jynew/wiki)来学习本项目的开发细节，其中包括详细的如何配置启动游戏、脚本修改、系统编程等一系列项目细节。
* 可通过查看[金庸群侠传3d重制版视频操作教程](https://github.com/jynew/jynew/wiki/%E9%87%91%E5%BA%B8%E7%BE%A4%E4%BE%A0%E4%BC%A03d%E9%87%8D%E5%88%B6%E7%89%88%E8%A7%86%E9%A2%91%E6%93%8D%E4%BD%9C%E6%95%99%E7%A8%8B)来学习如何搭建游戏场景、添加和配置技能动作等。
* 本项目非常欢迎社区进行贡献，请阅读[社区贡献指南](https://github.com/jynew/jynew/blob/main/CONTRIBUTING.md)查看具体规则和流程。
* 本项目美术原始文件位于[jynew/jynew_art](https://github.com/jynew/jynew_art)，你可以下载3DMAX等原始文件查看。但请注意：此项目内容同样遵守社区协议。

## 授权声明

* 本项目素材一部分为社区自制，一部分为互联网上取得
* 本项目遵循MIT协议，但[金庸群侠传3D重制版社区素材协议](https://github.com/jynew/jynew/tree/main/COMMUNITY_LICENSE_FOR_JYX2)描述了若干种在本项目中覆盖MIT协议情况，请仔细阅读
* 项目中所包含外部插件源代码、资源和链接库等，如希望在其他场景使用，请务必确认符合其本身授权规范
* 不遵守授权协议的产品和企业、个人都将被[耻辱榜](https://github.com/jynew/jynew/wiki/%E8%80%BB%E8%BE%B1%E6%A6%9C)记录在案

## 项目结构示意图

![image](https://user-images.githubusercontent.com/7448857/118384406-5b3bc680-b638-11eb-9186-8888b90bcc35.png)


## 引用项目

[xlua](https://github.com/Tencent/xLua), [xNode](https://github.com/Siccity/xNode), [UniTask](https://github.com/Cysharp/UniTask), [EasySave3](https://docs.moodkie.com/product/easy-save-3/)

## 参考项目

[kyscpp](https://github.com/scarsty/kys-cpp)

## 特别补充说明
以下插件仅供学习使用，若希望使用它还请自行购买重新导入：<br>
[Odin Inspector](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041)

## 社区

* 开发者Q群 749167552 （入群密码是本代码仓库名称）
* 玩家Q群 480072818

## 感谢支持

[JetBrains Rider](https://www.jetbrains.com/community/opensource/#support)

