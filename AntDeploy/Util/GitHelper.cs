using LibGit2Sharp;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AntDeploy.Util
{
    public class GitHelper : IDisposable
    {
        private readonly Logger _logger;
        private Repository _repository;
        private readonly string _projectPath;
        public GitHelper(string projectPath, Logger logger) : this(projectPath)
        {
            _logger = logger;
        }

        public GitHelper(string projectPath)
        {
            _projectPath = projectPath;
            CreateGit(_projectPath);
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
        /// 创建git仓库
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool CreateGit(string path = null)
        {
            try
            {
                if (string.IsNullOrEmpty(path)) path = _projectPath;
                var path2 = Path.Combine(path, ".git");
                if (Directory.Exists(path2))
                {
                    _logger?.Info("【git】creare git Repository is already created!");
                    _repository = new Repository(_projectPath);
                    InitSuccess = true;
                    return true;
                }

                string rootedPath = Repository.Init(path);
                if (!string.IsNullOrEmpty(rootedPath))
                {
                    _logger?.Info("【git】creare git Repository success :" + path);
                    _repository = new Repository(_projectPath);
                    CommitChanges("init");
                     InitSuccess = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger?.Error("【git】create git repository err:" + ex.Message);
            }

            return false;
        }


        public void SubmitChanges()
        {
            try
            {
                LibGit2Sharp.Commands.Stage(_repository, "*");
            }
            catch (Exception ex1)
            {
                _logger?.Warn("【git】stage err:" + ex1.Message);
            }

            CommitChanges(DateTime.Now.ToString("yyyyMMddHHmms"));

        }

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


                result.AddRange(filePaths_Modified);
                result.AddRange(filePaths_Untracked);
                result.AddRange(filePaths_Added);
                result = result.Distinct().ToList();

            }
            catch (Exception ex)
            {
                _logger?.Error("【git】Get Changes FileList err:" + ex.Message);
            }
            return result;
        }



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
