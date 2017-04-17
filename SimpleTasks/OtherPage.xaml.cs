using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using SimpleTasks.Resources;

using System.Data.Linq;

using System.Threading;
using Microsoft.Phone.Scheduler;

using SimpleTasks_DB;
using SimpleTasks_General;

namespace SimpleTasks
{
    public partial class OtherPage : PhoneApplicationPage
    {
        Grid SelectedItem;
        SolidColorBrush BrushSelect = new SolidColorBrush(Color.FromArgb(100, 100, 100, 100));
        SolidColorBrush BrushUnselect = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

        ApplicationBarIconButton AppBar_Sort;
        ApplicationBarIconButton AppBar_Select;
        ApplicationBarIconButton AppBar_Restore;
        ApplicationBarIconButton AppBar_Edit;
        ApplicationBarIconButton AppBar_Del;

        CustomMessageBox Box_Sort;

        public OtherPage()
        {
            InitializeComponent();

            AppBar_Sort = new ApplicationBarIconButton(new Uri("/Assets/AppBar/sort.png", UriKind.RelativeOrAbsolute));
            AppBar_Sort.Text = AppResources.But_Sort;
            AppBar_Sort.Click += AppBar_Sort_Click;

            AppBar_Select = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Select.png", UriKind.RelativeOrAbsolute));
            AppBar_Select.Text = AppResources.But_Select;
            AppBar_Select.Click += AppBar_Select_Click;

            AppBar_Restore = new ApplicationBarIconButton(new Uri("/Assets/AppBar/upload.png", UriKind.RelativeOrAbsolute));
            AppBar_Restore.Text = AppResources.But_Restore;
            AppBar_Restore.Click += AppBar_Restore_Click;

            AppBar_Edit = new ApplicationBarIconButton(new Uri("/Assets/AppBar/edit.png", UriKind.RelativeOrAbsolute));
            AppBar_Edit.Text = AppResources.But_Edit;
            AppBar_Edit.Click += AppBar_Edit_Click;

            AppBar_Del = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Delete.png", UriKind.RelativeOrAbsolute));
            AppBar_Del.Text = AppResources.But_Del;
            AppBar_Del.Click += AppBar_Del_Click;

            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.Menu_Set;

            List_Tasks.DataContext = new List<TasksListItem>();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            ButtonAdd(AppBar_Sort);

            if (!AppData.ended)
            {
                label_title.Text = AppResources.Other_Title_Ended;
                ButtonAdd(AppBar_Select);
            }
            else
            {
                label_title.Text = AppResources.Other_Title_Completed;
            }

            TasksList_Load();
            if (List_Tasks.ItemsSource.Count == 0)
                row_notasks.Height = new GridLength(80);
            else
                row_notasks.Height = new GridLength(0);
        }

        private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                SelectedItem.Background = BrushUnselect;
                SelectedItem = null;
            }

            ApplicationBar.Buttons.Remove(AppBar_Edit);
            ApplicationBar.Buttons.Remove(AppBar_Restore);
            ApplicationBar.Buttons.Remove(AppBar_Del);
        }

        private void TasksList_Load()
        {
            List<TasksListItem> TasksListTmp = new List<TasksListItem>();

            if (AppData.ended)
            {
                var TasksListDB = from TasksItem DBItem in AppData.DB.Items where DBItem.Item_Ended select DBItem;

                foreach (TasksItem item in TasksListDB)
                {
                    TasksListTmp.Add(new TasksListItem(item.Item_ID, item.Item_Text, item.Item_TimeCreate, item.Item_Complete, AppResources.Other_List_End_Prefix, item.Item_TimeEnd, item.Item_End, item.Item_AlertPush, item.Item_AlertIcon));
                }
            }
            else
            {
                var TasksListDB = from TasksItem DBItem in AppData.DB.Items where DBItem.Item_Complete select DBItem;

                foreach (TasksItem item in TasksListDB)
                {
                    TasksListTmp.Add(new TasksListItem(item.Item_ID, item.Item_Text, item.Item_TimeCreate, item.Item_Complete, AppResources.Other_List_Comleted_Prefix, item.Item_TimeComplete, item.Item_End, item.Item_AlertPush, item.Item_AlertIcon));
                }
            }

            switch (AppData.Settings.sort)
            {
                case SortType.name_2:
                    List_Tasks.DataContext = new List<TasksListItem>(TasksListTmp.OrderByDescending(i => i.item_text));
                    break;
                case SortType.time_1:
                    List_Tasks.DataContext = new List<TasksListItem>(TasksListTmp.OrderBy(i => i.item_create_time_long));
                    break;
                case SortType.time_2:
                    List_Tasks.DataContext = new List<TasksListItem>(TasksListTmp.OrderByDescending(i => i.item_create_time_long));
                    break;
                default:
                    List_Tasks.DataContext = new List<TasksListItem>(TasksListTmp.OrderBy(i => i.item_text));
                    break;
            }
        }

        private void AppBar_Select_Click(object sender, EventArgs e)
        {
            if (List_Tasks.ItemsSource.Count > 0)
                List_Tasks.IsSelectionEnabled = !List_Tasks.IsSelectionEnabled;
        }

        private void List_Tasks_IsSelectionEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                if (SelectedItem != null)
                {
                    SelectedItem.Background = BrushUnselect;
                    SelectedItem = null;
                    ApplicationBar.Buttons.Remove(AppBar_Edit);
                    ApplicationBar.Buttons.Remove(AppBar_Del);
                }
                ButtonAdd(AppBar_Restore);
                ButtonAdd(AppBar_Del);
            }
            else
            {
                ApplicationBar.Buttons.Remove(AppBar_Restore);
                ApplicationBar.Buttons.Remove(AppBar_Del);
            }
        }

        private void AppBar_Restore_Click(object sender, EventArgs e)
        {
            if (List_Tasks.SelectedItems.Count > 0 || SelectedItem != null)
            {
                if (MessageBox.Show(AppResources.Message_Restore_Text, AppResources.Message_Restore_Title, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    return;

                if (SelectedItem != null)
                {
                    AppData.DB.Items.FirstOrDefault(i => i.Item_ID == (int)SelectedItem.Tag).Item_Complete = false;
                }
                else if (List_Tasks.SelectedItems.Count > 0)
                {
                    foreach (TasksListItem item in List_Tasks.SelectedItems)
                    {
                        AppData.DB.Items.FirstOrDefault(i => i.Item_ID == item.item_tag).Item_Complete = false;
                    }
                }
                AppData.DB.SubmitChanges();
                TasksList_Load();

                ApplicationBar.Buttons.Remove(AppBar_Restore);
                ApplicationBar.Buttons.Remove(AppBar_Edit);
                ApplicationBar.Buttons.Remove(AppBar_Del);
            }
        }

        void AppBar_Del_Click(object sender, EventArgs e)
        {
            if (SelectedItem != null || List_Tasks.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(AppResources.Message_Del_Text, AppResources.Message_Del_Title, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    return;
            }
            else
            {
                return;
            }

            ApplicationBar.Buttons.Remove(AppBar_Restore);
            ApplicationBar.Buttons.Remove(AppBar_Edit);
            ApplicationBar.Buttons.Remove(AppBar_Del);

            if (SelectedItem != null)
            {
                int item_id = (int)SelectedItem.Tag;
                AppData.DB.Items.DeleteOnSubmit(AppData.DB.Items.FirstOrDefault(i => i.Item_ID == item_id));
                SelectedItem.Background = BrushUnselect;
                SelectedItem = null;
                ApplicationBar.Buttons.Add(AppBar_Select);
            }
            else if (List_Tasks.SelectedItems.Count > 0)
            {
                foreach (TasksListItem item in List_Tasks.SelectedItems)
                {
                    AppData.DB.Items.DeleteOnSubmit(AppData.DB.Items.FirstOrDefault(i => i.Item_ID == item.item_tag));
                }
            }

            AppData.DB.SubmitChanges();

            TasksList_Load();
        }

        private void AppBar_Edit_Click(object sender, EventArgs e)
        {
            if (SelectedItem == null) return;

            AppData.edit_id = (int)SelectedItem.Tag;
            NavigationService.Navigate(new Uri("/ItemPage.xaml", UriKind.Relative));
        }

        private void But_Sort_Click(object sender, RoutedEventArgs e)
        {
            if ((SortType)(sender as Button).Tag != AppData.Settings.sort)
            {
                AppData.Settings.sort = (SortType)(sender as Button).Tag;
                TasksList_Load();
            }
            Box_Sort.Dismiss();
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (List_Tasks.IsSelectionEnabled)
                return;

            try
            {
                Grid GridTmp;
                if (SelectedItem != null)
                    SelectedItem.Background = BrushUnselect;

                GridTmp = sender as Grid;
                if (GridTmp != SelectedItem)
                {
                    SelectedItem = GridTmp;
                    SelectedItem.Background = BrushSelect;
                }
                else
                {
                    SelectedItem = null;
                }
            }
            catch
            {
                SelectedItem = null;
            }

            if (SelectedItem != null)
            {
                if (!AppData.ended)
                {
                    ApplicationBar.Buttons.Remove(AppBar_Select);
                    ApplicationBar.Buttons.Add(AppBar_Restore);
                }
                ButtonAdd(AppBar_Edit);
                ButtonAdd(AppBar_Del);
            }
            else
            {
                ApplicationBar.Buttons.Remove(AppBar_Edit);
                ApplicationBar.Buttons.Remove(AppBar_Del);
                if (!AppData.ended)
                {
                    ApplicationBar.Buttons.Remove(AppBar_Restore);
                    ApplicationBar.Buttons.Add(AppBar_Select);
                }
            }
        }

        private void AppBar_Sort_Click(object sender, EventArgs e)
        {
            TextBlock Text_Sort_Title = new TextBlock();
            Text_Sort_Title.Text = AppResources.But_Sort;
            Text_Sort_Title.FontSize = 30;
            Text_Sort_Title.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            Button But_Sort_Name_1 = new Button();
            But_Sort_Name_1.Content = AppResources.Set_Sort_Name1;
            But_Sort_Name_1.Tag = SortType.name_1;
            But_Sort_Name_1.BorderThickness = new Thickness(0);
            But_Sort_Name_1.Click += But_Sort_Click;

            Button But_Sort_Name_2 = new Button();
            But_Sort_Name_2.Content = AppResources.Set_Sort_Name2;
            But_Sort_Name_2.Tag = SortType.name_2;
            But_Sort_Name_2.BorderThickness = new Thickness(0);
            But_Sort_Name_2.Click += But_Sort_Click;

            Button But_Sort_Time_1 = new Button();
            But_Sort_Time_1.Content = AppResources.Set_Sort_Time1;
            But_Sort_Time_1.Tag = SortType.time_1;
            But_Sort_Time_1.BorderThickness = new Thickness(0);
            But_Sort_Time_1.Click += But_Sort_Click;

            Button But_Sort_Time_2 = new Button();
            But_Sort_Time_2.Content = AppResources.Set_Sort_Time2;
            But_Sort_Time_2.Tag = SortType.time_2;
            But_Sort_Time_2.BorderThickness = new Thickness(0);
            But_Sort_Time_2.Click += But_Sort_Click;

            StackPanel Stack_Sort = new StackPanel();
            Stack_Sort.Children.Add(Text_Sort_Title);
            Stack_Sort.Children.Add(But_Sort_Name_1);
            Stack_Sort.Children.Add(But_Sort_Name_2);
            Stack_Sort.Children.Add(But_Sort_Time_1);
            Stack_Sort.Children.Add(But_Sort_Time_2);

            Box_Sort = new CustomMessageBox();
            Box_Sort.Content = Stack_Sort;
            Box_Sort.Show();
        }

        private void ButtonAdd(IApplicationBarIconButton button)
        {
            if (!ApplicationBar.Buttons.Contains(button))
                ApplicationBar.Buttons.Add(button);
        }

        private void AppBar_Set_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SetPage.xaml", UriKind.Relative));
        }
    }
}