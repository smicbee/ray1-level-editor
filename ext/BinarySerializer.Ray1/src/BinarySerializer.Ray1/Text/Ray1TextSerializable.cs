using System;
using System.Text;

namespace BinarySerializer.Ray1
{
    public abstract class Ray1TextSerializable
    {
        protected bool isFirstLoad = true;
        public Context Context { get; protected set; }

        public void Serialize(string path, Context context, bool read, Encoding encoding)
        {
            // Set the context
            Context = context;

            if (read)
            {
                using var s = context.FileManager.GetFileReadStream(context.GetAbsoluteFilePath(path));

                if (s == null || s.Length <= 0) 
                    return;

                OnPreSerialize(path);

                using (Ray1TextParser parser = new Ray1TextParser(context.GetRequiredSettings<Ray1Settings>(), s, encoding))
                    Read(parser);

                OnPostSerialize(path);
                isFirstLoad = false;
            }
            else
            {
                using var s = context.FileManager.GetFileWriteStream(context.GetAbsoluteFilePath(path));
                
                if (s == null) 
                    return;
                
                OnPreSerialize(path);

                using (Ray1TextParser parser = new Ray1TextParser(context.GetRequiredSettings<Ray1Settings>(), s, encoding))
                    Write(parser);

                OnPostSerialize(path);
                isFirstLoad = false;
            }
        }

        protected virtual void OnPreSerialize(string path) { }
        protected virtual void OnPostSerialize(string path) { }

        public abstract void Read(Ray1TextParser parser);
        public virtual void Write(Ray1TextParser parser) => throw new NotImplementedException();
    }
}