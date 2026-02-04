# ImageTagger

## 项目概述
ImageTagger 是一个基于 Windows Forms 的图像标记工具，专为配合 ImageGlass 图像查看器使用而设计。它允许用户在浏览图片时快速将图片分类到自定义的标签组中，并支持后续对这些标记图片进行批量复制或移动操作。该工具通过 ImageGlass Tools SDK 与 ImageGlass 进行深度集成，实现图片路径的实时同步和导航控制。

## 安装指南

### 前置条件
1.  Windows 操作系统 (支持 .NET 环境)。
2.  已安装 [ImageGlass](https://imageglass.org/) 图像查看器。

### 安装步骤
1.  下载 ImageTagger 的最新版本压缩包。
2.  将压缩包解压到任意目录。
3.  建议将 ImageTagger 配置为 ImageGlass 的外部工具，以便快速启动。

## 使用方法

### 启动程序
可以直接运行 `ImageTagger.exe`。如果通过 ImageGlass 启动（例如作为工具栏插件），它会自动加载当前查看的图片。

### 标签管理
*   **添加标签**：在 "Tags" 选项卡中，点击右上角的 "+" 按钮，输入标签名称并确认。
*   **删除标签**：选中列表中的标签，点击右上角的 "-" 按钮，或右键点击标签选择 "Delete"。
*   **管理标签**：右键点击标签列表，可以使用 "Clear" (清空标签内的图片) 或 "Duplicate" (复制标签及其内容)。

### 标记图片
1.  确保 ImageGlass 正在运行并显示图片。
2.  在 ImageTagger 的 "Tagging" 选项卡中，你会看到当前 ImageGlass 中显示的图片路径（如果已同步）。
3.  界面会为每个已创建的标签显示一个按钮。
4.  点击对应的标签按钮，将当前图片添加到该标签中。
5.  添加成功后，ImageGlass 会自动跳转到下一张图片（此行为支持撤销）。

### 批量操作
在 "Tags" 选项卡中选中一个标签，可以使用底部的功能按钮：
*   **Copy to...**：将该标签下的所有图片**复制**到指定文件夹。
*   **Move to...**：将该标签下的所有图片**移动**到指定文件夹（移动成功后会清空标签列表）。
*   **Undo**：撤销上一次的标记或导航操作。

## 配置说明

### 数据存储
所有的标签和图片路径数据存储在程序运行目录下的 `tags.json` 文件中。
*   这是一个标准 JSON 文件，可以手动备份或编辑（请确保格式正确）。

### 窗口行为
*   程序默认保持 "总在最前" (Always on Top)，以便在全屏浏览图片时操作。
*   窗口高度会根据标签数量和日志内容自动调整。

## API参考

本程序主要使用 `ImageGlass.Tools` 库与 ImageGlass 通信。

### 主要类说明
*   `DataManager`：负责 `tags.json` 的序列化与反序列化。
*   `MainForm`：主界面逻辑，包含与 ImageGlass 的连接 (`ImageGlassTool`)、UI 事件处理及自动布局逻辑。
*   `UndoManager`：实现命令模式的撤销栈，支持 `NavigationCommand` (导航) 和 `AddTagCommand` (标记) 的回滚。

## 贡献指南

欢迎提交 Issue 或 Pull Request 来改进此项目。

1.  Fork 本仓库。
2.  创建你的特性分支 (`git checkout -b feature/AmazingFeature`)。
3.  提交你的更改 (`git commit -m 'Add some AmazingFeature'`)。
4.  推送到分支 (`git push origin feature/AmazingFeature`)。
5.  开启一个 Pull Request。

## 许可证信息

本项目采用 MIT 许可证。详情请参阅项目根目录下的 [LICENSE](LICENSE) 文件。