using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using EhrgoHealth.Data;
using Fitbit;

namespace EhrgoHealth.Web.App_Start
{
    public class AutoMapperConfig
    {
        public static void ConfigureMap()
        {
            Mapper.Initialize(a =>
            {
                a.CreateMap<NutritionalValues, Fitbit.Models.NutritionalValues>();
                a.CreateMap<Fitbit.Models.NutritionalValues, NutritionalValues>();
            });
        }
    }
}