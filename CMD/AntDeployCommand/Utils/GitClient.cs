using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AntDeployCommand.Utils
{
    public class GitClient : IDisposable
    {
        private readonly Action<string,LogLevel> _logger;
        private Repository _repository;
        private readonly string _projectPath;
        public GitClient(string projectPath, Action<string,LogLevel>  logger) 
        {
            _logger = logger;
            _projectPath = projectPath;
            CreateGit(_projectPath);
        }
        
        public GitClient(string projectPath,string envName, Action<string,LogLevel>  logger) 
        {
            _logger = logger;
            _projectPath = projectPath;
            CreateGit(_projectPath,envName);
        }
        

        public GitClient(string projectPath)
        {
            _projectPath = projectPath;
            CreateGit(_projectPath);
        }

        public GitClient(string projectPath,string envName)
        {
            _projectPath = projectPath;
            CreateGit(_projectPath,envName);
        }
        
        public bool InitSuccess { get; private set; }

        //判断git残酷是否合法
        public bool IsGitExist(string path)
        {
            try
            {
                var gitFolder = Repository.Discover(path);
                if (Repository.IsValid(gitFolder))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger?.Invoke(ex.Message,LogLevel.Error);
            }

            return false;
        }

        /// <summary>
        /// 切换分支
        /// </summary>
        /// <returns></returns>
        public bool ChangeBranch(string name)
        {
            try
            {
                var branch = _repository.Branches[name];
                if (branch==null)
                {
                    _logger?.Invoke($"branch name:{name} not exist",LogLevel.Warning);
                    return false;
                }
                Branch currentBranch = LibGit2Sharp.Commands.Checkout(_repository , branch);
                return currentBranch != null;
            }
            catch (Exception e)
            {
                _logger?.Invoke($"checkout branch:{name} fail:{e.Message}",LogLevel.Error);
                return false;
            }
        }
        
        /// <summary>
        /// 创建分支
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool CreateBranch(string name)
        {
            try
            {
                var branch = _repository.Branches[name];
                if (branch!=null)
                {
                    _logger?.Invoke($"create branch warn:{name} is already exist",LogLevel.Info);
                    return true;
                }
                
                Branch newbranch = _repository.CreateBranch(name);
                if (newbranch != null)
                {
                    _logger?.Invoke($"create branch success:{name}",LogLevel.Info);
                    return true;
                }
                _logger?.Invoke($"create branch:{name} fail",LogLevel.Error);
                return false;
            }
            catch (Exception e)
            {
                _logger?.Invoke($"create branch:{name} fail:{e.Message}",LogLevel.Error);
                return false;
            }
        }
        
        /// <summary>
        /// 创建git仓库
        /// </summary>
        /// <param name="path"></param>
        /// <param name="envName"></param>
        /// <returns></returns>
        public bool CreateGit(string path = null,string envName = null)
        {
            try
            {
                //bool doBranch ()
                //{
                //    if (envName == null) return false;
                //    if (CreateBranch(envName))
                //    {
                //        return ChangeBranch(envName);
                //    }
                //    return false;
                //};

                if (string.IsNullOrEmpty(path)) path = _projectPath;
                var path2 = Path.Combine(path, ".git");
                if (Directory.Exists(path2))
                {
                    _logger?.Invoke("git Repository is already created!",LogLevel.Info);
                    _repository = new Repository(_projectPath);
                    InitSuccess =true;// doBranch();//切换成对应的分支
                    return true;
                }

                string rootedPath = Repository.Init(path);
                if (!string.IsNullOrEmpty(rootedPath))
                {
                    _logger?.Invoke("create git Repository success :" + path,LogLevel.Info);
                    _repository = new Repository(_projectPath);

                    CommitChanges("first init");
                    //master分支创建成功
                    InitSuccess =true;// doBranch();//切换成对应的分支
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger?.Invoke("create git repository err:" + ex.Message,LogLevel.Error);
            }

            return false;
        }


        /// <summary>
        /// 提交所有变动
        /// </summary>
        public void SubmitChanges(int count)
        {
            _logger?.Invoke("commit start",LogLevel.Info);
            try
            {
                StageIgnoreFile();

               LibGit2Sharp.Commands.Stage(_repository, "*");
            }
            catch (Exception ex1)
            {
                _logger?.Invoke("stage err:" + ex1.Message,LogLevel.Warning);
            }

            CommitChanges(DateTime.Now.ToString("yyyyMMddHHmms"));
            _logger?.Invoke($"commit success,file count:{count}",LogLevel.Info);
        }

        public void SubmitSelectedChanges(List<string> fileList,string dir)
        {
            _logger?.Invoke("commit start",LogLevel.Info);
            try
            {
                //StageIgnoreFile();

                //LibGit2Sharp.Commands.Stage(_repository, "*");

                foreach (var file in fileList)
                {
                    int length = file.Length - dir.Length;
                    var commitFile = ZipHelper.EntryFromPath(file, dir.Length, length);
                    _repository.Index.Add(commitFile);
                    _repository.Index.Write();
                }
            }
            catch (Exception ex1)
            {
                _logger?.Invoke("stage err:" + ex1.Message,LogLevel.Warning);
            }

            CommitChanges(DateTime.Now.ToString("yyyyMMddHHmms"));
            _logger?.Invoke($"commit success,file count:{fileList.Count}",LogLevel.Info);
        }
        /// <summary>
        /// 提交被忽略的文件列表
        /// </summary>
        private void StageIgnoreFile()
        {
            RepositoryStatus status = _repository.RetrieveStatus();
            List<string> filePaths_Ignored = status.Ignored.Select(mods => mods.FilePath).ToList();

            foreach (var item in filePaths_Ignored)
            {
                _repository.Index.Add(item);
                _repository.Index.Write();
            }

        }

        /// <summary>
        /// 获取有变动的列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetChanges()
        {
            var result = new List<string>();
            try
            {

                RepositoryStatus status = _repository.RetrieveStatus();

                if (!status.IsDirty)
                {
                    _logger?.Invoke("no file changed!",LogLevel.Error);
                    return result;
                }

                List<string> filePaths_Modified = status.Modified.Select(mods => mods.FilePath).ToList();//有修改的文件一览

                List<string> filePaths_Untracked = status.Untracked.Select(mods => mods.FilePath).ToList();//还没有commit过的新文件一览

                List<string> filePaths_Added = status.Added.Select(mods => mods.FilePath).ToList();//新增的文件一览

                List<string> filePaths_Ignored = status.Ignored.Select(mods => mods.FilePath).ToList();



                result.AddRange(filePaths_Modified);
                result.AddRange(filePaths_Untracked);
                result.AddRange(filePaths_Added);
                result.AddRange(filePaths_Ignored);
                result = result.Distinct().ToList();

            }
            catch (Exception ex)
            {
                _logger?.Invoke("Get Changes FileList err:" + ex.Message,LogLevel.Error);
            }
            return result;
        }

        
        
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="commit"></param>
        public void CommitChanges(string commit)
        {
            try
            {
                _repository.Commit(commit, new Signature("antdeploy", "antdeploy@email.com", DateTimeOffset.Now),
                    new Signature("antdeploy", "antdeploy@email.com", DateTimeOffset.Now));
            }
            catch (Exception e)
            {
                _logger?.Invoke("commit err:" + e.Message,LogLevel.Warning);
            }
        }

        public void Dispose()
        {
            this._repository?.Dispose();
        }
    }
}
