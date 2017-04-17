namespace SimpleTasks_General
{
    public class MemSet
    {
        public bool alert_block;
        public bool main_end_first;
        public bool main_end_select;
        public bool lock_end_first;
        public bool lock_end_select;

        public SortType sort_start;
        public SortType sort;

        public MemSet()
        {
            alert_block = false;
            main_end_first = true;
            main_end_select = true;
            lock_end_first = true;
            lock_end_select = true;
            sort = SortType.time_1;
            sort_start = SortType.time_1;
        }
    }

    public enum SortType
    {
        name_1 = 0,
        name_2,
        time_1,
        time_2,
        last
    }
}
