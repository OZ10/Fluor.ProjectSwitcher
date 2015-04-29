using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Fluor.ProjectSwitcher.Class;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Fluor.ProjectSwitcher.ViewModel
{
    public class ViewModelAddNew : ViewModelBase 
    {
        private Project selectedItem;
        public Project SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                RaisePropertyChanged("SelectedItem");
            }
        }

        private Association selectedAssociation;
        public Association SelectedAssociation
        {
            get
            {
                return selectedAssociation;
            }
            set
            {
                selectedAssociation = value;
                RaisePropertyChanged("SelectedAssociation");
            }
        }

        private ObservableCollection<ListBox> selectedItemCollection;
        public ObservableCollection<ListBox> SelectedItemCollection
        {
            get
            {
                return selectedItemCollection;
            }
            set
            {
                selectedItemCollection = value;
                RaisePropertyChanged("SelectedItemCollection");
            }
        }

        public ViewModelAddNew()
        {
            //Messenger.Default.Register<GenericMessage<SwitcherItem>>(this, DisplayTileDetails);
            //Messenger.Default.Register<Message.MessageAddOrEditTile>(this, DisplayTileDetails);
            Messenger.Default.Register<Message.M_AddOrEditTile>(this, ShowProjectDetails);
        }

        private void ShowProjectDetails(Message.M_AddOrEditTile msg)
        {
            if (msg.Sender is ViewModelTiles)
            {
                SelectedItem = msg.SelectedTile;

                // If the selected project only has one assoication, select it
                if (SelectedItem.Associations.Count == 1)
                {
                    SelectedItem.SelectedAssociation = SelectedItem.Associations[0];
                }
            }
        }

        //private void DisplayTileDetails(Message.MessageAddOrEditTile msg)
        //{
        //    if (msg.Sender is MainViewModel)
        //    {
        //        if (msg.SelectedTile != null)
        //        {
        //            SelectedItem = msg.SelectedTile;

        //            // If the selected project only has one assoication, select it
        //            if (SelectedItem.Associations.Count == 1)
        //            {
        //                SelectedItem.SelectedAssociation = SelectedItem.Associations[0];
        //            }
        //        }
        //        else
        //        {
        //            //SelectedItem = new Project("Project Name", null, "", true, null, true);
        //            //SelectedItem = new Project();
        //            //SelectedItem.Setup("Project Name", null, "", true, null, true);
        //            //msg.ProjectsCollection.Add(SelectedItem);
        //        }

        //        //TODO Refactor this
        //        // Add blank contextmenu item so user can edit it to create new context menus
        //        //SelectedItem.AddContextMenu(new Class.ContextMenu(Class.ContextMenu.TypeEnum.PATH, "", ""));

        //        Class.ContextMenu cm = new Class.ContextMenu();
        //        cm.Setup(Class.ContextMenu.ContextMenuTypeEnum.PATH, "", "");
        //        SelectedItem.AddContextMenu(cm);

        //        foreach (Association association in selectedItem.Associations)
        //        {
        //            //association.AddContextMenu(new Class.ContextMenu(Class.ContextMenu.TypeEnum.PATH, "", ""));

        //            cm = new Class.ContextMenu();
        //            cm.Setup(Class.ContextMenu.ContextMenuTypeEnum.PATH, "", "");
        //            association.AddContextMenu(cm);

        //            Parameter p = new Parameter();
        //            p.Setup(Parameter.ParameterTypeEnum.INI, "", "");
        //            association.Parameters.Add(p);
        //        }

        //        //SelectedItemCollection = new ObservableCollection<ListBox>();

        //        //ListBox lb = new ListBox();
        //        //lb.Template = (ControlTemplate)System.Windows.Application.Current.Resources["NewTileTemplate"];
        //        //lb.DataContext = SelectedItem;

        //        //SelectedItemCollection.Add(lb);
        //    }
        //}

        public void OkButton_Clicked()
        {
            Messenger.Default.Send<Message.M_AddOrEditTile>(new Message.M_AddOrEditTile(SelectedItem, this));
            //proj.Execute(SelectedItem);
            //Messenger.Default.Send<Message.M_SimpleAction>(new Message.M_SimpleAction(Message.M_SimpleAction.Action.RefreshViews));
        }




        //private void SavesChangesToSettings()
        //{
        //    if (SelectedItem != null)
        //    {
        //        XElement xProject = null;
        //        if (SelectedItem.IsNew)
        //        {
        //            xProject = AddNewProjectToXML(SelectedItem, xProject);
        //        }
        //        else
        //        {
        //            xProject = FindExistingProject(SelectedItem, xProject);
        //        }

        //        UpdateProjectDetails(SelectedItem, xProject);

        //        UpdateProjectAssociations(SelectedItem);

        //        // Change project name ONLY after searching for association 
        //        SelectedItem.OriginalName = SelectedItem.Name;
        //    }

        //    // Save changes
        //    _xmlSettings.Save("Fluor.ProjectSwitcher.Projects.xml");
        //    SetupEnvironment();

        //    IsTileTabSelected = true;
        //}

        //private void UpdateProjectAssociations(Project switcherItem)
        //{
        //    foreach (XElement xAssociation in _xmlSettings.Elements("ASSOCIATIONS").Elements("ASSOCIATION"))
        //    {
        //        if (xAssociation.Attribute("PROJECTNAME").Value == switcherItem.OriginalName)
        //        {
        //            if (switcherItem.Associations.Any())
        //            {
        //                List<XElement> associations = new List<XElement>();

        //                foreach (Association association in switcherItem.Associations)
        //                {
        //                    SaveContextMenus(association.ContextMenuCollection, xAssociation);
        //                    SaveParameters(association.Parameters, xAssociation);
        //                }
        //                //xAssociation.ReplaceNodes(associations);
        //            }
        //            else
        //            {
        //                // Remove all contextmenu nodes
        //                xAssociation.Element("CONTEXTMENUS").RemoveNodes();
        //            }
        //        }
        //    }
        //}

        //private static void UpdateProjectDetails(Project switcherItem, XElement xProject)
        //{
        //    if (xProject != null)
        //    {
        //        // Rename project if it's changed
        //        if (switcherItem.OriginalName != switcherItem.Name)
        //        {
        //            // Update xml project name
        //            xProject.Attribute("NAME").Value = switcherItem.Name;

        //            // Project has been renamed. Change name on any associations
        //            foreach (Association association in switcherItem.Associations)
        //            {
        //                association.ProjectName = switcherItem.OriginalName;
        //            }
        //        }

        //        SaveContextMenus(switcherItem.ContextMenuCollection, xProject);

        //        xProject.Attribute("MISCTEXT").Value = switcherItem.MiscText;
        //    }
        //}

        //private XElement FindExistingProject(Project switcherItem, XElement xProject)
        //{
        //    // Find existing project
        //    foreach (XElement xExistingProject in _xmlSettings.Elements("PROJECTS").Elements("PROJECT"))
        //    {
        //        //TODO If project has subprojects

        //        if (xExistingProject.Attribute("NAME").Value == switcherItem.OriginalName)
        //        {
        //            xProject = xExistingProject;
        //            break;
        //        }
        //    }
        //    return xProject;
        //}

        //private XElement AddNewProjectToXML(Project switcherItem, XElement xProject)
        //{
        //    // Add new xelement for new project
        //    xProject = new XElement("PROJECT", new XAttribute("NAME", switcherItem.Name), new XAttribute("MISCTEXT", switcherItem.MiscText));
        //    _xmlSettings.Element("PROJECTS").Add(xProject);

        //    if (switcherItem.Associations.Any())
        //    {
        //        // Add Association, context menus & parameters nodes
        //        foreach (Association association in switcherItem.Associations)
        //        {
        //            // Create association node
        //            XElement xAssociation = new XElement("ASSOCIATION", new XAttribute("PROJECTNAME", association.ProjectName), new XAttribute("APPLICATIONNAME", association.ApplicationName));

        //            // Create contextmenu node
        //            xAssociation.Add(new XElement("CONTEXTMENUS"));

        //            // Create parameter node
        //            xAssociation.Add(new XElement("PARAMETERS"));

        //            _xmlSettings.Element("ASSOCIATIONS").Add(xAssociation);
        //        }
        //    }
        //    return xProject;
        //}

        //private static void SaveContextMenus(ObservableCollection<Class.ContextMenu> contextMenuCollection, XElement xmlNode)
        //{
        //    if (contextMenuCollection.Any())
        //    {
        //        List<XElement> contextMenus = new List<XElement>();

        //        foreach (Class.ContextMenu contextMenu in contextMenuCollection)
        //        {
        //            if (contextMenu.Value != "" && contextMenu.DisplayName != "")
        //            {
        //                contextMenus.Add(new XElement("CONTEXTMENU", new XAttribute("TYPE", contextMenu.Type), new XAttribute("VALUE", contextMenu.Value), new XAttribute("DISPLAYNAME", contextMenu.DisplayName)));
        //            }
        //        }
        //        if (xmlNode.Element("CONTEXTMENUS") == null)
        //        {
        //            xmlNode.Add(new XElement("CONTEXTMENUS"));
        //        }

        //        xmlNode.Element("CONTEXTMENUS").ReplaceNodes(contextMenus);
        //    }
        //    else
        //    {
        //        // Remove all contextmenu nodes
        //        xmlNode.Element("CONTEXTMENUS").RemoveNodes();
        //    }
        //}

        //private static void SaveParameters(ObservableCollection<Parameter> parameterCollection, XElement xmlNode)
        //{
        //    if (parameterCollection.Any())
        //    {
        //        List<XElement> parameters = new List<XElement>();

        //        foreach (Parameter parameter in parameterCollection)
        //        {
        //            if (parameter.Value != "" && parameter.Path != "")
        //            {
        //                parameters.Add(new XElement("PARAMETER", new XAttribute("TYPE", parameter.Type), new XAttribute("VALUE", parameter.Value), new XAttribute("PATH", parameter.Path)));
        //            }
        //        }
        //        xmlNode.Element("PARAMETERS").ReplaceNodes(parameters);
        //    }
        //    else
        //    {
        //        // Remove all contextmenu nodes
        //        xmlNode.Element("PARAMETERS").RemoveNodes();
        //    }
        //}
    }
}
