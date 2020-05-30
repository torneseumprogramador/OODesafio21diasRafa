using System.Collections.Generic;

namespace ORM
{
    public abstract class CType : IType
    {   
        //public abstract string ConnectionString { get; }

        public virtual bool Insert()
        {
            return new Service(this).Insert();
        }

        public virtual bool Update()
        {
            return new Service(this).Update();
        }

        public virtual bool Delete()
        {
            return new Service(this).Delete();
        }

        public virtual void Destroy()
        {
            new Service(this).Destroy();
        }

        public virtual void Get()
        {
            new Service(this).Get();
        }

        public virtual List<IType> GetAll<IType>() where IType : class
        {
            return new Service(this).GetAll<IType>();
        }

        public virtual List<IType> GetById<IType>(IType item) where IType : class
        {
            return new Service(this).GetById(item);
        }

        public virtual List<IType> GetByFilter<IType>(IType item, bool like) where IType : class
        {
            return new Service(this).GetByFilter(item, like);
        }
    }
}
