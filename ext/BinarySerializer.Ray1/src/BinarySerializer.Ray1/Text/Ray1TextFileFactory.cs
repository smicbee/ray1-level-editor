using System;
using System.Text;

namespace BinarySerializer.Ray1
{
    public static class Ray1TextFileFactory
    {
        public static T ReadText<T>(string filePath, Context context, Action<T> onPreSerialize = null, Encoding encoding = null)
            where T : Ray1TextSerializable, new()
        {
            // Create the data object
            T t = new T();

            // Option pre-serialize action
            onPreSerialize?.Invoke(t);

            // Serialize the object
            t.Serialize(filePath, context, true, encoding);

            // Return the object
            return t;
        }

        public static T WriteText<T>(string filePath, T obj, Context context, Action<T> onPreSerialize = null, Encoding encoding = null)
            where T : Ray1TextSerializable, new()
        {
            // Option pre-serialize action
            onPreSerialize?.Invoke(obj);

            // Serialize the object
            obj.Serialize(filePath, context, false, encoding);

            // Return the object
            return obj;
        }
    }
}