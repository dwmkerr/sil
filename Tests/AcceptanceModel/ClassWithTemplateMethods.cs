namespace AcceptanceModel
{
    public class ClassWithTemplateMethods
    {
        public string WriteObject<T>(T obj)
        {
            return string.Format("{0} - {1}", typeof (T), obj);
        }
    }
}