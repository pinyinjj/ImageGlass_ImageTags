# Image Tagger

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)

**Image Tagger** 是一个专为 [ImageGlass 9](https://imageglass.org/) 图片浏览器打造的高效分类与整理插件。它通过无缝集成，允许用户在浏览图片时，仅需一次点击即可完成图片的归类标记，并支持后续的一键批量移动或复制，极大地提升了素材整理和图片筛选的效率。

---


## ✨ 核心功能

  **沉浸式标记体验**
    *   **实时同步**：插件自动感知 ImageGlass 当前显示的图片。
    *   **无焦点交互**：窗口保持置顶但不抢夺输入焦点（TopMost + NoActivate），确保你在 ImageGlass 中的快捷键（如缩放、平移）不受干扰。
    *   **自动翻页**：标记图片后，自动模拟键盘“右箭头”操作，切换至下一张图片，形成流畅的“查看 -> 标记 -> 下一张”工作流。

 **灵活的分类管理**
    *   支持动态创建、删除自定义分类标签（如“风景”、“待修图”、“素材”）。
    *   根据分类数量自动调整窗口高度，最大化利用屏幕空间。

  **高效的批量处理**
    *   **一键导出**：支持将某个分类下的所有图片批量 **复制** 或 **移动** 到指定文件夹。
    *   **冲突处理**：自动处理文件名冲突（如 `image.jpg` -> `image (1).jpg`），防止覆盖。

  **数据持久化**
    *   所有分类和标记数据自动保存为 `tags.json`，方便备份与迁移。
    *   智能识别并修正旧版本数据路径。

---


## 环境要求

*   **操作系统**: Windows 10 / 11
*   **运行环境**: [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
*   **宿主程序**: ImageGlass 9.0 或更高版本

---

## 🛠 编译指南

1.  **克隆仓库**
    ```bash
    git clone https://github.com/pinyinjj/ImageGlass_ImageTags.git
    cd ImageGlass_ImageTags
    ```

2.  **执行编译**
    使用 .NET CLI 进行发布模式编译：
    ```bash
    dotnet build -c Release
    ```

3.  **获取产物**
    编译成功后，可执行文件位于：
    `bin/Release/net8.0-windows/ImageTagger.exe`

---

## 📦 安装与配置

将编译好的 `ImageTagger` 集成到 ImageGlass 中：

1.  打开 ImageGlass，进入 **设置 (Settings)** -> **工具 (Tools)**。
2.  点击 **添加 (Add)** 按钮创建新工具。
3.  填写以下配置：
    *   **Name (名称)**: `Image Tagger` (或你喜欢的名字)
    *   **Command (命令)**: 浏览并选择你编译生成的 `ImageTagger.exe` 路径。
    *   **Argument (参数)**: `<file>`
        *   *注意：这会将当前图片路径作为启动参数传递。*
    *   **Hotkeys (快捷键)**: 建议设置一个顺手的快捷键，例如 `Ctrl+T`。
    *   **勾选选项**: ✅ **Integrated with ImageGlass.Tools**
        *   *重要：必须勾选此项，插件才能通过管道与 ImageGlass 通信。*
4.  点击 **Apply** 保存。

---

## 📖 使用手册

### 1. 初始化分类
*   启动插件（通过 ImageGlass 工具栏或快捷键）。
*   切换到 **"Category Management" (分类管理)** 标签页。
*   在输入框中输入分类名称（例如 "Wallpapers"），点击 **Add**。
*   建议预先建立好所有常用分类。

### 2. 开始标记
*   在 ImageGlass 中浏览图片。
*   点击插件界面上的 **"Add to [分类名]"** 按钮。
*   **效果**：
    1.  当前图片路径被记录到该分类。
    2.  按钮上显示的计数加 1。
    3.  ImageGlass 自动切换到下一张图片。

### 3. 导出整理结果
*   整理完成后，回到 **"Category Management"** 标签页。
*   选中一个分类。
*   点击 **Copy (复制)** 或 **Move (移动)**。
*   选择目标文件夹，程序将自动开始传输文件，并报告成功数量。

---

## ❓ 常见问题

**Q: 插件启动后显示 "No image loaded"？**
A: 请确保在 ImageGlass 工具设置中勾选了 **"Integrated with ImageGlass.Tools"**。如果问题依旧，尝试在 ImageGlass 中手动切换一张图片，插件应会自动同步。

**Q: 为什么我按了按钮，图片没有自动切换？**
A: 插件通过模拟键盘按键实现翻页。请确保 ImageGlass 没有被最小化，且系统未拦截按键模拟。

**Q: `tags.json` 文件在哪里？**
A: 该文件位于 `ImageTagger.exe` 同级目录下。你可以随时备份此文件以保存你的整理进度。

---

## 📜 许可证

本项目采用 [MIT License](LICENSE) 授权，欢迎自由修改与分发。
