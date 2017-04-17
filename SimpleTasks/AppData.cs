using System;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.IO.IsolatedStorage;
using SimpleTasks_DB;
using Microsoft.Phone.Scheduler;

using SimpleTasks_General;

using System.Threading;

using System.Windows;
using System.ComponentModel;

namespace SimpleTasks
{
    public class TasksListItem
    {
        public int item_tag { get; set; }
        public string item_text { get; set; }
        public string item_create_time { get; set; }
        public Visibility item_end_visibility { get; set; }
        public string item_end_time { get; set; }
        public Visibility item_push_visibility { get; set; }
        public Visibility item_icon_visibility { get; set; }
        public bool item_complete { get; set; }
        public long item_create_time_long;
        public long item_end_time_long;

        public TasksListItem(int tag, string text, long time_create, bool complete, string time_end_prefix, long time_end, bool end, bool push, bool icon)
        {
            item_tag = tag;
            item_text = text;
            item_create_time = DateTime.FromBinary(time_create).ToString();
            item_create_time_long = time_create;
            item_end_time = time_end_prefix + DateTime.FromBinary(time_end).ToString();
            item_end_time_long = time_end;
            item_end_visibility = (end) ? Visibility.Visible : Visibility.Collapsed;
            item_push_visibility = (push) ? Visibility.Visible : Visibility.Collapsed;
            item_icon_visibility = (icon) ? Visibility.Visible : Visibility.Collapsed;
            item_complete = complete;
        }
    }

    static class AppData
    {
        static public TasksDC DB;
        static public MemSet Settings;
        static public int edit_id = -1;
        static public bool ended = false;

        static public void SetSave()
        {
            Mutex mutex = new Mutex(true, "SimpleTasks_SetData");
            mutex.WaitOne();
            XmlSerializer Serializer = new XmlSerializer(typeof(MemSet));

            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream = store.OpenFile("SimpleTasks_Set.xml", FileMode.Create))
                {
                    Serializer.Serialize(stream, AppData.Settings);
                }
            }
            mutex.ReleaseMutex();
        }

        static public void SetLoad()
        {
            Mutex mutex = new Mutex(true, "SimpleTasks_SetData");
            mutex.WaitOne();
            try
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(MemSet));

                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var stream = store.OpenFile("SimpleTasks_Set.xml", FileMode.Open))
                    {
                        AppData.Settings = (MemSet)Serializer.Deserialize(stream);

                        if (AppData.Settings.sort_start != SortType.last)
                            AppData.Settings.sort = AppData.Settings.sort_start;
                    }
                }
            }
            catch
            {
                AppData.Settings = new MemSet();
            }
            mutex.ReleaseMutex();
        }

        static public void TaskStart(bool once = false)
        {
            PeriodicTask Task = ScheduledActionService.Find("SimpleTasks_Task") as PeriodicTask;

            if (Task != null)
            {
                if (once)
                    return;
                try
                {
                    ScheduledActionService.Remove("SimpleTasks_Task");
                }
                catch (Exception)
                {
                }
            }

            Task = new PeriodicTask("SimpleTasks_Task");
            Task.Description = "Task agent for SimpleTasks";
            
            try
            {
                ScheduledActionService.Add(Task);
                #if(DEBUG)
                    ScheduledActionService.LaunchForTest("SimpleTasks_Task", TimeSpan.FromSeconds(30));
                #endif
            }
            catch (Exception)
            {
            }
        }

        static public void TaskStop()
        {
            try
            {
                ScheduledActionService.Remove("SimpleTasks_Task");
            }
            catch (Exception)
            {
            }
        }

        public static Image ImageLoad(string path)
        {
            Image tmpimg = new Image();
            tmpimg.Source = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
            return tmpimg;
        }
    }
}
