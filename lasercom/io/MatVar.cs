using HDF5DotNet;
using System;
using System.Linq;
using System.Text;

namespace LuiHardware.io
{
    public abstract class MatVar : IDisposable
    {
        public string Name;

        public bool Disposed { get; protected set; }

        public bool Closed { get; protected set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract void Close();

        public abstract void Open();

        void Dispose(bool disposing)
        {
            if (disposing) Close();
            Disposed = true;
        }
    }

    /// <summary>
    ///     Represents a MATLAB array of specified type.
    ///     Note the array will be transposed from HDF5 row-major format
    ///     to MATLAB column major format automatically when loaded in MATLAB.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MatVar<T> : MatVar
    {
        long[] _Dims;

        H5DataSetId DataSetId;
        readonly H5FileOrGroupId FileOrGroupId;
        H5DataSpaceId SpaceId;
        H5DataTypeId TypeId;

        /// <summary>
        ///     Create new variable in the file or group.
        /// </summary>
        /// <param name="_Name"></param>
        /// <param name="_FileOrGroupId"></param>
        /// <param name="_Dims"></param>
        protected internal MatVar(string _Name, H5FileOrGroupId _FileOrGroupId, params long[] _Dims)
        {
            InitTypeId(out var MatlabClass);

            Name = _Name;
            FileOrGroupId = _FileOrGroupId;
            Dims = _Dims;
            Cursor = new long[Dims.Length];

            SpaceId = H5S.create_simple(Dims.Length, Dims);
            DataSetId = H5D.create(FileOrGroupId, "/" + Name, TypeId, SpaceId);
            var AttributeTypeId = H5T.create(H5T.CreateClass.STRING, MatlabClass.Length);
            var AttributeSpaceId = H5S.create(H5S.H5SClass.SCALAR);
            var AttributeId = H5A.create(DataSetId, "MATLAB_class", AttributeTypeId, AttributeSpaceId);
            var asciiBytes = Encoding.ASCII.GetBytes(MatlabClass);
            H5A.write(AttributeId, AttributeTypeId, new H5Array<byte>(asciiBytes));
            H5A.close(AttributeId);
            H5S.close(AttributeSpaceId);
            H5T.close(AttributeTypeId);
        }

        /// <summary>
        ///     Create new variable in file or group with existing data.
        /// </summary>
        /// <param name="_Name"></param>
        /// <param name="FileOrGroupId"></param>
        /// <param name="_Dims"></param>
        /// <param name="data"></param>
        protected internal MatVar(string _Name, H5FileOrGroupId FileOrGroupId, long[] _Dims, T[] data) :
            this(_Name, FileOrGroupId, _Dims)
        {
            Write(data, new long[Dims.Length], Dims);
        }

        /// <summary>
        ///     Variable already exists in file or group.
        /// </summary>
        /// <param name="_Name"></param>
        /// <param name="_FileOrGroupId"></param>
        protected internal MatVar(string _Name, H5FileOrGroupId _FileOrGroupId)
        {
            InitTypeId(out var MatlabClass);

            Name = _Name;
            FileOrGroupId = _FileOrGroupId;

            Closed = true;
            Open();
        }

        public long[] Dims
        {
            get => _Dims;
            set
            {
                _Dims = value;
                Length = 1;
                foreach (var N in _Dims) Length *= N;
            }
        }

        public long[] Cursor { get; set; }

        public long Length { get; private set; }

        protected void InitTypeId(out string MatlabClass)
        {
            if (typeof(T) == typeof(int))
            {
                TypeId = H5T.copy(H5T.H5Type.STD_I32LE);
                MatlabClass = "int32";
            }
            else if (typeof(T) == typeof(double))
            {
                TypeId = H5T.copy(H5T.H5Type.IEEE_F64LE);
                MatlabClass = "double";
            }
            else
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        ///     Write data along one dimension of the array using the cursor.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dim"></param>
        public void WriteNext(T[] data, long dim)
        {
            var count = Enumerable.Repeat(1L, Dims.Length).ToArray(); // Ones.
            for (var i = 0; i < Dims.Length; i++)
                if (i != dim)
                    count[i] = Dims[i];

            long RequiredLength = 1;
            foreach (var l in count) RequiredLength *= l;

            if (data.Length != RequiredLength)
                throw new ArgumentException("Data size must match array dimension");

            var start = new long[Dims.Length];
            start[dim] = Cursor[dim];

            Write(data, start, count);

            Cursor[dim]++;
            for (var i = 0; i < Cursor.Length; i++)
                if (i != dim)
                    Cursor[i] = 0;
        }

        /// <summary>
        ///     Write data into the variable's HDF5 data set in row-major order.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public void Write(T[] data, long[] start, long[] count)
        {
            H5S.selectHyperslab(SpaceId, H5S.SelectOperator.SET, start, count);
            var memSpaceId = H5S.create_simple(count.Length, count);
            var propListId = H5P.create(H5P.PropertyListClass.DATASET_XFER);
            H5D.write(DataSetId, TypeId, memSpaceId, SpaceId, propListId, new H5Array<T>(data));
            H5S.close(memSpaceId);
        }

        /// <summary>
        ///     Write single value in the HDF5 data set.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="position"></param>
        public void Write(T data, long[] position)
        {
            H5S.selectHyperslab(SpaceId, H5S.SelectOperator.SET, position, new long[] { 1, 1 });
            var propListId = H5P.create(H5P.PropertyListClass.DATASET_XFER);
            var memSpaceId = H5S.create_simple(1, new long[] { 1 });
            H5D.writeScalar(DataSetId, TypeId, memSpaceId, SpaceId, propListId, ref data);
            H5S.close(memSpaceId);
        }

        /// <summary>
        ///     Read data from the HDF5 data set directly into an array.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public void Read(T[] buffer, long[] start, long[] count)
        {
            long RequiredLength = 1;
            foreach (var l in count) RequiredLength *= l;

            if (buffer.Length != RequiredLength)
                throw new ArgumentException("Buffer and data set must have same length");
            ReadH5(new H5Array<T>(buffer), start, count);
        }

        /// <summary>
        ///     Read data from HDF5 data set directly into 2D array.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public void Read(T[,] buffer, long[] start, long[] count)
        {
            if (buffer.Rank != Dims.Length) throw new ArgumentException("Buffer and data set must have same rank");
            long RequiredLength = 1;
            foreach (var l in count) RequiredLength *= l;

            if (buffer.Length != RequiredLength)
                throw new ArgumentException("Buffer and data set must have same length");
            ReadH5(new H5Array<T>(buffer), start, count);
        }

        /// <summary>
        ///     Read data from HDF5 data set into H5Array.
        /// </summary>
        /// <param name="wrappedBuffer"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public void ReadH5(H5Array<T> wrappedBuffer, long[] start, long[] count)
        {
            H5S.selectHyperslab(SpaceId, H5S.SelectOperator.SET, start, count);
            var memSpaceId = H5S.create_simple(count.Length, count);
            var propListId = H5P.create(H5P.PropertyListClass.DATASET_XFER);
            H5D.read(DataSetId, TypeId, memSpaceId, SpaceId, propListId, wrappedBuffer);
            H5S.close(memSpaceId);
        }

        public override void Close()
        {
            if (!Closed)
            {
                H5D.close(DataSetId);
                H5S.close(SpaceId);
                H5T.close(TypeId);
                Closed = true;
            }
        }

        public override void Open()
        {
            if (Disposed) throw new ObjectDisposedException("Variable " + Name + " disposed");
            if (Closed)
            {
                DataSetId = H5D.open(FileOrGroupId, "/" + Name);
                SpaceId = H5D.getSpace(DataSetId);
                TypeId = H5D.getType(DataSetId);
                Dims = H5S.getSimpleExtentDims(SpaceId);
                Cursor = new long[Dims.Length];
                Closed = false;
            }
        }
    }
}