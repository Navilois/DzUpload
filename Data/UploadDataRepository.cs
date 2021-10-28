
using DotNetNuke.Data;
using DotNetNuke.Framework;
using System.Collections.Generic;
using WireMayr.Modules.DzUpload.Models;

namespace WireMayr.Modules.DzUpload.Data
{
    public interface IUploadDataRepository
    {
        void CreateItem(UploadData t);
        void DeleteItem(int itemId, int moduleId);
        void DeleteItem(UploadData t);
        IEnumerable<UploadData> GetItems(int moduleId);
        IEnumerable<UploadData> GetItems(string guid, int moduleId);
        UploadData GetItem(int itemId, int moduleId);
        void UpdateItem(UploadData t);
    }

    public interface IUploadFilesRepository
    {
        void CreateItem(UploadFile t);
        void DeleteItem(int itemId);
        void DeleteItem(UploadFile t);
        IEnumerable<UploadFile> GetItems(int uploadId);
        UploadFile GetItem(int itemId);
        void UpdateItem(UploadFile t);
    }

    public class UploadDataRepository : ServiceLocator<IUploadDataRepository, UploadDataRepository>, IUploadDataRepository
    {
        public void CreateItem(UploadData t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<UploadData>();
                rep.Insert(t);
            }
        }

        public void DeleteItem(int itemId, int moduleId)
        {
            var t = GetItem(itemId, moduleId);
            DeleteItem(t);
        }

        public void DeleteItem(UploadData t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<UploadData>();
                rep.Delete(t);
            }
        }

        public IEnumerable<UploadData> GetItems(int moduleId)
        {
            IEnumerable<UploadData> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<UploadData>();
                t = rep.Get(moduleId);
            }
            return t;
        }

        public IEnumerable<UploadData> GetItems(string guid, int moduleId)
        {
            IEnumerable<UploadData> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<UploadData>();
                t = rep.Find("WHERE GUID = @0 and ModuleId = @1", guid, moduleId);
            }
            return t;
        }

        public UploadData GetItem(int itemId, int moduleId)
        {
            UploadData t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<UploadData>();
                t = rep.GetById(itemId, moduleId);
            }
            return t;
        }

        public void UpdateItem(UploadData t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<UploadData>();
                rep.Update(t);
            }
        }

        protected override System.Func<IUploadDataRepository> GetFactory()
        {
            return () => new UploadDataRepository();
        }
    }



    public class UploadFilesRepository : ServiceLocator<IUploadFilesRepository, UploadFilesRepository>, IUploadFilesRepository
    {
        public void CreateItem(UploadFile t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<UploadFile>();
                rep.Insert(t);
            }
        }

        public void DeleteItem(int itemId)
        {
            var t = GetItem(itemId);
            DeleteItem(t);
        }

        public void DeleteItem(UploadFile t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<UploadFile>();
                rep.Delete(t);
            }
        }

        public IEnumerable<UploadFile> GetItems(int uploadId)
        {
            IEnumerable<UploadFile> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<UploadFile>();
                t = rep.Get(uploadId);
            }
            return t;
        }

        public UploadFile GetItem(int itemId)
        {
            UploadFile t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<UploadFile>();
                t = rep.GetById(itemId);
            }
            return t;
        }

        public void UpdateItem(UploadFile t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<UploadFile>();
                rep.Update(t);
            }
        }

        protected override System.Func<IUploadFilesRepository> GetFactory()
        {
            return () => new UploadFilesRepository();
        }
    }
}
