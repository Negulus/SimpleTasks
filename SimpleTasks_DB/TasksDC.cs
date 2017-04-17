using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

/*
Item_ID
Item_Text
Item_TimeCreate
Item_Complete
Item_TimeComplete
Item_End
Item_TimeEnd
Item_Ended
Item_Alert
Item_TimeAlert
Item_AlertIcon
Item_AlertPush
*/

namespace SimpleTasks_DB
{
    public class TasksDC : DataContext
    {
        static public string DBConnectionString = "Data Source=isostore:/SimpleTasks.sdf";

        // Pass the connection string to the base class.
        public TasksDC(string connectionString)
            : base(connectionString)
        {
        }

        // Specify a table for the to-do items.
        public Table<TasksItem> Items;
    }

    [Table]
    #region TasksItem
    public class TasksItem : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public TasksItem(string text, bool end, DateTime time_end, bool alert, DateTime time_alert, bool alert_icon, bool alert_push)
        {
            Item_Text = text;
            Item_End = end;
            Item_TimeEnd = time_end.ToBinary();
            Item_Alert = alert;
            Item_TimeAlert = time_alert.ToBinary();
            Item_AlertIcon = alert_icon;
            Item_AlertPush = alert_push;

            Item_TimeCreate = DateTime.Now.ToBinary();
            Item_Complete = false;
            Item_TimeComplete = new DateTime(0).ToBinary();
            Item_Ended = false;
        }

        public TasksItem() { }

        //id
        private int _Item_ID;
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false)]
        public int Item_ID
        {
            get { return _Item_ID; }
            set
            {
                if (_Item_ID != value)
                {
                    NotifyPropertyChanging("Item_ID");
                    _Item_ID = value;
                    NotifyPropertyChanged("Item_ID");
                }
            }
        }

        //Описание
        private string _Item_Text;
        [Column]
        public string Item_Text
        {
            get { return _Item_Text; }
            set
            {
                if (_Item_Text != value)
                {
                    NotifyPropertyChanging("Item_Text");
                    _Item_Text = value;
                    NotifyPropertyChanged("Item_Text");
                }
            }
        }

        //создано
        private long _Item_TimeCreate;
        [Column]
        public long Item_TimeCreate
        {
            get { return _Item_TimeCreate; }
            set
            {
                if (_Item_TimeCreate != value)
                {
                    NotifyPropertyChanging("Item_TimeCreate");
                    _Item_TimeCreate = value;
                    NotifyPropertyChanged("Item_TimeCreate");
                }
            }
        }

        //завершено?
        private bool _Item_Complete;
        [Column]
        public bool Item_Complete
        {
            get { return _Item_Complete; }
            set
            {
                if (_Item_Complete != value)
                {
                    NotifyPropertyChanging("Item_Complete");
                    _Item_Complete = value;
                    NotifyPropertyChanged("Item_Complete");
                }
            }
        }

        //время завершено
        private long _Item_TimeComplete;
        [Column]
        public long Item_TimeComplete
        {
            get { return _Item_TimeComplete; }
            set
            {
                if (_Item_TimeComplete != value)
                {
                    NotifyPropertyChanging("Item_TimeComplete");
                    _Item_TimeComplete = value;
                    NotifyPropertyChanged("Item_TimeComplete");
                }
            }
        }

        //есть время окончания?
        private bool _Item_End;
        [Column]
        public bool Item_End
        {
            get { return _Item_End; }
            set
            {
                if (_Item_End != value)
                {
                    NotifyPropertyChanging("Item_End");
                    _Item_End = value;
                    NotifyPropertyChanged("Item_End");
                }
            }
        }

        //время окончания
        private long _Item_TimeEnd;
        [Column]
        public long Item_TimeEnd
        {
            get { return _Item_TimeEnd; }
            set
            {
                if (_Item_TimeEnd != value)
                {
                    NotifyPropertyChanging("Item_TimeEnd");
                    _Item_TimeEnd = value;
                    NotifyPropertyChanged("Item_TimeEnd");
                }
            }
        }

        //окончено?
        private bool _Item_Ended;
        [Column]
        public bool Item_Ended
        {
            get { return _Item_Ended; }
            set
            {
                if (_Item_Ended != value)
                {
                    NotifyPropertyChanging("Item_Ended");
                    _Item_Ended = value;
                    NotifyPropertyChanged("Item_Ended");
                }
            }
        }

        //предупреждение?
        private bool _Item_Alert;
        [Column]
        public bool Item_Alert
        {
            get { return _Item_Alert; }
            set
            {
                if (_Item_Alert != value)
                {
                    NotifyPropertyChanging("Item_Alert");
                    _Item_Alert = value;
                    NotifyPropertyChanged("Item_Alert");
                }
            }
        }

        //время до завершения
        private long _Item_TimeAlert;
        [Column]
        public long Item_TimeAlert
        {
            get { return _Item_TimeAlert; }
            set
            {
                if (_Item_TimeAlert != value)
                {
                    NotifyPropertyChanging("Item_TimeAlert");
                    _Item_TimeAlert = value;
                    NotifyPropertyChanged("Item_TimeAlert");
                }
            }
        }

        //предупреждение: иконка
        private bool _Item_AlertIcon;
        [Column]
        public bool Item_AlertIcon
        {
            get { return _Item_AlertIcon; }
            set
            {
                if (_Item_AlertIcon != value)
                {
                    NotifyPropertyChanging("Item_AlertIcon");
                    _Item_AlertIcon = value;
                    NotifyPropertyChanged("Item_AlertIcon");
                }
            }
        }

        //предупреждение: push
        private bool _Item_AlertPush;
        [Column]
        public bool Item_AlertPush
        {
            get { return _Item_AlertPush; }
            set
            {
                if (_Item_AlertPush != value)
                {
                    NotifyPropertyChanging("Item_AlertPush");
                    _Item_AlertPush = value;
                    NotifyPropertyChanged("Item_AlertPush");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }
    }
    #endregion


}
