# GitHub 上传指南

## 步骤 1: 提交当前更改

首先，我们需要提交当前的更改：

```bash
# 添加所有更改的文件
git add .

# 提交更改
git commit -m "添加角色拖动功能和攻击系统"
```

## 步骤 2: 在 GitHub 上创建新仓库

1. 访问 https://github.com
2. 点击右上角的 "+" 按钮，选择 "New repository"
3. 填写仓库信息：
   - Repository name: `HeMetrics` (或你喜欢的名字)
   - Description: 可选，描述你的项目
   - 选择 Public 或 Private
   - **不要**勾选 "Initialize this repository with a README"（因为本地已有代码）
4. 点击 "Create repository"

## 步骤 3: 配置远程仓库并推送

创建仓库后，GitHub 会显示仓库 URL，类似：
- HTTPS: `https://github.com/你的用户名/HeMetrics.git`
- SSH: `git@github.com:你的用户名/HeMetrics.git`

然后执行以下命令：

```bash
# 删除旧的远程仓库配置（如果存在）
git remote remove HeMetrics

# 添加新的 GitHub 远程仓库（使用 HTTPS，替换为你的实际 URL）
git remote add origin https://github.com/你的用户名/HeMetrics.git

# 或者使用 SSH（如果你配置了 SSH key）
# git remote add origin git@github.com:你的用户名/HeMetrics.git

# 推送到 GitHub
git push -u origin master
```

如果 GitHub 默认分支是 `main`，使用：
```bash
git push -u origin master:main
```

## 步骤 4: 验证

访问你的 GitHub 仓库页面，确认代码已成功上传。

## 注意事项

- 确保 `.gitignore` 文件已正确配置（已检查，配置正确）
- Unity 项目的大文件（Library、Temp 等）会被自动忽略
- 如果推送时遇到认证问题，可能需要配置 GitHub Personal Access Token

