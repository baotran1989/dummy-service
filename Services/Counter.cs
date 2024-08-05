namespace IzotaDummy.Services
{
    public class Counter
    {
        private int value = 0;
        private static Counter? instance;
        private static readonly object lockObject = new object();

        private Counter() { }

        public static Counter Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new Counter();
                    }
                    return instance;
                }
            }
        }

        public int GetValue()
        {
            return value;
        }

        public void Increment()
        {
            value++;
        }
    }
}
