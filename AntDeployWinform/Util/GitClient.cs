using LibGit2Sharp;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AntDeployWinform.Util
{
    public class GitClient : IDisposable
    {
        private readonly Logger _logger;
        private Repository _repository;
        private readonly string _projectPath;
        public GitClient(string projectPath, Logger logger) 
        {
            _logger = logger;
            _projectPath = projectPath;
            CreateGit(_projectPath);
        }
        
        public GitClient(string projectPath,string envName, Logger logger) 
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
                _logger?.Error("【git】" + ex.Message);
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
                    _logger?.Warn($"【git】branch name:{name} not exist");
                    return false;
                }
                Branch currentBranch = LibGit2Sharp.Commands.Checkout(_repository , branch);
                return currentBranch != null;
            }
            catch (Exception e)
            {
                _logger?.Error($"【git】checkout branch:{name} fail:{e.Message}");
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
                    _logger?.Info($"【git】create branch warn:{name} is already exist");
                    return true;
                }
                
                Branch newbranch = _repository.CreateBranch(name);
                if (newbranch != null)
                {
                    _logger?.Info($"【git】create branch success:{name}");
                    return true;
                }
                _logger?.Error($"【git】create branch:{name} fail");
                return false;
            }
            catch (Exception e)
            {
                _logger?.Error($"【git】create branch:{name} fail:{e.Message}");
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
                    _logger?.Info("【git】 git Repository is already created!");
                    _repository = new Repository(_projectPath);
                    InitSuccess =true;// doBranch();//切换成对应的分支
                    return true;
                }

                string rootedPath = Repository.Init(path);
                if (!string.IsNullOrEmpty(rootedPath))
                {
                    _logger?.Info("【git】create git Repository success :" + path);
                    _repository = new Repository(_projectPath);

                    CommitChanges("first init");
                    //master分支创建成功
                    InitSuccess =true;// doBranch();//切换成对应的分支
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger?.Error("【git】create git repository err:" + ex.Message);
            }

            return false;
        }


        /// <summary>
        /// 提交所有变动
        /// </summary>
        public void SubmitChanges(int count)
        {
            _logger?.Info("【git】commit start");
            try
            {
                StageIgnoreFile();

               LibGit2Sharp.Commands.Stage(_repository, "*");
            }
            catch (Exception ex1)
            {
                _logger?.Warn("【git】stage err:" + ex1.Message);
            }

            CommitChanges(DateTime.Now.ToString("yyyyMMddHHmms"));
            _logger?.Info($"【git】commit success,file count:{count}");
        }

        public void SubmitSelectedChanges(List<string> fileList,string dir)
        {
            _logger?.Info("【git】commit start");
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
                _logger?.Warn("【git】stage err:" + ex1.Message);
            }

            CommitChanges(DateTime.Now.ToString("yyyyMMddHHmms"));
            _logger?.Info($"【git】commit success,file count:{fileList.Count}");
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
                    _logger?.Error("【git】no file changed!");
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
                _logger?.Error("【git】Get Changes FileList err:" + ex.Message);
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
                _logger?.Warn("【git】commit err:" + e.Message);
            }
        }

        public void Dispose()
        {
            this._repository?.Dispose();
        }
    }
}
