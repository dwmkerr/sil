namespace AcceptanceModel
{
    public class TemplateClassWithIndexer<T>
    {
        private T[] arr = new T[100];

        public T this[int i]
        {
            get
            {
                return arr[i];
            }
            set
            {
                arr[i] = value;
            }
        }
    }

    public class ClassWithIndexer
    {
        private int[] values = new int[100];

        public int this[int i]
        {
            get
            {
                return values[i];
            }
            set
            {
                values[i] = value;
            }
        }
    }
}