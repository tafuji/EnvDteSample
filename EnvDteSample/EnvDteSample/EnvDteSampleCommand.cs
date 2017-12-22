using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using EnvDTE80;

namespace EnvDteSample
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class EnvDteSampleCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("7247f027-14a4-42d8-aeb3-86d4766b1afb");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvDteSampleCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private EnvDteSampleCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static EnvDteSampleCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new EnvDteSampleCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var dte = ServiceProvider.GetService(typeof(SDTE)) as EnvDTE.DTE;

            var guid = PackageConstants.OutputWindowGuid;
            IVsOutputWindow output = (IVsOutputWindow)ServiceProvider.GetService(typeof(SVsOutputWindow));
            output.CreatePane(ref guid, PackageConstants.OutputWindowTitle, Convert.ToInt32(true), Convert.ToInt32(true));
            output.GetPane(ref guid, out IVsOutputWindowPane pane);

            var startstring = @"
  _    _      _ _        __      ___                 _    _____ _             _ _         ______      _                 _             
 | |  | |    | | |       \ \    / (_)               | |  / ____| |           | (_)       |  ____|    | |               (_)            
 | |__| | ___| | | ___    \ \  / / _ ___ _   _  __ _| | | (___ | |_ _   _  __| |_  ___   | |__  __  _| |_ ___ _ __  ___ _  ___  _ __  
 |  __  |/ _ \ | |/ _ \    \ \/ / | / __| | | |/ _` | |  \___ \| __| | | |/ _` | |/ _ \  |  __| \ \/ / __/ _ \ '_ \/ __| |/ _ \| '_ \ 
 | |  | |  __/ | | (_) |    \  /  | \__ \ |_| | (_| | |  ____) | |_| |_| | (_| | | (_) | | |____ >  <| ||  __/ | | \__ \ | (_) | | | |
 |_|  |_|\___|_|_|\___/      \/   |_|___/\__,_|\__,_|_| |_____/ \__|\__,_|\__,_|_|\___/  |______/_/\_\\__\___|_| |_|___/_|\___/|_| |_|
                                                                                                                                      
                                                                                                                                      
";

            pane.OutputString(startstring);


            OutputWindowPanes panes = ((DTE2)dte).ToolWindows.OutputWindow.OutputWindowPanes;
            OutputWindowPane outputPane = null;
            try
            {
                outputPane = panes.Item(PackageConstants.OutputWindowTitle);
            }
            catch (ArgumentException)
            {
                panes.Add(PackageConstants.OutputWindowTitle);
                outputPane = panes.Item(PackageConstants.OutputWindowTitle);
            }

            var envdtestring = @"
  _    _      _ _ _         ______              _____ _______ ______ 
 | |  | |    | | | |       |  ____|            |  __ \__   __|  ____|
 | |__| | ___| | | | ___   | |__   _ ____   __ | |  | | | |  | |__   
 |  __  |/ _ \ | | |/ _ \  |  __| | '_ \ \ / / | |  | | | |  |  __|  
 | |  | |  __/ | | | (_) | | |____| | | \ V /  | |__| | | |  | |____ 
 |_|  |_|\___|_|_|_|\___/  |______|_| |_|\_/   |_____/  |_|  |______|
                                                                     
                                                                     
";

            outputPane.OutputString(envdtestring);

            Solution solution = dte.Solution;
            Projects projects = solution.Projects;
            foreach (Project project in projects)
            {
                outputPane.OutputString(project.Name);
            }
        }
    }
}
