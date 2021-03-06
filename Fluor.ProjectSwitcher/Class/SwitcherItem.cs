﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Fluor.ProjectSwitcher.Class
{
    public class SwitcherItem : Base
    {
        private string name;
        [XmlAttribute()]
        public string Name { get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        } // Current name of the project

        [XmlIgnore]
        public string OriginalName { get; set; } // Used to store the name before it's modified in the Edit view

        //public ObservableCollection<SwitcherItem> SubItems { get; set; }

        [XmlAttribute()]
        public bool IsEnabled { get; set; }

        [XmlAttribute()]
        public string ParentItem { get; set; }

        [XmlIgnore]
        public bool IsNew { get; set; }

        [XmlIgnore]
        public bool IsDeleted { get; set; }

        private string miscText;
        [XmlAttribute()]
        public string MiscText
        {
            get
            {
                return miscText;
            }
            set
            {
                miscText = value;

                if (miscText != "")
                {
                    MiscTextVisibility = Visibility.Visible;
                }

                RaisePropertyChanged("MiscText");
            }
        }

        private Visibility miscTextVisibility = Visibility.Collapsed;
        [XmlIgnore]
        public Visibility MiscTextVisibility
        {
            get
            {
                return miscTextVisibility;
            }
            set
            {
                miscTextVisibility = value;
                RaisePropertyChanged("MiscTextVisibility");
            }
        }

        private ObservableCollection<ContextMenu> contextMenuCollection;
        public ObservableCollection<ContextMenu> ContextMenuCollection
        {
            get
            {
                return contextMenuCollection;
            }
            set
            {
                contextMenuCollection = value;
                RaisePropertyChanged("ContextMenuCollection");
            }
        }

        private bool isActive;
        [XmlIgnore]
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
                RaisePropertyChanged("IsActive");
            }
        }

        private ObservableCollection<MenuItem> menuItemCollection;
        [XmlIgnore]
        public ObservableCollection<MenuItem> MenuItemCollection
        {
            get
            {
                return menuItemCollection;
            }
            set
            {
                menuItemCollection = value;
                RaisePropertyChanged("MenuItemCollection");
            }
        }

        private bool _isActiveProject;
        [XmlIgnore]
        public bool IsActiveProject
        {
            get
            {
                return _isActiveProject;
            }
            set
            {
                _isActiveProject = value;
            }
        }

        [XmlAttribute]
        public int Position { get; set; }

        //private Visibility isEditMode;
        //[XmlIgnore]
        //public Visibility IsEditMode
        //{
        //    get
        //    {
        //        return isEditMode;
        //    }
        //    set
        //    {
        //        isEditMode = value;
        //        RaisePropertyChanged("IsEditMode");
        //    }
        //}

        //public SwitcherItem GetActiveSubItem()
        //{
        //    SwitcherItem item = RecurseSubItems(SubItems);
        //    return item;
        //}

        //private SwitcherItem RecurseSubItems(ObservableCollection<SwitcherItem> items)
        //{
        //    foreach (SwitcherItem subItem in items)
        //    {
        //        if (subItem.IsActive)
        //        {
        //            return subItem;
        //        }

        //        RecurseSubItems(subItem.SubItems);
        //    }

        //    return null;
        //}

        //public void GetContextMenus(IEnumerable<XElement> contextMenus)
        //{
        //    if (ContextMenuCollection == null)
        //    {
        //        ContextMenuCollection = new ObservableCollection<ContextMenu>();
        //    }

        //    if (contextMenus != null)
        //    {
        //        foreach (XElement contextMenu in contextMenus)
        //        {
        //            if (contextMenu.Attribute("TYPE").Value != "")
        //            {
        //                //TODO Hard-coded PATH enum needs to be changed
        //                ContextMenu newContextMenu = new ContextMenu();
        //                newContextMenu.Setup(ContextMenu.ContextMenuTypeEnum.PATH, contextMenu.Attribute("VALUE").Value, contextMenu.Attribute("DISPLAYNAME").Value);
        //                AddContextMenu(newContextMenu);
        //            }
        //        }
        //    }
        //    //else
        //    //{
        //    //    // Add a blank row so user can input data
        //    //    //TODO Hard-coded PATH enum needs to be changed
        //    //    ContextMenu newContextMenu = new ContextMenu();
        //    //    newContextMenu.Setup(ContextMenu.ContextMenuTypeEnum.PATH, "", "");
        //    //    AddContextMenu(newContextMenu);
        //    //}
        //}

        public void AddContextMenu(ContextMenu contextMenu)
        {
            if (ContextMenuCollection == null)
            {
                ContextMenuCollection = new ObservableCollection<ContextMenu>();
            }

            if (!contextMenuCollection.Contains(contextMenu))
            {
                ContextMenuCollection.Add(contextMenu);
            }
        }

        private static Path CreateIcon()
        {
            string folderIconPath = "F1 M 21,30.0001L 55.9999,30.0001L 55.9999,50L 21,50L 21,30.0001 Z M 52,28L 37,28C 38,25 39.4999,24.0001 39.4999,24.0001L 50.75,24C 51.3023,24 52,24.6977 52,25.25L 52,28 Z ";

            Path path = new Path();
            path.Stretch = System.Windows.Media.Stretch.Uniform;
            path.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(00, 68, 125));
            path.StrokeThickness = 2;
            path.Height = 15;
            path.Width = 15;
            path.Data = Geometry.Parse(folderIconPath);
            return path;
        }
    }
}
