﻿using ProjectEvent.UI.Controls.Action.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ProjectEvent.UI.Controls.Action
{
    public class ActionInput : Control
    {
        public Visibility PlaceholderVisibility
        {
            get { return (Visibility)GetValue(PlaceholderVisibilityProperty); }
            set { SetValue(PlaceholderVisibilityProperty, value); }
        }
        public static readonly DependencyProperty PlaceholderVisibilityProperty =
            DependencyProperty.Register("PlaceholderVisibility",
                typeof(Visibility),
                typeof(ActionInput));

        public bool PopupIsOpen
        {
            get { return (bool)GetValue(PopupIsOpenProperty); }
            set { SetValue(PopupIsOpenProperty, value); }
        }
        public static readonly DependencyProperty PopupIsOpenProperty =
            DependencyProperty.Register("PopupIsOpen",
                typeof(bool),
                typeof(ActionInput),
                new PropertyMetadata(false));

        public bool IsValidInput
        {
            get { return (bool)GetValue(IsValidInputProperty); }
            set { SetValue(IsValidInputProperty, value); }
        }
        public static readonly DependencyProperty IsValidInputProperty =
            DependencyProperty.Register("IsValidInput",
                typeof(bool),
                typeof(ActionInput),
                new PropertyMetadata(true));
        public InputType InputType
        {
            get { return (InputType)GetValue(InputTypeProperty); }
            set { SetValue(InputTypeProperty, value); }
        }
        public static readonly DependencyProperty InputTypeProperty =
            DependencyProperty.Register("InputType",
                typeof(InputType),
                typeof(ActionInput),
                new PropertyMetadata(InputType.Text));
        public int ActionID { get; set; }
        private Dictionary<int, List<string>> actionResults;
        private TextBox inputTextBox;
        private ComboBox ActionIDComboBox;
        private ComboBox ActionResultsComboBox;
        private ComboBox SelectComboBox;

        private Popup Popup;
        private bool isEnterPopup;
        private ActionContainer actionContainer;
        private Button addActionResultBtn;
        public List<string> SelectItems { get; set; }
        //public InputType InputType { get; set; }
        public ActionInput()
        {
            DefaultStyleKey = typeof(ActionInput);
            isEnterPopup = false;

            actionResults = new Dictionary<int, List<string>>();

        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            actionContainer = DataContext as ActionContainer;
            actionContainer.Items.CollectionChanged += Items_CollectionChanged;
            actionContainer.ItemIndexChanged += ActionContainer_ItemIndexChanged;
            inputTextBox = GetTemplateChild("InputTextBox") as TextBox;
            inputTextBox.TextChanged += InputTextBox_TextChanged;
            inputTextBox.GotKeyboardFocus += InputTextBox_GotKeyboardFocus;
            inputTextBox.LostKeyboardFocus += InputTextBox_LostKeyboardFocus;

            ActionIDComboBox = GetTemplateChild("ActionIDComboBox") as ComboBox;
            ActionResultsComboBox = GetTemplateChild("ActionResultsComboBox") as ComboBox;
            SelectComboBox = GetTemplateChild("SelectComboBox") as ComboBox;

            Popup = GetTemplateChild("Popup") as Popup;
            addActionResultBtn = GetTemplateChild("AddActionResultBtn") as Button;
            addActionResultBtn.Click += AddActionResultBtn_Click;
            ActionIDComboBox.SelectionChanged += ActionIDComboBox_SelectionChanged;

            Popup.MouseEnter += Popup_MouseEnter;
            Popup.MouseLeave += Popup_MouseLeave;
            Render();
        }

        private void AddActionResultBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ActionResultsComboBox.SelectedItem == null)
            {
                return;
            }
            inputTextBox.AppendText($"{{{ActionIDComboBox.SelectedItem}.{ActionResultsComboBox.SelectedItem}}}");
            Valid();
        }

        private void ActionContainer_ItemIndexChanged(object sender, EventArgs e)
        {
            Render();
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Render();
        }

        private void InputTextBox_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            Valid();
            if (!isEnterPopup)
            {
                PopupIsOpen = false;
            }
        }

        private void Popup_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isEnterPopup)
            {
                PopupIsOpen = false;
            }
            isEnterPopup = false;
        }

        private void Popup_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            isEnterPopup = true;
        }

        private void InputTextBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            PopupIsOpen = true;
        }

        private void ActionIDComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActionResultsComboBox.Items.Clear();
            if (ActionIDComboBox.SelectedItem != null)
            {
                var results = actionResults[(int)ActionIDComboBox.SelectedItem];
                foreach (var name in results)
                {
                    ActionResultsComboBox.Items.Add(name);
                }
                if (ActionResultsComboBox.Items.Count > 0)
                {
                    ActionResultsComboBox.SelectedIndex = 0;
                }
            }
        }

        private void Render()
        {
            if (InputType == InputType.Text)
            {
                UpdateActionResults();

                ActionIDComboBox.Items.Clear();
                foreach (var result in actionResults)
                {
                    ActionIDComboBox.Items.Add(result.Key);
                }
                if (ActionIDComboBox.Items.Count > 0)
                {
                    ActionIDComboBox.SelectedIndex = 0;
                }
            }
            else if (InputType == InputType.Select)
            {
                SelectComboBox.Items.Clear();
                SelectComboBox.Items.Add("请选择条件");
                foreach (var item in SelectItems)
                {
                    SelectComboBox.Items.Add(item);
                }
                SelectComboBox.SelectedIndex = 0;
            }
        }
        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderVisibility = inputTextBox.Text == string.Empty ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateActionResults()
        {
            actionResults.Clear();
            var selfItem = actionContainer.Items.Where(m => m.ID == ActionID).FirstOrDefault();
            if (selfItem.ParentID > 0)
            {
                return;
            }
            foreach (var action in actionContainer.Items)
            {
                //排除自身
                if (action.ID != ActionID &&
                    action.Index < selfItem.Index &&
                    action.ID > 0)
                {
                    actionResults.Add(action.ID, new List<string>());
                    var results = actionResults[action.ID];
                    switch (action.ActionType)
                    {
                        case UI.Types.ActionType.WriteFile:
                            results.Add("Status");
                            break;

                    }
                }
            }
            Valid();
        }


        private void Valid()
        {
            IsValidInput = true;
            if (inputTextBox.Text == string.Empty)
            {
                return;
            }
            //var self = actionContainer.Items.Where(m => m.ID == ActionID).FirstOrDefault();
            //var topActions = actionContainer.Items.Where(m => m.Index < self.Index).ToList();
            var matchs = Regex.Matches(inputTextBox.Text, @"\{([0-9]{1,3}?)\.(.*?)\}");
            foreach (Match match in matchs)
            {
                var actionID = int.Parse(match.Groups[1].Value);
                var actionResultKey = match.Groups[2].Value;
                if (ActionIDComboBox.Items.IndexOf(actionID) == -1 || ActionResultsComboBox.Items.IndexOf(actionResultKey) == -1)
                {
                    IsValidInput = false;
                    break;

                }
                //if (topActions.Where(m => m.ID == actionID).Count() == 0)
                //{
                //    IsValidInput = false;
                //    break;
                //}
            }
        }

    }
}