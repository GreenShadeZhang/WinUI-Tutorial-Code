## WASDK（WinUI 3）从入门到干活——开发上位机软件

### WASDK（WinUI 3）从入门到干活——开发上位机软件（一）序章

+ 感谢大家的支持
+ 出这个系列视频的初衷（为了让人学会自己开发软件功能）
+ 技术栈的讲解
+ 以及针对这个系列可以征集大家的意见
+ 希望大家能够学有所成

### WASDK（WinUI 3）从入门到干活——开发上位机软件（二）环境配置和电子脑壳项目运行
+ VS的下载和对应负载的安装
+ Xaml Style下载
+ 项目模板的的下载
+ 一些VS插件的下载
+ 对应电子脑壳源码的下载
+ 通过运行电子脑壳让大家有个信心

### WASDK（WinUI 3）从入门到干活——开发上位机软件（三）学习WASDK需要的前置知识
+ 了解什么是.NET框架
+ C#语言和.NET框架的关系
+ 面向对象一些基本概念和基本编程思想
+ [C# 文档](https://learn.microsoft.com/zh-cn/dotnet/csharp/)
+ [.NET 是用于构建新式应用和强大的云服务的免费开放源代码跨平台框架。](https://dotnet.microsoft.com/zh-cn/)

+ [学习WASDK（WinUI 3）要掌握的c#知识](https://github.com/GreenShadeZhang/WinUI-Tutorial-Code/blob/master/docs.md)

+ [Windows 应用 SDK](https://learn.microsoft.com/zh-cn/windows/apps/windows-app-sdk/)

### WASDK（WinUI 3）从入门到干活——开发上位机软件（四）XAML讲解和简单布局练习

+ [XAML 概述](https://learn.microsoft.com/zh-cn/windows/uwp/xaml-platform/xaml-overview)

+ [学习WASDK（WinUI 3）要掌握的XAML知识有哪些](https://github.com/GreenShadeZhang/WinUI-Tutorial-Code/blob/master/docs.md)

+ 一个简单的Windows App SDK的编译之后样子（了解XAML和CS之间的关系）

+ 简单的布局练习

+ [WinUI 3 Gallery](https://github.com/microsoft/WinUI-Gallery)

+ [WindowsCommunityToolkit](https://github.com/CommunityToolkit/WindowsCommunityToolkit)




### 学习WASDK（WinUI 3）要掌握的c#知识

GitHub Copilot: 以下是学习 Windows App SDK 必须要掌握的 C# 知识大纲：

1. C# 基础语法：包括变量、数据类型、运算符、控制流语句等基本语法知识。
2. 面向对象编程：包括类、对象、继承、多态等面向对象编程的基本概念和语法。
3. 异常处理：包括 try-catch-finally 语句、throw 语句等异常处理的基本语法。
4. 泛型编程：包括泛型类、泛型方法等泛型编程的基本概念和语法。
5. LINQ：包括 LINQ to Objects、LINQ to XML 等 LINQ 的基本概念和语法。
6. Windows App SDK 相关知识：包括应用程序生命周期、应用程序模型、应用程序 UI 设计等 Windows App SDK 相关的知识。

关键字解释：
- 变量：用于存储数据的内存空间。
- 数据类型：用于定义变量的类型，包括整型、浮点型、布尔型等。
- 运算符：用于进行数学运算、逻辑运算等操作的符号。
- 控制流语句：用于控制程序执行流程的语句，包括 if-else 语句、for 循环语句等。
- 类：用于封装数据和方法的模板。
- 对象：类的实例化，具有类的属性和方法。
- 继承：子类继承父类的属性和方法。
- 多态：同一种方法可以有不同的实现方式。
- 异常处理：用于处理程序运行时出现的异常情况。
- 泛型类：具有泛型参数的类。
- 泛型方法：具有泛型参数的方法。
- LINQ：Language Integrated Query，用于查询和操作数据的语言集成查询。
- 应用程序生命周期：应用程序从启动到关闭的整个过程。
- 应用程序模型：应用程序的架构模型。
- 应用程序 UI 设计：应用程序的用户界面设计。 

```csharp
// 示例代码
using System;

namespace MyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // 变量和数据类型
            int num = 10;
            double pi = 3.14;
            bool isTrue = true;

            // 控制流语句
            if (num > 0)
            {
                Console.WriteLine("num is positive");
            }
            else
            {
                Console.WriteLine("num is negative");
            }

            // 类和对象
            class Person
            {
                public string Name { get; set; }
                public int Age { get; set; }
            }

            Person person = new Person();
            person.Name = "Tom";
            person.Age = 20;

            // 异常处理
            try
            {
                int result = 10 / 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // LINQ
            int[] numbers = { 1, 2, 3, 4, 5 };
            var query = from num in numbers
                        where num % 2 == 0
                        select num;

            foreach (var num in query)
            {
                Console.WriteLine(num);
            }
        }
    }
}
```

### 学习WASDK（WinUI 3）要掌握的XAML知识有哪些

GitHub Copilot: 要学习WASDK（WinUI）开发，需要掌握以下XAML知识：

1. XAML语法：XAML是一种基于XML的标记语言，用于定义用户界面和应用程序行为。学习XAML语法是理解WASDK开发的基础。

2. 布局：WASDK应用程序的布局是通过XAML中的布局控件来实现的。例如，Grid、StackPanel、RelativePanel等。

3. 控件：WASDK应用程序中的控件是通过XAML中的控件元素来定义的。例如，Button、TextBox、ListView等。

4. 样式和模板：WASDK应用程序中的控件可以通过样式和模板来自定义外观和行为。学习如何创建和应用样式和模板是非常重要的。

5. 数据绑定：WASDK应用程序中的控件可以通过数据绑定来显示和编辑数据。学习如何使用数据绑定是非常重要的。

6. 动画：WASDK应用程序中的控件可以通过动画来实现视觉效果。学习如何创建和应用动画是非常重要的。

7. 事件处理：WASDK应用程序中的控件可以通过事件处理程序来响应用户交互。学习如何创建和应用事件处理程序是非常重要的。

8. 自定义控件：WASDK应用程序中的控件可以通过自定义控件来实现特定的功能。学习如何创建和使用自定义控件是非常重要的。

以上是学习WASDK开发所需掌握的XAML知识的一些重要方面。
