# Traffic2

## 项目简介

本项目模拟了十字路口交通调度的过程。

### 项目目的

- 体会理解多线程概念，编写异步程序
- 掌握调度机制
- 熟悉任务间同步机制
- 理解信号量机制

### 项目功能

项目实现的功能如下：

- **交通信号灯的交替红绿显示**

	在东西南北四个方向设计了四栈红绿灯，其中东西方向同步，南北方向同步，间隔8秒交替。

- **普通车辆正常通行及等待**

	绿灯方向的车辆可以正常通行，而红灯方向的车辆需要等待。

- **特种车辆闯红灯的实现**

	红灯方向的特种车辆若前方没有普通车辆遮挡，则可以抢占路口资源，实现闯红灯的效果。

### 项目环境

- **开发环境**

	C#.NET Framework 4.7.2 Windows窗体应用程序

	Windows11操作系统

- **运行环境**

	请在Windows7及以上版本运行，于Windows11运行最佳

- **运行方法**

	打开Traffic2.exe即可运行本项目

## 功能实现

### 实体介绍

本项目主要分为三个实体：道路、交通信号灯和车辆

- **道路**（Road）类：

道路实体较为简单，在界面中心对称呈现四条道路，每个道路为两个车道。

<img src="./assets/image-20240513182432775.png" alt="image-20240513182432775" style="zoom: 33%;" />

- **交通信号灯**（TrafficLightControl）类

每个方向各有一个交通信号灯，分别可以呈现红绿两种颜色，其中南北方向的信号灯同步，东西方向取反。红色表示不可通行，绿色表示可以通行。每隔8秒颜色会交替。

![image-20240513183045302](./assets/image-20240513183045302.png)![image-20240513183102654](./assets/image-20240513183102654.png)

- **车辆**（CarBase）类

车辆采用面向对象的方式设计，CarBase类为抽象基类，实现基本的方法；Car类和SpecialCar类继承自CarBase类，Car类为普通车辆，SpecialCar类为特种车辆；PoliceCar（警车）类，Ambulance（救护车）类，FireEngine（消防车）类继承自SpecialCar类。

| Car                                                          | PoliceCar                                                    | Ambulance                                                    | FireEngine                                                   |
| ------------------------------------------------------------ | ------------------------------------------------------------ | ------------------------------------------------------------ | ------------------------------------------------------------ |
| ![image-20240513183927227](./assets/image-20240513183927227.png) | ![image-20240513183759593](./assets/image-20240513183759593.png) | ![image-20240513183838578](./assets/image-20240513183838578.png) | ![image-20240513183850972](./assets/image-20240513183850972.png) |

对于车辆基类而言，他们将被赋予不同的随机颜色，有基本的行驶（Drive）方法，在执行行驶方法后将利用计时器模拟播放行驶动画。

### 系统介绍

- 交通信号灯系统（TrafficLightSystem）类

	- 在初始化界面时构造一个实例，将包含四个方向交通信号灯实例，一个方向映射字典。

	- 利用计时器设定每8秒切换可通行方向。

- 车道（Lane）类

	- 每个Lane的实例将包含一个车辆（CarBase）的队列，用于管理车辆。

	- 在初始化时利用EnterLane方法插入车辆实例。

	- 在该方向可通行时执行ExitLane方法，让车辆执行行驶动作，并退出队列。

- 车道系统（TrafficLaneSystem）类

	- 在初始化界面时构造一个实例，将包含各个方向的Lane的实例，一个方向与车道的映射字典。

	- 初始化界面时将执行PutCar方法，将Lane实例中包含的车辆队列中车辆绘画到窗口的道路上，其中CalculateCarPositions方法将负责车辆的初始化位置。

	- 初始化界面之后该实例将循环插入四个方向的异步行驶任务（DriveTask方法）。

	- 在每个异步行驶任务中将利用IsAbleToPass方法当前方向的车道能否通行并在结果为True时执行车辆的行驶操作，否则将等待50毫秒以减少CPU负担。

- Form1类

	- Form1类包含一个TrafficLightSystem的实例和一个TrafficLaneSystem类的实例（实际这两个类可以被作为静态类，限于时间没有修改）以及两个方向的信号量（依据我个人的算法应该只需要一个信号量即可，限于时间没有修改）。

	- Form1类将呈现整个界面，并作为主线程分支其他线程。

	- 程序开始执行后，Form1类的构造函数首先执行界面初始化（InitGUI）方法，包括初始化TrafficLightSystem和TrafficLaneSystem实例，初始化显示交通信号灯，初始化道路；界面初始化完成后将插入四个方向的行驶任务，异步进行。

		![image-20240513193706046](./assets/image-20240513193706046.png)

## 调度算法

通过信号量机制实现可通行方向的车辆与不可通行方向的特种车辆的竞争。

四个方向的线程将每个50毫秒检测该方向是否可通行，若可通行，则抢占路口资源（抢占信号量），并让该方向的车辆通行，然后将释放信号量，重新抢占。由于设计时车辆在开始行驶后便异步执行动画，



