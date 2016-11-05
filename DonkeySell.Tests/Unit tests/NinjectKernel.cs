using DonkeySellApi.Extra;
using DonkeySellApi.Workers;
using Ninject;

namespace DonkeySell.Tests.Unit_tests
{
    public class NinjectKernel
    {
        public IKernel kernel;

        public NinjectKernel()
        {
            kernel = new StandardKernel();
            kernel.Bind<ICrudOnUsers>().To<CrudOnUsers>();
            kernel.Bind<ICrudOnMessages>().To<CrudOnMessages>();
            kernel.Bind<ICrudOnProducts>().To<CrudOnProducts>();
            kernel.Bind<IMyQueryBuilder>().To<MyQueryBuilder>();
            kernel.Bind<IGetCitiesAndCategories>().To<GetCitiesAndCategories>();
            kernel.Bind<ICrudOnFavorites>().To<CrudOnFavorites>();
        }
    }
}
