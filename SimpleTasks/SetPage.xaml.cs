using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks_General;

namespace SimpleTasks
{
    public partial class SetPage : PhoneApplicationPage
    {
        public SetPage()
        {
            InitializeComponent();

            toggle_alert_block.IsChecked = AppData.Settings.alert_block;
            toggle_lock_end_first.IsChecked = AppData.Settings.lock_end_first;
            toggle_lock_end_select.IsChecked = AppData.Settings.lock_end_select;
            toggle_main_end_first.IsChecked = AppData.Settings.main_end_first;
            toggle_main_end_select.IsChecked = AppData.Settings.main_end_select;

            foreach (ListPickerItem item in list_sort.Items)
            {
                if (item.Tag.Equals(AppData.Settings.sort_start.ToString()))
                {
                    list_sort.SelectedItem = item;
                    break;
                }
            }
        }

        private void appbar_save_Click(object sender, EventArgs e)
        {
            AppData.Settings.alert_block = (bool)toggle_alert_block.IsChecked;
            AppData.Settings.lock_end_first = (bool)toggle_lock_end_first.IsChecked;
            AppData.Settings.lock_end_select = (bool)toggle_lock_end_select.IsChecked;
            AppData.Settings.main_end_first = (bool)toggle_main_end_first.IsChecked;
            AppData.Settings.main_end_select = (bool)toggle_main_end_select.IsChecked;

            if (list_sort.SelectedItem != null)
            {
                String tmps = (String)((ListPickerItem)list_sort.SelectedItem).Tag;
                if (tmps.Equals(SortType.last.ToString()))
                    AppData.Settings.sort_start = SortType.last;
                else if (tmps.Equals(SortType.name_1.ToString()))
                    AppData.Settings.sort_start = SortType.name_1;
                else if (tmps.Equals(SortType.name_2.ToString()))
                    AppData.Settings.sort_start = SortType.name_2;
                else if (tmps.Equals(SortType.time_1.ToString()))
                    AppData.Settings.sort_start = SortType.time_1;
                else if (tmps.Equals(SortType.time_2.ToString()))
                    AppData.Settings.sort_start = SortType.time_2;
            }

            AppData.SetSave();
            NavigationService.GoBack();
        }

        private void appbar_cancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}