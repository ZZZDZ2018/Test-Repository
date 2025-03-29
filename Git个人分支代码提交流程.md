# 提交代码到个人分支的完整流程

提交代码到个人分支的Git工作流程通常包括以下步骤：

## 1. 准备工作
- **克隆仓库**（如果尚未克隆）：
  ```bash
  git clone <仓库URL>
  cd <仓库目录>
  ```

- **创建并切换到个人分支**：
  ```bash
  git checkout -b your-branch-name
  ```
  或者如果分支已存在：
  ```bash
  git checkout your-branch-name
  ```

## 2. 日常开发流程

1. **拉取最新变更**（避免冲突）：
   ```bash
   git pull origin main  # 或其他基础分支
   ```

2. **进行代码修改** - 在你的编辑器中修改文件

3. **查看变更状态**：
   ```bash
   git status
   ```

4. **添加变更到暂存区**：
   ```bash
   git add <文件名>  # 添加特定文件
   或
   git add .         # 添加所有变更
   ```

5. **提交变更**：
   ```bash
   git commit -m "描述你的变更"
   ```

6. **推送变更到远程个人分支**：
   ```bash
   git push origin your-branch-name
   ```
   如果是第一次推送该分支：
   ```bash
   git push -u origin your-branch-name
   ```

## 3. 可选步骤（根据团队流程）

- **与基础分支同步**（如main）：
  ```bash
  git fetch origin
  git merge origin/main
  ```
  或使用rebase：
  ```bash
  git rebase origin/main
  ```

- **解决冲突**（如果有）

- **再次推送**（特别是在rebase后）：
  ```bash
  git push -f origin your-branch-name  # 注意强制推送的风险
  ```

## 4. 创建合并请求/拉取请求

在GitHub/GitLab等平台上：
1. 导航到你的仓库
2. 选择你的分支
3. 点击"New Pull Request"或"Create Merge Request"
4. 填写请求信息并提交

