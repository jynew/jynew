# 金庸群侠传3D重制版

![coverage](https://img.shields.io/badge/coverage-80%25-yellowgreen)
[![license](https://img.shields.io/badge/license-MIT%2B%E9%87%91%E7%BE%A43D%E9%87%8D%E5%88%B6%E7%89%88%E7%A4%BE%E5%8C%BA%E5%8D%8F%E8%AE%AE-blue)](https://github.com/jynew/jynew/blob/main/LICENSE)
[![release](https://img.shields.io/badge/release-v0.1%20inner-brightgreen)](https://github.com/jynew/jynew/releases)
[![Unity Version](https://img.shields.io/badge/unity-2020.3.9.f1c1-blue)](https://unity.cn/releases/lts/2020) 
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-blue.svg)](https://github.com/jynew/jynew/pulls) 
![commit](https://img.shields.io/github/last-commit/jynew/jynew)<br>
![Contributors](https://img.shields.io/github/contributors-anon/jynew/jynew) 
![fork](https://img.shields.io/github/forks/jynew/jynew?style=social)
![star](https://img.shields.io/github/stars/jynew/jynew?style=social)




* 项目主页 http://www.jy-new.com
* B站主页 https://space.bilibili.com/1092529660
* 联系我们 jy_remastered@163.com
* 开发者Q群 749167552 （入群密码是本代码仓库名称）

金庸群侠传3D重制版是一个非盈利游戏项目，重制经典游戏《金庸群侠传》并支持后续一系列MOD和二次开发。

> 我们承诺：除了爱好者的自愿捐款以外，不会在任何渠道取得收入，游戏亦不会在任何游戏平台进行发布。最终的代码、资源、包体都仅供学习使用，请勿用于任何商业目的。一切再度商用均不被本项目允许和授权，如果有任何侵犯您的权益，欢迎与我们取得联系。

## 开发中画面

![image](https://user-images.githubusercontent.com/7448857/118384457-aa81f700-b638-11eb-972b-810a88040939.png)
![image](https://user-images.githubusercontent.com/7448857/118384458-b2419b80-b638-11eb-8411-8822289759b4.png)
![image](https://user-images.githubusercontent.com/7448857/118384459-b53c8c00-b638-11eb-8a83-80228747067f.png)
![image](https://user-images.githubusercontent.com/7448857/118384466-b968a980-b638-11eb-89b3-11aec9ee8bd2.png)

## 项目目标

* 使用Unity引擎重制并致敬经典游戏《金庸群侠传》DOS版；
* 后续支持重制《金群》的一系列MOD，或二次开发；
* 提供开放的的能力给社区，有兴趣贡献力量的朋友可以让这个作品更加完善；
* 供有兴趣学习开发游戏的朋友进行参考；

## 开发环境和搭建

* 开发工具：[Unity 2020.3.9.f1c1(LTS) (64-bit)](https://unity.cn/releases/lts/2020)
* 直接点击播放按钮左方的"P"按钮启动游戏（或切换启动场景为0_GameStart.unity点击播放）

## 我如何开始？

1、查看视频教程

* [场景编辑：如何配置一个门？（控制物体显示/隐藏）](https://www.bilibili.com/video/BV1mz4y117j3?p=1)
* [场景编辑：典型的配置一段剧情的方式](https://www.bilibili.com/video/BV1mz4y117j3?p=2)
* [场景编辑：制作寻路网格，烘焙Navimesh](https://www.bilibili.com/video/BV1mz4y117j3?p=3)
* [场景编辑：配置和刷出NPC](https://www.bilibili.com/video/BV1mz4y117j3?p=4)
* [场景编辑：调整场景中的人物](https://www.bilibili.com/video/BV1mz4y117j3?p=6)  【部分内容已过期】
* [场景编辑：调整场景中的人物动作](https://www.bilibili.com/video/BV1mz4y117j3?p=13)
* [战斗编辑：生成格子](https://www.bilibili.com/video/BV1mz4y117j3?p=7)
* [战斗编辑：配置和测试一场战斗](https://www.bilibili.com/video/BV1mz4y117j3?p=8)
* [资产配置：导入模型并且调整风格化渲染参数](https://www.bilibili.com/video/BV1mz4y117j3?p=5)
* [资产配置：导入动作](https://www.bilibili.com/video/BV1mz4y117j3?p=9)
* [资产配置：战斗动作和技能](https://www.bilibili.com/video/BV1mz4y117j3?p=11)
* [资产配置：战斗技能编辑和预览](https://www.bilibili.com/video/BV1mz4y117j3?p=12)


2、系统的阅读文档深入了解

### 通用篇
* [1.1准备开始](https://github.com/jynew/jynew/wiki/1.1%E5%87%86%E5%A4%87%E5%BC%80%E5%A7%8B)
* [1.2游戏运行机制](https://github.com/jynew/jynew/wiki/1.2%E6%B8%B8%E6%88%8F%E8%BF%90%E8%A1%8C%E6%9C%BA%E5%88%B6)
* [1.3事件触发器](https://github.com/jynew/jynew/wiki/1.3%E4%BA%8B%E4%BB%B6%E8%A7%A6%E5%8F%91%E5%99%A8)
* [1.4Lua脚本](https://github.com/jynew/jynew/wiki/1.4Lua%E8%84%9A%E6%9C%AC)
* [1.5搭建游戏世界差异解决办法](https://github.com/jynew/jynew/wiki/1.5%E6%90%AD%E5%BB%BA%E6%B8%B8%E6%88%8F%E4%B8%96%E7%95%8C%E5%B7%AE%E5%BC%82%E8%A7%A3%E5%86%B3%E5%8A%9E%E6%B3%95)
* [1.6Git使用流程简介](https://github.com/jynew/jynew/wiki/1.6Git%E6%8B%89%E5%8F%96%EF%BC%8C%E6%8F%90%E4%BA%A4%EF%BC%8C%E6%8E%A8%E9%80%81%E6%95%99%E7%A8%8B)

### 游戏内工具篇
* [2.1技能编辑器（可预览模型、动作、武器挂点、技能特效等）](https://github.com/jynew/jynew/wiki/2.1%E6%8A%80%E8%83%BD%E7%BC%96%E8%BE%91%E5%99%A8)
* [2.2控制台](https://github.com/jynew/jynew/wiki/2.2%E6%8E%A7%E5%88%B6%E5%8F%B0)
* [2.3存档生成](https://github.com/jynew/jynew/wiki/2.3%E5%AD%98%E6%A1%A3%E7%94%9F%E6%88%90)
* [2.4战斗调试](https://github.com/jynew/jynew/wiki/2.4%E6%88%98%E6%96%97%E8%B0%83%E8%AF%95)
* [2.5角色模型配置管理器](https://github.com/jynew/jynew/wiki/2.5%E8%A7%92%E8%89%B2%E6%A8%A1%E5%9E%8B%E9%85%8D%E7%BD%AE%E7%AE%A1%E7%90%86%E5%99%A8)

### 程序篇
* [3.1项目代码总览](https://github.com/jynew/jynew/wiki/3.1%E9%A1%B9%E7%9B%AE%E4%BB%A3%E7%A0%81%E6%80%BB%E8%A7%88)
* 3.2资源加载管理机制(TODO)
* 3.3配置表加载和读取（TODO）
* 3.4存档数据结构（TODO）
* 3.5Lua虚拟机（TODO）
* 3.6地图流程说明（TODO）
* 3.7战斗流程说明（TODO）

### 艺术和资产篇
* 4.1艺术和资产概述（TODO）
* [4.2人物立绘制作标准](https://github.com/jynew/jynew/wiki/4.2%E4%BA%BA%E7%89%A9%E7%AB%8B%E7%BB%98%E5%88%B6%E4%BD%9C%E6%A0%87%E5%87%86)
* [4.3人物三维模型和动作制作标准](https://github.com/jynew/jynew/wiki/4.3%E4%BA%BA%E7%89%A9%E5%8A%A8%E4%BD%9C%E5%88%B6%E4%BD%9C%E6%A0%87%E5%87%86)
* 4.4技能特效制作标准（TODO）
* 4.5三维场景和道具制作标准（TODO）
* 4.6UI标准（TODO）
* 4.7音频标准（TODO）

### 项目篇
* 5.1我们如何制定ROADMAP(TODO)
* 5.2如何协作和参与开发(TODO)

## 贡献给项目

本项目非常欢迎社区进行贡献，请阅读[社区贡献指南](https://github.com/jynew/jynew/blob/main/CONTRIBUTING.md)查看具体规则和流程。

## 授权声明

* 本项目素材一部分为社区自制，一部分为互联网上取得
* 本项目遵循MIT协议，但[金庸群侠传3D重制版社区素材协议](https://github.com/jynew/jynew/tree/main/COMMUNITY_LICENSE_FOR_JYX2)描述了若干种在本项目中覆盖MIT协议情况，请仔细阅读
* 项目中所包含外部插件源代码、资源和链接库等，如希望在其他场景使用，请务必确认符合其本身授权规范


## 项目结构示意图

![image](https://user-images.githubusercontent.com/7448857/118384406-5b3bc680-b638-11eb-9186-8888b90bcc35.png)


## 引用项目

[xlua](https://github.com/Tencent/xLua)

## 参考项目

[kyscpp](https://github.com/scarsty/kys-cpp)

## 特别补充说明

以下插件仅供学习使用，若希望使用它还请自行购买重新导入：<br>
[Odin Inspector](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041)
