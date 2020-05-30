using System.Collections.Generic;

namespace ORM
{
    public interface IType
    {
        bool Insert();
        bool Update();
        bool Delete();
        void Destroy();
        void Get();
        List<IType> GetAll<IType>() where IType : class;
        List<IType> GetById<IType>(IType item) where IType : class;
        List<IType> GetByFilter<IType>(IType item, bool like) where IType : class;
    }
}
