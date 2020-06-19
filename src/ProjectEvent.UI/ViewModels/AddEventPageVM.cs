﻿using Newtonsoft.Json;
using ProjectEvent.Core.Event.Types;
using ProjectEvent.Core.Helper;
using ProjectEvent.UI.Controls.Action;
using ProjectEvent.UI.Controls.Action.Data;
using ProjectEvent.UI.Controls.InputGroup.Models;
using ProjectEvent.UI.Models;
using ProjectEvent.UI.Models.ConditionModels;
using ProjectEvent.UI.Models.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProjectEvent.UI.ViewModels
{
    public class AddEventPageVM : AddEventPageModel
    {
        public Command AddActionCommand { get; set; }
        public Command AddCommand { get; set; }
        public Command ActionDialogStateCommand { get; set; }
        public Command ShowActionDialogCommand { get; set; }
        private readonly MainViewModel mainVM;
        public Command RedirectCommand { get; set; }
        public Command LoadedCommand { get; set; }
        private ActionContainer actionContainer;

        public AddEventPageVM(MainViewModel mainVM)
        {
            this.mainVM = mainVM;
            RedirectCommand = new Command(new Action<object>(OnRedirectCommand));
            LoadedCommand = new Command(new Action<object>(OnLoadedCommand));
            Events = new System.Collections.ObjectModel.ObservableCollection<Controls.ItemSelect.Models.ItemModel>();
            ComBoxActions = new System.Collections.ObjectModel.ObservableCollection<ComBoxActionModel>();
            AddActionCommand = new Command(new Action<object>(OnAddActionCommand));
            AddCommand = new Command(new Action<object>(OnAddCommand));
            ActionDialogStateCommand = new Command(new Action<object>(OnActionDialogStateCommand));
            ShowActionDialogCommand = new Command(new Action<object>(OnShowActionDialogCommand));
            AddACtionDialogVisibility = System.Windows.Visibility.Hidden;
            PropertyChanged += AddEventPageVM_PropertyChanged;

            IsActionsTabItemSelected = true;
            InitEvents();
            InitConditions();
            InitAcions();

        }

        private void OnLoadedCommand(object obj)
        {
            actionContainer = obj as ActionContainer;

            HandleEdit();
            //Thread.Sleep(1000);
            //HandleEdit();
        }

        private void OnRedirectCommand(object obj)
        {
            mainVM.Uri = obj.ToString();
        }

        private void OnShowActionDialogCommand(object obj)
        {
            OnActionDialogStateCommand(true);
        }

        private void OnActionDialogStateCommand(object obj)
        {
            if (bool.Parse(obj.ToString()))
            {
                AddACtionDialogVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                AddACtionDialogVisibility = System.Windows.Visibility.Hidden;
            }
        }

        private void OnAddCommand(object obj)
        {
            var container = obj as ActionContainer;
            string json = GenerateJson(container);
            IOHelper.WriteFile($"Projects\\{ProjectName}.project.json", json);
            MessageBox.Show(mainVM.Data == null ? "项目已创建！" : "项目已更新，部分设置重启应用生效！");
            if (mainVM.Data == null)
            {
                mainVM.Uri = "IndexPage";
            }
        }

        private void AddEventPageVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedEventID":
                    HandleEventIDChanged();
                    break;
            }
        }

        private void HandleEventIDChanged()
        {
            StepIndex = 2;
            InitConditions();
            //Actions.Clear();
        }

        #region 初始化事件
        private void InitEvents()
        {
            Events.Add(new Controls.ItemSelect.Models.ItemModel()
            {
                ID = (int)EventType.OnIntervalTimer,
                Title = "计时器",
            });
            Events.Add(new Controls.ItemSelect.Models.ItemModel()
            {
                ID = (int)EventType.OnDeviceStartup,
                Title = "开机事件",
            });
            Events.Add(new Controls.ItemSelect.Models.ItemModel()
            {
                ID = (int)EventType.OnProcessStartup,
                Title = "进程创建",
            });
        }
        #endregion

        #region 初始化Actions
        private void InitAcions()
        {
            ComBoxActions.Add(new ComBoxActionModel()
            {
                ID = (int)Types.ActionType.WriteFile,
                Name = "创建文件"
            });
            ComBoxActions.Add(new ComBoxActionModel()
            {
                ID = (int)Types.ActionType.IF,
                Name = "判断"
            });
            ComBoxActions.Add(new ComBoxActionModel()
            {
                ID = (int)Types.ActionType.HttpGet,
                Name = "HTTP GET请求"
            });
            ComBoxSelectedAction = ComBoxActions[0];
        }
        #endregion

        #region 初始化事件条件
        /// <summary>
        /// 初始化事件条件
        /// </summary>
        private void InitConditions()
        {
            var cds = new List<InputModel>();
            switch ((EventType)SelectedEventID)
            {

                case EventType.OnIntervalTimer:
                    //循环计时
                    ConditionData = new IntervalTimerConditionModel();
                    cds.Add(new InputModel()
                    {
                        Type = Controls.InputGroup.InputType.Text,
                        BindingName = "Second",
                        BindingProperty = TextBox.TextProperty,
                        Title = "间隔秒数"
                    });
                    cds.Add(new InputModel()
                    {
                        Type = Controls.InputGroup.InputType.Text,
                        BindingName = "Num",
                        BindingProperty = TextBox.TextProperty,
                        Title = "循环次数（0时永远）"
                    });
                    break;
            }

            Conditions = cds;
        }
        #endregion
        private void OnAddActionCommand(object obj)
        {
            var container = obj as ActionContainer;
            switch ((Types.ActionType)ComBoxSelectedAction.ID)
            {
                //case 1:
                //    container.AddItem(new ActionItemModel()
                //    {
                //        ID = container.GetCreateActionID(),
                //        ActionName = "创建文件",
                //        ActionType = Types.ActionType.WriteFile,
                //        Icon = "\xF2E6",
                //        //Index = new Random().Next(10)
                //    });
                //    break;
                //特殊action 单独处理
                case Types.ActionType.IF:
                    int id = container.GetCreateActionID();
                    var ifmodel = ActionItemsData.Get((Types.ActionType)ComBoxSelectedAction.ID);
                    ifmodel.ID = id;
                    container.AddItem(ifmodel);

                    var elsemodel = ActionItemsData.Get(Types.ActionType.IFElse);
                    elsemodel.ID = container.GetCreateActionID();
                    elsemodel.ParentID = id;
                    container.AddItem(elsemodel);

                    var endmodel = ActionItemsData.Get(Types.ActionType.IFEnd);
                    endmodel.ID = container.GetCreateActionID();
                    endmodel.ParentID = id;
                    container.AddItem(endmodel);
                    break;
                default:
                    //非特殊action
                    var model = ActionItemsData.Get((Types.ActionType)ComBoxSelectedAction.ID);
                    model.ID = container.GetCreateActionID();
                    container.AddItem(model);
                    break;
            }

            OnActionDialogStateCommand(false);
        }


        private string GenerateJson(ActionContainer container)
        {
            var project = new ProjectModel();
            project.ProjectName = ProjectName;
            project.EventID = SelectedEventID;
            project.ConditionData = ConditionData;
            project.Actions = container.GenerateActions();
            var json = JsonConvert.SerializeObject(project);
            return json;
        }

        private void HandleEdit()
        {
            if (mainVM.Data != null)
            {
                string projectName = mainVM.Data.ToString();
                if (IOHelper.FileExists($"Projects\\{projectName}.project.json"))
                {
                    Title = $"编辑Project - {projectName}";
                    ButtonSaveName = "保存";

                    //导入
                    var project = JsonConvert.DeserializeObject<ProjectModel>(File.ReadAllText(IOHelper.GetFullPath($"Projects\\{projectName}.project.json")));
                    if (project != null)
                    {
                        ProjectName = project.ProjectName;
                        SelectedEventID = project.EventID;
                        ConditionData = project.ConditionData;
                        actionContainer.ImportActions(project.Actions);
                    }

                }
            }
            else
            {
                IsInfoTabItemSelected = true;
            }
        }
    }
}
