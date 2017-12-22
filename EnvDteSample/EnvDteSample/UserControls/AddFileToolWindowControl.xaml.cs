namespace EnvDteSample
{
    using Microsoft.VisualStudio.Shell;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Win32;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using EnvDTE;
    using VSLangProj;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Interaction logic for AddFileToolWindowControl.
    /// </summary>
    public partial class AddFileToolWindowControl : UserControl
    {
        private Package _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddFileToolWindowControl"/> class.
        /// </summary>
        public AddFileToolWindowControl()
        {
            this.InitializeComponent();
            ProjectList.ItemsSource = ProjectNameList;
            ProjectItemList.ItemsSource = ProjectItemsList;
        }

        public void InitializeWithPackage(Package package)
        {
            _package = package;
        }


        private IServiceProvider ServiceProvider
        {
            get
            {
                return this._package;
            }
        }

        public ObservableCollection<SampleProject> ProjectNameList = new ObservableCollection<SampleProject>();
        public ObservableCollection<SampleProjectItem> ProjectItemsList = new ObservableCollection<SampleProjectItem>();

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var item = (SampleProject)ProjectList.SelectedItem;

            if (item == null)
            {
                MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture, "プロジェクトを選択してください"), "AddFileToolWindow");
                return;
            }

            var dialog = new OpenFileDialog();
            dialog.Title = "ファイルを開く";
            dialog.Filter = "全てのファイル(*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                var file = dialog.FileName;
                var dte = ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

                Solution solution = dte.Solution;
                Project project = solution.Projects.Cast<Project>().Where(p => p.Name == item.Name).Select(p => p).FirstOrDefault();
                project.ProjectItems.AddFromFileCopy(file);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            ProjectNameList.Clear();
            ProjectItemsList.Clear();

            var dte = ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            Solution solution = dte.Solution;
            Projects projects = solution.Projects;
            foreach (Project project in projects)
            {
                var projectName = project.Name;
                var projectPath = project.FullName;

                var sampleProject = new SampleProject()
                {
                    Name = projectName,
                    Path = projectPath
                };
                ProjectNameList.Add(sampleProject);
                AddProjectItems(project.ProjectItems, project.Name);
            }
        } 

        private void AddProjectItems(ProjectItems items, string name)
        {
            if (items != null)
            {
                foreach (ProjectItem item in items)
                {
                    var projectItem = new SampleProjectItem()
                    {
                        ProjectName = name,
                        Name = item.Name,
                        FullPath = (string)item.Properties.Item("FullPath").Value
                    };
                    this.ProjectItemsList.Add(projectItem);
                    AddProjectItems(item.ProjectItems, name);
                }
            }
        }

        private void AddNestFile_Click(object sender, RoutedEventArgs e)
        {
            var item = (SampleProjectItem)ProjectItemList.SelectedItem;
            if (item == null)
            {
                MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture, "親ファイルを選択してください"), "AddFileToolWindow");
                return;
            }


            var dialog = new OpenFileDialog();
            dialog.Title = "ファイルを開く";
            dialog.Filter = "全てのファイル(*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {

                ProjectItem target = FindProjectItemByName(item.FullPath);

                if (target != null)
                {
                    ProjectItem newItem = target.ProjectItems.AddFromFileCopy(dialog.FileName);
                }
            }
        }

        private ProjectItem FindProjectItemByName(string fullPath)
        {
            var dte = ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            ProjectItem projectItem = null;
            foreach (Project project in dte.Solution.Projects)
            {
                projectItem = FindByProjectItemByName(project.ProjectItems, fullPath);
            }
            return projectItem;
        }


        private ProjectItem FindByProjectItemByName(ProjectItems items, string fullPath)
        {
            ProjectItem result = null;
            foreach(ProjectItem item in items)
            {
                result = FindByProjectItemByName(item, fullPath);
                if(result == null)
                {
                    result = FindByProjectItemByName(item.ProjectItems, fullPath);
                }
                if (result != null) return result;
            }
            return result;
        }

        private ProjectItem FindByProjectItemByName(ProjectItem item, string fullPath)
        {
            ProjectItem result = null;
            if(item != null)
            {
                var path = (string)item.Properties.Item("FullPath").Value;
                if (fullPath.Equals(path, StringComparison.OrdinalIgnoreCase))
                {
                    result = item;
                }
                else
                {
                    result = FindByProjectItemByName(item.ProjectItems, fullPath);
                }
            }
            return result;
        }

        private void StartupProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (SampleProject)ProjectList.SelectedItem;

            if (item == null)
            {
                MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture, "プロジェクトを選択してください"), "AddFileToolWindow");
                return;
            }
            var dte = ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

            Solution solution = dte.Solution;
            Project project = solution.Projects.Cast<Project>().Where(p => p.Name == item.Name).Select(p => p).FirstOrDefault();
            solution.Properties.Item("StartupProject").Value = project.Name;
        }

        private void AddReferenceButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (SampleProject)ProjectList.SelectedItem;

            if (item == null)
            {
                MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture, "プロジェクトを選択してください"), "AddFileToolWindow");
                return;
            }
            var dte = ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

            Solution solution = dte.Solution;
            Project parentProject = solution.Projects.Cast<Project>().Where(p => p.Name == item.Name).Select(p => p).FirstOrDefault();
            Project project = solution.Projects.Cast<Project>().Where(p => p.Name != item.Name).Select(p => p).FirstOrDefault();
            ((VSProject)parentProject.Object).References.AddProject(project);
        }
    }

    public class SampleProject
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public ObservableCollection<string> ProjectItems { get; set; } = new ObservableCollection<string>();
    }

    public class SampleProjectItem
    {
        public string ProjectName { get; set; }

        public string Name { get; set; }

        public string FullPath { get; set; }

    }
}