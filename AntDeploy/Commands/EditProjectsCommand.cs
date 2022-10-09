namespace AntDeploy.Commands
{
    internal sealed class EditProjectsCommand : BaseCommand
    {
        public static EditProjectsCommand Instance { get; private set; }

        public static void Initialize(EditProjectPackage package)
        {
            Instance = new EditProjectsCommand(package);
            package.CommandService.AddCommand(Instance);
        }

        public EditProjectsCommand(EditProjectPackage package)
            : base(package, Ids.CMD_SET, Ids.EDIT_PROJECTS_MENU_COMMAND_ID)
        {
        }

        protected override void OnExecute()
        {
            
        }
    }
}