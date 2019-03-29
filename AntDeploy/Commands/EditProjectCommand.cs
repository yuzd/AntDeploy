using AntDeploy.Models;
using AntDeployWinform.Models;
using AntDeployWinform.Winform;

using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using AppDomainToolkit;
using Expression = EnvDTE.Expression;

namespace AntDeploy.Commands
{
    internal sealed class EditProjectCommand : BaseCommand
    {
        public static EditProjectCommand Instance { get; private set; }

        public static void Initialize(EditProjectPackage package)
        {
            Instance = new EditProjectCommand(package);
            package.CommandService.AddCommand(Instance);
        }

        private string _projectFile;
        private EnvDTE.Project _project;

        private EditProjectCommand(EditProjectPackage package)
            : base(package, Ids.CMD_SET, Ids.EDIT_PROJECT_MENU_COMMAND_ID)
        {
        }

        protected override void OnBeforeQueryStatus()
        {


            var projects = SelectedProjects.ToArray();
            if (projects.Length == 1)
            {
                _project = projects[0];

                _projectFile = _project.FullName;
                Text = "AntDeploy";
                Visible = true;
                return;

                //var project = projects[0];
                //if (ProjectHelper.IsDotNetCoreProject(project))
                //{
                //    _projectFile = project.FullName;
                //    Text = "AntDeploy";
                //    Visible = true;

                //}
                //else
                //{
                //    Visible = false;
                //}
            }
            else
            {
                Visible = false;
            }
        }

        protected override void OnExecute()
        {
            //RootNamespace Title Product OutputFileName
            //var friendlyName = "antDomain";
            //var assembly = Assembly.GetExecutingAssembly();
            //var codeBase = assembly.Location;
            //var codeBaseDirectory = Path.GetDirectoryName(codeBase);
            //var setup = new AppDomainSetup()
            //{
            //    ApplicationName = "AntDeployApplication",
            //    ApplicationBase = codeBaseDirectory,
            //    DynamicBase = codeBaseDirectory,
            //};
            //setup.CachePath = setup.ApplicationBase;
            //setup.ShadowCopyFiles = "true";
            //setup.ShadowCopyDirectories = setup.ApplicationBase;
            //AppDomain.CurrentDomain.SetShadowCopyFiles();
            //SecurityZone zone = SecurityZone.MyComputer;
            //Evidence baseEvidence = AppDomain.CurrentDomain.Evidence;
            //Evidence evidence = new Evidence(baseEvidence);
            //string assemblyName = Assembly.GetExecutingAssembly().FullName;
            //evidence.AddAssembly(assemblyName);
            //evidence.AddHost(new Zone(zone));

            //AppDomain otherDomain = AppDomain.CreateDomain(friendlyName,evidence, setup);
            try
            {

                ProjectParam param = new ProjectParam();
                param.IsWebProejct = ProjectHelper.IsWebProject(_project);
                param.IsNetcorePorject = ProjectHelper.IsDotNetCoreProject(_project);
                param.OutPutName = _project.GetProjectProperty("OutputFileName");
                param.VsVersion = ProjectHelper.GetVsVersion();
                param.MsBuildPath = ProjectHelper.GetMsBuildPath();

                //Deploy deploy = new Deploy(_projectFile, param);
                //deploy.ShowDialog();

                //String name = Assembly.GetExecutingAssembly().GetName().FullName;
                //var remoteLoader = otherDomain.CreateInstanceAndUnwrap(name, typeof(AntDeployForm).FullName) as AntDeployForm;
                using(var context = AppDomainContext.Create())
                {
                    
                    var area = RemoteFunc.Invoke(
                        context.Domain,
                        _projectFile,
                        param,
                        (pi, r) =>
                        {
                            Deploy deploy = new Deploy(_projectFile, param);
                            deploy.ShowDialog();
                            return 1;
                        });
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                //AppDomain.Unload(otherDomain);
            }
        }

    }

    public class AntDeployForm : MarshalByRefObject
    {
        private Assembly _assembly;

        public void LoadAssembly(string assemblyFile)
        {
            try
            {
                _assembly = Assembly.LoadFrom(assemblyFile);
                //return _assembly;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T GetInstance<T>(string typeName) where T : class
        {
            if (_assembly == null) return null;
            var type = _assembly.GetType(typeName);
            if (type == null) return null;
            return Activator.CreateInstance(type) as T;
        }

        public void ExecuteMothod(string typeName, string methodName)
        {
            if (_assembly == null) return;
            var type = _assembly.GetType(typeName);
            var obj = Activator.CreateInstance(type);
            Expression<Action> lambda = System.Linq.Expressions.Expression.Lambda<Action>(System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Constant(obj), type.GetMethod(methodName)), null);
            lambda.Compile()();
        }

    }

}