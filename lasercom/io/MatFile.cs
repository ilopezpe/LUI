using HDF5DotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace lasercom.io
{
    public class MatFile : IDisposable
    {
        bool PrependOnClose = true;

        readonly Dictionary<string, MatVar> Variables;

        public MatFile(string fileName)
        {
            FileName = fileName;
            FileId = H5F.create(FileName, H5F.CreateMode.ACC_TRUNC);
            GroupId = H5G.open(FileId, "/");

            Variables = new Dictionary<string, MatVar>();
        }

        public string FileName { get; }

        public H5FileId FileId { get; private set; }

        public H5GroupId GroupId { get; private set; }

        public MatVar this[string Name]
        {
            get
            {
                Variables.TryGetValue(Name, out var V);
                return V;
            }
        }

        public bool Disposed { get; private set; }

        public bool Closed { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public MatVar<T> CreateVariable<T>(string Name, params long[] Dims)
        {
            var V = new MatVar<T>(Name, GroupId, Dims);
            Variables.Add(Name, V);
            return V;
        }

        void PrependMatlabHeader(string filename)
        {
            var header = new byte[512];

            var headerText = "MATLAB 7.3 MAT-file";
            var headerTextBytes = Encoding.ASCII.GetBytes(headerText);

            for (var i = 0; i < headerText.Length; i++) header[i] = headerTextBytes[i];

            header[124] = 0;
            header[125] = 2;
            header[126] = Encoding.ASCII.GetBytes("I")[0];
            header[127] = Encoding.ASCII.GetBytes("M")[0];

            var tempfile = Path.GetTempFileName();
            using (var newFile = new FileStream(tempfile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                newFile.Write(header, 0, header.Length);

                using (var oldFile = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    oldFile.CopyTo(newFile);
                }
            }

            File.Copy(tempfile, filename, true);
            File.Delete(tempfile);
            PrependOnClose = false;
        }

        public void Close()
        {
            if (!Closed)
            {
                foreach (var V in Variables.Values) V.Close();

                H5G.close(GroupId);
                H5F.close(FileId);

                if (PrependOnClose)
                    PrependMatlabHeader(FileName);

                Closed = true;
            }
        }

        public void Reopen()
        {
            if (Disposed) throw new ObjectDisposedException(FileName + " disposed");
            if (Closed)
            {
                FileId = H5F.open(FileName, H5F.OpenMode.ACC_RDWR);
                GroupId = H5G.open(FileId, "/");
                foreach (var V in Variables.Values) V.Open();
                Closed = false;
            }
        }

        void Dispose(bool disposing)
        {
            if (disposing) Close();
            Disposed = true;
        }
    }
}