using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks_DB;
using SimpleTasks.Resources;

namespace SimpleTasks
{
    public partial class ItemPage : PhoneApplicationPage
    {
        TasksItem Item = null;
        public ItemPage()
        {
            InitializeComponent();

            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.But_Save;
            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.But_Cancel;

            if (AppData.edit_id >= 0)
                Item = AppData.DB.Items.FirstOrDefault(i => i.Item_ID == AppData.edit_id);

            if (Item != null)
            {
                label_title.Text = AppResources.Item_Title_Edit;
                text_task.Text = Item.Item_Text;
                toggle_end.IsChecked = Item.Item_End;
                stack_end.Visibility = (Item.Item_End) ? Visibility.Visible : Visibility.Collapsed;
                toggle_alert.IsChecked = Item.Item_Alert;
                stack_alert.Visibility = (Item.Item_Alert) ? Visibility.Visible : Visibility.Collapsed;
                toggle_alert_icon.IsChecked = Item.Item_AlertIcon;
                toggle_alert_push.IsChecked = Item.Item_AlertPush;
                date_end.Value = DateTime.FromBinary(Item.Item_TimeEnd);
                if (date_end.Value.Equals(new DateTime(0)))
                {
                    date_end.Value = DateTime.Now.AddDays(7);
                    time_end.Value = DateTime.Now.Subtract(new TimeSpan(0, DateTime.Now.Minute, DateTime.Now.Second));
                }
                else
                {
                    TimeSpan TimeTmp = date_end.Value.Value.Subtract(DateTime.FromBinary(Item.Item_TimeAlert));
                    time_end.Value = new DateTime(1, 1, 1, date_end.Value.Value.Hour, date_end.Value.Value.Minute, 0);
                    date_end.Value = date_end.Value.Value.Subtract(new TimeSpan(time_end.Value.Value.Hour, time_end.Value.Value.Minute, 0));

                    int tmpi = TimeTmp.Hours + (TimeTmp.Days * 24);
                    foreach (ListPickerItem item in list_alert_time.Items)
                    {
                        if (item.Tag.Equals(tmpi.ToString()))
                        {
                            list_alert_time.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
            else
            {
                label_title.Text = AppResources.Item_Title_Add;
                date_end.Value = DateTime.Now.AddDays(7);
                time_end.Value = DateTime.Now.Subtract(new TimeSpan(0, DateTime.Now.Minute, DateTime.Now.Second));
            }
        }

        private void toggle_end_Change(object sender, RoutedEventArgs e)
        {
            if ((bool)toggle_end.IsChecked)
            {
                stack_end.Visibility = Visibility.Visible;
            }
            else
            {
                stack_end.Visibility = Visibility.Collapsed;
            }
        }

        private void toggle_alert_Change(object sender, RoutedEventArgs e)
        {
            if ((bool)toggle_alert.IsChecked)
            {
                stack_alert.Visibility = Visibility.Visible;
            }
            else
            {
                stack_alert.Visibility = Visibility.Collapsed;
            }
        }

        private void appbar_cancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void appbar_save_Click(object sender, EventArgs e)
        {
            if (text_task.Text.Length == 0)
            {
                MessageBox.Show(AppResources.Message_Empty_Title, AppResources.Message_Empty_Text, MessageBoxButton.OK);
                return;
            }

            DateTime datetime_end = new DateTime(date_end.Value.Value.Year, date_end.Value.Value.Month, date_end.Value.Value.Day, time_end.Value.Value.Hour, time_end.Value.Value.Minute, 0);
            DateTime datetime_alert = datetime_end.Subtract(new TimeSpan(int.Parse((string)(((ListPickerItem)list_alert_time.SelectedItem).Tag)), 0, 0));

            if (Item != null)
            {
                Item.Item_Text = text_task.Text;
                Item.Item_End = (bool)toggle_end.IsChecked;
                Item.Item_TimeEnd = (((bool)toggle_end.IsChecked) ? datetime_end : new DateTime(0)).ToBinary();
                Item.Item_Alert = (bool)toggle_alert.IsChecked;
                Item.Item_TimeAlert = ((Item.Item_Alert) ? datetime_alert : new DateTime(0)).ToBinary();
                Item.Item_AlertIcon = (Item.Item_Alert) ? (bool)toggle_alert_icon.IsChecked : false;
                Item.Item_AlertPush = (Item.Item_Alert) ? (bool)toggle_alert_push.IsChecked : false;
                if (datetime_end.CompareTo(DateTime.Now) > 0)
                {
                    Item.Item_Ended = false;
                }
            }
            else
            {
                AppData.DB.Items.InsertOnSubmit(new TasksItem(
                    text_task.Text,
                    (bool)toggle_end.IsChecked,
                    ((bool)toggle_end.IsChecked) ? datetime_end : new DateTime(0),
                    (bool)toggle_alert.IsChecked,
                    ((bool)toggle_alert.IsChecked) ? datetime_alert : new DateTime(0),
                    ((bool)toggle_alert.IsChecked) ? (bool)toggle_alert_icon.IsChecked : false,
                    ((bool)toggle_alert.IsChecked) ? (bool)toggle_alert_push.IsChecked : false
                    ));
            }

            AppData.DB.SubmitChanges();
            NavigationService.GoBack();
        }
    }
}