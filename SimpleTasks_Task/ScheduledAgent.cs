using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using System.Threading;
using System.IO.IsolatedStorage;
using Windows.Phone.System.UserProfile;
using Windows.UI;
using System.Collections.Generic;

using System.Linq;
using System.Net;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using SimpleTasks_DB;
using SimpleTasks_General;

using System.IO;
using System.Xml.Serialization;

namespace SimpleTasks_Task
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static MemSet Settings;

        static ScheduledAgent()
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
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
                        Settings = (MemSet)Serializer.Deserialize(stream);

                        if (Settings.sort_start != SortType.last)
                            Settings.sort = Settings.sort_start;
                    }
                }
            }
            catch
            {
                Settings = new MemSet();
            }
            mutex.ReleaseMutex();
        }

        static public void PushSend(string text)
        {
            ShellToast toast = new ShellToast();
            toast.Title = "Alert";
            toast.Content = text;
            toast.Show();
        }

        static public void TileClear()
        {
            TileUpdate(0, "");
        }

        static public void TileUpdate(int count, string text)
        {
            //Получение указатиля на плитку
            var primaryTile = ShellTile.ActiveTiles.FirstOrDefault();

            //Если плитки не существует, то выход
            if (primaryTile == null) return;

            var tileData = new Microsoft.Phone.Shell.StandardTileData()
            {
                BackBackgroundImage = null,
                BackContent = text,
                BackgroundImage = null,
                Count = count
            };

            try
            {
                primaryTile.Update(tileData);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        protected override void OnInvoke(ScheduledTask task)
        {
            SetLoad();
            TasksDC DB = new TasksDC(TasksDC.DBConnectionString);

            //Если базы не существует
            if (DB.DatabaseExists() == false)
            {
                TileClear();
                return;
            }

            String      text = "";                  //Текст для плитки
            int         cnt = 5;                    //Счётчик строк текста с установкой в максимум
            int         count = 0;                  //Счётчик для плитки
            bool        update = false;             //Флаг обновления базы
            List<int>   ListDel = new List<int>();  //Список игнорируемых при добавлении на плитку задач

            //Загрузка из базы актуальных задач
            var TasksListDB = from TasksItem DBItem in DB.Items where (!DBItem.Item_Complete && !DBItem.Item_Ended) select DBItem;
            if (TasksListDB.Count() <= 0)
            {
                //Если список задач пуст
                TileClear();
                return;
            }

            //Проход по списку задач, сортированного по времени завершения
            foreach (var item in TasksListDB.OrderBy(i => i.Item_TimeAlert))
            {
                //Если в списке сначала близкие к завершению
                if (Settings.lock_end_first && item.Item_AlertIcon && DateTime.FromBinary(item.Item_TimeAlert).CompareTo(DateTime.Now) < 0)
                {
                    //Если не достигнут максимум строк
                    if (cnt > 0)
                    {
                        if (text.Length > 0) text += "\n";
                        text += (Settings.lock_end_select) ? "! " + item.Item_Text.ToUpper() : item.Item_Text;
                        ListDel.Add(item.Item_ID);
                        count++;
                        cnt--;
                    }
                }

                //Если нужны уведомления о задаче
                if (!Settings.alert_block && item.Item_AlertPush && DateTime.FromBinary(item.Item_TimeAlert).CompareTo(DateTime.Now) < 0)
                {
                    //Отправка уведомлений
                    PushSend(item.Item_Text);

                    //Отключение необходимости уведомления о задаче в базе
                    (DB.Items.FirstOrDefault(i => i.Item_ID == item.Item_ID)).Item_AlertPush = false;
                    update = true;
                }

                //Если задача ограничена по времени и подошла к завершению
                if (item.Item_End && DateTime.FromBinary(item.Item_TimeEnd).CompareTo(DateTime.Now) < 0)
                {
                    //Установка флага удаления задачи в базе
                    (DB.Items.FirstOrDefault(i => i.Item_ID == item.Item_ID)).Item_Ended = true;
                    update = true;
                }
            }

            //Если нужно обновление базы
            if (update) DB.SubmitChanges();

            //Проход по списку задач, сортерованного по времени создания
            foreach (var item in TasksListDB.OrderBy(i => i.Item_TimeCreate))
            {
                //Если не дотигнут максимум строк
                if (cnt > 0)
                {
                    //Если задачи нет в списке игнорирования
                    if (!ListDel.Contains(item.Item_ID))
                    {
                        if (text.Length > 0) text += "\n";
                        text += "• " + item.Item_Text;
                        cnt--;
                    }
                }
                else
                {
                    break;
                }
            }

            TileUpdate(count, text);

            #if DEBUG
                ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(30));
            #endif

            NotifyComplete();
        }
    }
}