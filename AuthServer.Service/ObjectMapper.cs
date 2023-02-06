using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service
{
    public static class ObjectMapper
    {
        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
        {
            var config= new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DTOMapper>();
            });

            return config.CreateMapper();
        });
        //encapsulation to call the "lazy" method from other classes.
        //Lazy methods does not take a place in ram when project started
        //they only work when they are called
        public static IMapper Mapper => lazy.Value;
    }
}
