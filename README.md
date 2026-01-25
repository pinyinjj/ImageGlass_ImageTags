# ImageGlass Image Tagger

一个专为 [ImageGlass 9](https://imageglass.org/) 设计的轻量级图片分类管理插件。它允许用户在浏览图片时，通过一键点击将图片归类到自定义标签中，并支持后续的批量移动或复制操作。

---

## 1. 项目概述
ImageTagger 旨在解决大量图片快速筛选与分类的痛点。通过与 ImageGlass 9 的深度集成，本工具可以实时感知当前显示的图片，并提供一个动态生成的快捷操作面板，极上地提高了图片整理的效率。

## 2. 技术架构与功能模块实现

本项目采用 **C# / .NET 8.0 Windows Forms** 开发，核心架构基于“事件驱动 + 主动请求”的双向通信模型。

### 核心功能模块划分：

#### A. SDK 集成与通信模块 (`ImageGlass.Tools`)
*   **实现原理**：通过 Named Pipes（命名管道）与 ImageGlass 主程序通信。
*   **双向同步**：
    *   **被动接收**：订阅 `ToolMessageReceived` 事件，使用官方 SDK 的 `IgImageLoadedEventArgs` 解析 JSON 数据，获取当前图片的 `FilePath`。
    *   **主动请求（技术亮点）**：由于官方 SDK 未提供主动获取路径的 API，本项目利用 **Reflection（反射）** 技术访问 `ImageGlassTool` 的私有 `_client` 字段，发送 `igtool.request.get_image` 指令，确保在任何情况下（如插件中途启动）都能精准找回当前图片。

#### B. 动态 UI 渲染引擎 (`MainForm.Layout`)
*   **自适应缩放**：实现了 `AdjustFormHeight` 逻辑。程序会根据分类数量动态计算窗口高度，并根据屏幕分辨率进行自适应（最高占用屏幕 90% 空间）。
*   **动态按钮生成**：基于 `DataManager` 中的分类数据，实时在 `FlowLayoutPanel` 中创建操作按钮。
*   **实时计数**：每个按钮和列表项都会动态显示 `分类名 - 数量`，反馈极其直观。

#### C. 数据持久化模块 (`DataManager`)
*   **存储机制**：使用 `System.Text.Json` 将分类及图片路径列表序列化为 `tags.json`。
*   **数据兼容性**：具备自动转换逻辑，能够识别并迁移旧版本的 JSON 数据格式。

#### D. 远程控制模块 (`WinApi`)
*   **按键注入**：利用 `user32.dll` 的 `keybd_event` API，在添加图片后自动向 ImageGlass 发送 `Right Arrow` 指令，实现“标记 -> 下一张”的流畅体验。
*   **焦点管理**：通过 `SWP_NOACTIVATE` 标志强制窗口置顶（TopMost），在保持可见性的同时不抢夺 ImageGlass 的输入焦点。

---

## 3. 核心功能
*   **一键分类**：点击对应分类按钮，立即将当前图片路径存入该标签。
*   **快速翻页**：插件内置 Prev/Next 按钮，可直接控制 ImageGlass 切换图片。
*   **自动跳转**：成功标记后可配置自动切换至下一张图片。
*   **分类管理**：支持实时增加、删除分类标签。
*   **批量处理**：支持将某个分类下的所有图片一键 **复制** 或 **移动** 到指定文件夹，并具备重名冲突自动处理功能。

## 4. 技术栈
*   **语言**：C# 12.0
*   **框架**：.NET 8.0 (Windows Forms)
*   **SDK**：ImageGlass.Tools 1.9200.2
*   **API**：Windows API (user32.dll)

## 5. 安装部署

### 环境要求
*   Windows 10/11
*   .NET 8.0 Runtime
*   ImageGlass 9.0 或更高版本

### 编译与运行
1.  克隆仓库：`git clone https://github.com/pinyinjj/ImageGlass_ImageTags.git`
2.  进入目录：`cd ImageTagger`
3.  编译项目：`dotnet build -c Release`
4.  在 `bin/Release/net8.0-windows/` 目录下找到 `ImageTagger.exe`。

### 集成到 ImageGlass
1.  打开 ImageGlass 设置 -> **工具 (Tools)**。
2.  点击 **添加 (Add)**。
3.  **名称**：Image Tagger
4.  **可执行文件**：选择生成的 `ImageTagger.exe`。
5.  **参数**：`<file>`
6.  **重要**：勾选 **"Integrated with ImageGlass.Tools"**。

---

## 6. 使用方法
1.  在 ImageGlass 中查看图片时，点击工具栏中的 `Image Tagger` 图标（或使用快捷键）。
2.  在插件的“分类管理”页签中创建你的第一个标签（如：风景、人物）。
3.  切换回“图片操作”页签，点击对应的 `Add to ...` 按钮。
4.  整理完成后，在“分类管理”中点击 `Copy` 或 `Move` 导出你的战果。

## 7. 贡献指南
欢迎提交 Issue 或 Pull Request 来完善此工具。
*   在修改 UI 时，请注意 `AdjustFormHeight` 的布局逻辑。
*   保持对 `ImageGlass.Tools` SDK 规范的遵循。

## 8. 参考资料
*   **官方文档**：[ImageGlass Tools 官方构建指南](https://imageglass.org/docs/build-tools-for-imageglass)
*   **SDK 仓库**：[ImageGlass.Tools GitHub](https://github.com/ImageGlass/ImageGlass.Tools)
*   **开发者中心**：[ImageGlass 官网](https://imageglass.org/)

## 9. 许可证
本项目采用 [MIT License](LICENSE) 许可。